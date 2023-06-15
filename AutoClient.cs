using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KAutoHelper
{
    public class AutoClient
    {
        public static int cmd_start = 1000;
        public static int cmd_end = 1001;
        public static int cmd_push = 1002;
        public const int cmd_sendchar = 1003;

        public IntPtr WindowHwnd;
        public uint processId;
        public uint HookMsg;
        public void Attach(IntPtr hwnd)
        {
            this.WindowHwnd = hwnd;
            MemoryHelper.GetWindowThreadProcessId(WindowHwnd, out processId);
            MemoryHelper.OpenProcess(processId);
        }

        // Thủ tục định nghĩa chưa Inject hook
        private bool _isInjected = false;

        // Thủ tục định nghĩa chưa Inject hook
        public bool isInjected
        {
            get { return _isInjected; }
        }

        // Thủ tục Inject hook
        public int Inject()
        {
            var result = HookGame.InjectDll(WindowHwnd);
            if (result == 1)
            {
                _isInjected = true;
                this.HookMsg = HookGame.GetMsg();
            }
            return result;
        }

        // Thủ tục DeInject hook
        public int DeInject()
        {
            var result = HookGame.UnmapDll(WindowHwnd);
            _isInjected = false;
            return result;
        }
    }

    public static class HookGame
    {
        [DllImport("khook.dll", SetLastError = true)]
        public static extern Int32 InjectDll(IntPtr gameHwnd);
        [DllImport("khook.dll", SetLastError = true)]
        public static extern Int32 UnmapDll(IntPtr gameHwnd);
        [DllImport("khook.dll", SetLastError = true)]
        public static extern UInt32 GetMsg();
    }
}
