using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public bool applyGravity = false;
    public static float gravitationalConstant = (float).3;

    public bool applyConstraints = true;
    public List<Constraint> constraints;

    private float lastEnergy = float.PositiveInfinity;

    // Start is called before the first frame update
    void Awake()
    {
        constraints = new List<Constraint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Mass[] masses = GameObject.FindObjectsOfType<Mass>();
        //Vector3[,] forces = new Vector3[masses.Length, masses.Length]; 

        foreach (Mass m in masses) {
            m.forces = new Vector3(0, 0, 0);
        }
        
        if (applyGravity)
        {
            for (int i = 0; i < masses.Length; i++)
            {
                for (int j = i + 1; j < masses.Length; j++)
                {
                    Vector3 direction = (masses[j].transform.position - masses[i].transform.position);
                    Vector3 force = direction.normalized * (gravitationalConstant * masses[i].mass * masses[i].mass / (direction.magnitude * direction.magnitude));
                    //forces[i] += force;
                    //forces[j] -= force;
                }
            }
        }

        if (applyConstraints) {
            foreach (Constraint c in constraints) {
                Vector3 m1ToM2 = (c.m2.transform.position - c.m1.transform.position);
                float distance = m1ToM2.magnitude;
                Vector3 direction = m1ToM2.normalized;

                c.m1.forces -= (c.relaxed - distance) * direction * c.spring;
                c.m2.forces += (c.relaxed - distance) * direction * c.spring;
                
                float relativeSpeed = Vector3.Dot(c.m1.velocity, direction) - Vector3.Dot(c.m2.velocity, direction);

                c.m1.forces -= relativeSpeed * direction * c.damping;
                c.m2.forces += relativeSpeed * direction * c.damping;
                
            }
        }
        float highestForce = 0;
        for (int i = 0; i < masses.Length; i++) {
            masses[i].velocity += masses[i].forces / masses[i].mass;
            masses[i].transform.Translate(masses[i].velocity * Time.deltaTime);
            if (masses[i].forces.magnitude > highestForce) highestForce = masses[i].forces.magnitude;
        }
        //Debug.Log(highestForce);

        float energy = 0;
        foreach (Mass m in masses) {
            energy += m.velocity.magnitude * m.velocity.magnitude * .5f;
        }

        foreach (Constraint c in constraints) {
            float x = Mathf.Abs((c.m1.transform.position - c.m2.transform.position).magnitude - c.relaxed);
            energy += x * x * c.spring * .5f;
        }

        //if(energy > lastEnergy)Debug.Log(energy);

        lastEnergy = energy;

        
    }

    private void Update()
    {
        foreach (Constraint c in constraints)
        {

            Debug.DrawLine(c.m1.transform.position, c.m2.transform.position, Color.red, Time.deltaTime);
        }
    }
}
