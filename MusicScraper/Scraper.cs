using System;
using System.Collections.Concurrent;
using TagLib;

namespace MusicScraper;

/// <summary>
/// Scraper class for scraping metadata from web APIs
/// </summary>
public class Scraper
{
    public List<string> Files = new List<string>();
    public Spotify spotify = new Spotify();
    public YTMusic youtube = new YTMusic();

    private ConcurrentQueue<QueryData> queue = new ConcurrentQueue<QueryData>();

    public Scraper(Action<(Spotify, YTMusic)> connect)
    {
        connect((spotify, youtube));
    }
    /// <summary>
    /// Recursivly all files in a directory and its subdirectories and add them to the Files list
    /// </summary>
    /// <param name="path"></param>
    public void GetFiles(string path)
    {
        foreach (var asset in Directory.GetFiles(path, "*.mp3"))
        {
            Console.WriteLine(asset);

            Files.Add((asset));
        }
        foreach (var asset in Directory.GetDirectories(path))
        {
            GetFiles(asset);
        }
    }
    /// <summary>
    /// Writes Metadata to a file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public void WriteMetadata(string path, MetaData data)
    {
        var wawa = TagLib.File.Create(path);
        wawa.Tag.Album = data.Album;
        wawa.Tag.AlbumArtists = data.Artists;
        wawa.Tag.Title = data.Title;
        wawa.Tag.Year = (uint)data.Year;
        wawa.Tag.Track = (uint)data.TrackNo;

        TagLib.Id3v2.AttachmentFrame cover = new TagLib.Id3v2.AttachmentFrame
        {
            Type = PictureType.FrontCover,
            Description = "Cover",
            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
            Data = data.Cover,
            TextEncoding = StringType.UTF16

        };
        wawa.Tag.Pictures = new IPicture[] { cover };
        wawa.Save();
    }
    public struct QueryData
    {
        public QueryData(MetaData sdata, MetaData ytData, string path, string query)
        {
            SpotData = sdata;
            YTData = ytData;
            Path = path;
            Query = query;
        }

        public MetaData SpotData;
        public MetaData YTData;

        public string Path;
        public string Query;
    }
    /// <summary>
    /// Show the user a UI to confirm metadata
    /// </summary>
    /// <param name="data"></param>
    public void ServeConfidinceUI(ref QueryData data)
    {
        Console.WriteLine("Unsure about the following file: " + data.Path);
        Console.WriteLine("Title: " + data.SpotData.Title);
        Console.WriteLine("Artist: " + string.Join(", ", data.SpotData.Artists));
        Console.WriteLine("Album: " + data.SpotData.Album);
        Console.WriteLine("Year: " + data.SpotData.Year);
        Console.WriteLine("Confidence: " + data.SpotData.Compare(data.YTData));
        Console.WriteLine("(Y) to change metadata and write to file, (N) to keep and write to file, (S) to skip");
        while (true)
        {
            int response = Console.ReadKey().KeyChar;
            Console.CursorLeft = 0;
            if (response == 'y')
            {
                Console.WriteLine("Enter the new Title: ");
                string? title = Console.ReadLine();
                Console.WriteLine("Enter the new Artist(s): ");
                string? artist = Console.ReadLine();
                Console.WriteLine("Enter the new Album: ");
                string? album = Console.ReadLine();
                Console.WriteLine("Enter the new Year: ");
                string? year = Console.ReadLine();
                if (title != null && artist != null && album != null && year != null)
                {
                    data.SpotData = new MetaData(artist.Split(","), title, album, int.Parse(year));
                    Console.WriteLine("Are you sure you want to change the metadata? (y/n)");
                    response = Console.Read();
                    if (response == 'y')
                    {
                        WriteMetadata(data.Path, data.SpotData);
                        break;
                    }
                }
            }
            else if (response == 'n')
            {
                Console.WriteLine(data.Path);
                WriteMetadata(data.Path, data.SpotData);
                break;
            }
            else if (response == 's')
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid Input");
            }
        }

    }



    /// <summary>
    /// Scrape metadata from the web
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public async Task Scrape(Queue<string> paths)
    {
        List<Task> tasks = new List<Task>();

        // Queue all tasks
        while (paths.Count > 0)
        {
            string file = paths.Dequeue();
            tasks.Add(Search(Path.GetFileName(file), file));
        }

        // Continue processing while there are still tasks to complete
        while (tasks.Any())
        {
            // Wait for any task to complete
            Task finishedTask = await Task.WhenAny(tasks);

            // Remove the completed task from the list
            tasks.Remove(finishedTask);

            // Process available results from the queue (while waiting for others to complete)
            QueryData data;
            while (queue.TryDequeue(out data))
            {
                Console.WriteLine(data.ToString());
                ServeConfidinceUI(ref data); // Update the UI with the result
            }
        }

        // Ensure all remaining tasks are completed (if any are left in the background)
        await Task.WhenAll(tasks);

        // Process any remaining results that came in after the last task finished
        while (queue.TryDequeue(out QueryData remainingData))
        {
            Console.WriteLine(remainingData.ToString());
            ServeConfidinceUI(ref remainingData);
        }



    }
    /// <summary>
    /// asynchronously call the search function on the web APIs
    /// </summary>
    /// <param name="query"></param>
    public async Task Search(string query, string path)
    {
        MetaData spot = await spotify.Search(query);
        MetaData yt = await youtube.Search(query);
        queue.Enqueue(new QueryData(spot, yt, path, "Spotify"));
    }





}
