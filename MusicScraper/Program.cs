// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;
using MusicScraper;
class Program
{
	public static string? ssecret;
	public static string? sid;
	static ManualResetEvent reset = new ManualResetEvent(false);
	public static Scraper scraper;
	public static DirectoryStructure structure;
	private static Queue<string> paths = new Queue<string>();
	static void Main(string[] args)
	{
		LoadENV(".env");
		ssecret = Environment.GetEnvironmentVariable("SPOT_CLIENT_SECRET");
		sid = Environment.GetEnvironmentVariable("SPOT_CLIENT_ID");
		string query = "";
		if (args.Length == 0)
		{
			Console.WriteLine("What Would you like to do?");
			Console.WriteLine("1. Scrape Music");
			Console.WriteLine("2. Build A Libary Folder");
			bool response = false;
			while (!response)
			{
				switch (Console.Read())
				{
					case '1':
						Console.WriteLine("Enter the query you would like to search for: ");
						query = Console.ReadLine();
						response = true;
						query = RequestPath();
						scraper = new Scraper(a => Connect(a));
						scraper.GetFiles(query);
						foreach (var path in scraper.Files)
						{
							paths.Enqueue(path);
							Console.WriteLine(path);
						}
						BeginScrape();
						reset.WaitOne();
						break;
					case '2':
						Console.WriteLine("Enter the query you would like to search for: ");
						query = Console.ReadLine();
						response = true;
						query = RequestPath();
						structure = new DirectoryStructure();
						structure.GetFiles(query);
						structure.ResoloveMetaData();
						structure.BuildStructure();
						break;
				}
				Console.WriteLine("Invalid Input");

			}

		}
		else
		{
			query = args[0];
		}
	}

	public static string RequestPath()
	{
		Console.WriteLine("Enter the path you would like to search for: ");
		string? path;
		while (!Directory.Exists(path = Console.ReadLine()))
		{
			Console.WriteLine("Invalid Path");
		}
		return path;
	}
	public static async void BeginScrape()
	{
		await scraper.Scrape(paths).ContinueWith(a =>
		{
			reset.Set();
		});
	}

	public static void LoadENV(string filePath)
	{
		if (!File.Exists(filePath))
			return;

		foreach (var line in File.ReadAllLines(filePath))
		{
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
	static void Connect((Spotify, YTMusic) apis)
	{
		apis.Item1.Login(ssecret, sid);
		apis.Item2.Login("", "");
	}
}