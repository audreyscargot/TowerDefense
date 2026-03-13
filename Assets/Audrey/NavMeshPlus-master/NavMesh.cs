using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class NavMesh : MonoBehaviour
{
    public NavMeshSurface surface;

    void Start()
    {
        surface.BuildNavMeshAsync();
    }
}
