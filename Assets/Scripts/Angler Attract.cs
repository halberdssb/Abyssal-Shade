using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerAttract : MonoBehaviour
{
    //The color of the angler fish's light when attracting soul fish
    public Color attractColor;
    //Color of the angler fish's light when not attracting soul fish
    public Color normalColor;

    //Radius within which the angler fish attracts soul fish
    public float attractRadius = 5f;
    //Strength of the attraction force applied to soul fish
    public float attractionForce = 10f;
    //Minimum distance at which the attraction force is applied to soul fish (optional)
    public float minimumAttractionDistance = 1f;
    //List to keep track of soul fish currently within the attraction radius
    private List<BoidObject> attractedBoids = new List<BoidObject>();

    //Reference to the BoidCollectionHandler to manage the collection of boids in the scene
    private BoidCollectionHandler boidCollectionHandler;

    // Start is called before the first frame update
    void Start()
    {
        //Get the Renderer component to change the color of the angler fish's light
        Renderer renderer = GetComponent<Renderer>();

        //Finds the BoidCollectionHandler in the scene to manage the collection of boids
        boidCollectionHandler = FindObjectOfType<BoidCollectionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var boid in attractedBoids)
        {
            Vector3 directionToAttract = transform.position - boid.transform.position;
            boid.ApplyForce(directionToAttract.normalized * attractionForce);
        }
    }

    //Check for fish entering radius
    private void OnTriggerEnter(Collider other)
    {
        //Only check if player or boid b/c these are the only layers the angler collides with
        if (!other.CompareTag("Player"))
        {
            BoidObject boid = other.GetComponent<BoidObject>();
            attractedBoids.Add(boid);
        }
    }

    //Check for fish leaving radius
    private void OnTriggerExit(Collider other)
    {
        //Only check if player or boid b/c these are the only layers the angler collides with
        if (!other.CompareTag("Player"))
        {
            BoidObject boid = other.GetComponent<BoidObject>();
            attractedBoids.Remove(boid);
        }
    }

    //"Kills" all soulfish that collide with the angler fish, removing them from the scene and the boid collection
    void OnCollisionEnter(Collision collider)
    {
        if (!collider.gameObject.CompareTag("Player"))
        {
            BoidObject boid = collider.gameObject.GetComponent<BoidObject>();
            BoidManager.DespawnBoids(boid);
        }
    }

    //Coroutine to destroy the object after one frame to prevent immediate access to the object
    private IEnumerator DestroyObjectWithDelay(GameObject objectToDestroy)
    {
        yield return null; 
        Destroy(objectToDestroy);
    }
}