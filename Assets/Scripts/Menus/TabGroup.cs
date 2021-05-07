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
        InitTabs();
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
                //pages[i].SetActive(true);
                Show(pages[i].gameObject);
            }
            else
            {
                //pages[i].SetActive(false);
                Hide(pages[i].gameObject);
            }
        }
    }

    private void InitTabs()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            //Start up tab
            pages[i].SetActive(true);
            
            //Hide Children
            Hide(pages[i]);
        }
    }

    //Hides all the children of the tab.
    private void Hide(GameObject page)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[page.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in page.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now hide them
        foreach (GameObject child in allChildren)
        {
            child.gameObject.SetActive(false);
        }

    }
    
    //Shows all the children of the tab.
    private void Show(GameObject page)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[page.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in page.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now hide them
        foreach (GameObject child in allChildren)
        {
            child.gameObject.SetActive(true);
        }
    }
}
