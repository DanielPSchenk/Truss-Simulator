using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSE
{
    //A*x=b
    float[] b;
    public float[,] A;

    





    public LSE(List<Node> nodes, List<Beam> beams, List<DirectionalConstraint> constraints) {
        Node[] n = indexObjects(nodes);
        indexObjects(beams);
        indexObjects(constraints);

        int equationSize = 0;
        equationSize = nodes.Count * 2 >= beams.Count + constraints.Count ? nodes.Count * 2 : beams.Count + constraints.Count; 

        A = new float[equationSize, equationSize];
        b = new float[equationSize];
        
        for (int i = 0; i < nodes.Count; i++) {
            //x direction
            foreach (Beam beam in n[i].beams) {
                A[i * 2, beam.index] = Vector2.Dot(Vector2.right, (beam.otherNode(n[i]).transform.position - n[i].transform.position).normalized);
            }
            foreach (DirectionalConstraint c in n[i].constraints) {
                A[i * 2, beams.Count + c.index] = Vector2.Dot(c.normalDirection, Vector2.right);
            }

            //y direction
            foreach (Beam beam in n[i].beams)
            {
                A[i * 2 + 1, beam.index] = Vector2.Dot(Vector2.up, (beam.otherNode(n[i]).transform.position - n[i].transform.position).normalized);
            }
            foreach (DirectionalConstraint c in n[i].constraints)
            {
                A[i * 2 + 1, beams.Count + c.index] = Vector2.Dot(c.normalDirection, Vector2.up);
            }
        }

        solveGravity(new ConjugateGradient());
        
        //test gaussian solver
        //testSolver(new GaussianElimination());
    }

    void solveGravity(LSESolver solver) {
        for (int i = 1; i < b.Length; i += 2) {
            b[i] = 1;
        }

        DisplayResults(solver.solve(A, b));

    }

    public void solveForce(LSESolver solver, int index, Vector2 force) {
        b[index * 2] = -force.x;
        b[index * 2 + 1] = -force.y;

        DisplayResults(solver.solve(A, b));
    }

    void DisplayResults(float[] f) {
        Beam[] beams = GameObject.FindObjectsOfType<Beam>();

        float max = float.NegativeInfinity;
        float min = float.PositiveInfinity;

        for (int i = 0; i < f.Length; i++) {
            if (f[i] < min) min = f[i];
            if (f[i] > max) max = f[i];
        }

        Gradient g = GameObject.FindObjectOfType<NodePlacer>().vis;

        float range = max - min;

        foreach (Beam b in beams) {
            SpriteRenderer sp = b.GetComponent<SpriteRenderer>();
            sp.color = g.Evaluate((f[b.index] - min) / range);
        }
    }

    void testSolver(LSESolver s) {
        b = MatrixOps.randomVector(b.Length);
        //A = new float[,] { { 2, -2 }, { -2, 6 } };
        //b = new float[] { 0, -4 };
        //A = new float[,] { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
        //b = new float[] { 8, -11, -3 };
        float[,] aCopy = A.Clone() as float[,];
        float[] bCopy = b.Clone() as float[];
        float[] x = s.solve(A, b);
        
        float[] Ax = MatrixOps.matrixVector(aCopy, x);
        
        float[] diff = MatrixOps.add(bCopy, MatrixOps.vectorScalar(Ax, -1f));
        
        float diffLength = Mathf.Sqrt(MatrixOps.dot(diff, diff));
        Debug.Log(diffLength);
    }

    public void printMatrix() {
        string s = "";
        for (int i = 0; i < b.Length; i++) {
            
            for (int j = 0; j < b.Length; j++) {
                s = s + A[i, j].ToString("f2") + " ";
            }
            s = s + "\n";
            
        }
        Debug.Log(s);
    }

    public void printMatrixReordered(int[] reorder)
    {
        string s = "";
        for (int i = 0; i < b.Length; i++)
        {

            for (int j = 0; j < b.Length; j++)
            {
                s = s + A[reorder[i], j].ToString("f2") + " ";
            }
            s = s + "   " + b[reorder[i]].ToString("f2");
            s = s + "\n";

        }
        Debug.Log(s);
    }

    

    

    


    
    T[] indexObjects<T>(List<T> items) where T : Indexable {
        for (int i = 0; i < items.Count; i++) {
            items[i].index = i;
        }
        items.Sort((i1, i2) => i1.index.CompareTo(i2.index));
        return items.ToArray();
    }

    
}
