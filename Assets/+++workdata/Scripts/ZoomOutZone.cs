using Cinemachine;
using UnityEngine;

public class ZoomOutZone : MonoBehaviour
{
    #region Variables
    public enum Scene
    {
        cliff,
        bridge,
        city
    }

    public Scene currentScene;
    #endregion

    #region Access
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] CinemachineVirtualCamera mainCamCinemachine;
    [SerializeField] CinemachineVirtualCamera explosionCamera;
    [SerializeField] CinemachineVirtualCamera bridgeCam;
    [SerializeField] CinemachineVirtualCamera cityCam;
    [SerializeField] DialogueManager dialogueManager;

    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        switch (currentScene)
        {
            case Scene.cliff:
                PreparCamForSmoothSwap();
                explosionCamera.Priority = 13;
                break;
            case Scene.bridge:
                PreparCamForSmoothSwap();
                bridgeCam.Priority = 13;
                break;
            case Scene.city:
                PreparCamForSmoothSwap();
                cityCam.Priority = 13;
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        switch (currentScene)
        {
            case Scene.cliff:
                PreparCamForSmoothSwap();
                explosionCamera.Priority = 10;
                break;
            case Scene.bridge:
                PreparCamForSmoothSwap();
                bridgeCam.Priority = 10;
                break;
            case Scene.city:
                PreparCamForSmoothSwap();
                cityCam.Priority = 10;
                break;
            default:
                break;
        }
    }
    private void PreparCamForHardSwap()
    {
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Custom;
        cinemachineBrain.m_DefaultBlend.m_Time = 0f;
    }

    private void PreparCamForSmoothSwap()
    {
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        cinemachineBrain.m_DefaultBlend.m_Time = 2f;
    }

    #region Getters

    #endregion

    #region Setters

    #endregion

}

