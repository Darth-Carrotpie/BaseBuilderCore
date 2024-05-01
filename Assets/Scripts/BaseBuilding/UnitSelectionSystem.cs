using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using RaycastHit = Unity.Physics.RaycastHit;


//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] //for some reason this make mouse input not register more than half of the time
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class UnitSelectionSystem : SystemBase
{
    UnityEngine.Camera _mainCamera;
    [ReadOnly] CollisionWorld _collisionWorld;
    [ReadOnly] PhysicsWorldSingleton _physicsWorld;

    protected override void OnCreate()
    {
        _mainCamera = UnityEngine.Camera.main;
        RequireForUpdate<PhysicsWorldSingleton>();
        RequireForUpdate<BlockClickThrough>();
        RequireForUpdate<GridGeneratorConfig>();
    }

    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<GridGeneratorConfig>();

    }
    public void OnStartRunning(ref SystemState state)
    {
        _physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        _collisionWorld = _physicsWorld.PhysicsWorld.CollisionWorld;
    }
    protected override void OnUpdate()
    {
        if (SystemAPI.GetSingleton<BlockClickThrough>().Value) //this component follow if mouse if hovering over an UI element and wont allow to click!
            return;
        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            //UnityEngine.Debug.Log("button clicked");
            if (!UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftShift))
            {
                DeselectAllUnits();
            }

            SelectSingleUnit();
        }
    }
    void SelectSingleUnit()
    {
        //UnityEngine.Debug.Log("SelectSingleUnit");

        var ray = _mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        var rayStart = ray.origin;
        var rayEnd = ray.GetPoint(Camera.main.farClipPlane);
        Entity hitEntity = Raycast(rayStart, rayEnd);
        UnityEngine.Debug.Log(hitEntity);
        if (hitEntity != null)
        {
            if (EntityManager.HasComponent<SelectableCellTag>(hitEntity))
            {
                //bool isSelected = EntityManager.IsComponentEnabled<SelectedCellTag>(hitEntity);
                EntityManager.SetComponentEnabled<SelectedCellTag>(hitEntity, true);// !isSelected);
            }
        }
    }

    Entity Raycast(float3 rayStart, float3 rayEnd)
    {
        PhysicsWorld world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        //PhysicsWorld world = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;
        //CollisionFilter filter = world.GetCollisionFilter(ceIdx);
        CollisionFilter filter = new CollisionFilter() //this is problematic. throws null error if there are anything here. Apparently not. Changed how Im getting world and it works now.!
        {
            GroupIndex = 0,
            BelongsTo = (uint)CollisionLayers.Selection,
            CollidesWith = (uint)(CollisionLayers.Ground | CollisionLayers.Units),
        };


        RaycastInput raycastInput = new RaycastInput()
        {
            Start = rayStart,
            End = rayEnd,
            Filter = filter
        };
        UnityEngine.Debug.DrawLine(rayStart, rayEnd, UnityEngine.Color.red, 20f);
        bool hasHit = world.CastRay(raycastInput, out var hit);
        if (hasHit)
        {
            return hit.Entity;
        }
        return Entity.Null;
        //return _collisionWorld.CastRay(raycastInput, out hit);
    }

    void DeselectAllUnits()
    {
        //UnityEngine.Debug.Log("Deselect");
        foreach ((RefRO<SelectorStateData> selectionStateData, Entity selectedEntity) in SystemAPI.Query<RefRO<SelectorStateData>>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            EntityManager.SetComponentEnabled<SelectedCellTag>(selectedEntity, false);
        }
    }
}

public struct SelectableCellTag : IComponentData, IEnableableComponent
{
}

public struct SelectedCellTag : IComponentData, IEnableableComponent
{
}