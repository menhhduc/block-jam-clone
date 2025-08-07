using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Character : MonoBehaviour
{
    private TestGrid grid;
    private Vector2Int gridPosition;

    #region Color Configuration

    [Header("Color setup")] [SerializeField]
    private ColorType colorType = ColorType.None;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetColor();
    }

    public void SetColorType(ColorType newColor)
    {
        colorType = newColor;
        SetColor();
    }

    public ColorType GetColorType()
    {
        return colorType;
    }

    private void SetColor()
    {
        if (spriteRenderer == null)
            return;

        switch (colorType)
        {
            case ColorType.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case ColorType.Green:
                spriteRenderer.color = Color.green;
                break;
            case ColorType.Red:
                spriteRenderer.color = Color.red;
                break;
            case ColorType.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
            case ColorType.Cyan:
                spriteRenderer.color = Color.cyan;
                break;
            default:
                spriteRenderer.color = Color.white;
                break;
        }
    }

    #endregion

    public void Initialize(TestGrid grid, Vector2Int startPos)
    {
        this.grid = grid;
        this.gridPosition = startPos;
    }

    #region Events

    private void OnMouseDown()
    {
        if (grid == null || LevelManager.Instance == null) return;

        List<Vector2Int> endPositions = LevelManager.Instance.GetEndPos();
        if (endPositions == null || endPositions.Count == 0)
            return;

        TestNode[,] gridNodes = grid.GetGrid();
        List<Vector2Int> path = null;
        Vector2Int end = endPositions[0];

        foreach (var pos in endPositions)
        {
            var findPath = PathfindingUtility.FindPath(
                grid.GetGrid(),
                grid.GetWalkable(),
                grid.GetColumn(),
                grid.GetRow(),
                gridPosition,
                pos);

            if (findPath != null && findPath.Count > 0)
            {
                if (path == null || findPath.Count < path.Count)
                {
                    path = findPath;
                    end = pos;
                }
            }
        }

        if (path != null)
        {
            MoveCharacter(path, grid.GetGrid());
            Vector2Int oldPosition = gridPosition;
            gridPosition = end;
            LevelManager.Instance.UpdateCharacterPosition(oldPosition, gridPosition);
        }
    }

    #endregion

    private void MoveCharacter(List<Vector2Int> path, TestNode[,] grid)
    {
        if (path == null || path.Count == 0 || grid == null)
            return;

        List<Vector3> waypoints = new List<Vector3>();
        foreach (var pos in path)
        {
            int x = pos.x;
            int y = pos.y;
            var h = grid[x, y].transform.localScale.x / 2;
            Vector3 targetPos = new Vector3(x + h, y + h, 0);
            waypoints.Add(targetPos);
        }

        transform.DOPath(waypoints.ToArray(), waypoints.Count * 0.1f, PathType.Linear)
            .SetEase(Ease.Linear)
            .OnComplete(() => { MoveToFinish(); });
    }

    private void MoveToFinish()
    {
        Transform sortedSlot = LevelManager.Instance.GetSortedFinishSlot(this);
        if (sortedSlot != null)
        {
            LevelManager.Instance.UpdateCharacterPosition(gridPosition, new Vector2Int(-1, -1));
            transform.DOMove(sortedSlot.position, 0.5f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    LevelManager.Instance.AddCharacterToFinish(this);
                });
        }
    }
}