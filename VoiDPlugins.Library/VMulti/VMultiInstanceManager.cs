using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Library.VMulti
{
    public static class VMultiInstanceManager
    {
        private static readonly object _syncLock = new();
        private static readonly Dictionary<string, VMultiInstance> _map = new();

        public static VMultiInstance<T> RetrieveVMultiInstance<T>(string name, TabletReference tabletReference, Func<T> initialValue)
            where T : unmanaged
        {
            lock (_syncLock)
            {
                ref var instance = ref CollectionsMarshal.GetValueRefOrAddDefault(_map, tabletReference.Properties.Name, out var exists);
                if (!exists)
                    instance = new VMultiInstance<T>(name, initialValue);
                return (VMultiInstance<T>)instance!;
            }
        }

        public static VMultiInstance RetrieveVMultiInstance(TabletReference tabletReference)
        {
            return _map[tabletReference.Properties.Name];
        }
    }
}