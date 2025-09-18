// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Environment.WeatherService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Core.Attributes;
using Ktisis.Structs.Env;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Services.Environment;

[Singleton]
public class WeatherService
{
  private readonly IDataManager _data;
  private readonly IFramework _framework;
  private readonly ITextureProvider _texture;

  public WeatherService(IDataManager data, IFramework framework, ITextureProvider texture)
  {
    this._data = data;
    this._framework = framework;
    this._texture = texture;
  }

  public async Task<IEnumerable<WeatherInfo>> GetWeatherTypes(CancellationToken token)
  {
    WeatherService weatherService = this;
    await Task.Yield();
    List<WeatherInfo> results = new List<WeatherInfo>();
    Task<byte[]> task = weatherService._framework.RunOnFrameworkThread<byte[]>(new Func<byte[]>(weatherService.GetEnvWeatherIds));
    ExcelSheet<Weather> weatherSheet = weatherService._data.GetExcelSheet<Weather>(new ClientLanguage?(), (string) null);
    foreach (byte num in await task)
    {
      if (!token.IsCancellationRequested)
      {
        if (weatherSheet.HasRow((uint) num))
        {
          Weather row = weatherSheet.GetRow((uint) num);
          ITextureProvider texture = weatherService._texture;
          GameIconLookup gameIconLookup = GameIconLookup.op_Implicit((uint) ((Weather) ref row).Icon);
          ref GameIconLookup local = ref gameIconLookup;
          ISharedImmediateTexture fromGameIcon = texture.GetFromGameIcon(ref local);
          results.Add(new WeatherInfo(row, fromGameIcon));
        }
      }
      else
        break;
    }
    token.ThrowIfCancellationRequested();
    IEnumerable<WeatherInfo> weatherTypes = (IEnumerable<WeatherInfo>) results;
    results = (List<WeatherInfo>) null;
    weatherSheet = (ExcelSheet<Weather>) null;
    return weatherTypes;
  }

  public unsafe byte[] GetEnvWeatherIds()
  {
    EnvManagerEx* envManagerExPtr = EnvManagerEx.Instance();
    EnvScene* envScene = (IntPtr) envManagerExPtr != IntPtr.Zero ? envManagerExPtr->_base.EnvScene : (EnvScene*) null;
    return (IntPtr) envScene == IntPtr.Zero ? Array.Empty<byte>() : MemoryExtensions.TrimEnd<byte>(((EnvScene) (IntPtr) envScene).WeatherIds, (byte) 0).ToArray();
  }
}
