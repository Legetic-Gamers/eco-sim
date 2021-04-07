
using System;
using UnityEngine;
using UnityEngine.UI;


public class ButtonClick : Window_Graph
{

    public Action<int,int, int, int> GetListType;
    public static event EventHandler OnButtonReDraw;


    public Dropdown dropdownSpecies1;
    //public Dropdown dropdownSpecies2;
    public Dropdown dropdownTrait1;
    //public Dropdown dropdownTrait2;
    //public Dropdown dropDownDataType1;
    //public Dropdown dropDownDataType2;


 

    public Sprite square;
    public Sprite check;
    public Button buttonOne;
    public Button buttonTwo;



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

    public void DropDown()
    {
        int species1 = dropdownSpecies1.GetComponent<Dropdown>().value;
        //int species2 = dropdownSpecies2.GetComponent<Dropdown>().value;
        int trait1 = dropdownTrait1.GetComponent<Dropdown>().value;
        //int trait2 = dropdownTrait2.GetComponent<Dropdown>().value;
        //int dataType1 = dropDownDataType1.GetComponent<Dropdown>().value;
        //int dataType2 = dropDownDataType2.GetComponent<Dropdown>().value;

        GetListType(0, species1, trait1, 0);
        //GetListType(1, species2, trait2, dataType2);
        
        OnButtonReDraw?.Invoke(this, EventArgs.Empty);


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
