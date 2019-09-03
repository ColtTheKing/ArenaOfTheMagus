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

    public Vector3 palmOffset;

    public GameObject fireEffect, waterEffect, earthEffect, airEffect;

    public GameObject gem1, gem2, gem3, gem4, gem5;

    public Material fullMaterial, emptyMaterial;

    private Animator animator;
    private float sampleTime, healthThreshold;
    private bool grabbing, useElement;
    private GameObject currentEffect;
    private SpellHandler.SpellElement currentElement;

    private void Awake()
    {
        sampleTime = TIME_BETWEEN_SAMPLES;
        currentEffect = null;
        useElement = false;
        healthThreshold = 0.8f;
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //While hand is grabbing create samples to show the motion
        if (grabbing)
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
        if (grabbing)
        {
            animator.SetBool("IsGrabbing", false);

            sampleTime = TIME_BETWEEN_SAMPLES;

            if (useElement)
            {
                Destroy(currentEffect);
                currentEffect = null;
            }
        }
        else
        {
            animator.SetBool("IsGrabbing", true);

            CreateSample();

            if (useElement)
            {
                CreateEffect();
            }
        }

        grabbing = !grabbing;
    }

    public void CreateSample()
    {
        if (!showSamples)
            return;

        GameObject instNode = Instantiate(sample);
        instNode.transform.position = transform.position;
    }

    public Vector3 CenterPos()
    {
        Vector3 rotatedOffset = transform.rotation * palmOffset;

        return transform.position + rotatedOffset;
    }

    public void SetElement(SpellHandler.SpellElement element)
    {
        currentElement = element;
        useElement = true;
    }

    public void RemoveElement()
    {
        useElement = false;
    }

    private void CreateEffect()
    {
        switch (currentElement)
        {
            case SpellHandler.SpellElement.FIRE:
                currentEffect = Instantiate(fireEffect);
                break;
            case SpellHandler.SpellElement.WATER:
                currentEffect = Instantiate(waterEffect);
                break;
            case SpellHandler.SpellElement.EARTH:
                currentEffect = Instantiate(earthEffect);
                break;
            case SpellHandler.SpellElement.AIR:
                currentEffect = Instantiate(airEffect);
                break;
        }

        currentEffect.transform.parent = this.transform;
        currentEffect.transform.localPosition = palmOffset;

        ParticleSystem p = currentEffect.GetComponentInChildren<ParticleSystem>();
        p.Simulate(10);
        p.Play();
    }

    public void SetGems(float healthPercent)
    {
        if(healthPercent <= healthThreshold)
        {
            switch(healthThreshold)
            {
                case 0.8f:
                    healthThreshold = 0.6f;
                    gem1.GetComponent<MeshRenderer>().material = emptyMaterial;
                    break;
                case 0.6f:
                    healthThreshold = 0.4f;
                    gem2.GetComponent<MeshRenderer>().material = emptyMaterial;
                    break;
                case 0.4f:
                    healthThreshold = 0.2f;
                    gem3.GetComponent<MeshRenderer>().material = emptyMaterial;
                    break;
                case 0.2f:
                    healthThreshold = 0f;
                    gem4.GetComponent<MeshRenderer>().material = emptyMaterial;
                    break;
            }
        }
    }

    public void ResetGems()
    {
        healthThreshold = 0.8f;

        gem1.GetComponent<MeshRenderer>().material = fullMaterial;
        gem2.GetComponent<MeshRenderer>().material = fullMaterial;
        gem3.GetComponent<MeshRenderer>().material = fullMaterial;
        gem4.GetComponent<MeshRenderer>().material = fullMaterial;
        gem5.GetComponent<MeshRenderer>().material = fullMaterial;
    }
}