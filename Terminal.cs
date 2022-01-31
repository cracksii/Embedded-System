using System;
using System.Collections.Generic;
using System.Linq;


class Terminal  // Emulates a terminal for the virtual file system
{
    public static Terminal INSTANCE {get; private set;}  // singleton instance
    public string workingDirectory {get; set;}  // The current working directory
    public List<ICommand> commands {get; private set;}  // All commands
    private List<string> history = new List<string>();  // The command history

    public Terminal()  // Singleton initializer
    {
        commands = new List<ICommand>();

        // Finds all classes implementing the ICommand interface and creates new instances of them
        foreach(Type mytype in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(ICommand)))) 
            commands.Add((ICommand)Activator.CreateInstance(mytype));

        workingDirectory = "/";
        INSTANCE = this;
        if((FileSystem.INSTANCE.encrypted && Login()) || !FileSystem.INSTANCE.encrypted)
            Input();
    }

    public void Input()  // Prints the input message
    {
        string prefix = workingDirectory.Replace(" ", "_") + " >> ";
        Console.Write(prefix);
        string currentInput = "";
        var keyInfo = Console.ReadKey(true);
        int index = history.Count;
        while(keyInfo.Key != ConsoleKey.Enter)
        {
            switch(keyInfo.Key)
            {
                case(ConsoleKey.Tab):
                    if(currentInput.Contains(" "))
                    {
                        string path = currentInput.Split(" ").Last();
                        foreach(string p in FileSystem.INSTANCE.files.Keys)
                        {
                            //Console.WriteLine(p + ", " + path);
                            bool matches = false;
                            for(int i = 0; i < path.Length; i++)
                            {
                                if(p[i] != path[i])
                                    break;
                                if(i == path.Length - 1)
                                    matches = true;
                            }
                            if(matches)
                                path = p;
                        }
                        int pathCount = path.Count(x => x == '/');
                        int wdCount = Terminal.INSTANCE.workingDirectory.Count(x => x == '/');
                        log(wdCount.ToString() + ", " + string.Join('/', path.Split("/")));
                        string newLine = "";
                        try
                        {
                            newLine = currentInput.Replace(currentInput.Split(" ").Last(), "/" + string.Join('/', path.Split("/")[wdCount]));
                        }
                        catch
                        {
                            ClearLine();
                            Console.Write(prefix + currentInput);
                            break;
                        }
                        ClearLine();
                        Console.Write(prefix + newLine);
                        currentInput = newLine;
                    }
                    break;
                case(ConsoleKey.Backspace):
                    try
                    {
                        currentInput = currentInput[..^1];  // Causes an error if the line is empty
                        Console.Write("\b \b");  // Deletes the last character
                    }
                    catch{}
                    break;
                case(ConsoleKey.UpArrow):
                    currentInput = "";
                    try
                    {
                        ClearLine();
                        index--;
                        currentInput = history[index];
                        Console.Write(prefix + currentInput);
                    }
                    catch
                    {
                        if(index == -1)
                            currentInput = history[index + 1];
                        else
                            currentInput = "";
                        Console.Write(prefix + currentInput);
                    }
                    break;
                case(ConsoleKey.DownArrow):
                    currentInput = "";
                    try
                    {
                        ClearLine();
                        index++;
                        currentInput = history[index];
                        Console.Write(prefix + currentInput);
                    }
                    catch
                    {
                        Console.Write(prefix);
                    }
                    break;
                default:
                    ClearLine();
                    currentInput += keyInfo.KeyChar;
                    Console.Write(prefix + currentInput);
                    break;
            }
            if(index > history.Count)
                index = history.Count;
            else if(index < 0)
                index = 0;
            /*log(index.ToString());
            log(string.Join(", ", history));*/
            keyInfo = Console.ReadKey(true);
        }
        Console.WriteLine();
        if(currentInput.Length != 0)
        {
            history.Add(currentInput);
            ExecuteCommand(currentInput.Split(" "));
        }
        Input();
    }

    public void ExecuteCommand(string[] inputs)  // Parses the args and executes the given command
    {
        ICommand command = null;
        foreach(var cmd in commands)
        {
            Array.ForEach(cmd.aliases, i => 
            {
                if(inputs[0] == i)
                {
                    command = cmd;
                    return;
                }
            });
        }

        Dictionary<string, string> args = new Dictionary<string, string>();

        for(int i = 1; i < inputs.Length; i++)
        {
            List<string> vals = new List<string>();
            if(inputs[i].Contains("="))
                vals = inputs[i].Split("=").ToList();
            else
                vals = new List<string>(){inputs[i], ""};
            try
            {
                while(vals[0][0] == '-')
                    vals[0] = vals[0][1..^0];
            }
            catch
            {
                Input();
            }
            
            args.Add(vals[0], vals[1]);
        }

        List<string> copy = new List<string>{};
        try
        {
            copy = command.necessaryParameters.ConvertAll(str => new string(str));
        }
        catch
        {
            Console.WriteLine($"Command \"{inputs[0]}\" wasn't registered so far\n");
            Input();
        }
        foreach(string param in command.necessaryParameters)
        {
            if(args.Keys.Contains(param))
                copy.Remove(param);
        }

        if(copy.Count != 0)
        {
            Console.WriteLine($"The parameter/s \"{string.Join(',', copy.ToArray())}\" was/were missing.\n");
            Input();
        }

        /*foreach(var a in args)
            Console.WriteLine($"{a.Key}={a.Value}");*/

        if(command != null)
            command.Handle(args);
    }

    public bool Login()  // Login to the FileSystem if its encrypted
    {
        Console.Write("Username: ");
        string username = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine();
        if(FileSystem.INSTANCE.users.Contains(new User(username, password)))
        {
            Console.WriteLine("\n");
            return true;
        }
        return false;
    }

    void ClearLine()
    {
        var currentLine = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLine);
    }

    void log(string logstr)
    {
        System.IO.File.AppendAllText("log", logstr + "\n");
    }
}

interface ICommand  // The interface every command has to implement
{
    string[] aliases {get; set;}  // Aliases (other names) for the command
    string description {get; set;}  // A description for the command
    List<string> necessaryParameters {get; set;}  // Parameters necesarry to execute the command
    void Handle(Dictionary<string, string> args);  // Function to handle the command
}
