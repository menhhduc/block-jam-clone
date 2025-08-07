using UnityEngine;
using System.Collections.Generic;

public class TestGrid : MonoBehaviour
{
    private bool[,] walkable;
    private TestNode[,] grid;

    [Header("Grid setup")]
    public int column; // x
    public int row; // y
    public TestNode prefab;
    
    [Header("Obstacle Detection")]
    public LayerMask obstacleLayer;

    #region Public Getters
    
    public TestNode[,] GetGrid() => grid;
    public bool[,] GetWalkable() => walkable;
    public int GetColumn() => column;
    public int GetRow() => row;
    
    #endregion
    

    public void SpawnCharacter(GameObject prefab, Vector2Int pos)
    {
        int x = pos.x;
        int y = pos.y;

        var h = grid[x, y].transform.localScale.x / 2;
        Vector3 spawnPos = new Vector3(x + h, y + h, 0);
        var characterObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        characterObj.transform.parent = this.transform;

        var character = characterObj.GetComponent<Character>();
        if (character != null)
        {
            character.Initialize(this, pos);
        }

        SetWalkable(pos, false);
    }

    private void SetWalkable(Vector2Int position, bool isWalkable)
    {
        if (position.x >= 0 && position.x < column && position.y >= 0 && position.y < row)
        {
            walkable[position.x, position.y] = isWalkable;
        }
    }

    public void UpdatePos(Vector2Int oldPos, Vector2Int newPos)
    {
        SetWalkable(oldPos, true);
        SetWalkable(newPos, false);
    }

    // Tạo grid
    public void CreateGrid()
    {
        grid = new TestNode[column, row];
        walkable = new bool[column, row];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var node = Instantiate(prefab);
                node.Init(x, y);
                var h = node.transform.localScale.x / 2;
                Vector3 nodePos = new Vector3(x + h, y + h);
                node.transform.position = nodePos;
                grid[x, y] = node;
                
                bool hasObstacle = Physics2D.OverlapPoint(nodePos, obstacleLayer);
                walkable[x, y] = !hasObstacle;
            }
        }
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (walkable == null || grid == null)
        {
            return;
        }

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector3 pos;
                var node = grid[x, y];
                if (node != null)
                {
                    pos = node.transform.position;
                }
                else
                {
                    pos = new Vector3(x + 0.5f, y + 0.5f, 0);
                }

                if (walkable[x, y])
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    bool isObstacle = Physics2D.OverlapPoint(pos, obstacleLayer);
                    if (isObstacle)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.magenta;
                    }
                }
                Gizmos.DrawCube(pos, Vector3.one * 0.35f);
            }
        }
    }
    #endregion
}