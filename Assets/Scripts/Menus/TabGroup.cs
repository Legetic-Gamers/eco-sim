using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<Button> buttons;
    public List<GameObject> pages;

    public int selectedTab = 0;
    public Color selected;
    public Color unselected;

    private void Start()
    {
        OnTabChanged(buttons[selectedTab]);
    }


    public void OnTabChanged(Button button)
    {
        selectedTab = buttons.IndexOf(button);
        ResetTabs();
        button.image.color = selected;
        ShowTab(selectedTab);
    }

    public void ResetTabs()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != selectedTab)
            {
                buttons[i].image.color = unselected;
            }
        }
    }

    public void ShowTab(int selectedPageIndex)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == selectedPageIndex)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }
}
