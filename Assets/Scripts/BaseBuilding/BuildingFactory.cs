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
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
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
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        if (order.ValueRW.classValue == BuildingType.None) return;

        //create the building entity
        Entity newBuilding = ecb.CreateEntity();

        ecb.AddComponent(newBuilding, new Building { buildingType = order.ValueRW.classValue });
        string bName = order.ValueRW.classValue.ToString();
        ecb.SetName(newBuilding, "New_"+ bName);

        order.ValueRW.classValue = BuildingType.None;
        UnityEngine.Debug.Log("building created");

        //add LocalToWorld to be able to parent later
        ecb.AddComponent<LocalToWorld>(newBuilding);
    }
}