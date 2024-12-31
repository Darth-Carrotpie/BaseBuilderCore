Requirements to run:
- Install Blender
- Tick "Entities" subscene when first starting "BuildSample-ForceDirected"
- Install deps.

Dependencies:
- [BovineLabs.Core](https://github.com/tertle/com.bovinelabs.core) (core)
- TextMesh Pro (optional)
- [EditorCools](https://github.com/datsfain/EditorCools/tree/main) (optional)


# To Do - QoL:
- Remove Editor Cools code and add as a package dep instead
- Add CI/CD on push to release a package
- Improve existing code extensibility

#To Do - Features (Forces):
- Add global centrific force, to center the graph naturally on zero
- Add centrific force for each hex, for the nodes to eventually settle in a hexagonal pattern
- Add minimum range force, to prevent nodes from overlapping or coming too close each

#To Do - Features (Visuals):
- Add hex-grid based links to visualize node connections in runtime
- Split to transition and stable states, to allow for more complex animations of the links and nodes
