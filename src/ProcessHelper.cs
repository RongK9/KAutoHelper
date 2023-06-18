using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KAutoHelper
{
    public class ProcessHelper
    {
        private delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

        [DllImport("user32")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndStart, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        public static List<string> windowTitles = new List<string>();

        public static List<string> GetWindowTitles(bool includeChildren)
        {
            EnumWindows(EnumWindowsCallback, includeChildren ? (IntPtr)1 : IntPtr.Zero);
            return windowTitles;
        }

        public static bool EnumWindowsCallback(IntPtr testWindowHandle, IntPtr includeChildren)
        {
            string title = GetWindowTitle(testWindowHandle);
            if (TitleMatches(title))
            {
                windowTitles.Add(title);
            }
            if (includeChildren.Equals(IntPtr.Zero) == false)
            {
                EnumChildWindows(testWindowHandle, EnumWindowsCallback, IntPtr.Zero);
            }
            return true;
        }
        public static bool TitleMatches(string title)
        {
            bool match = title.Contains("e");
            return match;
        }
        public static string GetWindowTitle(IntPtr windowHandle)
        {
            uint SMTO_ABORTIFHUNG = 0x0002;
            uint WM_GETTEXT = 0xD;
            int MAX_STRING_SIZE = 32768;
            IntPtr result;
            string title = string.Empty;
            IntPtr memoryHandle = Marshal.AllocCoTaskMem(MAX_STRING_SIZE);
            Marshal.Copy(title.ToCharArray(), 0, memoryHandle, title.Length);
            SendMessageTimeout(windowHandle, WM_GETTEXT, (IntPtr)MAX_STRING_SIZE, memoryHandle, SMTO_ABORTIFHUNG, (uint)1000, out result);
            title = Marshal.PtrToStringAuto(memoryHandle);
            Marshal.FreeCoTaskMem(memoryHandle);
            return title;
        }

        public static List<Port> GetNetStatPorts()
        {
            var Ports = new List<Port>();

            try
            {
                using (Process p = new Process())
                {
                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
                    ps.CreateNoWindow = true;
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        // Command Errored. Handle Here If Need Be
                    }

                    //Get The Rows
                    string[] rows = Regex.Split(content, "\r\n");
                    foreach (string row in rows)
                    {
                        //Split it baby
                        string[] tokens = Regex.Split(row, "\\s+");
                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            Ports.Add(new Port
                            {
                                protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                port_number = localAddress.Split(':')[1],
                                process_name = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5])),
                                pid = Convert.ToInt16(tokens[5])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message)
            }
            return Ports;
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }
    }
    public class Port
    {
        public string name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
            }
            set { }
        }
        public string port_number { get; set; }
        public string process_name { get; set; }
        public string protocol { get; set; }
        public int pid { get; set; }
    }

    /// <summary>
    /// A utility class to determine a process parent.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ParentProcessUtilities
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1;
        internal IntPtr PebBaseAddress;
        internal IntPtr Reserved2_0;
        internal IntPtr Reserved2_1;
        internal IntPtr UniqueProcessId;
        internal IntPtr InheritedFromUniqueProcessId;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        /// <summary>
        /// Gets the parent process of the current process.
        /// </summary>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess()
        {
            return GetParentProcess(Process.GetCurrentProcess().Handle);
        }

        /// <summary>
        /// Gets the parent process of specified process.
        /// </summary>
        /// <param name="id">The process id.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(int id)
        {
            Process process = Process.GetProcessById(id);
            return GetParentProcess(process.Handle);
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(IntPtr handle)
        {
            ParentProcessUtilities pbi = new ParentProcessUtilities();
            int returnLength;
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
                throw new Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }
    }
}
