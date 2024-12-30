using Unity.Entities;
namespace BaseBuilderCore {

    public struct BuildingsParent : IComponentData { }
    public struct HexCellParent : IComponentData { }
    public struct NodesParent : IComponentData { }
    public struct LinksParent : IComponentData { }

    public struct ExcludeFromAutoParenting : IComponentData { }
}