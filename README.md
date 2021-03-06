# eco-sim

### To run without error:
- Add Empty game object with a data handler script attached.
- Add Empty game object with TickEventPublisher script.
- Ask Brage: Add a graph
- Attach an AnimalSelectionPanel.prefab to AnimalSelectPanel in the cameras CameraController script. (AnimalSelectionPanel should be added to the main Canvas gameobject)

#### Animal gameobject
* Tag: Animal, Layer: Target
* Animal Controller script (e.g. BearController, DeerController etc.)
* Field Of Fiew script
* Hearing Ability script
* Nav Mesh Agent script
* Animator script
* Transform
* Character Controller script

#### Water gameobject
* Tag: Water, Layer: Target
* Collider of some sort (so that animal's raycast can hit the object)

#### Plant gameObject
* Tag: Plant, Layer: Target
* Collider of some sort (so that animal's raycast can hit the object)
* Plant Controller script

#### Render pipeline (to get cel-shading and outline to work)
* To ensure that the Light Weight Render Pipeline is used, check projectSettings/Graphics, and set the object at the top (under Scriptable Render Pipeline Settings) as LWRP (found in Rendering Pipeline folder).
* If LWRP is in use, but a material is pink, ensure its shader is using Shader Graphs/Toon Cel-Shader (except plane, which has to use Universal Render Pipeline - Simple Lit, instead).

#### Cel-shading
* To change the effect of the cel-shading, each material (found in Rendering Pipeline/Materials) has four sliders and two colors in the inspector. 
* Shadow Size and ShadowBlend affect the internal shadows on the object. Both affect shadow strength, but ShadowBlend also adds a bit of diffusion to the shadow. Can also change the color of the shadow.
* Rim light adds a very faint shine effect when viewed at an angle, and has a dithering effect. Lighter rim light color is more visible than dark.

#### Outline
* To change the thickness of the outline affecting all objects, go to RenderingPipeline/Shaders/OutlineToon_SobelBlit.
