using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSelectPanel : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text size;
    [SerializeField] private Text maxEnergy;
    [SerializeField] private Text maxHealth;
    [SerializeField] private Text movementSpeed;
    [SerializeField] private Text ageLimit;
    [SerializeField] private Text temperatureResist;
    [SerializeField] private Text desirability;
    [SerializeField] private Text viewAngle;
    [SerializeField] private Text viewRadius;
    [SerializeField] private Text hearingRadius;

    private void Start()
    {
        Hide();
    }

    public void SetTraitText(Traits traits, string name)
    {
        gameObject.SetActive(true);
        title.text = name;
        size.text = "size: " + traits.size;
        maxEnergy.text = "maxEnergy: " + traits.maxEnergy;
        maxHealth.text = "maxHealth: " + traits.maxHealth;
        movementSpeed.text = "maxSpeed: " + traits.maxSpeed;
        ageLimit.text = "ageLimit: " + traits.ageLimit;
        desirability.text = "desirability: " + traits.desirability;
        viewAngle.text = "viewAngle: " + traits.viewAngle;
        viewRadius.text = "viewRadius: " + traits.viewRadius;
        hearingRadius.text = "hearingRadius: " +traits.hearingRadius;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
