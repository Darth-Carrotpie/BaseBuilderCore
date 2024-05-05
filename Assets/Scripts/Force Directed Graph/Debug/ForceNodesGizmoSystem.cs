using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class ForceNodesGizmoSystem : SystemBase
{
    EntityManager entityManager;

    protected override void OnStartRunning()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GizmoManager.OnDrawGizmos(DrawGizmos);
    }


    /*public void OnUpdate(ref SystemState state)
    {
        if (UnityEngine.Input.GetKey(KeyCode.Space))
        {
            foreach ((RefRO<LocalTransform> nodeTransform, ForceNode node) in SystemAPI.Query<RefRO<LocalTransform>, ForceNode>())
            {
                Gizmos.DrawSphere(nodeTransform.ValueRO.Position, 0.5f);
            }
        }
    }*/
    private void DrawGizmos()
    {
        // Local reference to EntityManager
        var entityManager = this.entityManager;
        Gizmos.color = Color.green;
        Entities
        .ForEach((in LocalToWorld localToWorld, in ForceNode forceNode) =>
        {
            // Accessing position from LocalToWorld component
            float3 position = localToWorld.Position;

            // Drawing sphere at the position
            Gizmos.DrawSphere(position, 0.2f);
        }).Run();
        Gizmos.color = Color.red;
        Entities
        .ForEach((in LocalToWorld localToWorld, in ForceLink forceLink) =>
        {
            // Get the entities referenced by NodeA and NodeB fields
            Entity nodeAEntity = forceLink.nodeA;
            Entity nodeBEntity = forceLink.nodeB;

            if (entityManager.HasComponent<LocalToWorld>(nodeAEntity) && entityManager.HasComponent<LocalToWorld>(nodeBEntity))
            {
                // Get LocalToWorld components of referenced entities
                LocalToWorld localToWorldA = entityManager.GetComponentData<LocalToWorld>(nodeAEntity);
                LocalToWorld localToWorldB = entityManager.GetComponentData<LocalToWorld>(nodeBEntity);
                // Drawing line at the positions
                Gizmos.DrawLine(localToWorldA.Position, localToWorldB.Position);
            }
        }).Run();
        
        /*foreach ((RefRO<LocalTransform> nodeTransform, ForceNode node) in SystemAPI.Query<RefRO<LocalTransform>, ForceNode>())
        {
            Gizmos.DrawSphere(nodeTransform.ValueRO.Position, 0.5f);
        }*/
        /*Entities.
            //WithName("GizmoDrawSystem_Dead").
            WithAll<ForceNode>().
            ForEach(
                    (in LocalToWorld ltw) => { Gizmos.DrawIcon(ltw.Position + new float3(0, 3, 0), "Dead"); }
                   ).
            Run();*/
    }

    protected override void OnUpdate()
    {
        // Intentionally empty.
    }
}