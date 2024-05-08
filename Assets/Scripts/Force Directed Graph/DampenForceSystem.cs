using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct DampenForceSystem : ISystem
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
        RefRW<ForceDirGraphConfig> graphConfig = SystemAPI.GetSingletonRW<ForceDirGraphConfig>();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var (nodeLocalToWorld, physicsMass, physicsVelocity,  node,  nodeEntity) in SystemAPI.Query<LocalToWorld, RefRO<PhysicsMass>, RefRW<PhysicsVelocity>, ForceNode>().WithEntityAccess()){
            physicsVelocity.ValueRW.Linear = physicsVelocity.ValueRW.Linear  * graphConfig.ValueRW.temperature;
            entityManager.SetComponentData(nodeEntity, physicsVelocity.ValueRW);
        }
        graphConfig.ValueRW.temperature *= graphConfig.ValueRW.coolingFactor;
    }
} 
