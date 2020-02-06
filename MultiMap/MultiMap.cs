using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MultiMap
{
    class NullNotAllowedException  : System.ApplicationException
    {
        public NullNotAllowedException(string message) : base(message) { }
    }

    public delegate bool ExceptionCaseDelegate<V>(V pV);

    class MultiMap<K, V> : IMultiMap<K, V>
    {
        public IEnumerable<V> this[K key]
        {
            get
            {
                List<V> values;
                if (mDict.TryGetValue(key, out values))
                {
                    return values;
                }
                else
                {
                    return new List<V>();
                }
            }
        }

        public IEnumerable<K> Keys => mDict.Keys;

        public IEnumerable<V> Values
        {
            get
            {
                foreach (KeyValuePair<K, List<V>> pair in mDict)
                {
                    foreach (V value in pair.Value)
                    {
                        yield return value;
                    }
                }
            }
        }

        private Dictionary<K, List<V>> mDict = new Dictionary<K, List<V>>();
        private ExceptionCaseDelegate<V> mExceptionFunctor;

        public event EventDelegate<K> AddKeyEvent;
        public event EventDelegate<V> AddValueEvent;
        public event EventDelegate<K> RemoveKeyEvent;
        public event EventDelegate<V> RemoveValueEvent;

        public MultiMap(ExceptionCaseDelegate<V> pDelegate) 
        {
            mExceptionFunctor = pDelegate;
        }

        protected virtual void OnAddKeyEvent(K pK) 
        {
            AddKeyEvent?.Invoke(this, pK);
        }
        protected virtual void OnAddValueEvent(V pV)
        {
            AddValueEvent?.Invoke(this, pV);
        }
        protected virtual void OnRemoveKeyEvent(K pK)
        {
            RemoveKeyEvent?.Invoke(this, pK);
        }
        protected virtual void OnRemoveValueEvent(V pV)
        {
            RemoveValueEvent?.Invoke(this, pV);
        }

        public void Add(K pK, V pV)
        {
            if (mExceptionFunctor(pV))
            {
                throw new NullNotAllowedException("EXCEPTION: Value can't be  NULL");
            }
            List<V> values;
            if (mDict.TryGetValue(pK, out values))
            {
                values.Add(pV);
                OnAddValueEvent(pV);
            }
            else
            {
                List<V> newValues = new List<V>();
                newValues.Add(pV);
                mDict.Add(pK, newValues);
                OnAddKeyEvent(pK);
                OnAddValueEvent(pV);

            }
        }

        public void Add<K2, V2>(IMultiMap<K2, V2> pMultiMap)
            where K2 : K
            where V2 : V
        {
            foreach (K2 key in pMultiMap.Keys)
            {
                foreach ( V2 value in pMultiMap[key])
                {
                    Add(key, value);
                }
            }
        }

        public void Remove(K pK, V pV)
        {
            List<V> values;
            if (mDict.TryGetValue(pK, out values))
            {
                values.Remove(pV);
                OnRemoveValueEvent(pV);
                if (values.Count == 0)
                {
                    mDict.Remove(pK);
                    OnRemoveKeyEvent(pK);
                }
            }
        }

        public void RemoveAll(Func<K, V, bool> pFunc)
        {
            List<K> toDeleteKeys = new List<K>();
            foreach (KeyValuePair<K, List<V>> pair in mDict)
            {
                pair.Value.RemoveAll((V value) => {
                    bool funcBool = pFunc(pair.Key, value);
                    if (funcBool)
                    {
                        OnRemoveValueEvent(value);
                    }
                    return funcBool;
                    }
                );
                if (pair.Value.Count == 0)
                {
                    toDeleteKeys.Add(pair.Key);
                }
            }
            foreach (K key in toDeleteKeys)
            {
                mDict.Remove(key);
                OnRemoveKeyEvent(key);
            }
        }

        public bool ContainsKey(K pK)
        {
           return mDict.ContainsKey(pK);
        }

        public bool ContainsValue(K pK ,V pV)
        {
            List<V> values;
            if (mDict.TryGetValue(pK, out values))
            {
               return values.Contains(pV);
            }
            else return false;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (KeyValuePair<K, List<V>> pair in mDict)
            {
                foreach (V value in pair.Value)
                {
                    yield return new KeyValuePair<K, V>(pair.Key, value);
                }
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
