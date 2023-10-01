using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInstance : MonoBehaviour
{
    public SO_Block blockInfo;
    public float comfortScore;

    public void UpdateAppearance()
    {
        transform.GetComponent<SpriteRenderer>().color = blockInfo.color;
        gameObject.name = $"Block ({blockInfo.blockName})";
    }
}
