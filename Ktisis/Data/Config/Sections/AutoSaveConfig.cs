// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.AutoSaveConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.IO;

#nullable enable
namespace Ktisis.Data.Config.Sections;

public class AutoSaveConfig
{
  public bool Enabled;
  public int Interval = 60;
  public int Count = 5;
  public string FilePath = Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder) 5), "Ktisis", "PoseAutoBackup");
  public string FolderFormat = "AutoSave - %Date% %Time%";
  public bool ClearOnExit;
  public bool OnDisconnect = true;
}
