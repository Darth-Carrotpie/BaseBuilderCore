using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TestForceDirectionInput : MonoBehaviour
{
    EntityManager entityManager;
    Entity testConfigEntity;

    void Start()
    {
    }

    [EditorCools.Button]
    public void GenerateSpawnAmountForceNodes()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager; //have to get EntityManager every time we need to ref it.
        testConfigEntity = entityManager.CreateEntityQuery(typeof(TestForceDirection)).GetSingletonEntity();

        TestForceDirection componentData = entityManager.GetComponentData<TestForceDirection>(testConfigEntity);
        componentData.generateNodes = true;
        entityManager.SetComponentData(testConfigEntity, componentData);
    }
}
