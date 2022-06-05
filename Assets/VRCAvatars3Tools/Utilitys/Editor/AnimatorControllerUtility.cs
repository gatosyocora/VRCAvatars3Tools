using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.IO;

// ver 1.0.3
// Copyright (c) 2020 gatosyocora
// MIT License. See LICENSE.txt

namespace Gatosyocora.VRCAvatars3Tools.Utilitys
{
    public static class AnimatorControllerUtility
    {
        public static void CombineAnimatorController(AnimatorController srcController, AnimatorController dstController)
        {
            var dstControllerPath = AssetDatabase.GetAssetPath(dstController);

            for (int i = 0; i < srcController.layers.Length; i++)
            {
                AddLayer(dstController, srcController.layers[i], i == 0, dstControllerPath);
            }

            foreach (var parameter in srcController.parameters)
            {
                AddParameter(dstController, parameter);
            }

            EditorUtility.SetDirty(dstController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static AnimatorControllerLayer AddLayer(AnimatorController controller, AnimatorControllerLayer srcLayer, bool setWeightTo1 = false, string controllerPath = "")
        {
            if (string.IsNullOrEmpty(controllerPath))
            {
                controllerPath = AssetDatabase.GetAssetPath(controller);
            }

            var newLayer = DuplicateLayer(srcLayer, controller.MakeUniqueLayerName(srcLayer.name), controllerPath, setWeightTo1);
            controller.AddLayer(newLayer);

            // Unity再起動後も保持するためにアセットにオブジェクトを追加する必要がある
            AddObjectsInStateMachineToAnimatorController(newLayer.stateMachine, controllerPath);

            return newLayer;
        }

        public static AnimatorControllerParameter AddParameter(AnimatorController controller, AnimatorControllerParameter srcParameter)
        {
            // 同じParameterがあれば追加しない
            if (controller.parameters.Any(p => p.name == srcParameter.name))
                return null;

            var parameter = new AnimatorControllerParameter
            {
                defaultBool = srcParameter.defaultBool,
                defaultFloat = srcParameter.defaultFloat,
                defaultInt = srcParameter.defaultInt,
                name = srcParameter.name,
                type = srcParameter.type
            };

            controller.AddParameter(parameter);
            return parameter;
        }

        private static AnimatorControllerLayer DuplicateLayer(AnimatorControllerLayer srcLayer, string dstLayerName, string controllerPath, bool firstLayer = false)
        {
            var newLayer = new AnimatorControllerLayer()
            {
                avatarMask = srcLayer.avatarMask,
                blendingMode = srcLayer.blendingMode,
                defaultWeight = srcLayer.defaultWeight,
                iKPass = srcLayer.iKPass,
                name = dstLayerName,
                // 新しく作らないとLayer削除時にコピー元LayerのStateが消える
                stateMachine = DuplicateStateMachine(srcLayer.stateMachine, controllerPath),
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

        private static AnimatorStateMachine DuplicateStateMachine(AnimatorStateMachine srcStateMachine, string controllerPath)
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
                                            stateMachine = DuplicateStateMachine(cs.stateMachine, controllerPath)
                                        })
                                    .ToArray(),
                states = DuplicateChildStates(srcStateMachine.states, controllerPath),
            };

            // behaivoursを設定
            foreach (var srcBehaivour in srcStateMachine.behaviours)
            {
                var behaivour = dstStateMachine.AddStateMachineBehaviour(srcBehaivour.GetType());
                DeepCopy(srcBehaivour, behaivour);
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

        private static ChildAnimatorState[] DuplicateChildStates(ChildAnimatorState[] srcChildStates, string controllerPath)
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

                AssetDatabase.AddObjectToAsset(dstStates[i].state, controllerPath);

                // behavioursを設定
                foreach (var srcBehaivour in srcChildStates[i].state.behaviours)
                {
                    var behaivour = dstStates[i].state.AddStateMachineBehaviour(srcBehaivour.GetType());
                    DeepCopy(srcBehaivour, behaivour);
                }
            }

            return dstStates;
        }

        private static AnimatorState DuplicateAnimatorState(AnimatorState srcState)
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

        private static void CopyTransitions(AnimatorStateMachine srcStateMachine, AnimatorStateMachine dstStateMachine)
        {
            var srcStates = GetAllStates(srcStateMachine);
            var dstStates = GetAllStates(dstStateMachine);
            var srcStateMachines = GetAllStateMachines(srcStateMachine);
            var dstStateMachines = GetAllStateMachines(dstStateMachine);

            // StateからのTransitionを設定
            for (int i = 0; i < srcStates.Length; i++)
            {
                foreach (var srcTransition in srcStates[i].transitions)
                {
                    AnimatorStateTransition dstTransition;

                    if (srcTransition.isExit)
                    {
                        dstTransition = dstStates[i].AddExitTransition();
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

            // SubStateMachine, EntryState, AnyStateからのTransitionを設定
            for (int i = 0; i < srcStateMachines.Length; i++)
            {
                // SubStateMachineからのTransitionを設定
                CopyTransitionOfSubStateMachine(srcStateMachines[i], dstStateMachines[i],
                                                srcStates, dstStates,
                                                srcStateMachines, dstStateMachines);

                // AnyStateからのTransitionを設定
                foreach (var srcTransition in srcStateMachines[i].anyStateTransitions)
                {
                    AnimatorStateTransition dstTransition;

                    // AnyStateからExitStateへの遷移は存在しないはず
                    if (srcTransition.isExit)
                    {
                        Debug.LogError($"Unknown transition:{srcStateMachines[i].name}.AnyState->Exit");
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

                    // EntryStateからExitStateへの遷移は存在しないはず
                    if (srcTransition.isExit)
                    {
                        Debug.LogError($"Unknown transition:{srcStateMachines[i].name}.Entry->Exit");
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

        private static void CopyTransitionOfSubStateMachine(AnimatorStateMachine srcParentStateMachine, AnimatorStateMachine dstParentStateMachine,
                                                     AnimatorState[] srcStates, AnimatorState[] dstStates,
                                                     AnimatorStateMachine[] srcStateMachines, AnimatorStateMachine[] dstStateMachines)
        {
            // SubStateMachineからのTransitionを設定
            for (int i = 0; i < srcParentStateMachine.stateMachines.Length; i++)
            {
                var srcSubStateMachine = srcParentStateMachine.stateMachines[i].stateMachine;
                var dstSubStateMachine = dstParentStateMachine.stateMachines[i].stateMachine;

                foreach (var srcTransition in srcParentStateMachine.GetStateMachineTransitions(srcSubStateMachine))
                {
                    AnimatorTransition dstTransition;

                    if (srcTransition.isExit)
                    {
                        dstTransition = dstParentStateMachine.AddStateMachineExitTransition(dstSubStateMachine);
                    }
                    else if (srcTransition.destinationState != null)
                    {
                        var stateIndex = Array.IndexOf(srcStates, srcTransition.destinationState);
                        dstTransition = dstParentStateMachine.AddStateMachineTransition(dstSubStateMachine, dstStates[stateIndex]);
                    }
                    else if (srcTransition.destinationStateMachine != null)
                    {
                        var stateMachineIndex = Array.IndexOf(srcStateMachines, srcTransition.destinationStateMachine);
                        dstTransition = dstParentStateMachine.AddStateMachineTransition(dstSubStateMachine, dstStateMachines[stateMachineIndex]);
                    }
                    else continue;

                    CopyTransitionParameters(srcTransition, dstTransition);
                }
            }
        }

        private static AnimatorState[] GetAllStates(AnimatorStateMachine stateMachine)
        {
            var stateList = stateMachine.states.Select(sc => sc.state).ToList();
            foreach (var subStatetMachine in stateMachine.stateMachines)
            {
                stateList.AddRange(GetAllStates(subStatetMachine.stateMachine));
            }
            return stateList.ToArray();
        }

        private static AnimatorStateMachine[] GetAllStateMachines(AnimatorStateMachine stateMachine)
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

        private static void CopyTransitionParameters(AnimatorStateTransition srcTransition, AnimatorStateTransition dstTransition)
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

        private static void CopyTransitionParameters(AnimatorTransition srcTransition, AnimatorTransition dstTransition)
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

        private static void AddObjectsInStateMachineToAnimatorController(AnimatorStateMachine stateMachine, string controllerPath)
        {
            AssetDatabase.AddObjectToAsset(stateMachine, controllerPath);

            foreach (var transition in stateMachine.anyStateTransitions)
            {
                AssetDatabase.AddObjectToAsset(transition, controllerPath);
            }
            foreach (var transition in stateMachine.entryTransitions)
            {
                AssetDatabase.AddObjectToAsset(transition, controllerPath);
            }
            foreach (var behaviour in stateMachine.behaviours)
            {
                AssetDatabase.AddObjectToAsset(behaviour, controllerPath);
            }
            foreach (var SubStateMachine in stateMachine.stateMachines)
            {
                foreach (var transition in stateMachine.GetStateMachineTransitions(SubStateMachine.stateMachine))
                {
                    AssetDatabase.AddObjectToAsset(transition, controllerPath);
                }
                AddObjectsInStateMachineToAnimatorController(SubStateMachine.stateMachine, controllerPath);
            }
        }

        public static AnimatorController DuplicateAnimationLayerController(string originalControllerPath, string outputFolderPath, string avatarName)
        {
            var controllerName = $"{Path.GetFileNameWithoutExtension(originalControllerPath)}_{avatarName}.controller";
            var controllerPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(outputFolderPath, controllerName));
            AssetDatabase.CopyAsset(originalControllerPath, controllerPath);
            return AssetDatabase.LoadAssetAtPath(controllerPath, typeof(AnimatorController)) as AnimatorController;
        }

        private static T DeepCopy<T>(T src, T dst)
        {
            var srcFields = src.GetType().GetFields();
            var dstFields = dst.GetType().GetFields();
            foreach (var srcField in srcFields)
            {
                foreach (var dstField in dstFields)
                {
                    if (srcField.Name != dstField.Name || srcField.FieldType != dstField.FieldType) continue;
                    dstField.SetValue(dst, srcField.GetValue(src));
                    break;
                }
            }

            var srcProperties = src.GetType().GetProperties();
            var dstProperties = dst.GetType().GetProperties();
            foreach (var srcProperty in srcProperties)
            {
                foreach (var dstProperty in dstProperties)
                {
                    if (srcProperty.Name != dstProperty.Name || srcProperty.PropertyType != dstProperty.PropertyType) continue;
                    dstProperty.SetValue(dst, srcProperty.GetValue(src));
                    break;
                }
            }
            return src;
        }
    }
}
