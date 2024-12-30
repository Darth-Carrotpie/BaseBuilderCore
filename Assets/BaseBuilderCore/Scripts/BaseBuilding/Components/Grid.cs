using Unity.Entities;

namespace BaseBuilderCore {
    public struct GridCell : IComponentData {
        public Entity cellUI;
        public Entity building;
    }

    //this shouldnt be neighbour building, instead either Repr Building or LocalGridCell
    //Then another System would match update GridCell type when querying Nodes.
    public struct GridCellArea : IBufferElementData {
        public Entity GridCellEntity;
    }
}