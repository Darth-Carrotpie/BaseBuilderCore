using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    [BurstCompile]
    public partial struct MarkLinkStartSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<LinkOrder>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((RefRW<SelectableCellTag> cell, RefRW<GridCell> gridCell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>, RefRW<GridCell>>().WithDisabled<SelectedCellTag>().WithAll<MarkedForLinkStart>().WithEntityAccess()) {
                ecb.RemoveComponent<MarkedForLinkStart>(selectedEntity);
            }

            var query = SystemAPI.QueryBuilder()
                .WithAll<MarkedForLinkStart>()
                .Build();
            var startComponentsCount = query.ToEntityArray(Allocator.TempJob).Length;
            foreach ((RefRW<SelectableCellTag> cell, RefRW<GridCell> gridCell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>, RefRW<GridCell>>().WithAll<SelectedCellTag>().WithNone<MarkedForLinkStart>().WithEntityAccess()) {
                if (startComponentsCount == 0) {
                    ecb.AddComponent<MarkedForLinkStart>(selectedEntity);
                }
            }

        }
    }
}