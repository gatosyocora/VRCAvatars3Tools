using Boo.Lang;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ver 1.0
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class AnimationBindingSeparater : Editor
    {
        [MenuItem("CONTEXT/Motion/Separate the binding that changes Transform", false, 0)]
        public static void SeparateBindingsThatChangesTransform(MenuCommand command)
        {
            var animationClip = command.context as AnimationClip;
            var transformClip = new AnimationClip();

            var humanBodyBoneNames = Enum.GetNames(typeof(HumanBodyBones))
                                        .SelectMany(n => new string[] { n, ToContainSpace(n) })
                                        .Distinct()
                                        .ToArray();

            foreach (var name in humanBodyBoneNames)
            {
                Debug.Log(name);
            }

            bool isSeparate = false;
            foreach (var binding in AnimationUtility.GetCurveBindings(animationClip).ToArray())
            {
                if (binding.type == typeof(Transform) ||
                    (binding.type == typeof(Animator) && humanBodyBoneNames
                                                            .Any(n => binding.propertyName.StartsWith(n))))
                {
                    var curve = AnimationUtility.GetEditorCurve(animationClip, binding);

                    // 元から削除
                    AnimationUtility.SetEditorCurve(animationClip, binding, null);

                    // Transform用のAnimationClipに追加
                    AnimationUtility.SetEditorCurve(transformClip, binding, curve);

                    isSeparate = true;
                }
            }

            EditorUtility.SetDirty(animationClip);
            EditorUtility.SetDirty(transformClip);

            if (isSeparate)
            {
                var animationClipPath = AssetDatabase.GetAssetPath(animationClip);
                var transformClipPath = AssetDatabase.GenerateUniqueAssetPath(
                                            Path.Combine(Path.GetDirectoryName(animationClipPath),
                                            $"{Path.GetFileNameWithoutExtension(animationClipPath)}_Transform.anim"));
                AssetDatabase.CreateAsset(transformClip, transformClipPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string ToContainSpace(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            int startIndex = 0, count = 1;
            // 最初が小文字の可能性があるため+1
            List<string> words = new List<string>();
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    words.Add(input.Substring(startIndex, count));
                    startIndex = i;
                    count = 1;
                }
                else
                {
                    count++;
                }
            }
            words.Add(input.Substring(startIndex, count));

            return string.Join(" ", words);
        }
    }
}