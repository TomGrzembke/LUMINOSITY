using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The behavior of keians AI in the main scene
/// </summary>
public class KeianMainSceneBehavior : MonoBehaviour, IDataPersistence
{
    #region Access
    /// <summary>
    /// The child objects of the player, which are used for the pathing of keian. 0:TopLeft, 1:TopRight, 2:BotLeft, 3:BotRight
    /// </summary>
    [SerializeField] Transform[] movePoints;

    /// <summary>
    /// Keeps track wether keian isMoving or not
    /// </summary>
    [SerializeField] bool isMoving;
    /// <summary>
    /// The NavmeshAgent component of keian
    /// </summary>
    NavMeshAgent agent;
    /// <summary>
    /// Access to the playerController
    /// </summary>
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerMovement playerMovement;
    /// <summary>
    /// The current Point where Keian should move to
    /// </summary>
    Vector3 currentMovePoint;
    /// <summary>
    /// The ID of the current movepoint that should be used
    /// </summary>
    int currentMovePointID;
    /// <summary>
    /// The ID of the last movepoint, which is used in the CheckForNewPos method
    /// </summary>
    int lastMovePointID;
    /// <summary>
    /// Is active when the enemy is repositioning his follow point 
    /// </summary>
    bool checksForNewIdlePos;
    /// <summary>
    /// Keeps track wether keian should turn in the looking direction of the player when he readjusted
    /// </summary>
    bool turnToPlayerAfterReadjusting;
    /// <summary>
    /// Acces to the animator component on this GameObject
    /// </summary>
    Animator anim;

    [SerializeField] WalkState walkState;
    public enum WalkState
    {
        follows,
        focused
    }

    /// <summary>
    /// Gets the Access to the navmeshAgent and prevents his agent from rotating the z axis
    /// </summary>
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        anim = GetComponent<Animator>();
    }
    #endregion


    private void OnEnable()
    {
        currentMovePointID = 1;
    }

    /// <summary>
    /// Keeps track of the remaining distance of the destination of the agent and updates the animator
    /// Searches the current movepoint and executes the setAgentPosition method
    /// </summary>
    private void FixedUpdate()
    {
        if (agent.velocity == Vector3.zero)
        {
            anim.SetBool("isMoving", false);
        }

        TurnAnimation();
        CheckFollowMovement();
        if (walkState == WalkState.focused) return;
        currentMovePoint = movePoints[currentMovePointID].position;


    }

    /// <summary>
    /// Normalizes the movement var for the Animator
    /// </summary>
    void TurnAnimation()
    {
        if (isMoving) return;
        if (!turnToPlayerAfterReadjusting) return;

        float movementX = playerMovement.GetCurrentIdlePoint().x;
        float movementY = playerMovement.GetCurrentIdlePoint().y;

        movementX = NormalizeValue(movementX);
        movementY = NormalizeValue(movementY);

        turnToPlayerAfterReadjusting = false;
        anim.SetFloat("moveDirX", movementX);
        anim.SetFloat("moveDirY", movementY);

    }

    /// <summary>
    /// Normalizes the given value to either 1, 0 or -1
    /// </summary>
    /// <param name="value">The pre normalized value</param>
    /// <returns>The normalized value</returns>
    float NormalizeValue(float value)
    {
        switch (value)
        {
            case float n when n > 0 && n != 0:
                return value = 1;
            case float n when n < 0 && n != 0:
                return value = -1;
            default:
                return value = 0;
        }
    }

    void CheckFollowMovement()
    {
        Movement();
        if (!(walkState == WalkState.follows)) return;

        if (!turnToPlayerAfterReadjusting) //Prevents Keian from turning in the playerposition before he reached his new point when he readjusts while the player idles
        {
            isMoving = false;
        }
        else if (turnToPlayerAfterReadjusting)
        {
            Invoke("IsMovingFalse", .1f);
        }

        if (!checksForNewIdlePos)
        {
            StartCoroutine(CheckForNewPosition());
        }

        agent.SetDestination(currentMovePoint);
    }

    private void Movement()
    {
        if (agent.velocity == Vector3.zero) return;

        isMoving = true;
        checksForNewIdlePos = false;
        float lookX = agent.destination.x - transform.position.x;
        float lookY = agent.destination.y - transform.position.y;

        anim.SetBool("isMoving", true);
        anim.SetFloat("moveDirX", lookX);
        anim.SetFloat("moveDirY", lookY);
        if (walkState == WalkState.follows)
            agent.SetDestination(currentMovePoint);
    }

    /// <summary>
    /// Switches keian to the focused state and lets him go to the target pos
    /// </summary>
    /// <param name="targetPos"></param>
    public void FocusedMovement(Vector3 targetPos)
    {
        walkState = WalkState.focused;
        agent.stoppingDistance = 0;
        agent.SetDestination(targetPos);

    }

    public void FollowMovement()
    {
        walkState = WalkState.follows;
        agent.stoppingDistance = 2.9f;
    }

    /// <summary>
    /// Checks if the player still isnt moving and adjusts the following point of keian accordingly
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckForNewPosition()
    {
        checksForNewIdlePos = true;

        yield return new WaitForSeconds(0.5f);
        if (agent.velocity != Vector3.zero)
        {
            checksForNewIdlePos = false;
            yield break;
        }

        yield return new WaitForSeconds(6);
        if (agent.velocity == Vector3.zero)
        {
            float idlePointX = playerMovement.GetCurrentIdlePoint().x;
            float idlePointY = playerMovement.GetCurrentIdlePoint().y;

            if (idlePointX == 1)
            {
                currentMovePointID = 2;
            }
            else if (idlePointX == -1)
            {
                currentMovePointID = 1;
            }
            else if (idlePointY == 1)
            {
                currentMovePointID = 3;
            }
            else if (idlePointY == -1)
            {
                currentMovePointID = 0;
            }

            if (lastMovePointID != currentMovePointID) //Only gives the order to face the same direction as the player when he stops moving if the currentMovePoint was changed in this method
            {
                isMoving = true;
                turnToPlayerAfterReadjusting = true;
            }
            lastMovePointID = currentMovePointID;
            checksForNewIdlePos = false;
        }
    }

    /// <summary>
    /// Checks where the currentMovepoint should be and adjusts it according to the player facing direction
    /// </summary>
    public void UpdateMovementPoint()
    {
        float idlePointX = playerMovement.GetCurrentIdlePoint().x;
        float idlePointY = playerMovement.GetCurrentIdlePoint().y;

        if (idlePointX == 1)
        {
            currentMovePointID = 2;
        }
        else if (idlePointX == -1)
        {
            currentMovePointID = 1;
        }
        else if (idlePointY == 1)
        {
            currentMovePointID = 3;
        }
        else if (idlePointY == -1)
        {
            currentMovePointID = 0;
        }
    }

    /// <summary>
    /// Checks if the player has entered the trigger and executes UpdateMovementPoint()
    /// </summary>
    /// <param name="collision">The object that collided witht the triggers</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UpdateMovementPoint();
        }
    }

    #region Setter
    /// <summary>
    /// Sets the current move point to the given context
    /// </summary>
    /// <param name="newMovePoint"> Sets the current move point to the given context</param>
    public void ChangeCurrentMovePoint(Vector3 newMovePoint)
    {
        currentMovePoint = newMovePoint;
    }

    /// <summary>
    /// Sets isMoving to false
    /// </summary>
    void IsMovingFalse()
    {
        isMoving = false;
    }

    public void SetSpeed(float value)
    {
        agent.speed = value;
    }
    #endregion

    #region Save
    /// <summary>
    /// Loads the data from the json file at the start of the game
    /// </summary>
    /// <param name="data">Data which origin is a json from the system , which gets converted by the data handler to get passed here </param>
    public void LoadData(GameData data)
    {
        transform.position = data.keianPos;

    }

    /// <summary>
    /// Saves the wanted data when quitting the application or when the game is saved
    /// </summary>
    /// <param name="data">Data which will be converted to a json in the system by the DataHandler</param>
    public void SaveData(ref GameData data)
    {
        data.keianPos = this.transform.position;

    }
    #endregion
}
