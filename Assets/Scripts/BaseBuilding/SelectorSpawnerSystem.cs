using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;
using static Unity.Physics.CompoundCollider;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;


public struct SelectorStateData : IComponentData
{
    public Entity SelectionUI;
}
[UpdateAfter(typeof(UnitSelectionSystem))]
[UpdateAfter(typeof(GenerateGridSystem))]
//[UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct SelectorSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridGeneratorConfig>();

    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        //EndSimulationEntityCommandBufferSystem.Singleton endSimEcb  = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        BeginSimulationEntityCommandBufferSystem.Singleton endSimEcb  = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = endSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        GridGeneratorConfig gridGeneratorConfig = SystemAPI.GetSingleton<GridGeneratorConfig>(); //for some reason i need to get it every frame

        foreach ((RefRW<SelectableCellTag> cell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>>().WithAll<SelectedCellTag>().WithNone<SelectorStateData>().WithEntityAccess())
        {
            //UnityEngine.Debug.Log(gridGeneratorConfig.cellPrefabEntity);
            var selectionUI = ecb.Instantiate(gridGeneratorConfig.cellSelectorPrefabEntity);
            ecb.AddComponent<Parent>(selectionUI);
            ecb.SetComponent(selectionUI, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            var newSelectionStateData = new SelectorStateData()
            {
                SelectionUI = selectionUI
            };
            ecb.AddComponent<SelectorStateData>(selectedEntity);
            ecb.SetComponent(selectedEntity, newSelectionStateData);
            ecb.SetComponent(selectionUI, new Parent { Value = selectedEntity });
            //ecb.SetComponent(selectionUI, World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalToWorld>(selectedEntity));
            /*ecb.AddComponent(selectionUI, new LocalToWorld
            {
                Value = new float4x4(quaternion.identity, float3.zero)
            });*/

            //ecb.SetName(selectionUI, "Selector_of_"+selectedEntity);
        }
    }
}