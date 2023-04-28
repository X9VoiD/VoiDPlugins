using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTabletDriver;
using OpenTabletDriver.Tablet;

namespace VoiDPlugins.Library.VoiD
{
    public class SharedStore
    {
        private static readonly Dictionary<InputDevice, Dictionary<string, SharedStore>> _storeMap = new();

        private readonly Dictionary<int, object> _sharedStore = new();

        public static SharedStore GetStore(InputDevice inputDevice, string storeKey)
        {
            lock (_storeMap)
            {
                ref var tabletStore = ref CollectionsMarshal.GetValueRefOrAddDefault(_storeMap, inputDevice, out var exists);
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

        public T GetOrUpdate<T>(int key, Func<T> valueFactory, out bool updated)
        {
            if (_sharedStore.TryGetValue(key, out object? value) && SafeCast<T>(value) is T casted)
            {
                updated = false;
                return casted;
            }

            var newValue = valueFactory();
            _sharedStore[key] = newValue!;
            updated = true;
            return newValue;
        }

        public void Set<T>(int key, T value)
        {
            _sharedStore[key] = value!;
        }

        public void Add<T>(int key, T value)
        {
            _sharedStore.Add(key, value!);
        }

        public void SetOrAdd<T>(int key, T value)
        {
            if (_sharedStore.ContainsKey(key))
            {
                _sharedStore[key] = value!;
            }
            else
            {
                _sharedStore.Add(key, value!);
            }
        }

        private static T? SafeCast<T>(object value)
        {
            try
            {
                return (T)value;
            }
            catch
            {
                return default;
            }
        }
    }
}
