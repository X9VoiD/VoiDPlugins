using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HidSharp;
using HidSharp.Reports;
using OpenTabletDriver.Plugin;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    public class VMultiInstance
    {
        private readonly HidStream? _device;
        private readonly bool _extended;
        protected readonly byte[] Buffer;
        public unsafe VMultiReportHeader* Header { get; }
        public bool Extended => _extended;

        public unsafe VMultiInstance(string name, int size)
        {
            Buffer = GC.AllocateArray<byte>(size, true);
            Header = (VMultiReportHeader*)Unsafe.AsPointer(ref Buffer[0]);
            _device = Retrieve(name, out _extended);
        }

        public void Write()
        {
            _device?.Write(Buffer);
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

        private static HidStream? Retrieve(string Name, out bool extended)
        {
            HidStream? VMultiDev = null;
            var devices = DeviceList.Local.GetHidDevices(vendorID: 255, productID: 47820).ToArray();

            foreach (var device in devices)
            {
                if (device.GetMaxOutputReportLength() == 65 && device.GetMaxInputReportLength() == 65)
                {
                    if (device.TryOpen(out VMultiDev))
                        break;
                }
            }

            bool normal = false;
            extended = false;

            foreach (var device in devices)
            {
                if (device.GetMaxInputReportLength() == 10)
                {
                    var reportDescriptor = device.GetReportDescriptor();
                    if (reportDescriptor.TryGetReport(ReportType.Input, DigitizerInputReport.NormalReportID, out _))
                        normal = true;
                    if (reportDescriptor.TryGetReport(ReportType.Input, DigitizerInputReport.ExtendedReportID, out _))
                        extended = true;
                }

                if (normal && extended)
                    break;
            }

            if (VMultiDev == null || (!normal && !extended))
            {
                Log.WriteNotify(Name, "Cannot find VirtualHID. Install VMulti driver here: https://github.com/X9VoiD/vmulti-bin/releases/latest", LogLevel.Error);
            }

            return VMultiDev;
        }
    }

    public class VMultiInstance<T> : VMultiInstance where T : unmanaged
    {
        public unsafe T* Pointer { get; }

        public unsafe VMultiInstance(string name, T initialValue) : base(name, Unsafe.SizeOf<T>())
        {
            Pointer = (T*)Unsafe.AsPointer(ref Buffer[0]);
            *Pointer = initialValue;
        }

        public unsafe VMultiInstance(string name, Func<bool, T> initialValue) : base(name, Unsafe.SizeOf<T>())
        {
            Pointer = (T*)Unsafe.AsPointer(ref Buffer[0]);
            *Pointer = initialValue(Extended);
        }
    }
}