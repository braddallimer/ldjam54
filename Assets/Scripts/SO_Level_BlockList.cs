using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Block List", menuName = "Blocks/Block List", order = 1)]
public class SO_Level_BlockList : ScriptableObject
{
    public SO_Block[] blockList;
}
