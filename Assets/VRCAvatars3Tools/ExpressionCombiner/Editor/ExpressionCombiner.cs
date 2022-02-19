using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Gatosyocora.VRCAvatars3Tools
{
    public class ExpressionCombiner : EditorWindow
    {
        private VRCExpressionParameters srcParameters;
        private bool[] isCopyParameters;

        private Vector2 srcControllerScrollPos = Vector2.zero;

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
                using (var scroll = new EditorGUILayout.ScrollViewScope(srcControllerScrollPos, new GUIStyle(), new GUIStyle("verticalScrollbar")))
                {
                    srcControllerScrollPos = scroll.scrollPosition;
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
        }
    }
}
