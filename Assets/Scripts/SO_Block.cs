using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Blocks/BlockData", order = 1)]
public class SO_Block: ScriptableObject
{
    public string blockName = "Enter name...";
    [SerializeField] public PreferenceValue[] preferences;
}
