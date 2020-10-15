using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System.Collections;

namespace ScriptRunner
{
    internal static class ScriptRunnerHelper
    {
        public static Dictionary<int, string> ScriptPathList = new Dictionary<int, string>();
    }

    [PluginName("Script Runner")]
    public class ScriptRunner : ITool, IValidateBinding, IBinding
    {
        public Dictionary<int, string> ScriptPathList = ScriptRunnerHelper.ScriptPathList;
        public string[] ValidProperties
        {
            get
            {
                string[] Properties = new string[10];
                for (int index = 0; index <= 9; index++)
                {
                    Properties[index] = $"Script Path {index}";
                }
                return Properties;
            }
        }
        public string Property { set; get; }
        public Action Press => (Action)(() =>  RunScript(ScriptPathList[Int32.Parse(Property)]) );
        private void RunScript(string path)
        {
            //TODO: distinguish between platforms
            new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
        public Action Release => (Action)(() => { });
        public bool Initialize() { return true; }
        public void Dispose() { }

        //TODO: make this dynamic
        [Property("Script Path 0")]
        public string ScriptPath0
        {
            get => ScriptPathList[0];
            set { ScriptPathList[0] = value; }
        }

        [Property("Script Path 1")]
        public string ScriptPath1
        {
            get => ScriptPathList[1];
            set { ScriptPathList[1] = value; }
        }

        [Property("Script Path 2")]
        public string ScriptPath2
        {
            get => ScriptPathList[2];
            set { ScriptPathList[2] = value; }
        }

        [Property("Script Path 3")]
        public string ScriptPath3
        {
            get => ScriptPathList[3];
            set { ScriptPathList[3] = value; }
        }

        [Property("Script Path 4")]
        public string ScriptPath4
        {
            get => ScriptPathList[4];
            set { ScriptPathList[4] = value; }
        }

        [Property("Script Path 5")]
        public string ScriptPath5
        {
            get => ScriptPathList[5];
            set { ScriptPathList[5] = value; }
        }

        [Property("Script Path 6")]
        public string ScriptPath6
        {
            get => ScriptPathList[6];
            set { ScriptPathList[6] = value; }
        }

        [Property("Script Path 7")]
        public string ScriptPath7
        {
            get => ScriptPathList[7];
            set { ScriptPathList[7] = value; }
        }

        [Property("Script Path 8")]
        public string ScriptPath8
        {
            get => ScriptPathList[8];
            set { ScriptPathList[8] = value; }
        }

        [Property("Script Path 9")]
        public string ScriptPath9
        {
            get => ScriptPathList[9];
            set { ScriptPathList[9] = value; }
        }
    }
}