using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LSESolver 
{
    public abstract float[] solve(float[,] A, float[] b);
}

public abstract class GradientSolver : LSESolver {
    float[,] AT = null;

    public float[] ATAVmul(float[,] A, float[] v) {
        if (AT == null) AT = MatrixOps.transposed(A);
        return MatrixOps.matrixVector(AT, MatrixOps.matrixVector(A, v));
    }
}
