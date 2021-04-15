#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;

[SerializeField]
[ExecuteInEditMode]
[RequireComponent(typeof(NavMeshSurface))]
public class SerializeNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshData navMeshData;

    void Awake()
    {
        if (navMeshData != null)
        {
            GetComponent<NavMeshSurface>().UpdateNavMesh(navMeshData);
        }
    }

    void Start()
    {
        if (navMeshData == null)
        {
            Serialize();
        }

    }

    public void Serialize()
    {
        navMeshData = GetComponent<NavMeshSurface>().navMeshData;
    }

}