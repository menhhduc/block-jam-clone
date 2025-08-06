using JetBrains.Annotations;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] TestMob mob;

    public TestMob Mob
    {
        get => mob;
        set => mob = value;
    }

    [SerializeField] Tunnel tunnel;

    public Tunnel Tunnel
    {
        get => tunnel;
        set => tunnel = value;
    }
}
