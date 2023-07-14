
using UnityEngine;

/// <summary>
/// The variables for the combat types, which modify the bullets
/// </summary>
public class InstantiateSettings : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// the float that is used for bullet speed
    /// </summary>
    [SerializeField] float bulletSpeed;
    /// <summary>
    /// The maximum of seconds it will take for the shootCD
    /// </summary>
    [SerializeField] float maxShootCD;
    /// <summary>
    /// The maximum amount of speed a bullet will hget when its speed is randomized
    /// </summary>
    [SerializeField] float maxBulletSpeedWhenRand;

    /// <summary>
    /// Wether the bullet will fly at the position of the player or not
    /// </summary>
    [SerializeField] bool isAiming;
    /// <summary>
    /// Wether the bullet speeed will be randomized
    /// </summary>
    [SerializeField] bool bulletSpeedRandom;

    /// <summary>
    /// The amount of bullets which will get fired in the circle pattern
    /// </summary>
    [SerializeField] int bulletAmountInCircle;
    /// <summary>
    /// The amount of bullets which will get fired in the shotgun pattern
    /// </summary>
    [SerializeField] int bulletAmountInShotgun;

    /// <summary>
    /// the zrotation for circle int will save the angles used for the shotpatterns
    /// </summary>
    int zRotationForCircle;

    /// <summary>
    /// This is used to return a random number 
    /// </summary>
    int bulletID;
    /// <summary>
    /// The amount of available attack patterns
    /// </summary>
    [SerializeField] int attackPatternAmount;
    #endregion

    #region Access
    /// <summary>
    /// The Transform, where the bullets spawn.
    /// </summary>
    [SerializeField] Transform bulletOrigin;
    /// <summary>
    /// The Transform, where the circle bullets spawn.
    /// </summary>
    [SerializeField] Transform circleBulletOrigin;

    /// <summary>
    /// The prefab of the bullet which will be instantiated
    /// </summary>
    [SerializeField] GameObject bulletPrefab;
    #endregion

    /// <summary>
    /// Updates the speed of the bullets at the start of the game
    /// </summary>
    private void Start()
    {
        bulletPrefab.GetComponent<BulletBehavior>().speed = bulletSpeed;
    }

    /// <summary>
    /// returns a random number between 0 and the amount of available patterns
    /// </summary>
    /// <returns></returns>
    int GetRandomBulletBehaviorID()
    {
        bulletID = Random.Range(0, attackPatternAmount);
        return bulletID;
    }

    /// <summary>
    /// Instantiates bullets
    /// </summary>
    /// <param name="prefab">The bullet that will be used for instantiating</param>
    public void InstantiateBullet(GameObject prefab)
    {
        int randomBulletBehaviorID = GetRandomBulletBehaviorID();


        switch (randomBulletBehaviorID)
        {
            default:
                NormalPattern(prefab);
                break;
            case 0:
                NormalPattern(prefab);
                break;
            case 1:
                CircleBulletPattern(prefab);
                break;
            case 2:
                HalfCircleBulletPattern(prefab);
                break;
            case 3:
                ShotgunPattern(prefab);
                break;
            case 4:
                DoubleShot(prefab);
                break;
        }

    }

    public void NormalPattern(GameObject prefab)
    {
        GameObject newBullet = Instantiate(prefab, bulletOrigin.position, Quaternion.identity);
        if (isAiming)
        {
            newBullet.transform.position = bulletOrigin.position;
            newBullet.transform.rotation = bulletOrigin.rotation;
        }

        if (bulletSpeedRandom)
        {
            newBullet.GetComponent<BulletBehavior>().speed = Random.Range(bulletSpeed - 5, maxBulletSpeedWhenRand);
        }
    }

    /// <summary>
    /// Instantiates bullets in a circle pattern
    /// </summary>
    /// <param name="prefab">The bullet that will be used for instantiating</param>
    public void CircleBulletPattern(GameObject prefab) //1
    {
        zRotationForCircle = -40;
        for (int i = 0; i < bulletAmountInCircle; i++)
        {
            zRotationForCircle += 45;

            circleBulletOrigin.rotation = Quaternion.Euler(0, 0, zRotationForCircle);
            GameObject newBullet = Instantiate(prefab, bulletOrigin.position, Quaternion.identity);
            newBullet.transform.position = circleBulletOrigin.position;
            newBullet.transform.rotation = circleBulletOrigin.rotation;

            if (isAiming)
            {
                newBullet.transform.position = bulletOrigin.position;
                newBullet.transform.rotation = bulletOrigin.rotation;
            }

            if (bulletSpeedRandom)
            {
                newBullet.GetComponent<BulletBehavior>().speed = Random.Range(bulletSpeed - 5, maxBulletSpeedWhenRand);
            }
        }


    }

    /// <summary>
    /// Instantiates bullets in a half circle pattern which will switch between the left and right side
    /// </summary>
    /// <param name="prefab">The bullet that will be used for instantiating</param>
    public void HalfCircleBulletPattern(GameObject prefab)//2
    {
        zRotationForCircle = -40;
        for (int i = 0; i < bulletAmountInCircle; i++)
        {
            zRotationForCircle += 22;

            circleBulletOrigin.rotation = Quaternion.Euler(0, 0, zRotationForCircle);
            GameObject newBullet = Instantiate(prefab, bulletOrigin.position, Quaternion.identity);
            newBullet.transform.position = circleBulletOrigin.position;
            newBullet.transform.rotation = circleBulletOrigin.rotation;

            if (isAiming)
            {
                newBullet.transform.position = bulletOrigin.position;
                newBullet.transform.rotation = bulletOrigin.rotation;
            }

            if (bulletSpeedRandom)
            {
                newBullet.GetComponent<BulletBehavior>().speed = Random.Range(bulletSpeed - 5, maxBulletSpeedWhenRand);
            }
        }
    }

    /// <summary>
    /// Instantiates bullets in a shotgun pattern
    /// </summary>
    /// <param name="prefab">The bullet that will be used for instantiating</param>
    public void ShotgunPattern(GameObject prefab)//3
    {
        for (int i = 0; i < bulletAmountInShotgun; i++)
        {
            zRotationForCircle = Random.Range(0, 61);

            circleBulletOrigin.rotation = Quaternion.Euler(0, 0, zRotationForCircle);
            GameObject newBullet = Instantiate(prefab, bulletOrigin.position, Quaternion.identity);
            newBullet.transform.position = circleBulletOrigin.position;
            newBullet.transform.rotation = circleBulletOrigin.rotation;

            if (isAiming)
            {
                newBullet.transform.position = bulletOrigin.position;
                newBullet.transform.rotation = bulletOrigin.rotation;
            }

            if (bulletSpeedRandom)
            {
                newBullet.GetComponent<BulletBehavior>().speed = Random.Range(bulletSpeed - 5, maxBulletSpeedWhenRand);
            }
        }
    }

    /// <summary>
    /// Instantiates bullets in a double shot pattern
    /// </summary>
    /// <param name="prefab">The bullet that will be used for instantiating</param>
    public void DoubleShot(GameObject prefab)
    {
        int lastDegree = -60;
        for (int i = 0; i < 2; i++)
        {
            lastDegree += 30;
            zRotationForCircle = lastDegree;

            circleBulletOrigin.rotation = Quaternion.Euler(0, 0, zRotationForCircle);
            GameObject newBullet = Instantiate(prefab, bulletOrigin.position, Quaternion.identity);
            newBullet.transform.position = circleBulletOrigin.position;
            newBullet.transform.rotation = circleBulletOrigin.rotation;

            if (isAiming)
            {
                newBullet.transform.position = bulletOrigin.position;
                newBullet.transform.rotation = bulletOrigin.rotation;
            }

            if (bulletSpeedRandom)
            {
                newBullet.GetComponent<BulletBehavior>().speed = Random.Range(bulletSpeed - 5, maxBulletSpeedWhenRand);
            }
        }
    }

    public void SetIsAiming(bool newValue)
    {
         isAiming = newValue;
    }
}
