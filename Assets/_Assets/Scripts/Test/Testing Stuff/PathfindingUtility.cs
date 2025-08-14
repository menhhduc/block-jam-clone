using UnityEngine;
using System.Collections.Generic;

public static class PathfindingUtility
{
    public static List<Vector2Int> FindPath(
        TestNode[,] grid,
        bool[,] walkable,
        int column,
        int row,
        Vector2Int startPos,
        Vector2Int endPos)
    {
        List<TestNode> openList = new List<TestNode>();
        List<TestNode> closedList = new List<TestNode>();
        
        TestNode startNode = grid[startPos.x, startPos.y];
        TestNode endNode = grid[endPos.x, endPos.y];
        openList.Add(startNode);

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                grid[x, y].parent = null;
                grid[x, y].SetCost(0, 0, 0);
            }
        }

        int maxIterations = column * row * 10;
        int iterations = 0;

        while (openList.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            
            TestNode currentNode = openList[0];
            
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost <= currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                TestNode pathNode = endNode;
                while (pathNode != null)
                {
                    path.Insert(0, pathNode.position);
                    pathNode = pathNode.parent;
                }

                return path;
            }

            Vector2Int[] direction =
            {
                new Vector2Int(0, 1), // Up
                new Vector2Int(0, -1), // Down
                new Vector2Int(-1, 0), // Left
                new Vector2Int(1, 0), // Right
            };

            foreach (var dir in direction)
            {
                int neighborX = currentNode.position.x + dir.x;
                int neighborY = currentNode.position.y + dir.y;

                if (neighborX < 0 || neighborX >= column || neighborY < 0 || neighborY >= row)
                    continue;
                TestNode neighborNode = grid[neighborX, neighborY];
                
                if (!walkable[neighborX, neighborY] || closedList.Contains(neighborNode))
                    continue;
                
                float gCost = Vector2.Distance(new Vector2(neighborX, neighborY), new Vector2(startPos.x, startPos.y));
                float hCost = Vector2.Distance(new Vector2(neighborX, neighborY), new Vector2(endPos.x, endPos.y));
                float fCost = gCost + hCost;

                if (!openList.Contains(neighborNode) || gCost < neighborNode.gCost)
                {
                    neighborNode.parent = currentNode;
                    neighborNode.SetCost(gCost, hCost, fCost);
                    openList.Add(neighborNode);
                }
            }
        }
        return null;
    }
}
