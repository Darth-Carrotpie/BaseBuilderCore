using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct LinkFactory : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    { 
        var ecbSystem = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
        Entity orderEntity = SystemAPI.GetSingletonEntity<BuildOrder>();
        if (SystemAPI.TryGetSingletonEntity<MarkedNodeForLinkStart>(out var entity))
        {
            Entity markStartEntity = SystemAPI.GetSingletonEntity<MarkedNodeForLinkStart>();

            DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = SystemAPI.GetBuffer<BuildOrderAtPosition>(orderEntity);

            if (buildOrdersAtPos.Length <= 0) return;
            ForceDirGraphConfig config = SystemAPI.GetSingleton<ForceDirGraphConfig>();

            //prepare a list for fresh links
            NativeList<Entity> linksEnts = new NativeList<Entity>(Allocator.Temp);
            //create a ForceLink entity for each order:
            for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--)
            {
                UnityEngine.Debug.Log("-------------------New order check for ForceLink!!-------------");
                BuildOrderAtPosition bo = buildOrdersAtPos[i];
                if (bo.forceLinkProduced != Entity.Null || bo.forceNodeProduced == Entity.Null) return;
                //find closest node
                //Entity closestNode = Entity.Null;
                //float closestDist = 9999f;
                float3 nodeAPos = entityManager.GetComponentData<LocalToWorld>(bo.forceNodeProduced).Position;
                if (bo.forceNodeProduced == markStartEntity)
                {
                    //perhaps create a dummy link?
                    //var newBoSelf = bo;
                    //newBoSelf.forceLinkProduced = newLinkEntity;
                    //buildOrdersAtPos[i] = newBo;
                    UnityEngine.Debug.Log("Skipping self!!");
                    continue;
                }
                /*foreach((var node, var localTr, var nodeEntity) in SystemAPI.Query<ForceNode, LocalTransform>().WithEntityAccess())
                {
                    if (nodeEntity == markStartEntity)
                    {
                            UnityEngine.Debug.Log("Skipping self!!");
                        continue;
                    }
                    UnityEngine.Debug.Log("Checking node dist: "+entityManager.GetName(nodeEntity));
                    float3 direction = nodeAPos - localTr.Position;
                    //float testDist = math.length(nodeAPos-localTr.Position);
                    /*float distance = (float)math.sqrt(direction.x * direction.x + direction.z * direction.z);


                    if (closestNode != Entity.Null)
                    {
                        UnityEngine.Debug.Log("distance check: "+ distance + " vs "+ closestDist);
                        if(distance < closestDist)
                        {
                            UnityEngine.Debug.Log("Found Closer!!");
                            closestDist = distance;
                            closestNode = nodeEntity;
                        } else
                        {
                            UnityEngine.Debug.Log("too far!!");
                        }
                    } else
                    {
                            UnityEngine.Debug.Log("set initial params:" + distance + " vs " + closestDist);
                        closestDist = distance;
                        closestNode = nodeEntity;
                    }
                }*/
                //If there are no closest no candidates, do nothing 
                //if (closestNode == Entity.Null) {continue; }
                //create ForceLink component 
                ForceLink newLink = new ForceLink
                {
                    nodeA = bo.forceNodeProduced,
                    nodeB = markStartEntity,
                };

                //check if a link of this kind already exists, then skip to another Order
                Entity newLinkEntity = ForceLinkExists(ref state, newLink, linksEnts);
                if (newLinkEntity == Entity.Null)
                {
                    //otherwise continue with link entity creation
                    newLinkEntity = entityManager.Instantiate(config.linkEntityPrefab);
                    ecb.SetComponent(newLinkEntity, newLink);
                    linksEnts.Add(newLinkEntity);
                    string bName = bo.buildOrder.classValue.ToString();
                    ecb.SetName(newLinkEntity, "ForceLink_" + bName + "_" + markStartEntity);

                    UnityEngine.Debug.Log("ForceLink created: " + bName + "_" + markStartEntity);
                }

                var newBo = bo;
                newBo.forceLinkProduced = newLinkEntity;

                buildOrdersAtPos[i] = newBo;
            }
            linksEnts.Dispose();

            ecbSystem.AddJobHandleForProducer(state.Dependency);
        }
    }
    public Entity ForceLinkExists(ref SystemState state, ForceLink linkToCheck, NativeList<Entity> linksEnts)
    {
        Entity output = Entity.Null;
        foreach ((var link, var entity) in SystemAPI.Query<ForceLink>().WithEntityAccess())
        {
            if (link.nodeB == linkToCheck.nodeB && link.nodeA == linkToCheck.nodeA) { output = entity; }
            if (link.nodeA == linkToCheck.nodeB && link.nodeB == linkToCheck.nodeA) output = entity;
        }
        foreach (var linkEnt in linksEnts)
        {
            ForceLink link = entityManager.GetComponentData<ForceLink>(linkEnt);
            if (link.nodeB == linkToCheck.nodeB && link.nodeA == linkToCheck.nodeA) { output = linkEnt; }
            if (link.nodeA == linkToCheck.nodeB && link.nodeB == linkToCheck.nodeA) output = linkEnt;
        }
        return output;
    }
}