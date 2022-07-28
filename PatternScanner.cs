using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Astrum
{
    // writen by dom1n1k and Patrick
    // converted to c# by atom0s [aka Wiccaan]
    // adapted by Astral Astrum
    // changes:
    //   works on 64 bit
    //   static instead of instance
    // todo:
    //  cache result
    public static class PatternScanner
    {
        private static bool MaskCheck(byte[] mem, int nOffset, string pattern)
        {
            for (int x = 0; x < pattern.Length / 2; x++)
            {
                string bite = pattern.Substring(x * 2, 2);
                if (bite == "??") continue;
                if (byte.Parse(bite, System.Globalization.NumberStyles.HexNumber) != mem[nOffset + x]) return false;
            }

            return true;
        }

        public static IntPtr Scan(string module, string pattern, int offset = 0)
        {
            Process process = Process.GetCurrentProcess();
            IntPtr baseAddr = GetModuleHandle(module);

            if (!GetModuleInformation(process.Handle, baseAddr, out MODULEINFO info, 24))
                return IntPtr.Zero;

            int size = (int)info.SizeOfImage;

            pattern = pattern.Replace(" ", "");

            try
            {
                if (baseAddr == IntPtr.Zero) return IntPtr.Zero;
                if (size == 0) return IntPtr.Zero;

                byte[] mem = new byte[size];

                if (!ReadProcessMemory(process.Handle, baseAddr, mem, size, out int nBytesRead) || nBytesRead != size)
                    return IntPtr.Zero;

                for (int x = 0; x < mem.Length; x++)
                    if (MaskCheck(mem, x, pattern))
                        return new IntPtr(baseAddr.ToInt64() + x + offset);

                return IntPtr.Zero;
            }
            catch { return IntPtr.Zero; }
        }

        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("psapi.dll", SetLastError = true)] static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, uint cb);
        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEINFO
        {
            public IntPtr lpBaseOfDll;
            public uint SizeOfImage;
            public IntPtr EntryPoint;
        }
    }
}
