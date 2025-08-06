using UnityEngine;
using System;

public class TestNode : MonoBehaviour
{
    public Vector2Int position;
    public float gCost;
    public float hCost;
    public float fCost;

    public SpriteRenderer spriteRenderer;
    public TextMesh coordinates;
    public TextMesh costInfo;

    public TestNode parent;
    public Action<TestNode> OnNodeClicked;

    public TestNode(Vector2Int pos)
    {
        position = pos;
    }

    public void Init(int x, int y)
    {
        position = new Vector2Int(x, y);
    }

    public void SetColor(Color color)
    {
        var material = spriteRenderer.material;
        material.color = color;
    }

    public void SetCost(float gCost, float hCost, float fCost)
    {
        this.gCost = gCost;
        this.hCost = hCost;
        this.fCost = fCost;
    }

    private void OnMouseDown()
    {
        OnNodeClicked?.Invoke(this);
    }
}