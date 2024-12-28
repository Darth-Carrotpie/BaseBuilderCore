using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(ConsumerSystemsGroup))]
public partial struct DestroyAtPosOrderConsumer : ISystem, ISystemStartStop {
    EntityManager entityManager;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state) {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStopRunning(ref SystemState state) {
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state) {
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
        DynamicBuffer<DestroyOrderAtPosition> destroyOrdersAtPos = entityManager.GetBuffer<DestroyOrderAtPosition>(orderEntity);

        for (int i = destroyOrdersAtPos.Length - 1; i >= 0; i--) {
            //remove element from the DynamicBuffer if both conditions are met, thus completing the build cycle: 
            if (destroyOrdersAtPos[i].forceNodeDestroyed == true &&
                destroyOrdersAtPos[i].forceLinkDestroyed == true) {
                destroyOrdersAtPos.RemoveAt(i);
                continue;
            }
            //if there is just a single node, there won't be anything to link to, so we remove the order
            var nodesQuery = entityManager.CreateEntityQuery(typeof(ForceNode)).ToEntityArray(Allocator.TempJob);
            if (nodesQuery.Length <= 1) {
                if (destroyOrdersAtPos[i].forceNodeDestroyed == true) {
                    destroyOrdersAtPos.RemoveAt(i);
                    continue;
                }
            }
            //if TTL is less than zero, remove the order
            if (destroyOrdersAtPos[i].TTL <= 0) {
                destroyOrdersAtPos.RemoveAt(i);
                continue;
            }
        }
    }
}