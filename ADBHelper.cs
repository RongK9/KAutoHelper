using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KAutoHelper
{
    public class ADBHelper
    {
        #region commands query
        static string LIST_DEVICES = "adb devices";
        static string TAP_DEVICES = "adb -s {0} shell input tap {1} {2}";
        static string SWIPE_DEVICES = "adb -s {0} shell input swipe {1} {2} {3} {4} {5}";
        static string KEY_DEVICES = "adb -s {0} shell input keyevent {1}";
        static string INPUT_TEXT_DEVICES = "adb -s {0} shell input text \"{1}\"";
        static string CAPTURE_SCREEN_TO_DEVICES = "adb -s {0} shell screencap -p \"{1}\"";
        static string PULL_SCREEN_FROM_DEVICES = "adb -s {0} pull \"{1}\"";
        static string REMOVE_SCREEN_FROM_DEVICES = "adb -s {0} shell rm -f \"{1}\"";
        static string GET_SCREEN_RESOLUTION = "adb -s {0} shell dumpsys display | Find \"mCurrentDisplayRect\"";
        const int DEFAULT_SWIPE_DURATION = 100;

        #endregion


        static string ADB_FOLDER_PATH = "";
        static string ADB_PATH = "";
        public static string SetADBFolderPath(string folderPath)
        {
            ADB_FOLDER_PATH = folderPath;
            ADB_PATH = folderPath + "\\adb.exe";

            if (!File.Exists(ADB_PATH))
            {
                return "ADB Path not Exits!!!";
            }
            return "OK";
        }
        public void SetTextFromClipboard(string deviceID, string text)
        {
            var temp = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            int count = 0;
            foreach (var item in temp)
            {
                var res = ADBHelper.ExecuteCMDBat(deviceID, $"adb -s {deviceID} shell am broadcast -a clipper.set -e text \"\\\"" + item + "\\\"\"");
                ADBHelper.ExecuteCMD($"adb -s {deviceID} shell input keyevent 279");
                count++;
                if (count < temp.Length)
                {
                    ADBHelper.Key(deviceID, ADBKeyEvent.KEYCODE_ENTER);
                }
            }
        }
        void Note(string deviceID)
        {
            /*
            cài app cliper hỗ trợ copy páste
            adb install C:\images\clipper.apk

            */


            // stop zalo đang chạy
            ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am force-stop com.zing.zalo");

            // delete images
            var res = ADBHelper.ExecuteCMD($"adb -s {deviceID} shell rm -f /sdcard/Pictures/Images/*");

            // tạo folder
            res = ADBHelper.ExecuteCMD($"adb -s {deviceID} shell mkdir /sdcard/Pictures/Images");

            // push hình vào phone
            var imgDir = new DirectoryInfo(@"C:\images");
            var files = imgDir.GetFiles().Select(x => x.FullName);

            foreach (var item in files)
            {
                ADBHelper.ExecuteCMD($"adb -s {deviceID} push {item} sdcard/Pictures/Images");
            }
        }

        public static string ExecuteCMD(string cmdCommand)
        {
            try
            {
                Process cmd = new Process();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = ADB_FOLDER_PATH;
                startInfo.FileName = "cmd.exe";
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";

                cmd.StartInfo = startInfo;
                cmd.Start();

                cmd.StandardInput.WriteLine(cmdCommand);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();

                string result = cmd.StandardOutput.ReadToEnd();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public static string ExecuteCMDBat(string deviceID, string cmdCommand)
        {
            try
            {
                string batName = $"bat_{deviceID}.bat";
                File.WriteAllText(batName, cmdCommand);

                Process cmd = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = ADB_FOLDER_PATH;
                startInfo.FileName = batName;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";

                cmd.StartInfo = startInfo;
                cmd.Start();


                cmd.StandardInput.WriteLine(cmdCommand);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();

                string result = cmd.StandardOutput.ReadToEnd();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public static List<string> GetDevices()
        {
            List<string> ListDevices = new List<string>();
            string input = KAutoHelper.ADBHelper.ExecuteCMD("adb devices");

            string pattern = @"(?<=List of devices attached)([^\n]*\n+)+";

            MatchCollection matchCollection = Regex.Matches(input, pattern, RegexOptions.Singleline);

            if (matchCollection.Count > 0)
            {
                string AllDevices = matchCollection[0].Groups[0].Value;
                string[] lines = Regex.Split(AllDevices, "\r\n");

                foreach (var device in lines)
                {
                    if (!string.IsNullOrEmpty(device) && device != " ")
                    {
                        var temp = device.Trim().Split('\t');

                        string devices = temp[0];
                        string state = "";

                        try
                        {
                            state = temp[1];
                            if(state != "device")
                            {
                                continue;
                            }
                        }
                        catch { }
                        ListDevices.Add(devices.Trim());
                    }
                }
            }

            return ListDevices;
        }

        //public static List<string> GetDevices()
        //{
        //    List<string> devices = new List<string>();

        //    string result = ExecuteCMD(LIST_DEVICES);

        //    string path = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
        //    path = path.Replace("\\", "");
        //    string pattern = @"(?<=List of devices attached ).*?(?=" + path + ")";
        //    var mathces = Regex.Matches(result, pattern, RegexOptions.Singleline);

        //    if (mathces != null && mathces.Count > 0)
        //    {
        //        foreach (var item in mathces)
        //        {
        //            string temp = item.ToString();

        //            var temp2 = temp.Split(new string[] { "device" }, StringSplitOptions.None);

        //            for (int i = 0; i < temp2.Length - 1; i++)
        //            {
        //                var item2 = temp2[i];
        //                var temp3 = item2.Replace("\r", "").Replace("\n", "").Replace("\t", "");

        //                devices.Add(temp3);
        //            }
        //        }
        //    }

        //    return devices;
        //}

        public static string GetDeviceName(string deviceID)
        {
            string name = "";

            string cmd = "";
            var res = ExecuteCMD(cmd);

            return name;
        }

        public static void TapByPercent(string deviceID, double x, double y, int count = 1)
        {
            var resolution = GetScreenResolution(deviceID);

            int X = (int)(x *( resolution.X*1.0/100));
            int Y = (int)(y * (resolution.Y*1.0/100));

            if(X < 0 || Y < 0)
            {
                Console.WriteLine($"Out of resolutions {X},{Y}");
                return;
            }

            string cmdCommand = string.Format(TAP_DEVICES, deviceID, X, Y);

            for (int i = 1; i < count; i++)
            {
                cmdCommand += " && " + string.Format(TAP_DEVICES, deviceID, x, y);
            }

            string result = ExecuteCMD(cmdCommand);
        }

        public static void Tap(string deviceID, int x, int y, int count = 1)
        {
            string cmdCommand = string.Format(TAP_DEVICES, deviceID, x, y);

            for (int i = 1; i < count; i++)
            {
                cmdCommand += " && "+ string.Format(TAP_DEVICES, deviceID, x, y);
            }

            string result = ExecuteCMD(cmdCommand);
        }

        public static void Key(string deviceID, ADBKeyEvent key)
        {
            string cmdCommand = string.Format(KEY_DEVICES, deviceID, key);
            string result = ExecuteCMD(cmdCommand);
        }

        public static void InputText(string deviceID, string text)
        {
            string cmdCommand = string.Format(INPUT_TEXT_DEVICES, deviceID, text/*.Replace(" ", "%s")*/.Replace("&","\\&").Replace("<", "\\<").Replace(">", "\\>").Replace("?", "\\?").Replace(":", "\\:").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace("|", "\\|"));
            string result = ExecuteCMD(cmdCommand);
        }

        public static void SwipeByPercent(string deviceID, double x1, double y1, double x2, double y2, int duration = DEFAULT_SWIPE_DURATION)
        {
            var resolution = GetScreenResolution(deviceID);

            int X1 = (int)(x1 * (resolution.X * 1.0 / 100));
            int Y1 = (int)(y1 * (resolution.Y * 1.0 / 100));
            int X2 = (int)(x2 * (resolution.X * 1.0 / 100));
            int Y2 = (int)(y2 * (resolution.Y * 1.0 / 100));

            string cmdCommand = string.Format(SWIPE_DEVICES, deviceID, X1, Y1, X2, Y2, duration);
            string result = ExecuteCMD(cmdCommand);
        }

        public static void Swipe(string deviceID, int x1, int y1, int x2, int y2, int duration = DEFAULT_SWIPE_DURATION)
        {
            string cmdCommand = string.Format(SWIPE_DEVICES, deviceID, x1, y1, x2, y2, duration);
            string result = ExecuteCMD(cmdCommand);
        }

        public static void LongPress(string deviceID, int x, int y, int duration = DEFAULT_SWIPE_DURATION)
        {
            string cmdCommand = string.Format(SWIPE_DEVICES, deviceID, x, y, x, y, duration);
            string result = ExecuteCMD(cmdCommand);
        }

        public static System.Drawing.Point GetScreenResolution(string deviceID)
        {
            string cmdCommand = string.Format(GET_SCREEN_RESOLUTION, deviceID);
            string result = ExecuteCMD(cmdCommand);

            result = result.Substring(result.IndexOf("- "));
            result = result.Substring(result.IndexOf(' '), result.IndexOf(')') - result.IndexOf(' '));
            string[] temp = result.Split(',');

            int x =Convert.ToInt32(temp[0].Trim());
            int y = Convert.ToInt32(temp[1].Trim());

            return new System.Drawing.Point(x,y);
        }

        /// <summary>
        /// Mặc định capture thiết bị đầu tiên nếu k truyền vào deviceID
        /// return null nếu không có thiết bị hoặc chụp lỗi
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Bitmap ScreenShoot(string deviceID = null, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            bool flag = string.IsNullOrEmpty(deviceID);
            if (flag)
            {
                List<string> listDevice = KAutoHelper.ADBHelper.GetDevices();
                bool flag2 = listDevice != null && listDevice.Count > 0;
                if (!flag2)
                {
                    return null;
                }
                deviceID = listDevice.First<string>();
            }
            string screenShotCount = deviceID;
            try
            {
                screenShotCount = deviceID.Split(new char[]
                {
    ':'
                })[1];
            }
            catch
            {
            }
            string nameToSave = Path.GetFileNameWithoutExtension(fileName) + screenShotCount + Path.GetExtension(fileName);
            for (; ; )
            {
                bool flag3 = File.Exists(nameToSave);
                if (!flag3)
                {
                    break;
                }
                try
                {
                    File.Delete(nameToSave);
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            string Current = Directory.GetCurrentDirectory() + "\\" + nameToSave;
            string CurrentPath = Directory.GetCurrentDirectory().Replace(@"\\", @"\");
            CurrentPath = "\"" + CurrentPath + "\"";

            string cmdCommand1 = string.Format("adb -s {0} shell screencap -p \"{1}\"", deviceID, "/sdcard/" + nameToSave);
            string cmdCommand2 = string.Format("adb -s " + deviceID + " pull " + "/sdcard/" + nameToSave + " " + CurrentPath);
            string result1 = KAutoHelper.ADBHelper.ExecuteCMD(cmdCommand1);
            string result2 = KAutoHelper.ADBHelper.ExecuteCMD(cmdCommand2);

            Bitmap result = null;
            try
            {
                using (Bitmap bitmap = new Bitmap(Current))
                {
                    result = new Bitmap(bitmap);
                }
            }
            catch { }
            if (isDeleteImageAfterCapture)
            {
                try
                {
                    File.Delete(nameToSave);
                }
                catch
                {
                }
                try
                {
                    string cmdCommand3 = string.Format("adb -s " + deviceID + " shell \"rm " + "/sdcard/" + nameToSave + "\"");
                    string result3 = KAutoHelper.ADBHelper.ExecuteCMD(cmdCommand3);
                }
                catch { }
            }
            return result;
        }
        //public static System.Drawing.Bitmap ScreenShoot(string deviceID = null, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        //{
        //    if (string.IsNullOrEmpty(deviceID))
        //    {
        //        var listDevice = GetDevices();
        //        if (listDevice != null && listDevice.Count > 0)
        //        {
        //            deviceID = listDevice.First();
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }

        //    string screenShotCount = deviceID;
        //    try
        //    {
        //        screenShotCount = deviceID.Split(':')[1];
        //    }
        //    catch { }


        //    string nameToSave = Path.GetFileNameWithoutExtension(fileName) + screenShotCount + Path.GetExtension(fileName);
        //    while (true)
        //    {
        //        if (File.Exists(nameToSave))
        //        {
        //            try
        //            {
        //                File.Delete(nameToSave);
        //                break;
        //            }
        //            catch
        //            {

        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }


        //    string cmdCommand = string.Format(CAPTURE_SCREEN_TO_DEVICES, deviceID, "/sdcard/" + nameToSave);

        //    cmdCommand += Environment.NewLine + string.Format(PULL_SCREEN_FROM_DEVICES, deviceID, "/sdcard/" + nameToSave);
        //    cmdCommand += Environment.NewLine + string.Format(REMOVE_SCREEN_FROM_DEVICES, deviceID, "/sdcard/" + nameToSave) + Environment.NewLine;
        //    string result = ExecuteCMD(cmdCommand);
        //    Bitmap cloneBMP;

        //    using (System.Drawing.Bitmap bmp = new Bitmap(nameToSave))
        //    {
        //        cloneBMP = new Bitmap(bmp);
        //    }

        //    if (isDeleteImageAfterCapture)
        //    {
        //        try
        //        {
        //            File.Delete(nameToSave);
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    return cloneBMP;
        //}



        public static void ConnectNox(int count = 1)
        {
            //ADBHelper.ExecuteCMD("adb shell su -c \"/dir > /dev/null 2> /dev/null < /dev/null &\"");

            string connectNoxCMD = "";
            int noxPort = 62000;

            if (count <= 1)
            {
                connectNoxCMD += "adb connect 127.0.0.1:" + (noxPort + 1);
            }
            else
            {
                connectNoxCMD += "adb connect 127.0.0.1:" + (noxPort + 1);
                for (int i = 25; i < count + 24; i++)
                {
                    connectNoxCMD += Environment.NewLine + "adb connect 127.0.0.1:" + (noxPort + i);
                }
            }
           
           
            ADBHelper.ExecuteCMD(connectNoxCMD);
            //Delay(2000);
        }

        public static void PlanModeON(string deviceID, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            // //adb shell settings put global airplane_mode_on 1
            string cmdClearShoppe = "adb -s " + deviceID + " settings put global airplane_mode_on 1";
            cmdClearShoppe += Environment.NewLine + "adb -s " + deviceID + " am broadcast -a android.intent.action.AIRPLANE_MODE";
            ADBHelper.ExecuteCMD(cmdClearShoppe);
        }

        public static void PlanModeOFF(string deviceID, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            string cmdClearShoppe = "adb -s " + deviceID + " settings put global airplane_mode_on 0";
            cmdClearShoppe += Environment.NewLine + "adb -s " + deviceID + " am broadcast -a android.intent.action.AIRPLANE_MODE";
            ADBHelper.ExecuteCMD(cmdClearShoppe);
        }

        public static void Delay(double delayTime)
        {
            double count = 0;
            while (count < delayTime)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                count += 100;
            }
        }

        /// <summary>
        /// Tìm hình trong hình
        /// thử 3 lần liên tục
        /// Nếu không tìm thấy trả về null
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="ImagePath"></param>
        /// <param name="delayPerCheck"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static System.Drawing.Point? FindImage(string deviceID, string ImagePath, int delayPerCheck = 2000, int count = 5)
        {

            DirectoryInfo dir = new DirectoryInfo(ImagePath);
            var icons = dir.GetFiles();
            do
            {
                Bitmap screen = null;
                int countScreenShot = 3;
                do
                {
                    try
                    {
                        screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                        break;
                    }
                    catch (Exception ex)
                    {
                        //Utilities.WriteLogFile(ex.Message);
                        countScreenShot--;
                        Delay(1000);
                    }
                }
                while (countScreenShot > 0);

                if (screen == null)
                {
                    return null;
                }

                System.Drawing.Point? point = null;
                foreach (var item in icons)
                {
                    Bitmap icon = (Bitmap)Bitmap.FromFile(item.FullName);
                    point = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, icon);
                    if (point != null)
                        break;
                }

                if (point != null)
                {
                    return point;
                }
                else
                {
                    Delay(2000);
                }

                count--;
            }
            while (count > 0);

            return null;
        }

        /// <summary>
        /// Tìm và click vào hình theo tọa độ hình đã tìm được
        /// Nếu không tìm thấy thì thôi
        /// Thử 3 lần liên tục
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="ImagePath"></param>
        /// <param name="delayPerCheck"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool FindImageAndClick(string deviceID, string ImagePath, int delayPerCheck = 2000, int count = 5)
        {
            DirectoryInfo dir = new DirectoryInfo(ImagePath);
            var icons = dir.GetFiles();
            do
            {
                Bitmap screen = null;
                int countScreenShot = 3;
                do
                {
                    try
                    {
                        screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                        break;
                    }
                    catch (Exception ex)
                    {
                        //Utilities.WriteLogFile(ex.Message);
                        countScreenShot--;
                        Delay(1000);
                    }
                }
                while (countScreenShot > 0);

                if (screen == null)
                {
                    return false;
                }

                System.Drawing.Point? point = null;
                foreach (var item in icons)
                {
                    Bitmap icon = (Bitmap)Bitmap.FromFile(item.FullName);
                    point = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, icon);
                    if (point != null)
                        break;
                }

                if (point != null)
                {
                    KAutoHelper.ADBHelper.Tap(deviceID, point.Value.X, point.Value.Y);
                    return true;
                }
                else
                {
                    Delay(delayPerCheck);
                }

                count--;
            }
            while (count > 0);

            return false;
        }
    }


    public static class BitmapConversion
    {
        public static BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            if (source == null)
                return null;
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }
    }

    public enum ADBKeyEvent : int
    {
        KEYCODE_0 = 0,
        KEYCODE_SOFT_LEFT,
        KEYCODE_SOFT_RIGHT,
        KEYCODE_HOME,
        KEYCODE_BACK,
        KEYCODE_CALL,
        KEYCODE_ENDCALL,
        KEYCODE_0_,
        KEYCODE_1,
        KEYCODE_2,
        KEYCODE_3,
        KEYCODE_4,
        KEYCODE_5,
        KEYCODE_6,
        KEYCODE_7,
        KEYCODE_8,
        KEYCODE_9,
        KEYCODE_STAR,
        KEYCODE_POUND,
        KEYCODE_DPAD_UP,
        KEYCODE_DPAD_DOWN,
        KEYCODE_DPAD_LEFT,
        KEYCODE_DPAD_RIGHT,
        KEYCODE_DPAD_CENTER,
        KEYCODE_VOLUME_UP,
        KEYCODE_VOLUME_DOWN,
        KEYCODE_POWER,
        KEYCODE_CAMERA,
        KEYCODE_CLEAR,
        KEYCODE_A,
        KEYCODE_B,
        KEYCODE_C,
        KEYCODE_D,
        KEYCODE_E,
        KEYCODE_F,
        KEYCODE_G,
        KEYCODE_H,
        KEYCODE_I,
        KEYCODE_J,
        KEYCODE_K,
        KEYCODE_L,
        KEYCODE_M,
        KEYCODE_N,
        KEYCODE_O,
        KEYCODE_P,
        KEYCODE_Q,
        KEYCODE_R,
        KEYCODE_S,
        KEYCODE_T,
        KEYCODE_U,
        KEYCODE_V,
        KEYCODE_W,
        KEYCODE_X,
        KEYCODE_Y,
        KEYCODE_Z,
        KEYCODE_COMMA,
        KEYCODE_PERIOD,
        KEYCODE_ALT_LEFT,
        KEYCODE_ALT_RIGHT,
        KEYCODE_SHIFT_LEFT,
        KEYCODE_SHIFT_RIGHT,
        KEYCODE_TAB,
        KEYCODE_SPACE,
        KEYCODE_SYM,
        KEYCODE_EXPLORER,
        KEYCODE_ENVELOPE,
        KEYCODE_ENTER,
        KEYCODE_DEL,
        KEYCODE_GRAVE,
        KEYCODE_MINUS,
        KEYCODE_EQUALS,
        KEYCODE_LEFT_BRACKET,
        KEYCODE_RIGHT_BRACKET,
        KEYCODE_BACKSLASH,
        KEYCODE_SEMICOLON,
        KEYCODE_APOSTROPHE,
        KEYCODE_SLASH,
        KEYCODE_AT,
        KEYCODE_NUM,
        KEYCODE_HEADSETHOOK,
        KEYCODE_FOCUS,
        KEYCODE_PLUS,
        KEYCODE_MENU,
        KEYCODE_NOTIFICATION,
        KEYCODE_APP_SWITCH = 187
    }


    public class NoxMultiIni
    {
        public int pid { get; set; }
        public int vmpid { get; set; }
        private static List<Port> Ports { get; set; }

        public static List<NoxMultiIni> GetNoxMultiIni()
        {
            List<NoxMultiIni> multi = new List<NoxMultiIni>();

            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;

            var data = File.ReadAllLines(path + "\\Local\\Nox\\multi.ini");

            for (int i = 0; i < data.Length; i += 4)
            {
                NoxMultiIni nox = new NoxMultiIni();
                nox.pid = Convert.ToInt32(data[i + 1].Split('=')[1]);
                nox.vmpid = Convert.ToInt32(data[i + 2].Split('=')[1]);
                multi.Add(nox);
            }

            return multi;
        }

        public static string GetNoxTitleFromADBPort(string port)
        {
            Ports = ProcessHelper.GetNetStatPorts();
            var multiNox = NoxMultiIni.GetNoxMultiIni();
            var abc = Ports.Where(p => p.port_number == port).FirstOrDefault();
            var pid = multiNox.Where(p => p.vmpid == abc.pid).FirstOrDefault();
            var nox = Process.GetProcessById(pid.pid);
            return nox.MainWindowTitle;
        }
    }
}
