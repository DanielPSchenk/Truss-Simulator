using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixOps 
{
    public static float[] add(float[] a, float[] b) {
        float[] ret = new float[a.Length];
        for (int i = 0; i < a.Length; i++) {
            ret[i] = a[i] + b[i];
        }
        return ret;
    }

    public static float[] sub(float[] a, float[] b)
    {
        float[] ret = new float[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            ret[i] = a[i] - b[i];
        }
        return ret;
    }

    public static float dot(float[] a, float[] b) {
        float ret = 0;
        for (int i = 0; i < a.Length; i++) {
            ret += a[i] * b[i];
        }
        return ret;
    }

    public static float[] matrixVector(float[,] M, float[] v) {
        float[] ret = new float[v.Length];
        for (int i = 0; i < v.Length; i++) {
            for (int j = 0; j < v.Length; j++) {
                ret[i] += M[i, j] * v[j];
            }
        }
        return ret;
    }

    public static float[] randomVector(int l) {
        float[] ret = new float[l];
        for (int i = 0; i < l; i++) {
            ret[i] = Random.value;
        }
        return ret;
    }

    public static float[] vectorScalar(float[] v, float s) {
        float[] ret = new float[v.Length];
        for (int i = 0; i < v.Length; i++) {
            ret[i] = v[i] * s;
        }
        return ret;
    }

    public static void printVector<T>(T[] v)
    {
        string s = "";
        for (int i = 0; i < v.Length; i++)
        {
            s = s + v[i] + " ";
        }
        Debug.Log(s);
    }

    public static float[,] transposed(float[,] A) {
        float[,] ret = new float[A.GetLength(1), A.GetLength(0)];
        for (int i = 0; i < A.GetLength(0); i++) {
            for (int j = 0; j < A.GetLength(1); j++) {
                ret[j, i] = A[i, j];
            }
        }
        return ret;
    }

    public static float[,] matrixMatrix(float[,] A, float[,] B) {
        float[,] ret = new float[A.GetLength(0), A.GetLength(0)];
        for (int i = 0; i < A.GetLength(0); i++) {
            for (int j = 0; j < A.GetLength(0); j++) {
                float acc = 0;
                for (int k = 0; k < A.GetLength(0); k++) {
                    acc += A[i, k] * B[k, j];
                }
                ret[i, j] = acc;
            }
        }
        return ret;
    }

}
