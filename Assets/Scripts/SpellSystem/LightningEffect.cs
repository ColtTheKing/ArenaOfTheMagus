using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    public float particleStart;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particles)
        {
            p.Simulate(particleStart);
            p.Play();
        }
    }
}
