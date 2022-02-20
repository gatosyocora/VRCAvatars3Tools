using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Gatosyocora.VRCAvatars3Tools
{
    public class ExpressionCombiner : EditorWindow
    {
        private VRCExpressionParameters srcParameters;
        private VRCExpressionParameters dstParameters;
        private bool[] isCopyParameters;

        private Vector2 srcParametersScrollPos = Vector2.zero;
        private Vector2 dstParametersScrollPos = Vector2.zero;

        [MenuItem("VRCAvatars3Tools/ExpressionCombiner")]
        public static void Open()
        {
            GetWindow<ExpressionCombiner>("ExpressionCombiner");
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
                    EditorGUILayout.LabelField("ExpressionParameters", EditorStyles.boldLabel);
                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (int i = 0; i < srcParameters.parameters.Length; i++)
                        {
                            var parameter = srcParameters.parameters[i];
                            isCopyParameters[i] = EditorGUILayout.ToggleLeft($"[{parameter.valueType}]{parameter.name}", isCopyParameters[i]);
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
                using (new EditorGUI.IndentLevelScope())
                using (var scroll = new EditorGUILayout.ScrollViewScope(dstParametersScrollPos, new GUIStyle(), new GUIStyle("verticalScrollbar")))
                {
                    dstParametersScrollPos = scroll.scrollPosition;
                    EditorGUILayout.LabelField("ExpressionParameters", EditorStyles.boldLabel);
                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach (var parameter in dstParameters.parameters)
                        {
                            EditorGUILayout.LabelField($"[{parameter.valueType}]{parameter.name}");
                        }
                        if (!dstParameters.parameters.Any())
                        {
                            EditorGUILayout.LabelField("Not found parameters");
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledGroupScope(!CanCombine()))
            {
                if (GUILayout.Button("Combine"))
                {
                }
            }

            EditorGUILayout.Space();
        }

        private bool CanCombine()
        {
            return srcParameters != null &&
                dstParameters != null &&
                srcParameters.parameters.Any();
        }
    }
}
