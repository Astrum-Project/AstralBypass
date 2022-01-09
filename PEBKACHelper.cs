using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Astrum
{
    partial class AstralBypass
    {
        public static class PEBKACHelper
        {
            private const string pluginsPath = "Plugins/AstralBypass.dll";
            private const string modsPath = "Mods/AstralBypass.dll";
            private const int renameExitStatus = 69420_3697;  // random number, but the 69420 should indicate that it's human written;

            public static void CheckForUserError()
            {
                if (Directory.Exists("BepInEx")) return;

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
        }
    }
}
