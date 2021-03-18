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
    public Action<bool> SetGraphOne;
    public Action<bool> SetGraphTwo;

    [SerializeField] private int dotSize = 5;
    [SerializeField] Color lineColor = new Color(1, 1, 1, .5f);
    [SerializeField] float lineWidth = 2f; // line connecting dots
    
    [SerializeField] private float yBufferTop = 1.2f;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private float windowGraphSizeX = 1000;
    [SerializeField] private float windowGraphSizeY = 700;
    [SerializeField] private float graphContainerSizeX = 720;
    [SerializeField] private float graphContainerSizeY = 405;

    private static List<int> _list1 = new List<int>() {1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 51};
    private static List<int> _list2 = new List<int>() {1, 5, 10, 20, 10, 20, 40, 30, 60, 10, 10, 90, 80, 70, 50, 30, 10};
    private static int _truncateFactor = 1;
    private static int _gridCountY = 10;
    private int firstX = 0;
    private static bool _isGraphTwo = false;
    private static bool _isGraphOne = false;


    private RectTransform window_graph;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    
    private List<GameObject> gameObjectList;
    private List<GameObject> gridXList;
    private List<GameObject> gridYList;
    private List<GameObject> curveList;
    

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
    
    public static List<int> List1 
    {
        get => _list1;
        set => _list1 = value;
    }
    
    public static List<int> List2 
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
        window_graph = GetComponent<RectTransform>();
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
        gridXList = new List<GameObject>();
        gridYList = new List<GameObject>();
        curveList = new List<GameObject>();
        DataHandler dh = FindObjectOfType<DataHandler>();

        window_graph.sizeDelta = new Vector2(windowGraphSizeX, windowGraphSizeY);
        graphContainer.sizeDelta = new Vector2(graphContainerSizeX, graphContainerSizeY);
        dashTemplateX.sizeDelta = new Vector2(graphContainerSizeY + 2, 1);
        dashTemplateY.sizeDelta = new Vector2(graphContainerSizeX + 2, 1);
        dh.Display += Draw;
        ButtonClick.OnButtonX += ReDrawX;
        ButtonClick.OnButtonY += ReDrawY;
        ButtonClick.OnButtonTwoGraphs += ReDraw2;
        ButtonClick.OnButtonOneGraph += ReDraw1;
    }
    
    private float x = 0;
    float delta = .5f;
    private int count = 0;

    
    
/*public void Update()
{
    //testList = testlist;
    x += Time.deltaTime;
    

    if (x > delta && count <= 200)
    {
        x = 0;
        //DestroyGraph(gameObjectList);
        //ShowGraph(testlist);
        int r = Mathf.RoundToInt(Random.Range(0f + count, 20f + count));
        int r2 = Mathf.RoundToInt(Random.Range(0f + count*3, 20f + count));
        //testlist.Add(r);
        //_list1.Add(r);
        //_list2.Add(r2);
        //count++;
        //DestroyGraph(gameObjectList);
        //ShowGraph(_list1);
        //if (_isTwoGraphs)
        //    DrawCurve(_list2, Color.blue);
    }
    
    
}
*/
private void Draw(List<int> list1, List<int> list2)
{
    _list1 = list1;
    _list2 = list2;
    DestroyGraph(gameObjectList);
    if (_isGraphOne && _isGraphTwo)
    {
        ShowGraph(list1);
        DrawCurve(list2, Color.blue);
    }
        
    else if (_isGraphOne)
        ShowGraph(list1);
    else if (_isGraphTwo)
        ShowGraph(list2);
}


private void ReDrawX(object sender, EventArgs e)
{
    DestroyGraph(gameObjectList);
    if (_isGraphOne && _isGraphTwo)
    {
        ShowGraph(_list1);
        DrawCurve(_list2, Color.blue);
    }
    else if (_isGraphOne)
        ShowGraph(_list1);
    else if (_isGraphTwo)
        ShowGraph(_list2);
}

private void ReDrawY(object sender, EventArgs e)
{
    DestroyGraph(gameObjectList);
    if (_isGraphOne && _isGraphTwo)
    {
        ShowGraph(_list1);
        DrawCurve(_list2, Color.blue);
    }
    else if (_isGraphOne)
        ShowGraph(_list1);
    else if (_isGraphTwo)
        ShowGraph(_list2);
}

private void ReDraw2(object sender, EventArgs e)
{
    //DestroyGraph(gameObjectList);
    //ShowGraph(_list1);
    //DrawCurve(_list2, Color.blue);
    SetGraphTwo(true);
}

private void ReDraw1(object sender, EventArgs e)
{
    //DestroyGraph(gameObjectList);
    //ShowGraph(_list1);
    SetGraphOne(true);
}

    
    // Draws entire graph.
    private void ShowGraph(List<int> valueList)
    {

        _list1 = valueList;
        if (valueList.Count == 0)
            return;
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;

        AddGridX(valueList);
        AddGridY(valueList);
        DrawCurve(valueList, lineColor);
        
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
    protected void AddGridX(List<int> valueList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        //int separatorCount = _gridCountX;
        int numberOfValues = valueList.Count;
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
            gridXList.Add(labelX.gameObject);
            gameObjectList.Add(labelX.gameObject); //TODO

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true); 
            dashX.anchoredPosition = new Vector2(xPosition, -2f); 
            gridXList.Add(dashX.gameObject);
            gameObjectList.Add(dashX.gameObject); //TODO
            
            count += _truncateFactor;
        }

    }

    // Draws the grid of the Y-axis, as well as the labels of the Y-axis.
    private void AddGridY(List<int> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = valueList.Max() * yBufferTop;
        int separatorCount = _gridCountY;
        if (_isGraphOne && _isGraphTwo)
            yMax = Mathf.Max(_list1.Max(), _list2.Max())*yBufferTop;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true); 
            float normalizedValue = (i * 1f) / separatorCount;
            labelY.anchoredPosition = (new Vector2(-10f, normalizedValue * graphHeight));
            labelY.GetComponent<Text>().text = (yMax * normalizedValue).ToString("0.0"); 
            gridYList.Add(labelY.gameObject);
            gameObjectList.Add(labelY.gameObject); //TODO


            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true); 
            dashY.anchoredPosition = new Vector2(-2f, normalizedValue * graphHeight);
            gridYList.Add(dashY.gameObject);
            gameObjectList.Add(dashY.gameObject); //TODO 
        }
    }

    // Draws the curve of the graph.
    private void DrawCurve(List<int> valueList, Color color)
    {
        var sizeDelta = graphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x;
        int numberOfValues = valueList.Count;
        float xDelta = graphWidth / numberOfValues;
        float yMax = valueList.Max() * yBufferTop;
        int count = firstX;

        if (_isGraphOne && _isGraphTwo)
            yMax = Mathf.Max(_list1.Max(), _list2.Max()) * yBufferTop; 
        

        GameObject lastCircleGameObject = null;

        foreach (int value in valueList)
        {
            float xPosition = (count-firstX) * xDelta;
            float yPosition = (value / yMax) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), color);
            curveList.Add(circleGameObject);
            gameObjectList.Add(circleGameObject);

            if (lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(
                    lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObject.GetComponent<RectTransform>().anchoredPosition, color);
                curveList.Add(dotConnectionGameObject);
                gameObjectList.Add(dotConnectionGameObject);
            }

            lastCircleGameObject = circleGameObject;
            count++;
        }

    }
}
