using TMPro;
using UnityEngine;

public class QuestsManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] QuestSO currentMainQuestSO;
    [SerializeField] QuestSO currentQuest1SO;

    [SerializeField] TextMeshProUGUI questTextMainName;
    [SerializeField] TextMeshProUGUI questMainText;

    [SerializeField] TextMeshProUGUI questText1Name;
    [SerializeField] TextMeshProUGUI quest1Text;

    [SerializeField] TextMeshProUGUI mainTextName;
    [SerializeField] TextMeshProUGUI mainText;

    [SerializeField] TextMeshProUGUI ingameMainQuestText;
    private void Start()
    {
        UpdateMainQuest();
        UpdateQuest1();
    }

    public void UpdateMainQuest()
    {
        ingameMainQuestText.text = currentMainQuestSO.questShortDescripton;
        mainText.text = currentMainQuestSO.questDetailedDescription;
        questTextMainName.text = currentMainQuestSO.questName;
        questMainText.text = currentMainQuestSO.questShortDescripton;
    }

    public void UpdateQuest1()
    {
        questText1Name.text = currentQuest1SO.questName;
        quest1Text.text = currentQuest1SO.questShortDescripton;

    }

    public void OnMainQuestClick()
    {
        mainTextName.text = currentMainQuestSO.questName;
        mainText.text = currentMainQuestSO.questDetailedDescription;
    }

    public void OnQuest1Click()
    {
        mainTextName.text = currentQuest1SO.questName;
        mainText.text = currentQuest1SO.questDetailedDescription;
    }

    #region Setter
    public void SetCurrentMainQuestSO(QuestSO newQuestSO)
    {
        currentMainQuestSO = newQuestSO;
        UpdateMainQuest();
    }

    public void SetCurrentQuest1SO(QuestSO newQuestSO)
    {
        currentQuest1SO = newQuestSO;
        UpdateQuest1();
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        if (data.currentQuestSO == null) return;

        this.currentMainQuestSO = data.currentQuestSO;
        UpdateMainQuest();
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.currentQuestSO = currentMainQuestSO;
    }
    #endregion
}
