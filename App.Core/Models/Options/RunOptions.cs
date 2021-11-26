using CommandLine;

namespace App.Core.Models.Options
{
    public class RunOptions
    {
        [Option('o', "output", Required = false, HelpText = "Folder into which the generated website will be saved", Default = null)]
        public string OutputDirectory { get; set; }
    }
}
