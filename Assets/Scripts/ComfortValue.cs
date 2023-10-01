using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ComfortValue
{
    // The block they have a preference to
    public SO_Block targetBlock;
    [Tooltip("The score they hold to that block (1 - really likes, 0 - indifferent, -1 - really dislikes)")]
    public float value;
}
