using System;
namespace MusicScraper
{
	public class MetaData
	{
		public string Title, Artist, Album;
		public int Year;

		public MetaData(string title="N/A", string artist = "N/A", string album = "N/A", int year = 0)
		{
			Title = title;
			Artist = artist;
			Album = album;
			Year = year;
		}
		public override string ToString()
		{
			return $"Title: {Title}, Artist: {Artist}, Album: {Album}, Year: {Year}";
		}
	}
}

