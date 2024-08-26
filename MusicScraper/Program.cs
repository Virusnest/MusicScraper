// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using FuzzySharp;
using MusicScraper;
using SpotifyAPI.Web;
class Program
{
	public static string? ssecret;
	public static string? sid;
	static ManualResetEvent reset = new ManualResetEvent(false);
	public static Scraper scraper;
	private static Queue<string> paths = new Queue<string>();
	static void Main(string[] args)
	{
		LoadENV(".env");
		ssecret = Environment.GetEnvironmentVariable("SPOT_CLIENT_SECRET");
		sid = Environment.GetEnvironmentVariable("SPOT_CLIENT_ID");
		string query="";
		if(args.Length==0){
			Console.WriteLine("Please enter a Path");
			while(!Directory.Exists(query)){
				query=Console.ReadLine();
			}
		}else{
			query=args[0];
		}
		scraper = new Scraper(a=>Connect(a));
		Console.WriteLine(query);
		scraper.GetFiles(query);
		foreach(var path in scraper.Files){
			paths.Enqueue(path);
			Console.WriteLine(path);
		}
		BeginScrape();
		reset.WaitOne();
	}
	public static async void BeginScrape()
	{
		await scraper.Scrape(paths).ContinueWith(a => {
			reset.Set();
		});
	}
	public static void LoadENV(string filePath)
	{
		if (!File.Exists(filePath))
			return;

		foreach (var line in File.ReadAllLines(filePath)) {
			var parts = line.Split(
				'=',
				StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length != 2)
				continue;

			Environment.SetEnvironmentVariable(parts[0], parts[1]);
		}
	}

	public static string PreProcessString(string input)
	{
		if (input.Contains(".mp3"))
			input = input.Remove(input.LastIndexOf(".mp3"));
		//magic regex garbage that seperates normal charactures from special chars
		string[] output = Regex.Matches(input, @"[\p{L}\p{N}]+").Cast<Match>().Select(x => x.Value).ToArray();
		//recombine into clean string
		return string.Join(" ", output);
	}
	static void Connect((Spotify,YTMusic) apis)
	{
		apis.Item1.Login(ssecret,sid);
		apis.Item2.Login("","");
	}
}