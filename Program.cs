class Program
{
    static void Main(string[] args)  // Entry-point for the compiler and the executable
    {
        new FileSystem(@"system.bink");  // Initializes the FileSystem with the register path
        //FileSystem.INSTANCE.CopyFileToHost(@"/mami", "mami.jpg");
        new Terminal();  // Initializes a new Terminal
    }
}
