using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AnimationModule;

public class HandGrabAnimation : MonoBehaviour
{
    public string inputName;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(inputName) >= 0.01f)
        {
            if (!animator.GetBool("IsGrabbing"))
                animator.SetBool("IsGrabbing", true);
        }
        else
        {
            if (animator.GetBool("IsGrabbing"))
                animator.SetBool("IsGrabbing", false);
        }
    }
}
