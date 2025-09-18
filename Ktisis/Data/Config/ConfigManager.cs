// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.ConfigManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Sections;
using Ktisis.Data.Serialization;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

#nullable enable
namespace Ktisis.Data.Config;

[Singleton]
public class ConfigManager : IDisposable
{
  private readonly IDalamudPluginInterface _dpi;
  private bool _isLoaded;
  private bool _isDisposing;

  public Configuration File { get; private set; }

  public event OnConfigSaved? OnSaved;

  public ConfigManager(IDalamudPluginInterface dpi) => this._dpi = dpi;

  public void Load()
  {
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    Configuration cfg = (Configuration) null;
    try
    {
      cfg = this.OpenConfigFile();
      if (cfg != null)
      {
        if (cfg.Version < 10)
        {
          cfg.Version = 10;
          this.MigrateSchema(cfg);
        }
      }
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to load configuration:\n{ex}", Array.Empty<object>());
    }
    try
    {
      if (cfg == null)
        cfg = this.CreateDefault();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to create default configuration:\n{ex}", Array.Empty<object>());
      throw;
    }
    this.File = cfg;
    this._isLoaded = true;
    stopwatch.Stop();
    Ktisis.Ktisis.Log.Debug($"Configuration loaded in {stopwatch.Elapsed.TotalMilliseconds:0.00}ms", Array.Empty<object>());
  }

  public void Save()
  {
    if (!this._isLoaded)
      return;
    try
    {
      this.SaveConfigFile();
      if (this._isDisposing)
        return;
      OnConfigSaved onSaved = this.OnSaved;
      if (onSaved == null)
        return;
      onSaved(this.File);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to save configuration:\n{ex}", Array.Empty<object>());
    }
  }

  private void MigrateSchema(Configuration cfg)
  {
    Ktisis.Ktisis.Log.Debug("Updating category schema.", Array.Empty<object>());
    CategoryConfig categoryConfig = SchemaReader.ReadCategories();
    foreach (BoneCategory category in categoryConfig.CategoryList)
    {
      BoneCategory byName = cfg.Categories.GetByName(category.Name);
      if (byName != null)
      {
        category.BoneColor = byName.BoneColor;
        category.GroupColor = byName.GroupColor;
        category.LinkedColors = byName.LinkedColors;
      }
    }
    cfg.Categories = categoryConfig;
  }

  public bool GetConfigFileExists() => Path.Exists(this.GetConfigFilePath());

  private Configuration? OpenConfigFile()
  {
    Ktisis.Ktisis.Log.Verbose("Loading configuration...", Array.Empty<object>());
    string configFilePath = this.GetConfigFilePath();
    return !Path.Exists(configFilePath) ? (Configuration) null : JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(configFilePath));
  }

  private void SaveConfigFile()
  {
    Ktisis.Ktisis.Log.Verbose("Saving configuration...", Array.Empty<object>());
    System.IO.File.WriteAllText(this.GetConfigFilePath(), JsonConvert.SerializeObject((object) this.File, (Formatting) 1));
  }

  private string GetConfigFilePath()
  {
    return Path.Join(this._dpi.GetPluginConfigDirectory(), "KtisisV3.json");
  }

  private Configuration CreateDefault()
  {
    return new Configuration()
    {
      Categories = SchemaReader.ReadCategories()
    };
  }

  public void Dispose()
  {
    this._isDisposing = true;
    this.Save();
    GC.SuppressFinalize((object) this);
  }
}
