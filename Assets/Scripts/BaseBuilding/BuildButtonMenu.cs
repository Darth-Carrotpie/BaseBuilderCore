using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class BuildButtonMenu : MonoBehaviour
{
    EntityManager entityManager;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void BuildClearButton()
    {
        NewBuildOrder(BuildingClass.Clear);
    }
    public void BuildWorkshopButton()
    {
        NewBuildOrder(BuildingClass.Workshop);
    }
    public void BuildKitchenButton()
    {
        NewBuildOrder(BuildingClass.Kitchen);
    }
    public void BuildBarracksButton()
    {
        NewBuildOrder(BuildingClass.Barracks);
    }
    public void BuildArenaButton()
    {
        NewBuildOrder(BuildingClass.Arena);
    }

    void NewBuildOrder(BuildingClass newOrderClass)
    {
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();

        BuildOrder newOrder = new BuildOrder { Value = newOrderClass };

        entityManager.SetComponentData<BuildOrder>(orderEntity, newOrder);
        //entityManager.GetComponentData<BuildOrder>(orderEntity).Value = BuildingClass.Kitchen;
    }
}
