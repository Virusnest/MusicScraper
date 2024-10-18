using System;
using NVBS;
using NVBS.Structure;

namespace MusicScraper
{
  /// <summary>
  /// Satatic class for storing settings
  /// </summary>
  public static class Settings
  {
    public const string PATH = "config.nvbs";
    public static bool AgressiveCleaning = false;
    public static bool ConfidenceChecks = true;
    public static bool PreferSpotify = true;
    public static bool PreferLocal = true;
    public static bool PreferMainArtist = true;
    public static string StructurePattern = "{Artist[0]}/{Artist}/{Album}/{Artist} - {Title}{ext}";
    public static string SpotifyClientID = "";
    public static string SpotifyClientSecret = "";

    public static string Location = AppContext.BaseDirectory;

    /// <summary>
    /// Save settings to PATH
    /// </summary>
    public static void SaveSettings()
    {
      // Save settings to file
      NVBSMap config = new NVBSMap() {
        { "AgressiveCleaning", (NVBSByte)AgressiveCleaning },
        { "ConfidenceChecks", (NVBSByte)ConfidenceChecks },
        { "PreferSpotify", (NVBSByte)PreferSpotify },
        { "PreferLocal", (NVBSByte)PreferLocal },
        { "PreferMainArtist", (NVBSByte)PreferMainArtist },
        { "StructurePattern", (NVBSString)StructurePattern },
        { "SpotifyClientID", (NVBSString)SpotifyClientID },
        { "SpotifyClientSecret", (NVBSString)SpotifyClientSecret }
      };
      FileStream stream = File.Open(Path.Combine(Location, PATH), FileMode.OpenOrCreate);
      NVBSWriter writer = new NVBSWriter(new BinaryWriter(stream));
      writer.Write(config);
      stream.Close();
    }

    /// <summary>
    /// Load settings from PATH
    /// </summary>
    public static void LoadSettings()
    {
      if (File.Exists(Path.Combine(Location, PATH)))
      {
        FileStream stream = File.Open(Path.Combine(Location, PATH), FileMode.Open);
        NVBSReader reader = new NVBSReader(new BinaryReader(stream));
        NVBSMap config = reader.Read();
        stream.Close();
        AgressiveCleaning = config["AgressiveCleaning"].AsBool();
        ConfidenceChecks = config["ConfidenceChecks"].AsBool();
        PreferSpotify = config["PreferSpotify"].AsBool();
        PreferLocal = config["PreferLocal"].AsBool();
        PreferMainArtist = config["PreferMainArtist"].AsBool();
        StructurePattern = config["StructurePattern"].AsString();
        SpotifyClientID = config["SpotifyClientID"].AsString();
        SpotifyClientSecret = config["SpotifyClientSecret"].AsString();
      }
      else
      {
        SaveSettings();
      }
    }

    /// <summary>
    /// Serve the settings UI
    /// </summary>
    public static void ServeSettingsUI()
    {
      Console.WriteLine("Agressive Cleaning: " + AgressiveCleaning);
      Console.WriteLine("Confidence Checks: " + ConfidenceChecks);
      Console.WriteLine("Prefer Spotify: " + PreferSpotify);
      Console.WriteLine("Prefer Local: " + PreferLocal);
      Console.WriteLine("Prefer Main Artist: " + PreferMainArtist);
      Console.WriteLine("Structure Pattern: " + StructurePattern);
      Console.WriteLine("Spotify Client ID: " + SpotifyClientID);
      Console.WriteLine("Spotify Client Secret: " + SpotifyClientSecret);
      Console.WriteLine("Would you like to change these settings? (y/n)");
      if (Console.ReadKey().Key == ConsoleKey.Y)
      {
        Console.CursorLeft = 0;
        string read;
        SetSetting("Agressive Cleaning", ref AgressiveCleaning, s => s.ToLower() == "y");
        SetSetting("Confidence Checks", ref ConfidenceChecks, s => s.ToLower() == "y");
        SetSetting("Prefer Spotify", ref PreferSpotify, s => s.ToLower() == "y");
        SetSetting("Prefer Local", ref PreferLocal, s => s.ToLower() == "y");
        SetSetting("Prefer Main Artist", ref PreferMainArtist, s => s.ToLower() == "y");
        SetSetting("Structure Pattern", ref StructurePattern, s => s);
        SetSetting("Spotify Client ID", ref SpotifyClientID, s => s);
        SetSetting("Spotify Client Secret", ref SpotifyClientSecret, s => s);
        SaveSettings();
        Environment.Exit(0);
      }

      Console.CursorLeft = 0;
    }

    private static void SetSetting<T>(string name, ref T var, Func<string, T> parse)
    {
      Console.WriteLine(name + ": ");
      string read;
      if ((read = Console.ReadLine().Trim()) != "")
      {
        var = parse(read.Trim());
      }
    }
  }
}