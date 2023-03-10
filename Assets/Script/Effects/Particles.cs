using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    private ParticleSystem[] particles;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void PlayParticles()
    {
        foreach(ParticleSystem par in particles)
        {
            par.Play();
        }
    }
}
