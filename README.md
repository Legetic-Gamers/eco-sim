[![Video of the project](https://img.youtube.com/vi/X1XuxMXvc0k/maxresdefault.jpg)](https://www.youtube.com/watch?v=X1XuxMXvc0k)

# Interactive Ecosystem Simulator

The unity project belonging to the bachelor thesis "Interactive Ecosystem Simulator -
Applying Reinforcement Learning, Genetic Algorithms and Procedural Content Generation to Create a Graphical and Interactive Simulator" at Chalmers.

The thesis can be found [here](https://odr.chalmers.se/items/d935561e-3a82-4ffc-8130-db4aabc5c54c) and a video of the project can be found [here](https://www.youtube.com/watch?v=X1XuxMXvc0k)

## Authors

- Johan Atterfors
- [Carl Holmberg](https://github.com/cajoho99)
- Alexander Huang
- Robin Karhu
- Brage Lae
- Alexander Larsson Vahlberg

---

## Building considirations

### To run without error:

- Add Empty game object with a data handler script attached.
- Add Empty game object with TickEventPublisher script.
- Ask Brage: Add a graph
- Attach an AnimalSelectionPanel.prefab to AnimalSelectPanel in the cameras CameraController script. (AnimalSelectionPanel should be added to the main Canvas gameobject)

#### Animal gameobject

- Tag: Animal, Layer: Target
- Animal Controller script (e.g. BearController, DeerController etc.)
- Field Of Fiew script
- Hearing Ability script
- Nav Mesh Agent
- Animator
- Transform
- Character Controller

#### Water gameobject

- Tag: Water, Layer: Target
- Collider of some sort (so that animal's raycast can hit the object)

#### Plant gameObject

- Tag: Plant, Layer: Target
- Collider of some sort (so that animal's raycast can hit the object)
- Plant Controller script

#### Render pipeline (to get cel-shading and outline to work)

- To ensure that the Light Weight Render Pipeline is used, check projectSettings/Graphics, and set the object at the top (under Scriptable Render Pipeline Settings) as LWRP (found in Rendering Pipeline folder).
- If LWRP is in use, but a material is pink, ensure its shader is using Shader Graphs/Toon Cel-Shader (except plane, which has to use Universal Render Pipeline - Simple Lit, instead).

#### Cel-shading

- To change the effect of the cel-shading, each material (found in Rendering Pipeline/Materials) has four sliders and two colors in the inspector.
- Shadow Size and ShadowBlend affect the internal shadows on the object. Both affect shadow strength, but ShadowBlend also adds a bit of diffusion to the shadow. Can also change the color of the shadow.
- Rim light adds a very faint shine effect when viewed at an angle, and has a dithering effect. Lighter rim light color is more visible than dark.

#### Outline

- To change the thickness of the outline affecting all objects, go to RenderingPipeline/Shaders/OutlineToon_SobelBlit.
