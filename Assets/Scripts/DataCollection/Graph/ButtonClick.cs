
using System;
using JetBrains.Annotations;
using UnityEngine;


public class ButtonClick : Window_Graph
{

    public static event EventHandler OnButtonPressed;

    public void ButtonPressed()
    {
        OnButtonPressed?.Invoke(this, EventArgs.Empty);
    }


    public void OnButtonIncTruncate()
    {
        if (TruncateFactor < ValueList.Count)
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
