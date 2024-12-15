using UnityEngine;

public class Parallex : MonoBehaviour
{
    float startposX;
    float startposY;
    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
    }

    void Update()
    {
        float distX = (cam.transform.position.x * parallaxEffectX);
        float distY = (cam.transform.position.y * parallaxEffectY);
        transform.position = new(startposX + distX, startposY + distY, transform.position.z);
    }
}
