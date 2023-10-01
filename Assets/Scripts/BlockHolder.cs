using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHolder : MonoBehaviour
{
    public BlockInstance heldBlock;
    [SerializeField] BlockManager blockMang;

    private void Start()
    {
        blockMang = FindObjectOfType<BlockManager>();
    }

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

        blockMang.UpdateBlockDiscomfortScores();
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

        blockMang.UpdateBlockDiscomfortScores();
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
        BlockInstance oldBlock = heldBlock;

        RemoveBlockFromHolder();
        blockMang.ReturnBlockToPool(oldBlock);

        AddBlockToHolder(newBlock);
    }

    
}
