
using System;
using Menus;
using UnityEngine;
using UnityEngine.UI;


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
    //public Dropdown dropdownSpecies2;
    public Dropdown dropdownTrait1;

    public Dropdown dropdownBirthRate;
    //public Dropdown dropdownTrait2;
    //public Dropdown dropDownDataType1;
    //public Dropdown dropDownDataType2;


 

    public Sprite square;
    public Sprite check;
    public Button buttonOne;
    public Button buttonTwo;

    private bool isPerMinute = false;

    public void Start()
    {
        var sgm = FindObjectOfType<ShowGraphManager>();
        if (sgm) sgm.SetDropDownValues += InitDropDownValues;
    }


    private void InitDropDownValues()
    {
        bool r = FindObjectOfType<RabbitController>();
        bool w = FindObjectOfType<WolfController>();
        bool d = FindObjectOfType<DeerController>();
        bool b = FindObjectOfType<BearController>();
        
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
        if (FindObjectOfType<RabbitController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (FindObjectOfType<WolfController>())  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (FindObjectOfType<DeerController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (FindObjectOfType<BearController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        dropdownPopulation.value = 1;
        dropdownPopulation.value = 0;
        DropDownPopulation();
    }

    public void ButtonPopulationMinute()
    {
        isPerMinute = true;
        dropdownPopulation.options.Clear();
        if (FindObjectOfType<RabbitController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (FindObjectOfType<WolfController>())  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (FindObjectOfType<DeerController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (FindObjectOfType<BearController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        dropdownPopulation.value = 1;
        dropdownPopulation.value = 0;
        DropDownPopulation();
    }

    public void DropDownPopulation()
    {
        dropdownPopulation.options.Clear();
        if (!isPerMinute) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "All Animals"});
        if (FindObjectOfType<RabbitController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (FindObjectOfType<WolfController>())  dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (FindObjectOfType<DeerController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (FindObjectOfType<BearController>()) dropdownPopulation.options.Add(new Dropdown.OptionData() {text = "Bear"});
        
        int species = dropdownPopulation.GetComponent<Dropdown>().value;
        {
            GetListPopulationPerGeneration(species);
            xLabel.text = "Generation";
        }

        if (isPerMinute)
        {
            GetListPopulationPerMinute(species);
            xLabel.text = "Time (minute)";
        }

        yLabel.text = dropdownPopulation.GetComponent<Dropdown>().options [species].text + " population";
        
    }

    public void DropDownTrait()
    {
        dropdownSpecies1.options.Clear();
        if (FindObjectOfType<RabbitController>()) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (FindObjectOfType<WolfController>())  dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (FindObjectOfType<DeerController>()) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (FindObjectOfType<BearController>()) dropdownSpecies1.options.Add(new Dropdown.OptionData() {text = "Bear"});
        int species1 = dropdownSpecies1.GetComponent<Dropdown>().value;
        //int species2 = dropdownSpecies2.GetComponent<Dropdown>().value;
        int trait1 = dropdownTrait1.GetComponent<Dropdown>().value;
        //int trait2 = dropdownTrait2.GetComponent<Dropdown>().value;
        //int dataType1 = dropDownDataType1.GetComponent<Dropdown>().value;
        //int dataType2 = dropDownDataType2.GetComponent<Dropdown>().value;

        GetListTrait(0, species1, trait1, 0);
        //GetListType(1, species2, trait2, dataType2);
        
        yLabel.text = dropdownSpecies1.GetComponent<Dropdown>().options [species1].text + " " + dropdownTrait1.GetComponent<Dropdown>().options [trait1].text;
        xLabel.text = "Generation";
    }

    public void DropdownBirthRate()
    {
        dropdownBirthRate.options.Clear();
        if (FindObjectOfType<RabbitController>()) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Rabbit"});
        if (FindObjectOfType<WolfController>())  dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Wolf"});
        if (FindObjectOfType<DeerController>()) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Deer"});
        if (FindObjectOfType<BearController>()) dropdownBirthRate.options.Add(new Dropdown.OptionData() {text = "Bear"});
        int species = dropdownBirthRate.GetComponent<Dropdown>().value;
        GetListBirthRate(species);
        yLabel.text = "Birth rate " + dropdownBirthRate.GetComponent<Dropdown>().options [species].text;
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
