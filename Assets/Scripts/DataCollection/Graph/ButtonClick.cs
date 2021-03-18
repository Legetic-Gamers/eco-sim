
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class ButtonClick : Window_Graph
{

    public static event EventHandler OnButtonReDraw;
   
    public static event EventHandler OnButtonSize;
    public static event EventHandler OnButtonPopulation;
    public static event EventHandler OnButtonTwoGraphs;
    public static event EventHandler OnButtonOneGraph;

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
    }

    public void ButtonTwo()
    {
        IsGraphTwo = !IsGraphTwo;
        OnButtonTwoGraphs?.Invoke(this, EventArgs.Empty);
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
