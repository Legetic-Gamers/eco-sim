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
    //[SerializeField] private float windowGraphSizeX;
    //[SerializeField] private float windowGraphSizeY;
    //[SerializeField] private float graphContainerSizeX;
    //[SerializeField] private float graphContainerSizeY;

    private static List<float> _list1 = new List<float>() {0};
    private static List<float> _list2 = new List<float>() {0};
    private static int _truncateFactor = 1;
    private static int _gridCountY = 12;
    private int firstX = 1;
    private static bool _isGraphOne = false;
    private static bool _isGraphTwo = false;
    


    private RectTransform window_graph;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    
    private List<GameObject> gameObjectList;
    
    

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
    
    public static List<float> List1 
    {
        get => _list1;
        set => _list1 = value;
    }
    
    public static List<float> List2 
    {
        get => _list2;
        set => _list2 = value;
    }

    public static bool IsGraphOne
    {
        get => _isGraphOne;
        set => _isGraphOne = value;
    }

    public static bool IsGraphTwo
    {
        get => _isGraphTwo;
        set => _isGraphTwo = value;
    }


    private void Awake()
    {
        window_graph = this.GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        graphContainer = GameObject.Find("graphContainer").GetComponent<RectTransform>();
        //graphContainer = window_graph.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        
        
        
        DataHandler dh = FindObjectOfType<DataHandler>();
        dh.Display += Draw;
        ButtonClick.OnButtonReDraw += ReDraw;
    }

    private void Start()
    {
        //window_graph.sizeDelta = new Vector2(windowGraphSizeX, windowGraphSizeY);
        //graphContainer.sizeDelta = new Vector2(graphContainerSizeX, graphContainerSizeY);
        //dashTemplateX.sizeDelta = new Vector2(graphContainerSizeY + 2, 1);
        //dashTemplateY.sizeDelta = new Vector2(graphContainerSizeX + 2, 1);
        
    }


    private void Draw(List<float> list1, List<float> list2)
{
    _list1 = list1;
    _list2 = list2;
    if (gameObjectList != null) DestroyGraph(gameObjectList);
    if (_isGraphOne && _isGraphTwo)
    {
        ShowGraph(list1, Color.red);
        DrawCurve(list2, Color.blue);
    }
        
    else if (_isGraphOne)
        ShowGraph(list1, Color.red);
    else if (_isGraphTwo)
        ShowGraph(list2, Color.blue);
}


private void ReDraw(object sender, EventArgs e)
{
    if (gameObjectList != null) DestroyGraph(gameObjectList);
    if (_isGraphOne && _isGraphTwo)
    {
        ShowGraph(_list1, Color.red);
        DrawCurve(_list2, Color.blue);
    }
    else if (_isGraphOne)
        ShowGraph(_list1, Color.red);
    else if (_isGraphTwo)
        ShowGraph(_list2, Color.blue);
}



    
    // Draws entire graph.
    private void ShowGraph(List<float> valueList, Color color)
    {
        if (valueList.Count == 0) return;
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;

        AddGridX(valueList);
        AddGridY(valueList);
        DrawCurve(valueList, color);
        
    }
    
    // Destroys the previous graph.
    

    private void DestroyGraph(List<GameObject> gameobjects)
    {
        foreach (GameObject obj in gameobjects)
        {
            Destroy(obj);
        }

        gameObjectList.Clear();
    }
    
    

    private GameObject CreateCircle(Vector2 anchoredPosition, Color color)
    {
        // create circle object, make it child of graph container, set its position in graph container.

        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
        rectTransform.anchorMin = Vector2.zero; 
        rectTransform.anchorMax = Vector2.zero; 
        return gameObject;
    }


    
    
    // Draws lines between points.
    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = color;
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
    protected void AddGridX(List<float> valueList)
    {
        dashTemplateX.sizeDelta = new Vector2(graphContainer.sizeDelta.y, 1f);
        float graphWidth = graphContainer.sizeDelta.x;
        int numberOfValues = valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        int count = firstX;
        
        

        foreach (int value in valueList)
        {
            float xPosition = (count-firstX) * xDelta;

            if (xPosition > graphWidth)
                break;
            
            RectTransform labelX = Instantiate(labelTemplateX, graphContainer, false);
            //labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true); 
            labelX.anchoredPosition = new Vector2(xPosition, -7f); 
            labelX.GetComponent<Text>().text = count.ToString();
            gameObjectList.Add(labelX.gameObject); 

            RectTransform dashX = Instantiate(dashTemplateX, graphContainer, false);
            //dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true); 
            dashX.anchoredPosition = new Vector2(xPosition, -2f);
            gameObjectList.Add(dashX.gameObject); 
            
            count += _truncateFactor;
        }

    }

    // Draws the grid of the Y-axis, as well as the labels of the Y-axis.
    private void AddGridY(List<float> valueList)
    {
        dashTemplateY.sizeDelta = new Vector2(graphContainer.sizeDelta.x, 1f);
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = valueList.Max() * yBufferTop;
        int separatorCount = _gridCountY;
        if (_isGraphOne && _isGraphTwo)
            yMax = Mathf.Max(_list1.Max(), _list2.Max())*yBufferTop;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY, graphContainer, false);
            //labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true); 
            float normalizedValue = (i * 1f) / separatorCount;
            labelY.anchoredPosition = (new Vector2(-10f, normalizedValue * graphHeight));
            labelY.GetComponent<Text>().text = (yMax * normalizedValue).ToString("0.0");
            gameObjectList.Add(labelY.gameObject); 


            RectTransform dashY = Instantiate(dashTemplateY, graphContainer, false);
            //dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true); 
            dashY.anchoredPosition = new Vector2(-2f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject); 
        }
    }

    // Draws the curve of the graph.
    private void DrawCurve(List<float> valueList, Color color)
    {
        if (valueList.Count == 0) return;

        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;
        int numberOfValues = valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        float yMax = valueList.Max() *yBufferTop;
        int count = firstX;

        if (_isGraphOne && _isGraphTwo)
        {
            if (_list1.Count == 0 || _list2.Count == 0) return;
            yMax = Mathf.Max(_list1.Max(), _list2.Max()) *yBufferTop;
        }


        GameObject lastCircleGameObject = null;

        foreach (float value in valueList)
        {
            float xPosition = (count-firstX) * xDelta;
            float yPosition = (value / yMax) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), color);
            gameObjectList.Add(circleGameObject);

            if (lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(
                    lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObject.GetComponent<RectTransform>().anchoredPosition, color);
                gameObjectList.Add(dotConnectionGameObject);
            }

            lastCircleGameObject = circleGameObject;
            count++;
        }

    }
}
