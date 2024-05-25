using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Search;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct AutoParentingCreateParents : ISystem
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
        bool parentCheck = false;
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        CreateParentBuildings<BuildingsParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem
        CreateParentHexCell<HexCellParent>(ecb, state); // to do: change to generic CreateParentEntityIfNotExists<T>, example attempt that couldnt make it work is in AutoParentBuildingsSystem

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
    void CreateParentBuildings<T>(EntityCommandBuffer ecb, SystemState state)
    {
        var query = SystemAPI.QueryBuilder().WithAll<BuildingsParent>().Build();

        if (query.IsEmpty)
        {
            Entity newParentEntity = ecb.CreateEntity();
            ecb.AddComponent<BuildingsParent>(newParentEntity);
            ecb.SetName(newParentEntity, typeof(T).Name);
            UnityEngine.Debug.Log("parent created: Buildings!");
            ecb.AddComponent<LocalToWorld>(newParentEntity);
        }
    }
    void CreateParentHexCell<T>(EntityCommandBuffer ecb, SystemState state)
    {
        var query = SystemAPI.QueryBuilder().WithAll<HexCellParent>().Build();

        if (query.IsEmpty)
        {
            Entity newParentEntity = ecb.CreateEntity();
            ecb.AddComponent<HexCellParent>(newParentEntity);
            ecb.SetName(newParentEntity, typeof(T).Name);
            UnityEngine.Debug.Log("parent created: HexCells!");
            ecb.AddComponent<LocalToWorld>(newParentEntity);
        }
    }
}