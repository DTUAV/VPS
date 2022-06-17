using System.Collections;
using System.Collections.Generic;
namespace DTUAVCARS.Algorithm.DataStruct
{
    public class CircularBuffer<T>
    {
        private int _bufferSize;
        private List<T> _buffer;

        public T this[int index] 
        { 
            get 
            {
                if (index < _buffer.Count)
                {
                    return _buffer[index];
                }
                else
                {
                    return default(T);
                }
            }
            set
            {
                
                    if (index < _buffer.Count)
                    {
                        _buffer[index] = value;
                    }
                
            }
        }
        public CircularBuffer(int bufferSize)
        {
            _bufferSize = bufferSize;
            _buffer = new List<T>();
        }
        public T Pop()
        {
            T data = _buffer[0];
            _buffer.RemoveAt(0);
            return data;
        }
        public bool Push(T data)
        {
            if(_buffer.Count<_bufferSize)
            {
                _buffer.Add(data);
            }
            else
            {
                
                for(int i=0;i<_bufferSize;i++)
                {
                    if(i<_bufferSize-1)
                    {
                        _buffer[i] = _buffer[i + 1];
                    }
                    else
                    {
                        _buffer[i] = data;
                    }
                }
            }
            return true;
        }
        
    }

}
