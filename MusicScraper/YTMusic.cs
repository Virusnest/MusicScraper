using YouTubeMusicAPI.Client;
using YouTubeMusicAPI.Models;

namespace MusicScraper
{
	public class YTMusic : MusicAPI
	{
		public YouTubeMusicClient ?Client;

		public void Login(string secret, string key)
		{
			Client = new YouTubeMusicClient();
		}

		public async Task<MetaData> Search(string query)
		{
			var missing = new MetaData(new string[] {"N/A"});
			if (Client==null) return missing;
			try{
				IEnumerable<Song> searchResults = await Client.SearchAsync<Song>(query);
				Song song = searchResults.First();
				MetaData data = new(
					song.Artists.Select(s => s.Name).ToArray(),
					song.Name,
					song.Album.Name
				);
				return data;
			}
			catch{
				return missing;
			}

		}
	}
}

