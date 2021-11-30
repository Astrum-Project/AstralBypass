using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: MelonInfo(typeof(Astrum.AstralBypass), nameof(Astrum.AstralBypass), "0.1.0", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralBypass))]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    public class AstralBypass : MelonPlugin
    {
        private const string pluginsPath = "Plugins/AstralBypass.dll";
        private const string modsPath = "Mods/AstralBypass.dll";
        private const int renameExitStatus = 69_8153697;  // random number, but the 69 should indicate that it's human written;

        private static IntPtr pIC = IntPtr.Zero;

        static AstralBypass()
        {
            CheckLocation();

            FindIC();
        }

        private static void CheckLocation()
        {
            if (File.Exists(pluginsPath)) return;

            if (File.Exists(modsPath)) MoveToPlugins(modsPath);

            // since people love renaming their dlls for some reason
            string plugin = Directory.EnumerateFiles("Plugins")
                .Where(x => x.ToLower().EndsWith(".dll"))
                .Where(x => new FileInfo(x).Length < 100_000) // 100kb
                .FirstOrDefault(x => File.ReadAllText(x).Contains("AstralBypass.pdb"));

            if (plugin != null) MoveToPlugins(plugin);

            string mod = Directory.EnumerateFiles("Mods")
                .Where(x => x.ToLower().EndsWith(".dll"))
                .Where(x => new FileInfo(x).Length < 100_000)
                .FirstOrDefault(x => File.ReadAllText(x).Contains("AstralBypass.pdb"));

            if (mod != null) MoveToPlugins(mod);

            MessageBox.Show("Failed to locate AstralBypass.\nPlease place \"AstralBypass.dll\" into your plugins folder.\nThe game has been halted for you to do this.", "Problems loading AstralBypass");
            Environment.Exit(renameExitStatus);
        }

        private static void MoveToPlugins(string orig)
        {
            Console.Beep();
            Console.Beep();
            Console.Beep();
            MessageBox.Show("AstralBypass was in the wrong folder or incorrectly named.\nIt will now be relocated to your plugins folder", "Restarting game...");

            if (File.Exists(pluginsPath))
                File.Delete(pluginsPath);
            File.Move(orig, pluginsPath);
            
            Environment.Exit(renameExitStatus);
        }

        private static void FindIC()
        {
            // TODO: add other ML versions to this
            pIC = AstralCore.Utils.PatternScanner.Scan(
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

            if (pIC == IntPtr.Zero) AstralCore.Logger.Error("Failed to find Integrity Check");
            else BypassIC();
        }

        public static void BypassIC() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] { 0x90, 0x90 });
        public static void RepairIC() => AstralCore.Utils.MemoryUtils.WriteBytes(pIC, new byte[2] { 0xFF, 0xD3 });
    }
}
