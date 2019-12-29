using System.IO;
using System;
using System.Collections.Generic;
using InteractiveConsole.Attributes;
using InteractiveConsole.Constants;
using InteractiveConsole.Models.Storage;

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
            if (Variable.TypeInfo.IsList)
            {
                for (var i = 0; i < Variable.Length; i++)
                {
                    var listItem = Variable.TypeInfo.Type.GetProperty("Item").GetValue(Variable.Value, new object[] { i });
                    variableString += listItem.ToString() + "\n";
                }
            }
            else if (!String.IsNullOrEmpty(Variable.ValueString.Trim()))
            {
                variableString += Variable.ValueString;
            }

            File.WriteAllText(fullPath, variableString);
            Printer.WriteLine().Success($"Wrote to {fullPath}");

            return null;
        }
    }
}