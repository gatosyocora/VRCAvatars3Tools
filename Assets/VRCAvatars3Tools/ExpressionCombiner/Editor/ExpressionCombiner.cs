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
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.LabelField("ExpressionParameters", EditorStyles.boldLabel);
                            for (int i = 0; i < srcParameters.parameters.Length; i++)
                            {
                                var parameter = srcParameters.parameters[i];
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    isCopyParameters[i] = EditorGUILayout.ToggleLeft($"[{parameter.valueType}]{parameter.name}", isCopyParameters[i]);
                                }
                            }
                        }
                    }
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUI.IndentLevelScope())
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
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                            foreach (var parameter in dstParameters.parameters)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField($"[{parameter.valueType}]{parameter.name}");
                                }
                            }
                        }
                    }
                    EditorGUILayout.Space();
                }
            }
        }
    }
}
