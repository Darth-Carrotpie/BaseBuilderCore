//By default all groups and systems in SimulationSystemGroup
using Unity.Entities;

[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class ProducerSystemsGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class ConsumerSystemsGroup : ComponentSystemGroup{ }

