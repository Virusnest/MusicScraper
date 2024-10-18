using System;
using YouTubeMusicAPI.Models;
namespace MusicScraper
{
	/// <summary>
	/// Simplified MetaData struct for storing metadata
	/// </summary>
	public struct MetaData
	{
		public string Title, Album;
		public string[] Artists;
		public int Year;
		public int TrackNo = 0;
		public byte[] Cover;

		public MetaData(string[] artist, string title = "N/A", string album = "N/A", int year = 0)
		{
			Title = title;
			Artists = artist;
			Album = album;
			Year = year;
			Cover = new byte[0];
		}
		public override string ToString()
		{
			return $"Title: {Title}, Artist: {string.Join(", ", Artists)}, Album: {Album}, Year: {Year}";
		}
		/// <summary>
		/// Compare two MetaData objects
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public float Compare(MetaData data)
		{
			int total = 0;
			if (stringCheck(Title.ToLower(), data.Title.ToLower())) total++;
			if (Artists.Equals(data.Artists)) total++;
			if (stringCheck(Album.ToLower(), data.Album.ToLower())) total++;
			return total / 2f;
		}
		private bool stringCheck(string a, string b)
		{
			if (a == b) return true;
			if (a.Length > b.Length)
			{
				return a.Contains(b);
			}
			else
			{
				return b.Contains(a);
			}
		}
	}
}

