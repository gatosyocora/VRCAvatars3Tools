using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Components;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAvatarConverterTo3 : EditorWindow
    {
        /*
            114210250354209916 : PileLineManager
                - blueprintId
            114962464251023968 : VRCAvatarDescripter
                - VisemeBlendShapes
                - ViewPosition : {x: 0, y: 1.21, z: 0.069}
                - CustomStandingAnims : guid: 156f670b4f8094e488e6e6c82595507a
                - ScaleIPD : 0 or 1
                - VisemeSkinnedMesh : {fileID: 137651239226841522}
        */

        private GameObject avatarPrefab;

        private VRCAvatarDescripterDeserializedObject avatar2Info;

        [MenuItem("VRCAvatars3Tools/VRCAvatarConverterTo3")]
        public static void Open()
        {
            GetWindow<VRCAvatarConverterTo3>(nameof(VRCAvatarConverterTo3));
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                avatarPrefab = EditorGUILayout.ObjectField("2.0 Avatar Prefab", avatarPrefab, typeof(GameObject), false) as GameObject;

                if (check.changed && avatarPrefab != null)
                {
                    avatar2Info = GetAvatar2Info(avatarPrefab);
                }
            }

            if (avatarPrefab != null && avatar2Info != null)
            {
                EditorGUILayout.LabelField("Prefab Name", avatarPrefab.name);

                EditorGUILayout.LabelField("View", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("ViewPosition", avatar2Info.ViewPosition.ToString());
                EditorGUILayout.LabelField("ScaleIPD", avatar2Info.ScaleIPD.ToString());

                EditorGUILayout.LabelField("LipSync", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("FaceMeshPath", avatar2Info.faceMeshRendererPath);

                EditorGUILayout.LabelField("EyeLook", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Eyes.LeftEyeBonePath", "Unimplemented");
                EditorGUILayout.LabelField("Eyes.RightEyeBonePath", "Unimplemented");
                EditorGUILayout.LabelField("Eyelids.BlendShapeStates", "Unimplemented");

                EditorGUILayout.LabelField("AnimationLayers", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("StandingOverrideControllerPath", avatar2Info.standingOverrideControllerPath);
                EditorGUILayout.LabelField("SittingOverrideControllerPath", "Unimplemented");
            }

            using (new EditorGUI.DisabledGroupScope(avatarPrefab is null || avatar2Info is null))
            {
                if (GUILayout.Button("Convert Avatar To 3.0"))
                {
                    ConvertAvatarTo3(avatarPrefab, avatar2Info);
                }
            }
        }

        private GameObject ConvertAvatarTo3(GameObject avatarPrefab2, VRCAvatarDescripterDeserializedObject avatar2Info)
        {
            var avatarObj3 = PrefabUtility.InstantiatePrefab(avatarPrefab2) as GameObject;
            avatarObj3.name += "_3.0";
            var avatar = avatarObj3.AddComponent<VRCAvatarDescriptor>();
            avatar.Name = avatar2Info.Name;
            avatar.ViewPosition = avatar2Info.ViewPosition;
            avatar.ScaleIPD = avatar2Info.ScaleIPD;
            avatar.lipSync = avatar2Info.lipSync;
            avatar.VisemeSkinnedMesh = avatarObj3.transform.Find(avatar2Info.faceMeshRendererPath)?.GetComponent<SkinnedMeshRenderer>() ?? null;
            avatar.VisemeBlendShapes = avatar2Info.VisemeBlendShapes;
            avatar.enableEyeLook = true;
            var customEyeLookSettings = new VRCAvatarDescriptor.CustomEyeLookSettings
            {
                leftEye = avatarObj3.transform.Find("Armature/Hips/Spine/Chest/Neck/Head/LeftEye"),
                rightEye = avatarObj3.transform.Find("Armature/Hips/Spine/Chest/Neck/Head/RightEye"),
                eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes,
                eyelidsSkinnedMesh = avatarObj3.transform.Find("Body")?.GetComponent<SkinnedMeshRenderer>() ?? null
            };
            avatar.customEyeLookSettings = customEyeLookSettings;

            if (customEyeLookSettings.eyelidsSkinnedMesh is null)
            {
                customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.None;
            }

            if (customEyeLookSettings.leftEye is null && customEyeLookSettings.rightEye is null &&
                customEyeLookSettings.eyelidType == VRCAvatarDescriptor.EyelidType.None)
            {
                avatar.enableEyeLook = false;
            }

            avatar.customizeAnimationLayers = true;
            avatar.baseAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[]
            {
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.Base,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.Additive,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.Gesture,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.Action,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.FX,
                    isDefault = true
                }
            };

            avatar.specialAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[]
            {
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.Sitting,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.TPose,
                    isDefault = true
                },
                new VRCAvatarDescriptor.CustomAnimLayer
                {
                    type = VRCAvatarDescriptor.AnimLayerType.IKPose,
                    isDefault = true
                }
            };

            var originalHandLayerControllerPath = GetAssetPathForSearch("vrc_AvatarV3HandsLayer t:AnimatorController");
            var fxControllerName = $"{Path.GetFileNameWithoutExtension(originalHandLayerControllerPath)}_{avatarPrefab2.name}.controller";
            var fxControllerPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath), fxControllerName));
            AssetDatabase.CopyAsset(originalHandLayerControllerPath, fxControllerPath);
            var fxController = AssetDatabase.LoadAssetAtPath(fxControllerPath, typeof(AnimatorController));

            avatar.baseAnimationLayers[4].isDefault = false;
            avatar.baseAnimationLayers[4].animatorController = fxController as RuntimeAnimatorController;
            avatar.baseAnimationLayers[4].mask = null;

            return avatarObj3;
        }

        private VRCAvatarDescripterDeserializedObject GetAvatar2Info(GameObject avatarPrefab2)
        {
            var avatar2Info = new VRCAvatarDescripterDeserializedObject();
            var filePath = AssetDatabase.GetAssetPath(avatarPrefab2);
            var yaml = new YamlStream();
            using (var sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                yaml.Load(sr);
            }

            // コンポーネントレベルでDocumentが存在する
            foreach (var document in yaml.Documents)
            {
                var node = document.RootNode;
                // MonoBehaiviour以外は処理しない
                if (node.Tag != "tag:unity3d.com,2011:114") continue;

                var mapping = (YamlMappingNode)node;
                var vrcAvatarDescripter = (YamlMappingNode)mapping.Children["MonoBehaviour"];

                // VRCAvatarDescripter以外は処理しない
                if (((YamlScalarNode)((YamlMappingNode)vrcAvatarDescripter["m_Script"]).Children["guid"]).Value != "f78c4655b33cb5741983dc02e08899cf") continue;

                avatar2Info.Name = ((YamlScalarNode)vrcAvatarDescripter["Name"]).Value;

                // [View]
                // ViewPosition
                var viewPosition = (YamlMappingNode)vrcAvatarDescripter["ViewPosition"];
                avatar2Info.ViewPosition = new Vector3(
                                                float.Parse(((YamlScalarNode)viewPosition["x"]).Value),
                                                float.Parse(((YamlScalarNode)viewPosition["y"]).Value),
                                                float.Parse(((YamlScalarNode)viewPosition["z"]).Value)
                                            );
                // ScaleIPD
                avatar2Info.ScaleIPD = ((YamlScalarNode)vrcAvatarDescripter["ScaleIPD"]).Value == "1";

                // [LipSync]
                // Mode
                var lipSyncTypeIndex = int.Parse(((YamlScalarNode)vrcAvatarDescripter["lipSync"]).Value);
                avatar2Info.lipSync = (VRC.SDKBase.VRC_AvatarDescriptor.LipSyncStyle)Enum.ToObject(typeof(VRC.SDKBase.VRC_AvatarDescriptor.LipSyncStyle), lipSyncTypeIndex);
                // FaceMesh
                var faceMeshRendererGuid = ((YamlScalarNode)((YamlMappingNode)vrcAvatarDescripter["VisemeSkinnedMesh"]).Children["fileID"]).Value;
                var path = GetSkinnedMeshRendererPathFromGUID(yaml.Documents, faceMeshRendererGuid);
                avatar2Info.faceMeshRendererPath = path;
                // VisemeBlendShapes
                avatar2Info.VisemeBlendShapes = new string[15];
                var visemeBlendShapes = ((YamlSequenceNode)vrcAvatarDescripter["VisemeBlendShapes"]);
                for (int i = 0; i < 15; i++)
                {
                    avatar2Info.VisemeBlendShapes[i] = ((YamlScalarNode)visemeBlendShapes[i]).Value;
                }

                // [AnimationLayers]
                // CustomStaindingAnims
                var standingOverrideControllerGuid = ((YamlScalarNode)((YamlMappingNode)vrcAvatarDescripter["CustomStandingAnims"]).Children["guid"]).Value;
                avatar2Info.standingOverrideControllerPath = AssetDatabase.GUIDToAssetPath(standingOverrideControllerGuid);

                break;
            }

            return avatar2Info;
        }

        private YamlNode GetNodeFromGUID(IList<YamlDocument> components, string guid)
        {
            foreach (var component in components)
            {
                var node = component.RootNode;
                if (node.Anchor != guid) continue;
                return node;
            }
            return null;
        }

        private string GetSkinnedMeshRendererPathFromGUID(IList<YamlDocument> components, string rendererGuid)
        {
            string path = string.Empty;
            var node = GetNodeFromGUID(components, rendererGuid);
            var skinnedMeshRenderer = (YamlMappingNode)((YamlMappingNode)node).Children["SkinnedMeshRenderer"];
            
            var gameObjectGuid = ((YamlScalarNode)((YamlMappingNode)skinnedMeshRenderer["m_GameObject"]).Children["fileID"]).Value;
            node = GetNodeFromGUID(components, gameObjectGuid);
            var gameObjectNode = (YamlMappingNode)((YamlMappingNode)node).Children["GameObject"];

            string gameObjectName = ((YamlScalarNode)gameObjectNode["m_Name"]).Value;
            while (true)
            {
                string parentGuid = string.Empty;
                var componentInGameObject = (YamlSequenceNode)gameObjectNode["m_Component"];
                foreach (YamlMappingNode component in componentInGameObject)
                {
                    var componentGuid = ((YamlScalarNode)((YamlMappingNode)component["component"]).Children["fileID"]).Value;
                    node = GetNodeFromGUID(components, componentGuid);
                    // Transform以外処理しない
                    if (node.Tag != "tag:unity3d.com,2011:4") continue;

                    var transform = (YamlMappingNode)((YamlMappingNode)node).Children["Transform"];
                    parentGuid = ((YamlScalarNode)((YamlMappingNode)transform["m_Father"]).Children["fileID"]).Value;
                    break;
                }

                if (string.IsNullOrEmpty(parentGuid)) break;

                node = GetNodeFromGUID(components, parentGuid);

                if (node is null) break;

                var parentTransform = (YamlMappingNode)((YamlMappingNode)node).Children["Transform"];
                gameObjectGuid = ((YamlScalarNode)((YamlMappingNode)parentTransform["m_GameObject"]).Children["fileID"]).Value;
                node = GetNodeFromGUID(components, gameObjectGuid);
                gameObjectNode = (YamlMappingNode)((YamlMappingNode)node).Children["GameObject"];
                path = $"{gameObjectName}/{path}";
                gameObjectName = ((YamlScalarNode)gameObjectNode["m_Name"]).Value;
            }

            path = path.Substring(0, path.Length - 1);
            return path;
        }

        private static string GetAssetPathForSearch(string filter)
        {
            var guid = AssetDatabase.FindAssets(filter).FirstOrDefault();
            return AssetDatabase.GUIDToAssetPath(guid);
        }
    }
}

