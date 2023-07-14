
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour, IDataPersistence
{
    #region Fields
    [SerializeField] Image keianImage;
    [SerializeField] Animator collectedAnim;
    /// <summary>
    /// Access to each of the slots scripts
    /// </summary>
    [SerializeField] Slot[] slotScripts;
    /// <summary>
    /// Saves the found Item Ids for deleting the collectables when they were found already
    /// </summary>
    [SerializeField] List<int> IDList;
    /// <summary>
    /// A list that stores the ItemSos in order for the inventory saving
    /// </summary>
    [SerializeField] List<ItemSO> itemSos;
    #endregion

    void UpdateIDList()
    {
        int index = 0;
        foreach (var item in itemSos)
        {
            IDList.Add(itemSos[index].itemID);
            index++;
        }
    }
    /// <summary>
    /// Passes the given Item to the next available 
    /// </summary>
    public void AddItem(ItemBehavior newItemBehavior)
    {
        for (int i = 0; i < slotScripts.Length; i++)
        {
            if (!slotScripts[i].GetOccupied())
            {
                slotScripts[i].SetItemSo(newItemBehavior.GetItemSo());
                AddItemSo(newItemBehavior.GetItemSo());
                if (collectedAnim == null)
                {
                    Debug.Log("Collected anim wasnt referenced in SlotManager", gameObject);
                    return;
                }
                collectedAnim.SetTrigger("itemCollected");
                return;
            }
        }
    }
    /// <summary>
    /// Updates the Inventory visuals after Loading and deltes missing components
    /// </summary>
    void UpdateInventory()
    {

        int index = 0;
        foreach (var item in itemSos)
        {
            slotScripts[index].SetItemSo(itemSos[index]);
            index++;
        }

        for (int i = 0; i < itemSos.Count; i++)
        {
            if (itemSos[i] == null)
            {
                itemSos.RemoveAt(i);
                IDList.RemoveAt(i);
            }
        }

        for (int i = 3; i > index; i--)
        {
            slotScripts[index].SetItemSo(null);
        }
    }
    #region Getters
    /// <summary>
    /// Checks if the given ID is in the IDList
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CheckIfIDIsInIDList(int id)
    {
        for (int i = 0; i < IDList.Count; i++)
        {
            if (id == IDList[i])
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckHasItems()
    {
        if (itemSos.Count == 0)
            return false;
        return true;
    }
    #endregion

    #region Setters
    /// <summary>
    /// Adds the new ItemSo to the itemSos list
    /// </summary>
    /// <param name="newItemSo"></param>
    public void AddItemSo(ItemSO newItemSo)
    {
        itemSos.Add(newItemSo);
        IDList.Add(newItemSo.itemID);
    }
    /// <summary>
    /// Gets and deletes the first Item
    /// </summary>
    /// <returns></returns>
    public ItemSO GetFirstItem()
    {
        if (itemSos.Count == 0)
        {
            UpdateInventory();
            return null;
        }

        ItemSO itemSoToPass = itemSos[0];
        itemSos.RemoveAt(0);
        IDList.Clear();
        UpdateIDList();

        UpdateInventory();
        return itemSoToPass;
    }

    public List<ItemSO> GetItemSos()
    {
        try
        {
            if (itemSos[0])
            {
                return itemSos;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }

    }

    public void EnableKeianSprite()
    {
        keianImage.material = null;
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        itemSos.Clear();
        this.itemSos = data.itemSos;
        UpdateInventory();
        UpdateIDList();
    }
    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.itemSos = this.itemSos;
    }
    #endregion
}

