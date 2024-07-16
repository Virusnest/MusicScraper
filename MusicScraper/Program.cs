// See https://aka.ms/new-console-template for more information
using MusicScraper;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
class Program
{
	public static SpotifyClient? client;
	public static string? secret;
	public static string? id;


	static void Main(string[] args)
	{
		LoadENV(".env");
		secret = Environment.GetEnvironmentVariable("SPOT_CLIENT_SECRET");
		id = Environment.GetEnvironmentVariable("SPOT_CLIENT_ID");
		Connect();
		string? query = Console.ReadLine();
		Console.WriteLine(Search(PreProcessString(query)).Result);
		Console.Read();
		
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
		return input.Remove(input.LastIndexOf(".mp3"));
	}
	static void Connect()
	{


		var config = SpotifyClientConfig.CreateDefault();

		var request = new ClientCredentialsRequest(id, secret);
		var response = new OAuthClient(config).RequestToken(request);

		client = new SpotifyClient(config.WithToken(response.Result.AccessToken));
		
	
	}
	public async static Task<MetaData> Search(string query)
	{
		var data = new MetaData();
		if (client == null) { return data; }
		SearchRequest trackSearch = new SearchRequest(SearchRequest.Types.Track, query);
		SearchRequest artistSearch = new SearchRequest(SearchRequest.Types.Track, query);
		var trackResponce = await client.Search.Item(trackSearch);
		var artistResponce = await client.Search.Item(trackSearch);
		//Console.WriteLine(ConfidenceRate(query, trackResponce.Tracks.Items.ToArray(), artistResponce.Artists.Items.ToArray()));
		//if (ConfidenceRate(query, trackResponce.Tracks.Items.ToArray(), artistResponce.Artists.Items.ToArray()) > 0) {
			if (trackResponce.Tracks.Items != null) {
				data.Title = trackResponce.Tracks.Items[0].Name;
				data.Artist = trackResponce.Tracks.Items[0].Artists[0].Name;
				data.Album = trackResponce.Tracks.Items[0].Album.Name;
				data.Year = Convert.ToInt32(trackResponce.Tracks.Items[0].Album.ReleaseDate.Remove(4));
			}
		//}
		return data;
	}

	public static int ConfidenceRate(string query, FullTrack[] tracks, FullArtist[] artists,int atempt)
	{
		int condfidence=0;
		var artistAsplit = artists[atempt].Name.ToLower().Split(" ");
		var artistBsplit = tracks[atempt].Artists[0].Name.ToLower().Split(" ");
		var trackSplit = tracks[atempt].Name.ToLower().Split("");
		foreach(string check in artistAsplit) {
			
		}
		if (query.Contains(tracks[atempt].Artists[atempt].Name)) condfidence++;
		if (query.ToLower().Replace(" ", "").Contains(artists[atempt].Name.ToLower().Replace(" ",""))) condfidence++;
		if (query.Contains(tracks[atempt].Name)) condfidence++;
		return condfidence;
	}
	

}