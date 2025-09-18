// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Environment.WeatherInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Interface.Textures;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

#nullable enable
namespace Ktisis.Services.Environment;

public class WeatherInfo
{
  public readonly string Name;
  public readonly uint RowId;
  public readonly ISharedImmediateTexture? Icon;

  public WeatherInfo(Weather row, ISharedImmediateTexture? icon)
  {
    ReadOnlySeString name = ((Weather) ref row).Name;
    string str = ((ReadOnlySeString) ref name).ExtractText();
    if (StringExtensions.IsNullOrEmpty(str))
      str = $"Weather #{((Weather) ref row).RowId}";
    this.Name = str;
    this.RowId = ((Weather) ref row).RowId;
    this.Icon = icon;
  }
}
