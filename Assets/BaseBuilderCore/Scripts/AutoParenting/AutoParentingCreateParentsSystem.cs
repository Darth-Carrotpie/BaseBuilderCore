using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.Search;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace BaseBuilderCore {
    [BurstCompile]
    public partial struct AutoParentingCreateParentsSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildOrder>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            bool parentCheck = false;
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
                                                                                                                                                         //EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            CreateParentBuildings<BuildingsParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem
            CreateParentHexCell<HexCellParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem
            CreateParentNodes<NodesParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem
            CreateParentLinks<LinksParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        /*Entity CreateParentEntityIfNotExists<T>(EntityCommandBuffer ecb, SystemState state) where T : struct, IComponentData
            //To Do:
            // : struct, IComponentData should have solved the error, but hasn't.
        {
            var singletonQuery = state.GetEntityQuery(ComponentType.ReadOnly<T>());
            if (singletonQuery.IsEmpty)
            {
                Entity newParentEntity = ecb.CreateEntity();
                ecb.AddComponent<T>(newParentEntity); //should be type T, but gives non-nullable error
                ecb.SetName(newParentEntity, typeof(T).Name);
                return newParentEntity;
            }
            return singletonQuery.GetSingletonEntity();
        }*/
        void CreateParentBuildings<T>(EntityCommandBuffer ecb, SystemState state) {
            var query = SystemAPI.QueryBuilder().WithAll<BuildingsParent>().Build();

            if (query.IsEmpty) {
                Entity newParentEntity = ecb.CreateEntity();
                AddDefaultsToParent(ecb, newParentEntity);
                ecb.AddComponent<BuildingsParent>(newParentEntity);
                ecb.SetName(newParentEntity, typeof(T).Name);
                //UnityEngine.Debug.Log("parent created: Buildings!");
            }
        }
        void CreateParentHexCell<T>(EntityCommandBuffer ecb, SystemState state) {
            var query = SystemAPI.QueryBuilder().WithAll<HexCellParent>().Build();

            if (query.IsEmpty) {
                Entity newParentEntity = ecb.CreateEntity();
                AddDefaultsToParent(ecb, newParentEntity);
                ecb.AddComponent<HexCellParent>(newParentEntity);
                ecb.SetName(newParentEntity, typeof(T).Name);
                //UnityEngine.Debug.Log("parent created: HexCells!");
            }
        }
        void CreateParentNodes<T>(EntityCommandBuffer ecb, SystemState state) {
            var query = SystemAPI.QueryBuilder().WithAll<NodesParent>().Build();

            if (query.IsEmpty) {
                Entity newParentEntity = ecb.CreateEntity();
                AddDefaultsToParent(ecb, newParentEntity);
                ecb.AddComponent<NodesParent>(newParentEntity);
                ecb.SetName(newParentEntity, typeof(T).Name);
                //UnityEngine.Debug.Log("parent created: ForceNodes!");
                /*ecb.AddComponent<PhysicsStep>(newParentEntity);
                ecb.SetComponent<PhysicsStep>(newParentEntity, new PhysicsStep
                {
                    SimulationType = SimulationType.UnityPhysics,
                    SolverIterationCount = 4,
                    MultiThreaded = 1,
                });*/
            }
        }
        void CreateParentLinks<T>(EntityCommandBuffer ecb, SystemState state) {
            var query = SystemAPI.QueryBuilder().WithAll<LinksParent>().Build();

            if (query.IsEmpty) {
                Entity newParentEntity = ecb.CreateEntity();
                AddDefaultsToParent(ecb, newParentEntity);
                ecb.AddComponent<LinksParent>(newParentEntity);
                ecb.SetName(newParentEntity, typeof(T).Name);
                //UnityEngine.Debug.Log("parent created: ForceLinks!");
            }
        }

        void AddDefaultsToParent(EntityCommandBuffer ecb, Entity newParentEntity) {
            ecb.AddComponent<LocalTransform>(newParentEntity); //without this children positions wont update
                                                               //scale is 0 by default, so have to set it to 1, for proper position updates:
            ecb.SetComponent<LocalTransform>(newParentEntity, new LocalTransform {
                Scale = 1,
                Position = float3.zero,
                Rotation = quaternion.identity
            });
            ecb.AddComponent<LocalToWorld>(newParentEntity);
        }
    }
}