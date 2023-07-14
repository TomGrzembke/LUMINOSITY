using UnityEngine;
using UnityEngine.UI;

public class ObjSaveManager : MonoBehaviour, IDataPersistence
{
    #region Access
    [SerializeField] KeianMainSceneBehavior keianMainSceneBehavior;
    [SerializeField] Image keianImage;
    #endregion

    #region Saving/Loading
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        keianMainSceneBehavior.enabled = data.keianActive;
        if (data.keianActive)
            keianImage.material = null;
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.keianActive = keianMainSceneBehavior.enabled;
    }
    #endregion
}

