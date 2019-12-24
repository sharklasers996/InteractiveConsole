using System.IO;
using System;
using System.Collections.Generic;
using InteractiveConsole.Attributes;
using InteractiveConsole.Storage;
using InteractiveConsole.Constants;

namespace InteractiveConsole.Commands
{
    [Command(Description = "Writes variables to a file", Category = CommandCategories.BuiltIn)]
    public class WriteToFileCommand : BaseCommand
    {
        [CommandParameter]
        public InMemoryStorageVariable Variable { get; set; }

        [CommandParameter]
        public List<InMemoryStorageVariable> Variables { get; set; }

        [CommandParameter]
        [Required]
        public string Path { get; set; }

        public override bool IsValid()
        {
            if (Variable == null
                && Variables == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(Path))
            {
                return false;
            }

            return true;
        }

        public override object Execute()
        {
            var fileInfo = new FileInfo(Path);
            string filename;
            string directory;
            if (String.IsNullOrEmpty(fileInfo.Extension))
            {
                filename = Guid.NewGuid() + ".txt";
                directory = fileInfo.FullName;
                Printer.WriteLine().Info($"File doesn't exist, created '{filename}'");
            }
            else
            {
                filename = fileInfo.Name;
                directory = fileInfo.DirectoryName;
            }
            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            var fullPath = System.IO.Path.Combine(directory, filename);

            var variableString = string.Empty;
            if (Variable != null)
            {
                variableString = Variable.ValueString;
            }

            if (Variables != null)
            {
                foreach (var v in Variables)
                {
                    variableString += v.ToString();
                }
            }

            File.WriteAllText(fullPath, variableString);
            Printer.WriteLine().Success($"Wrote to {fullPath}");

            return null;
        }
    }
}