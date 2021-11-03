using System;
using System.Collections.Generic;
using System.Linq;

namespace SCADA.Common.SyncCollections
{
    public class ThreadSafeDictionaryDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _dictionaryInternal = new Dictionary<TKey, TValue>();
        private object _lockObj = new object();

        public void Add(TKey key, TValue value)
        {
            lock (_lockObj)
            {
                _dictionaryInternal.Add(key, value); 
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_lockObj)
                {
                    return _dictionaryInternal[key];
                }
            }
            set
            {
                lock (_lockObj)
                {
                    _dictionaryInternal[key] = value;
                }
            }
        }


        public bool Remove(TKey key)
        {
            lock (_lockObj)
            {
                return _dictionaryInternal.Remove(key);
            }
        }

        public int Count
        {
            get
            {
                lock (_lockObj)
                {
                    return _dictionaryInternal.Count;
                }
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _dictionaryInternal.Clear();
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return getCopy().Select(x => x.Key).ToList();
            }
        }
        public ICollection<TValue> Values
        {
            get
            {
                return getCopy().Select(x => x.Value).ToList();
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_lockObj)
            {
                return _dictionaryInternal.ContainsKey(key);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return getCopy().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return getCopy().GetEnumerator();
        }

        private List<KeyValuePair<TKey, TValue>> getCopy()
        {
            List<KeyValuePair<TKey, TValue>> copy = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
            {
                foreach (var item in _dictionaryInternal)
                    copy.Add(item);
            }
            return copy;
        }
    }
}

