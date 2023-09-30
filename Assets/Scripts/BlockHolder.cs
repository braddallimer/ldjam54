using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHolder : MonoBehaviour
{
    [SerializeField] string blockTag = "Block";
    public BlockInstance heldBlock;
    [SerializeField] float preferenceScore;

    public void AddBlockToHolder(BlockInstance newBlock)
    {
        // If somehow this holder's block is still present, this should prevent errors
        if (heldBlock != null)
            RemoveBlockFromHolder();

        // Remove block from Pool
        // ph: BlockPool.instance.RemoveBlock(newBlock);

        // Set transform of block
        newBlock.transform.position = transform.position;
        newBlock.transform.parent = transform;
        heldBlock = newBlock;

        CalculatePreferenceScore();
    }

    public void RemoveBlockFromHolder()
    {
        // If somehow the this holder's block has been destroyed/moved, this should prevent errors
        if (heldBlock == null)
            return;

        // Cache heldBlock (just in case)
        BlockInstance blockToRemove = heldBlock;

        // Set transform of block
        blockToRemove.transform.position = Vector3.down * 10;
        blockToRemove.transform.parent = null;
        heldBlock = null;

        // Add block to Pool
        // ph: BlockPool.instance.AddBlock(blockToRemove);
        preferenceScore = 0;
    }

    public void SwapBlockFromOtherHolder(BlockHolder otherHolder)
    {
        // If somehow the other holder's block has been destroyed/moved, this should prevent errors
        if (otherHolder.heldBlock == null)
            return;

        // Check if this holder has a held block before a swap is made, otherwise just add block to this holder
        if (heldBlock == null)
        {
            AddBlockToHolder(otherHolder.heldBlock);
            return;
        }
        
        // Cache other holder's held block and my held block (just in case)
        BlockInstance myNewBlock = otherHolder.heldBlock;
        BlockInstance myOldBlock = heldBlock;

        // Remove blocks from both holders
        RemoveBlockFromHolder();
        otherHolder.RemoveBlockFromHolder();

        // Add blocks to holders
        otherHolder.AddBlockToHolder(myOldBlock);
        AddBlockToHolder(myNewBlock);
    }

    public void ReplaceBlock(BlockInstance newBlock)
    {
        RemoveBlockFromHolder();
        AddBlockToHolder(newBlock);
    }

    void CalculatePreferenceScore()
    {
        float scoreAverage = 0;
        int numInCloseProx = 0;

        SO_Block heldBlockInfo = heldBlock.blockInfo;

        foreach(GameObject gmob in GameObject.FindGameObjectsWithTag(blockTag))
        {
            if (gmob != null && gmob != heldBlock.gameObject)
            {
                float distance = Vector2.Distance(transform.position, gmob.transform.position);

                if (distance < 4)
                    numInCloseProx++;

                foreach (PreferenceValue pref in heldBlockInfo.preferences)
                {
                    if (pref.prefBlock == gmob.GetComponent<BlockInstance>().blockInfo)
                    {
                        scoreAverage += pref.prefValue * DistanceMultiplier(distance);
                    }

                    else
                        scoreAverage += .5f * DistanceMultiplier(distance);
                }
            }
        }

        scoreAverage /= numInCloseProx;
        preferenceScore = scoreAverage;
        Debug.Log($"{heldBlock.name}'s preference score is {preferenceScore}");
    }

    float DistanceMultiplier(float distance)
    {
        if (distance > 4)
            return 0;

        else if (distance > 3)
            return .25f;

        else if (distance > 2)
            return .5f;

        else
            return 1;
    }
}
