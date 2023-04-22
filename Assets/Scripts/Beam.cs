using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Indexable
{
    public Node[] nodes;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Node otherNode(Node n) {
        if (nodes[0] == n) return nodes[1];
        if (nodes[1] == n) return nodes[0];
        return null;
    }
}
