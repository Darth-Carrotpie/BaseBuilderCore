using BovineLabs.Core.Spatial;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct ForceNodeBufferGridCellSystem : ISystem
{
    private PositionBuilder positionBuilderCells;
    private SpatialMap<SpatialPosition> spatialMap;
    EntityQuery gridCellQuery;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridGeneratorConfig>();

        this.gridCellQuery = SystemAPI.QueryBuilder().WithAll<GridCell>().WithAll<LocalTransform>().Build();
        this.positionBuilderCells = new PositionBuilder(ref state, gridCellQuery);

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

        state.Dependency = this.positionBuilderCells.Gather(ref state, state.Dependency, out NativeArray<SpatialPosition> cellsPositions);
        state.Dependency = this.spatialMap.Build(cellsPositions, state.Dependency);

        GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // The entities in this will match the indices from the spatial map
        var cellEntities = this.gridCellQuery.ToEntityListAsync(state.WorldUpdateAllocator, state.Dependency, out var cellDependency);
        state.Dependency = cellDependency;

        new FindNeighbourJob
        {
            Radius = config.hexRadius,
            CellEntities = cellEntities.AsDeferredJobArray(),
            CellsPositions = cellsPositions,
            SpatialMap = this.spatialMap.AsReadOnly(),
        }.ScheduleParallel(); //we input a query to pass on to Execute: this.nodeQuery

    }
    // Find and store all other entities within 'Radius' range of each other 
    [BurstCompile]
    private partial struct FindNeighbourJob : IJobEntity
    {
        public float Radius;

        [ReadOnly]
        public NativeArray<Entity> CellEntities;

        [ReadOnly]
        public NativeArray<SpatialPosition> CellsPositions;

        [ReadOnly]
        public SpatialMap.ReadOnly SpatialMap;

        //what does Execute take in??? Which localTransforms are these?
        //this Query contains ForceNodes, they do not need neighbour
        //need to switch it, so that DynamicBuffer<NeighbourBuilding> is a component on CellGrid instead of ForceNode
        private void Execute(Entity entity, ForceNode node, in LocalTransform localTransform, DynamicBuffer<GridCellArea> neighbours)
        {
            //so neighbours exist and are accessible globally? i.e. in-between frames?
            //easy to check that, just query them from another ISystem and DebugLog
            //if yes, to what entities are they added?
            neighbours.Clear();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

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
                        //"entity" here is constant in while loop, it will always be a node 
                        //then that means the index which is being operated has to be CellGrid, because
                        //there will be a lot more CellGrid objects than ForceNodes
                        var otherEntity = this.CellEntities[item];

                        // Don't add ourselves
                        if (otherEntity.Equals(entity))
                        {
                            continue;
                        }
                        // Don't add a Node by accident
                        if (entityManager.HasComponent<ForceNode>(otherEntity)) 
                        {
                            continue;
                        }
                        var otherPosition = this.CellsPositions[item].Position;

                        // The spatialmap serves as the broad-phase but most of the time we still need to ensure entities are actually within range
                        if (math.distancesq(localTransform.Position.xz, otherPosition.xz) <= Radius * Radius)
                        {
                            //Add to output into a list of components
                            neighbours.Add(new GridCellArea { GridCellEntity = otherEntity });
                            //problem: should be added, but cannot see any change in Inspector and via Debugging Gizmo
                            //it does not update in the inspector, but is actually added, because we can see debug!
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
//Then another System would match update GridCell type when querying Nodes.
public struct GridCellArea : IBufferElementData
{
    public Entity GridCellEntity;
}