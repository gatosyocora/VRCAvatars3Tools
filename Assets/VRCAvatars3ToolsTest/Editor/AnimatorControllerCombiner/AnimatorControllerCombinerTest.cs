using Gatosyocora.VRCAvatars3Tools;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorControllerCombinerTest
{
    [Test]
    public void CombineAnimatorControllers()
    {
        var folderPath = "Assets/VRCAvatars3ToolsTest/Editor/AnimatorControllerCombiner/";
        var srcController = AssetDatabase.LoadAssetAtPath(folderPath + "SrcController.controller", typeof(AnimatorController)) as AnimatorController;
        var dstControllerPath = folderPath + "DstController.controller";
        AssetDatabase.CreateAsset(new AnimatorController(), dstControllerPath);
        var dstController = AssetDatabase.LoadAssetAtPath(dstControllerPath, typeof(AnimatorController)) as AnimatorController;

        var isCopyLayers = srcController.layers.Select(_ => true).ToArray();
        var isCopyParameters = srcController.parameters.Select(_ => true).ToArray();
        AnimatorControllerCombiner.CombineAnimatorControllers(srcController, dstController, isCopyLayers, isCopyParameters);

        //Object.DestroyImmediate(dstController);
    }
}
