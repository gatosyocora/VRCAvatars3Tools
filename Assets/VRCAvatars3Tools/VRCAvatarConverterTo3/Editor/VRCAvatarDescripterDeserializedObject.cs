using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

// ver 1.0
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAvatarDescripterDeserializedObject
    {
        public string Name { get; set; }

        // View
        public Vector3 ViewPosition { get; set; }
        public bool ScaleIPD { get; set; }

        public enum AnimationSet
        {
            Male = 0,
            Female = 1,
            None = 2
        }
        public AnimationSet DefaultAnimationSet { get; set; }

        // LipSync
        public VRC_AvatarDescriptor.LipSyncStyle lipSync { get; set; }
        public string faceMeshRendererPath { get; set; }
        public string[] VisemeBlendShapes { get; set; }

        // AnimationLayers
        public string standingOverrideControllerPath { get; set; }
        public AnimationClipInfo[] OverrideAnimationClips { get; set; }
    }

    public class AnimationClipInfo
    {
        public string Type { get; set; }
        public string Path { get; set; }
    }
}