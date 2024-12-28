using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[UpdateBefore(typeof(BuildOrderToPositionConsumerSystem))]
[BurstCompile]
public partial struct MarkNodeFirstForLinkSystem : ISystem {
    //This sytem reads BuldOrdersAtPos and does things depending if the order was marked IsFirst or not (selected first in build hexes).

    EntityManager entityManager;

    public void OnCreate(ref SystemState state) {
    }
    public void OnStartRunning(ref SystemState state) {
    }
    public void OnDestroy(ref SystemState state) {
    }
    public void OnUpdate(ref SystemState state) {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

        if (buildOrdersAtPos.Length <= 0) return;

        foreach ((ForceNode node, Entity nodeEntity) in SystemAPI.Query<ForceNode>().WithEntityAccess()) {

            for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--) {
                BuildOrderAtPosition bo = buildOrdersAtPos[i];
                if (bo.forceNodeProduced == Entity.Null) continue;

                if (bo.forceNodeProduced == nodeEntity && bo.isFirst) {
                    //adding a marker component later used for Link systems
                    ecb.AddComponent<MarkedNodeForLinkStart>(nodeEntity);

                    //updatting order to mark as completed (set to false)
                    BuildOrderAtPosition newBo = new BuildOrderAtPosition {
                        isFirst = false,
                        buildOrder = bo.buildOrder,
                        buildingProduced = bo.buildingProduced,
                        forceNodeProduced = bo.forceNodeProduced,
                        position = bo.position,
                    };
                    buildOrdersAtPos.RemoveAt(i);
                    buildOrdersAtPos.Add(newBo);
                }
            }
        }
    }
}
