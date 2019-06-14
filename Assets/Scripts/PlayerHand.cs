using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    //Amount of time between the creation of samples in a gesture
    public float TIME_BETWEEN_SAMPLES;

    public bool showSamples;

    //Temporary visualization of the sample
    public GameObject sample;

    private Animator animator;
    private float sampleTime;
    private bool grabbing;

    private void Awake()
    {
        sampleTime = TIME_BETWEEN_SAMPLES;
        grabbing = false;
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //While hand is grabbing create samples to show the motion
        if(grabbing)
        {
            //Subtract the time until next sample and make one if time is up
            sampleTime -= Time.deltaTime;

            if (sampleTime <= 0)
            {
                CreateSample();

                sampleTime = TIME_BETWEEN_SAMPLES;
            }
        }
    }

    public bool GetGrabbing()
    {
        return grabbing;
    }

    public void ToggleGrabbing()
    {
        if(grabbing)
        {
            animator.SetBool("IsGrabbing", false);

            sampleTime = TIME_BETWEEN_SAMPLES;
        }
        else
        {
            animator.SetBool("IsGrabbing", true);

            CreateSample();
        }

        grabbing = !grabbing;
    }

    void CreateSample()
    {
        if (!showSamples)
            return;

        GameObject instNode = Instantiate(sample);
        instNode.transform.position = transform.position;
    }
}
