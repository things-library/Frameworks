using System.Reflection;
using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.Base.DataType.Extensions
{
    public static class AssemblyExtensions
    {
        public static string AgentString(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var name = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? throw new ArgumentNullException("Assembly Product Name");

            var version = assembly.GetName()?.Version ?? throw new ArgumentNullException("Assembly File Version");

            return $"{name}/{version.ToDotString()} ({Metrics.MachineMetrics.OsVersion()})";
        }
    }
}
