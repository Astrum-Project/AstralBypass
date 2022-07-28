using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Astrum
{
    public static class MemoryUtils
    {
        const uint PAGE_EXECUTE_READWRITE = 0x40;

        public static void WriteBytes(IntPtr address, byte[] bytes)
        {
            IntPtr process = Process.GetCurrentProcess().Handle;
            VirtualProtectEx(process, address, (UIntPtr)bytes.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect);
            Marshal.Copy(bytes, 0, address, bytes.Length);
            VirtualProtectEx(process, address, (UIntPtr)bytes.Length, oldProtect, out _);
        }

        [DllImport("kernel32.dll")] static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}
