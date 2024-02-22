using System.Collections;
using System.Collections.Generic;
using System.Xml;
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
        BuildOrder orderData = entityManager.GetComponentData<BuildOrder>(orderEntity);

        BuildOrder newOrder = new BuildOrder { classValue = newOrderClass,
            cellPrefabEntityClear = orderData.cellPrefabEntityClear,
            cellPrefabEntityArena = orderData.cellPrefabEntityArena,
            cellPrefabEntityBarracks = orderData.cellPrefabEntityBarracks,
            cellPrefabEntityKitchen = orderData.cellPrefabEntityKitchen,
            cellPrefabEntityWorkshop = orderData.cellPrefabEntityWorkshop,
        };

        entityManager.AddComponentData<BuildOrder>(orderEntity, newOrder);
        //entityManager.GetComponentData<BuildOrder>(orderEntity).Value = BuildingClass.Kitchen;
    }
}
