using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Object = System.Object;
using Random = UnityEngine.Random;

// http://devmag.org.za/2009/05/03/poisson-disk-sampling/
public class ObjectPlacement : MonoBehaviour
{

    public List<GameObject> groups;
    public SimulationSettings simulationSettings;
    int size;
    private List<string> pooledObjects = new List<string> { "Rabbits", "Wolfs", "Deers", "Bears" };

    public void Awake()
    {
        GetSettings();
    }

    private void GetSettings()
    {
        simulationSettings = FindObjectOfType<SimulationSettings>();
    }


    public void PlaceObjects(Vector2 positionOffset)
    {
        if (simulationSettings == null)
        {
            GetSettings();
        }

        int size;
        groups = new List<GameObject>();

        if (simulationSettings.MeshSettings.UseFlatShading)
        {
            size = MeshSettings.supportedChunkSizes[simulationSettings.MeshSettings.FlatShadedChunkSizeIndex];
        }
        else
        {
            size = MeshSettings.supportedChunkSizes[simulationSettings.MeshSettings.ChunkSizeIndex];
        }

        size = Mathf.RoundToInt(size * simulationSettings.MeshSettings.MeshScale);
        this.size = size;

        for (int i = 0; i < simulationSettings.ObjectPlacementSettings.ObjectTypes.Count; i++)
        {
            PlaceObjectType(simulationSettings.ObjectPlacementSettings.GetObjectType(i), positionOffset);
        }
    }

    public void PlaceObjectType(ObjectType objectType, Vector2 positionOffset)
    {
        int deleted = 0;
        if (objectType.GameObjectSettings == null || objectType.GameObjectSettings.Count <= 0)
        {
            return;
        }
        GameObject groupObject = new GameObject(objectType.Name + " Group");
        groups.Add(groupObject);
        groupObject.transform.parent = this.transform;
        List<Vector2> points = GeneratePlacementPoints(objectType, simulationSettings.MeshSettings.MeshScale, size);
        foreach (var point in points)
        {
            float maxLength = 0;
            foreach (var obj in objectType.GameObjectSettings)
            {
                maxLength += obj.Probability;
            }

            float randomChoice = Random.Range(0f, maxLength);
            float counter = 0;
            int randomIndex = 0;

            for (int j = 0; j < objectType.GameObjectSettings.Count; j++)
            {
                counter += objectType.GameObjectSettings[j].Probability;
                if (randomChoice <= counter)
                {
                    randomIndex = j;
                    break;
                }
            }
            //Debug.Log("GameObjectSettings Count [" + objectType.GameObjectSettings.Count + "] Random index: " + randomIndex);
            if (objectType.GameObjectSettings[randomIndex].GameObject != null)
            {
                GameObject gameObject = Instantiate(objectType.GameObjectSettings[randomIndex].GameObject, new Vector3(point.x - size / 2 + positionOffset.x, simulationSettings.HeightMapSettings.MaxHeight + 10, point.y - size / 2 + positionOffset.y), Quaternion.identity);

                gameObject.transform.parent = groupObject.transform;

                Ray ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.down));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        bool withinSpan = hit.point.y <= (simulationSettings.HeightMapSettings.MaxHeight - simulationSettings.HeightMapSettings.MinHeight) * objectType.MaxHeight
                        && hit.point.y >= (simulationSettings.HeightMapSettings.MaxHeight - simulationSettings.HeightMapSettings.MinHeight) * objectType.MinHeight;

                        if (withinSpan)
                        {
                            Vector3 oldPosition = gameObject.transform.position;
                            var agent = gameObject.GetComponent<NavMeshAgent>();
                            if (agent == null)
                            {
                                gameObject.transform.position = new Vector3(oldPosition.x, hit.point.y + objectType.yOffset, oldPosition.z);
                            }
                            else
                            {
                                agent.Warp(new Vector3(oldPosition.x, hit.point.y + objectType.yOffset, oldPosition.z));
                            }
                            var animalName = objectType.Name;
                            //Indirect logs animal as instantiated in collector
                            if (pooledObjects.IndexOf(animalName) != -1)
                            {
                                ObjectPooler.Instance?.HandleAnimalInstantiated(gameObject, animalName);
                            }

                            continue;
                        }
                    }
                }

                if (Application.isEditor)
                {
                    DestroyImmediate(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
                deleted++;
            }
        }
        ObjectPooler.Instance?.HandleFinishedSpawning();
    }

    public static List<Vector2> GeneratePlacementPoints(ObjectType objectType, float meshScale, int size)
    {
        List<Vector2> points = PoissonDiskSampling.GeneratePoisson(size, size, objectType.MinimumDistance * meshScale, objectType.NewPointCount);

        return points;
    }

    public void UpdateObjectType(int index, Vector2 positionOffset)
    {
        DestoryGroupObjectWithIndex(index);
        PlaceObjectType(simulationSettings.ObjectPlacementSettings.GetObjectType(index), positionOffset);
    }

    public void DestoryGroupObjectWithIndex(int index)
    {
        DestroyGroupObjectWithName(simulationSettings.ObjectPlacementSettings.GetObjectType(index).Name);
    }

    public void DestroyGroupObjectWithName(string name)
    {
        var groupObject = GameObject.Find(name + " Group");
        if (groupObject != null)
        {
            Destroy(GameObject.Find(name + " Group"));
        }
    }
}

public static class PoissonDiskSampling
{

    private static System.Random rand = new System.Random();
    public static List<Vector2> GeneratePoisson(int width, int height, float minDist, int newPointCount)
    {
        float cellSize = minDist / (2 * Mathf.PI);

        Vector2[,] grid = new Vector2[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];

        List<Vector2> processList = new List<Vector2>();
        List<Vector2> samplePoints = new List<Vector2>();

        Vector2 firstPoint = new Vector2(Random.Range(0f, width), Random.Range(0f, height));
        processList.Add(firstPoint);
        samplePoints.Add(firstPoint);
        Vector2 gridPoint = ImageToGrid(firstPoint, cellSize);
        grid[(int)gridPoint.x, (int)gridPoint.y] = firstPoint;

        while (processList.Count > 0)
        {
            Vector2 point = PopRandom(processList);

            for (int i = 0; i < newPointCount; i++)
            {
                Vector2 newPoint = GenerateRandomPointAround(point, minDist);

                if (InRectangle(newPoint, width, height) && !InNeighbourhood(grid, newPoint, minDist, cellSize))
                {
                    processList.Add(newPoint);
                    samplePoints.Add(newPoint);
                    var imagePoint = ImageToGrid(newPoint, cellSize);
                    grid[(int)imagePoint.x, (int)imagePoint.y] = newPoint;
                }
            }
        }
        return samplePoints;
    }

    private static Vector2 ImageToGrid(Vector2 point, float cellSize)
    {
        return new Vector2((int)(point.x / cellSize), (int)(point.y / cellSize));
    }

    private static T PopRandom<T>(List<T> list)
    {
        int index = rand.Next(0, list.Count);
        T element = list[index];
        list.RemoveAt(index);
        return element;
    }

    private static bool InRectangle(Vector2 point, int width, int height)
    {
        bool result = point.x >= 0 && point.x < width;
        result = result && point.y >= 0 && point.y < height;
        return result;
    }

    private static Vector2[] SquaresArountPoint(Vector2[,] grid, Vector2 point, int size)
    {

        if (size % 2 == 0)
        {
            Debug.LogError("Cannot take a size that is an even number");
            return new Vector2[0];
        }

        Vector2[] output = new Vector2[size * size];

        int gridX = (int)point.x;
        int gridY = (int)point.y;
        int halfSize = (size - 1) / 2;
        int outIndex = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (gridX - halfSize + x >= 0 && gridX - halfSize + x < grid.GetLength(0))
                {
                    if (gridY - halfSize + y >= 0 && gridY - halfSize + y < grid.GetLength(1))
                    {
                        output[outIndex] = grid[gridX - halfSize + x, gridY - halfSize + y];
                        outIndex++;
                    }
                }

            }
        }

        return output;

    }


    private static float SqrDistance(Vector2 a, Vector2 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    private static bool InNeighbourhood(Vector2[,] grid, Vector2 point, float minDist, float cellSize)
    {
        Vector2 gridPoint = ImageToGrid(point, cellSize);
        Vector2[] cellsAroundPoint = SquaresArountPoint(grid, gridPoint, 5);

        foreach (var cell in cellsAroundPoint)
        {
            if (cell != null)
            {
                if (SqrDistance(cell, point) < minDist * minDist)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static Vector2 GenerateRandomPointAround(Vector2 point, float minDist)
    {
        double r1 = rand.NextDouble();
        double r2 = rand.NextDouble();

        double radius = minDist * (r1 + 1);

        double angle = 2 * Mathf.PI * r2;

        double newX = point.x + radius * Mathf.Cos((float)angle);
        double newY = point.y + radius * Mathf.Sin((float)angle);

        Vector2 output = new Vector2((float)newX, (float)newY);
        return output;
    }

}

