using System;

namespace Astrum
{
    partial class AstralBypass
    {
        public static class IntegrityChecks
        {
            private static IntPtr pIC = AstralCore.Utils.PatternScanner.Scan(
                "bootstrap.dll",
                "66 0F 2F C7" + //             comisd xmm0, xmm7
                "77 1E" + //                   ja bootstrap.7FFF9D5688D6
                "66 0F 2F ?? ?? ?? ?? ??" + // comisd xmm7,qword ptr ds:[7FFF9D583EC8]
                "77 14" + //                   ja bootstrap.7FFF9D5688D6
                "48 8D 4D B0" + //             lea rcx,qword ptr ss:[rbp-50]
                "E8 ?? ?? ?? ??" + //          call bootstrap.7FFF9D567AF0
                "49 8B CF" + //                mov rcx,r15
                "FF 15 ?? ?? ?? ??" + //       call qword ptr ds:[< &mono_image_close >]
                "EB 2E" + //                   jmp bootstrap.7FFF9D568904
                "48 8D 4D B0" + //             lea rcx,qword ptr ss:[rbp-50]
                "E8 ?? ?? ?? ??" + //          call bootstrap.7FFF9D567AF0
                "49 8B CF" + //                mov rcx,r15
                "FF 15 ?? ?? ?? ??" + //       call qword ptr ds:[< &mono_image_close >]
                "48 8B ?? ?? ?? ?? ??" + //    mov rbx, qword ptr ds:[< &mono_raise_exception >]
                "48 8B ?? ?? ?? ?? ??" + //    mov rax, qword ptr ds:[< &mono_get_exception_bad_image_format >]
                "48 8D ?? ?? ?? ?? ??" + //    lea rcx, qword ptr ds:[7FFF9D5820B8]
                "FF D0" + //                   call rax
                "48 8B C8" + //                mov rcx, rax
                "FF D3", //                    call rbx ; <== PATCHING THIS
                80
            );

            public static void Bypass() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] { 0x90, 0x90 });
            public static void Repair() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] { 0xFF, 0xD3 });
        }
    }
}
