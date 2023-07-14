using UnityEngine;

/// <summary>
/// Is used for keian and his hiding spot. He will give a signal based on a rng
/// </summary>
public class KeianBattleBehavior : MonoBehaviour
{
    [SerializeField] int keianDamage = 1;
    [SerializeField] GameObject spaceUI;
    /// <summary>
    /// The Animator of Keian
    /// </summary>
    [SerializeField] Animator keianAnimator;
    /// <summary>
    /// The amount of time where the player could react to keian signal
    /// </summary>
    [SerializeField] float timeWindowToReact;
    /// <summary>
    /// ´The Percent chance that keian attacks which will get checked every second in the enemy turn
    /// </summary>
    [SerializeField] int keianAttackPercent;
    /// <summary>
    /// Keeps track of the Combat
    /// </summary>
    bool isInCombat;
    /// <summary>
    /// Will be able to attack if this is true
    /// </summary>
    bool canAttack;

    /// <summary>
    /// Rolls for keian to give a signal or not
    /// </summary>
    void CheckRandom()
    {
        int randNum;
        randNum = Random.Range(0, 101);
        if (randNum <= keianAttackPercent)
        {
            KeianCouldAttack();
        }
        else
        {
            canAttack = false;
            spaceUI.SetActive(false);
        }
    }
    /// <summary>
    /// This will be executed when the checkRandom method rolled that keian couldAttack and gives the player a signal via the animator
    /// </summary>
    public void KeianCouldAttack()
    {
        keianAnimator.SetTrigger("canAttack");
    }
    /// <summary>
    /// Gets executed when the Player reacts while Keian gives a Signal
    /// </summary>
    public void Attacks()
    {
        keianAnimator.SetTrigger("isAttacking");
        spaceUI.SetActive(false);
        try
        {
            GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyBehavior>().ChangeHP(-keianDamage);
        }
        catch
        {
            Debug.Log("Couldnt find an Enemy");
        }
    }
    /// <summary>
    /// An empty method used to have an empty Idle for keian
    /// </summary>
    void Nothing() //is used to get an empty Animation
    {
        canAttack = false;
        spaceUI.SetActive(false);
    }
    /// <summary>
    /// Changes the is in combat bool and updates the animator about the new condition
    /// </summary>
    /// <param name="state">Changes the is in combat bool</param>
    public void ChangeIsInCombat(bool state)
    {
        isInCombat = state;

        if (isInCombat)
        {
            keianAnimator.SetBool("inBattle", true);
        }
        else
        {
            keianAnimator.SetBool("inBattle", false);
            canAttack = false;
        }
    }

    /// <summary>
    /// Sets the canAttack bool to true
    /// </summary>
    public void CanAttack()
    {
        canAttack = true;
        spaceUI.SetActive(true);
    }
    /// <summary>
    /// Sets the canAttack bool to false
    /// </summary>
    public void CantAttack()
    {
        canAttack = false;
        spaceUI.SetActive(false);
    }
    /// <summary>
    /// returns the canAttack bool
    /// </summary>
    /// <returns>returns the canAttack bool</returns>
    public bool GetCanAttack()
    {
        return canAttack;
    }
}
