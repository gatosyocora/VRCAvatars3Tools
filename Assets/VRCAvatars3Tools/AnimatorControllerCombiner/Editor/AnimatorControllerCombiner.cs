using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Parameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;
using System.Collections.Generic;
using Gatosyocora.VRCAvatars3Tools.Utilitys;

// ver 1.0.3.2
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
            if (srcController != null)
            {
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
                        foreach (var layer in srcController.layers)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.ToggleLeft(layer.name, true);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        foreach (var parameter in srcController.parameters)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.ToggleLeft(parameter.name, true);
                            }
                        }
                    }
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

            dstController = EditorGUILayout.ObjectField("Dst AnimatorController", dstController, typeof(AnimatorController), true) as AnimatorController;
            if (dstController != null)
            {
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
                        foreach (var layer in dstController.layers)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(layer.name);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        foreach (var parameter in dstController.parameters)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(parameter.name);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();

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