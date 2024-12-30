using Unity.Entities;

public struct GridCellVisualState : IComponentData
{
    public byte Value;
}

internal struct GridCellVisualStatePrevious : IComponentData
{
    public byte Value;
}

public enum GridCellVisualStates : byte
{
    None = 0,

    Arena = 250,
    Workshop = 251,
    Kitchen = 252,
    Barracks = 253,
    Clear = 254,
}

public struct ArenaGridCellVisualState : IComponentData { }
public struct WorkshopGridCellVisualState : IComponentData { }
public struct KitchenGridCellVisualState : IComponentData { }
public struct BarracksGridCellVisualState : IComponentData { }
public struct ClearGridCellVisualState : IComponentData { }
