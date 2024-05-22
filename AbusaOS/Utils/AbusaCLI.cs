using Cosmos.Core;
using Cosmos.System.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AbusaOS.Windows;
using Color = System.Drawing.Color;
using System;

namespace AbusaOS.Utils
{
    public class CLICommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Aliases { get; private set; }

        public CLICommand(string name, string description, string[] aliases)
        {
            Name = name;
            Description = description;
            Aliases = aliases;
        }

        public virtual void Execute(List<string> args, Terminal instance) { }
    }

    public class CLIEcho : CLICommand
    {
        public CLIEcho() : base("Echo", "Echoes given string", new string[] { "echo", "out" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            instance.curcol = Color.White;
            for (int i = 1; i < args.Count; i++)
            {
                instance.print_str(args[i] + " ");
            }
            instance.print_str("\n");
        }
    }

    public class CLIClearScreen : CLICommand
    {
        public CLIClearScreen() : base("ClearScreen", "Clears the screen", new string[] { "cls", "clear", "clr" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            instance.print_clear();
        }
    }

    public class CLICallPSOD : CLICommand
    {
        public CLICallPSOD() : base("CallPSOD", "Calls the Purple Screen Of Death", new string[] { "CallPSOD" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            // Получаем строку из аргументов
            string message = args.Count > 0 ? args[0] : "U ok? Nothing happened";

            // Создаём исключение с этим сообщением
            Exception exception = new Exception(message);

            // Передаём исключение в Kernel.FatalErrorInternal
            Kernel.FatalErrorInternal(exception);
        }

    }

    public class CLIInfo : CLICommand
    {
        public CLIInfo() : base("Info", "Gets System Info", new string[] { "sysinfo", "sysfetch", "info" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            string vmname = "Environment isn't virtualized";
            if (Cosmos.System.VMTools.IsVMWare)
            {
                vmname = "VMWare";
            }
            else if (Cosmos.System.VMTools.IsQEMU)
            {
                vmname = "QEMU or KVM";
            }
            else if (Cosmos.System.VMTools.IsVirtualBox)
            {
                vmname = "VirtualBox";
            }

            if (CPU.GetCPUVendorName().Contains("Intel"))
            {
                instance.curcol = Color.Aqua;
                instance.print_str($"88                              88 | Abusa OS\n");
                instance.print_str($"''              ,d              88 | ---------\n");
                instance.print_str($"                88              88 | CPU : {Cosmos.Core.CPU.GetCPUBrandString()}\n");
                instance.print_str($"88 8b,dPPYba, MM88MMM ,adPPYba, 88 | RAM : {Cosmos.Core.CPU.GetAmountOfRAM()} MB\n");
                instance.print_str($"88 88P'   `'8a  88   a8P_____88 88 | VM : {vmname}\n");
                instance.print_str($"88 88       88  88   8PP''''''' 88 |\n");
                instance.print_str($"88 88       88  88,  '8b,   ,aa 88 |\n");
                instance.print_str($"88 88       88  'Y888 `'Ybbd8'' 88 |\n");
            }
            else if (CPU.GetCPUVendorName().Contains("AMD"))
            {
                instance.curcol = Color.Red;
                instance.print_str($"              *@@@@@@@@@@@@@@@     | Abusa OS\n");
                instance.print_str($"                 @@@@@@@@@@@@@     | ---------\n");
                instance.print_str($"                @%       @@@@@     | CPU : {Cosmos.Core.CPU.GetCPUBrandString()}\n");
                instance.print_str($"              @@@%       @@@@@     | RAM : {Cosmos.Core.CPU.GetAmountOfRAM()} MB\n");
                instance.print_str($"             @@@@&       @@@@@     | VM : {vmname}\n");
                instance.print_str($"             @@@@@@@@@     @@@     | \n");
                instance.print_str($"             #######               | \n");
                instance.print_str($"                                   | \n");
                instance.print_str($"            @@     @\\ /@  @@@@*    | \n");
                instance.print_str($"           @..@    @ @ @  @.   @   | \n");
                instance.print_str($"          @    @   @   @  @@@@*    | \n");
            }
            instance.curcol = Color.White;
        }
    }

    public class CLIDir : CLICommand
    {
        public CLIDir() : base("Dir", "Outputs Current Directory Listing", new string[] { "dir", "ls" }) { }

        void PrintRecursiveDirectory(string path, int ident, Terminal instance)
        {
            string identstr = "";
            for (int i = 0; i < ident; i++) { identstr += "--"; }

            string[] dirs = Directory.GetDirectories(path);
            instance.curcol = Color.Yellow;
            foreach (string dir in dirs)
            {
                instance.print_str($" {identstr} DIR   {dir}\n");
                PrintRecursiveDirectory(Path.Combine(path, dir), ident + 1, instance);
            }
            instance.curcol = Color.White;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                instance.print_str($" {identstr} FILE  {file}\n");
            }
        }

        public override void Execute(List<string> args, Terminal instance)
        {
            if (args.Count == 2 && args[1] == "-r")
            {
                PrintRecursiveDirectory(instance.pwd, 1, instance);
            }
            else
            {
                instance.print_str($" --- Directory Listing of {instance.pwd} ---\n");
                string[] dirs = Directory.GetDirectories(instance.pwd);
                instance.curcol = Color.Yellow;
                foreach (string dir in dirs)
                {
                    instance.print_str($" - DIR   {dir}\n");
                }
                instance.curcol = Color.White;
                string[] files = Directory.GetFiles(instance.pwd);
                foreach (string file in files)
                {
                    instance.print_str($" - FILE  {file}\n");
                }
                instance.curcol = Color.White;
            }
        }
    }

    public class CLICD : CLICommand
    {
        public CLICD() : base("CD", "Change Directory", new string[] { "cd", "chdir" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            if (args.Count != 2)
            {
                instance.print_str("Usage: cd [dirpath]");
                return;
            }

            if (args[1] == "..")
            {
                instance.pwd = Directory.GetParent(instance.pwd).FullName;
            }
            else if (args[1] == ".")
            {
                return;
            }
            else
            {
                string oldpwd = instance.pwd;
                if (Path.IsPathRooted(args[1]))
                {
                    instance.pwd = args[1];
                }
                else
                {
                    instance.pwd = Path.Join(instance.pwd, args[1]);
                }
                if (!Directory.Exists(instance.pwd))
                {
                    instance.curcol = Color.Red;
                    instance.print_str($"[ERR] No such path {instance.pwd}\n");
                    instance.curcol = Color.White;
                    instance.pwd = oldpwd;
                }
            }
        }
    }

    public class CLIHelp : CLICommand
    {
        public CLIHelp() : base("Help", "Displays help information for commands", new string[] { "help", "?" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            if (args.Count == 1 || (args.Count == 2 && args[1].ToLower() == "all"))
            {
                instance.curcol = Color.Green;
                instance.print_str("Available commands:\n");
                foreach (CLICommand cmd in AbusaCLI.Commands)
                {
                    instance.print_str($"- {cmd.Name}: {cmd.Description}\n");
                    if (cmd.Aliases.Length > 0)
                    {
                        instance.print_str($"  Aliases: {string.Join(", ", cmd.Aliases)}\n");
                    }
                }
            }
            else if (args.Count == 2)
            {
                string commandName = args[1].ToLower();
                CLICommand command = AbusaCLI.Commands.FirstOrDefault(cmd =>
                    cmd.Name.ToLower() == commandName || cmd.Aliases.Any(alias => alias.ToLower() == commandName));
                if (command != null)
                {
                    instance.curcol = Color.Green;
                    instance.print_str($"Command: {command.Name}\n");
                    instance.print_str($"Description: {command.Description}\n");
                    if (command.Aliases.Length > 0)
                    {
                        instance.print_str($"Aliases: {string.Join(", ", command.Aliases)}\n");
                    }
                }
                else
                {
                    instance.curcol = Color.Red;
                    instance.print_str($"[ERR] No such command {commandName}\n");
                }
            }
            else
            {
                instance.print_str("Usage: help [command | all]\n");
            }
            instance.curcol = Color.White;
        }
    }


    public class CLICat : CLICommand
    {
        public CLICat() : base("Cat", "Read file", new string[] { "cat", "read" }) { }
        public override void Execute(List<string> args, Terminal instance)
        {
            if (args.Count != 2)
            {
                instance.print_str("Usage: cat [dirpath]");
                return;
            }
            string fpath = "";

            if (Path.IsPathRooted(args[1]))
            {
                fpath = args[1];
            }
            else
            {
                fpath = Path.Join(instance.pwd, args[1]);
            }

            if (!File.Exists(fpath))
            {
                instance.curcol = Color.Red;
                instance.print_str($"[ERR] No such file {fpath}\n");
                instance.curcol = Color.White;
                return;
            }

            string contents = File.ReadAllText(fpath);

            instance.print_str(contents);
        }
    }

    public class AbusaCLI
    {
        public static CLICommand[] Commands = {
        new CLIClearScreen(),
        new CLIInfo(),
        new CLIEcho(),
        new CLIDir(),
        new CLICD(),
        new CLICat(),
        new CLIHelp(),
        new CLICallPSOD()
    };

        public static void ParseCommand(string command, Terminal instance)
        {
            List<string> args = command.Split(' ').ToList();
            args = args.Where(arg => !string.IsNullOrEmpty(arg)).ToList();

            foreach (CLICommand cmd in Commands)
            {
                if (cmd.Aliases.Any(alias => alias.ToLower() == args[0].ToLower()))
                {
                    cmd.Execute(args, instance);
                    return;
                }
            }
            instance.curcol = Color.Red;
            instance.print_str($"[ERR] No such command {args[0]}\n");
            instance.curcol = Color.White;
        }
    }

}