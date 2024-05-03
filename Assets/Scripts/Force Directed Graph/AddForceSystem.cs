using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AddForceSystem : ISystem
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
        foreach ((RefRO<PhysicsMass> physicsMass, RefRW<PhysicsVelocity> physicsVelocity, Entity nodeEntity) in SystemAPI.Query<RefRO<PhysicsMass>, RefRW<PhysicsVelocity>>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            var forceVector = (float3)Vector3.forward * graphConfig.pullStrength * deltaTime;
            physicsVelocity.ValueRW.ApplyLinearImpulse(physicsMass.ValueRO, forceVector);
        }
    }
}
