using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KAutoHelper
{
    public class MemoryHelper
    {
        #region Memory API
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, UIntPtr nSize, uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory( IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, FreeType dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 WaitForSingleObject(IntPtr hProcess, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32.dll")]
        internal static extern Int32 CloseHandle(IntPtr hProcess);
        [DllImport("kernel32", SetLastError = true)]
        public static extern int GetProcessId(IntPtr hProcess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);


        const UInt32 INFINITE = 0xFFFFFFFF;
        const UInt32 WAIT_ABANDONED = 0x00000080;
        const UInt32 WAIT_OBJECT_0 = 0x00000000;
        const UInt32 WAIT_TIMEOUT = 0x00000102;
        #endregion


        public static IntPtr OpenProcess(int pId, ProcessAccessFlags ProcessAccess = ProcessAccessFlags.All)
        {
             return OpenProcess((uint)ProcessAccess, false, (uint)pId);
        }
        public static IntPtr OpenProcess(uint pId, ProcessAccessFlags ProcessAccess = ProcessAccessFlags.All)
        {
            return OpenProcess((uint)ProcessAccess, false, pId);
        }

        public static int AllocateMemory(IntPtr ProcessHandle, int memorySize)
        {
            return (int)VirtualAllocEx(ProcessHandle, (IntPtr)0, (IntPtr)memorySize, 0x1000, 0x40);
        }
        public static IntPtr CreateRemoteThread(IntPtr ProcessHandle, int address)
        {
            return CreateRemoteThread(ProcessHandle, (IntPtr)0, (IntPtr)0, (IntPtr)address, (IntPtr)0, 0, (IntPtr)0);
        }
        public static void WaitForSingleObject(IntPtr ProcessHandle, IntPtr threadHandle)
        {
            if (WaitForSingleObject(threadHandle, INFINITE) != WAIT_OBJECT_0)
            {
                Console.WriteLine("Failed waiting for single object");
            }
        }
        public static void FreeMemory(IntPtr ProcessHandle, int address)
        {
            bool result;
            result = VirtualFreeEx(ProcessHandle, (IntPtr)address, (IntPtr)0, FreeType.Release);
        }
        public static void CloseProcess(IntPtr ProcessHandle, IntPtr handle)
        {
            Int32 result = CloseHandle(handle);
        }

        #region write memory
        public static bool WriteInt(IntPtr Handle, IntPtr pointer, uint offset, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteFloat(IntPtr Handle, IntPtr pointer, uint offset, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteUInt(IntPtr Handle, IntPtr pointer, uint offset, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteString(IntPtr Handle, IntPtr pointer, uint offset, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteStruct(IntPtr Handle, IntPtr pointer, uint offset, object value)
        {
            byte[] bytes = RawSerialize(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteBytes(IntPtr Handle, IntPtr pointer, uint offset, byte[] bytes)
        {
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteByte(IntPtr Handle, IntPtr pointer, uint offset, byte value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }

        public static bool WriteUnicode(IntPtr Handle, IntPtr pointer, uint offset, string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            IntPtr nBytesRead = IntPtr.Zero;
            success = WriteProcessMemory(Handle, (IntPtr)address, bytes, bytes.Length, out nBytesRead);
            return success;
        }
        #endregion

        #region read memory   
        public static int ReadInt(IntPtr Handle, IntPtr pointer, uint offset)
        {
            byte[] bytes = new byte[24];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            
            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)4, 0);
            if (success)
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            else
            {
                return 0;
            }
        }

        public static uint ReadUInt(IntPtr Handle, IntPtr pointer, uint offset)
        {
            byte[] bytes = new byte[24];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;

            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)4, 0);
            if (success)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }
            else
            {
                return 0;
            }
        }

        public static float ReadFloat(IntPtr Handle, IntPtr pointer, uint offset)
        {
            byte[] bytes = new byte[24];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;

            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)4, 0);
            if (success)
            {
                return BitConverter.ToSingle(bytes, 0);
            }
            else
            {
                return 0;
            }
        }

        public static string ReadString(IntPtr Handle, IntPtr pointer, uint offset)
        {
            byte[] bytes = new byte[24];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;

            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)Marshal.SizeOf("".GetType()), 0);
            if (success)
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// đọc ra unicode string
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="pointer"></param>
        /// <param name="offset"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string ReadUnicode(IntPtr Handle, IntPtr pointer, uint offset, uint maxSize)
        {
            byte[] bytes = new byte[maxSize];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)maxSize, 0);

            if(success)
                return ByteArrayToString(bytes, EncodingType.Unicode);
            else
                return null;
        }

        /// <summary>
        /// Đọc ra mảng bytes
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="pointer"></param>
        /// <param name="offset"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(IntPtr Handle, IntPtr pointer, uint offset, uint maxSize)
        {
            byte[] bytes = new byte[maxSize];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)maxSize, 0);

            if (success)
                return bytes;
            else
                return null;
        }

        /// <summary>
        /// đọc ra một class theo cấu trúc
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="pointer"></param>
        /// <param name="offset"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static object ReadStruct<T>(IntPtr Handle, IntPtr pointer, uint offset)
        {
            int rawsize = Marshal.SizeOf(default(T));
            byte[] bytes = new byte[rawsize];
            bool success = false;

            uint address = (uint)ReadPointer(Handle, pointer) + offset;
            success = ReadProcessMemory(Handle, (IntPtr)address, bytes, (UIntPtr)rawsize, 0);

            if (success)
                return RawDeserialize<T>(bytes, 0);
            else
                return null;
        }

        /// <summary>
        /// Đọc giá trị từ pointer ra kiểu Int32
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public static int ReadPointer(IntPtr Handle, IntPtr pointer)
        {
            byte[] bytes = new byte[24];
            ReadProcessMemory(Handle, pointer, bytes, (UIntPtr)sizeof(int), 0);
            return BitConverter.ToInt32(bytes, 0);
        }
        #endregion

        private static object RawDeserialize<T>(byte[] rawData, int position)
        {
            int rawsize = Marshal.SizeOf(default(T));
            if (rawsize > rawData.Length)
                return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        private static byte[] RawSerialize(object anything)
        {
            int rawSize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawDatas = new byte[rawSize];
            Marshal.Copy(buffer, rawDatas, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            return ByteArrayToString(bytes, EncodingType.Unicode);
        }

        private static string ByteArrayToString(byte[] bytes, EncodingType encodingType)
        {
            System.Text.Encoding encoding = null;
            string result = "";
            switch (encodingType)

            {
                case EncodingType.ASCII:
                    encoding = new System.Text.ASCIIEncoding();
                    break;
                case EncodingType.Unicode:
                    encoding = new System.Text.UnicodeEncoding();
                    break;
                case EncodingType.UTF7:
                    encoding = new System.Text.UTF7Encoding();
                    break;
                case EncodingType.UTF8:
                    encoding = new System.Text.UTF8Encoding();
                    break;
            }

            for (int i = 0; i < bytes.Length; i += 2)
            {
                if (bytes[i] == 0 && bytes[i + 1] == 0)
                {
                    result = encoding.GetString(bytes, 0, i);
                    break;
                }
            }

            return result;
        }
        private enum EncodingType
        {
            ASCII,
            Unicode,
            UTF7,
            UTF8
        }
        [Flags]
        internal enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }
    }    
}
