using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerAttract : MonoBehaviour
{
    public Color attractColor;
    public Color normalColor;
    private bool attractingFish = false;
    public float attractRadius = 5f;   // The radius within which objects will be attracted
    public float attractionForce = 10f; // How strong the attraction force is
    public float minimumAttractionDistance = 1f; // Optional: Minimum distance for attraction
    private List<Rigidbody> attractedObjects = new List<Rigidbody>(); // List to keep track of attracted objects
    private List<BoidObject> attractedBoids = new List<BoidObject>(); // List of boids attracted

    private BoidCollectionHandler boidCollectionHandler;

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        boidCollectionHandler = FindObjectOfType<BoidCollectionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        bool anyAttracted = false; // Flag to check if any object is still within the radius

/*        // Attract nearby objects if they are within the specified radius
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, attractRadius);

        // Loop through all nearby objects and apply the attraction force
        foreach (Collider nearbyObject in nearbyObjects)
        {
            if (nearbyObject.gameObject != gameObject && nearbyObject.attachedRigidbody != null)
            {
                Rigidbody rb = nearbyObject.attachedRigidbody;

                // If the object is not already being attracted, add it to the list
                if (!attractedObjects.Contains(rb))
                {
                    attractedObjects.Add(rb);
                }

                // Apply attraction force only if the object is within the radius
                if (Vector3.Distance(transform.position, nearbyObject.transform.position) <= attractRadius)
                {
                    // Calculate the direction towards the current object
                    Vector3 directionToAttract = transform.position - nearbyObject.transform.position;
                    rb.AddForce(directionToAttract.normalized * attractionForce);
                    anyAttracted = true;
                }
                else
                {
                    // Stop the object from moving towards the main object if it is outside the radius
                    // You can set the velocity to zero to effectively stop its movement.
                    rb.velocity = Vector3.zero;  // Stop the object from moving further
                }
            }
        }

        // Remove objects that have moved out of the radius or are destroyed
        for (int i = attractedObjects.Count - 1; i >= 0; i--)
        {
            Rigidbody attractedObject = attractedObjects[i];

            // Check if the Rigidbody is destroyed or the object has moved out of range
            if (attractedObject == null || Vector3.Distance(transform.position, attractedObject.position) > attractRadius)
            {
                attractedObjects.RemoveAt(i); // Stop attracting the object
            }
        }

        // Change color based on whether any object is still within the radius
        if (anyAttracted)
        {
            GetComponent<Renderer>().material.color = attractColor;
            attractingFish = true;
        }
        else
        {
            GetComponent<Renderer>().material.color = normalColor;
            attractingFish = false;
        }*/



        foreach (var boid in attractedBoids)
        {
            Vector3 directionToAttract = transform.position - boid.transform.position;
            boid.ApplyForce(directionToAttract.normalized * attractionForce);
        }
    }

    // check for boids entering radius
    private void OnTriggerEnter(Collider other)
    {
        // only check if player or boid b/c these are the only layers the angler collides with
        if (!other.CompareTag("Player"))
        {
            BoidObject boid = other.GetComponent<BoidObject>();
            attractedBoids.Add(boid);
        }
    }

    // check for boids leaving radius
    private void OnTriggerExit(Collider other)
    {
        // only check if player or boid b/c these are the only layers the angler collides with
        if (!other.CompareTag("Player"))
        {
            BoidObject boid = other.GetComponent<BoidObject>();
            attractedBoids.Remove(boid);
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        /*        if (collider.gameObject.CompareTag("Soulfish"))
                {
                    Debug.Log("Hit");
                    // Call a coroutine to delay the destruction and prevent immediate access to the object
                    StartCoroutine(DestroyObjectWithDelay(collider.gameObject));
                }*/
        if (!collider.gameObject.CompareTag("Player"))
        {
            BoidObject boid = collider.gameObject.GetComponent<BoidObject>();
            BoidManager.DespawnBoids(boid);
        }
    }

    // Coroutine to destroy the object after one frame to prevent immediate access to the object
    private IEnumerator DestroyObjectWithDelay(GameObject objectToDestroy)
    {
        yield return null; // Wait until the next frame
        Destroy(objectToDestroy);
    }
}