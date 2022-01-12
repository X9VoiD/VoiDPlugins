using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Library.VoiD
{
    /// <summary>
    /// Stores one instance of <typeparamref name="T"/> per <see cref="TabletReference"/>.
    /// </summary>
    /// <typeparam name="T">A class.</typeparam>
    public static class GlobalStore<T> where T : class
    {
        private static readonly object _syncLock = new();
        private static readonly Dictionary<string, T> _map = new();

        public static U GetOrInitialize<U>(TabletReference tabletReference, Func<U> initialValue) where U : T
        {
            lock (_syncLock)
            {
                ref var instance = ref CollectionsMarshal.GetValueRefOrAddDefault(_map, tabletReference.Properties.Name, out var exists);
                if (!exists)
                    instance = initialValue();
                return (U)instance!;
            }
        }

        public static U Get<U>(TabletReference tabletReference) where U : T
        {
            return (U)_map[tabletReference.Properties.Name];
        }

        public static T Get(TabletReference tabletReference)
        {
            return _map[tabletReference.Properties.Name];
        }
    }
}