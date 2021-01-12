using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Archon
{
    [Command]
    public class WriteCommand : ICommand
    {
        public ValueTask ExecuteAsync(IConsole console)
        {
            WriteSessionManager wsm = new(console.Output);
            wsm.PromptSessionTitle();
            wsm.PromptSessionNumber();

            return default; //this command is synchronous
        }
    }
}
