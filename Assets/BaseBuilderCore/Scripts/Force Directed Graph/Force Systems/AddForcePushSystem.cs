using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace BaseBuilderCore {
    [UpdateInGroup(typeof(ForcesSystemGroup))]
    public partial struct AddForcePushSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            //state.RequireForUpdate<ForceDirGraphConfig>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            ForceDirGraphConfig graphConfig = SystemAPI.GetSingleton<ForceDirGraphConfig>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;


            foreach (var (nodeLocalToWorld, physicsMass, physicsVelocity, node, nodeEntity) in SystemAPI.Query<LocalToWorld, RefRO<PhysicsMass>, RefRW<PhysicsVelocity>, ForceNode>().WithEntityAccess()) {
                foreach (var (otherNode, otherlocalToWorld, otherEntity) in SystemAPI.Query<ForceNode, LocalToWorld>().WithEntityAccess()) {
                    float3 direction = math.normalize(otherlocalToWorld.Position - nodeLocalToWorld.Position);
                    float distance = math.length(nodeLocalToWorld.Position - otherlocalToWorld.Position);
                    if (distance > 0 && distance < 10f) {
                        float force = graphConfig.repulsiveForce / math.sqrt(distance);
                        physicsVelocity.ValueRW.Linear -= direction * force;
                    }
                }
            }
        }
    }
}
