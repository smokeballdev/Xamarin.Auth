﻿using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using UnitTests.HeadlessRunner;
using Xunit.Runners.UI;

namespace Xamarin.Auth.DeviceTests.Droid
{
    [Activity(MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            var hostIp = Intent.Extras?.GetString("HOST_IP", null);
            var hostPort = Intent.Extras?.GetInt("HOST_PORT", 10578) ?? 10578;

            if (!string.IsNullOrEmpty(hostIp))
            {
                // Run the headless test runner for CI
                Task.Run(() =>
                {
                    return Tests.RunAsync(new TestOptions
                    {
                        Assemblies = new List<Assembly> { typeof(Battery_Tests).Assembly },
                        NetworkLogHost = hostIp,
                        NetworkLogPort = hostPort,
                        Format = TestResultsFormat.XunitV2
                    });
                });
            }

            // tests can be inside the main assembly
            AddTestAssembly(Assembly.GetExecutingAssembly());
            AddTestAssembly(typeof(Utils).Assembly);
            AddExecutionAssembly(typeof(Utils).Assembly);

            // or in any reference assemblies
            //   AddTestAssembly(typeof(PortableTests).Assembly);
            // or in any assembly that you load (since JIT is available)

#if false
            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
            Writer = new TcpTextWriter("10.0.1.2", 16384);
            // start running the test suites as soon as the application is loaded
            AutoStart = true;
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;
#endif

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
