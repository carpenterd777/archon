using System;
using System.Threading.Tasks;
using CliFx;

namespace Archon
{
    public static class Program
    {
       public static async Task<int> Main() => 
           await new CliApplicationBuilder()
           .AddCommandsFromThisAssembly()
           .Build()
           .RunAsync();
    }
}
