using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianElimination : LSESolver
{
    public override float[] solve(float[,] A, float[] b)
    {
        A = A.Clone() as float[,];
        b = b.Clone() as float[];

        int[] reorder = new int[b.Length];
        for (int i = 0; i < b.Length; i++)
        {
            reorder[i] = i;
        }

        void findPivot(int progress)
        {
            int largest = progress;
            float largestValue = A[reorder[progress], progress];
            for (int i = progress + 1; i < b.Length; i++)
            {
                if (Mathf.Abs(A[reorder[i], progress]) > largestValue)
                {
                    largestValue = Mathf.Abs(A[reorder[i], progress]);
                    largest = i;
                }
            }
            int temp = reorder[progress];
            reorder[progress] = reorder[largest];
            reorder[largest] = temp;
        }


        //forward processing
        for (int i = 0; i < b.Length - 1; i++)
        {
            findPivot(i);
            //printMatrixReordered(reorder);
            if (A[reorder[i], i] != 0)
            {
                for (int k = i + 1; k < b.Length; k++)
                {
                    float factor = A[reorder[k], i] / A[reorder[i], i];
                    A[reorder[k], i] = 0;
                    for (int j = i + 1; j < b.Length; j++)
                    {
                        A[reorder[k], j] = A[reorder[k], j] - factor * A[reorder[i], j];
                    }
                    b[reorder[k]] = b[reorder[k]] - b[reorder[i]] * factor;
                }
                //printMatrixReordered(reorder);
            }
            else
            {
                Debug.Log("skipped line");
            }
        }

        //testForwardProcessing();

        //backward processing
        for (int j = b.Length - 1; j > 0; j--)
        {
            for (int i = 0; i < j; i++)
            {
                float factor = A[reorder[i], j] / A[reorder[j], j];
                A[reorder[i], j] = 0;
                b[reorder[i]] = b[reorder[i]] - factor * b[reorder[j]];
            }
            
        }
        //printMatrixReordered(reorder);
        
        float[] result = new float[b.Length];
        for (int i = 0; i < b.Length; i++)
        {
            if (A[reorder[i], i] != 0) result[i] = b[reorder[i]] / A[reorder[i], i];
        }
        

        return result;
    }
}
