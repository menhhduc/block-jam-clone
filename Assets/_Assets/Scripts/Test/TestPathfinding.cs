using System.Collections.Generic;
using UnityEngine;

public static class TestPathfinding
{
    public static List<Slot> FindPath(Vector2Int start, Vector2Int end, TestGridAStar grid)
    {
        Node startNode = grid.grid[start.x, start.y], endNode = grid.grid[end.x, end.y];
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || (openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost))
                {
                    node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (Node neighbour in grid.GetNeighbors(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    private static List<Slot> RetracePath(Node startNode, Node endNode)
    {
        List<Slot> path = new List<Slot>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.slot);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }


    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
