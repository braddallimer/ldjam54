using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Blocks/BlockData", order = 1)]
public class SO_Block: ScriptableObject
{
    public string blockName = "Enter name...";
    public Color color = Color.white;
    public Sprite face = null;
    [SerializeField] public DiscomfortValue[] relationships;
}
