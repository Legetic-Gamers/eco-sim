
using System;
using UnityEngine;
using UnityEngine.UI;


public class ButtonClick : Window_Graph
{

    public Action<int,int, int, int> GetListType;
    public static event EventHandler OnButtonReDraw;
   
    public static event EventHandler OnButtonSize;
    public static event EventHandler OnButtonPopulation;
    public static event EventHandler OnButtonTwoGraphs;
    public static event EventHandler OnButtonOneGraph;

    public Sprite square;
    public Sprite check;
    public Button buttonOne;
    public Button buttonTwo;

    public void ButtonReDraw()
    {
        OnButtonReDraw?.Invoke(this, EventArgs.Empty);
    }


    public void ButtonSize()
    {
        OnButtonSize?.Invoke(this, EventArgs.Empty);
    }


    public void ButtonPopulation()
    {
        OnButtonPopulation?.Invoke(this, EventArgs.Empty);
    }
    
    public void ButtonOne()
    {
        IsGraphOne = !IsGraphOne;
        OnButtonOneGraph?.Invoke(this, EventArgs.Empty);
        GetListType?.Invoke(0,0,0,0);
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
    }

    public void ButtonTwo()
    {
        IsGraphTwo = !IsGraphTwo;
        OnButtonTwoGraphs?.Invoke(this, EventArgs.Empty);
        GetListType?.Invoke(1,0,0,0);
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
