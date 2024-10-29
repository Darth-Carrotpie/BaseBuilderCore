using NUnit.Framework.Internal;
using Unity.Entities;

public partial struct DestroySystem : ISystem
{
    //This system clears up the Entities which were marked by other systems.
    //This pattern prevents race conditions
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((MarkedForDestruction tag, Entity entity) in SystemAPI.Query<MarkedForDestruction>().WithEntityAccess())
        {
            UnityEngine.Debug.Log("destroy entity: " + entity);
            ecb.DestroyEntity(entity);
        }
    }
}