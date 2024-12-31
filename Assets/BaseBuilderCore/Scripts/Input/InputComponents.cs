using Unity.Entities;
using Unity.Physics;

namespace BaseBuilderCore {
    public struct LeftMouseClickInput : IBufferElementData {
        public RaycastInput Value;
        internal int index;
    }
}