using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BlockManager : MonoBehaviour
{
    [SerializeField] SO_Level_BlockList blockListSO;
    [SerializeField] Object blockPrefab;
    [SerializeField] Object buttonPrefab;
    public Dictionary<BlockInstance, bool> blocksInPool; // block instance, bool isInInPool?
    [SerializeField] Transform content;
    [SerializeField] PlayerControls playerCont;

    void Start()
    {
        blocksInPool = new Dictionary<BlockInstance, bool>();

        if(blockListSO != null && blockListSO.blockList.Length > 0)
        {
            PopulatePool();
        }
    }

    public void UpdateBlockDiscomfortScores()
    {
        foreach(BlockInstance block in blocksInPool.Keys)
        {
            // Only calculate discomfort score if the block is not in the pool (i.e. in play)
            if (!blocksInPool[block])
                block.discomfortScore = CalculateDiscomfortScore(block);
        }
    }

    float CalculateDiscomfortScore(BlockInstance targetBlock)
    {
        float scoreAverage = 0;
        int numInCloseProx = 0;

        SO_Block targetBlockInfo = targetBlock.blockInfo;

        foreach (BlockInstance blockFromList in blocksInPool.Keys)
        {
            if (blockFromList != targetBlock)
            {
                float distance = Vector2.Distance(targetBlock.transform.position, blockFromList.transform.position);

                if (distance < 4)
                    numInCloseProx++;

                foreach (DiscomfortValue discomfortVal in targetBlockInfo.relationships)
                {
                    #region Grab discomfortvalblock from list
                    bool discomValBlockIsInPlay = false;
                    foreach(BlockInstance blockCheck in blocksInPool.Keys)
                    {
                        if(blockCheck.blockInfo == discomfortVal.targetBlock)
                        {
                            discomValBlockIsInPlay = !blocksInPool[blockCheck];
                        }
                    }
                    #endregion

                    if (discomValBlockIsInPlay)
                    {
                        if (discomfortVal.targetBlock == blockFromList.blockInfo)
                        {
                            scoreAverage += discomfortVal.value * DistanceMultiplier(distance);
                        }

                        else
                            scoreAverage += .5f * DistanceMultiplier(distance);
                    }
                        
                }
            }
        }

        scoreAverage /= numInCloseProx;
        Debug.Log($"{targetBlock.blockInfo.blockName}'s preference score is {scoreAverage}");
        return scoreAverage;
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
            instantiatedBlock.transform.position = Vector3.down * 100;
            #endregion

            #region Instantiate Button

            Button instantiatedButton = Instantiate(buttonPrefab, content).GetComponent<Button>();
            BlockInstance correspondingBlock = blocksInPool.Keys.ElementAt(i);
            instantiatedButton.onClick.AddListener( delegate { playerCont.SetSelectedBlock(correspondingBlock); });
            instantiatedButton.onClick.AddListener( delegate { RemoveBlockFromPool(correspondingBlock); });

            instantiatedButton.GetComponentInChildren<TextMeshProUGUI>().text = instBlockInst.blockInfo.blockName;
            instantiatedButton.transform.Find("Image").GetComponent<Image>().color = instBlockInst.blockInfo.color;

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
            block.transform.position = Vector3.down * 100;
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
