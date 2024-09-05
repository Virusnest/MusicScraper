using System;
using System.Text;
using TagLib;

namespace MusicScraper;
    /// <summary>
    /// Used for Building the directory structure for the files
    /// </summary>
public class DirectoryStructure
{

    public string Layout { get; set; }
    public List<(MetaData,string)> Entries { get; set; }

    public List<string> Paths;

    public DirectoryStructure(string layout = "{Artist[0]}/{Artist}/{Album}/{Artist} - {Title}{ext}")
    {
        Layout = layout;
        Paths = new List<string>();
        Entries = new List<(MetaData, string)>();
    }


    /// <summary>
    /// Recursively get all files in a directory and its subdirectories then write them to the Paths list
    /// </summary>
    /// <param name="path"></param>
    public void GetFiles(string path){

        foreach (var asset in Directory.GetFiles(path,"*.mp3")){
            Console.WriteLine(asset);
            
            Paths.Add(asset);
        }
        foreach (var asset in Directory.GetDirectories(path)){
            GetFiles(asset);
        }
    }

    public void ResoloveMetaData()
    {
        foreach (var path in Paths)
        {
            var data = TagLib.File.Create(path);
            Entries.Add((new MetaData(data.Tag.AlbumArtists, data.Tag.Title, data.Tag.Album, (int)data.Tag.Year), path));
        }
    }
/// <summary>
/// Generate the directory structure for a file
/// </summary>
/// <param name="file"></param>
/// <returns></returns>
    private string GenerateLayoutForFile(TagLib.File file){
        Stack<char> text = new Stack<char>();
        StringBuilder builder = new StringBuilder();
        var c=GenerateStreamFromString(Layout);
        Stack<char> num = new Stack<char>();
        while(c.Position<Layout.Length){
            char h = (char)c.ReadByte();
            if (h== '{'){
                char ch;
                while ((ch = (char)c.ReadByte()) != '}'){
                    if(ch=='['){
                        char chch;
                        while ((chch=(char)c.ReadByte()) != ']'){
                            num.Push(chch);
                        }
                    }else{
                        text.Push(ch);
                    }
                }
                if (num.Count>0){
                    builder.Append(ParseType(Reverse(new string( text.ToArray())),file)[Convert.ToInt32(new string(num.ToArray()))]);
                }else{
                    builder.Append(ParseType(Reverse(new string( text.ToArray())),file));
                }
                num.Clear();
                text.Clear();
            }
            if (h!='{'){
                builder.Append(h);
            }
            
        }
        return builder.ToString();
    }

    /// <summary>
    /// Generate Stream from a string;
    /// </summary>
    /// <param name="s"></param>
    /// <returns> 
    /// A stream from the Inputs string
    /// </returns>
    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// Reverse a string 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>

    public static string Reverse( string s )
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// Parse the type of metadata to be used in the layout string
    /// </summary>
    /// <param name="type"></param>
    /// <param name="file"></param>
    /// <returns></returns>

    private string ParseType(string type, TagLib.File file){
        if(file.Tag.IsEmpty){
            return "UNKNOWN";
        }
        switch(type){
            case "Artist":
            if (file.Tag.Performers.Length==0){break;}
                return
                StripInvalidChars(file.Tag.Performers.First());
            case "Album":
                if (file.Tag.Album==null){break;}
                return
                StripInvalidChars(file.Tag.Album);
            case "Title":
                if (file.Tag.Title==null){break;}
                return
                StripInvalidChars(file.Tag.Title);
            case "Year":
                return
                StripInvalidChars(file.Tag.Year.ToString());
            case "ext":
                if (file.Name==null){break;}
                return
                StripInvalidChars(Path.GetExtension(file.Name));
        }
        return "UNKNOWN";
    }
    public void BuildStructure(string path){
        // Create the directory structure with the layout string dynamicly
        foreach (var entry in Entries){
            var newPath = GenerateLayoutForFile(TagLib.File.Create(entry.Item2));
            newPath = Path.Combine(path, newPath);
            Console.WriteLine(newPath);
        }

    }

    private string StripInvalidChars(string input)
    {
        return string.Concat(input.Split(Path.GetInvalidFileNameChars()));
    }


    
}
