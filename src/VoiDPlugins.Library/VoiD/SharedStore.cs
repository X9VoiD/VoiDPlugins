using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Library.VoiD
{
    public class SharedStore : IDictionary<int, object>
    {
        private static readonly Dictionary<TabletReference, Dictionary<string, SharedStore>> _storeMap = new(new Comparer());

        private readonly Dictionary<int, object> _sharedStore = new();

        public object this[int key] { get => ((IDictionary<int, object>)_sharedStore)[key]; set => ((IDictionary<int, object>)_sharedStore)[key] = value; }
        public ICollection<int> Keys => ((IDictionary<int, object>)_sharedStore).Keys;
        public ICollection<object> Values => ((IDictionary<int, object>)_sharedStore).Values;
        public int Count => ((ICollection<KeyValuePair<int, object>>)_sharedStore).Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<int, object>>)_sharedStore).IsReadOnly;

        public static SharedStore GetStore(TabletReference reference, string storeKey)
        {
            lock (_storeMap)
            {
                ref var tabletStore = ref CollectionsMarshal.GetValueRefOrAddDefault(_storeMap, reference, out var exists);
                if (!exists)
                    tabletStore = new Dictionary<string, SharedStore>();

                ref var store = ref CollectionsMarshal.GetValueRefOrAddDefault(tabletStore!, storeKey, out var exists2);
                if (!exists2)
                    store = new SharedStore();

                return store!;
            }
        }

        public T Get<T>(int key)
        {
            return (T)_sharedStore[key];
        }

        public void Set<T>(int key, T value)
        {
            _sharedStore[key] = value!;
        }

        public void Add(int key, object value)
        {
            ((IDictionary<int, object>)_sharedStore).Add(key, value);
        }

        public void Add(KeyValuePair<int, object> item)
        {
            ((ICollection<KeyValuePair<int, object>>)_sharedStore).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<int, object>>)_sharedStore).Clear();
        }

        public bool Contains(KeyValuePair<int, object> item)
        {
            return ((ICollection<KeyValuePair<int, object>>)_sharedStore).Contains(item);
        }

        public bool ContainsKey(int key)
        {
            return ((IDictionary<int, object>)_sharedStore).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<int, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, object>>)_sharedStore).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<int, object>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, object>>)_sharedStore).GetEnumerator();
        }

        public bool Remove(int key)
        {
            return ((IDictionary<int, object>)_sharedStore).Remove(key);
        }

        public bool Remove(KeyValuePair<int, object> item)
        {
            return ((ICollection<KeyValuePair<int, object>>)_sharedStore).Remove(item);
        }

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out object value)
        {
            return ((IDictionary<int, object>)_sharedStore).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_sharedStore).GetEnumerator();
        }

        private class Comparer : IEqualityComparer<TabletReference>
        {
            public bool Equals(TabletReference? x, TabletReference? y)
            {
                return x?.Properties.Name == y?.Properties.Name;
            }

            public int GetHashCode(TabletReference obj)
            {
                return obj.Properties.Name.GetHashCode();
            }
        }
    }
}
