using System;
using HidSharp;
using OpenTabletDriver.Plugin;

namespace VoiDPlugins.Library.VMulti
{
    public static class VMultiDevice
    {
        public static HidStream Retrieve(string Name)
        {
            HidStream VMultiDev = null;
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
}