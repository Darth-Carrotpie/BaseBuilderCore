using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public partial class ActiveGridCellGizmoSystem : SystemBase
{
    EntityManager entityManager;
    protected override void OnCreate()
    {
        base.OnCreate();
        GizmoManager.OnDrawGizmos(DrawGizmos);
    }
    protected override void OnStartRunning()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    protected override void OnDestroy()
    {
        GizmoManager.OnStopDrawGizmos(DrawGizmos);
    }
    private void DrawGizmos()
    {
        // Local reference to EntityManager
        var entityManager = this.entityManager; 
        Gizmos.color = Color.yellow;
        Entities
        .ForEach((in ForceNode forceNode, in DynamicBuffer<GridCellArea> dynBuffer) =>
        {
            float3 position = new float3(0, 0, 0);
            foreach (var cellEntity in dynBuffer)
            {
                //take in first value of neighbours
                if (math.length(position) == 0f)
                {
                    position = entityManager.GetComponentData<LocalToWorld>(cellEntity.GridCellEntity).Position;
                }
            }

            // Drawing sphere at the position 
            if (math.length(position) > 0f)
            {
                Gizmos.DrawWireSphere(position, 0.8f);
            }
        }).Run();

        /*Gizmos.color = Color.red;
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
        }).Run();*/
        
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
        // Intentionally empty. Method must exist.
    }
}