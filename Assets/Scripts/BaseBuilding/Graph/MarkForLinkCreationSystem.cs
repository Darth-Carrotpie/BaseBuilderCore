using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct MarkForLinkCreationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LinkOrder>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        RefRW <LinkOrder> order = SystemAPI.GetSingletonRW<LinkOrder>(); //for some reason i need to get it every frame, otherwise null error
        if (order.ValueRW.startLinking == false) return;

        foreach ((RefRW<SelectableCellTag> cell, RefRW<GridCell> gridCell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>, RefRW<GridCell>>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            ecb.AddComponent<MarkedForLinkStart>(selectedEntity);
            ecb.SetComponentEnabled<SelectedCellTag>(selectedEntity, false);
            UnityEngine.Debug.Log("mark and unselect");
        }
        order.ValueRW.startLinking = false;
    }
}