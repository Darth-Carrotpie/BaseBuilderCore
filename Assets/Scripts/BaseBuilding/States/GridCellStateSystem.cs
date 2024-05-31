using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
public partial struct GridCellStateSystem : ISystem, ISystemStartStop
{
    private StateModel impl;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        this.impl = new StateModel(ref state, ComponentType.ReadWrite<GridCellVisualState>(), ComponentType.ReadWrite<GridCellVisualStatePrevious>());
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
        this.impl.Dispose(ref state);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator); // This can't be CommandBuffer System if prefered
        this.impl.Run(ref state, ecb);
        ecb.Playback(state.EntityManager);
    }
}