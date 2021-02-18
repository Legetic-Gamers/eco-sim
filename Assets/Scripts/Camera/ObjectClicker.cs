using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
    [SerializeField]
    private AnimalSelectPanel animalSelectPanel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Variable to store the hit
            RaycastHit hit;
            // Shoot ray from mouseposition in relative position to the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If ray != null, out hit (store data in hit)
            if (Physics.Raycast(ray, out hit, 100f))
            {
                // If transform of the hit is not null
                if (hit.transform != null)
                {
                    // Print the name of the object
                    //PrintName(hit.transform.gameObject);
                    // Method to handle the hit
                    HandleHit(hit.transform.gameObject);

                }
            }
        }
    }

    private void HandleHit(GameObject gameObject)
    {
        // See if the gameobject has animalcontroller, if so; we want to show panel with traits
        AnimalController animalController = gameObject.GetComponent<AnimalController>();
        if (animalController != null && animalController?.animal?.traits != null)
        {
            animalSelectPanel.SetTraitText(animalController.animal.traits, gameObject.name);
        }
        else
        {
            animalSelectPanel.hide();
        }
    }
    
    private void PrintName(GameObject go)
    {
        Debug.Log("THIS IS THE CLICKED OBJECT" + go);
    }
    
    
}
