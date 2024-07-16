// See https://aka.ms/new-console-template for more information
using MusicScraper;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
class Program
{
	public static SpotifyClient? client;
	static void Main(string[] args)
	{

		Connect();
		Console.WriteLine(Search("MANESKIN-Beggin").Result.ToString());
		Console.Read();
		
	}
	static void Connect()
	{

		var config = SpotifyClientConfig.CreateDefault();

		var request = new ClientCredentialsRequest("803aa0f3be4c486f8978e42452c0d2a0", "b9fba9ee5cd042549ee801b3603b7f30");
		var response = new OAuthClient(config).RequestToken(request);

		client = new SpotifyClient(config.WithToken(response.Result.AccessToken));
		
	
	}
	public async static Task<MetaData> Search(string wawa)
	{
		var data = new MetaData();
		if (client == null) { return data; }
		SearchRequest trackSearch = new SearchRequest(SearchRequest.Types.Track,wawa);
		SearchRequest artistSearch = new SearchRequest(SearchRequest.Types.Track, wawa);
		SearchRequest albumSearch = new SearchRequest(SearchRequest.Types.Track, wawa);
		var trackResponce = await client.Search.Item(trackSearch);
		if (trackResponce.Tracks.Items != null) {
			data.Title = trackResponce.Tracks.Items[0].Name;
			data.Artist = trackResponce.Tracks.Items[0].Artists[0].Name;
			data.Album = trackResponce.Tracks.Items[0].Album.Name;
			data.Year = Convert.ToInt32(trackResponce.Tracks.Items[0].Album.ReleaseDate.Remove(4));
		}


		return data;
	}

}