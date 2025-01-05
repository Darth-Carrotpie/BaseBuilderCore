using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    public class PanelClickThroughPrevention : MonoBehaviour {
        EntityManager entityManager;

        private void Start() {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }


        public void OnPointerEnter() {
            DisableClickThrough(true);
        }
        public void OnPointerExit() {
            DisableClickThrough(false);
        }

        void DisableClickThrough(bool state) {
            Entity orderEntity = entityManager.CreateEntityQuery(typeof(BlockClickThrough)).GetSingletonEntity();
            BlockClickThrough newBlockState = new BlockClickThrough { Value = state };
            entityManager.SetComponentData<BlockClickThrough>(orderEntity, newBlockState);
        }
    }
}
