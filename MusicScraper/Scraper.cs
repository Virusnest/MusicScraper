using System;
using System.Collections.Concurrent;

namespace MusicScraper;

/// <summary>
/// Scraper class for scraping metadata from web APIs
/// </summary>
public class Scraper
{
    public List<string> Files = new List<string>();
    public Spotify spotify = new Spotify();
	public YTMusic youtube = new YTMusic();

	private ConcurrentQueue<MetaData> queue = new ConcurrentQueue<MetaData>();

    public Scraper(Action<(Spotify,YTMusic)> connect){   
        connect((spotify,youtube));
    }
    /// <summary>
    /// Recursivly all files in a directory and its subdirectories and add them to the Files list
    /// </summary>
    /// <param name="path"></param>
    public void GetFiles(string path){
        foreach (var asset in Directory.GetFiles(path,"*.mp3")){
            Console.WriteLine(asset);
            
            Files.Add(Path.GetFileName(asset));
        }
        foreach (var asset in Directory.GetDirectories(path)){
            GetFiles(asset);
        }
    }
    /// <summary>
    /// Writes Metadata to a file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public void WriteMetadata (string path, MetaData data){
        var wawa = TagLib.File.Create(path);
        wawa.Tag.Album=data.Album;
        wawa.Tag.AlbumArtists=data.Artists;
        wawa.Tag.Title=data.Title;
        wawa.Tag.Year=(uint)data.Year;
    }

    /// <summary>
    /// Scrape metadata from the web
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public async Task Scrape(Queue<string> paths)
	{
		List<Task> tasks = new List<Task>();

		while (paths.Count > 0)
        {
            string file = paths.Dequeue();
            tasks.Add(Search(file));
        }
		while (tasks.Any())
        {
            // Wait for any task to complete
            Task finishedTask = await Task.WhenAny(tasks);

            // Remove the completed task from the list
            tasks.Remove(finishedTask);

            // Process the results
			while (queue.TryDequeue(out MetaData data)){
				Console.WriteLine(data.ToString());
			}
        }
        await Task.WhenAll(tasks);

	}
	public async Task Search(string query){
		MetaData spot = await spotify.Search(query);
		MetaData yt = await youtube.Search(query);
		if (spot.Compare(yt)>0.4){
			queue.Enqueue(spot);
		}
	}



    

}
