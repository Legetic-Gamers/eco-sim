using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour 
{
    public event EventHandler OnRedraw();

    // Add updateEvent upon button pressed so that graph redraws whenever a button has been pressed.

    public void OnButtonClick()
    {
        int i = Window_Graph.GetGridCountX();
        i++;
        Window_Graph.SetGridCountX(i);
    }
}
