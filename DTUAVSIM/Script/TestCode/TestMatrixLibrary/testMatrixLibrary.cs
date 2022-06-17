using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
public class testMatrixLibrary : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        //define all kinds of matrix
        var matrix33 = new DenseMatrix(3);
        for(int i=0;i<matrix33.RowCount;i++)
        {
            for (int j = 0; j < matrix33.ColumnCount; j++)
            {
                matrix33.At(i,j,i);
                Debug.Log("matrix33----: " + matrix33.At(i, j));
            }           
        }
        var submatrix = matrix33.SubMatrix(0, 1, 0, 1);
        for (int i = 0; i < submatrix.RowCount; i++)
        {
            for (int j = 0; j < submatrix.ColumnCount; j++)
            {
                Debug.Log("submatrix----: " + submatrix.At(i, j));
            }
        }
        var matrix22 = new DenseMatrix(2, 2);
        var matrix23 = new DenseMatrix(2, 3);
        var matrix331 = new DenseMatrix(3, 3, 1.0);
        var matrix55 = DenseMatrix.Identity(5);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
