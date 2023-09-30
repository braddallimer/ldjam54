using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PreferenceValue
{
    // The block they have a preference to
    public SO_Block prefBlock;
    // The score they hold to that block (0 - really likes, 0.5 - indifferent, 1 - really dislikes)
    [Tooltip("The score they hold to that block (0 - really likes, 0.5 - indifferent, 1 - really dislikes)")]
    public float prefValue;
}
