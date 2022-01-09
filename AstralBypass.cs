using MelonLoader;
using System;
using System.Reflection;

[assembly: MelonInfo(typeof(Astrum.AstralBypass), nameof(Astrum.AstralBypass), "0.1.3", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralBypass))]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    public partial class AstralBypass : MelonPlugin
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        static AstralBypass() => PEBKACHelper.CheckForUserError();

        public override void OnApplicationEarlyStart() =>
            HarmonyInstance.Patch(
                typeof(Assembly).GetMethod(nameof(Assembly.Load), new Type[2] { typeof(byte[]), typeof(byte[]) }),
                typeof(AstralBypass).GetMethod(nameof(AssemblyLoadPre), PrivateStatic).ToNewHarmonyMethod(),
                typeof(AstralBypass).GetMethod(nameof(AssemblyLoadPost), PrivateStatic).ToNewHarmonyMethod()
            );

        private static void AssemblyLoadPre() => IntegrityChecks.Bypass();
        private static void AssemblyLoadPost() => IntegrityChecks.Repair();
        
        // if this bypass breaks, open an issue on the github
        // i have a more aggressive fix than this non-intrusive one
    }
}
