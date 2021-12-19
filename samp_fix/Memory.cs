using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MemoryEdit
{
    class Memory
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Boolean bInheritHandle,
        UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, UIntPtr nSize, uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, UIntPtr nSize, uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
        UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public enum Protection : uint
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

        //Create handle
        IntPtr Handle;

        public static bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name)
                {
                    return true;
                }
            }
            return false;
        }

        //constructor
        public Memory(string sprocess, uint access)
        {
            //Get the specific process
            Process[] Processes = Process.GetProcessesByName(sprocess);
            Process nProcess = Processes[0];
            //access to the process
            //0x10 - read
            //0x20 - write
            //0x001F0FFF - all
            Handle = OpenProcess(access, false, (uint)nProcess.Id);
        }

        //Memory reading

        //Byte
        public int ReadBytePointer(uint pointer, uint offset, int blen)
        {
            byte[] bytes = new byte[24];

            //Creating the address (reading the Base and add the offset)
            uint adress = (uint)Read(pointer) + offset;
            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)adress, bytes, (UIntPtr)blen, 0);
            //Return the result as 4 byte int
            return BitConverter.ToInt32(bytes, 0);

        }

        public int ReadByte(uint pointer, int blen)
        {
            byte[] bytes = new byte[24];

            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)pointer, bytes, (UIntPtr)blen, 0);
            //Return the result as 4 byte int
            return BitConverter.ToInt32(bytes, 0);
        }

        //Float
        public float ReadFloatPointer(uint pointer, uint offset)
        {
            byte[] bytes = new byte[24];

            //Creating the address (reading the Base and add the offset)
            uint adress = (uint)Read(pointer) + offset;
            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)adress, bytes, (UIntPtr)sizeof(float), 0);
            //Return the result as 4 byte int
            return BitConverter.ToSingle(bytes, 0);

        }

        public float ReadFloat(uint pointer)
        {
            byte[] bytes = new byte[24];

            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)pointer, bytes, (UIntPtr)sizeof(float), 0);
            //Return the result as 4 byte int
            return BitConverter.ToSingle(bytes, 0);
        }

        //Double
        public double ReadDoublePointer(uint pointer, uint offset)
        {
            byte[] bytes = new byte[24];

            //Creating the address (reading the Base and add the offset)
            uint adress = (uint)Read(pointer) + offset;
            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)adress, bytes, (UIntPtr)sizeof(double), 0);
            //Return the result as 4 byte int
            return BitConverter.ToDouble(bytes, 0);

        }

        public double ReadDouble(uint pointer)
        {
            byte[] bytes = new byte[24];

            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)pointer, bytes, (UIntPtr)sizeof(double), 0);
            //Return the result as 4 byte int
            return BitConverter.ToDouble(bytes, 0);
        }

        //String
        public string ReadStringPointer(uint pointer, uint offset, int blen)
        {
            byte[] bytes = new byte[24];

            //Creating the address (reading the Base and add the offset)
            uint adress = (uint)Read(pointer) + offset;
            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)adress, bytes, (UIntPtr)blen, 0);
            //Return the result as 4 byte int
            return BitConverter.ToString(bytes, 0);

        }

        public string ReadString(uint pointer, int blen)
        {
            byte[] bytes = new byte[24];

            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)pointer, bytes, (UIntPtr)blen, 0);
            //Return the result as 4 byte int
            return BitConverter.ToString(bytes, 0);
        }

        //Used for pointers
        public int Read(uint pointer)
        {
            byte[] bytes = new byte[24];

            //Reading the specific address within the process
            ReadProcessMemory(Handle, (IntPtr)pointer, bytes, (UIntPtr)sizeof(int), 0);
            //Return the result as 4 byte int
            return BitConverter.ToInt32(bytes, 0);
        }

        //Memory writing

        //Byte
        public void WriteBytePointer(uint pointer, uint offset, byte[] Buffer, int blen)
        {
            uint adress = (uint)Read(pointer) + offset;
            WriteProcessMemory(Handle, (IntPtr)adress, Buffer, (UIntPtr)blen, 0);
        }

        public void WriteByte(uint pointer, byte[] Buffer, int blen)
        {
            WriteProcessMemory(Handle, (IntPtr)pointer, Buffer, (UIntPtr)blen, 0);
        }

        //Double
        public void WriteDoublePointer(uint pointer, uint offset, byte[] Buffer, int blen)
        {
            uint adress = (uint)Read(pointer) + offset;
            WriteProcessMemory(Handle, (IntPtr)adress, Buffer, (UIntPtr)sizeof(double), 0);
        }

        public void WriteDouble(uint pointer, byte[] Buffer, int blen)
        {
            WriteProcessMemory(Handle, (IntPtr)pointer, Buffer, (UIntPtr)sizeof(double), 0);
        }

        //Float
        public void WriteFloatPointer(uint pointer, uint offset, byte[] Buffer)
        {
            uint adress = (uint)Read(pointer) + offset;
            WriteProcessMemory(Handle, (IntPtr)adress, Buffer, (UIntPtr)sizeof(float), 0);
        }

        public void WriteFloat(uint pointer, byte[] Buffer)
        {
            WriteProcessMemory(Handle, (IntPtr)pointer, Buffer, (UIntPtr)sizeof(float), 0);
        }

        //String
        public void WriteStringPointer(uint pointer, uint offset, byte[] Buffer, int blen)
        {
            uint adress = (uint)Read(pointer) + offset;
            WriteProcessMemory(Handle, (IntPtr)adress, Buffer, (UIntPtr)blen, 0);
        }

        public void WriteString(uint pointer, byte[] Buffer, int blen)
        {
            WriteProcessMemory(Handle, (IntPtr)pointer, Buffer, (UIntPtr)blen, 0);
        }

        bool WriteProtectedMemory(IntPtr hProcess, IntPtr dwAddress, UIntPtr dwSize, uint flNewProtect, uint lpflOldProtect)
        {
            if (!VirtualProtectEx(hProcess, dwAddress, dwSize, flNewProtect, out lpflOldProtect))
                return true;

            return false;
        }

        public bool SetProtection(uint dwAddress, int dwSize, Protection flNewProtect)
        {
            return WriteProtectedMemory(Handle, (IntPtr)dwAddress, (UIntPtr)dwSize, (uint)flNewProtect, 0);
        }
    }
}