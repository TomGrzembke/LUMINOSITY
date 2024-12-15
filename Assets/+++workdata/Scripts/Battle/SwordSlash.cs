using UnityEngine;

/// <summary>
/// The behavior of each player slash shot
/// </summary>
public class SwordSlash : MonoBehaviour
{
    #region stats
    /// <summary>
    /// The amount of speed of the swordSlashProjectile
    /// </summary>
    public float speed = 10;
    /// <summary>
    /// The GameObject which got in the trigger of the swordslash
    /// </summary>
    GameObject target;
    #endregion

    #region Start
    /// <summary>
    /// Invokes the DestroyItself method
    /// </summary>
    private void Start()
    {
        Invoke("DestroyItself", 5f);

    }

    /// <summary>
    /// Transforms the position of the swordSlash in the given direction
    /// </summary>
    void FixedUpdate()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    #endregion

    #region special methods (DestroyItself,DestroyItself)
    /// <summary>
    /// Destroys the gameObject
    /// </summary>
    void DestroyItself()
    {
        Destroy(gameObject);
    }
    #endregion

    /// <summary>
    /// Is used for the trigger component on the bullet
    /// </summary>
    /// <param name="collision">Collision is the GameObject which triggered the swordSlash</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Props"))
        {
            DestroyItself();
        }

        else if (collision.CompareTag("Enemy"))
        {
            target = collision.gameObject;

            if (target.TryGetComponent(out EnemyBehavior enemyBehavior))
                enemyBehavior.ChangeHP(-1);

            else if (target.TryGetComponent(out VaraBehavior varaBehavior))
                varaBehavior.ChangeHP(-1);
        }
    }
}

