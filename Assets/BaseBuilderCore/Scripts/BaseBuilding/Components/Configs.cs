using Unity.Entities;

namespace BaseBuilderCore {
    public struct GraphConfig : IComponentData {
        public Entity linkPrefabEntity;
        public float linkWidth;
        public Entity configEntity;
    }
}