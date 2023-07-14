
using System.Collections.Generic;
using UnityEngine;

public class FountainManager : MonoBehaviour, IDataPersistence
{
    #region Access 
    [SerializeField] List<ItemSO> itemSOs = new();
    [SerializeField] SpriteRenderer[] slotSRs;
    [SerializeField] SlotManager slotManager;
    [SerializeField] StoryManager storyManager;
    [SerializeField] GameObject eUICanvas;
    [SerializeField] SoundController soundController;
    PlayerController playerController;
    #endregion

    private void Start()
    {
        UpdateSlots();
        Invoke(nameof(UpdateSlots), 0.01f);
    }

    public void AddNewItem(ItemSO newItemSO)
    {
        itemSOs.Add(newItemSO);
        if (itemSOs.Count > 2)
        {
            storyManager.StartCoroutine(storyManager.DwayneAltar());
            eUICanvas.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
        }
        if (!slotManager.CheckHasItems())
            eUICanvas.SetActive(false);
        UpdateSlots();
        soundController.PlayDisposeSfx();
    }

    void UpdateSlots()
    {
        for (int i = 0; i < itemSOs.Count; i++)
        {
            if(itemSOs[i] == null)
            {
                itemSOs.RemoveAt(i);
                i--;
            }
        }

        if (itemSOs.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < itemSOs.Count; i++)
        {
            slotSRs[i].sprite = itemSOs[i].itemSprite;
            slotSRs[i].enabled = true;
        }
    }

    public List<ItemSO> GetItemList()
    {
        return itemSOs;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (slotManager.CheckHasItems())
                eUICanvas.SetActive(true);

            playerController = collision.GetComponent<PlayerController>();
            playerController.SetFountainManager(this);
            UpdateSlots();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eUICanvas.SetActive(false);

            playerController.SetFountainManager(null);
            UpdateSlots();
        }
    }

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {

        this.itemSOs = data.altarItemSos;
    }
    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.altarItemSos = this.itemSOs;
    }
    #endregion
}
