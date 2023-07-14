
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour, IDataPersistence
{
    #region Access
    /// <summary>
    /// The given scriptable Objects which contains variables for the slot to display
    /// </summary>
    [SerializeField] ItemSO itemSO;
    /// <summary>
    /// The manager for the payer inventory
    /// </summary>
    [SerializeField] SlotManager slotManager;

    [SerializeField] FountainManager fountainManager;
    #endregion

    private void Start()
    {
        Invoke(nameof(CheckIfThisWasCollected), 0.01f);
        Invoke(nameof(CheckIfThisWasCollected), 0.5f);
        Invoke(nameof(CheckIfThisWasCollected), 1f);
    }
    /// <summary>
    /// Checks if this was collected in the recent save state
    /// </summary>
    void CheckIfThisWasCollected()
    {
        if (slotManager.CheckIfIDIsInIDList(itemSO.itemID))
        {
            Destroy(gameObject);
        }

        FountainLogic();
    }

    void FountainLogic()
    {
        if (fountainManager == null)
        {
            Debug.Log("Theres no refernce for the fountainmanager");
            return;
        }

        List<ItemSO> tempItemSO = new List<ItemSO>();
        tempItemSO = fountainManager.GetItemList();

        for (int i = 0; i < tempItemSO.Count; i++)
        {
            if (itemSO.itemID == tempItemSO[i].itemID)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            slotManager.AddItem(this);
            Destroy(gameObject);
        }
    }

    #region Getters
    /// <summary>
    /// Returns the given ItemScripatableObject
    /// </summary>
    /// <returns></returns>
    public ItemSO GetItemSo()
    {
        return itemSO;
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        
    }
    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        //data.itemSos = this.itemSos;
    }
    #endregion
}

