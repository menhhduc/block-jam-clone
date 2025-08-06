using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private float cornerRadius = 0.1f;
    [SerializeField] private Color tileColor = Color.white;
    // [SerializeField] private int cornerSegments = 8;
    
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        CreateRoundedTile();
    }
    
    private void CreateRoundedTile()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Texture2D texture = CreateRoundedRectTexture(100, 100, cornerRadius * 100, tileColor);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        
        spriteRenderer.sprite = sprite;
    }
    
    private Texture2D CreateRoundedRectTexture(int width, int height, float radius, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsInsideRoundedRect(x, y, width, height, radius))
                {
                    pixels[y * width + x] = color;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    private bool IsInsideRoundedRect(int x, int y, int width, int height, float radius)
    {
        float centerX = width / 2f;
        float centerY = height / 2f;
        
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        float dx = Mathf.Abs(x - centerX);
        float dy = Mathf.Abs(y - centerY);
        
        if (dx <= halfWidth - radius || dy <= halfHeight - radius)
        {
            return true;
        }
        
        float cornerCenterX = halfWidth - radius;
        float cornerCenterY = halfHeight - radius;
        
        if (dx > cornerCenterX && dy > cornerCenterY)
        {
            float distance = Mathf.Sqrt((dx - cornerCenterX) * (dx - cornerCenterX) + (dy - cornerCenterY) * (dy - cornerCenterY));
            return distance <= radius;
        }
        
        return true;
    }
}