using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace DTUAVCARS.Algorithm.Predictor
{
    public class LeastSquare
    {
        private List<float> _t;
        private List<float> _x;
        private int _order;
        private DenseMatrix _A;
        private DenseMatrix _B;
        private List<float> _ret;
        public List<float> GetT()
        {
            return _t;
        }

        public List<float> GetX()
        {
            return _x;
        }

        public int GetOrder()
        {
            return _order;
        }

        public List<float> GetResult()
        {
            _A = new DenseMatrix(_t.Count, _order + 1);
            _B = new DenseMatrix(_x.Count, 1);
            for (int i = 0;i<_t.Count;++i)
            {
                for(int n= _order, dex=0;n>=1;--n,++dex)
                {
                    _A.At(i, dex, Mathf.Pow(_t[i], n));
                }
                _A.At(i, _order, 1);
            } 
            for(int i=0;i<_x.Count;++i)
            {
                _B.At(i, 0, _x[i]);
            }
            var W = (_A.Transpose() * _A).Inverse() * _A.Transpose() * _B;
            for(int i=0;i< _order + 1;i++)
            {
                _ret[i] = W.At(i, 0);
            }
            return _ret;///wn--wn-1---wn-2--->w0
        }

        public bool SetT(List<float> t)
        {
            _t = t;
            return true;
        }

        public bool SetX(List<float> x)
        {
            _x = x;
            return true;
        }
        public bool SetOrder(int order)
        {
            _order = order;
            return true;
        }

        public LeastSquare()
        {

        }
        public LeastSquare(List<float> t, List<float> x, int order)
        {
            _order = order;
            _t = t;
            _x = x;
            _A = new DenseMatrix(_t.Count, _order + 1);
            _B = new DenseMatrix(_x.Count, 1);
            _ret = new List<float>();
            for (int i = 0; i < _order + 1; i++)
            {
                _ret.Add(0.0f);
            }
        }
    }
}
