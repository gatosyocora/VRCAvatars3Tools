using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Gatosyocora.VRCAvatars3Tools
{
    public class ExpressionCombiner : EditorWindow
    {
        private VRCExpressionParameters srcParameters;

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
                }
            }
        }
    }
}
