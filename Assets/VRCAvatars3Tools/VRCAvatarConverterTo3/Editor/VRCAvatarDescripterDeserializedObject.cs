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

        // LipSync
        public VRC_AvatarDescriptor.LipSyncStyle lipSync { get; set; }
        public string faceMeshRendererPath { get; set; }
        public string[] VisemeBlendShapes { get; set; }

        // AnimationLayers
        public string standingOverrideControllerPath { get; set; }
        public string[] OverrideAnimationClips { get; set; }
    }
}