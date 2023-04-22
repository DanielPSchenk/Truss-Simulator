using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : Indexable
{
    public List<DirectionalConstraint> constraints;
    public List<Beam> beams;
    // Start is called before the first frame update
    void Awake()
    {
        beams = new List<Beam>();
        constraints = new List<DirectionalConstraint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
