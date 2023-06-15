using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace KAutoHelper
{
    public class AutoControl
    {
        #region win API import
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public delegate bool CallBack(IntPtr hwnd, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, CallBack lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

        [DllImport("user32.dll")]
        static extern bool SetDlgItemTextA(IntPtr hWnd, int nIDDlgItem, string gchar);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

      

        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100,
            XBUTTON1 = 0x00000001,
            XBUTTON2 = 0x00000002
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion

        #region Hỗ trợ handle
        /// <summary>
        /// Đưa cửa sổ được truyền vào lên top most
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static IntPtr BringToFront(string className, string windowName = null)
        {
            IntPtr hWnd = FindWindow(className, windowName);
            SetForegroundWindow(hWnd);

            return hWnd;
        }

        /// <summary>
        /// Đưa cửa sổ được truyền vào lên top most
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static IntPtr BringToFront(IntPtr hWnd)
        {
            SetForegroundWindow(hWnd);

            return hWnd;
        }

        /// <summary>
        /// Kiem tra xem cua so do co hien ra chua
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool IsWindowVisible_(IntPtr handle)
        {
            return IsWindowVisible(handle);
        }
        #endregion

        #region Tìm handle
        /// <summary>
        /// Lấy ra window handle kiểu dữ liệu IntPtr từ Class name và WindowName (là cái text hiển thị trong spy hay là WindowCaption). Tìm hiểu thêm tại https://www.howkteam.com/
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <returns></returns>
        public static IntPtr FindWindowHandle(string className, string windowName)
        {
            IntPtr hwnd = IntPtr.Zero;
            hwnd = FindWindow(className, windowName);
            return hwnd;
        }

        /// <summary>
        /// Tìm tất cả các process thỏa mãn class name và window name. có thể tìm một lúc nhiều process.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static List<IntPtr> FindWindowHandlesFromProcesses(string className, string windowName, int maxCount = 1)
        {
            var processes = Process.GetProcesses();
            List<IntPtr> handle = new List<IntPtr>();
            int count = 0;
            var handlesNotZero = processes;//.Where(p => p.MainWindowHandle != IntPtr.Zero);
            foreach (var item in handlesNotZero)
            {
                IntPtr ptr = item.MainWindowHandle;
                var classname = GetClassName(ptr);
                var textDisplay = GetText(ptr);

                if (classname == className || textDisplay == windowName)
                {
                    handle.Add(ptr);
                    if (count >= maxCount)
                        break;
                    count++;
                }
            }

            return handle;
        }

        /// <summary>
        /// Tìm process thỏa mãn class name và window name.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static IntPtr FindWindowHandleFromProcesses(string className, string windowName)
        {
            var processes = Process.GetProcesses();
            IntPtr handle = IntPtr.Zero;
            foreach (var item in processes.Where(p => p.MainWindowHandle != IntPtr.Zero))
            {
                IntPtr ptr = item.MainWindowHandle;
                var classname = GetClassName(ptr);
                var textDisplay = GetText(ptr);

                if (classname == className || textDisplay == windowName)
                {
                    handle = ptr;
                    break;
                }
            }

            return handle;
        }               

        /// <summary>
        /// Tìm handle của cửa sổ module con trong handle cha
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="text"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IntPtr FindWindowExFromParent(IntPtr parentHandle, string text, string className)
        {
            return FindWindowEx(parentHandle, IntPtr.Zero, className, text);
        }

        /// <summary>
        /// Tìm control theo thứ tự của nó
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        static IntPtr FindWindowByIndex(IntPtr hWndParent, int index)
        {
            if (index == 0)
                return hWndParent;
            else
            {
                int ct = 0;
                IntPtr result = IntPtr.Zero;
                do
                {
                    result = FindWindowEx(hWndParent, result, "Button", null);
                    if (result != IntPtr.Zero)
                        ++ct;
                }
                while (ct < index && result != IntPtr.Zero);
                return result;
            }
        }

        /// <summary>
        /// Lấy ra control handle từ control id. ControlID tìm bằng cách dùng spy++ -> Find -> kéo vào control -> nhấn ok -> ControlID.
        /// Ví dụ tìm được giá trị 00001397 thì ControlID là 0x1397
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="controlId"></param>
        /// <returns></returns>
        public static IntPtr GetControlHandleFromControlID(IntPtr parentHandle, int controlId)
        {
            IntPtr hWndButton = GetDlgItem(parentHandle, controlId);
            return hWndButton;
        }               

        /// <summary>
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <returns></returns>
        public static List<IntPtr> GetChildHandle(IntPtr parentHandle)
        {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc((hWnd, lParam) =>
                {
                    GCHandle _gcChildhandlesList = GCHandle.FromIntPtr(lParam);

                    if (_gcChildhandlesList == null || _gcChildhandlesList.Target == null)
                    {
                        return false;
                    }

                    List<IntPtr> _childHandles = _gcChildhandlesList.Target as List<IntPtr>;
                    _childHandles.Add(hWnd);

                    return true;
                });
                EnumChildWindows(parentHandle, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }
            return childHandles;
        }

        /// <summary>
        /// Tìm thằng có thông tin trùng với class name hoặc text từ đám lâu la đưa vào
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="className"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IntPtr FindHandleWithText(List<IntPtr> handles,string className, string text)
        {
            var panelHandle = handles.Find(ptr =>
            {
                var classname = GetClassName(ptr);
                var textDisplay = GetText(ptr);

                if (classname == className || textDisplay == text)
                    return true;
                return false;
            });

            return panelHandle;
        }

        /// <summary>
        /// Lấy ra danh sách handle thỏa
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="className"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<IntPtr> FindHandlesWithText(List<IntPtr> handles, string className, string text)
        {
            List<IntPtr> handless = new List<IntPtr>();
            var panelHandle = handles.Where(ptr =>
            {
                var classname = GetClassName(ptr);
                var textDisplay = GetText(ptr);

                if (classname == className || textDisplay == text)
                {
                    return true;
                }
                return false;
            });

            handless = panelHandle.ToList();

            return handless;
        }

        /// <summary>
        /// Tìm ra handle con thỏa mãn class name hoặc text trùng
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="className"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IntPtr FindHandle(IntPtr parentHandle, string className, string text)
        {
            return FindHandleWithText(GetChildHandle(parentHandle), className, text);
        }

        /// <summary>
        /// Tìm tất cả handle thỏa điều kiện
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="className"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<IntPtr> FindHandles(IntPtr parentHandle, string className, string text)
        {
            return FindHandlesWithText(GetChildHandle(parentHandle), className, text);
        }

        /// <summary>
        /// Hàm callback của EnumChildWindows
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static bool CallbackChild(IntPtr hWnd, IntPtr lParam)
        {
            string text = GetText(hWnd);
            string className = GetClassName(hWnd);

            //OK: These need to be class level feilds or something
            if (text == "&Options >>" && className.StartsWith("ToolbarWindow32"))
            {
                //send BN_CLICKED message
                SendMessage(hWnd, 0, IntPtr.Zero, IntPtr.Zero);
                return false;
            }
            return true;
        }
        #endregion

        #region Giả lập chuột ngầm
        /// <summary>
        /// Click vào control dựa vào handle của parent và controlID của control. ControlID tìm bằng cách dùng spy++ -> Find -> kéo vào control -> nhấn ok -> ControlID.
        /// Ví dụ tìm được giá trị 00001397 thì ControlID là 0x1397
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="parentHWND"></param>
        /// <param name="controlID"></param>
        public static void SendClickOnControlById(IntPtr parentHWND, int controlID)
        {
            IntPtr hWndButton = GetDlgItem(parentHWND, controlID);
            int wParam = ((int)WMessages.BN_CLICKED << 16) | (controlID & 0xffff);
            SendMessage(parentHWND, (int)WMessages.WM_COMMAND, wParam, hWndButton);
        }

        /// <summary>
        /// Gửi click tới control theo handle
        /// </summary>
        /// <param name="hWndButton"></param>
        public static void SendClickOnControlByHandle(IntPtr hWndButton)
        {
            SendMessage(hWndButton, (int)WMessages.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hWndButton, (int)WMessages.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Click vào vị trí theo x,y của 1 control nào đó. dựa vào Control Handle. ControlHandle có thể tìm bằng hàm GetControlHandleFromControlID.
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="controlHandle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mouseButton"></param>
        /// <param name="clickTimes"></param>
        /// 
       
        public static void SendClickOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = EMouseKey.LEFT, int clickTimes = 1)
        {
            int btnDown = 0;
            int btnUp = 0;
            if (mouseButton == EMouseKey.LEFT)
            {
                btnDown = (int)WMessages.WM_LBUTTONDOWN;
                btnUp = (int)WMessages.WM_LBUTTONUP;
            }
            if (mouseButton == EMouseKey.RIGHT)
            {
                btnDown = (int)WMessages.WM_RBUTTONDOWN;
                btnUp = (int)WMessages.WM_RBUTTONUP;
            }            

            var point = MakeLParamFromXY(x, y);
            //const uint moveCode = 0x200;
            // SendMessage(controlHandle, moveCode, IntPtr.Zero, point);
            if (mouseButton == EMouseKey.LEFT || mouseButton == EMouseKey.RIGHT)
            {
                for (int i = 0; i < clickTimes; i++)
                {
                    PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1)/*IntPtr.Zero*/, point);
                    //PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1)/*IntPtr.Zero*/, point);
                    PostMessage(controlHandle, btnDown, new IntPtr(1)/*IntPtr.Zero*/, point);
                    PostMessage(controlHandle, btnUp, new IntPtr(0)/*IntPtr.Zero*/, point);
                }
            }
            else
            {
                if (mouseButton == EMouseKey.DOUBLE_LEFT)
                {
                    btnDown = (int)WMessages.WM_LBUTTONDBLCLK;
                    btnUp = (int)WMessages.WM_LBUTTONUP;
                }
                if (mouseButton == EMouseKey.DOUBLE_RIGHT)
                {
                    btnDown = (int)WMessages.WM_RBUTTONDBLCLK;
                    btnUp = (int)WMessages.WM_RBUTTONUP;
                }
                //PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1)/*IntPtr.Zero*/, point);
                PostMessage(controlHandle, btnDown, new IntPtr(1)/*IntPtr.Zero*/, point);
                PostMessage(controlHandle, btnUp, new IntPtr(0)/*IntPtr.Zero*/, point);
            }
        }

        static Random rand = new Random();
        public static void SendClickOnPositionRandom(IntPtr controlHandle, int x, int y,int ranX, int ranY, EMouseKey mouseButton = EMouseKey.LEFT, int clickTimes = 1)
        {
            int btnDown = 0;
            int btnUp = 0;
            if (mouseButton == EMouseKey.LEFT)
            {
                btnDown = (int)WMessages.WM_LBUTTONDOWN;
                btnUp = (int)WMessages.WM_LBUTTONUP;
            }
            if (mouseButton == EMouseKey.RIGHT)
            {
                btnDown = (int)WMessages.WM_RBUTTONDOWN;
                btnUp = (int)WMessages.WM_RBUTTONUP;
            }

            ranX = rand.Next(ranX);
            ranY = rand.Next(ranY);

            var point = MakeLParamFromXY(x + ranX, y + ranY);
            //const uint moveCode = 0x200;
            // SendMessage(controlHandle, moveCode, IntPtr.Zero, point);
            if (mouseButton == EMouseKey.LEFT || mouseButton == EMouseKey.RIGHT)
            {
                for (int i = 0; i < clickTimes; i++)
                {
                    PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1)/*IntPtr.Zero*/, point);
                    //PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1)/*IntPtr.Zero*/, point);
                    PostMessage(controlHandle, btnDown, new IntPtr(1)/*IntPtr.Zero*/, point);
                    PostMessage(controlHandle, btnUp, new IntPtr(0)/*IntPtr.Zero*/, point);
                }
            }
            else
            {
                if (mouseButton == EMouseKey.DOUBLE_LEFT)
                {
                    btnDown = (int)WMessages.WM_LBUTTONDBLCLK;
                    btnUp = (int)WMessages.WM_LBUTTONUP;
                }
                if (mouseButton == EMouseKey.DOUBLE_RIGHT)
                {
                    btnDown = (int)WMessages.WM_RBUTTONDBLCLK;
                    btnUp = (int)WMessages.WM_RBUTTONUP;
                }
                //PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1)/*IntPtr.Zero*/, point);
                PostMessage(controlHandle, btnDown, new IntPtr(1)/*IntPtr.Zero*/, point);
                PostMessage(controlHandle, btnUp, new IntPtr(0)/*IntPtr.Zero*/, point);
            }
        }

        public static void SendDragAndDropOnPosition(IntPtr controlHandle, int x, int y, int x2, int y2, int stepx = 10, int stepy = 10, double delay = 0.05)
        {
            int btnDown = 0;
            int btnUp = 0;

            btnDown = (int)WMessages.WM_LBUTTONDOWN;
            btnUp = (int)WMessages.WM_LBUTTONUP;
            

            var point = MakeLParamFromXY(x, y);
            var point2 = MakeLParamFromXY(x2, y2);


            if (x2 < x)
            {
                stepx *= -1;
            }
            if (y2 < y)
            {
                stepy *= -1;
            }

            PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), point);           
            PostMessage(controlHandle, btnDown, new IntPtr(1), point);

            bool isStopX = false;
            bool isStopY = false;
            while (true)
            {
                PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1), MakeLParamFromXY(x, y));

                if (stepx > 0)
                {
                    if (x < x2)
                    {
                        x += stepx;
                    }
                    else
                    {
                        isStopX = true;
                    }
                }
                else
                {
                    if (x > x2)
                    {
                        x += stepx;
                    }
                    else
                    {
                        isStopX = true;
                    }
                }

                if (stepy > 0)
                {
                    if (y < y2)
                    {
                        y += stepy;
                    }
                    else
                    {
                        isStopY = true;
                    }
                }
                else
                {
                    if (y > y2)
                    {
                        y += stepy;
                    }
                    else
                    {
                        isStopY = true;
                    }
                }

                if (isStopX && isStopY)
                    break;

                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
            
            PostMessage(controlHandle, btnUp, new IntPtr(0), point2);
        }

        public static void SendDragAndDropOnMultiPosition(IntPtr controlHandle, Point[] points, int stepx = 10, int stepy = 10, double delay = 0.05)
        {
            if (points == null || points.Length < 2)
                return;

            int btnDown = 0;
            int btnUp = 0;

            btnDown = (int)WMessages.WM_LBUTTONDOWN;
            btnUp = (int)WMessages.WM_LBUTTONUP;


            var point = MakeLParamFromXY(points[0].X, points[0].Y);
            PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), point);
            PostMessage(controlHandle, btnDown, new IntPtr(1), point);

            for (int i = 0; i < points.Length-1; i++)
            {
                MouseMoveDrag(controlHandle, points[i].X, points[i].Y, points[i+1].X, points[i+1].Y, stepx, stepy, delay);
            }
         
            PostMessage(controlHandle, btnUp, new IntPtr(0), MakeLParamFromXY(points[points.Length-1].X, points[points.Length-1].Y));
        }

        static void MouseMoveDrag(IntPtr controlHandle, int x, int y, int x2, int y2, int stepx = 10, int stepy = 10, double delay = 0.05)
        {

            var point2 = MakeLParamFromXY(x2, y2);

            if (x2 < x)
            {
                stepx *= -1;
            }
            if (y2 < y)
            {
                stepy *= -1;
            }

            bool isStopX = false;
            bool isStopY = false;
            while (true)
            {
                PostMessage(controlHandle, (int)WMessages.WM_MOUSEMOVE, new IntPtr(1), MakeLParamFromXY(x, y));

                if (stepx > 0)
                {
                    if (x < x2)
                    {
                        x += stepx;
                    }
                    else
                    {
                        isStopX = true;
                    }
                }
                else
                {
                    if (x > x2)
                    {
                        x += stepx;
                    }
                    else
                    {
                        isStopX = true;
                    }
                }

                if (stepy > 0)
                {
                    if (y < y2)
                    {
                        y += stepy;
                    }
                    else
                    {
                        isStopY = true;
                    }
                }
                else
                {
                    if (y > y2)
                    {
                        y += stepy;
                    }
                    else
                    {
                        isStopY = true;
                    }
                }

                if (isStopX && isStopY)
                    break;

                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
        }

        /// <summary>
        /// Click vào vị trí theo x,y của 1 control nào đó. dựa vào Control Handle. ControlHandle có thể tìm bằng hàm GetControlHandleFromControlID.
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="controlHandle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mouseButton"></param>
        /// <param name="clickTimes"></param>
        public static void SendClickDownOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = EMouseKey.LEFT, int clickTimes = 1)
        {
            int btnDown = 0;
            if (mouseButton == EMouseKey.LEFT)
            {
                btnDown = (int)WMessages.WM_LBUTTONDOWN;
            }
            if (mouseButton == EMouseKey.RIGHT)
            {
                btnDown = (int)WMessages.WM_RBUTTONDOWN;
            }

            var point = MakeLParamFromXY(x, y);

            for (int i = 0; i < clickTimes; i++)
            {
                PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1)/*IntPtr.Zero*/, point);
                PostMessage(controlHandle, btnDown, new IntPtr(1), point);
            }
        }

        /// <summary>
        /// Click vào vị trí theo x,y của 1 control nào đó. dựa vào Control Handle. ControlHandle có thể tìm bằng hàm GetControlHandleFromControlID.
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="controlHandle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mouseButton"></param>
        /// <param name="clickTimes"></param>
        public static void SendClickUpOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = EMouseKey.LEFT, int clickTimes = 1)
        {
            int btnUp = 0;
            if (mouseButton == EMouseKey.LEFT)
            {
                btnUp = (int)WMessages.WM_LBUTTONUP;
            }
            if (mouseButton == EMouseKey.RIGHT)
            {
                btnUp = (int)WMessages.WM_RBUTTONUP;
            }

            var point = MakeLParamFromXY(x, y);

            for (int i = 0; i < clickTimes; i++)
            {
                PostMessage(controlHandle, (int)WMessages.WM_ACTIVATE, new IntPtr(1)/*IntPtr.Zero*/, point);
                SendMessage(controlHandle, btnUp, new IntPtr(0), point);
            }
        }
        #endregion

        #region Giả lập bàn phím gửi ngầm
        /// <summary>
        /// Dùng để set lại title của cửa sổ cho tiện auto
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="text"></param>
        public static void SendText(IntPtr handle, string text)
        {
            SendMessage(handle, (int)WMessages.WM_SETTEXT, 0, text);
        }

        /// <summary>
        /// Gửi phím ngầm đến app
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void SendKeyBoardPress(IntPtr handle, VKeys key)
        {
            //SendKeyBoardDown(handle, key);
            //SendKeyBoardUp(handle, key);
            PostMessage(handle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), new IntPtr(0));
            PostMessage(handle, (int)WMessages.WM_KEYDOWN, new IntPtr((int)key), new IntPtr(1));
            PostMessage(handle, (int)WMessages.WM_KEYUP, new IntPtr((int)key), new IntPtr(0));
        }

        public static void SendKeyBoardPressStepByStep(IntPtr handle, string message, float delay = 0.1f)
        {
            foreach (var item in message.ToLower())
            {
                VKeys key = VKeys.VK_0;
                switch (item)
                {
                    case '0':
                        key = VKeys.VK_0;
                        break;
                    case '1':
                        key = VKeys.VK_1;
                        break;
                    case '2':
                        key = VKeys.VK_2;
                        break;
                    case '3':
                        key = VKeys.VK_3;
                        break;
                    case '4':
                        key = VKeys.VK_4;
                        break;
                    case '5':
                        key = VKeys.VK_5;
                        break;
                    case '6':
                        key = VKeys.VK_6;
                        break;
                    case '7':
                        key = VKeys.VK_7;
                        break;
                    case '8':
                        key = VKeys.VK_8;
                        break;
                    case '9':
                        key = VKeys.VK_9;
                        break;
                    case 'a':
                        key = VKeys.VK_A;
                        break;
                    case 'b':
                        key = VKeys.VK_B;
                        break;
                    case 'c':
                        key = VKeys.VK_V;
                        break;
                    case 'd':
                        key = VKeys.VK_D;
                        break;
                    case 'e':
                        key = VKeys.VK_E;
                        break;
                    case 'f':
                        key = VKeys.VK_F;
                        break;
                    case 'g':
                        key = VKeys.VK_G;
                        break;
                    case 'h':
                        key = VKeys.VK_H;
                        break;
                    case 'i':
                        key = VKeys.VK_I;
                        break;
                    case 'j':
                        key = VKeys.VK_J;
                        break;
                    case 'k':
                        key = VKeys.VK_K;
                        break;
                    case 'l':
                        key = VKeys.VK_L;
                        break;
                    case 'm':
                        key = VKeys.VK_M;
                        break;
                    case 'n':
                        key = VKeys.VK_N;
                        break;
                    case 'o':
                        key = VKeys.VK_O;
                        break;
                    case 'p':
                        key = VKeys.VK_P;
                        break;
                    case 'q':
                        key = VKeys.VK_Q;
                        break;
                    case 'r':
                        key = VKeys.VK_R;
                        break;
                    case 's':
                        key = VKeys.VK_S;
                        break;
                    case 't':
                        key = VKeys.VK_T;
                        break;
                    case 'u':
                        key = VKeys.VK_U;
                        break;
                    case 'v':
                        key = VKeys.VK_V;
                        break;
                    case 'w':
                        key = VKeys.VK_W;
                        break;
                    case 'x':
                        key = VKeys.VK_X;
                        break;
                    case 'y':
                        key = VKeys.VK_Y;
                        break;
                    case 'z':
                        key = VKeys.VK_Z;
                        break;
                    default:
                        break;
                }
                SendKeyBoardPress(handle, key);
                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
        }

        /// <summary>
        /// Gửi phím ngầm đến app
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void SendKeyBoardUp(IntPtr handle, VKeys key)
        {
            PostMessage(handle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), new IntPtr(0));
            PostMessage(handle, (int)WMessages.WM_KEYUP, new IntPtr((int)key), new IntPtr(0));
        }

        /// <summary>
        /// gửi ascii theo
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void SendKeyChar(IntPtr handle, VKeys key)
        {
            PostMessage(handle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), new IntPtr(0));
            PostMessage(handle, (int)WMessages.WM_CHAR, new IntPtr((int)key), new IntPtr(0));
        }

        public static void SendKeyChar(IntPtr handle, int key)
        {
            PostMessage(handle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), new IntPtr(0));
            PostMessage(handle, (int)WMessages.WM_CHAR, new IntPtr(key), new IntPtr(0));
        }

        /// <summary>
        /// Gửi phím ngầm đến app
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void SendKeyBoardDown(IntPtr handle, VKeys key)
        {
            PostMessage(handle, (int)WMessages.WM_ACTIVATE, new IntPtr(1), new IntPtr(0));
            PostMessage(handle, (int)WMessages.WM_KEYDOWN, new IntPtr((int)key), new IntPtr(0));   
        }

        /// <summary>
        /// Gửi text đến app. giới hạn từ 0-9 và a-z viết thường
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        public static void SendTextKeyBoard(IntPtr handle, string text, float delay = 0.1f)
        {
            foreach (var item in text)
            {
                
                    SendKeyChar(handle, item);
                
                //Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
        }
        #endregion

        #region Giả lập bàn phím lên cửa sổ đang focus
        /// <summary>
        /// Gửi phím chiếm bàn phím vào cửa sổ đang focus
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="key"></param>
        public static void SendKeyFocus(KeyCode key)
        {
            SendKeyPress(key);
        }

        /// <summary>
        /// Gửi nhiều phím 1 lúc
        /// </summary>
        /// <param name="keys"></param>
        public static void SendMultiKeysFocus(KeyCode[] keys)
        {
            foreach (var item in keys)
            {
                SendKeyDown(item);
            }
            foreach (var item in keys)
            {
                SendKeyUp(item);
            }
        }

        /// <summary>
        /// Gửi một chuỗi ký tự chiếm bàn phím vào cửa sổ đang focus
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="message"></param>
        public static void SendStringFocus(string message)
        {
            Clipboard.SetText(message);
            SendMultiKeysFocus(new KeyCode[] { KeyCode.CONTROL, KeyCode.KEY_V});
        }

        /// <summary>
        /// Nhấn và nhả phím ra
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyPress(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            INPUT input2 = new INPUT
            {
                Type = 1
            };
            input2.Data.Keyboard = new KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 2,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };
            INPUT[] inputs = new INPUT[] { input, input2 };
            if (SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();
        }

        /// <summary>
        /// Gõ từng ký tự
        /// </summary>
        /// <param name="message"></param>
        public static void SendKeyPressStepByStep(string message, double delay = 0.5)
        {
            foreach (var item in message)
            {
                switch (item)
                {
                    case '0':
                        SendKeyPress(KeyCode.KEY_0);
                        break;
                    case '1':
                        SendKeyPress(KeyCode.KEY_1);
                        break;
                    case '2':
                        SendKeyPress(KeyCode.KEY_2);
                        break;
                    case '3':
                        SendKeyPress(KeyCode.KEY_3);
                        break;
                    case '4':
                        SendKeyPress(KeyCode.KEY_4);
                        break;
                    case '5':
                        SendKeyPress(KeyCode.KEY_5);
                        break;
                    case '6':
                        SendKeyPress(KeyCode.KEY_6);
                        break;
                    case '7':
                        SendKeyPress(KeyCode.KEY_7);
                        break;
                    case '8':
                        SendKeyPress(KeyCode.KEY_8);
                        break;
                    case '9':
                        SendKeyPress(KeyCode.KEY_9);
                        break;
                   
                    default:
                        break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
        }

        /// <summary>
        /// Nhấn đè phím
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT();
            input.Data.Keyboard.Vk = (ushort)keyCode;
            input.Data.Keyboard.Scan = 0;
            input.Data.Keyboard.Flags = 0;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            INPUT[] inputs = new INPUT[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Giải phím nhấn
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT();
            input.Data.Keyboard.Vk = (ushort)keyCode;
            input.Data.Keyboard.Scan = 0;
            input.Data.Keyboard.Flags = 2;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            INPUT[] inputs = new INPUT[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();

        }
        #endregion

        #region Giả lập chuột thật
        /// <summary>
        /// Click chiếm chuột. phải di chuyển chuột đến tọa độ rồi mới click
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mouseKey"></param>
        public static void MouseClick(int x, int y, EMouseKey mouseKey = EMouseKey.LEFT)
        {
            Cursor.Position = new System.Drawing.Point(x,y);

            Click(mouseKey);
        }        

        /// <summary>
        /// Drag mouse theo độ lệch trục X
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="deltaX"></param>
        /// <param name="isNegative"></param>
        public static void MouseDragX(Point startPoint, int deltaX, bool isNegative = false)
        {
            Cursor.Position = startPoint;
            mouse_event((uint)(MouseEventFlags.LEFTDOWN), 0, 0, 0, UIntPtr.Zero);

            for (int i = 0; i < deltaX; i++)
            {
                if (!isNegative)
                {
                    mouse_event((uint)(MouseEventFlags.MOVE), 1, 0, 0, UIntPtr.Zero);
                }
                else
                {
                    mouse_event((uint)(MouseEventFlags.MOVE), -1, 0, 0, UIntPtr.Zero);
                }
            }

            mouse_event((uint)(MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Drag mouse theo độ lệch trục y
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="deltaY"></param>
        /// <param name="isNegative"></param>
        public static void MouseDragY(Point startPoint, int deltaY, bool isNegative = false)
        {
            Cursor.Position = startPoint;
            mouse_event((uint)(MouseEventFlags.LEFTDOWN), 0, 0, 0, UIntPtr.Zero);

            for (int i = 0; i < deltaY; i++)
            {
                if (!isNegative)
                {
                    mouse_event((uint)(MouseEventFlags.MOVE), 0, 1, 0, UIntPtr.Zero);
                }
                else
                {
                    mouse_event((uint)(MouseEventFlags.MOVE), 0, -1, 0, UIntPtr.Zero);
                }
                
            }

            mouse_event((uint)(MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        ///Drag chuột từ điểm này qua điểm khác
        ///Speed là tốc độ drag qua mỗi 1 dlta
        ///Sleep là nghỉ giữa mỗi điểm
        /// </summary>
        /// <param name="points"></param>
        /// <param name="speed"></param>
        /// <param name="sleep"></param>
        public static void MouseDrag(System.Drawing.Point[] points, double speed = 0.1, double sleep = 0.1)
        {
            if (points == null || points.Length == 0)
            {
                Console.WriteLine("Không có tọa độ drag");
                return;
            }

            var lastPosition = points.First();
            System.Windows.Forms.Cursor.Position = lastPosition;
            mouse_event((uint)(MouseEventFlags.LEFTDOWN), 0, 0, 0, UIntPtr.Zero);

            if (points.Length > 1)
            {
                for (int i = 1; i < points.Length; i++)
                {
                    while (points[i].X != System.Windows.Forms.Cursor.Position.X || System.Windows.Forms.Cursor.Position.Y != points[i].Y)
                    {
                        //var deltaX = points[i].X - lastPosition.X;
                        //var deltaY = points[i].Y - lastPosition.Y;
                        var deltaX = points[i].X == lastPosition.X ? 0 : points[i].X > lastPosition.X ? 1 : -1;
                        var deltaY = points[i].Y == lastPosition.Y ? 0 : points[i].Y > lastPosition.Y ? 1 : -1;

                        mouse_event((uint)(MouseEventFlags.MOVE), deltaX, deltaY, 0, UIntPtr.Zero);
                        Thread.Sleep(TimeSpan.FromSeconds(speed));
                        lastPosition = System.Windows.Forms.Cursor.Position;
                        Console.WriteLine($"{deltaX},{deltaY} - {points[i].X},{points[i].Y} - {lastPosition.X},{lastPosition.Y}");
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(sleep));
                }
            }

            mouse_event((uint)(MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
        }


        public static void MouseScroll(Point startPoint, int deltaY, bool isNegative = false)
        {
            Cursor.Position = startPoint;

            //for (int i = 0; i < deltaY; i++)
            //{
            //    if (!isNegative)
            //    {
            //        mouse_event((uint)(MouseEventFlags.WHEEL), 0, 1, 0, UIntPtr.Zero);
            //    }
            //    else
            //    {
            //        mouse_event((uint)(MouseEventFlags.WHEEL), 0, -1, 0, UIntPtr.Zero);
            //    }
            //}

            mouse_event((uint)(MouseEventFlags.WHEEL), 0, 0, deltaY, UIntPtr.Zero);
        }

        /// <summary>
        /// Click chiếm chuột. phải di chuyển chuột đến tọa độ rồi mới click
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="point"></param>
        /// <param name="mouseKey"></param>
        public static void MouseClick(Point point, EMouseKey mouseKey = EMouseKey.LEFT)
        {
            Cursor.Position = point;
            
            Click(mouseKey);
        }

        /// <summary>
        /// Thực hiện thao tác click tại vị trí hiện tại của con trỏ
        /// </summary>
        /// <param name="mouseKey"></param>
        public static void Click(EMouseKey mouseKey = EMouseKey.LEFT)
        {
            //InputSimulator simu = new InputSimulator();
            switch (mouseKey)
            {
                case EMouseKey.LEFT:
                    mouse_event((uint)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);

                    //simu.Mouse.LeftButtonClick();
                    break;
                case EMouseKey.RIGHT:
                    mouse_event((uint)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);

                    //simu.Mouse.RightButtonClick();
                    break;
                case EMouseKey.DOUBLE_LEFT:
                    mouse_event((uint)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
                    mouse_event((uint)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);

                    //simu.Mouse.LeftButtonDoubleClick();
                    break;
                case EMouseKey.DOUBLE_RIGHT:
                    mouse_event((uint)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);
                    mouse_event((uint)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP | MouseEventFlags.ABSOLUTE), 0, 0, 0, UIntPtr.Zero);

                    //simu.Mouse.RightButtonDoubleClick();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Lấy thông tin của handle window
        /// <summary>
        /// Lấy ra khung chữ nhật của sửa sổ truyền vào. LEFT-TOP    RIGHT-BOTTOM
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rct = new RECT();
            GetWindowRect(hWnd, ref rct);

            return rct;
        }

        /// <summary>
        /// Tính toán ra tọa độ của một điểm trong app thành tọa độ so với toàn màn hình
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point GetGlobalPoint(IntPtr hWnd, Point? point = null)
        {
            Point res = new Point();
            var windowRect = GetWindowRect(hWnd);

            if (point == null)
                point = new Point();

            res.X = point.Value.X + windowRect.Left;
            res.Y = point.Value.Y + windowRect.Top;

            return res;
        }

        /// <summary>
        /// Tính toán ra tọa độ của một điểm trong app thành tọa độ so với toàn màn hình
        /// Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Point GetGlobalPoint(IntPtr hWnd, int x = 0, int y = 0)
        {
            Point res = new Point();
            var windowRect = GetWindowRect(hWnd);

            res.X = x + windowRect.Left;
            res.Y = y + windowRect.Top;

            return res;
        }

        /// <summary>
        /// Lấy ra text mà handle đó đang có
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetText(IntPtr hWnd)
        {
            StringBuilder formDetails = new StringBuilder(256);
            GetWindowText(hWnd, formDetails, 256);
            return formDetails.ToString().Trim();
        }

        /// <summary>
        /// Lấy ra class name của handle
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder formDetails = new StringBuilder(256);
            GetClassName(hWnd, formDetails, 256);
            return formDetails.ToString().Trim();
        }

        /// <summary>
        /// MakeLParam Macro
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        /// <summary>
        /// MakeLParam Macro from x,y
        ///  Tìm hiểu thêm tại https://www.howkteam.com/
        /// </summary>
        public static IntPtr MakeLParamFromXY(int x, int y)
        {
            return (IntPtr)((y << 16) | x);
        }
        #endregion
    }

    public class FindWindow
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static List<IntPtr> GetWindowHandles(string processName, string className)
        {
            List<IntPtr> handleList = new List<IntPtr>();
            Process[] processes = Process.GetProcessesByName(processName);
            Process proc = null;

            // Cycle through all top-level windows
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                // Get PID of current window
                GetWindowThreadProcessId(hWnd, out int processId);

                // Get process matching PID
                proc = processes.FirstOrDefault(p => p.Id == processId);

                if (proc != null)
                {
                    // Get class name of current window
                    StringBuilder classNameBuilder = new StringBuilder(256);
                    GetClassName(hWnd, classNameBuilder, 256);

                    // Check if class name matches what we're looking for
                    if (classNameBuilder.ToString() == className)
                    {
                        //Console.WriteLine($"{proc.ProcessName} process found with ID {proc.Id}, handle {hWnd.ToString("X")}");
                        handleList.Add(hWnd);
                    }
                }

                // return true so that we iterate through all windows
                return true;
            }, IntPtr.Zero);

            return handleList;
        }
    }
}
