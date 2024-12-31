using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    public class CellSelectorAuthoring : MonoBehaviour {
        public class Baker : Baker<CellSelectorAuthoring> {
            public override void Bake(CellSelectorAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CellSelector());
            }
        }
    }
}
