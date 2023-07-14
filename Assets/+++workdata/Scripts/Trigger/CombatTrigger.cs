using System.Collections;
using UnityEngine;

public class CombatTrigger : MonoBehaviour, IDataPersistence
{
    [SerializeField] int sceneIDToLoad;
    [SerializeField] int combatID;
    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] SpriteRenderer spriteChild;
    [SerializeField] Animator transition;
    GameData gameData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (gameData.combatsDone[combatID]) return;

        StartCoroutine(LoadScene());

    }
    /// <summary>
    /// Loads the combat scene with the help of a given id
    /// </summary>
    IEnumerator LoadScene()
    {
        transition.SetBool("isOn", true);
        yield return new WaitForSeconds(1);
        sceneLoadManager.ChangeSceneTo(sceneIDToLoad);
    }

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        gameData = data;
        if (data.combatsDone[combatID])
        {
            try
            {
                GetComponent<Collider2D>().enabled = false;
                spriteChild.enabled = false;
            }
            catch { }
        }
        else if(!data.combatsDone[combatID])
        {
            spriteChild.enabled = true;
        }
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        if (data.combatsDone[combatID])
            GetComponent<Collider2D>().enabled = false;

    }
    #endregion
}
