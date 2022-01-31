using System.Text.Json;
using System.Text;
using System.Collections.Generic;

public class Json  // Class for serializing and deserializing an object to json or from json to an object
{
    public static string PrettySerialize(object _obj)  // Serializes an object to an intended json string
    {
        byte[] jsonUtf8Bytes;
        var options = new JsonSerializerOptions{WriteIndented = true};
        jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(_obj, options);
        return Encoding.UTF8.GetString(jsonUtf8Bytes);
    }

    public static string Serialize(object _obj) => JsonSerializer.Serialize(_obj);  // Serializes an object to its json representation

    public static void SaveObjectToJsonFile(string _path, object _obj) => System.IO.File.WriteAllText(_path, PrettySerialize(_obj));  // Saves an object to a file

    public static T Deserialize<T>(string _json) => (T)JsonSerializer.Deserialize<T>(_json);  // Gets an object from its json representation

    public static T ReadObjectFromJsonFile<T>(string _path) => Deserialize<T>(System.IO.File.ReadAllText(_path));  // Gets an object from a file containing the representation
}

public class Wrapper<T>  // Wrapper class to contain multiple objects of type T
{ 
    public List<T> data {get; set;} 

    public Wrapper()
    {
        this.data = new List<T>();
    }
}

public class Wrapper<T, S>  // Wrapper class to contain multiple objects of type T and S
{ 
    public List<T> data1 {get; set;}
    public List<S> data2 {get; set;}

    public Wrapper()
    {
        this.data1 = new List<T>();
        this.data2 = new List<S>();
    }
}
