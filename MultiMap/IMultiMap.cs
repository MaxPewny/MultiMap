using System;
using System.Collections.Generic;
using System.Text;

namespace MultiMap
{
    public delegate void EventDelegate<T>(object source, T pT);

    public interface IMultiMap<K, V> : IEnumerable<KeyValuePair<K,V>>
    {
        IEnumerable<K> Keys { get; }
        IEnumerable<V> Values { get; }
        IEnumerable<V> this[K key] { get; }

        event EventDelegate<K> AddKeyEvent;
        event EventDelegate<V> AddValueEvent;

        event EventDelegate<K> RemoveKeyEvent;
        event EventDelegate<V> RemoveValueEvent;

        bool ContainsKey(K pK);
        bool ContainsValue(K pK, V pV);

        void Add(K pK, V pV);
        void Add<K2, V2>(IMultiMap<K2, V2> pMultiMap) 
            where K2 : K 
            where V2 : V;

        void Remove(K pK, V pV);
        void RemoveAll(System.Func<K,V, bool> pFunc);
    }
}
