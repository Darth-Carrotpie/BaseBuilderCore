using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BaseBuilderCore {
    public class MouseInputController : MonoBehaviour {
        public InputAction Input;

        Entity entity;
        World world;

        public int clickIndex;

        private void OnEnable() {
            Input.started += OnMouseClicked;
            Input.Enable();

            world = World.DefaultGameObjectInjectionWorld;
        }

        private void OnMouseClicked(InputAction.CallbackContext ctx) {
            Vector2 screenPosition = ctx.ReadValue<Vector2>();
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            Debug.Log(ray.GetPoint(Camera.main.farClipPlane));

            if (world.IsCreated && !world.EntityManager.Exists(entity)) {
                entity = world.EntityManager.CreateEntity();
                world.EntityManager.AddBuffer<LeftMouseClickInput>(entity);
            }

            CollisionFilter filter = new CollisionFilter() //this is problematic. throws null error if there are anything here. Apparently not. Changed how Im getting world and it works now.!
            {
                GroupIndex = 0,
                BelongsTo = (uint)CollisionLayers.Selection,
                CollidesWith = (uint)(CollisionLayers.Ground | CollisionLayers.Units),
            };

            RaycastInput input = new RaycastInput() {
                Start = ray.origin,
                Filter = filter,
                End = ray.GetPoint(Camera.main.farClipPlane)
            };
            UnityEngine.Debug.DrawLine(ray.origin, ray.GetPoint(Camera.main.farClipPlane), UnityEngine.Color.blue, 20f);
            clickIndex++;
            world.EntityManager.GetBuffer<LeftMouseClickInput>(entity).Add(new LeftMouseClickInput() { Value = input, index = clickIndex });
        }

        private void OnDisable() {
            Input.started -= OnMouseClicked;
            Input.Disable();

            if (world.IsCreated && world.EntityManager.Exists(entity)) {
                world.EntityManager.DestroyEntity(entity);
            }

        }
    }
}