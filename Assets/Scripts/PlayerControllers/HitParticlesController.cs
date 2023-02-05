using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticlesController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] bloodHitParticles;

    public void PlayBloodHit(int particleIndex)
    {
        bloodHitParticles[particleIndex].Play();
    }
}
