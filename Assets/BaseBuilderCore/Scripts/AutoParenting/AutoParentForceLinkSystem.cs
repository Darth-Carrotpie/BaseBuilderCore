using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BaseBuilderCore {
    //[BurstCompile]
    [UpdateInGroup(typeof(ParentingSystemGroup))]
    public partial struct AutoParentForceLinkSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            //state.RequireForUpdate<BuildOrder>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            bool parentCheck = false;
            EndSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
            EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            var singletonQuery = SystemAPI.QueryBuilder().WithAll<LinksParent>().Build();
            if (singletonQuery.IsEmpty) return;
            Entity parentEntity = singletonQuery.GetSingletonEntity();

            foreach ((RefRW<ForceLink> link, Entity entity) in SystemAPI.Query<RefRW<ForceLink>>().WithNone<Parent>().WithNone<PreviousParent>().WithNone<ExcludeFromAutoParenting>().WithEntityAccess()) {
                ecb.AddComponent<Parent>(entity);
                ecb.SetComponent(entity, new Parent { Value = parentEntity });
                parentCheck = true;
            }
            if (parentCheck) {
                //UnityEngine.Debug.Log("parented ForceLinks");
            }
        }
    }
}