using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
//[UpdateAfter(typeof(SelectorSpawnerSystem))]
namespace BaseBuilderCore {
    public partial struct CleanupSelectorSystem : ISystem {
        EndSimulationEntityCommandBufferSystem.Singleton endSimEcb;

        public void OnCreate(ref SystemState state) {
            //state.RequireForUpdate<EndSimulationEntityCommandBufferSystem>();
        }
        public void OnStartRunning(ref SystemState state) {
        }

        public void OnUpdate(ref SystemState state) {
            EndSimulationEntityCommandBufferSystem.Singleton endSimEcb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = endSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            foreach ((RefRO<SelectorStateData> selectionStateData, Entity selectedEntity) in SystemAPI.Query<RefRO<SelectorStateData>>().WithAll<SelectableCellTag>().WithDisabled<SelectedCellTag>().WithEntityAccess()) {
                //UnityEngine.Debug.Log("destroy Entity:" + selectionStateData.ValueRO.SelectionUI);
                ecb.DestroyEntity(selectionStateData.ValueRO.SelectionUI);
                ecb.RemoveComponent<SelectorStateData>(selectedEntity);
            }
        }
    }
}