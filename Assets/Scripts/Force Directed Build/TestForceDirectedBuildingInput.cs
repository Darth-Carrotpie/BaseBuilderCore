using Unity.Entities;
using UnityEngine;

public class TestForceDirectedBuildingInput : MonoBehaviour
{
    [EditorCools.Button]
    public void GenerateSpawnAmountForceNodes()
    {
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
