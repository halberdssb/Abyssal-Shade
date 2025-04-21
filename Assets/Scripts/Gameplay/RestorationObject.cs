using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * An object that can be restored by a certain number of soulfish
 * Handles pulling soulfish towards and swapping models
 * 
 * Jeff Stevenson
 * 4.21.25 
 */

public class RestorationObject : MonoBehaviour
{
    [SerializeField]
    private int soulfishNeededToRestore;
    [SerializeField]
    private float fishMoveToPositionTime = 2f;
    [SerializeField]
    private float fishSphereRadius = 2f;
    [SerializeField]
    private Vector3 fishSphereOffset = Vector3.zero;

    [Space]
    [SerializeField]
    private AnimationCurve sphereSpinUpCurve;
    [SerializeField]
    private AnimationCurve sphereSpinDownCurve;

    private PlayerStateController player;
    private GameObject vortexSphere;
    private Vector3[] fishSurroundPoints;

    private bool isRestored;

    void Start()
    {
        fishSurroundPoints = NavigationSphereCaster.GetNavigationSphereVectors(soulfishNeededToRestore);
        player = FindObjectOfType<PlayerStateController>();
        vortexSphere = CreateVortexSphereMesh();

        isRestored = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRestored)
        {
            BoidCollectionHandler playerBoids = player.boidCollectionHandler;
            if (playerBoids.GetNumberOfBoids() >= soulfishNeededToRestore)
            {
                Debug.Log("restoring object! enough boids present");
                StartCoroutine(RestoreObject(playerBoids.CallBoids(soulfishNeededToRestore)));
                isRestored = true;
            }
            else Debug.Log("not enough fish in player! num fish player has: " + playerBoids.GetNumberOfBoids());
        }
    }

    // handles putting soulfish into position around object and playing restore anim
    private IEnumerator RestoreObject(BoidObject[] boids)
    {
        // create boid sphere
        SurroundObjectWithBoids(boids, fishMoveToPositionTime);

        // wait until boids are in position
        yield return new WaitForSeconds(fishMoveToPositionTime);

        // this should tween up an alpha value on the vortex shader, but right now just turn sphere on
        vortexSphere.SetActive(true);

        // rotate object and fish around it at increasing speed while fading in vortex sphere
        float spinSpeed = 10f;
        float spinTime = 3f;
        float spinTimer = 0f;

        // spin up to speed
        while (spinTimer < spinTime)
        {
            float rotationAmt = sphereSpinUpCurve.Evaluate(spinTimer / spinTime);
            transform.Rotate(0, rotationAmt * spinSpeed, 0);
            spinTimer += Time.deltaTime;
            yield return null;
        }

        // spin at top speed
        float maxSpeedSpinTime = 1f;
        spinTimer = 0f;

        while (spinTimer < maxSpeedSpinTime)
        {
            float rotationAmt = 1f;
            transform.Rotate(0, rotationAmt * spinSpeed, 0);
            spinTimer += Time.deltaTime;
            yield return null;
        }

        // spin down to stop
        spinTimer = 0f;

        while (spinTimer < spinTime)
        {
            float rotationAmt = sphereSpinDownCurve.Evaluate(spinTimer / spinTime);
            transform.Rotate(0, rotationAmt * spinSpeed, 0);
            spinTimer += Time.deltaTime;
            yield return null;
        }

        // send boids back to player and fade out sphere
        vortexSphere.SetActive(false);
        ReturnBoidsToPlayer(boids, fishMoveToPositionTime);
    }

    // moves fish in sphere shape around object
    private void SurroundObjectWithBoids(BoidObject[] boids, float tweenTime)
    {
        // move boids to position in sphere around object
        for (int i = 0; i < boids.Length; i++)
        {
            BoidObject boid = boids[i];
            boid.ToggleBoidBehavior(false);

            Vector3 positionAroundObject = transform.position + (fishSurroundPoints[i] * fishSphereRadius) + fishSphereOffset;
            Tween moveToSphereTween = boid.transform.DOMove(positionAroundObject, tweenTime);

            Vector3 fishLookVector = Quaternion.AngleAxis(90, transform.up) * fishSurroundPoints[i];
            Tween rotationTween = boid.transform.DORotate(fishLookVector, tweenTime);

            // used for rotating fish around sphere by rotating object
            moveToSphereTween.onComplete += () =>
            {
                boid.transform.parent = transform;
            };
        }
    }

    // calls boids back to player when visuals are done
    private void ReturnBoidsToPlayer(BoidObject[] boids, float tweenTime)
    {
        for (int i = 0; i < boids.Length; i++)
        {
            BoidObject boid = boids[i];

            boid.transform.parent = null;
            boid.transform.DOMove(player.transform.position, tweenTime).onComplete += () => boid.ToggleBoidBehavior(true);
        }
    }

    // spins the object at a given speed for a certain amount of time

    // creates the vortex sphere visual effect mesh on runtime around the object
    private GameObject CreateVortexSphereMesh()
    {
        GameObject sphereMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphereMesh.GetComponent<Collider>().enabled = false;
        sphereMesh.transform.localScale = Vector3.one * (fishSphereRadius * 0.9f * 2);
        sphereMesh.transform.position = transform.position + fishSphereOffset;
        sphereMesh.transform.parent = transform;

        // set active right now - eventually should also set alpha value on shader to 0
        sphereMesh.SetActive(false);
        return sphereMesh;

    }

    // draw representation of fish sphere around object for testing in editor - should cover mesh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position + fishSphereOffset, fishSphereRadius * 0.9f);
    }
}
