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
        NewBuildOrder(BuildingType.Clear);
    }
    public void BuildWorkshopButton()
    {
        NewBuildOrder(BuildingType.Workshop);
    }
    public void BuildKitchenButton()
    {
        NewBuildOrder(BuildingType.Kitchen);
    }
    public void BuildBarracksButton()
    {
        NewBuildOrder(BuildingType.Barracks);
    }
    public void BuildArenaButton()
    {
        NewBuildOrder(BuildingType.Arena);
    }

    void NewBuildOrder(BuildingType newOrderClass)
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
