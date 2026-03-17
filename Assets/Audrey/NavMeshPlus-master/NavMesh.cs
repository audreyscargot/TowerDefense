using NavMeshPlus.Components;
using UnityEngine;

public class NavMesh : MonoBehaviour
{
    public NavMeshSurface surface;

    public void BakeNavMesh()
    {
        surface.BuildNavMeshAsync();
    }
}