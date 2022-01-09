using MelonLoader;
using System;
using System.Collections.Generic;

namespace Astrum
{
    partial class AstralBypass
    {
        public static class IntegrityChecks
        {
            private static readonly IntPtr pIC;

            static IntegrityChecks()
            {
                if (!versionSigs.TryGetValue((string)typeof(BuildInfo).GetField(nameof(BuildInfo.Version)).GetValue(null), out (string, int) data))
                {
                    AstralCore.Logger.Warn("[AstralBypass] Missing signature for your version of MelonLoader");
                    return;
                }

                pIC = AstralCore.Utils.PatternScanner.Scan(
                    "bootstrap.dll",
                    data.Item1,
                    data.Item2
                );

                AstralCore.Logger.Debug("Integrity Check = 0x" + pIC.ToInt64().ToString("X"));
            }

            private static readonly Dictionary<string, (string, int)> versionSigs = new Dictionary<string, (string, int)>()
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
                    "FF D3", //                 call rbx
                    55
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
                    44
                ),
                ["0.5.2"] = (
                    "FF 15 8A 51 02 00" + //    call qword ptr ds:[<&mono_image_close>]
                    "48 8B 1D ?? ?? ?? ??" + // mov rbx,qword ptr ds:[<&mono_raise_exception>]
                    "48 8B 05 ?? ?? ?? ??" + // mov rax,qword ptr ds:[<&mono_get_exception_bad_image_format>]
                    "48 8D 0D ?? ?? ?? ??" + // lea rcx,qword ptr ds:[7FFC39894230]
                    "FF D0" + //                call rax
                    "48 8B C8" + //             mov rcx,rax
                    "FF D3" + //                call rbx <INJECTING HERE>
                    "48 8B 05 ?? ?? ?? ??" + // mov rax,qword ptr ds:[7FFC3989DD48]
                    "48 8B 4C 24 58" + //       mov rcx,qword ptr ss:[rsp+58]
                    "48 89 4C 24 30" + //       mov qword ptr ss:[rsp+30],rcx
                    "48 8B 4C 24 60" + //       mov rcx,qword ptr ss:[rsp+60]
                    "48 89 4C 24 28", //        mov qword ptr ss:[rsp+28],rcx
                    32
                )
            };

            public static void Bypass()
            {
                if (pIC == IntPtr.Zero) return;
                AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] {0x66, 0x90});
            }

            public static void Repair()
            {
                if (pIC == IntPtr.Zero) return;
                AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] {0xFF, 0xD3});
            }
        }
    }
}
