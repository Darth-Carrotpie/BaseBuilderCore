using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct LinkCreateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GraphConfig>();
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
        //RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>();
        GraphConfig graphConfig = SystemAPI.GetSingleton<GraphConfig>();

        foreach ((RefRW<SelectableCellTag> cell, RefRW<GridCell> gridCell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>, RefRW<GridCell>>().WithAll<SelectedCellTag>().WithEntityAccess()){
            foreach ((MarkedForLinkStart linkStartMarker, Entity linkStart) in SystemAPI.Query<MarkedForLinkStart>().WithEntityAccess()){
                var linkEntity = ecb.Instantiate(graphConfig.linkPrefabEntity);
                ecb.AddComponent<Parent>(linkEntity);
                ecb.SetComponent(linkEntity, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                ecb.SetComponent(linkEntity, new Parent { Value = graphConfig.configEntity });

                //ecb.SetComponent(linkEntity, new GridCell { cellUI = building });
                string hexName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(selectedEntity);
                UnityEngine.Debug.Log("Built a {" + " link " + "} at: " + hexName);
                //ecb.SetName(selectionUI, "Selector_of_"+selectedEntity);
                ecb.SetComponentEnabled<SelectedCellTag>(selectedEntity, false); // this unselects the looped entities
            }
        }
    }
}