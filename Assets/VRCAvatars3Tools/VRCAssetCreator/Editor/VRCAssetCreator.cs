using System.IO;
using UnityEditor;
using Gatosyocora.VRCAvatars3Tools.Utilitys;
using UnityEditor.Animations;

// ver 1.0.1
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAssetCreator : Editor
    {
        [MenuItem("Assets/Create/VRChat/Controllers/ActionLayer", priority = 4)]
        public static AnimatorController CreateActionLayer()
             => CreateVRCController("vrc_AvatarV3ActionLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/FaceLayer", priority = 4)]
        public static AnimatorController CreateFaceLayer()
            => CreateVRCController("vrc_AvatarV3FaceLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer", priority = 5)]
        public static AnimatorController CreateHandsLayer()
            => CreateVRCController("vrc_AvatarV3HandsLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/HandsLayer2", priority = 5)]
        public static AnimatorController CreateHandsLayer2()
            => CreateVRCController("vrc_AvatarV3HandsLayer2");

        [MenuItem("Assets/Create/VRChat/Controllers/IdleLayer", priority = 2)]
        public static AnimatorController CreateIdleLayer()
            => CreateVRCController("vrc_AvatarV3IdleLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/LocomotionLayer", priority = 1)]
        public static AnimatorController CreateLocomotionLayer()
            => CreateVRCController("vrc_AvatarV3LocomotionLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer", priority = 16)]
        public static AnimatorController CreateSittingLayer()
            => CreateVRCController("vrc_AvatarV3SittingLayer");

        [MenuItem("Assets/Create/VRChat/Controllers/SittingLayer2", priority = 16)]
        public static AnimatorController CreateSittingLayer2()
            => CreateVRCController("vrc_AvatarV3SittingLayer2");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityIKPose", priority = 18)]
        public static AnimatorController CreateUtilityIKPose()
            => CreateVRCController("vrc_AvatarV3UtilityIKPose");

        [MenuItem("Assets/Create/VRChat/Controllers/UtilityTPose", priority = 17)]
        public static AnimatorController CreateUtilityTPose()
            => CreateVRCController("vrc_AvatarV3UtilityTPose");

        private static AnimatorController CreateVRCController(string controllerName)
        {
            var controllerPath = VRCAssetUtility.GetVRCAssetPathForSearch($"{controllerName} t:AnimatorController");

            var folder = Selection.activeObject;
            var folderPath = AssetDatabase.GetAssetPath(folder);
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, $"{controllerName}.controller"));
            AssetDatabase.CopyAsset(controllerPath, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            Selection.activeObject = controller;
            return controller;
        }
    }
}


