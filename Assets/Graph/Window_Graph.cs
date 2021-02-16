using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// source: https://www.youtube.com/watch?v=ck72XNhxeS0&list=PLzDRvYVwl53v5ur4GluoabyckImZz3TVQ&index=1

public class Window_Graph : MonoBehaviour{

    
    //TODO make dynamic x- and y-axis, scale entire graph, make visuals connect better with logic (ie rm hard coded lengths, replace w dynamic var)
    //TODO clean spaghetti, fix constants, clean up comments, test & explore performance

    private int circleSize = 5;
    Color lineColor = new Color(1,1,1, .5f); //rgb white, 50% transparent
    float lineWidth = 1f; // line connecting dots
    Vector2 origo = new Vector2(0, 0);

    // circleSprite is for drawing circles in graph

    [SerializeField] private Sprite circleSprite;  
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();

        List<int> testList = new List<int>() {0, 28, 44, 55, 64, 72, 78, 83, 88};
        ShowGraph(testList);

    }

    // Create circle at given coordinate.

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        // create circle object, make it child of graph container, set its position in graph container.

        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(circleSize, circleSize);
        rectTransform.anchorMin = origo; // anchor point at lower left.
        rectTransform.anchorMax = origo; // anchor point at lower left.
        return gameObject;
    }

    private void ShowGraph(List<int> valueList) {
        int count = 0; // NOTE x starts iterating on 0
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = 50f;  //distance between points in x-axis.
        float yMax = 100f; // top of graph

        GameObject lastCircleGameObject = null;

        foreach (int value in valueList) {
            float xPosition = (count) * xSize;
            float yPosition = (value / yMax) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            
            // if it is not the first point in graph: create a connection between this point and the previous.
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true); //template is not activated by default.
            labelX.anchoredPosition = new Vector2 (xPosition, -7f); //TODO fix const
            labelX.GetComponent<Text>().text = count.ToString();

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true); //template is not activated by default.
            dashX.anchoredPosition = new Vector2 (xPosition, -7f); //TODO fix const


            count++;
        }

        int separatorCount = 10; //TODO fix const
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true); //template is not activated by default.
            float normalizedValue = (i*1f)/separatorCount;
            labelY.anchoredPosition = (new Vector2 (-7f, normalizedValue * graphHeight)); //TODO fix const
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(yMax * normalizedValue).ToString();

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true); //template is not activated by default.
            dashY.anchoredPosition = new Vector2 (-2f, normalizedValue * graphHeight); //TODO fix const
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = lineColor; 
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchorMin = origo;
            rectTransform.anchorMax = origo;
            rectTransform.sizeDelta = new Vector2(distance, lineWidth); 
            rectTransform.anchoredPosition = dotPositionA + dir * distance/2f;  // center of two points A, B.
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.localEulerAngles = new Vector3(0, 0, angle); // rotate connection line to angle between a and b

    }

}
