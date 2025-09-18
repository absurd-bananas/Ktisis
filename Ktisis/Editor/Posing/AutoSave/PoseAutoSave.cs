// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.AutoSave.PoseAutoSave
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Sections;
using Ktisis.Data.Files;
using Ktisis.Data.Json;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Types;
using Ktisis.Services.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;

#nullable enable
namespace Ktisis.Editor.Posing.AutoSave;

public class PoseAutoSave : IDisposable
{
  private readonly IEditorContext _ctx;
  private readonly IFramework _framework;
  private readonly FormatService _format;
  private readonly Timer _timer = new Timer();
  private readonly Queue<string> _prefixes = new Queue<string>();
  private AutoSaveConfig _cfg;

  private IPosingManager Posing => this._ctx.Posing;

  private ISceneManager Scene => this._ctx.Scene;

  public PoseAutoSave(IEditorContext ctx, IFramework framework, FormatService format)
  {
    this._ctx = ctx;
    this._framework = framework;
    this._format = format;
  }

  public void Initialize(Configuration cfg)
  {
    this._timer.Elapsed += new ElapsedEventHandler(this.OnElapsed);
    this.Configure(cfg);
  }

  public void Configure(Configuration cfg)
  {
    this._cfg = cfg.AutoSave;
    this._timer.Interval = TimeSpan.FromSeconds((long) this._cfg.Interval).TotalMilliseconds;
    if (this._timer.Enabled == this._cfg.Enabled)
      return;
    this._timer.Enabled = this._cfg.Enabled;
  }

  private void OnElapsed(object? sender, ElapsedEventArgs e)
  {
    if (!this.Posing.IsValid)
    {
      this._timer.Stop();
    }
    else
    {
      if (!this._cfg.Enabled || !this.Posing.IsEnabled)
        return;
      this._framework.RunOnFrameworkThread(new Action(this.Save));
    }
  }

  public void Save()
  {
    string str1 = this._format.Replace(this._cfg.FolderFormat);
    string str2 = Path.Combine(this._cfg.FilePath, str1);
    this._prefixes.Enqueue(str1);
    if (!Directory.Exists(str2))
      Directory.CreateDirectory(str2);
    List<CharaEntity> list = this.Scene.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is CharaEntity)).Cast<CharaEntity>().ToList<CharaEntity>();
    if (list.Count == 0)
    {
      Ktisis.Ktisis.Log.Warning("No valid entities, skipping auto save.", Array.Empty<object>());
    }
    else
    {
      Ktisis.Ktisis.Log.Info($"Auto saving poses for {list.Count} character(s)", Array.Empty<object>());
      foreach (CharaEntity charaEntity in list)
      {
        if (charaEntity.Pose != null)
        {
          int num = 1;
          string str3;
          string str4;
          string str5;
          for (str3 = Path.Combine(str2, charaEntity.Name + ".pose"); Path.Exists(str3); str3 = Path.Combine(str4, str5))
          {
            str4 = str2;
            str5 = $"{charaEntity.Name} ({++num}).pose";
          }
          JsonFileSerializer jsonFileSerializer = new JsonFileSerializer();
          PoseFile poseFile = new EntityPoseConverter(charaEntity.Pose).SaveFile();
          File.WriteAllText(str3, jsonFileSerializer.Serialize((object) poseFile));
        }
      }
      Ktisis.Ktisis.Log.Verbose($"Prefix count: {this._prefixes.Count} max: {this._cfg.Count}", Array.Empty<object>());
      while (this._prefixes.Count > this._cfg.Count)
        this.DeleteOldest();
    }
  }

  private void DeleteOldest()
  {
    string str = Path.Combine(this._cfg.FilePath, this._prefixes.Dequeue());
    if (Directory.Exists(str))
    {
      Ktisis.Ktisis.Log.Verbose("Deleting " + str, Array.Empty<object>());
      Directory.Delete(str, true);
    }
    PoseAutoSave.DeleteEmptyDirs(this._cfg.FilePath);
  }

  private static void DeleteEmptyDirs(string dir)
  {
    if (StringExtensions.IsNullOrEmpty(dir))
      throw new ArgumentException("Starting directory is a null reference or empty string", nameof (dir));
    try
    {
      foreach (string enumerateDirectory in Directory.EnumerateDirectories(dir))
        PoseAutoSave.DeleteEmptyDirs(enumerateDirectory);
      if (Directory.EnumerateFileSystemEntries(dir).Any<string>())
        return;
      try
      {
        Directory.Delete(dir);
      }
      catch (DirectoryNotFoundException ex)
      {
      }
    }
    catch (UnauthorizedAccessException ex)
    {
      Ktisis.Ktisis.Log.Error(ex.ToString(), Array.Empty<object>());
    }
  }

  private void Clear()
  {
    try
    {
      while (this._cfg.ClearOnExit && this._prefixes.Count > 0)
        this.DeleteOldest();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to clear auto saves:\n{ex}", Array.Empty<object>());
    }
  }

  public void Dispose()
  {
    this._timer.Elapsed -= new ElapsedEventHandler(this.OnElapsed);
    this._timer.Stop();
    ((Component) this._timer).Dispose();
    this.Clear();
    GC.SuppressFinalize((object) this);
  }
}
