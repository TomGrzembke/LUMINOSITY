using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Keeps track of the current speed which will be set to the agent speed
    /// </summary>
    [SerializeField] float speed;
    /// <summary>
    /// Saves the defaultSpeed of the player
    /// </summary>
    [SerializeField] float defaultSpeed;
    /// <summary>
    /// The amount of speed that will be added when the player sprints
    /// </summary>
    [SerializeField] float sprintSpeed;
    /// <summary>
    /// The storage for the movement x and ys for the wasd movement
    /// </summary>
    float movementX, movementY;
    /// <summary>
    /// The current lookPosition in 1s and 0s which is determined when the player stops moving
    /// </summary>
    Vector2 currentIdlePoint;
    /// <summary>
    /// A storage slot for the input actions
    /// </summary>
    PlayerControls inputActions;
    /// <summary>
    /// Determines the percantage of slows through bushes
    /// </summary>
    [SerializeField] float slowPercentage;
    /// <summary>
    /// Keeps track wether the player is sprinting or not
    /// </summary>
    [SerializeField] bool isSprinting;
    /// <summary>
    /// Keeps track if the player is slowed
    /// </summary>
    [SerializeField] bool isSlowed;
    /// <summary>
    /// The playerMovement Controlstate
    /// </summary>
    [SerializeField] ControlState controlState;

    public enum ControlState
    {
        playerControl,
        gameControl
    }
    #endregion

    #region Access
    /// <summary>
    /// Acces to keians script, will be ignored if null
    /// </summary>
    [SerializeField] KeianMainSceneBehavior keianBehavior;
    /// A storage for the current wasd movepoint which the agent pathes to, 0: TopLeft, 1: Up, 2: TopRight, 3: Left, 4: Right, 5: BotLeft, 6: Down, 7:
    /// </summary>
    [SerializeField] Transform currentWASDMovepoint;
    /// <summary>
    /// The array of the movepoints which are used for wasd movement
    /// </summary>
    [SerializeField] Transform[] movePoints;
    NavMeshAgent agent;
    Animator anim;
    void Awake()
    {
        inputActions = new PlayerControls();

        inputActions.PlayerControl.WASD.performed += ctx => Movement(ctx.ReadValue<Vector2>());
        inputActions.PlayerControl.WASD.canceled += ctx => Movement(ctx.ReadValue<Vector2>());
        inputActions.PlayerControl.Sprint.performed += ctx => Sprint();
        inputActions.PlayerControl.Sprint.canceled += ctx => Sprint();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    #endregion

    void Start()
    {
        agent.speed = defaultSpeed;
        currentWASDMovepoint = gameObject.transform;
    }

    /// <summary>
    /// This ensures that the animation of the player will stop when it stops moving 
    /// </summary>
    private void FixedUpdate()
    {
        if (agent.remainingDistance < 0.2f)
        {
            anim.SetBool("isMoving", false);
        }

        if (controlState == ControlState.playerControl)
            SetAgentPosition();
    }
    /// <summary>
    /// The method that handels the wasd movement of the player and the correct animations
    /// </summary>
    /// <param name="direction">The vector2 that is provided by the inputactions</param>
    void Movement(Vector2 direction)
    {
        if (!(controlState == ControlState.playerControl)) return;
        movementX = direction.x;
        movementY = direction.y;

        movementX = NormalizeValue(movementX);
        movementY = NormalizeValue(movementY);

        if (!CalculateCurrentWasdPoint())
            return; //Returns if movementX and Y is 0

        anim.SetBool("isMoving", true);
        anim.SetFloat("moveDirX", movementX);
        anim.SetFloat("moveDirY", movementY);
        currentIdlePoint = new Vector2(movementX, movementY);


        SetAgentPosition();
        SetKeianAgentSpeed(speed);

    }

    /// <summary>
    /// Normalizes the current valu of movement x and y
    /// </summary>
    float NormalizeValue(float value)
    {
        float passValue;

        switch (value)
        {
            case float n when n > 0 && n != 0:
                passValue = 1;
                break;
            case float n when n < 0 && n != 0:
                passValue = -1;
                break;
            default:
                passValue = 0;
                break;
        }

        return passValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The return determines if movement or Idle would be played in the movemént </returns>
    bool CalculateCurrentWasdPoint()
    {
        switch (movementX)
        {
            case -1:
                switch (movementY)
                {
                    case -1:
                        currentWASDMovepoint = movePoints[5];
                        break;
                    case 0:
                        currentWASDMovepoint = movePoints[3];
                        break;
                    case 1:
                        currentWASDMovepoint = movePoints[0];
                        break;
                }
                break;
            case 0:
                switch (movementY)
                {
                    case -1:
                        currentWASDMovepoint = movePoints[6];
                        break;
                    case 0:
                        currentWASDMovepoint = gameObject.transform;
                        anim.SetBool("isMoving", false);
                        return false;

                    case 1:
                        currentWASDMovepoint = movePoints[1];
                        break;
                }
                break;
            case 1:
                switch (movementY)
                {
                    case -1:
                        currentWASDMovepoint = movePoints[7];
                        break;
                    case 0:
                        currentWASDMovepoint = movePoints[4];
                        break;
                    case 1:
                        currentWASDMovepoint = movePoints[2];
                        break;
                }
                break;
        }
        return true;
    }


    /// <summary>
    /// Applies the slow (If active) and calculates it according to sprint and walking
    /// </summary>
    public void CheckForSlow()
    {
        if (!isSlowed)
        {
            if (!isSprinting)
                SetMoveSpeed(defaultSpeed);
            else
                SetMoveSpeed(sprintSpeed);
            return;
        }

        SetMoveSpeed(speed * slowPercentage);
        SetKeianAgentSpeed(speed);
    }

    /// <summary>
    /// Forces the Player to move to the right
    /// </summary>
    public void MoveToRight()
    {
        controlState = ControlState.gameControl;
        var movePos = new Vector3(movePoints[4].position.x + 5, movePoints[4].position.y, movePoints[4].position.z);
        agent.ResetPath();
        agent.SetDestination(movePos);
        anim.SetBool("isMoving", true);
        anim.SetFloat("moveDirX", 1);
    }

    /// <summary>
    /// This gets called when the player should automatically move to the cliff
    /// </summary>
    public void MoveToCliff(Vector3 position)
    {
        agent.SetDestination(position);
        anim.SetBool("isMoving", true);
        anim.SetFloat("moveDirY", 1);
    }

    /// <summary>
    /// This gets executed when the player presses shift 
    /// </summary>
    void Sprint()
    {
        if (!isSprinting)
        {
            speed = sprintSpeed;
            agent.speed = speed;
            isSprinting = true;
            CheckForSlow();
        }
        else if (isSprinting)
        {
            speed = defaultSpeed;
            agent.speed = speed;
            isSprinting = false;
            CheckForSlow();
        }
        SetKeianAgentSpeed(speed);
    }

    /// <summary>
    /// Changes some player configurations for cut scenes
    /// </summary>
    public void UpdateForCutscene()
    {
        currentWASDMovepoint = transform;
        anim.SetBool("isMoving", false);
        anim.SetFloat("moveDirX", 0);
        anim.SetFloat("moveDirY", 1);
        currentIdlePoint = new Vector2(0, 1);
    }

    public void ResetWASDPoint()
    {
        movementX = 0;
        movementY = 0;
        CalculateCurrentWasdPoint();
        currentWASDMovepoint = gameObject.transform;
    }
    #region Getters
    /// <summary>
    /// The Getter for the current Idle point
    /// </summary>
    /// <returns>returns the currentIdlePoint vector2 </returns>
    public Vector2 GetCurrentIdlePoint()
    {
        return currentIdlePoint;
    }

    public float GetPlayerSpeed()
    {
        return speed;
    }

    #endregion

    #region Setters

    /// <summary>
    /// This sets the agents destination with the context of the movement input
    /// </summary>
    public void SetAgentPosition()
    {
        agent.SetDestination(currentWASDMovepoint.position);

    }
    /// <summary>
    /// Sets the Speed for the companion
    /// </summary>
    /// <param name="value"></param>
    void SetKeianAgentSpeed(float value)
    {
        if (keianBehavior == null) return;
        keianBehavior.SetSpeed(value);
    }
    /// <summary>
    /// Assigns the given context to the corresponding float and agent component
    /// </summary>
    /// <param name="speed"></param>
    public void SetMoveSpeed(float newSpeed)
    {
        speed = newSpeed;
        agent.speed = speed;
    }
    /// <summary>
    /// Sets the isSlowed bool to the new value
    /// </summary>
    public void SetIsSlowed(bool value)
    {
        isSlowed = value;
        CheckForSlow();
        SetKeianAgentSpeed(speed);
    }

    public void SetControlState(ControlState newControlState)
    {
        controlState = newControlState;

        if (controlState == ControlState.gameControl)
            ResetWASDPoint();
    }  
    public void ReenableMovement()
    {
        controlState = ControlState.playerControl;
    }
    #endregion

    #region OnEnable/Disable
    /// <summary>
    /// Gets called when this script is enabled
    /// </summary>
    public void OnEnable()
    {
        inputActions.Enable();
    }

    /// <summary>
    /// Gets called when this script is disabled
    /// </summary>
    public void OnDisable()
    {
        inputActions.Disable();
    }

    #endregion
}

