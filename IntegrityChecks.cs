using MelonLoader;
using System;
using System.Collections.Generic;

namespace Astrum
{
    partial class AstralBypass
    {
        public static class IntegrityChecks
        {
            private static IntPtr pIC;
            private static readonly byte[] oBytes;
            private static readonly byte[] tBytes;

            static IntegrityChecks()
            {
                if (!versionSigs.TryGetValue((string)typeof(BuildInfo).GetField(nameof(BuildInfo.Version)).GetValue(null), out (string, int, byte[], byte[]) data)) return;

                oBytes = data.Item3;
                tBytes = data.Item4;

                pIC = AstralCore.Utils.PatternScanner.Scan(
                    "bootstrap.dll",
                    data.Item1,
                    data.Item2
                );

                AstralCore.Logger.Debug("Integrity Check = 0x" + pIC.ToInt64().ToString("X"));
            }

            private static readonly Dictionary<string, (string, int, byte[], byte[])> versionSigs = new Dictionary<string, (string, int, byte[], byte[])>()
            {
                ["0.4.3"] = (
                    "49 8B CF" + //             mov rcx,r15
                    "FF 15 ?? ?? ?? ??" + //    call qword ptr ds:[< &mono_image_close >]
                    "EB 2E" + //                jmp bootstrap.7FFF9D568904
                    "48 8D 4D ??" + //          lea rcx,qword ptr ss:[rbp-50]
                    "E8 ?? ?? ?? ??" + //       call bootstrap.7FFF9D567AF0
                    "49 8B ??" + //             mov rcx,r15
                    "FF 15 ?? ?? ?? ??" + //    call qword ptr ds:[< &mono_image_close >]
                    "48 8B ?? ?? ?? ?? ??" + // mov rbx, qword ptr ds:[< &mono_raise_exception >]
                    "48 8B ?? ?? ?? ?? ??" + // mov rax, qword ptr ds:[< &mono_get_exception_bad_image_format >]
                    "48 8D ?? ?? ?? ?? ??" + // lea rcx, qword ptr ds:[7FFF9D5820B8]
                    "FF D0" + //                call rax
                    "48 8B C8" + //             mov rcx, rax
                    "FF D3", //  
                    55,
                    new byte[2] { 0xFF, 0xD3 },
                    new byte[2] { 0x90, 0x90 }
                ),
                ["0.5.1"] = (
                    "48 8D 4D ??" + //          lea rcx, qword ptr ss:[rbp-78]             
                    "E8 ?? ?? ?? ??" + //       call bootstrap.7FFA228B8FF0    
                    "49 8B ??" + //             mov rcx, r14           
                    "FF 15 ?? ?? ?? ??" + //    call qword ptr ds:[<&mono_image_close>]         
                    "48 8B ?? ?? ?? ?? ??" + // mov rbx, qword ptr ds:[<&mono_raise_exception>]   
                    "48 8B ?? ?? ?? ?? ??" + // mov rax, qword ptr ds:[<&mono_get_exception_bad_image_format>] 
                    "48 8D ?? ?? ?? ?? ??" + // lea rcx, qword ptr ds:[7FFA228D4230]      
                    "FF D0" + //                call rax     
                    "48 8B C8" + //             mov rcx, rax     
                    "FF D3", //                 call rbx  
                    45,
                    new byte[2] { 0xFF, 0xD3 },
                    new byte[2] { 0x90, 0x90 }
                )
            };

            public static void Bypass() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, tBytes);
            public static void Repair() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, oBytes);
        }
    }
}
