Installation instructions (Unity 6):
- Install Blender
- Open Unity Package Manager
- Add a scoped registry, first open package manager settings:

	![1](./Instructions/1.png)

- Add scoped registry data:

	![2](./Instructions/2.png)

- Go back go Unity Package Manager and select "My Registries" in the side tab strip
- Select the package and click install. You can chose a particular version if needed.

	![3](./Instructions/3.png)
- Thats it! Enjoy the package. Contributions are welcome.

Dependencies:
- Blender. Required for basic hex model.
- [BovineLabs.Core](https://github.com/tertle/com.bovinelabs.core) (core)
- TextMesh Pro (optional)
- [EditorCools](https://github.com/datsfain/EditorCools/tree/main) (optional)

Instructions to use, example scene:
- Open the example scene.
- Check the subscene checkmark to load it.
- Run it.
- Mouse click to select hex grid objects.
- Click UI buttons to "build" the graph.
- Use Shift+Mouse click to select multiple grid hexes.
- The first selected will have a connection to all the others. This way you can make any amount of connections with a single build.
- Edit the HexGrid component to change the grid size and other parameters.
- Edit prefabs to change visuals.

Instructions to extend:
You can extend using any additional components you wish. Simply create a new component and extend the 

# To Do - QoL:
- [x] Remove Editor Cools code and add as a package dep instead (removed, it is not available as a managed package)
- [ ] Add CI/CD on push to release a package
- [ ] Improve existing code extensibility
- [ ] Remove blender dep
- [ ] Add Tests
- [ ] Make a more gradual namespace separation for the package. I.e. MyPackage.Systems, MyPackage.Authoring, MyPackage.Utilities

#To Do - Features (Forces):
- Add global centrific force, to center the graph naturally on zero
- Add centrific force for each hex, for the nodes to eventually settle in a hexagonal pattern
- Add minimum range force, to prevent nodes from overlapping or coming too close each

#To Do - Features (Visuals):
- Add hex-grid based links to visualize node connections in runtime
- Split to transition and stable states, to allow for more complex animations of the links and nodes
