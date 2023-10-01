using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BlockManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Object blockPrefab;
    [SerializeField] Object buttonPrefab;
    [SerializeField] Object comfBarPrefab;

    [Header("Other")]
    [SerializeField] SO_Level_BlockList blockListSO;
    public Dictionary<BlockInstance, bool> blocksInPool; // block instance, bool isInInPool?
    [SerializeField] Transform buttonParent;
    [SerializeField] Transform comfBarParent;
    [SerializeField] PlayerControls playerCont;
    [SerializeField] AnimationCurve distanceMultiplierCurve;
    [SerializeField] Vector2 comfBarOffset;

    void Start()
    {
        blocksInPool = new Dictionary<BlockInstance, bool>();

        if(blockListSO != null && blockListSO.blockList.Length > 0)
        {
            PopulatePool();
        }
    }

    public void UpdateBlockComfortScores()
    {
        foreach(BlockInstance block in blocksInPool.Keys)
        {
            // Only calculate discomfort score if the block is not in the pool (i.e. in play)
            if (!blocksInPool[block])
                block.comfortScore = CalculateComfortScore(block);
        }
    }

    float CalculateComfortScore(BlockInstance targetBlock)
    {
        float scoreAverage = 0;
        int numInCloseProx = 0;

        SO_Block targetBlockInfo = targetBlock.blockInfo;

        foreach (BlockInstance blockFromList in blocksInPool.Keys)
        {
            if (blockFromList != targetBlock)
            {
                float distance = Vector2.Distance(targetBlock.transform.position, blockFromList.transform.position);

                foreach (ComfortValue comfortVal in targetBlockInfo.relationships)
                {
                    #region Grab comfortvalblock from list
                    bool comValBlockIsInPlay = false;
                    foreach(BlockInstance blockCheck in blocksInPool.Keys)
                    {
                        if(blockCheck.blockInfo == comfortVal.targetBlock)
                        {
                            comValBlockIsInPlay = !blocksInPool[blockCheck];
                        }
                    }
                    #endregion

                    if (comValBlockIsInPlay)
                    {
                        if (comfortVal.targetBlock == blockFromList.blockInfo)
                        {
                            scoreAverage += comfortVal.value * distanceMultiplierCurve.Evaluate(distance);
                        }

                        else
                            scoreAverage += .15f * distanceMultiplierCurve.Evaluate(distance);

                        Debug.Log(distanceMultiplierCurve.Evaluate(distance));
                    }   
                }
            }

        }
        /*
        if(scoreAverage != 0)
            scoreAverage /= numInCloseProx;*/

        PlaceComfortBar(targetBlock, comfBarParent.GetChild(blocksInPool.Keys.
                ToList().IndexOf(targetBlock)).GetComponent<RectTransform>());

        Debug.Log($"{targetBlock.blockInfo.blockName}'s preference score is {scoreAverage}, " +
            $"surrounded by {numInCloseProx} other block(s) in close proximity.");
        return scoreAverage;
    }

    /*float DistanceMultiplier(float distance)
    {
        if (distance >= 5)
            return 0;

        if (distance >= 4)
            return .25f;

        else if (distance >= 3)
            return .5f;

        else if (distance >= 2)
            return .75f;

        else if (distance >= 1)
            return 1;

        else
            return 1.5f;
    }*/

    void PlaceComfortBar(BlockInstance targetBlock, RectTransform comfortBar)
    {
        comfortBar.position = Camera.main.WorldToScreenPoint(targetBlock.transform.position + (Vector3)comfBarOffset);

        Image cmfBarFill = comfortBar.Find("Fill").GetComponent<Image>();
        if (cmfBarFill != null)
            cmfBarFill.fillAmount = (targetBlock.comfortScore / 2) + .5f;

        Debug.Log($"Setting {targetBlock.blockInfo.blockName}'s bar: {targetBlock.comfortScore} / 2 + .5 = {(targetBlock.comfortScore / 2) + .5f} at... {Time.time}");

        TextMeshProUGUI cmfBarTMP = comfortBar.GetComponentInChildren<TextMeshProUGUI>();
        if (cmfBarTMP != null)
            cmfBarTMP.text = targetBlock.blockInfo.blockName;
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

            Button instantiatedButton = Instantiate(buttonPrefab, buttonParent).GetComponent<Button>();
            BlockInstance correspondingBlock = blocksInPool.Keys.ElementAt(i);
            instantiatedButton.onClick.AddListener( delegate { playerCont.SetSelectedBlock(correspondingBlock); });
            instantiatedButton.onClick.AddListener( delegate { RemoveBlockFromPool(correspondingBlock); });

            instantiatedButton.GetComponentInChildren<TextMeshProUGUI>().text = instBlockInst.blockInfo.blockName;
            instantiatedButton.transform.Find("Image").GetComponent<Image>().color = instBlockInst.blockInfo.color;

            #endregion

            #region Instantiate Block Comfort Bars

            Instantiate(comfBarPrefab, comfBarParent).GetComponent<Image>();

            #endregion
        }
    }

    public void ReturnBlockToPool(BlockInstance block)
    {
        if (blocksInPool.ContainsKey(block))
        {
            blocksInPool[block] = true;
            block.gameObject.SetActive(false);
            buttonParent.GetChild(blocksInPool.Keys.ToList().IndexOf(block)).gameObject.SetActive(true);
            block.transform.position = Vector3.down * 100;

            UpdateBlockComfortScores();
        }
    }

    public void RemoveBlockFromPool(BlockInstance block)
    {
        if (blocksInPool.ContainsKey(block))
        {
            blocksInPool[block] = false;
            block.gameObject.SetActive(true);
            buttonParent.GetChild(blocksInPool.Keys.ToList().IndexOf(block)).gameObject.SetActive(false);
        }
            
    }
}
