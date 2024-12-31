using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using Unity.Collections;

namespace BaseBuilderCore {
    [UpdateInGroup(typeof(ProducerSystemsGroup))]
    public partial struct BuildOrderToPositionProducerSystem : ISystem, ISystemStartStop {
        private StateModel impl;
        EntityManager entityManager;

        [BurstCompile]
        public void OnStartRunning(ref SystemState state) {
            this.impl = new StateModel(ref state, ComponentType.ReadWrite<GridCellVisualState>(), ComponentType.ReadWrite<GridCellVisualStatePrevious>());
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnStopRunning(ref SystemState state) {
            this.impl.Dispose(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
            if (order.ValueRW.classValue == BuildingType.None) return;

            Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();

            var allOrdersAtPosTemp = new NativeList<BuildOrderAtPosition>(Allocator.Temp);
            var builtOrdersAtPosTemp = new NativeList<BuildOrderAtPosition>(Allocator.Temp);
            DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

            //First we check for marker of first click and create a first order
            foreach ((var localTransform, var cell, var gridCell, Entity gridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithAll<MarkedForLinkStart>().WithEntityAccess()) {
                BuildOrderAtPosition newBOatPosition = new BuildOrderAtPosition {
                    isFirst = true,
                    buildOrder = order.ValueRW,
                    position = localTransform.Position,
                    buildingProduced = Entity.Null,
                    forceNodeProduced = Entity.Null,
                };
                allOrdersAtPosTemp.Add(newBOatPosition);
            }

            //Then query for the rest of selected cells, doesnt matter if they have a building or not, create an order
            foreach ((var localTransform, var cell, var gridCell, Entity gridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithNone<MarkedForLinkStart>().WithEntityAccess()) {
                BuildOrderAtPosition newBOatPosition = new BuildOrderAtPosition {
                    isFirst = false,
                    buildOrder = order.ValueRW,
                    position = localTransform.Position,
                    buildingProduced = Entity.Null,
                    forceNodeProduced = Entity.Null,
                };
                allOrdersAtPosTemp.Add(newBOatPosition);
            }

            //Lastly query for the selected cells, which do have a building on them already assigned via a node
            //This Query does not include non-building/non-foreNode items, therefore we create another temp list

            foreach ((var selectedLocalTransform, var cell, var gridCell, Entity selectedGridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithEntityAccess()) {
                foreach ((ForceNode forceNode, DynamicBuffer<GridCellArea> dynBuffer, Entity nodeEntity) in SystemAPI.Query<ForceNode, DynamicBuffer<GridCellArea>>().WithEntityAccess()) {
                    if (forceNode.buildingRepr == Entity.Null) continue;

                    //check their ref to current grid cell
                    //this uses grid map indexing, to increase map indexing speed by 10X
                    Entity gridCellEntity = Entity.Null;
                    GridCellArea gca = dynBuffer.AsNativeArray().FirstOrDefault();
                    gridCellEntity = gca.GridCellEntity;

                    if (gridCellEntity == selectedGridCellEntity) {
                        foreach (BuildOrderAtPosition orderAtPos in allOrdersAtPosTemp) {
                            if (f3Equals(orderAtPos.position, selectedLocalTransform.Position)) {
                                BuildOrderAtPosition newBOatPosition = new BuildOrderAtPosition {
                                    isFirst = orderAtPos.isFirst,
                                    buildOrder = orderAtPos.buildOrder,
                                    position = orderAtPos.position,
                                    buildingProduced = forceNode.buildingRepr,
                                    forceNodeProduced = nodeEntity,
                                };
                                builtOrdersAtPosTemp.Add(newBOatPosition);
                            }
                        }
                    }
                }
                entityManager.SetComponentEnabled<SelectedCellTag>(selectedGridCellEntity, false);
            }
            // Iterate through both lists and determine which orders to add to the Buffer,
            // this way a new one will not be created, instead only a link will be created
            buildOrdersAtPos.AddRange(builtOrdersAtPosTemp.AsArray());
            NativeList<BuildOrderAtPosition> trimmed = GetDifference(allOrdersAtPosTemp, builtOrdersAtPosTemp, Allocator.Temp);
            buildOrdersAtPos.AddRange(trimmed.AsArray());

            order.ValueRW.classValue = BuildingType.None;
            builtOrdersAtPosTemp.Dispose();
            allOrdersAtPosTemp.Dispose();
        }

        public bool f3Equals(float3 a, float3 b) {
            return Vector3.SqrMagnitude(a - b) < 0.0001;
        }

        private bool IsEqual(BuildOrderAtPosition a, BuildOrderAtPosition b) {
            return f3Equals(a.position, b.position);
        }

        public NativeList<BuildOrderAtPosition> GetDifference(
        NativeList<BuildOrderAtPosition> listA,
        NativeList<BuildOrderAtPosition> listB,
        Allocator allocator) {
            NativeList<BuildOrderAtPosition> differenceList = new NativeList<BuildOrderAtPosition>(allocator);

            for (int i = 0; i < listA.Length; i++) {
                BuildOrderAtPosition elementA = listA[i];
                bool found = false;

                // Check if elementA exists in listB
                for (int j = 0; j < listB.Length; j++) {
                    if (IsEqual(elementA, listB[j])) {
                        found = true;
                        break;
                    }
                }

                // If not found in listB, add to differenceList
                if (!found) {
                    differenceList.Add(elementA);
                }
            }

            return differenceList;
        }
    }
}