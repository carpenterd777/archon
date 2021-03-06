using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace Archon
{
    [Command("write", Description = "Edit an existing session of roleplay.")]
    public class WriteCommand : ICommand
    {
        [CommandParameter(0, Description = "The name of the file to edit.")]
        public string Filepath {get; set;}

        public ValueTask ExecuteAsync(IConsole console)
        {
            WriteSessionManager wsm = new(console.Output, console.Input);
            wsm.Load(Filepath);
            wsm.CommandLoop();

            return default; 
        }
    }

    [Command("write new", Description = "Start taking notes for a new session of roleplay.")]
    public class WriteNewCommand : ICommand
    {
        [CommandOption("title", Description = "The title for this session of roleplay." )]
        public string Title {get; set;}

        [CommandOption("session", Description = "The number for this session of roleplay.")]
        public int SessionNumber {get; set;}
        public ValueTask ExecuteAsync(IConsole console)
        {
            WriteSessionManager wsm = new(console.Output, console.Input);

            if (Title == default)
            {
                wsm.PromptSessionTitle();
            } 
            else
            {
                wsm.SessionTitle = Title;
            }

            console.Output.WriteLine(); // a new line for padding

            if (SessionNumber == default)
            {
                wsm.PromptSessionNumber();
            } 
            else
            {
                wsm.SessionNumber = SessionNumber;
            }

            wsm.CommandLoop();

            return default; 
        }
    }
}