using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BlockPool : MonoBehaviour
{
    [SerializeField] SO_Level_BlockList blockListSO;
    [SerializeField] Object blockPrefab;
    [SerializeField] Object buttonPrefab;
    public Dictionary<BlockInstance, bool> blocksInPool;
    [SerializeField] Transform content;
    [SerializeField] DragAndDrop mouseControls;

    void Start()
    {
        blocksInPool = new Dictionary<BlockInstance, bool>();

        if(blockListSO != null && blockListSO.blockList.Length > 0)
        {
            PopulatePool();
        }
    }

    void PopulatePool()
    {
        for(int i = 0; i < blockListSO.blockList.Length; i++)
        {
            #region Instantiate Block Instance
            GameObject instantiatedBlock = (GameObject)Instantiate(blockPrefab);
            instantiatedBlock.TryGetComponent(out BlockInstance instBlockInst);

            if(instBlockInst != null)
            {
                blocksInPool.Add(instBlockInst, true);
                instBlockInst.blockInfo = blockListSO.blockList[i];
                instBlockInst.UpdateAppearance();
            }

            instantiatedBlock.SetActive(false);
            #endregion

            #region Instantiage Button

            Button instantiatedButton = Instantiate(buttonPrefab, content).GetComponent<Button>();
            BlockInstance correspondingBlock = blocksInPool.Keys.ElementAt(i);
            instantiatedButton.onClick.AddListener( delegate { mouseControls.SetSelectedBlock(correspondingBlock); });
            instantiatedButton.onClick.AddListener( delegate { RemoveBlockFromPool(correspondingBlock); });

            #endregion
        }
    }

    public void ReturnBlockToPool(BlockInstance block)
    {
        if (blocksInPool.ContainsKey(block))
        {
            blocksInPool[block] = true;
            block.gameObject.SetActive(false);
            content.GetChild(blocksInPool.Keys.ToList().IndexOf(block)).gameObject.SetActive(true);
        }
    }

    public void RemoveBlockFromPool(BlockInstance block)
    {
        if (blocksInPool.ContainsKey(block))
        {
            blocksInPool[block] = false;
            block.gameObject.SetActive(true);
            content.GetChild(blocksInPool.Keys.ToList().IndexOf(block)).gameObject.SetActive(false);
        }
            
    }
}
