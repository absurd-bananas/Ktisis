// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.FormatService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable
namespace Ktisis.Services.Data;

[Singleton]
public class FormatService
{
  private readonly IClientState _client;
  private readonly IDataManager _data;
  private readonly List<string> ReplacerKeys;

  public FormatService(IClientState client, IDataManager data)
  {
    int capacity = 9;
    List<string> stringList = new List<string>(capacity);
    CollectionsMarshal.SetCount<string>(stringList, capacity);
    Span<string> span = CollectionsMarshal.AsSpan<string>(stringList);
    int num1 = 0;
    span[num1] = "%Date%";
    int num2 = num1 + 1;
    span[num2] = "%Year%";
    int num3 = num2 + 1;
    span[num3] = "%Month%";
    int num4 = num3 + 1;
    span[num4] = "%Day%";
    int num5 = num4 + 1;
    span[num5] = "%Time%";
    int num6 = num5 + 1;
    span[num6] = "%PlayerName%";
    int num7 = num6 + 1;
    span[num7] = "%CurrentWorld%";
    int num8 = num7 + 1;
    span[num8] = "%HomeWorld%";
    int num9 = num8 + 1;
    span[num9] = "%Zone%";
    this.ReplacerKeys = stringList;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this._client = client;
    this._data = data;
  }

  public string Replace(string path)
  {
    using (IEnumerator<string> enumerator = this.ReplacerKeys.Where<string>(new Func<string, bool>(path.Contains)).GetEnumerator())
    {
label_4:
      if (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        string keyReplacement = this.GetKeyReplacement(current);
        do
        {
          path = path.Replace(current, keyReplacement, true, (CultureInfo) null);
        }
        while (path.Contains(current));
        goto label_4;
      }
    }
    return path;
  }

  public Dictionary<string, string> GetReplacements()
  {
    return this.ReplacerKeys.ToDictionary<string, string, string>((Func<string, string>) (key => key), new Func<string, string>(this.GetKeyReplacement));
  }

  private string GetKeyReplacement(string key)
  {
    string keyReplacement;
    if (key != null)
    {
      switch (key.Length)
      {
        case 5:
          if (key == "%Day%")
          {
            keyReplacement = DateTime.Now.ToString("dd");
            goto label_22;
          }
          break;
        case 6:
          switch (key[1])
          {
            case 'D':
              if (key == "%Date%")
              {
                keyReplacement = DateTime.Now.ToString("yyyy-MM-dd");
                goto label_22;
              }
              break;
            case 'T':
              if (key == "%Time%")
              {
                keyReplacement = DateTime.Now.ToString("hh-mm-ss");
                goto label_22;
              }
              break;
            case 'Y':
              if (key == "%Year%")
              {
                keyReplacement = DateTime.Now.ToString("yyyy");
                goto label_22;
              }
              break;
            case 'Z':
              if (key == "%Zone%")
              {
                keyReplacement = this.GetZone();
                goto label_22;
              }
              break;
          }
          break;
        case 7:
          if (key == "%Month%")
          {
            keyReplacement = DateTime.Now.ToString("MM");
            goto label_22;
          }
          break;
        case 11:
          if (key == "%HomeWorld%")
          {
            keyReplacement = this.GetHomeWorld();
            goto label_22;
          }
          break;
        case 12:
          if (key == "%PlayerName%")
          {
            keyReplacement = this.GetPlayerName();
            goto label_22;
          }
          break;
        case 14:
          if (key == "%CurrentWorld%")
          {
            keyReplacement = this.GetCurrentWorld();
            goto label_22;
          }
          break;
      }
    }
    keyReplacement = string.Empty;
label_22:
    return keyReplacement;
  }

  private string GetPlayerName()
  {
    return ((IGameObject) this._client.LocalPlayer)?.Name.ToString() ?? "Unknown";
  }

  private string GetCurrentWorld()
  {
    IPlayerCharacter localPlayer = this._client.LocalPlayer;
    string str;
    if (localPlayer == null)
    {
      str = (string) null;
    }
    else
    {
      World world = localPlayer.CurrentWorld.Value;
      str = ((World) ref world).Name.ToString();
    }
    return str ?? "Unknown";
  }

  private string GetHomeWorld()
  {
    IPlayerCharacter localPlayer = this._client.LocalPlayer;
    string str;
    if (localPlayer == null)
    {
      str = (string) null;
    }
    else
    {
      World world = localPlayer.HomeWorld.Value;
      str = ((World) ref world).Name.ToString();
    }
    return str ?? "Unknown";
  }

  private string GetZone()
  {
    ushort territoryType = this._client.TerritoryType;
    ExcelSheet<TerritoryType> excelSheet = this._data.GetExcelSheet<TerritoryType>(new ClientLanguage?(), (string) null);
    if (excelSheet.HasRow((uint) territoryType))
    {
      TerritoryType row = excelSheet.GetRow((uint) territoryType);
      RowRef<PlaceName> placeName1 = ((TerritoryType) ref row).PlaceName;
      if (placeName1.IsValid)
      {
        PlaceName placeName2 = placeName1.Value;
        ReadOnlySeString name = ((PlaceName) ref placeName2).Name;
        return ((ReadOnlySeString) ref name).ExtractText();
      }
    }
    return "Unknown";
  }
}
