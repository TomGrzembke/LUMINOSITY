using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelManager : MonoBehaviour
{
    #region Variables

    #endregion

    #region Access
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    #endregion

    public void OnHover()
    {
        anim.SetBool("isHovering", true);
    }  
    
    public void OnHoverExit()
    {
        anim.SetBool("isHovering", false);
    }
}

