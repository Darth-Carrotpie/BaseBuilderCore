using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace BaseBuilderCore {
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct AddForcePullSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ForceDirGraphConfig>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            ForceDirGraphConfig graphConfig = SystemAPI.GetSingleton<ForceDirGraphConfig>();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //foreach (var (nodeLocalToWorld, physicsMass, physicsVelocity, node, nodeEntity) in SystemAPI.Query<LocalToWorld, RefRO<PhysicsMass>, RefRW<PhysicsVelocity>, ForceNode>().WithEntityAccess())
            foreach (var (link, otherEntity) in SystemAPI.Query<ForceLink>().WithEntityAccess()) {
                Entity nodeAEntity = link.nodeA;
                Entity nodeBEntity = link.nodeB;

                if (!entityManager.HasComponent<PhysicsMass>(nodeAEntity) || !entityManager.HasComponent<PhysicsMass>(nodeBEntity)) {
                    Debug.Log("returning because no mass");
                    continue; // Skip entities without PhysicsMass components. Also skips ForceLink entities with missing nodes
                }

                PhysicsVelocity physicsVelA = entityManager.GetComponentData<PhysicsVelocity>(nodeAEntity);
                LocalToWorld localToWorldA = entityManager.GetComponentData<LocalToWorld>(nodeAEntity);

                PhysicsVelocity physicsVelB = entityManager.GetComponentData<PhysicsVelocity>(nodeBEntity);
                LocalToWorld localToWorldB = entityManager.GetComponentData<LocalToWorld>(nodeBEntity);

                float3 directionNorm = math.normalize(localToWorldB.Position - localToWorldA.Position);

                float3 direction = localToWorldA.Position - localToWorldB.Position;

                float distance = (float)math.sqrt(direction.x * direction.x + direction.z * direction.z);
                if (distance > 0) {
                    float force = graphConfig.springConstant * (distance);
                    physicsVelA.Linear += directionNorm * force;
                    physicsVelB.Linear -= directionNorm * force;
                    Debug.Log("Pulling by: "+ force);
                }

                // Write back the modified PhysicsVelocity components into the ECS system
                entityManager.SetComponentData(nodeAEntity, physicsVelA);
                entityManager.SetComponentData(nodeBEntity, physicsVelB);
            }
        }
    }
}
