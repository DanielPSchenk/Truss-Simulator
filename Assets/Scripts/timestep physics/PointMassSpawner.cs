using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMassSpawner : MonoBehaviour
{
    public int gridSize = 10;
    public Mass pointMass;
    public float distance = 10f;
    float sr = 200f;

    public bool solarSystem;
    public int numberPlanets = 9;

    public bool constraintContraption;
    public int contraptionSize = 100;
    public int constraintCount = 100;

    PhysicsManager manager;
    // Start is called before the first frame update

    Mass spawnObject(float mass) {
        Mass newMass = Instantiate(pointMass);
        newMass.mass = mass;
        float scale = Mathf.Pow(newMass.mass + 1, .3f);
        newMass.transform.localScale = new Vector3(scale, scale, scale);
        return newMass;
    }
    void spawnSatellite(Mass parent, Mass grandParent, float massMaximum, int children) {
        float mass = Random.Range(1, parent.mass * massMaximum);
        
        Mass newMass = spawnObject(mass);
        float distance = 0;
        if (grandParent != parent) distance = Random.Range((newMass.transform.localScale.x + parent.transform.localScale.x) * 5, .7f * (grandParent.transform.position - parent.transform.position).magnitude * (float)Mathf.Pow(parent.mass / grandParent.mass, (float).4));
        else distance = Random.Range(5 * (newMass.transform.localScale.x + parent.transform.localScale.x), 200);
        float orbitalSpeed = Mathf.Sqrt(parent.mass * PhysicsManager.gravitationalConstant / distance);
        float speed = Random.Range(orbitalSpeed * .8f, orbitalSpeed * 1.3f);

        float argument = Random.Range(0, 2 * Mathf.PI);

        Vector3 offset = new Vector3(Mathf.Cos(argument), Mathf.Sin(argument));
        newMass.velocity = Vector3.Cross(Vector3.forward, offset).normalized * speed;
        newMass.transform.position = parent.transform.position + offset * distance;

        int numberSatellites = children + Random.Range(-2, 1);

        for (int i = 0; i < numberSatellites; i++) {
            spawnSatellite(newMass, parent, .05f, children / 10);
        }
    }

    void Start()
    {
        manager = GameObject.FindObjectOfType<PhysicsManager>();
        if (solarSystem)
        {
            Mass sun = spawnObject(20000);

            for (int i = 0; i < numberPlanets; i++)
            {
                spawnSatellite(sun, sun, .0001f, 6);
            }

            
        }

        if (constraintContraption) {
            Mass[] masses = new Mass[contraptionSize];
            for (int i = 0; i < contraptionSize; i++) {
                Mass nMass = spawnObject(Random.Range(1, 50));
                nMass.transform.Translate(new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), 0));
                masses[i] = nMass;
            }
            for (int i = 0; i < constraintCount; i++) {
                int firstIndex = (int)Random.Range(0, contraptionSize - 1);
                int secondIndex = (int)Random.Range(0, contraptionSize - 1);
                while(firstIndex == secondIndex) secondIndex = (int)Random.Range(0, contraptionSize - 1);
                Constraint c = new Constraint(masses[firstIndex], masses[secondIndex], Random.Range(0.6f, .9f), Random.Range(.5f, 2), Random.Range(5, 30));
                manager.constraints.Add(c);
            }
        }
    }


}
