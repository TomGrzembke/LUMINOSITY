using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DwayneBehavior : MonoBehaviour, IDataPersistence
{
    #region Variables
    enum State
    {
        idle,
        sitting,
        goTo

    }

    /// <summary>
    /// An overview of Dwaynes current behavior
    /// </summary>
#pragma warning disable CS0414
    [SerializeField] State currentState;
#pragma warning restore CS0414 
    [SerializeField] Transform startTrans;
    [SerializeField] Transform resetTrans;

    #endregion

    #region Access
    Animator anim;
    NavMeshAgent agent;
    Rigidbody2D rb;
    Collider2D standingCollider;
    SpriteRenderer sr;
    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        standingCollider = GetComponent<Collider2D>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    #endregion


    public IEnumerator SetMovePosition(Vector3 targetVec3)
    {
        PrepareMovement();
        agent.destination = targetVec3;
        float xMoveDir = targetVec3.x - transform.position.x;
        if (xMoveDir < 0)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
        yield return new WaitForSeconds(.5f);
        bool reachedDestination = agent.velocity.magnitude < 0.1f;
        while (!reachedDestination)
        {
            reachedDestination = agent.velocity.magnitude < 0.1f;
            yield return null;
        }
        PrepareSitting();

    }

    public IEnumerator SetMovePointList(List<Vector3> targetList)
    {
        PrepareMovement();
        int repeatAmount = targetList.Count;
        for (int i = 0; i < repeatAmount; i++)
        {
            agent.destination = targetList[0];
            float xMoveDir = targetList[0].x - transform.position.x;
            if (xMoveDir < 0)
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
            }

            yield return new WaitForSeconds(.1f);
            bool reachedDestination = agent.velocity.magnitude < 0.1f;
            while (!reachedDestination)
            {
                reachedDestination = agent.velocity.magnitude < 0.1f;
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
            targetList.RemoveAt(0);
        }
        PrepareSitting();
    }

    void PrepareSitting()
    {
        anim.SetBool("isRolling", false);
        anim.SetBool("isSitting", true);
        currentState = State.sitting;
        DisableFalling();
    }


    void PrepareMovement()
    {
        currentState = State.goTo;
        DisableFalling();
        agent.enabled = true;
        agent.ResetPath();
        anim.SetBool("isRolling", true);
        anim.SetBool("isSitting", false);
        anim.SetBool("isFalling", false);
    }
    void DisableFalling()
    {
        rb.gravityScale = 0;
        standingCollider.enabled = false;
    }

    public void ResetToStart()
    {
        agent.enabled = false;
        transform.position = startTrans.position;
    }

    public void ResetDwayne()
    {
        DisableFalling();
        agent.enabled = true;
        if (resetTrans)
            agent.Warp(resetTrans.position);
        rb.velocity = Vector2.zero;
    }

    public IEnumerator DwayneBonked()
    {
        anim.SetBool("isFalling", true);
        yield return new WaitForSeconds(3);
        anim.SetBool("isFalling", false);
    }
    #region Setters
    public void SetSpeed(int speed)
    {
        agent.speed = speed;
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        agent.Warp(data.dwayneData.dwaynePos);
        standingCollider.enabled = data.dwayneData.dwayneColliderEnabled;
        agent.enabled = data.dwayneData.dwayneAgentEnabled;
        rb.gravityScale = data.dwayneData.gravityScale;
    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.dwayneData.dwaynePos = transform.position;
        data.dwayneData.dwayneColliderEnabled = standingCollider.enabled;
        data.dwayneData.dwayneAgentEnabled = agent.enabled;
        data.dwayneData.gravityScale = rb.gravityScale;
    }
    #endregion
}

/// <summary>
/// This Class contains all variables of the game which should be saved
/// </summary>
[System.Serializable]
public class DwayneData
{
    public Vector3 dwaynePos;
    public float gravityScale = 0;
    public bool dwayneColliderEnabled;
    public bool dwayneAgentEnabled;
}
