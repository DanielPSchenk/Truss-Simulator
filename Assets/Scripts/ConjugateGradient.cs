using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConjugateGradient : GradientSolver
{
    static int iterations = 100;

    public override float[] solve(float[,] A, float[] b)
    {
        float[] x = new float[b.Length];
        
        
        b = MatrixOps.matrixVector(MatrixOps.transposed(A), b);
        float[] r = MatrixOps.sub(b, x);
        float[] d = r.Clone() as float[];

        float alpha = 0;
        float beta = 0;

        for (int i = 0; i < iterations; i++) {
            float riri = MatrixOps.dot(r, r);
            if (riri < .001) return x;
            float[] Adi = ATAVmul(A, d);
            float diAdi = MatrixOps.dot(d, Adi);
            alpha = riri / diAdi;

            float[] xi1 = MatrixOps.add(x, MatrixOps.vectorScalar(d, alpha));

            float[] ri1 = MatrixOps.sub(r, MatrixOps.vectorScalar(Adi, alpha));

            beta = MatrixOps.dot(ri1, ri1) / riri;

            d = MatrixOps.add(ri1, MatrixOps.vectorScalar(d, beta));
            x = xi1;
            r = ri1;
        }

        return x;
    }
}
