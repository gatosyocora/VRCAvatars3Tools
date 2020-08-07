using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using YamlDotNet.RepresentationModel;

// ver 1.1
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools
{
    public class VRCAvatarConverterTo3 : EditorWindow
    {
        private GameObject avatarPrefab;

        private VRCAvatarDescripterDeserializedObject avatar2Info;

        private const string LEFT_EYE_PATH = "Armature/Hips/Spine/Chest/Neck/Head/LeftEye";
        private const string RIGHT_EYE_PATH = "Armature/Hips/Spine/Chest/Neck/Head/RightEye";
        private const string EYELIDS_MESH_PATH = "Body";

        private readonly static Dictionary<string, string> animationTypes = new Dictionary<string, string>
        {
            {"7400002", "Idle"},
            {"7400052", "Fist"},
            {"7400054", "Point"},
            {"7400056", "RockNRoll"},
            {"7400058", "Open"},
            {"7400060", "Thumbs up"},
            {"7400062", "Peace"},
            {"7400064", "Gun"},
            {"7400006", "Emote1"},
            {"7400008", "Emote2"},
            {"7400010", "Emote3"},
            {"7400012", "Emote4"},
            {"7400014", "Emote5"},
            {"7400016", "Emote6"},
            {"7400018", "Emote7"},
            {"7400020", "Emote8"},
        };

        private enum AnimationLayerType
        {
            Base = 0,
            Additive = 1,
            Gesture = 2,
            Action = 3,
            FX = 4
        }

        private enum SpecialAnimationLayerType
        {
            Sitting = 0,
            TPose = 1,
            IKPose = 2
        }

        private bool showViewInfo = true;
        private bool showLipSyncInfo = true;
        private bool showEyeLookInfo = true;
        private bool showAnimationLayersInfo = true;
        private Vector2 scrollPos = Vector2.zero;

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
                if (ObjectSelectorWrapper.isVisible)
                {
                    ObjectSelectorWrapper.SetFilterString("t:prefab");
                }

                if (check.changed && avatarPrefab != null)
                {
                    avatar2Info = GetAvatar2Info(avatarPrefab);
                }
            }

            if (avatarPrefab != null && avatar2Info != null)
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scroll.scrollPosition;

                    EditorGUILayout.LabelField("Prefab Name", avatarPrefab.name);

                    showViewInfo = EditorGUILayout.Foldout(showViewInfo, "View");
                    if (showViewInfo)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.LabelField("ViewPosition", avatar2Info.ViewPosition.ToString());
                            EditorGUILayout.LabelField("ScaleIPD", avatar2Info.ScaleIPD.ToString());
                        }
                    }

                    showLipSyncInfo = EditorGUILayout.Foldout(showLipSyncInfo, "LipSync");
                    if (showLipSyncInfo)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.LabelField("FaceMeshPath", avatar2Info.faceMeshRendererPath);
                        }
                    }

                    showEyeLookInfo = EditorGUILayout.Foldout(showEyeLookInfo, "EyeLook");
                    if (showEyeLookInfo)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.LabelField("Eyes.LeftEyeBone", LEFT_EYE_PATH);
                            EditorGUILayout.LabelField("Eyes.RightEyeBone", RIGHT_EYE_PATH);
                            EditorGUILayout.LabelField("EyelidType", "None");
                            EditorGUILayout.HelpBox("If use this, change type after convert", MessageType.Info);
                            EditorGUILayout.LabelField("Eyelids.FyelidsMesh", EYELIDS_MESH_PATH);
                            EditorGUILayout.LabelField("Eyelids.BlendShapeStates", "<Unimplemented>");
                            EditorGUILayout.HelpBox("Set LeftEyeBone, RightEyeBone and EyelidsMesh if found them", MessageType.Warning);
                        }
                    }

                    showAnimationLayersInfo = EditorGUILayout.Foldout(showAnimationLayersInfo, "AnimationLayers");
                    if (showAnimationLayersInfo)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.LabelField("StandingOverrideController", avatar2Info.standingOverrideControllerPath);
                            EditorGUILayout.LabelField("SittingOverrideController", "<Unimplemented>");

                            using (new EditorGUI.IndentLevelScope())
                            {
                                foreach (var animationClipInfo in avatar2Info.OverrideAnimationClips)
                                {
                                    if (animationClipInfo is null) continue;
                                    EditorGUILayout.LabelField(animationClipInfo.Type, animationClipInfo.Path);
                                }
                            }
                        }
                    }
                }
            }

            using (new EditorGUI.DisabledGroupScope(avatarPrefab is null || avatar2Info is null))
            {
                if (GUILayout.Button("Convert Avatar To 3.0"))
                {
                    var avatar3Obj = ConvertAvatarTo3(avatarPrefab, avatar2Info);
                    Selection.activeObject = avatar3Obj;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Remove missing component after convert", MessageType.Warning);
            EditorGUILayout.Space();
        }

        private GameObject ConvertAvatarTo3(GameObject avatarPrefab2, VRCAvatarDescripterDeserializedObject avatar2Info)
        {
            var avatarObj3 = PrefabUtility.InstantiatePrefab(avatarPrefab2) as GameObject;
            avatarObj3.name = GameObjectUtility.GetUniqueNameForSibling(avatarObj3.transform.parent, $"{ avatarObj3.name}_3.0");
            var avatar = avatarObj3.AddComponent<VRCAvatarDescriptor>();
            avatar.Name = avatar2Info.Name;
            avatar.ViewPosition = avatar2Info.ViewPosition;
            avatar.ScaleIPD = avatar2Info.ScaleIPD;
            avatar.lipSync = avatar2Info.lipSync;
            avatar.VisemeSkinnedMesh = avatarObj3.transform.Find(avatar2Info.faceMeshRendererPath)?.GetComponent<SkinnedMeshRenderer>() ?? null;
            avatar.VisemeBlendShapes = avatar2Info.VisemeBlendShapes;
            avatar.enableEyeLook = true;
            avatar.customEyeLookSettings = new VRCAvatarDescriptor.CustomEyeLookSettings
            {
                leftEye = avatarObj3.transform.Find(LEFT_EYE_PATH),
                rightEye = avatarObj3.transform.Find(RIGHT_EYE_PATH),
                // TODO: 設定が未完了なのでアバターが鏡に写らなくなってしまう
                //eyelidType = VRCAvatarDescriptor.EyelidType.Blendshapes,
                eyelidsSkinnedMesh = avatarObj3.transform.Find(EYELIDS_MESH_PATH)?.GetComponent<SkinnedMeshRenderer>() ?? null
            };

            if (avatar.customEyeLookSettings.eyelidsSkinnedMesh is null)
            {
                avatar.customEyeLookSettings.eyelidType = VRCAvatarDescriptor.EyelidType.None;
            }

            if (avatar.customEyeLookSettings.leftEye is null && avatar.customEyeLookSettings.rightEye is null &&
                avatar.customEyeLookSettings.eyelidType == VRCAvatarDescriptor.EyelidType.None)
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

            // FaceEmotion
            var searchTargetHL = "vrc_AvatarV3HandsLayer t:AnimatorController";
            if (avatar2Info.DefaultAnimationSet == VRCAvatarDescripterDeserializedObject.AnimationSet.Female) searchTargetHL = "vrc_AvatarV3HandsLayer2 t:AnimatorController";
            var originalHandLayerControllerPath = GetAssetPathForSearch(searchTargetHL);
            var fxController = DuplicateAnimationLayerController(
                                    originalHandLayerControllerPath,
                                    Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                    avatarPrefab2.name);

            avatar.baseAnimationLayers[(int)AnimationLayerType.FX].isDefault = false;
            avatar.baseAnimationLayers[(int)AnimationLayerType.FX].isEnabled = true;
            avatar.baseAnimationLayers[(int)AnimationLayerType.FX].animatorController = fxController;
            avatar.baseAnimationLayers[(int)AnimationLayerType.FX].mask = null;

            foreach (var layer in fxController.layers)
            {
                if (layer.name != "Left Hand" && layer.name != "Right Hand") continue;

                for (int i = 0; i < avatar2Info.OverrideAnimationClips.Length; i++)
                {
                    var animationInfo = avatar2Info.OverrideAnimationClips[i];
                    if (animationInfo is null || string.IsNullOrEmpty(animationInfo.Path) || animationInfo.Type.StartsWith("Emote")) continue;

                    var animClip = AssetDatabase.LoadAssetAtPath(animationInfo.Path, typeof(AnimationClip)) as AnimationClip;
                    var state = GetAnimatorStateFromStateName(layer.stateMachine, animationInfo.Type);
                    if (state is null) continue;
                    state.motion = animClip;
                    state.AddStateMachineBehaviour<VRCAnimatorTrackingControl>();
                }
            }

            if (HasEmoteAnimation(avatar2Info.OverrideAnimationClips))
            {
                // Emote
                var originalActionLayerController = Resources.Load<AnimatorController>("Controllers/vrc_AvatarV3ActionLayer_VRCAV3T");
                var originalActionLayerControllerPath = AssetDatabase.GetAssetPath(originalActionLayerController);
                var actionController = DuplicateAnimationLayerController(
                                            originalActionLayerControllerPath,
                                            Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                            avatarPrefab2.name);

                avatar.baseAnimationLayers[(int)AnimationLayerType.Action].isDefault = false;
                avatar.baseAnimationLayers[(int)AnimationLayerType.Action].isEnabled = true;
                avatar.baseAnimationLayers[(int)AnimationLayerType.Action].animatorController = actionController;
                avatar.baseAnimationLayers[(int)AnimationLayerType.Action].mask = null;

                var actionLayer = actionController.layers[0];
                for (int i = 0; i < avatar2Info.OverrideAnimationClips.Length; i++)
                {
                    var animationInfo = avatar2Info.OverrideAnimationClips[i];
                    if (animationInfo is null || string.IsNullOrEmpty(animationInfo.Path) || !animationInfo.Type.StartsWith("Emote")) continue;

                    var animClip = AssetDatabase.LoadAssetAtPath(animationInfo.Path, typeof(AnimationClip)) as AnimationClip;
                    var state = GetAnimatorStateFromStateName(actionLayer.stateMachine, animationInfo.Type);
                    if (state is null) continue;
                    state.motion = animClip;
                }

                avatar.customExpressions = true;
                var exMenu = CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(
                                exMenu,
                                AssetDatabase.GenerateUniqueAssetPath(
                                    Path.Combine(
                                        Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                        $"ExMenu_{avatarPrefab2.name}.asset")));
                var subMenuEmotes = CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(
                                subMenuEmotes,
                                AssetDatabase.GenerateUniqueAssetPath(
                                    Path.Combine(
                                        Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                        $"ExMenu_Emotes_{avatarPrefab2.name}.asset")));
                var exParameters = CreateInstance<VRCExpressionParameters>();
                AssetDatabase.CreateAsset(
                                exParameters,
                                AssetDatabase.GenerateUniqueAssetPath(
                                    Path.Combine(
                                        Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                        $"ExParams_{avatarPrefab2.name}.asset")));
                avatar.expressionsMenu = exMenu;
                avatar.expressionParameters = exParameters;

                var emoteIconPath = GetAssetPathForSearch("person_dance t:texture");
                var emoteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(emoteIconPath);

                exMenu.controls.Add(new VRCExpressionsMenu.Control
                {
                    name = "Emotes",
                    icon = emoteIcon,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = subMenuEmotes
                });

                for (int i = 0; i < avatar2Info.OverrideAnimationClips.Length; i++)
                {
                    var animationInfo = avatar2Info.OverrideAnimationClips[i];
                    if (animationInfo is null || string.IsNullOrEmpty(animationInfo.Path) || !animationInfo.Type.StartsWith("Emote")) continue;

                    subMenuEmotes.controls.Add(new VRCExpressionsMenu.Control
                    {
                        name = Path.GetFileNameWithoutExtension(animationInfo.Path),
                        icon = emoteIcon,
                        type = VRCExpressionsMenu.Control.ControlType.Button,
                        parameter = new VRCExpressionsMenu.Control.Parameter
                        {
                            name = "VRCEmote"
                        },
                        value = int.Parse(animationInfo.Type.Replace("Emote", string.Empty))
                    });
                }

                Selection.activeObject = exParameters;
            }

            // Sitting Animation
            var searchTargetSL = "vrc_AvatarV3SittingLayer t:AnimatorController";
            if (avatar2Info.DefaultAnimationSet == VRCAvatarDescripterDeserializedObject.AnimationSet.Female) searchTargetSL = "vrc_AvatarV3SittingLayer2 t:AnimatorController";
            var originalSittingLayerControllerPath = GetAssetPathForSearch(searchTargetSL);
            var sittingController = DuplicateAnimationLayerController(
                                    originalSittingLayerControllerPath,
                                    Path.GetDirectoryName(avatar2Info.standingOverrideControllerPath),
                                    avatarPrefab2.name);

            avatar.specialAnimationLayers[(int)SpecialAnimationLayerType.Sitting].isDefault = false;
            avatar.specialAnimationLayers[(int)SpecialAnimationLayerType.Sitting].isEnabled = true;
            avatar.specialAnimationLayers[(int)SpecialAnimationLayerType.Sitting].animatorController = sittingController;
            avatar.specialAnimationLayers[(int)SpecialAnimationLayerType.Sitting].mask = null;


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

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

                // Default Animation Set
                avatar2Info.DefaultAnimationSet = (VRCAvatarDescripterDeserializedObject.AnimationSet)Enum.Parse(typeof(VRCAvatarDescripterDeserializedObject.AnimationSet),
                                                    ((YamlScalarNode)vrcAvatarDescripter["Animations"]).Value);

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

                var yamlController = new YamlStream();
                using (var sr = new StreamReader(avatar2Info.standingOverrideControllerPath, System.Text.Encoding.UTF8))
                {
                    yaml.Load(sr);
                }
                var controllerNode = (YamlMappingNode)yaml.Documents[0].RootNode;
                var overrideController = (YamlMappingNode)controllerNode.Children["AnimatorOverrideController"];
                var clips = (YamlSequenceNode)overrideController.Children["m_Clips"];
                avatar2Info.OverrideAnimationClips = new AnimationClipInfo[clips.Count()];
                for (int i = 0; i < clips.Count(); i++)
                {
                    var clip = clips[i];
                    var clipPair = (YamlMappingNode)clip;
                    var originalClip = (YamlMappingNode)clipPair.Children["m_OriginalClip"];
                    var originalClipFileID = ((YamlScalarNode)originalClip.Children["fileID"]).Value;
                    var overrideClip = (YamlMappingNode)clipPair.Children["m_OverrideClip"];

                    if (!overrideClip.Children.TryGetValue("guid", out YamlNode overrideClipGuidNode))
                    {
                        continue;
                    }
                    var overrideClipGuid = ((YamlScalarNode)overrideClipGuidNode).Value;
                    if (!animationTypes.TryGetValue(originalClipFileID, out string animationType))
                    {
                        Debug.Log($"Don't Exist {originalClipFileID}");
                        continue;
                    }

                    avatar2Info.OverrideAnimationClips[i] = new AnimationClipInfo
                    {
                        Type = animationType,
                        Path = AssetDatabase.GUIDToAssetPath(overrideClipGuid)
                    };
                }

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

        private AnimatorState GetAnimatorStateFromStateName(AnimatorStateMachine stateMachine, string stateName)
        {
            foreach (var state in stateMachine.states)
            {
                if (state.state.name != stateName) continue;
                return state.state;
            }
            return null;
        }

        private static string GetAssetPathForSearch(string filter) =>
            AssetDatabase.FindAssets(filter)
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .OrderBy(p => Path.GetFileName(p).Count())
                .First();

        private AnimatorController DuplicateAnimationLayerController(string originalControllerPath, string outputFolderPath, string avatarName)
        {
            var controllerName = $"{Path.GetFileNameWithoutExtension(originalControllerPath)}_{avatarName}.controller";
            var controllerPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(outputFolderPath, controllerName));
            AssetDatabase.CopyAsset(originalControllerPath, controllerPath);
            return AssetDatabase.LoadAssetAtPath(controllerPath, typeof(AnimatorController)) as AnimatorController;
        }

        private bool HasEmoteAnimation(AnimationClipInfo[] infos) =>
            infos
                .Where(i => i != null)
                .Select(i => i.Type)
                .Any(t => t.StartsWith("Emote"));
    }
}

