using System;

namespace MusicScraper;

public interface MusicAPI
{
  public Task<MetaData> Search(string query);

  public void Login(string key, string secret);
}
