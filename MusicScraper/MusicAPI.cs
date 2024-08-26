using System;
namespace MusicScraper
{
	public interface MusicAPI
	{
		public void Login(string secret, string key);
		public Task<MetaData> Search(string query);
	}
}

