using System;
using System.Collections.Generic;
using System.Linq;

namespace SCADA.Common.SyncCollections
{
    public class ThreadSafeList<T> : IEnumerable<T>
    {
        private List<T> _listInternal = new List<T>();
        private object _lockObj = new object();

        public void Add(T newItem)
        {
            lock (_lockObj)
            {
                _listInternal.Add(newItem);
            }
        }

        public void FullUpdate(IEnumerable<T> newData)
        {
            lock (_lockObj)
            {
                _listInternal.Clear();
                _listInternal.AddRange(newData);
            }
        }

        public void Insert(int index, T newItem)
        {
            lock (_lockObj)
            {
                _listInternal.Insert(index, newItem);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_lockObj)
                {
                    return _listInternal[index];
                }
            }
            set
            {
                lock (_lockObj)
                {
                    _listInternal[index] = value;
                }
            }
        }


        public bool Remove(T itemToRemove)
        {
            lock (_lockObj)
            {
                return _listInternal.Remove(itemToRemove);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lockObj)
            {
                if (_listInternal.Count > 0)
                    _listInternal.RemoveAt(index);
            }
        }

        public int Count
        {
            get
            {
                lock (_lockObj)
                {
                    return _listInternal.Count;
                }
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _listInternal.Clear();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return getCopy().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return getCopy().GetEnumerator();
        }

        private List<T> getCopy()
        {
            List<T> copy = new List<T>();
            lock (_lockObj)
            {
                foreach (T item in _listInternal)
                    copy.Add(item);
            }
            return copy;
        }
    }
}
