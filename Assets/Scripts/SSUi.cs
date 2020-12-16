using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSUi : MonoBehaviour
{
    public static SSUi instance;
    public Animator animator;

    void Awake()
    {
        instance = this;
    }
    public void SSImageEnable()
    {
        animator.SetBool("isSSOn",true);
    }
    public void SSImageDisable()
    {
        animator.SetBool("isSSOn", false);
    }
}
