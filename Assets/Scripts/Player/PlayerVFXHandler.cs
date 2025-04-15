using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles triggering/adjusting all VFX for the player
 * 
 * Jeff Stevenson
 * 4.14.25
 */

public class PlayerVFXHandler : MonoBehaviour
{
    [Header("Dash VFX")]
    [SerializeField]
    private TrailRenderer[] _dashTrails;
    [SerializeField]
    private ParticleSystem[] _dashSpinParticles;

    void Start()
    {
        // make sure all vfx are turned off
        SetDashTrailActiveState(false);
    }


    // activates dash vfx
    public void TriggerDashVFX(float dashTime)
    {
        StartCoroutine(DashTrailRoutine(dashTime));

        foreach (var particle in _dashSpinParticles)
        {
            particle.Play();
        }
    }

    private IEnumerator DashTrailRoutine(float dashTime)
    {
        SetDashTrailActiveState(true);

        yield return new WaitForSeconds(dashTime);

        SetDashTrailActiveState(false);
    }
    
    private void SetDashTrailActiveState(bool active)
    {
        foreach (var trail in _dashTrails)
        {
            trail.emitting = active;
        }
    }

}
