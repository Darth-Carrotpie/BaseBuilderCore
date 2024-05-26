using BovineLabs.Core.Spatial;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct MatchBuildingToCellSystem : ISystem
{
    private PositionBuilder positionBuilderCells;
    private PositionBuilder positionBuilderNodes;
    private SpatialMap<SpatialPosition> spatialMap;
    EntityQuery gridCellQuery;
    EntityQuery nodeQuery;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridGeneratorConfig>();

        this.gridCellQuery = SystemAPI.QueryBuilder().WithAll<GridCell>().WithAll<LocalTransform>().WithAll<DynamicBuffer<NeighbourBuilding>>().Build();
        this.nodeQuery = SystemAPI.QueryBuilder().WithAll<ForceNode>().WithAll<LocalTransform>().Build();
        this.positionBuilderCells = new PositionBuilder(ref state, gridCellQuery);
        this.positionBuilderNodes = new PositionBuilder(ref state, nodeQuery);

        const int size = 4096;
        const int quantizeStep = 16;

        this.spatialMap = new SpatialMap<SpatialPosition>(quantizeStep, size);
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
        this.spatialMap.Dispose();
    }
    //alterantive - find closest using physics https://forum.unity.com/threads/best-way-to-find-closest-target-entity.672436/
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        this.gridCellQuery = SystemAPI.QueryBuilder().WithAll<GridCell>().WithAll<LocalTransform>().Build();
        this.nodeQuery = SystemAPI.QueryBuilder().WithAll<ForceNode>().WithAll<LocalTransform>().Build();

        state.Dependency = this.positionBuilderCells.Gather(ref state, state.Dependency, out NativeArray<SpatialPosition> cellsPositions);
        state.Dependency = this.spatialMap.Build(cellsPositions, state.Dependency);
        state.Dependency = this.positionBuilderNodes.Gather(ref state, state.Dependency, out NativeArray<SpatialPosition> nodesPositions);
        //state.Dependency = this.spatialMap.Build(nodesPositions, state.Dependency); //do not add these, because they will then be probably itterated through the SpacialHashMap

        GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /*
        //query force nodes
        foreach (var (node, nodeLocalToWorld, nodeEntity) in SystemAPI.Query<ForceNode, LocalToWorld>().WithEntityAccess()){
            //query hex cells
            foreach (var (hexCell, cellLocalToWorld, cellEntity) in SystemAPI.Query<RefRW<GridCell>, LocalToWorld>().WithEntityAccess()){
                //if there is a force node within range of hex cell apply building type
                float distance = math.length(nodeLocalToWorld.Position - cellLocalToWorld.Position);
                if(distance < config.hexRadius){
                    hexCell.ValueRW.building = node.buildingRepr;
                }
            }
        }
        //then clear the ones which did not hacve an in-range
        */

        // The entities in this will match the indices from the spatial map
        var cellEntities = this.gridCellQuery.ToEntityListAsync(state.WorldUpdateAllocator, state.Dependency, out var cellDependency);
        var nodeEntities = this.nodeQuery.ToEntityListAsync(state.WorldUpdateAllocator, state.Dependency, out var nodeDependency);
        state.Dependency = cellDependency;
        state.Dependency = nodeDependency;

        new FindNeighbourJob
        {
            CellEntities = cellEntities.AsDeferredJobArray(),
            NodeEntities = nodeEntities.AsDeferredJobArray(),
            CellsPositions = cellsPositions,
            NodesPositions = nodesPositions,
            SpatialMap = this.spatialMap.AsReadOnly(),
        }.ScheduleParallel(this.nodeQuery); //we input a query to pass on to Execute

        //then use the spacial map to itterate through neighbours

    }
    // Find and store all other entities within 10 of each other
    [BurstCompile]
    private partial struct FindNeighbourJob : IJobEntity
    {
        private const float Radius = 1.5f;

        [ReadOnly]
        public NativeArray<Entity> CellEntities;
        [ReadOnly]
        public NativeArray<Entity> NodeEntities;

        [ReadOnly]
        public NativeArray<SpatialPosition> NodesPositions;
        [ReadOnly]
        public NativeArray<SpatialPosition> CellsPositions;

        [ReadOnly]
        public SpatialMap.ReadOnly SpatialMap;

        //what does Execute take in??? Which localTransforms are these?
        //this Query contains ForceNodes, they do not need neighbour
        //need to switch it, so that DynamicBuffer<NeighbourBuilding> is a component on CellGrid instead of ForceNode
        private void Execute(Entity entity, in LocalTransform localTransform, DynamicBuffer<NeighbourBuilding> neighbours)
        {
            //so neighbours exist and are accessible globally? i.e. in-between frames?
            //easy to check that, just query them from another ISystem and DebugLog
            //if yes, to what entities are they added?
            neighbours.Clear();

            // Find the min and max boxes
            // The boxes are of size Radius, 
            var min = this.SpatialMap.Quantized(localTransform.Position.xz - Radius);
            var max = this.SpatialMap.Quantized(localTransform.Position.xz + Radius);

            for (var j = min.y; j <= max.y; j++)
            {
                for (var i = min.x; i <= max.x; i++)
                {
                    var hash = this.SpatialMap.Hash(new int2(i, j));
                    //how does it get out the index?
                    if (!this.SpatialMap.Map.TryGetFirstValue(hash, out int item, out var it))
                    {
                        continue;
                    }

                    do
                    {
                        //"entity" here is constant, it will always be a node
                        //then that means the index which is being operated has to be CellGrid, because
                        //there will be a lot more CellGrid objects than ForceNodes
                        var otherEntity = this.CellEntities[item];

                        // Don't add ourselves
                        if (otherEntity.Equals(entity))
                        {
                            continue;
                        }

                        var otherPosition = this.CellsPositions[item].Position;

                        // The spatialmap serves as the broad-phase but most of the time we still need to ensure entities are actually within range
                        if (math.distancesq(localTransform.Position.xz, otherPosition.xz) <= Radius * Radius)
                        {
                            //Add to output into a list of components
                            neighbours.Add(new NeighbourBuilding { Entity = otherEntity });
                        }
                    }
                    //iterates the indexes via this? Perhaps interates indexes of all items while iterator returns smth
                    while (this.SpatialMap.Map.TryGetNextValue(out item, ref it));
                }

            }
        }
    }

}

//this shouldnt be neighrbour building, instead either Repr Building or LocalGridCell
//Then another System would match update GridCell type when queryring Nodes.
public struct NeighbourBuilding : IBufferElementData
{
    public Entity Entity;
}