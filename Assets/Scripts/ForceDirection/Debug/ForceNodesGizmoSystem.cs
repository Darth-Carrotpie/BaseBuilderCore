using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class ForceNodesGizmoSystem : SystemBase
{
    protected override void OnStartRunning()
    {
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

        Entities
        .ForEach((in LocalToWorld localToWorld, in ForceNode forceNode) =>
        {
            // Accessing position from LocalToWorld component
            float3 position = localToWorld.Position;

            // Drawing sphere at the position
            Gizmos.DrawSphere(position, 0.1f);
            Debug.Log(forceNode);
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
            Run();

        /*Entities.
            WithName("GizmoDrawSystem_Fire").
            ForEach(
                    (in LocalToWorld ltw, in Fire fire) =>
                    {
                        Gizmos.DrawSphere(ltw.Position, math.log(fire.fuel / 300 + 1));
                    }
                   ).
            Run();*/

        // More ForEach queries as needed ...
    }

    protected override void OnUpdate()
    {
        // Intentionally empty.
    }
}