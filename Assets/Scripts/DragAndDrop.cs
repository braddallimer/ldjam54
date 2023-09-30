using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] string rayTag;
    [SerializeField] BlockInstance selectedBlock;

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
                    if (targetHolder != null)
                    {
                        if (targetHolder.heldBlock != null)
                            targetHolder.ReplaceBlock(selectedBlock);

                        else
                            targetHolder.AddBlockToHolder(selectedBlock);

                        SetSelectedBlock(null);
                    }

                    else 
                    {
                        // return block to pool
                    }
                }

                else
                {
                    if (targetHolder != null)
                    {
                        if (targetHolder.heldBlock != null)
                        {
                            SetSelectedBlock(targetHolder.heldBlock);
                            targetHolder.RemoveBlockFromHolder();
                        }
                    }
                }
            }
        }
    }

    public void SetSelectedBlock(BlockInstance newSelectedBlock)
    {
        selectedBlock = newSelectedBlock;
    }
}
