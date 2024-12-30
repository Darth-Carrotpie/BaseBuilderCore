using Unity.Entities;
namespace BaseBuilderCore {
    public struct BlockClickThrough : IComponentData {
        public bool Value;
    }
    public struct MarkedForDestruction : IComponentData { }
}
