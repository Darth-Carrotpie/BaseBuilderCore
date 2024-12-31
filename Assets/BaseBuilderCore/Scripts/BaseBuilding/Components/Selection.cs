using Unity.Entities;

namespace BaseBuilderCore {
    public struct SelectableCellTag : IComponentData, IEnableableComponent {}

    public struct SelectedCellTag : IComponentData, IEnableableComponent {}

    public struct SelectorStateData : IComponentData {
        public Entity SelectionUI;
    }

    public struct CellSelector : IComponentData { }
}