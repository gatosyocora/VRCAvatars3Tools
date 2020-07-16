using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using VRC.SDK3.Components;
using Parameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;
using System.Collections.Generic;
using Gatosyocora.VRCAvatars3Tools.Utilitys;

// ver 1.0.3
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class AnimatorControllerCombiner : EditorWindow
    {
        private AnimatorController srcController;
        private AnimatorController dstController;

        [MenuItem("VRCAvatars3Tools/AnimatorControllerCombiner")]
        public static void Open()
        {
            GetWindow<AnimatorControllerCombiner>("AnimatorControllerCombiner");
        }

        private void OnGUI()
        {
            srcController = EditorGUILayout.ObjectField("Src AnimatorController", srcController, typeof(AnimatorController), true) as AnimatorController;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Copy ↓");
            }
            dstController = EditorGUILayout.ObjectField("Dst AnimatorController", dstController, typeof(AnimatorController), true) as AnimatorController;

            using (new EditorGUI.DisabledGroupScope(!srcController || !dstController))
            {
                if (GUILayout.Button("Combine"))
                {
                    AnimatorControllerUtility.CombineAnimatorController(srcController, dstController);
                }
            }
        }
    }
}