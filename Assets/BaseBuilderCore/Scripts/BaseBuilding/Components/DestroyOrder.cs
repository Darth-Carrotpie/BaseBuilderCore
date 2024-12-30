using Unity.Entities;
using Unity.Mathematics;
namespace BaseBuilderCore {
    public struct DestroyOrder : IComponentData {
        public bool Value;
    }
    public struct DestroyOrderAtPosition : IBufferElementData {
        public float3 position;
        public bool buildingDestroyed;
        public bool forceNodeDestroyed;
        public bool forceLinkDestroyed;
        public int TTL;
    }
}