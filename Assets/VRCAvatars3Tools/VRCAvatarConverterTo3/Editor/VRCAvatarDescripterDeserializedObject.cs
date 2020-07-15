using UnityEngine;
using VRC.SDKBase;

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
    }
}