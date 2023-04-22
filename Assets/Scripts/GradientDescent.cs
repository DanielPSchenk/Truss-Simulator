using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientDescent : GradientSolver
{
    static int iterations = 100;
    
    public override float[] solve(float[,] A, float[] b)
    {
        
        float[] x = new float[b.Length];
        float[] r = new float[b.Length];
        float alpha = 0;

        b = MatrixOps.matrixVector(MatrixOps.transposed(A), b);

        for (int i = 0; i < iterations; i++) {
            //MatrixOps.printVector(x);
            r = MatrixOps.sub(b, ATAVmul(A, x));

            if (Mathf.Sqrt(MatrixOps.dot(r, r)) < .001) return x;

            alpha = (MatrixOps.dot(r, r)) / MatrixOps.dot(r, ATAVmul(A, r));
            //Debug.Log(alpha);
            x = MatrixOps.add(x, MatrixOps.vectorScalar(r, alpha));
        }

        return x;
    }
}
