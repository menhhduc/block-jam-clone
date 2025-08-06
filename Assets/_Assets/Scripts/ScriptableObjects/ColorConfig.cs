using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;


[System.Serializable]
public class ColorDict : SerializableDictionaryBase<ColorType, Material> { }


[CreateAssetMenu(menuName = "Scriptable Object/Color Config", fileName = "Color Config")]
public class ColorConfig : ScriptableObject
{
    [SerializeField] ColorDict colorDict;
    public ColorDict ColorDict => colorDict;
}
