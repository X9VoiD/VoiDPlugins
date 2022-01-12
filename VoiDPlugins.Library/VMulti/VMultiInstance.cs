using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HidSharp;
using OpenTabletDriver.Plugin;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    public class VMultiInstance
    {
        private readonly object[] _data;
        private readonly HidStream _device;
        protected readonly byte[] Buffer;
        public unsafe VMultiReportHeader* Header { get; }

        internal unsafe VMultiInstance(string name, int size)
        {
            Buffer = GC.AllocateArray<byte>(size, true);
            Header = (VMultiReportHeader*)Unsafe.AsPointer(ref Buffer[0]);
            _device = Retrieve(name);
            _data = new object[32];
        }

        public void Write()
        {
            _device.Write(Buffer);
        }

        public void InitializeData<T>(int i, T data)
        {
            _data[i] = data!;
        }

        public unsafe ref T GetData<T>(int i) where T : class
        {
            return ref Unsafe.AsRef<T>(Unsafe.AsPointer(ref _data[i]));
        }

        public unsafe void EnableButtonBit(int bit)
        {
            Header->Buttons = (byte)(Header->Buttons | bit);
        }

        public unsafe void DisableButtonBit(int bit)
        {
            Header->Buttons = (byte)(Header->Buttons & ~bit);
        }

        public unsafe bool HasButtonBit(int bit)
        {
            return HasBit(Header->Buttons, bit);
        }

        public static bool HasBit(byte buttons, int bit)
        {
            return (buttons & bit) != 0;
        }

        private static HidStream Retrieve(string Name)
        {
            HidStream? VMultiDev = null;
            foreach (var device in DeviceList.Local.GetHidDevices(productID: 47820))
            {
                if (device.GetMaxOutputReportLength() == 65 && device.GetMaxInputReportLength() == 65)
                {
                    if (device.TryOpen(out VMultiDev))
                        break;
                }
            }

            if (VMultiDev == null)
            {
                Log.Write(Name, "Cannot find VirtualHID", LogLevel.Error);
                Log.Write(Name, "Install VMulti driver here: https://github.com/X9VoiD/vmulti-bin/releases/latest", LogLevel.Error);
                throw new Exception();
            }

            return VMultiDev;
        }
    }

    public class VMultiInstance<T> : VMultiInstance where T : unmanaged
    {
        public unsafe T* Pointer { get; }

        internal unsafe VMultiInstance(string name, Func<T> initialValue) : base(name, Unsafe.SizeOf<T>())
        {
            Pointer = (T*)Unsafe.AsPointer(ref Buffer[0]);
            *Pointer = initialValue();
        }
    }
}