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


    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attractingFish)
        {
            GetComponent<Renderer>().material.color = attractColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = normalColor;
        }

        // Attract nearby objects if they are within the specified radius
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
                    attractingFish = true;  // Change color to attract color when attracting at least one fish
                }

                // Calculate the direction towards the current object
                Vector3 directionToAttract = transform.position - nearbyObject.transform.position;

                // Apply attraction force to the object
                rb.AddForce(directionToAttract.normalized * attractionForce);
            }
        }

        // Remove objects that have moved out of the radius
        for (int i = attractedObjects.Count - 1; i >= 0; i--)
        {
            Rigidbody attractedObject = attractedObjects[i];
            float distanceToObject = Vector3.Distance(transform.position, attractedObject.position);
            if (distanceToObject > attractRadius)
            {
                attractedObjects.Remove(attractedObject); // Stop attracting the object
            }
        }

        // If no objects are being attracted, change the color back to normal
        if (attractedObjects.Count == 0)
        {
            attractingFish = false;
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Soulfish"))
        {
            Debug.Log("Hit");
            Destroy(collider.gameObject);
        }
    }
}
