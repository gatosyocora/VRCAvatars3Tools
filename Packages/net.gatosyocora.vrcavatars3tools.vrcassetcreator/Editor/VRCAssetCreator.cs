using System.IO;
using UnityEditor;
using Gatosyocora.VRCAvatars3Tools.Utilitys;
using UnityEditor.Animations;

// ver 1.1.1
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAssetCreator : Editor
    {
        [MenuItem("Assets/Create/VRChat/Controllers/ActionLayer", priority = 4)]
        public static AnimatorController CreateActionLayer()
             => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3ActionLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/FaceLayer", priority = 4)]
        public static AnimatorController CreateFaceLayer()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3FaceLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer", priority = 5)]
        public static AnimatorController CreateHandsLayer()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3HandsLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer2", priority = 5)]
        public static AnimatorController CreateHandsLayer2()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3HandsLayer2.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/IdleLayer", priority = 2)]
        public static AnimatorController CreateIdleLayer()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3IdleLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/LocomotionLayer", priority = 1)]
        public static AnimatorController CreateLocomotionLayer()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3LocomotionLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer", priority = 16)]
        public static AnimatorController CreateSittingLayer()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3SittingLayer.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer2", priority = 16)]
        public static AnimatorController CreateSittingLayer2()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3SittingLayer2.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityIKPose", priority = 18)]
        public static AnimatorController CreateUtilityIKPose()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3UtilityIKPose.controller");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityTPose", priority = 17)]
        public static AnimatorController CreateUtilityTPose()
            => DuplicateVRCAsset<AnimatorController>("Controllers/vrc_AvatarV3UtilityTPose.controller");

        [MenuItem("Assets/Create/VRChat/BlendTrees/New BlendTree", priority = 0)]
        public static BlendTree CreateNewBlendTree()
            => CreateNewBlendTree("new BlendTree");

        [MenuItem("Assets/Create/VRChat/BlendTrees/StandingLocomotion", priority = 1)]
        public static BlendTree CreateStandingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("BlendTrees/vrc_StandingLocomotion.asset");

        [MenuItem("Assets/Create/VRChat/BlendTrees/CrouchingLocomotion", priority = 2)]
        public static BlendTree CreateCrouchingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("BlendTrees/vrc_CrouchingLocomotion.asset");

        [MenuItem("Assets/Create/VRChat/BlendTrees/ProneLocomotion", priority = 3)]
        public static BlendTree CreateProneLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("BlendTrees/vrc_ProneLocomotion.asset");

        private static BlendTree CreateNewBlendTree(string assetName)
        {
            var folder = Selection.activeObject;
            var folderPath = AssetDatabase.GetAssetPath(folder);
            var extention = ".asset";
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, $"{assetName}{extention}"));

            var asset = new BlendTree 
            {
                name = Path.GetFileNameWithoutExtension(assetPath)
            };
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
            return asset;
        }

        private static T DuplicateVRCAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            var baseAssetPath = Path.Combine(VRCConsts.ANIMATION_ASSET_FOLDER_PATH, assetPath);

            var folder = Selection.activeObject;
            var folderPath = AssetDatabase.GetAssetPath(folder);
            var assetName = Path.GetFileName(assetPath);
            var assetFullPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, assetName));
            AssetDatabase.CopyAsset(baseAssetPath, assetFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
            Selection.activeObject = asset;
            return asset;
        }
    }
}


