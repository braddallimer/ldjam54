using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] string rayTag;
    [SerializeField] BlockInstance selectedBlock;
    [SerializeField] BlockHolder lastBlockHolder;

    private void Update()
    {
        MouseControls();

        if(selectedBlock != null)
        {
            selectedBlock.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(Vector3.zero, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void MouseControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 vector = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(vector, Vector2.zero, 0);

            if (hit.collider != null && hit.transform.CompareTag("Holder"))
            {
                hit.transform.TryGetComponent<BlockHolder>(out BlockHolder targetHolder);

                if (selectedBlock != null)
                {
                    Debug.Log("1) Player is holding block...");

                    if (targetHolder != null)
                    {
                        Debug.Log("2) ...over a block holder...");

                        // If the holder being triggered has a held block
                        if (targetHolder.heldBlock != null)
                        {
                            Debug.Log("3) ...that has a held block...");

                            // if the selected block comes from a previously triggered holder
                            if (lastBlockHolder != null)
                            {
                                Debug.Log("4) ...being swapped from another holder.");

                                targetHolder.SwapBlockFromOtherHolder(lastBlockHolder);
                                lastBlockHolder = null;
                            }

                            // if there is no previously triggered holder
                            else
                            {
                                Debug.Log("4) ...being replaced.");

                                targetHolder.ReplaceBlock(selectedBlock);
                            }
                        }

                        else // if the holder being triggered has no held block
                        {
                            Debug.Log("2) ...and adding it to a holder");

                            if(lastBlockHolder != null)
                                lastBlockHolder.RemoveBlockFromHolder();

                            targetHolder.AddBlockToHolder(selectedBlock);
                            lastBlockHolder = null;
                        }

                        SetSelectedBlock(null);
                    }

                    else 
                    {
                        Debug.Log("2) ..they tried placing it over the void. Returning to pool.");

                        // return block to pool
                    }
                }

                else
                {
                    Debug.Log("1) Player is not holding a block...");

                    if (targetHolder != null)
                    {
                        Debug.Log("2) ...hovering over a holder...");

                        if (targetHolder.heldBlock != null)
                        {
                            Debug.Log("3) ...with a held block, they select it.");

                            SetSelectedBlock(targetHolder.heldBlock);
                            lastBlockHolder = targetHolder;
                        }
                    }
                }
            }

            else if(selectedBlock != null)
            {
                Debug.Log("..they tried placing it over the void. Returning to pool.");

                if(lastBlockHolder != null)
                {
                    selectedBlock.transform.position = lastBlockHolder.transform.position;
                    selectedBlock = null;
                    lastBlockHolder = null;
                }

                // return block to pool
            }
        }
    }

    public void SetSelectedBlock(BlockInstance newSelectedBlock)
    {
        selectedBlock = newSelectedBlock;
    }
}
