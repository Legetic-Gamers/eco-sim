
using System;
using Menus;
using UnityEngine;
using UnityEngine.UI;
using ViewController;


public class ButtonClick : Window_Graph
{

    public Action<int,int, int, int> GetListTrait;
    public Action<int> GetListPopulationPerGeneration;
    public Action<int> GetListPopulationPerMinute;
    
    public Action<int> GetListBirthRate;
    public Action GetListFoodAvailable;
    public static event EventHandler OnButtonReDraw;

    public Text xLabel;
    public Text yLabel;

    public Dropdown dropdownPopulation;
    public Dropdown dropdownSpecies1;
    public Dropdown dropdownTrait1;
    public Dropdown dropdownBirthRate;
    

    public Sprite square;
    public Sprite check;
    public Button buttonOne;
    public Button buttonTwo;

    private bool isPerMinute = false;
    private static bool r;
    private static bool w;
    private static bool d;
    private static bool b;
    
    
    public void Start()
    {
        var sgm = FindObjectOfType<ShowGraphManager>();
        if (sgm) sgm.SetDropDownValues += InitDropDownValues;
    }


    private void InitDropDownValues()
    {
        r = FindObjectOfType<RabbitController>() || FindObjectOfType<MLRabbitSteeringController>() ||
            FindObjectOfType<MLRabbitController>();
        w = FindObjectOfType<WolfController>() || FindObjectOfType<MLWolfController>();
        d = FindObjectOfType<DeerController>() || FindObjectOfType<MLDeerController>();
        b = FindObjectOfType<BearController>() || FindObjectOfType<MLBearController>();
        
        dropdownPopulation.options.Clear();
        if (!isPerMinute) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "All Animals"});
        if (r) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        
        dropdownSpecies1.options.Clear();
        if (r) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Bear"});

        
        dropdownBirthRate.options.Clear();
        if (r) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Bear"});
    }

    public void ButtonReDraw()
    {
        OnButtonReDraw?.Invoke(this, EventArgs.Empty);
    }
    
    
    public void ButtonOne()
    {
        IsGraphOne = !IsGraphOne;
        if (IsGraphOne)
        {
            buttonOne.GetComponent<Image>().sprite = check;
            buttonOne.GetComponent<Image>().color = Color.white;
        }
        else
        {
            buttonOne.GetComponent<Image>().sprite = square;
            buttonOne.GetComponent<Image>().color = Color.grey;
        }
        OnButtonReDraw?.Invoke(this, EventArgs.Empty);
    }

    public void ButtonTwo()
    {
        IsGraphTwo = !IsGraphTwo;
        if (IsGraphTwo)
        {
            buttonTwo.GetComponent<Image>().sprite = check;
            buttonTwo.GetComponent<Image>().color = Color.white;
        }
        else
        {
            buttonTwo.GetComponent<Image>().sprite = square;
            buttonTwo.GetComponent<Image>().color = Color.grey;
        }
        OnButtonReDraw?.Invoke(this, EventArgs.Empty);
    }

    public void ButtonPopulationGeneration()
    {
        isPerMinute = false;
        dropdownPopulation.options.Clear();
        dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "All Animals"});
        if (r) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        
        dropdownPopulation.value = 1;
        dropdownPopulation.value = 0;
        DropDownPopulation();
    }

    public void ButtonPopulationMinute()
    {
        isPerMinute = true;
        dropdownPopulation.options.Clear();
        if (r) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        
        dropdownPopulation.value = 1;
        dropdownPopulation.value = 0;
        DropDownPopulation();
    }

    public void DropDownPopulation()
    {
        dropdownPopulation.options.Clear();
        if (!isPerMinute) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "All Animals"});
        if (r) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (w)  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (d) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (b) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});

        int speciesNumber = dropdownPopulation.GetComponent<Dropdown>().value;
        string species = dropdownPopulation.GetComponent<Dropdown>().options[speciesNumber].text;
        int number = 0;
        if (!isPerMinute)
        {
            switch (species)
            {
                case "All Animals":
                    number = 0;
                    break;
                case "Rabbit":
                    number = 1;
                    break;
                case "Wolf":
                    number = 2;
                    break;
                case "Deer":
                    number = 3;
                    break;
                case "Bear":
                    number = 4;
                    break;
            }
              
            GetListPopulationPerGeneration(number);
            xLabel.text = "Generation";
        }

        if (isPerMinute)
        {
            switch (species)
            {
                case "Rabbit":
                    number = 0;
                    break;
                case "Wolf":
                    number = 1;
                    break;
                case "Deer":
                    number = 2;
                    break;
                case "Bear":
                    number = 3;
                    break;
            }
            GetListPopulationPerMinute(number);
            xLabel.text = "Time (minute)";
        }

        yLabel.text = species + " population";
        
    }

    public void DropDownTrait()
    {
        int speciesNumber = dropdownSpecies1.GetComponent<Dropdown>().value;
        string species = dropdownSpecies1.GetComponent<Dropdown>().options[speciesNumber].text;
        int trait1 = dropdownTrait1.GetComponent<Dropdown>().value;
        int number = 0;
        
        switch (species)
        {
            case "Rabbit":
                number = 0;
                break;
            case "Wolf":
                number = 1;
                break;
            case "Deer":
                number = 2;
                break;
            case "Bear":
                number = 3;
                break;
        }
        
        GetListTrait(0, number, trait1, 0);

        yLabel.text = species + " " + dropdownTrait1.GetComponent<Dropdown>().options [trait1].text;
        xLabel.text = "Generation";
    }

    public void DropdownBirthRate()
    {
        int speciesNumber = dropdownBirthRate.GetComponent<Dropdown>().value;
        string species = dropdownBirthRate.GetComponent<Dropdown>().options [speciesNumber].text;
        int number = 0;
        switch (species)
        {
            case "Rabbit":
                number = 0;
                break;
            case "Wolf":
                number = 1;
                break;
            case "Deer":
                number = 2;
                break;
            case "Bear":
                number = 3;
                break;
        }
        GetListBirthRate(number);
        yLabel.text = "Birth rate " + species;
        xLabel.text = "Time (minute)";
    }

    public void OnButtonFoodAvailable()
    {
        GetListFoodAvailable();
        yLabel.text = "Amount of food";
        xLabel.text = "Time (minute)";
    }
    

    public void OnButtonIncTruncate()
    {
        if (TruncateFactor < (Mathf.Max(List1.Count, List2.Count)))
            TruncateFactor++;
    }

    public void OnButtonDecTruncate()
    {
        if (TruncateFactor > 1)
            TruncateFactor--;
    }

    public void OnButtonIncGridY()
    {
        GridCountY++;
    }

    public void OnButtonDecGridY()
    {
        if (GridCountY > 1)
            GridCountY--;
    }
    
}
