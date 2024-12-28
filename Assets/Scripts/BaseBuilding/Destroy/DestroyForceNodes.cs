using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct DestroyForceNodes : ISystem
{
    //EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        Entity orderEntity = SystemAPI.GetSingletonEntity<DestroyOrder>();
        DynamicBuffer<DestroyOrderAtPosition> destroyOrderAtPos = SystemAPI.GetBuffer<DestroyOrderAtPosition>(orderEntity);
        
        //containx hex grid size:
        GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();


        foreach ((LocalTransform localTransform, ForceNode node, DynamicBuffer <GridCellArea> dynBuffer, Entity entity) in SystemAPI.Query<LocalTransform, ForceNode, DynamicBuffer<GridCellArea>>().WithEntityAccess())
        {
            for (int i = destroyOrderAtPos.Length-1; i >=0; i--) {
                UnityEngine.Debug.Log("Query ForceNode at position");

                DestroyOrderAtPosition order = destroyOrderAtPos[i];
                if (order.forceNodeDestroyed == true) continue;
                if (order.forceLinkDestroyed == false) continue;

                //check their ref to current grid cell
                GridCellArea gca = dynBuffer.AsNativeArray().FirstOrDefault();
                Entity gridCellEntity = gca.GridCellEntity;
                //check gridCellEntity position instead of node position!
                LocalTransform gridCellTransform = SystemAPI.GetComponent<LocalTransform>(gridCellEntity);

                // Check if the entity's position correlates with the order's position
                //the problem is with positioning, should instead get the closest node somehow more deterministic!
                if (math.distance(gridCellTransform.Position, order.position) < config.hexRadius/2f)
                {
                    // Destroy the ForceNode if it correlates with the order
                    ecb.AddComponent(entity, new MarkedForDestruction { }); //this will tag to destroy entity later
                    // Put tag for destruction to the data layer
                    ecb.AddComponent(node.buildingRepr, new MarkedForDestruction { }); //this will tag to destroy entity later
                    //ecb.DestroyEntity(entity);
                    UnityEngine.Debug.Log("Destroyed ForceNode at position: " + localTransform.Position);

                    //change order state to reflect the destruction of force node
                    var newDo = order;
                    newDo.forceNodeDestroyed = true;
                    destroyOrderAtPos[i] = newDo;
                }
            }
        }
    }
}
