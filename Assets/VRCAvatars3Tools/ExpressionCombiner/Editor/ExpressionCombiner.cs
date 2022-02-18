using UnityEditor;

namespace Gatosyocora.VRCAvatars3Tools
{
    public class ExpressionCombiner : EditorWindow
    {
        [MenuItem("VRCAvatars3Tools/ExpressionCombiner")]
        public static void Open()
        {
            GetWindow<ExpressionCombiner>("ExpressionCombiner");
        }

        public void OnGUI()
        {
        }
    }
}
