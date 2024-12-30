using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace BaseBuilderCore {
    //[BurstCompile]
    public partial struct AutoParentBuildingsSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildOrder>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            //state.Enabled = false;

            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
            EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            var query = SystemAPI.QueryBuilder().WithAll<BuildingsParent>().Build();
            if (query.IsEmpty) return;
            Entity parentEntity = query.GetSingletonEntity();

            foreach ((RefRW<Building> building, Entity entity) in SystemAPI.Query<RefRW<Building>>().WithAbsent<Parent>().WithAbsent<ExcludeFromAutoParenting>().WithEntityAccess()) {
                ecb.AddComponent<Parent>(entity);
                ecb.SetComponent(entity, new Parent { Value = parentEntity });
            }
        }
    }
}