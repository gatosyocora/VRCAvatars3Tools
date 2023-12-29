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
             => DuplicateVRCAsset<AnimatorController>("3e479eeb9db24704a828bffb15406520");

        [MenuItem("Assets/Create/VRChat/Controllers/FaceLayer", priority = 4)]
        public static AnimatorController CreateFaceLayer()
            => DuplicateVRCAsset<AnimatorController>("d40be620cf6c698439a2f0a5144919fe");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer", priority = 5)]
        public static AnimatorController CreateHandsLayer()
            => DuplicateVRCAsset<AnimatorController>("404d228aeae421f4590305bc4cdaba16");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer2", priority = 5)]
        public static AnimatorController CreateHandsLayer2()
            => DuplicateVRCAsset<AnimatorController>("5ecf8b95a27552840aef66909bdf588f");

        [MenuItem("Assets/Create/VRChat/Controllers/IdleLayer", priority = 2)]
        public static AnimatorController CreateIdleLayer()
            => DuplicateVRCAsset<AnimatorController>("573a1373059632b4d820876efe2d277f");

        [MenuItem("Assets/Create/VRChat/Controllers/LocomotionLayer", priority = 1)]
        public static AnimatorController CreateLocomotionLayer()
            => DuplicateVRCAsset<AnimatorController>("4e4e1a372a526074884b7311d6fc686b");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer", priority = 16)]
        public static AnimatorController CreateSittingLayer()
            => DuplicateVRCAsset<AnimatorController>("1268460c14f873240981bf15aa88b21a");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer2", priority = 16)]
        public static AnimatorController CreateSittingLayer2()
            => DuplicateVRCAsset<AnimatorController>("74c2e15937e5c95478edd251f20e126f");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityIKPose", priority = 18)]
        public static AnimatorController CreateUtilityIKPose()
            => DuplicateVRCAsset<AnimatorController>("a9b90a833b3486e4b82834c9d1f7c4ee");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityTPose", priority = 17)]
        public static AnimatorController CreateUtilityTPose()
            => DuplicateVRCAsset<AnimatorController>("00121b5812372b74f9012473856d8acf");

        [MenuItem("Assets/Create/VRChat/BlendTrees/New BlendTree", priority = 0)]
        public static BlendTree CreateNewBlendTree()
            => CreateNewBlendTree("new BlendTree");

        [MenuItem("Assets/Create/VRChat/BlendTrees/StandingLocomotion", priority = 1)]
        public static BlendTree CreateStandingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("b7ff0bc6ae31ce4458992fa6ce9f6897");

        [MenuItem("Assets/Create/VRChat/BlendTrees/CrouchingLocomotion", priority = 2)]
        public static BlendTree CreateCrouchingLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("1fe93258fe621c344be8713451c5104f");

        [MenuItem("Assets/Create/VRChat/BlendTrees/ProneLocomotion", priority = 3)]
        public static BlendTree CreateProneLocomotionBlendTree()
            => DuplicateVRCAsset<BlendTree>("667633d86ecc9c0408e81156d77d9a83");

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

        private static T DuplicateVRCAsset<T>(string guid) where T : UnityEngine.Object
        {
            var baseAssetPath = AssetDatabase.GUIDToAssetPath(guid);
            var assetName = Path.GetFileNameWithoutExtension(baseAssetPath);

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


