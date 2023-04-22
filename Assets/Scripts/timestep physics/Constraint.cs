using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constraint
{
    public float damping = 1;
    public float spring = 1;
    public float relaxed = 1;

    public Mass m1;
    public Mass m2;

    public Constraint(Mass m1, Mass m2, float damping, float spring, float relaxed) {
        this.m1 = m1;
        this.m2 = m2;
        this.damping = damping;
        this.spring = spring;
        this.relaxed = relaxed;
    }

}
