using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

// ver 1.0
// Copyright (c) 2022 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class ExpressionParametersCombiner : EditorWindow
    {
        private const int MAX_TOTAL_COST = 128;
        private const int COST_INT = 8;
        private const int COST_FLOAT = 8;
        private const int COST_BOOL = 1;

        private VRCExpressionParameters srcParameters;
        private VRCExpressionParameters dstParameters;
        private bool[] isCopyParameters;

        private int selectedTotalCost = 0;
        private int copiedTotalCost = 0;
        private int totalCost = 0;

        private Vector2 srcParametersScrollPos = Vector2.zero;
        private Vector2 dstParametersScrollPos = Vector2.zero;

        // 簡易的な日本語対応
        private readonly static string[] textEN = new string[]
        {
            "Src ExpressionParameters",
            "Selected Total Cost",
            "Not found parameters",
            "Copy",
            "Dst ExpressionParameters",
            "Total Cost",
            "Contains same name in selected parameter. These parameter is not copied.",
            "Combine",
            "The cost is over the limit."
        };

        private readonly static string[] textJA = new string[]
        {
            "コピー元のExpressionParameters",
            "選択中の総コスト",
            "パラメータが見つかりませんでした",
            "コピー",
            "コピー先のExpressionParameters",
            "総コスト",
            "選択中のものにコピー先と同じ名前のパラメータが含まれています。このパラメータはコピーされません",
            "コピー開始",
            "コストが上限を超えています"
        };

        private string[] texts = textEN;

        [MenuItem("VRCAvatars3Tools/ExpressionParametersCombiner")]
        public static void Open()
        {
            GetWindow<ExpressionParametersCombiner>("ExpressionParametersCombiner");
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("EN"))
                {
                    texts = textEN;
                }
                if (GUILayout.Button("JA"))
                {
                    texts = textJA;
                }
            }
            EditorGUILayout.Space();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                srcParameters = EditorGUILayout.ObjectField($"{texts[0]}", srcParameters, typeof(VRCExpressionParameters), true) as VRCExpressionParameters;
                if (check.changed)
                {
                    if (srcParameters != null)
                    {
                        isCopyParameters = srcParameters.parameters
                                            .Select(_ => true)
                                            .ToArray();
                    }
                }
            }
            if (srcParameters != null)
            {
                using (new EditorGUI.IndentLevelScope())
                using (var scroll = new EditorGUILayout.ScrollViewScope(srcParametersScrollPos, new GUIStyle(), new GUIStyle("verticalScrollbar")))
                {
                    srcParametersScrollPos = scroll.scrollPosition;
                    selectedTotalCost = CaluculateSelectedTotalCost(srcParameters, isCopyParameters);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"{texts[1]}: {selectedTotalCost}");
                    }
                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (int i = 0; i < srcParameters.parameters.Length; i++)
                        {
                            var parameter = srcParameters.parameters[i];
                            isCopyParameters[i] = EditorGUILayout.ToggleLeft($"[{parameter.valueType}] {parameter.name}", isCopyParameters[i]);
                        }
                        if (!srcParameters.parameters.Any())
                        {
                            EditorGUILayout.LabelField($"{texts[2]}");
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField($"{texts[3]} ↓", GUILayout.Width(60f));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space();

            dstParameters = EditorGUILayout.ObjectField($"{texts[4]}", dstParameters, typeof(VRCExpressionParameters), true) as VRCExpressionParameters;
            if (dstParameters != null)
            {
                var copiedParameters = GetCopiedParameters();

                using (new EditorGUI.IndentLevelScope())
                using (var scroll = new EditorGUILayout.ScrollViewScope(dstParametersScrollPos, new GUIStyle(), new GUIStyle("verticalScrollbar")))
                {
                    dstParametersScrollPos = scroll.scrollPosition;
                    totalCost = dstParameters.CalcTotalCost();
                    copiedTotalCost = CaluculateTotalCost(copiedParameters);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"{texts[5]}: {totalCost} -> {totalCost + copiedTotalCost} / {MAX_TOTAL_COST}");
                    }
                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach (var parameter in dstParameters.parameters)
                        {
                            EditorGUILayout.LabelField($"    [{parameter.valueType}] {parameter.name}");
                        }
                        var dstParameterNames = dstParameters.parameters.Select(p => p.name).ToArray();
                        foreach (var copiedParameter in copiedParameters)
                        {
                            EditorGUILayout.LabelField($"+ [{copiedParameter.valueType}] {copiedParameter.name}");
                        }
                        if (!dstParameters.parameters.Any() && !copiedParameters.Any())
                        {
                            EditorGUILayout.LabelField($"{texts[2]}");
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            if (ContainsSameNameParameter())
            {
                EditorGUILayout.HelpBox($"{texts[6]}", MessageType.Warning);
            }

            if (IsOverCost())
            {
                EditorGUILayout.HelpBox($"{texts[8]}", MessageType.Error);
            }

            using (new EditorGUI.DisabledGroupScope(!CanCombine()))
            {
                if (GUILayout.Button($"{texts[7]}"))
                {
                    Combine(GetCopiedParameters());
                }
            }

            EditorGUILayout.Space();
        }

        private void Combine(VRCExpressionParameters.Parameter[] copiedParameters)
        {
            if (copiedParameters == null || !copiedParameters.Any()) return;

            dstParameters.parameters = dstParameters.parameters
                                        .Union(copiedParameters)
                                        .ToArray();
            EditorUtility.SetDirty(dstParameters);
            AssetDatabase.SaveAssets();
        }

        private bool CanCombine()
        {
            return srcParameters != null &&
                dstParameters != null &&
                srcParameters.parameters.Any() &&
                !IsOverCost();
        }

        private VRCExpressionParameters.Parameter[] GetCopiedParameters()
        {
            if (srcParameters == null || dstParameters == null)
            {
                return Array.Empty<VRCExpressionParameters.Parameter>();
            }

            var dstParameterNames = dstParameters.parameters.Select(p => p.name).ToArray();

            return srcParameters.parameters
                    .Where((_, index) => isCopyParameters[index])
                    .Where(p => !dstParameterNames.Contains(p.name))
                    .ToArray();
        }

        private int CaluculateSelectedTotalCost(VRCExpressionParameters parameters, bool[] isSelected)
        {
            if (parameters.parameters.Length != isSelected.Length)
            {
                throw new Exception("no match array size");
            }

            return CaluculateTotalCost(parameters.parameters
                    .Where((_, index) => isSelected[index]).ToArray());
        }

        private int CaluculateTotalCost(VRCExpressionParameters.Parameter[] parameters)
        {
            return parameters
                    .Select(parameter =>
                    {
                        switch (parameter.valueType)
                        {
                            case VRCExpressionParameters.ValueType.Int:
                                return COST_INT;
                            case VRCExpressionParameters.ValueType.Float:
                                return COST_FLOAT;
                            case VRCExpressionParameters.ValueType.Bool:
                                return COST_BOOL;
                            default:
                                throw new Exception("Undefined parameter");
                        }
                    })
                    .Sum();
        }

        private bool IsOverCost() => totalCost + copiedTotalCost > MAX_TOTAL_COST;

        private bool ContainsSameNameParameter() => copiedTotalCost < selectedTotalCost;
    }
}
