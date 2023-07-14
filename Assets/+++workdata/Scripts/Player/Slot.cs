using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    #region Variables
    bool occupied;
    [SerializeField] int slotID;
    #endregion

    #region Access
    ItemSO itemSo;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    #endregion

    /// <summary>
    /// Updates the Slot Visuals
    /// </summary>
    void UpdateSlotProperties()
    {
        try
        {
            itemIcon.enabled = true;
            itemIcon.sprite = itemSo.itemSprite;
            itemName.text = itemSo.name;
            occupied = true;
        }
        catch
        {
            itemIcon.enabled = false;
            itemName.text = null;
            occupied = false;
        }
    }
    #region Setters
    /// <summary>
    /// Stores the given ItemSo and Updates the visuals
    /// </summary>
    /// <param name="newItemSo"></param>
    public void SetItemSo(ItemSO newItemSo)
    {
        itemSo = newItemSo;
        UpdateSlotProperties();
    }
    #endregion

    #region Getters
    public bool GetOccupied()
    {
        return occupied;
    }
    #endregion


}
