using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHolder : MonoBehaviour
{
    public Block heldBlock;

    public void AddBlockToHolder(Block newBlock)
    {
        // If somehow this holder's block is still present, this should prevent errors
        if (heldBlock != null)
            RemoveBlockFromHolder();

        // Remove block from corral
        // ph: BlockCorral.instance.RemoveBlock(newBlock);

        // Set transform of block
        newBlock.transform.position = transform.position;
        newBlock.transform.parent = transform;
        heldBlock = newBlock;
    }

    public void RemoveBlockFromHolder()
    {
        // If somehow the this holder's block has been destroyed/moved, this should prevent errors
        if (heldBlock == null)
            return;

        // Cache heldBlock (just in case)
        Block blockToRemove = heldBlock;

        // Set transform of block
        blockToRemove.transform.position = Vector3.down * 10;
        blockToRemove.transform.parent = null;
        heldBlock = null;

        // Add block to corral
        // ph: BlockCorral.instance.AddBlock(blockToRemove);
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
        Block myNewBlock = otherHolder.heldBlock;
        Block myOldBlock = heldBlock;

        // Remove blocks from both holders
        RemoveBlockFromHolder();
        otherHolder.RemoveBlockFromHolder();

        // Add blocks to holders
        otherHolder.AddBlockToHolder(myOldBlock);
        AddBlockToHolder(myNewBlock);
    }
}
