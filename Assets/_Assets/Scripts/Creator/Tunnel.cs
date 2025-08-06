using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tunnel : MonoBehaviour
{
    [SerializeField] private List<ColorType> mobs = new List<ColorType>();
    [SerializeField] private TMP_Text mobLeftText;
    [SerializeField] private Vector2Int pos;

    public Vector2Int Pos
    {
        get => pos;
        set => pos = value;
    }

    private void Start()
    {
        mobLeftText.text = mobs.Count.ToString();
    }

    public bool GetMob(out ColorType mob)
    {
        if (mobs.Count > 0)
        {
            mob = mobs[0];
            mobs.RemoveAt(0);
            mobLeftText.text = mobs.Count.ToString();
            return true;
        }
        mob = ColorType.None;
        return false;
    }
}
