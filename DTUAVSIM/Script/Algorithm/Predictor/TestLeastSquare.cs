using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.Algorithm.Predictor;
public class TestLeastSquare : MonoBehaviour
{

    // Start is called before the first frame update
    public List<float> t;
    public List<float> x;
    public int order;
    private LeastSquare _leastSquare;
    private List<float> ret;
    void Start()
    {
        _leastSquare = new LeastSquare(t, x, order);
        ret = new List<float>();
        
    }

    // Update is called once per frame
    void Update()
    {
         ret = _leastSquare.GetResult();
        for(int i =0; i<ret.Count;i++)
        {
            Debug.Log("w" + i + ":" + ret[i]);
        }
    }
}
