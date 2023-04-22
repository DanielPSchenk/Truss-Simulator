using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour
{
    public float mass = 1;
    public Vector3 velocity;
    public Vector3 forces;

    private void Start()
    {
        velocity = new Vector3(0, 0, 0);
    }





}
