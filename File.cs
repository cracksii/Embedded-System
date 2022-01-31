class File
{
    private byte[] content;  // The content as a byte array
    private string path = null;  // The path of the file in the embedded system
    private int size = 0;  // The size of the virtual file

    public File(string _content)  // Creates a new file with a string as content
    {
        content = System.Text.Encoding.UTF8.GetBytes(_content);
        size = content.Length;
    }
    public File(byte[] _content)  // Creates a new file with a byte array as content
    {
        content = _content;
        size = content.Length;
    }

    public byte[] GetByteContent() => content;  // Gets the content as bytes
    public string GetTextContent() => System.Text.Encoding.UTF8.GetString(content);  // Gets the content as utf8 string
    public string GetPath()  // Gets the path of the file
    {
        if(path != null)
            return path;
        else
        {
            path = FileSystem.INSTANCE.GetPathFromFile(this);
            return path;
        }
    }
    public int GetSize() => size;  // Gets the size of the file
    public void Write(string _content)  // Writes a new string content to the file
    {
        content = System.Text.Encoding.UTF8.GetBytes(_content);
    }
    public void Write(byte[] _content)  // Writes a new byte array as content to the file
    {
        content = _content;
    }
}

struct Data  // The representation of a file in the register
{
    public byte[] content {get; set;}
    public string path {get; set;}
    public int size {get; set;}

    public Data(byte[] _content, string _path, int _size)
    {
        content = _content;
        path = _path;
        size = _size;
    }
}

struct User  // The representation of a user in the register
{
    string username;
    string password;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
