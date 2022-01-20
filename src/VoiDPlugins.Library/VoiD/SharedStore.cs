using System.Runtime.CompilerServices;

namespace VoiDPlugins.Library.VoiD
{
    /// <summary>
    /// Fast shared store with support for generic ref returning <see cref="GetData"/>. 
    /// </summary>
    public class SharedStore
    {
        private readonly object[] _data;

        public SharedStore()
        {
            _data = new object[32];
        }

        public void InitializeData<T>(int i, T data)
        {
            _data[i] = data!;
        }

        public unsafe ref T GetData<T>(int i)
        {
            return ref Unsafe.AsRef<T>(Unsafe.AsPointer(ref _data[i]));
        }
    }
}