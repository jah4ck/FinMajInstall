using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FinMajInstall
{
    public class Trace
    {
        public void WriteLog(string arg0, int arg1, string arg2)
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
            startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + arg2 + "";
            Process Trace = Process.Start(startinfo);
            Trace.WaitForExit();
        }
    }
}
