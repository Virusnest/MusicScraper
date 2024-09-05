using System;
namespace MusicScraper
{
	/// <summary>
	/// Interface for an API that can be used to search for music metadata
	/// </summary>
	public interface MusicAPI
	{
		/// <summary>
		/// Login to the API
		/// </summary>
		/// <param name="secret"></param>
		/// <param name="key"></param>
		public void Login(string secret, string key);
		/// <summary>
		/// Search for metadata
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Task<MetaData> Search(string query);
	}
}

