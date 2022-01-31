using System;
using System.Diagnostics;
using System.Collections.Generic;

class FileSystem  // A virtual file-system in a single file
{
    public static FileSystem INSTANCE {get; private set;}  // The singleton instance
    public FileCollection files {get; private set;}  // A custom dictionary containing the files
    public bool encrypted {get; private set;}  // FileSystem is encrypted or not
    public List<User> users {get; private set;}  // The users registered
    private string saveFile;  // The path to the register

    public FileSystem(string _saveFile)  // Creates a new FileSytem singleton
    {
        INSTANCE = this;  // Creates the instance
        encrypted = true;  // Wether to encrypt the register or not
        saveFile = _saveFile;  // The path to the register
        files = new FileCollection();  // The files
        users = new List<User>();  // The users
        Load();  // Loads existing users and files from the register
        if(encrypted)
            users.Add(new User("Admin", "1234"));  // Create a defaukt user
        files.Add("/information", new File("This system is running on your host system, very cool!"));  // Create an information file
    }

    public void ListFiles(string path)  // Searches for files containing the given path
    {
        foreach(var file in files)
        {
            bool isPath = true;
            for(int i = 0; i < path.Length; i++)
            {
                if(path[i] != file.Key[i])
                {
                    isPath = false;
                    break;
                }
            }
            if(isPath)
                Console.WriteLine(file.Key);
        }
    }
    
    public File GetFile(string path)  // Gets the file by path
    {
        try
        {
            return files[path];
        }
        catch
        {
            throw new ArgumentException($"File at path {path} wasn't found");
        }
    }
    
    public string GetPathFromFile(File file)  // Gets the path by file
    {
        foreach(var path in files.Keys)
        {
            if(files[path] == file)
                return path;
        }
        throw new ArgumentException("Path for file wasn't found");
    }
    
    public void DeleteFile(string path)  // Deletes the file at a path
    {
        try
        {
            files.Remove(path);
        }
        catch
        {
            throw new ArgumentException($"File at path {path} couldn't be deleted");
        }
    }
    
    public void DeleteFiles(params string[] paths)  // Deletes multiple files
    {
        foreach(string path in paths)
            DeleteFile(path);
    }
    
    public void CreateFile(string path, string content)  // Creates a file with path and string content
    {
        files.Add(path, new File(content));
    }
    
    public void CreateFile(string path, byte[] content)  // Creates a file with path and bytes[] content
    {
        files.Add(path, new File(content));
    }

    public void CopyHostFile(string hostPath, string newPath)  // Copys file from the host- to the embedded system
    {
        CreateFile(newPath, System.IO.File.ReadAllBytes(hostPath));
    }
    
    public void CopyFileToHost(string path, string hostPath)  // Copys file from the embedded- to the host system
    {
        System.IO.File.WriteAllBytes(hostPath, GetFile(path).GetByteContent());
    }

    public void Save()  // Saves the files and users to the register
    {
        if(encrypted)
        {
            Wrapper<Data, User> wrapper = new Wrapper<Data, User>();
            foreach(var file in files)
                wrapper.data1.Add(new Data(file.Value.GetByteContent(), file.Key, file.Value.GetSize()));

            foreach(var user in users)
                wrapper.data2.Add(user);

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(saveFile), "temp");
            Json.SaveObjectToJsonFile(path, wrapper);
            Encrypt(path);
            System.IO.File.WriteAllBytes(saveFile, System.IO.File.ReadAllBytes(path));
            System.IO.File.Delete(path);
        }
        else
        {
            Wrapper<Data> wrapper = new Wrapper<Data>();
            foreach(var file in files)
                wrapper.data.Add(new Data(file.Value.GetByteContent(), file.Key, file.Value.GetSize()));
            Json.SaveObjectToJsonFile(saveFile, wrapper);
        }
    }
    
    public void Load()  // Loads the files and users from the register
    {
        if(!System.IO.File.Exists(saveFile))
        {
            Console.WriteLine($"Warning: Wasn't able to load files from saveFile at {saveFile}. Creating a new one.");
            Save();
            return;
        }

        if(encrypted)
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(saveFile), "temp");
            System.IO.File.WriteAllBytes(path, System.IO.File.ReadAllBytes(saveFile));
            Decrypt(path);
            Wrapper<Data, User> wrapper = Json.ReadObjectFromJsonFile<Wrapper<Data, User>>(path);
            System.IO.File.Delete(path);

            files.Clear();
            foreach(var data in wrapper.data1)
                files.Add(data.path, new File(data.content));

            users.Clear();
            foreach(var user in wrapper.data2)
                users.Add(user);
        }
        else
        {
            Wrapper<Data> wrapper = Json.ReadObjectFromJsonFile<Wrapper<Data>>(saveFile);
            files.Clear();
            foreach(var data in wrapper.data)
                files.Add(data.path, new File(data.content));
        }
    }

    public void Encrypt(string _file)  // Encrypts the register with ghost-encrypt
    {
        using(System.Diagnostics.Process process = new Process())
        {
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = $"/c ghost --ef -p {_file} -k 1234567890";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "powershell.exe";
            process.Start();
            process.WaitForExit();
        }
    }
   
    public void Decrypt(string _file)  // Decrypts the register with ghost-encrypt
    {
        using(System.Diagnostics.Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = $"/c ghost --df -p {_file} -k 1234567890";
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                //Console.WriteLine(process.StandardOutput.ReadToEnd());
                process.WaitForExit();
        }
    }
}

internal sealed class FileCollection : Dictionary<string, File>  // Extension of a dictionary, adding replace and save functionality
{
    public new void Add(string path, File file)
    {
        if(base.ContainsKey(path))
            base.Remove(path);
        base.Add(path, file);
        FileSystem.INSTANCE.Save();
    }

    public new void Remove(string path)
    {
        base.Remove(path);
        FileSystem.INSTANCE.Save();
    }
}
