using System;
using System.Reflection;
using System.Windows;

namespace sample.wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            foreach (var an in GetType().Assembly.GetReferencedAssemblies())
            {
                var a = Assembly.Load(an);
                yocto.AutoRegistration.Register(a);
            }
        }
    }
}
