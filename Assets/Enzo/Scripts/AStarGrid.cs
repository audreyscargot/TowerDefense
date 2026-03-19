using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public static AStarGrid Instance { get; private set; }

    [Header("Grid Settings — auto-set by MapGenerator")]
    public Vector2 gridWorldSize = new Vector2(50, 50);
    public float nodeRadius = 0.5f;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void BuildGridFromObstacles(bool[,] obstacleGrid, int mapSize)
    {
        nodeDiameter = nodeRadius * 2f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid = new Node[gridSizeX, gridSizeY];

        Vector2 bottomLeft = (Vector2)transform.position
            - Vector2.right * gridWorldSize.x / 2
            - Vector2.up    * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft
                    + Vector2.right * (x * nodeDiameter + nodeRadius)
                    + Vector2.up    * (y * nodeDiameter + nodeRadius);

                int obsX = Mathf.Clamp(x * mapSize / gridSizeX, 0, mapSize - 1);
                int obsY = Mathf.Clamp(y * mapSize / gridSizeY, 0, mapSize - 1);
                bool walkable = !obstacleGrid[obsX, obsY];

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public void SetNodeWalkable(Vector2 worldPos, bool walkable)
    {
        Node n = NodeFromWorldPoint(worldPos);
        if (n != null) n.walkable = walkable;
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if (grid == null) return null;

        Node startNode  = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        if (startNode == null || targetNode == null) return null;

        if (!targetNode.walkable)
            targetNode = FindNearestWalkable(targetNode);
        if (targetNode == null) return null;

        List<Node> openSet    = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < current.fCost || (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                    current = openSet[i];

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCost = current.gCost + GetDistance(current, neighbour);
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost  = newCost;
                    neighbour.hCost  = GetDistance(neighbour, targetNode);
                    neighbour.parent = current;
                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    // BFS outward from origin until a walkable node is found outside the wall cluster
    private Node FindNearestWalkable(Node origin)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(origin);
        visited.Add(origin);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            foreach (Node neighbour in GetNeighbours(current))
            {
                if (visited.Contains(neighbour)) continue;
                visited.Add(neighbour);

                if (neighbour.walkable) return neighbour;

                queue.Enqueue(neighbour);
            }
        }

        return null;
    }

    private List<Vector2> RetracePath(Node start, Node end)
    {
        List<Vector2> path = new List<Vector2>();
        Node current = end;
        while (current != start) { path.Add(current.worldPosition); current = current.parent; }
        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int cx = node.gridX + x;
                int cy = node.gridY + y;
                if (cx >= 0 && cx < gridSizeX && cy >= 0 && cy < gridSizeY)
                    neighbours.Add(grid[cx, cy]);
            }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector2 worldPos)
    {
        if (grid == null) return null;
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
    }
}

public class Node
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX, gridY;
    public int gCost, hCost;
    public Node parent;
    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable      = walkable;
        this.worldPosition = worldPos;
        this.gridX         = gridX;
        this.gridY         = gridY;
    }
}
