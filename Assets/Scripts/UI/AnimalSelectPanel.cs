using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSelectPanel : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text maxSize;
    [SerializeField] private Text maxEnergy;
    [SerializeField] private Text maxHealth;
    [SerializeField] private Text movementSpeed;
    [SerializeField] private Text ageLimit;
    [SerializeField] private Text temperatureResist;
    [SerializeField] private Text desirability;
    [SerializeField] private Text viewAngle;
    [SerializeField] private Text viewRadius;
    [SerializeField] private Text hearingRadius;


    public void SetTraitText(Traits traits, string name)
    {
        gameObject.SetActive(true);
        title.text = name;
        maxSize.text = "maxSize: " + traits.maxSize;
        maxEnergy.text = "maxEnergy: " + traits.maxEnergy;
        maxHealth.text = "maxHealth: " + traits.maxHealth;
        movementSpeed.text = "movementSpeed: " + traits.movementSpeed;
        ageLimit.text = "ageLimit: " + traits.ageLimit;
        temperatureResist.text = "temperatureResist: " + traits.temperatureResist;
        desirability.text = "desirability: " + traits.desirability;
        viewAngle.text = "viewAngle: " + traits.viewAngle;
        viewRadius.text = "viewRadius: " + traits.viewRadius;
        hearingRadius.text = "hearingRadius: " +traits.hearingRadius;

    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
    
}
