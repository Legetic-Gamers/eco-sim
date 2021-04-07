using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonManager : MonoBehaviour
{
    public GameObject WindowButtonsMain;
    public GameObject WindowButtonsPopulation;
    public GameObject WindowButtonsTraits;
    public GameObject WindowButtonsBirthRate;

    public void Back()
    {
        WindowButtonsMain.SetActive(true);
        WindowButtonsPopulation.SetActive(false);
        WindowButtonsTraits.SetActive(false);
        WindowButtonsBirthRate.SetActive(false);
    }

    public void Population()
    {
        WindowButtonsMain.SetActive(false);
        WindowButtonsPopulation.SetActive(true);
    }

    public void Traits()
    {
        WindowButtonsMain.SetActive(false);
        WindowButtonsTraits.SetActive(true);
    }

    public void BirthRate()
    {
        WindowButtonsMain.SetActive(false);
        WindowButtonsBirthRate.SetActive(true);
    }
}
