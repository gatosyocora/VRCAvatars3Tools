using System.IO;
using UnityEditor;
using Gatosyocora.VRCAvatars3Tools.Utilitys;
using UnityEditor.Animations;

// ver 1.1
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAssetCreator : Editor
    {
        [MenuItem("Assets/Create/VRChat/Controllers/ActionLayer", priority = 4)]
        public static AnimatorController CreateActionLayer()
             => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3ActionLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/FaceLayer", priority = 4)]
        public static AnimatorController CreateFaceLayer()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3FaceLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer", priority = 5)]
        public static AnimatorController CreateHandsLayer()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3HandsLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer2", priority = 5)]
        public static AnimatorController CreateHandsLayer2()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3HandsLayer2");

        [MenuItem("Assets/Create/VRChat/Controllers/IdleLayer", priority = 2)]
        public static AnimatorController CreateIdleLayer()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3IdleLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/LocomotionLayer", priority = 1)]
        public static AnimatorController CreateLocomotionLayer()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3LocomotionLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer", priority = 16)]
        public static AnimatorController CreateSittingLayer()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3SittingLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer2", priority = 16)]
        public static AnimatorController CreateSittingLayer2()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3SittingLayer2");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityIKPose", priority = 18)]
        public static AnimatorController CreateUtilityIKPose()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3UtilityIKPose");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityTPose", priority = 17)]
        public static AnimatorController CreateUtilityTPose()
            => DuplicateVRCAsset<AnimatorController>("vrc_AvatarV3UtilityTPose");

        [MenuItem("Assets/Create/VRChat/BlendTrees/New BlendTree", priority = 0)]
        public static BlendTree CreateNewBlendTree()
            => CreateNewBlendTree("new BlendTree");

        [MenuItem("Assets/Create/VRChat/BlendTrees/StandingLocomotion", priority = 1)]
        public static BlendTree CreateStandingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("vrc_StandingLocomotion");

        [MenuItem("Assets/Create/VRChat/BlendTrees/CrouchingLocomotion", priority = 2)]
        public static BlendTree CreateCrouchingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("vrc_CrouchingLocomotion");

        [MenuItem("Assets/Create/VRChat/BlendTrees/ProneLocomotion", priority = 3)]
        public static BlendTree CreateProneLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("vrc_ProneLocomotion");

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

        private static T DuplicateVRCAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var baseAssetPath = VRCAssetUtility.GetVRCAssetPathForSearch($"{assetName} t:{typeof(T).Name}");

            var folder = Selection.activeObject;
            var folderPath = AssetDatabase.GetAssetPath(folder);
            var extention = Path.GetExtension(baseAssetPath);
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, $"{assetName}{extention}"));
            AssetDatabase.CopyAsset(baseAssetPath, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            Selection.activeObject = asset;
            return asset;
        }
    }
}


