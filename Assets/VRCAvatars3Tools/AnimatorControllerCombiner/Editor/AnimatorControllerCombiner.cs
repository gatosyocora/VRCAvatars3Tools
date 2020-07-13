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

// ver 1.0.2
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
                    CombineAnimatorController(srcController, dstController);
                }
            }
        }

        private void CombineAnimatorController(AnimatorController srcController, AnimatorController dstController)
        {
            var dstControllerPath = AssetDatabase.GetAssetPath(dstController);

            for (int i = 0; i < srcController.layers.Length; i++)
            {
                var layer = srcController.layers[i];
                var newLayer = DuplicateLayer(layer, dstController.MakeUniqueLayerName(layer.name), i == 0);
                dstController.AddLayer(newLayer);

                // Unity再起動後も保持するためにアセットにオブジェクトを追加する必要がある
                AddObjectsInLayerToAnimatorController(newLayer, dstControllerPath);
            }

            foreach (var parameter in srcController.parameters)
            {
                // 同じParameterがあれば追加しない
                if (dstController.parameters
                        .Select(x => new
                            {
                                Name = x.name,
                                Type = x.type
                            })
                        .Where(x => parameter.name == x.Name && parameter.type == x.Type)
                        .Any()) continue;
                
                dstController.AddParameter(parameter);
            }

            EditorUtility.SetDirty(dstController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private AnimatorControllerLayer DuplicateLayer(AnimatorControllerLayer srcLayer, string dstLayerName, bool firstLayer = false)
        {
            var newLayer = new AnimatorControllerLayer()
            {
                avatarMask = srcLayer.avatarMask,
                blendingMode = srcLayer.blendingMode,
                defaultWeight = srcLayer.defaultWeight,
                iKPass = srcLayer.iKPass,
                name = dstLayerName,
                // 新しく作らないとLayer削除時にコピー元LayerのStateが消える
                stateMachine = DuplicateStateMachine(srcLayer.stateMachine),
                syncedLayerAffectsTiming = srcLayer.syncedLayerAffectsTiming,
                syncedLayerIndex = srcLayer.syncedLayerIndex
            };

            // 最初のレイヤーはdefaultWeightがどんなものでも自動的にweightが1扱いになっているので
            // defaultWeightを1にする
            if (firstLayer) newLayer.defaultWeight = 1f;

            // StateとStateMachineをすべて追加後に遷移を設定
            // 親階層へ伸びているっぽい遷移もある?
            CopyTransitions(srcLayer.stateMachine, newLayer.stateMachine);

            return newLayer;
        }

        private AnimatorStateMachine DuplicateStateMachine(AnimatorStateMachine srcStateMachine)
        {
            var dstStateMachine = new AnimatorStateMachine
            {
                anyStatePosition = srcStateMachine.anyStatePosition,
                entryPosition = srcStateMachine.entryPosition,
                exitPosition = srcStateMachine.exitPosition,
                hideFlags = srcStateMachine.hideFlags,
                name = srcStateMachine.name,
                parentStateMachinePosition = srcStateMachine.parentStateMachinePosition,
                stateMachines = srcStateMachine.stateMachines
                                    .Select(cs =>
                                        new ChildAnimatorStateMachine
                                        {
                                            position = cs.position,
                                            stateMachine = DuplicateStateMachine(cs.stateMachine)
                                        })
                                    .ToArray(),
                states = DuplicateChildStates(srcStateMachine.states),
            };

            // behaivoursを設定
            foreach (var srcBehaivour in srcStateMachine.behaviours)
            {
                var behaivour = dstStateMachine.AddStateMachineBehaviour(srcBehaivour.GetType());
                CopyBehaivourParameters(srcBehaivour, behaivour);
            }

            // defaultStateの設定
            if (srcStateMachine.defaultState != null)
            {
                var defaultStateIndex = srcStateMachine.states
                                    .Select((value, index) => new { Value = value.state, Index = index })
                                    .Where(s => s.Value == srcStateMachine.defaultState)
                                    .Select(s => s.Index).SingleOrDefault();
                dstStateMachine.defaultState = dstStateMachine.states[defaultStateIndex].state;
            }

            return dstStateMachine;
        }

        private ChildAnimatorState[] DuplicateChildStates(ChildAnimatorState[] srcChildStates)
        {
            var dstStates = new ChildAnimatorState[srcChildStates.Length];

            for (int i = 0; i < srcChildStates.Length; i++)
            {
                var srcState = srcChildStates[i].state;
                dstStates[i] = new ChildAnimatorState
                {
                    position = srcChildStates[i].position,
                    state = DuplicateAnimatorState(srcState)
                };

                // behavioursを設定
                foreach (var srcBehaivour in srcChildStates[i].state.behaviours)
                {
                    var behaivour = dstStates[i].state.AddStateMachineBehaviour(srcBehaivour.GetType());
                    CopyBehaivourParameters(srcBehaivour, behaivour);
                }
            }

            return dstStates;
        }

        private AnimatorState DuplicateAnimatorState(AnimatorState srcState)
        {
            return new AnimatorState
            {
                cycleOffset = srcState.cycleOffset,
                cycleOffsetParameter = srcState.cycleOffsetParameter,
                cycleOffsetParameterActive = srcState.cycleOffsetParameterActive,
                hideFlags = srcState.hideFlags,
                iKOnFeet = srcState.iKOnFeet,
                mirror = srcState.mirror,
                mirrorParameter = srcState.mirrorParameter,
                mirrorParameterActive = srcState.mirrorParameterActive,
                motion = srcState.motion,
                name = srcState.name,
                speed = srcState.speed,
                speedParameter = srcState.speedParameter,
                speedParameterActive = srcState.speedParameterActive,
                tag = srcState.tag,
                timeParameter = srcState.timeParameter,
                timeParameterActive = srcState.timeParameterActive,
                writeDefaultValues = srcState.writeDefaultValues
            };
        }

        // TODO: StateMachineへの対応
        private void CopyTransitions(AnimatorStateMachine srcStateMachine, AnimatorStateMachine dstStateMachine,
                                    AnimatorState[] srcStates = null, AnimatorState[] dstStates = null,
                                    AnimatorStateMachine[] srcStateMachines = null, AnimatorStateMachine[] dstStateMachines = null)
        {
            if (srcStates is null)
            {
                srcStates = GetAllStates(srcStateMachine);
                dstStates = GetAllStates(dstStateMachine);
                srcStateMachines = GetAllStateMachines(srcStateMachine);
                dstStateMachines = GetAllStateMachines(dstStateMachine);
            }

            // StateからのTransitionを設定
            for (int i = 0; i < srcStates.Length; i++)
            {
                foreach (var srcTransition in srcStates[i].transitions)
                {
                    AnimatorStateTransition dstTransition;

                    if (srcTransition.isExit)
                    {
                        dstTransition = dstStates[i].AddExitTransition();
                        Debug.Log($"state:{srcStates[i].name} -> Exit");
                    }
                    else if (srcTransition.destinationState != null)
                    {
                        var stateIndex = Array.IndexOf(srcStates, srcTransition.destinationState);
                        dstTransition = dstStates[i].AddTransition(dstStates[stateIndex]);
                    }
                    else if (srcTransition.destinationStateMachine != null)
                    {
                        var stateMachineIndex = Array.IndexOf(srcStateMachines, srcTransition.destinationStateMachine);
                        dstTransition = dstStates[i].AddTransition(dstStateMachines[stateMachineIndex]);
                    }
                    else continue;
                    CopyTransitionParameters(srcTransition, dstTransition);
                }
            }

            for (int i = 0; i < srcStateMachines.Length; i++)
            {
                // AnyStateからのTransitionを設定
                foreach (var srcTransition in srcStateMachines[i].anyStateTransitions)
                {
                    AnimatorStateTransition dstTransition;
                    if (srcTransition.isExit)
                    {
                        Debug.Log($"any:{srcStateMachines[i].name} -> Exit");
                        continue;
                    }
                    else if (srcTransition.destinationState != null)
                    {
                        var stateIndex = Array.IndexOf(srcStates, srcTransition.destinationState);
                        dstTransition = dstStateMachines[i].AddAnyStateTransition(dstStates[stateIndex]);
                    }
                    else if (srcTransition.destinationStateMachine != null)
                    {
                        var stateMachineIndex = Array.IndexOf(srcStateMachines, srcTransition.destinationStateMachine);
                        dstTransition = dstStateMachines[i].AddAnyStateTransition(dstStateMachines[stateMachineIndex]);
                    }
                    else continue;

                    CopyTransitionParameters(srcTransition, dstTransition);
                }

                // EntryStateからのTransitionを設定
                foreach (var srcTransition in srcStateMachines[i].entryTransitions)
                {
                    AnimatorTransition dstTransition;
                    if (srcTransition.isExit)
                    {
                        //dstTransition = dstStateMachines[i].AddStateMachineExitTransition(dstStateMachine);
                        Debug.Log($"entry:{srcStateMachines[i].name} -> Exit");
                        continue;
                    }
                    else if (srcTransition.destinationState != null)
                    {
                        var stateIndex = Array.IndexOf(srcStates, srcTransition.destinationState);
                        dstTransition = dstStateMachines[i].AddEntryTransition(dstStates[stateIndex]);
                    }
                    else if (srcTransition.destinationStateMachine != null)
                    {
                        var stateMachineIndex = Array.IndexOf(srcStateMachines, srcTransition.destinationStateMachine);
                        dstTransition = dstStateMachines[i].AddEntryTransition(dstStateMachines[stateMachineIndex]);
                    }
                    else continue;

                    CopyTransitionParameters(srcTransition, dstTransition);
                }
            }
        }

        private AnimatorState[] GetAllStates(AnimatorStateMachine stateMachine)
        {
            var stateList = stateMachine.states.Select(sc => sc.state).ToList();
            foreach (var subStatetMachine in stateMachine.stateMachines)
            {
                stateList.AddRange(GetAllStates(subStatetMachine.stateMachine));
            }
            return stateList.ToArray();
        }

        private AnimatorStateMachine[] GetAllStateMachines(AnimatorStateMachine stateMachine)
        {
            var stateMachineList = new List<AnimatorStateMachine>
            {
                stateMachine
            };

            foreach (var subStateMachine in stateMachine.stateMachines)
            {
                stateMachineList.AddRange(GetAllStateMachines(subStateMachine.stateMachine));
            }

            return stateMachineList.ToArray();
        }

        private void CopyTransitionParameters(AnimatorStateTransition srcTransition, AnimatorStateTransition dstTransition)
        {
            dstTransition.canTransitionToSelf = srcTransition.canTransitionToSelf;
            dstTransition.duration = srcTransition.duration;
            dstTransition.exitTime = srcTransition.exitTime;
            dstTransition.hasExitTime = srcTransition.hasExitTime;
            dstTransition.hasFixedDuration = srcTransition.hasFixedDuration;
            dstTransition.hideFlags = srcTransition.hideFlags;
            dstTransition.isExit = srcTransition.isExit;
            dstTransition.mute = srcTransition.mute;
            dstTransition.name = srcTransition.name;
            dstTransition.offset = srcTransition.offset;
            dstTransition.interruptionSource = srcTransition.interruptionSource;
            dstTransition.orderedInterruption = srcTransition.orderedInterruption;
            dstTransition.solo = srcTransition.solo;
            foreach (var srcCondition in srcTransition.conditions)
            {
                dstTransition.AddCondition(srcCondition.mode, srcCondition.threshold, srcCondition.parameter);
            }
        }

        private void CopyTransitionParameters(AnimatorTransition srcTransition, AnimatorTransition dstTransition)
        {
            dstTransition.hideFlags = srcTransition.hideFlags;
            dstTransition.isExit = srcTransition.isExit;
            dstTransition.mute = srcTransition.mute;
            dstTransition.name = srcTransition.name;
            dstTransition.solo = srcTransition.solo;
            foreach (var srcCondition in srcTransition.conditions)
            {
                dstTransition.AddCondition(srcCondition.mode, srcCondition.threshold, srcCondition.parameter);
            }

        }

        private void CopyBehaivourParameters(StateMachineBehaviour srcBehaivour, StateMachineBehaviour dstBehaivour)
        {
            if (srcBehaivour.GetType() != dstBehaivour.GetType())
            {
                throw new ArgumentException("Should be same type");
            }

            if (dstBehaivour is VRCAnimatorLayerControl layerControl)
            {
                var srcControl = srcBehaivour as VRCAnimatorLayerControl;
                layerControl.ApplySettings = srcControl.ApplySettings;
                layerControl.blendDuration = srcControl.blendDuration;
                layerControl.debugString = srcControl.debugString;
                layerControl.goalWeight = srcControl.goalWeight;
                layerControl.playable = srcControl.playable;
            }
            else if (dstBehaivour is VRCAnimatorLocomotionControl locomotionControl)
            {
                var srcControl = srcBehaivour as VRCAnimatorLocomotionControl;
                locomotionControl.ApplySettings = srcControl.ApplySettings;
                locomotionControl.debugString = srcControl.debugString;
                locomotionControl.disableLocomotion = srcControl.disableLocomotion;
            }
            else if (dstBehaivour is VRCAnimatorSetView setView)
            {
                var srcView = srcBehaivour as VRCAnimatorSetView;
                setView.ApplySettings = srcView.ApplySettings;
                setView.debugString = srcView.debugString;
                setView.delayTime = srcView.delayTime;
                setView.setView = srcView.setView;
            }
            else if (dstBehaivour is VRCAnimatorTrackingControl trackingControl)
            {
                var srcControl = srcBehaivour as VRCAnimatorTrackingControl;
                trackingControl.ApplySettings = srcControl.ApplySettings;
                trackingControl.debugString = srcControl.debugString;
                trackingControl.trackingEyes = srcControl.trackingEyes;
                trackingControl.trackingHead = srcControl.trackingHead;
                trackingControl.trackingHip = srcControl.trackingHip;
                trackingControl.trackingLeftFingers = srcControl.trackingLeftFingers;
                trackingControl.trackingLeftFoot = srcControl.trackingLeftFoot;
                trackingControl.trackingLeftHand = srcControl.trackingLeftHand;
                trackingControl.trackingRightFingers = srcControl.trackingRightFingers;
                trackingControl.trackingRightFoot = srcControl.trackingRightFoot;
                trackingControl.trackingRightHand = srcControl.trackingRightHand;
            }
            else if (dstBehaivour is VRCAvatarParameterDriver parameterDriver)
            {
                var srcDriver = srcBehaivour as VRCAvatarParameterDriver;
                parameterDriver.ApplySettings = srcDriver.ApplySettings;
                parameterDriver.debugString = srcDriver.debugString;
                parameterDriver.parameters = srcDriver.parameters
                                                .Select(p =>
                                                new Parameter
                                                {
                                                    name = p.name,
                                                    value = p.value
                                                })
                                                .ToList();
            }
            else if (dstBehaivour is VRCPlayableLayerControl playableLayerControl)
            {
                var srcControl = srcBehaivour as VRCPlayableLayerControl;
                playableLayerControl.ApplySettings = srcControl.ApplySettings;
                playableLayerControl.blendDuration = srcControl.blendDuration;
                playableLayerControl.debugString = srcControl.debugString;
                playableLayerControl.goalWeight = srcControl.goalWeight;
                playableLayerControl.layer = srcControl.layer;
                playableLayerControl.outputParamHash = srcControl.outputParamHash;
            }
        }

        // TODO: StateMachineへの対応
        private void AddObjectsInLayerToAnimatorController(AnimatorControllerLayer layer, string controllerPath)
        {
            AssetDatabase.AddObjectToAsset(layer.stateMachine, controllerPath);
            foreach (var childState in layer.stateMachine.states)
            {
                AssetDatabase.AddObjectToAsset(childState.state, controllerPath);
                foreach (var transition in childState.state.transitions)
                {
                    AssetDatabase.AddObjectToAsset(transition, controllerPath);
                }
                foreach (var behaviour in childState.state.behaviours)
                {
                    AssetDatabase.AddObjectToAsset(behaviour, controllerPath);
                }
            }
            //foreach (var childStateMachine in layer.stateMachine.stateMachines)
            //{
            //    AssetDatabase.AddObjectToAsset(childStateMachine.stateMachine, controllerPath);
            //}
            foreach (var transition in layer.stateMachine.anyStateTransitions)
            {
                AssetDatabase.AddObjectToAsset(transition, controllerPath);
            }
            foreach (var transition in layer.stateMachine.entryTransitions)
            {
                AssetDatabase.AddObjectToAsset(transition, controllerPath);
            }
            foreach (var behaviour in layer.stateMachine.behaviours)
            {
                AssetDatabase.AddObjectToAsset(behaviour, controllerPath);
            }
        }
    }
}