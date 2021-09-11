using CommandLine;

namespace App.Console.Options
{
    [Verb("run", HelpText = "Run functions.")]
    public class RunOptions
    {
        [Option('a', "action", Required = true, HelpText = "Identify the action to perform.")]
        public RunAction Action { get; set; }
    }

    public enum RunAction
    {
        AnAction = 0
    }
}
