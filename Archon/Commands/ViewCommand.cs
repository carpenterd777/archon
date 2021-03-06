using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Archon
{
    [Command("view", Description = "View a session of roleplay.")]
    public class ViewCommand : ICommand
    {
        [CommandParameter(0, Description = "The name of the file to view.")]
        public string Filepath { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            ViewSessionManager vsm = new(console.Output, console.Input);
            vsm.Load(Filepath);
            vsm.RenderLoop();

            return default;
        }
    }
}
