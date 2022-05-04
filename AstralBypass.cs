using MelonLoader;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

[assembly: MelonInfo(typeof(Astrum.AstralBypass), nameof(Astrum.AstralBypass), "0.2.0", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralBypass))]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    // if this bypass breaks, open an issue on the github
    // i have a more aggressive fix than this non-intrusive one    
    public partial class AstralBypass : MelonPlugin
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        
        static AstralBypass() => PEBKACHelper.CheckForUserError();

        public override void OnApplicationEarlyStart()
        {
            if (!System.IO.File.Exists("UserData/.AstralBypass.nag.finished"))
            {
                Console.Beep();
                MessageBox.Show( // if you are a paster, you can create that file inside your .zip folder to prevent this message 
                    "AstralBypass is under attack from \"wholesome\" modders\n"
                    + "The VRCMG is an oligarchy intent on restrict the freedom of modding\n"
                    + "Please, urge your friends and mod developers to use BepInEx\n" 
                    + "BepInEx is a unbiased mod loader, better than MelonLoader\n"
                    + "(this message will only appear once)",
                    "Important message from AstralBypass",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop
                );

                System.IO.File.Create("UserData/.AstralBypass.nag.finished");
            }

            HarmonyInstance.Patch(
                typeof(Assembly).GetMethod(nameof(Assembly.Load), new Type[2] { typeof(byte[]), typeof(byte[]) }),
                typeof(AstralBypass).GetMethod(nameof(AssemblyLoadPre), PrivateStatic).ToNewHarmonyMethod(),
                typeof(AstralBypass).GetMethod(nameof(AssemblyLoadPost), PrivateStatic).ToNewHarmonyMethod()
            );
        }

        private static void AssemblyLoadPre()
        {
            // this may be slow, but it is what MelonModLogger/MelonLogger has been doing since day 1 :)
            StackTrace stack = new();
            if (stack.FrameCount < 2 || stack.GetFrame(2).GetMethod().Name != "LoadFromByteArray")
                return;
            IntegrityChecks.Bypass();
        }
        
        private static void AssemblyLoadPost() => IntegrityChecks.Repair();
    }
}
