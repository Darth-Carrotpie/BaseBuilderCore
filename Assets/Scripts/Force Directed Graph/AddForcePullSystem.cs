using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AddForcePullSystem : ISystem
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
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var deltaTime = SystemAPI.Time.DeltaTime;

        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        //foreach (var (nodeLocalToWorld, physicsMass, physicsVelocity, node, nodeEntity) in SystemAPI.Query<LocalToWorld, RefRO<PhysicsMass>, RefRW<PhysicsVelocity>, ForceNode>().WithEntityAccess())
        foreach (var (link, otherEntity) in SystemAPI.Query<ForceLink>().WithEntityAccess())
        {
            Entity nodeAEntity = link.nodeA;   
            Entity nodeBEntity = link.nodeB; 

            if (!entityManager.HasComponent<PhysicsMass>(nodeAEntity) || !entityManager.HasComponent<PhysicsMass>(nodeBEntity))
            {
                Debug.Log("returning because no mass");
                continue; // Skip entities without PhysicsMass components 
            }

            PhysicsMass physicsMassA = entityManager.GetComponentData<PhysicsMass>(nodeAEntity);
            PhysicsVelocity physicsVelA = entityManager.GetComponentData<PhysicsVelocity>(nodeAEntity);
            LocalToWorld localToWorldA = entityManager.GetComponentData<LocalToWorld>(nodeAEntity);

            PhysicsMass physicsMassB = entityManager.GetComponentData<PhysicsMass>(nodeBEntity);
            PhysicsVelocity physicsVelB = entityManager.GetComponentData<PhysicsVelocity>(nodeBEntity);
            LocalToWorld localToWorldB = entityManager.GetComponentData<LocalToWorld>(nodeBEntity);

            var pullDirA = math.normalize(localToWorldB.Position - localToWorldA.Position);
            var pullDirB = math.normalize(localToWorldA.Position - localToWorldB.Position);
            var distance = math.length(localToWorldA.Position - localToWorldB.Position);

            /*if (distance < 1f)
            {
                continue;
            }*/
            var distrtanceStrength = graphConfig.offsetDistance * distance;

            var forceVectorA = pullDirA * distrtanceStrength * graphConfig.pullStrength * deltaTime;
            var forceVectorB = pullDirB * distrtanceStrength * graphConfig.pullStrength * deltaTime;

            physicsVelA.ApplyLinearImpulse(in physicsMassA, in forceVectorA);
            physicsVelB.ApplyLinearImpulse(physicsMassB, forceVectorB);

            // Write back the modified PhysicsVelocity components into the ECS system
            entityManager.SetComponentData(nodeAEntity, physicsVelA);
            entityManager.SetComponentData(nodeBEntity, physicsVelB);
        }
    }
}
