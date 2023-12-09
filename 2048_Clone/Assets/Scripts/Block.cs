using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Blocks")]
public class Block : ScriptableObject
{
    public int value;
    public Color color;
}