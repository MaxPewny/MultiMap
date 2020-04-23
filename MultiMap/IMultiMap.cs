using System;
using System.Collections.Generic;
using System.Text;

namespace MultiMap
{
    public delegate void EventDelegate<T>(object source, T pT);

    public interface IMultiMap<K, V> : IEnumerable<KeyValuePair<K,V>>
    {
        IEnumerable<K> Keys { get; } // get all Keys in Multimap
        IEnumerable<V> Values { get; } // get all Values in Multimap
        IEnumerable<V> this[K key] { get; } // get values assigned to the given key

        event EventDelegate<K> AddKeyEvent; // Delegate for Event for added Keys
        event EventDelegate<V> AddValueEvent; // Delegate for Event for added Values

        event EventDelegate<K> RemoveKeyEvent; // Delegate for Event for removed Keys
        event EventDelegate<V> RemoveValueEvent; // Delegate for Event for removed Values

        bool ContainsKey(K pK); // returns true if the Multimap contains the given Key
        bool ContainsValue(K pK, V pV); // returns true if the Multimap contains the given Value

        void Add(K pK, V pV); // add a Key Value pair
        void Add<K2, V2>(IMultiMap<K2, V2> pMultiMap) // Add another Multimap where Key and Value of the added Map inherit from Key and Value of the Multimap 
            where K2 : K 
            where V2 : V;

        void Remove(K pK, V pV); // Remove Key Value Pair
        void RemoveAll(System.Func<K,V, bool> pFunc); // Removes all Key Value pairs and calls given Function with removed Key Value Pair as parameters
    }
}
