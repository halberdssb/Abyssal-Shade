using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCollectionHandler : MonoBehaviour
{
    public int numBoids;
    [SerializeField]
    private List<BoidObject> collectedBoids = new List<BoidObject>();

    [Space]
    [SerializeField]
    private AudioSource[] pickupSounds;
    [SerializeField]
    private AudioSource deathSound;

    public void AddCollectedBoid(BoidObject boid)
    {
        collectedBoids.Add(boid);
        numBoids++;
    }

    public void RemoveBoid(BoidObject boid)
    {
        if (collectedBoids.Contains(boid))
        {
            collectedBoids.Remove(boid);
            numBoids--;
        }
    }

    public int GetNumberOfBoids()
    {
        return collectedBoids.Count;
    }

    public BoidObject[] CallBoids()
    {
        return CallBoids(GetNumberOfBoids());
    }

    public BoidObject[] CallBoids(int numToPull)
    {
        BoidObject[] boidsToPull = new BoidObject[numToPull];

        for (int i = 0; i < numToPull; i++)
        {
            boidsToPull[i] = collectedBoids[i];
            numBoids--;
        }
        for (int i = 0; i < numToPull; i++)
        {
            collectedBoids.RemoveAt(0);
        }

        return boidsToPull;
    }

    public void PlayPickupSoundRandom()
    {
        int randomInt = Random.Range(0, pickupSounds.Length);
        pickupSounds[randomInt].PlayOneShot(pickupSounds[randomInt].clip);
    }

    public void PlayDeathSound()
    {
        deathSound.PlayOneShot(deathSound.clip);
    }
}
