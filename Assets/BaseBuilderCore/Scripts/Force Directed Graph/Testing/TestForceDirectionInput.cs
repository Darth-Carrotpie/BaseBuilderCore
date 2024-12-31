using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace BaseBuilderCore {
    public class TestForceDirectionInput : MonoBehaviour {

        //if needed you can use EditorCools to make a button in the inspector to call this method.
        //https://github.com/datsfain/EditorCools
        //[EditorCools.Button]

        public void GenerateSpawnAmountForceNodes() {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager; //have to get EntityManager every time we need to ref it.
            Entity testForceDirEntity = entityManager.CreateEntityQuery(typeof(TestForceDirection)).GetSingletonEntity();
            Entity testConfigEntity = entityManager.CreateEntityQuery(typeof(ForceDirGraphConfig)).GetSingletonEntity();
            TestForceDirection testForceDirData = entityManager.GetComponentData<TestForceDirection>(testForceDirEntity);
            ForceDirGraphConfig configData = entityManager.GetComponentData<ForceDirGraphConfig>(testConfigEntity);
            testForceDirData.generateNodes = true;
            configData.temperature = configData.initialTemperature;
            entityManager.SetComponentData(testForceDirEntity, testForceDirData);
            entityManager.SetComponentData(testConfigEntity, configData);
        }
    }
}
