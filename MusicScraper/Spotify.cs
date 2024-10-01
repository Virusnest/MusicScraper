using System;
using SpotifyAPI.Web;

namespace MusicScraper
{
	public class Spotify : IMusicAPI
	{
		public SpotifyClient? Client;
		public Spotify()
		{
		}

		public void Login(string secret, string key)
		{

			var config = SpotifyClientConfig.CreateDefault();

			var request = new ClientCredentialsRequest(key, secret);
			var response = new OAuthClient(config).RequestToken(request);

			Client = new SpotifyClient(config.WithToken(response.Result.AccessToken));
		}

		public async Task<MetaData> Search(string query)
		{
			var missing = new MetaData(new string[] { "N/A",});
			if (Client == null) { return missing; }
			SearchRequest trackSearch = new SearchRequest(SearchRequest.Types.Track, query);
			var trackResponce = await Client.Search.Item(trackSearch);
			if (trackResponce.Tracks.Items == null) { return missing;}
			if (trackResponce.Tracks.Items.Count == 0) { return missing;}
            var data = new MetaData(trackResponce.Tracks.Items.First().Artists.Select(s => s.Name).ToArray())
            {
                Title = trackResponce.Tracks.Items.First().Name,
                Album = trackResponce.Tracks.Items.First().Album.Name,
                Year = Convert.ToInt32(trackResponce.Tracks.Items.First().Album.ReleaseDate.Remove(4))
            };

            return data;
		}
	}
}

