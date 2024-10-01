using System;

namespace MusicScraper;

public interface IMusicAPI
{
  public Task<MetaData> Search(string query);

  public void Login(string key, string secret);
}
