using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach the GameObject under the camera to avoid flickering when moving and not adjusting the mouse
/// </summary>
public class CursorLightBehavior : MonoBehaviour
{
    #region Access
    Animator cursorLightAnim;
    Light2D cursorLight;
    void Awake()
    {
        cursorLightAnim = GetComponent<Animator>();
        cursorLight = GetComponent<Light2D>();
    }
    #endregion

    void FixedUpdate()
    {
        LightPosUpdate();
    }

    /// <summary>
    /// Updates the current light position in relation to the cursor pos
    /// </summary>
    private void LightPosUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
    }

    /// <summary>
    /// can be used for a flashing effect onclick
    /// </summary>
    public void CursorClicked()
    {
        cursorLightAnim.SetTrigger("isClicked");
    }

    public void CursorLightPurple()
    {
        cursorLight.color = Color.magenta;
    } 
    public void CursorLightBlue()
    {
        cursorLight.color = Color.blue;
    }
}

