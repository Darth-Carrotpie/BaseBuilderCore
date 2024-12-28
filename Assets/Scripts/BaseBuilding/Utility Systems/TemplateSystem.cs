using Unity.Entities;

public struct TemplateComponent : IComponentData
{
}

public partial struct TemplateSystem : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<TemplateComponent>();
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

        foreach ((TemplateComponent comp, Entity entity) in SystemAPI.Query<TemplateComponent>().WithEntityAccess())
        {
            //Do something
        }
    }
}