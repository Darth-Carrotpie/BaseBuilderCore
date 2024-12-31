using Unity.Entities;

namespace BaseBuilderCore {
    public struct ForceNode : IComponentData {
        public Entity buildingRepr;
    }
    public struct ForceLink : IComponentData {
        public Entity nodeA;
        public Entity nodeB;
    }
}