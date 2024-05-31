using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct BuildFactory : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        //RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

        if (buildOrdersAtPos.Length <= 0) return;

        //create the building entity for each order:
        for(int i = buildOrdersAtPos.Length-1; i >=0; i--) { 
            BuildOrderAtPosition bo = buildOrdersAtPos[i];
            if (bo.buildingProduced != Entity.Null) return;
            Entity newBuilding = ecb.CreateEntity();

            ecb.AddComponent(newBuilding, new Building { buildingType = bo.buildOrder.classValue });
            string bName = bo.buildOrder.classValue.ToString();
            ecb.SetName(newBuilding, "New_" + bName);

            //bo.buildOrder.classValue = BuildingType.None;
            UnityEngine.Debug.Log("building created: " + bName);

            //add LocalToWorld to be able to parent later
            ecb.AddComponent<LocalToWorld>(newBuilding);

            //fill in the order with new building 
            //Problem: this is not working. Building not being set!
            ///bo.buildingProduced = newBuilding;
            ///a fix:
            BuildOrderAtPosition newBo = new BuildOrderAtPosition
            {
                buildOrder = bo.buildOrder,
                buildingProduced = newBuilding,
                forceNodeProduced = bo.forceNodeProduced,
                position = bo.position,
            };
            buildOrdersAtPos.RemoveAt(i);
            buildOrdersAtPos.Add(newBo);
        }
    }
}