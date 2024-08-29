using System;

namespace MusicScraper;

public class DirectoryStructure
{
    public string Layout { get; set; }
    public List<(MetaData,string)> Entries { get; set; }

    public List<string> Paths;

    public DirectoryStructure(string layout = "{Artist}/{Album}/{Title}")
    {
        Layout = layout;
        Paths = new List<string>();
        Entries = new List<(MetaData, string)>();
    }

    public void GetFiles(string path){

        foreach (var asset in Directory.GetFiles(path,"*.mp3")){
            Console.WriteLine(asset);
            
            Paths.Add(Path.GetFileName(asset));
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

    public void BuildStructure(string path){
        foreach (var entry in Entries)
        {
            var newPath = Layout.Replace("{Artist}", string.Join(", ",entry.Item1.Artists))
                .Replace("{Album}", entry.Item1.Album);
            Directory.CreateDirectory(Path.Combine(path, newPath));
            File.Move(entry.Item2, Path.Combine(path, newPath, StripInvalidChars(entry.Item1.Title) + Path.GetExtension(entry.Item2)));
        }
    }

    public string StripInvalidChars(string input)
    {
        return string.Concat(input.Split(Path.GetInvalidFileNameChars()));
    }


    
}
