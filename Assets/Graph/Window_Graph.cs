using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// source: https://www.youtube.com/watch?v=ck72XNhxeS0&list=PLzDRvYVwl53v5ur4GluoabyckImZz3TVQ&index=1

public class Window_Graph : MonoBehaviour{

    private int circleSize = 11;
    Color lineColor = new Color(1,1,1, .5f); //rgb white, 50% transparent
    float lineWidth = 3f; // line connecting dots
    Vector2 origo = new Vector2(0, 0);

    // circleSprite is for drawing circles in graph

    [SerializeField] private Sprite circleSprite;  
    private RectTransform graphContainer;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        List<int> testList = new List<int>() {5, 35, 45, 65, 54, 32, 1, 40};
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
        int count = 0;
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = 50f;  //distance between points in x-axis.
        float yMax = 100f; // top of graph

        GameObject lastCircleGameObject = null;

        foreach (int value in valueList) {
            float xPosition = (1 + count) * xSize;
            float yPosition = (value / yMax) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            
            // if it is not the first point in graph: create a connection between this point and the previous.
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
            count++;

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
