using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MultiMap
{
    class ValueNotAllowedException  : System.ApplicationException // Custom Value not allowed Exception
    {
        public ValueNotAllowedException(string message) : base(message) { }
    }

    public delegate bool ExceptionCaseDelegate<V>(V pV); // custom Delegate for ExceptionFunction

    class MultiMap<K, V> : IMultiMap<K, V> // Implementation of IMultimap
    {
        public IEnumerable<V> this[K key]
        {
            get
            {
                List<V> values;
                if (mDict.TryGetValue(key, out values))
                {
                    return values; // returns all Values assigned to the given Key
                }
                else
                {
                    return new List<V>(); // returns empty List if no matching Values where found
                }
            }
        }

        public IEnumerable<K> Keys => mDict.Keys; // returns all Keys of the Dictionary

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

        private Dictionary<K, List<V>> mDict = new Dictionary<K, List<V>>(); // Dictionary where Keys and Values are stored
        private ExceptionCaseDelegate<V> mExceptionFunctor; // ExceptionFunction Delegate

        public event EventDelegate<K> AddKeyEvent;
        public event EventDelegate<V> AddValueEvent;
        public event EventDelegate<K> RemoveKeyEvent;
        public event EventDelegate<V> RemoveValueEvent;

        public MultiMap(ExceptionCaseDelegate<V> pDelegate) // Constructor takes a Delegate Function
        {
            mExceptionFunctor = pDelegate; // stores the given function in the Delegate
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
            if (mExceptionFunctor(pV)) // throws Exception if Value doesnt fit the requirements defined through the Functor
            {
                throw new ValueNotAllowedException(String.Format("EXCEPTION: Value can't be {0}", pV));
            }
            List<V> values;
            if (mDict.TryGetValue(pK, out values)) // Adds Value to the Key if Key already exists
            {
                values.Add(pV);
                OnAddValueEvent(pV);
            }
            else // Creates new Entry in Dict with given Key and Value
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
                    Add(key, value); // Calls Add for every Key Value pair in added Multimap
                }
            }
        }

        public void Remove(K pK, V pV)
        {
            List<V> values;
            if (mDict.TryGetValue(pK, out values))
            {
                values.Remove(pV); // Removes Value
                OnRemoveValueEvent(pV);
                if (values.Count == 0)
                {
                    mDict.Remove(pK); // also removes Key if Value was the last assigned to the Key
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
                    bool funcBool = pFunc(pair.Key, value); // Removes the Value and executes the delegate function
                    if (funcBool)
                    {
                        OnRemoveValueEvent(value);
                    }
                    return funcBool;
                    }
                );
                if (pair.Value.Count == 0)
                {
                    toDeleteKeys.Add(pair.Key); // If all Values of the Key are removed adds Key to list of Key to delete
                }
            }
            foreach (K key in toDeleteKeys) // removes all keys 
            {
                mDict.Remove(key);
                OnRemoveKeyEvent(key);
            }
        }

        public bool ContainsKey(K pK) // Check if the MultiMap contains the given Parameter as a Key
        {
           return mDict.ContainsKey(pK);
        }

        public bool ContainsValue(K pK ,V pV) // Check if the MultiMap contains the given Parameter as a Value
        {
            List<V> values;
            if (mDict.TryGetValue(pK, out values))
            {
               return values.Contains(pV);
            }
            else return false;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() // Returns the MultiMap as a Enumerator
        {
            foreach (KeyValuePair<K, List<V>> pair in mDict)
            {
                foreach (V value in pair.Value)
                {
                    yield return new KeyValuePair<K, V>(pair.Key, value);
                }
            }
        }


        IEnumerator IEnumerable.GetEnumerator() // not used in this case
        {
            throw new NotImplementedException();
        }
    }
}
