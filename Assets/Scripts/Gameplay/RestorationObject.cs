using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

/*
 * An object that can be restored by a certain number of soulfish
 * Handles pulling soulfish towards and swapping models
 * 
 * Jeff Stevenson
 * 4.21.25 
 */

public class RestorationObject : MonoBehaviour
{
    public delegate void OnRestoredDel();
    public OnRestoredDel RestoredDelegate;

    [SerializeField]
    private RestorationObjectUI ui;
    [SerializeField]
    private MeshRenderer mesh;
    [SerializeField]
    private Color deadColor;
    [SerializeField]
    private float restoredEmissiveIntensity;
    [SerializeField]
    private AudioSource restoredSound;

    [Space]
    [SerializeField]
    public int soulfishNeededToRestore;
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
    private Color restoredColor;
    private Vector3[] fishSurroundPoints;

    private bool isRestored;

    void Start()
    {
        fishSurroundPoints = NavigationSphereCaster.GetNavigationSphereVectors(soulfishNeededToRestore);
        player = FindObjectOfType<PlayerStateController>();
        vortexSphere = CreateVortexSphereMesh();

        isRestored = false;

        // visuals
        restoredColor = mesh.material.color;
        SwapToUnrestoredVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isRestored)
            {
                BoidCollectionHandler playerBoids = player.boidCollectionHandler;
                if (playerBoids.GetNumberOfBoids() >= soulfishNeededToRestore)
                {
                    Debug.Log("restoring object! enough boids present: " + playerBoids.GetNumberOfBoids());
                    StartCoroutine(RestoreObject(playerBoids.CallBoids(soulfishNeededToRestore)));
                    isRestored = true;
                }
                else Debug.Log("not enough fish in player! num fish player has: " + playerBoids.GetNumberOfBoids());
            }
        }
    }

    // handles putting soulfish into position around object and playing restore anim
    private IEnumerator RestoreObject(BoidObject[] boids)
    {
        // create boid sphere
        SurroundObjectWithBoids(boids, fishMoveToPositionTime);
        ui.FadeUI(false);

        // wait until boids are in position
        yield return new WaitForSeconds(fishMoveToPositionTime);

        // this should tween up an alpha value on the vortex shader, but right now just turn sphere on
        vortexSphere.SetActive(true);

        // rotate object and fish around it at increasing speed while fading in vortex sphere
        float spinSpeed = 10f;
        float spinTime = 1.8f;
        float spinTimer = 0f;

        restoredSound.Play();

        // spin up to speed
        while (spinTimer < spinTime)
        {
            float rotationAmt = sphereSpinUpCurve.Evaluate(spinTimer / spinTime);
            transform.Rotate(0, rotationAmt * spinSpeed, 0);
            spinTimer += Time.deltaTime;
            yield return null;
        }

        // swap to restored visuals
        SwapToRestoredVisuals();

        // spin at top speed
        float maxSpeedSpinTime = 0.8f;
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

        // invoke check delegate after delay to check if area restored
        yield return new WaitForSeconds(1f);
        RestoredDelegate?.Invoke();
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


            // used for rotating fish around sphere by rotating object
            int fishID = i;
            moveToSphereTween.onComplete += () =>
            {
                boid.transform.parent = transform;

                // rotate to proper position
                Vector3 oldRotation = boid.transform.eulerAngles;
                boid.transform.LookAt(transform.position + (fishSurroundPoints[fishID] * fishSphereRadius * 2));
                boid.transform.Rotate(new Vector3(0, 90, 0));
                Vector3 correctRotation = boid.transform.eulerAngles;
                boid.transform.eulerAngles = oldRotation;
                boid.transform.DORotate(correctRotation, 1f);
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
            boid.ToggleBoidBehavior(true);
            player.boidCollectionHandler.AddCollectedBoid(boid);
        }
    }

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

    private void SwapToUnrestoredVisuals()
    {
        mesh.material.color = deadColor;
        HDMaterial.SetEmissiveIntensity(mesh.material, 0, UnityEditor.Rendering.HighDefinition.EmissiveIntensityUnit.Nits);
    }
    private void SwapToRestoredVisuals()
    {
        mesh.material.color = restoredColor;
        HDMaterial.SetEmissiveIntensity(mesh.material, restoredEmissiveIntensity, UnityEditor.Rendering.HighDefinition.EmissiveIntensityUnit.Nits);
    }

    // draw representation of fish sphere around object for testing in editor - should cover mesh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position + fishSphereOffset, fishSphereRadius * 0.9f);
    }
}
