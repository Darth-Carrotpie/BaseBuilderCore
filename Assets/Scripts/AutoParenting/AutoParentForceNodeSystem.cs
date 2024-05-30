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

//[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AutoParentForceNodeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<BuildOrder>();
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
        EndSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        //using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        //{

            //var singletonQuery = state.GetEntityQuery(ComponentType.ReadOnly<HexCellParent>());

            var singletonQuery = SystemAPI.QueryBuilder().WithAll<NodesParent>().Build();
            if (singletonQuery.IsEmpty) return;
            Entity parentEntity = singletonQuery.GetSingletonEntity();

            foreach ((RefRW<ForceNode> gridCell, Entity entity) in SystemAPI.Query<RefRW<ForceNode>>().WithNone<Parent>().WithNone<PreviousParent>().WithNone<ExcludeFromAutoParenting>().WithEntityAccess())
            {
                ecb.AddComponent<Parent>(entity);
                ecb.SetComponent(entity, new Parent { Value = parentEntity });
                parentCheck = true;
            }
            if (parentCheck)
            {
                //state.Enabled = false;
                UnityEngine.Debug.Log("parented HexCells");
            }

            //ecb.Playback(state.EntityManager);
        //}
    }
}