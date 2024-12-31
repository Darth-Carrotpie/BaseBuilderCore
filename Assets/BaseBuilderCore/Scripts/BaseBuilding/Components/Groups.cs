//By default all groups and systems in SimulationSystemGroup
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class ProducerSystemsGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class ConsumerSystemsGroup : ComponentSystemGroup{ }

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class GraphicalUpdatesSystemGroup : ComponentSystemGroup { }

[UpdateBefore(typeof(PhysicsSystemGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ForcesSystemGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(TransformSystemGroup))]
public partial class ParentingSystemGroup : ComponentSystemGroup { }