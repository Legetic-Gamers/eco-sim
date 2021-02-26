using System;
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
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private float windowGraphSizeX = 1000;
    [SerializeField] private float windowGraphSizeY = 700;
    [SerializeField] private float graphContainerSizeX = 720;
    [SerializeField] private float graphContainerSizeY = 405;

    private static List<int> _valueList = new List<int>() {1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 51, 51, 47, 43, 41, 37,31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2, 1};
    private static int _truncateFactor = 1;
    private static int _gridCountY = 10;
    private int firstX = 0;
    
    private RectTransform window_graph;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    
    private List<GameObject> gameObjectList;
    //private List<GameObject> gridXList;
    //private List<GameObject> gridYList;
    //private List<GameObject> dotList;
    //private List<GameObject> lineList;

    public static List<int> ValueList
    {
        get => _valueList;
        set => _valueList = value;
    }

    public static int TruncateFactor
    {
        get => _truncateFactor;
        set => _truncateFactor = value;
    }

    public static int GridCountY
    {
        get => _gridCountY;
        set => _gridCountY = value;
    }

    private void Awake()
    {
        window_graph = GetComponent<RectTransform>();
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        //gridXList = new List<GameObject>();
        DataHandler dh = FindObjectOfType<DataHandler>();

        window_graph.sizeDelta = new Vector2(windowGraphSizeX, windowGraphSizeY);
        graphContainer.sizeDelta = new Vector2(graphContainerSizeX, graphContainerSizeY);
        dashTemplateX.sizeDelta = new Vector2(graphContainerSizeY + 2, 1);
        dashTemplateY.sizeDelta = new Vector2(graphContainerSizeX + 2, 1);
        //dh.Display += ShowGraph;
        ButtonClick.OnButtonPressed += ReDraw;
        ShowGraph();
    }
    
    private float x = 0;
    float delta = .1f;
    private int count = 0;
    List<int> testlist = new List<int>();
    
    
public void Update()
{
    _valueList = testlist;
    x += Time.deltaTime;
    

    if (x > delta && count <= 40)
    {
        DestroyGraph();
        ShowGraph();
        x = 0;
        int r = Mathf.RoundToInt(Random.Range(0f, 20f + count));
        testlist.Add(r);
        count++;
    }
    
    
}

private void ReDraw(object sender, EventArgs e)
{
    Debug.Log("HELLO");
    DestroyGraph();
    ShowGraph();
}

    
    // Draws entire graph.
    private void ShowGraph()
    {
        if (_valueList.Count == 0)
            return;
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;

        AddGridX();
        AddGridY();
        DrawCurve();
        
    }
    
    // Destroys the previous graph.
    

    private void DestroyGraph()
    {
        foreach (GameObject obj in gameObjectList)
        {
            Destroy(obj);
        }

        gameObjectList.Clear();
    }

    //protected void DestroyXGrid()
    //{
    //    foreach (GameObject gameObject in gridXList)
    //    {
    //        Destroy(gameObject);
    //    }
    //    gridXList.Clear();
    //}
    

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
    protected void AddGridX()
    {
        float graphWidth = graphContainer.sizeDelta.x;
        //int separatorCount = _gridCountX;
        int numberOfValues = _valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        int count = firstX;

        foreach (int value in _valueList)
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
            
            count += _truncateFactor;
        }

    }

    // Draws the grid of the Y-axis, as well as the labels of the Y-axis.
    private void AddGridY()
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = _valueList.Max() * yBufferTop;
        int separatorCount = _gridCountY;
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
    private void DrawCurve()
    {
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;
        int numberOfValues = _valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        float yMax = _valueList.Max() * yBufferTop;
        int count = firstX;

        GameObject lastCircleGameObject = null;

        foreach (int value in _valueList)
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
