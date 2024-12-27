using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct DestroyBuildings : ISystem
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
        //write a similar prompt to copilot like I did for the DestroyLinks from LinkFactory

        //no way to find a building entity because there are no refs inside of it and it is not placed physically (only data layer)
    }
}