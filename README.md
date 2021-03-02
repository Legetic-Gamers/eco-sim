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

