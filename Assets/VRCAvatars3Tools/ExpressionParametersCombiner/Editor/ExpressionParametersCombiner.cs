using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

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

        private int copiedTotalCost = 0;
        private int totalCost = 0;

        private bool containsSameNameParameter = false;

        private Vector2 srcParametersScrollPos = Vector2.zero;
        private Vector2 dstParametersScrollPos = Vector2.zero;

        [MenuItem("VRCAvatars3Tools/ExpressionParametersCombiner")]
        public static void Open()
        {
            GetWindow<ExpressionParametersCombiner>("ExpressionParametersCombiner");
        }

        public void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                srcParameters = EditorGUILayout.ObjectField("Src ExpressionParameters", srcParameters, typeof(VRCExpressionParameters), true) as VRCExpressionParameters;
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
                    var selectedTotalCost = CaluculateSelectedTotalCost(srcParameters, isCopyParameters);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Selected Total Cost: {selectedTotalCost}");
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
                            EditorGUILayout.LabelField("Not found parameters");
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Copy ↓", GUILayout.Width(60f));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space();

            dstParameters = EditorGUILayout.ObjectField("Dst ExpressionParameters", dstParameters, typeof(VRCExpressionParameters), true) as VRCExpressionParameters;
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
                        EditorGUILayout.LabelField($"Total Cost: {totalCost} -> {totalCost + copiedTotalCost} / {MAX_TOTAL_COST}");
                    }
                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach (var parameter in dstParameters.parameters)
                        {
                            EditorGUILayout.LabelField($"    [{parameter.valueType}] {parameter.name}");
                        }
                        var dstParameterNames = dstParameters.parameters.Select(p => p.name).ToArray();
                        containsSameNameParameter = false;
                        foreach (var copiedParameter in copiedParameters)
                        {
                            EditorGUILayout.LabelField($"+ [{copiedParameter.valueType}] {copiedParameter.name}");
                        }
                        if (!dstParameters.parameters.Any() && !copiedParameters.Any())
                        {
                            EditorGUILayout.LabelField("Not found parameters");
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            if (containsSameNameParameter)
            {
                EditorGUILayout.HelpBox("Contains same name in selected parameter. These parameter is not copied.", MessageType.Warning);
            }

            using (new EditorGUI.DisabledGroupScope(!CanCombine()))
            {
                if (GUILayout.Button("Combine"))
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
    }
}
