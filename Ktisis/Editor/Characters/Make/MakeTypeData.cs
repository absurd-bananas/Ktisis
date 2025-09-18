// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Make.MakeTypeData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using Ktisis.GameData.Chara;
using Ktisis.GameData.Excel;
using Ktisis.Services.Data;
using Ktisis.Structs.Characters;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Editor.Characters.Make;

public class MakeTypeData
{
  private readonly Dictionary<(Tribe, Gender), MakeTypeRace> MakeTypes = new Dictionary<(Tribe, Gender), MakeTypeRace>();
  private CommonColors Colors = new CommonColors();

  public MakeTypeRace? GetData(Tribe tribe, Gender gender)
  {
    lock (this.MakeTypes)
      return CollectionExtensions.GetValueOrDefault<(Tribe, Gender), MakeTypeRace>((IReadOnlyDictionary<(Tribe, Gender), MakeTypeRace>) this.MakeTypes, (tribe, gender));
  }

  public async Task Build(IDataManager data, CustomizeService discover)
  {
    Stopwatch stop = new Stopwatch();
    stop.Start();
    await this.BuildMakeType(data);
    Ktisis.Ktisis.Log.Debug($"Built MakeType data in {stop.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
    \u003C\u003Ey__InlineArray2<Task> buffer = new \u003C\u003Ey__InlineArray2<Task>();
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task>, Task>(ref buffer, 0) = this.PopulateDiscoveryData(discover);
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task>, Task>(ref buffer, 1) = this.BuildColors(data);
    // ISSUE: reference to a compiler-generated method
    await Task.WhenAll(\u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray2<Task>, Task>(in buffer, 2));
    stop.Stop();
    Ktisis.Ktisis.Log.Debug($"Total {stop.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
    stop = (Stopwatch) null;
  }

  private async Task BuildMakeType(IDataManager data)
  {
    await Task.Yield();
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    foreach (CharaMakeType row in data.GetExcelSheet<CharaMakeType>(new ClientLanguage?(), (string) null))
      this.BuildRowCustomize(row);
    IPluginLog log1 = Ktisis.Ktisis.Log;
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
    interpolatedStringHandler.AppendLiteral("Built customize data in ");
    ref DefaultInterpolatedStringHandler local1 = ref interpolatedStringHandler;
    TimeSpan elapsed = stopwatch.Elapsed;
    double totalMilliseconds1 = elapsed.TotalMilliseconds;
    local1.AppendFormatted<double>(totalMilliseconds1, "00.00");
    interpolatedStringHandler.AppendLiteral("ms");
    string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
    object[] objArray1 = Array.Empty<object>();
    log1.Debug(stringAndClear1, objArray1);
    stopwatch.Restart();
    this.PopulateCustomizeIcons(data);
    stopwatch.Stop();
    IPluginLog log2 = Ktisis.Ktisis.Log;
    interpolatedStringHandler = new DefaultInterpolatedStringHandler(31 /*0x1F*/, 1);
    interpolatedStringHandler.AppendLiteral("Populated customize icons in ");
    ref DefaultInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    elapsed = stopwatch.Elapsed;
    double totalMilliseconds2 = elapsed.TotalMilliseconds;
    local2.AppendFormatted<double>(totalMilliseconds2, "00.00");
    interpolatedStringHandler.AppendLiteral("ms");
    string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
    object[] objArray2 = Array.Empty<object>();
    log2.Debug(stringAndClear2, objArray2);
  }

  public uint[] GetColors(CustomizeIndex index)
  {
    switch (index - 9)
    {
      case 0:
      case 6:
        return this.Colors.EyeColors;
      case 1:
      case 3:
      case 5:
        throw new Exception($"Invalid index {index} for color lookup.");
      case 2:
        return this.Colors.HighlightColors;
      case 4:
        return this.Colors.FaceFeatureColors;
      default:
        if (index == 20)
          return this.Colors.LipColors;
        if (index == 25)
          return this.Colors.FacepaintColors;
        goto case 1;
    }
  }

  public uint[] GetColors(CustomizeIndex index, Tribe tribe, Gender gender)
  {
    return index == 8 ? this.GetData(tribe, gender)?.Colors.SkinColors ?? Array.Empty<uint>() : (index == 10 ? this.GetData(tribe, gender)?.Colors.HairColors ?? Array.Empty<uint>() : this.GetColors(index));
  }

  private void BuildRowCustomize(CharaMakeType row)
  {
    Tribe rowId = (Tribe) row.Tribe.RowId;
    Gender gender = (Gender) row.Gender;
    MakeTypeRace data = new MakeTypeRace(rowId, gender);
    lock (this.MakeTypes)
      this.MakeTypes[(rowId, gender)] = data;
    foreach (CharaMakeType.CharaMakeStructStruct feature in ((IEnumerable<CharaMakeType.CharaMakeStructStruct>) row.CharaMakeStruct).Where<CharaMakeType.CharaMakeStructStruct>((Func<CharaMakeType.CharaMakeStructStruct, bool>) (make => make.Customize > 0U)))
    {
      CustomizeIndex customize1 = (CustomizeIndex) (int) feature.Customize;
      if (customize1 != 12 || !data.Customize.ContainsKey(customize1))
      {
        bool isCustomize = feature.SubMenuType == (byte) 1 && feature.SubMenuNum > (byte) 10;
        IEnumerable<MakeTypeParam> source = MakeTypeData.BuildParamData(customize1, feature, isCustomize);
        Dictionary<CustomizeIndex, MakeTypeFeature> customize2 = data.Customize;
        CustomizeIndex key = customize1;
        MakeTypeFeature makeTypeFeature = new MakeTypeFeature();
        RowRef<Lobby> menu = feature.Menu;
        string str;
        if (!menu.IsValid)
        {
          str = string.Empty;
        }
        else
        {
          menu = feature.Menu;
          Lobby lobby = menu.Value;
          ReadOnlySeString text = ((Lobby) ref lobby).Text;
          str = ((ReadOnlySeString) ref text).ExtractText();
        }
        makeTypeFeature.Name = str;
        makeTypeFeature.Index = customize1;
        makeTypeFeature.Params = source.ToArray<MakeTypeParam>();
        makeTypeFeature.IsCustomize = isCustomize;
        makeTypeFeature.IsIcon = feature.SubMenuType == (byte) 1;
        customize2[key] = makeTypeFeature;
      }
    }
    MakeTypeData.BuildRowFaceFeatures(row, data);
  }

  private static IEnumerable<MakeTypeParam> BuildParamData(
    CustomizeIndex index,
    CharaMakeType.CharaMakeStructStruct feature,
    bool isCustomize)
  {
    if (feature.SubMenuType <= (byte) 1)
    {
      int num1 = !isCustomize || index != 24 ? 0 : 1;
      int len = isCustomize ? num1 + 1 : (int) feature.SubMenuNum;
      for (int i = num1; i < len; ++i)
      {
        byte num2 = feature.SubMenuGraphic[i];
        uint num3 = feature.SubMenuParam[i];
        yield return new MakeTypeParam()
        {
          Value = num2,
          Graphic = num3
        };
      }
    }
  }

  private static void BuildRowFaceFeatures(CharaMakeType row, MakeTypeRace data)
  {
    MakeTypeFeature feature = data.GetFeature((CustomizeIndex) 5);
    if (feature == null)
      return;
    int[,] facialFeatureOption = row.FacialFeatureOption;
    for (byte index1 = 0; (int) index1 < feature.Params.Length; ++index1)
    {
      byte key = feature.Params[(int) index1].Value;
      uint[] numArray = new uint[7];
      for (int index2 = 0; index2 < facialFeatureOption.GetLength(1); ++index2)
        numArray[index2] = (uint) facialFeatureOption[(int) index1, index2];
      data.FaceFeatureIcons[key] = numArray;
    }
  }

  private void PopulateCustomizeIcons(IDataManager data)
  {
    ExcelSheet<CharaMakeCustomize> excelSheet = data.GetExcelSheet<CharaMakeCustomize>(new ClientLanguage?(), (string) null);
    IEnumerable<MakeTypeFeature> list;
    lock (this.MakeTypes)
      list = (IEnumerable<MakeTypeFeature>) this.MakeTypes.SelectMany<KeyValuePair<(Tribe, Gender), MakeTypeRace>, MakeTypeFeature>((Func<KeyValuePair<(Tribe, Gender), MakeTypeRace>, IEnumerable<MakeTypeFeature>>) (make => (IEnumerable<MakeTypeFeature>) make.Value.Customize.Values)).Where<MakeTypeFeature>((Func<MakeTypeFeature, bool>) (feat =>
      {
        if (feat != null && feat.IsCustomize)
        {
          MakeTypeParam[] makeTypeParamArray = feat.Params;
          if (makeTypeParamArray != null)
            return makeTypeParamArray.Length > 0;
        }
        return false;
      })).ToList<MakeTypeFeature>();
    foreach (MakeTypeFeature makeTypeFeature in list)
    {
      uint start = makeTypeFeature.Params[0].Graphic - 2U;
      uint count = makeTypeFeature.Index == 6 ? 99U : 49U;
      makeTypeFeature.Params = MakeTypeData.BuildParamFromCustomize(excelSheet, start, count).ToArray<MakeTypeParam>();
    }
  }

  private static IEnumerable<MakeTypeParam> BuildParamFromCustomize(
    ExcelSheet<CharaMakeCustomize> custom,
    uint start,
    uint count)
  {
    for (uint i = start; i < start + count; ++i)
    {
      if (custom.HasRow(i))
      {
        CharaMakeCustomize row = custom.GetRow(i);
        if (((CharaMakeCustomize) ref row).FeatureID != (byte) 0 || ((CharaMakeCustomize) ref row).Icon != 0U)
          yield return new MakeTypeParam()
          {
            Value = ((CharaMakeCustomize) ref row).FeatureID,
            Graphic = ((CharaMakeCustomize) ref row).Icon
          };
      }
    }
  }

  private async Task BuildColors(IDataManager dataMgr)
  {
    await Task.Yield();
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    CharaCmpReader charaCmpReader = CharaCmpReader.Open(dataMgr);
    this.Colors = charaCmpReader.ReadCommon();
    IEnumerable<MakeTypeRace> list;
    lock (this.MakeTypes)
      list = (IEnumerable<MakeTypeRace>) this.MakeTypes.Values.ToList<MakeTypeRace>();
    foreach (MakeTypeRace makeTypeRace in list)
      makeTypeRace.Colors = charaCmpReader.ReadTribeData(makeTypeRace.Tribe, makeTypeRace.Gender);
    stopwatch.Stop();
    Ktisis.Ktisis.Log.Debug($"Built color data in {stopwatch.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
  }

  private async Task PopulateDiscoveryData(CustomizeService discover)
  {
    await Task.Yield();
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    IEnumerable<MakeTypeRace> list;
    lock (this.MakeTypes)
      list = (IEnumerable<MakeTypeRace>) this.MakeTypes.Values.ToList<MakeTypeRace>();
    foreach (MakeTypeRace makeTypeRace in list)
    {
      ushort dataId = discover.CalcDataIdFor(makeTypeRace.Tribe, makeTypeRace.Gender);
      MakeTypeFeature feature1 = makeTypeRace.GetFeature((CustomizeIndex) 5);
      if (feature1 != null)
      {
        IEnumerable<byte> bytes = discover.GetFaceTypes(dataId).Except<byte>(((IEnumerable<MakeTypeParam>) feature1.Params).Select<MakeTypeParam, byte>((Func<MakeTypeParam, byte>) (param => param.Value)));
        bool flag;
        switch (makeTypeRace.Tribe)
        {
          case Tribe.Duskwight:
          case Tribe.Dunesfolk:
          case Tribe.MoonKeeper:
          case Tribe.Hellsguard:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        if (flag)
          bytes = bytes.Except<byte>(((IEnumerable<MakeTypeParam>) feature1.Params).Select<MakeTypeParam, byte>((Func<MakeTypeParam, byte>) (param => (byte) ((uint) param.Value + 100U))));
        MakeTypeData.ConcatFeatIds(feature1, bytes);
      }
      MakeTypeFeature feature2 = makeTypeRace.GetFeature((CustomizeIndex) 6);
      if (feature2 != null)
      {
        IEnumerable<byte> ids = discover.GetHairTypes(dataId).Except<byte>(((IEnumerable<MakeTypeParam>) feature2.Params).Select<MakeTypeParam, byte>((Func<MakeTypeParam, byte>) (param => param.Value)));
        MakeTypeData.ConcatFeatIds(feature2, ids);
      }
    }
    stopwatch.Stop();
    Ktisis.Ktisis.Log.Debug($"Populated discovery data in {stopwatch.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
  }

  private static void ConcatFeatIds(MakeTypeFeature feat, IEnumerable<byte> ids)
  {
    feat.Params = ((IEnumerable<MakeTypeParam>) feat.Params).Concat<MakeTypeParam>(ids.Select<byte, MakeTypeParam>((Func<byte, MakeTypeParam>) (id => new MakeTypeParam()
    {
      Value = id,
      Graphic = 0U
    }))).ToArray<MakeTypeParam>();
  }
}
