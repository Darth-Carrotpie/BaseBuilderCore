using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AddForcePushSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ForceDirGraphConfig>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    } 
    public void OnUpdate(ref SystemState state)
    {
        ForceDirGraphConfig graphConfig = SystemAPI.GetSingleton<ForceDirGraphConfig>();
        var deltaTime = SystemAPI.Time.DeltaTime;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Get the array of entities with ForceNode component
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ForceNode>());
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
        int sizeEntities = entities.Length; 
        // Check if there are any entities  in the array
        if (entities.Length > 0)
        {
            foreach (var (nodeLocalToWorld, physicsMass, physicsVelocity,  node,  nodeEntity) in SystemAPI.Query<LocalToWorld, RefRO<PhysicsMass>, RefRW<PhysicsVelocity>, ForceNode>().WithEntityAccess())
            {
                foreach (var (otherNode, localToWorld, otherEntity) in SystemAPI.Query<ForceNode, LocalToWorld>().WithEntityAccess())
                {
                    if(nodeEntity == otherEntity) 
                    {
                        //Debug.Log("self, return");  
                        continue;
                    }
                    var pushDir = math.normalize(nodeLocalToWorld.Position - localToWorld.Position);
                    var distance = math.length(nodeLocalToWorld.Position - localToWorld.Position);
                    if(distance < 1f) 
                    {
                        continue;
                    }
                    var pushStrength = graphConfig.offsetDistance / distance;
                    var forceVector = pushDir * pushStrength * graphConfig.pushStrength; 
                    //var forceVector = (float3)Vector3.forward * graphConfig.pullStrength * deltaTime;
                    physicsVelocity.ValueRW.ApplyLinearImpulse(physicsMass.ValueRO, forceVector * deltaTime); 
                }
            }
        }
        entities.Dispose();
    }
}
