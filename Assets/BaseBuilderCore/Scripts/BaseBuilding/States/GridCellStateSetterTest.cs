using BovineLabs.Core.States;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace BaseBuilderCore {
    [UpdateBefore(typeof(BuildOrderToPositionConsumerSystem))]
    [BurstCompile]
    public partial struct GridCellStateSetterTest : ISystem {
        //Only use this system if no other systems are registering or changing the state
        EntityManager entityManager;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<GridCell>();
            //can use the StateAPI.Register just once per system. It's meant to use a single state for a single system.
            //Registering injects a query filter to the system State. This can act similar to RequireForUpdate if argument is true
            //StateAPI.Register<GridCellVisualState, ArenaGridCellVisualState>(ref state, (byte)GridCellVisualStates.Arena, false);
            //StateAPI.Register<GridCellVisualState, WorkshopGridCellVisualState>(ref state, (byte)GridCellVisualStates.Workshop, false);
            //StateAPI.Register<GridCellVisualState, BarracksGridCellVisualState>(ref state, (byte)GridCellVisualStates.Barracks, false);
            //StateAPI.Register<GridCellVisualState, KitchenGridCellVisualState>(ref state, (byte)GridCellVisualStates.Kitchen, false);
            //StateAPI.Register<GridCellVisualState, ClearGridCellVisualState>(ref state, (byte)GridCellVisualStates.Clear, false);
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //Only use this system if no other systems are registering or changing the state
            //RegisterMultipleState(ref state);
        }
        void RegisterMultipleState(ref SystemState state) {
            var stateTypeIndex = TypeManager.GetTypeIndex<GridCellVisualState>();

            var e0 = state.EntityManager.CreateEntity(typeof(StateInstance));
            entityManager.SetComponentData(e0, new StateInstance {
                State = stateTypeIndex,
                StateKey = (byte)GridCellVisualStates.Clear,
                StateInstanceComponent = TypeManager.GetTypeIndex<ClearGridCellVisualState>(),
            });

            var e1 = state.EntityManager.CreateEntity(typeof(StateInstance));
            entityManager.SetComponentData(e1, new StateInstance {
                State = stateTypeIndex,
                StateKey = (byte)GridCellVisualStates.Arena,
                StateInstanceComponent = TypeManager.GetTypeIndex<ArenaGridCellVisualState>(),
            });
            var e2 = state.EntityManager.CreateEntity(typeof(StateInstance));
            entityManager.SetComponentData(e2, new StateInstance {
                State = stateTypeIndex,
                StateKey = (byte)GridCellVisualStates.Workshop,
                StateInstanceComponent = TypeManager.GetTypeIndex<WorkshopGridCellVisualState>(),
            });
            var e3 = state.EntityManager.CreateEntity(typeof(StateInstance));
            entityManager.SetComponentData(e3, new StateInstance {
                State = stateTypeIndex,
                StateKey = (byte)GridCellVisualStates.Barracks,
                StateInstanceComponent = TypeManager.GetTypeIndex<BarracksGridCellVisualState>(),
            });
            var e4 = state.EntityManager.CreateEntity(typeof(StateInstance));
            entityManager.SetComponentData(e4, new StateInstance {
                State = stateTypeIndex,
                StateKey = (byte)GridCellVisualStates.Kitchen,
                StateInstanceComponent = TypeManager.GetTypeIndex<KitchenGridCellVisualState>(),
            });

        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            //Only use this system if no other systems are changing the state
            state.Enabled = false;
            return;

            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
            var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            byte newState = 0;
            if (Input.GetKeyUp(KeyCode.BackQuote)) {
                newState = (byte)GridCellVisualStates.Clear;
                UnityEngine.Debug.Log("Set state to Clear");
            }
            if (Input.GetKeyUp(KeyCode.Alpha1)) {
                newState = (byte)GridCellVisualStates.Workshop;
                UnityEngine.Debug.Log("Set state to Workshop");
            }
            if (Input.GetKeyUp(KeyCode.Alpha2)) {
                newState = (byte)GridCellVisualStates.Kitchen;
                UnityEngine.Debug.Log("Set state to Kitchen");
            }
            if (Input.GetKeyUp(KeyCode.Alpha3)) {
                newState = (byte)GridCellVisualStates.Barracks;
                UnityEngine.Debug.Log("Set state to Barracks");
            }
            if (Input.GetKeyUp(KeyCode.Alpha4)) {
                newState = (byte)GridCellVisualStates.Arena;
                UnityEngine.Debug.Log("Set state to Arena");
            }
            if (newState == 0) return;
            //Set new state:
            //query all GridCells and set their state to a particular state
            //for the test to work, regular GridCellStateSetter has to be disabled

            foreach ((var gridCell, Entity gridCellEntity) in SystemAPI.Query<RefRW<GridCell>>().WithEntityAccess()) {
                ecb.AddComponent(gridCellEntity, new GridCellVisualState { Value = newState });
                //UnityEngine.Debug.Log(entityManager.GetName(gridCellEntity)+ " <new state in bytes>: " + newState);
            }
        }
    }
}