using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

public class MakeLinksButton : MonoBehaviour
{
    EntityManager entityManager;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void MakeLinks()
    {
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(LinkOrder)).GetSingletonEntity();
        LinkOrder newBlockState = new LinkOrder { startLinking = true };
        entityManager.SetComponentData<LinkOrder>(orderEntity, newBlockState);
    }
}
