using System.Collections.Generic;
using System;
using System.Linq;

class Ls : ICommand  // Command to list files in  the current directory
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Ls()  // Default constructor
    {
        this.description = "Shows all files and directorys in the current working directory";
        this.aliases = new string[]{"ls", "Ls", "dir", "Dir"};
        this.necessaryParameters = new List<string>();
    }
 
    public void Handle(Dictionary<string, string> args)  // Function handling the command
    {
        if(args.Count == 0)
        {
            int offset = 15;
            int count = Terminal.INSTANCE.workingDirectory.Count(x => x == '/');
            Dictionary<string, File> files = new Dictionary<string, File>();
            Console.Write($"  Directory: \"{Terminal.INSTANCE.workingDirectory}\"\n\n  Name");
            int longestName = 0;
            int greatestSize = 0;
            foreach(var pair in FileSystem.INSTANCE.files)
            {
                if(pair.Key.Contains(Terminal.INSTANCE.workingDirectory))
                {
                    files.Add(pair.Key, pair.Value);
                    if(pair.Key.Split('/').Last().Length > longestName)
                        longestName = pair.Key.Split('/').Last().Length;
                    if(pair.Value.GetSize().ToString().Length > greatestSize)
                        greatestSize = pair.Value.GetSize().ToString().Length;
                }
            }
            
            for(int i = 0; i < longestName + offset - 6; i++)
                Console.Write(" ");
            Console.Write("Size");

            for(int i = 0; i < greatestSize + offset - 4; i++)
                Console.Write(" ");
            Console.WriteLine("Type");
            
            List<string> used = new List<string>();

            foreach(var pair in files)
            {
                int fileCount = pair.Key.Count(x => x == '/');
                string output = "";
                string type = "";

                if(count == fileCount)
                {
                    output = $"  {pair.Key.Split('/').Last()}";
                    type = "file";
                }
                else if(count < fileCount)
                {
                    if(used.Contains(pair.Key.Split('/')[count]))
                        continue;
                    else
                        used.Add(pair.Key.Split('/')[count]);
                    output = $"  {pair.Key.Split('/')[count]}";
                    type = "directory";
                }

                while(output.Length < longestName + offset)  
                    output += " ";

                if(type == "file")
                {
                    if(pair.Value.GetSize() < 1000)
                        output += $"{pair.Value.GetSize().ToString()} B";
                    else if(pair.Value.GetSize() < 1000000)
                        output += $"{(pair.Value.GetSize() / 1000).ToString()} KB";
                    else if(pair.Value.GetSize() < 1000000000)
                        output += $"{(pair.Value.GetSize() / 1000000).ToString()} MB";
                    else
                        output += $"{(pair.Value.GetSize() / 1000000000).ToString()} GB";
                }

                while(output.Length < greatestSize + longestName + offset * 2) 
                    output += " ";

                output += type;
                Console.WriteLine(output);
            }
            Console.WriteLine();
        }
        else if(args.Count == 1)
        {
            string directory = args.Keys.ToList()[0];
            if(directory[^1] != '/')
                directory += "/";
            int offset = 15;
            int count = directory.Count(x => x == '/');
            Dictionary<string, File> files = new Dictionary<string, File>();
            Console.Write($"  Directory: \"{directory}\"\n\n  Name");
            int longestName = 0;
            int greatestSize = 0;
            foreach(var pair in FileSystem.INSTANCE.files)
            {
                if(pair.Key.Contains(directory))
                {
                    files.Add(pair.Key, pair.Value);
                    if(pair.Key.Split('/').Last().Length > longestName)
                        longestName = pair.Key.Split('/').Last().Length;
                    if(pair.Value.GetSize().ToString().Length > greatestSize)
                        greatestSize = pair.Value.GetSize().ToString().Length;
                }
            }
            
            for(int i = 0; i < longestName + offset - 6; i++)
                Console.Write(" ");
            Console.Write("Size");

            for(int i = 0; i < greatestSize + offset - 4; i++)
                Console.Write(" ");
            Console.WriteLine("Type");
            
            List<string> used = new List<string>();

            foreach(var pair in files)
            {
                int fileCount = pair.Key.Count(x => x == '/');
                string output = "";
                string type = "";

                if(count == fileCount)
                {
                    output = $"  {pair.Key.Split('/').Last()}";
                    type = "file";
                }
                else if(count < fileCount)
                {
                    if(used.Contains(pair.Key.Split('/')[count]))
                        continue;
                    else
                        used.Add(pair.Key.Split('/')[count]);
                    output = $"  {pair.Key.Split('/')[count]}";
                    type = "directory";
                }

                while(output.Length < longestName + offset)  
                    output += " ";

                if(type == "file")
                {
                    if(pair.Value.GetSize() < 1000)
                        output += $"{pair.Value.GetSize().ToString()} B";
                    else if(pair.Value.GetSize() < 1000000)
                        output += $"{(pair.Value.GetSize() / 1000).ToString()} KB";
                    else if(pair.Value.GetSize() < 1000000000)
                        output += $"{(pair.Value.GetSize() / 1000000).ToString()} MB";
                    else
                        output += $"{(pair.Value.GetSize() / 1000000000).ToString()} GB";
                }

                while(output.Length < greatestSize + longestName + offset * 2) 
                    output += " ";

                output += type;
                Console.WriteLine(output);
            }
            Console.WriteLine();
        }
    }
}

class Cd : ICommand  // Command to change the working directory
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Cd()  // Default constructor
    {
        this.description = "Changes the current working directory";
        this.aliases = new string[]{"cd", "Cd"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handles the command
    {
        if(args.ContainsKey(".."))
        {
            try
            {
                Terminal.INSTANCE.workingDirectory = "/" + string.Join('/', Terminal.INSTANCE.workingDirectory.Remove(Terminal.INSTANCE.workingDirectory.Length - 1).Replace(Terminal.INSTANCE.workingDirectory, "").Split('/')[..^1])[1..];
                if(Terminal.INSTANCE.workingDirectory[1] == '/')
                    Terminal.INSTANCE.workingDirectory = Terminal.INSTANCE.workingDirectory[1..];
                if(Terminal.INSTANCE.workingDirectory[^1] != '/')
                    Terminal.INSTANCE.workingDirectory += "/";
            }
            catch
            {
                Terminal.INSTANCE.workingDirectory = "/";
            }
        }
        else if(args.Count == 1)
        {
            string newDir;
            if(new List<string>(args.Keys)[0] == "/")
            {
                Terminal.INSTANCE.workingDirectory = "/";
                return;
            }
            if(new List<string>(args.Keys)[0][0] == '/')
                newDir = new List<string>(args.Keys)[0] + "/";
            else
                newDir = Terminal.INSTANCE.workingDirectory + new List<string>(args.Keys)[0] + "/";
            bool hasCorrectVal = false;
            
            foreach(string path in FileSystem.INSTANCE.files.Keys)
            {
                for(int i = 0; i < newDir.Length; i++)
                {
                    
                    try
                    {
                        if(i == newDir.Length - 1 && path[i] == '/')  // Causes error if file and not directory
                            hasCorrectVal = true;
                        if(newDir[i] != path[i])
                            break;
                    }
                    catch{return;}
                }
                if(hasCorrectVal)
                    break;
            }
            if(hasCorrectVal)
                Terminal.INSTANCE.workingDirectory = newDir;
        }
    }
}

class Exit : ICommand  // Command to exit the console
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Exit()  // Default constructor
    {
        this.description = "Closes the terminal";
        this.aliases = new string[]{"exit", "Exit"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handle function 
    {
        Environment.Exit(0);
    }
}

class Cat : ICommand  // Command to show the contents of a file
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Cat()  // Default constructor
    {
        this.description = "Shows the content of a file";
        this.aliases = new string[]{"cat", "Cat"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handle function
    {
        if(args.Keys.Count == 1)
        {
            string path = new List<string>(args.Keys)[0]; 
            if(path[0] != '/')
                path = Terminal.INSTANCE.workingDirectory + path;

            try
            {
                string p = "Path: \"" + path + "\"\n\n";
                File file = FileSystem.INSTANCE.GetFile(path);
                Console.WriteLine(p + file.GetTextContent() + "\n");
            }
            catch
            {
                Console.WriteLine($"File at path {path} wasn't found.\n");
            }
        }
    }
}

class Clear : ICommand  // Command to clear the contents
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Clear()  // Default constructor
    {
        this.description = "Clears the console buffer";
        this.aliases = new string[]{"clear", "Clear"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handle function
    {
        Console.Clear();
    }
}

class Echo : ICommand  // Command to print content or write to files
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Echo()  // Default constructor
    {
        this.description = "Echos input to the console or to a file";
        this.aliases = new string[]{"echo", "Echo"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handle function
    {
        if(!args.Keys.Contains(">"))
        {
            foreach(var str in args.Keys.ToList())
            {
                Console.WriteLine(str);
            }
            Console.WriteLine();
        }
        else
        {
            string filename = args.Keys.ToList()[args.Keys.ToList().LastIndexOf(">") + 1];
            if(filename[0] != '/')
                filename = Terminal.INSTANCE.workingDirectory + filename;
            FileSystem.INSTANCE.CreateFile(filename, string.Join("\n", args.Keys.ToArray()[..^2]));
        }
    }
}

class Del : ICommand  // Command to delete files and directorys
{
    public string[] aliases {get; set;}  // other names for the command
    public string description {get; set;}  // descriptions for the command
    public List<string> necessaryParameters {get; set;}  // Necessary parameters for the command

    public Del()  // Default constructor
    {
        this.description = "Deletes a file or directory";
        this.aliases = new string[]{"del", "Del"};
        this.necessaryParameters = new List<string>();
    }

    public void Handle(Dictionary<string, string> args)  // Handle function
    {
        if(args.Keys.Count == 1)
        {
            string path;
            if(args.Keys.ToList()[0][0] != '/')
                path = Terminal.INSTANCE.workingDirectory + args.Keys.ToList()[0];
            else
                path = args.Keys.ToList()[0];
            List<string> filesToDelete = new List<string>();
            foreach(var file in FileSystem.INSTANCE.files)
            {
                bool isInSubDir = false;
                for(int i = 0; i < path.Length; i++)
                {
                    if(path[i] != file.Key[i])
                        break;
                    if(i == path.Length - 1 && (file.Key.Length == path.Length || file.Key[i + 1] == '/'))
                        isInSubDir = true;
                }
                if(isInSubDir)
                    filesToDelete.Add(file.Key);
            }
            FileSystem.INSTANCE.DeleteFiles(filesToDelete.ToArray());
        }
    }
}


