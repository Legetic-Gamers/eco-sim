using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataCollection;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Draws a graph in the graphContainer.
/// Use function ShowGraph to draw graph, and DestroyGraph to erase it.
/// </summary>

public class Window_Graph : MonoBehaviour
{

    [SerializeField] private int dotSize = 5;
    [SerializeField] Color lineColor = new Color(1, 1, 1, .5f);
    [SerializeField] float lineWidth = 2f; // line connecting dots
    
    [SerializeField] private float yBufferTop = 1.2f;
    [SerializeField] private int firstX = 0;
    [SerializeField] private int gridCountY = 10;
    private static int _gridCountX = 10;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private float windowGraphSizeX = 1000;
    [SerializeField] private float windowGraphSizeY = 700;
    [SerializeField] private float graphContainerSizeX = 720;
    [SerializeField] private float graphContainerSizeY = 405;

    private RectTransform window_graph;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;



    private void Awake()
    {
        window_graph = GetComponent<RectTransform>();
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        DataHandler dh = FindObjectOfType<DataHandler>();
        SetSize();
        //dh.Display += ShowGraph;
        

    }
    
    private float x = 0;
    float delta = 1f;
    private int count = 0;
    List<int> testlist = new List<int>();

    private void SetSize()
    {
        window_graph.sizeDelta = new Vector2(windowGraphSizeX, windowGraphSizeY);
        graphContainer.sizeDelta = new Vector2(graphContainerSizeX, graphContainerSizeY);
        dashTemplateX.sizeDelta = new Vector2(graphContainerSizeY + 2, 1);
        dashTemplateY.sizeDelta = new Vector2(graphContainerSizeX + 2, 1);
    }

    public static int GetGridCountX()
    {
        return _gridCountX;
    }

    public static void SetGridCountX(int x)
    {
        _gridCountX = x;
    }
    
    private void Update()
    {
        x += Time.deltaTime;
        if (x > delta && count <= 40)
        {
            DestroyGraph();
            x = 0;
            int r = Mathf.RoundToInt(Random.Range(0f, 20f + count));
            ShowGraph(testlist);
            testlist.Add(r);
            count++;
        }
        
        
    }


    // Draws entire graph.
    void ShowGraph(List<int> valueList)
    {
        if (valueList.Count == 0)
            return;
        DestroyGraph();
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;

        AddGridX(valueList, graphWidth);
        AddGridY(valueList, graphHeight);
        DrawCurve(valueList, sizeDelta);
        
    }
    
    // Destroys the previous graph.
    

    public void DestroyGraph()
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }

        gameObjectList.Clear();
    }
    

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        // create circle object, make it child of graph container, set its position in graph container.

        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = lineColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
        rectTransform.anchorMin = Vector2.zero; 
        rectTransform.anchorMax = Vector2.zero; 
        return gameObject;
    }


    
    
    // Draws lines between points.
    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = lineColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchoredPosition = dotPositionA + dir * distance / 2f; // center of two points A, B.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle); // rotate connection line to angle between a and b
        return gameObject;

    }



    // Draws the grid and labels of the X-axis.
    private void AddGridX(List<int> valueList, float graphWidth)
    {

        int separatorCount = _gridCountX;
        int numberOfValues = valueList.Count;
        int truncateFactor = (int)Math.Ceiling((numberOfValues)*1f / separatorCount);
        float xDelta = graphWidth / numberOfValues;
        int count = firstX;

        foreach (int value in valueList)
        {
            float xPosition = (count-firstX) * xDelta;

            if (xPosition > graphWidth)
                break;
            
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true); 
            labelX.anchoredPosition = new Vector2(xPosition, -7f); 
            labelX.GetComponent<Text>().text = count.ToString();
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true); 
            dashX.anchoredPosition = new Vector2(xPosition, -2f); 
            gameObjectList.Add(dashX.gameObject);
            
            count += truncateFactor;
        }

    }

    // Draws the grid of the Y-axis, as well as the labels of the Y-axis.
    void AddGridY(List<int> valueList, float graphHeight)
    {
        float yMax = valueList.Max() * yBufferTop;
        int separatorCount = gridCountY;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true); 
            float normalizedValue = (i * 1f) / separatorCount;
            labelY.anchoredPosition = (new Vector2(-10f, normalizedValue * graphHeight));
            labelY.GetComponent<Text>().text =
                (yMax * normalizedValue).ToString("0.0"); 
            gameObjectList.Add(labelY.gameObject);


            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true); 
            dashY.anchoredPosition = new Vector2(-2f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    // Draws the curve of the graph.
    private void DrawCurve(List<int> valueList, Vector2 graphSize)
    {
        float graphHeight = graphSize.y;
        float graphWidth = graphSize.x;
        int numberOfValues = valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        float yMax = valueList.Max() * yBufferTop;
        int count = firstX;

        GameObject lastCircleGameObject = null;

        foreach (int value in valueList)
        {
            float xPosition = (count-firstX) * xDelta;
            float yPosition = (value / yMax) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);

            if (lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(
                    lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }

            lastCircleGameObject = circleGameObject;
            count++;
        }

    }
}
