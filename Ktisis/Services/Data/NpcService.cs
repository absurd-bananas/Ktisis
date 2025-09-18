// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.NpcService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Plugin.Services;
using Ktisis.Common.Extensions;
using Ktisis.Core.Attributes;
using Ktisis.GameData.Excel;
using Ktisis.GameData.Excel.Types;
using Ktisis.Structs.Characters;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Services.Data;

[Singleton]
public class NpcService
{
  private readonly IDataManager _data;

  public NpcService(IDataManager data) => this._data = data;

  public async Task<IEnumerable<INpcBase>> GetNpcList()
  {
    await Task.Yield();
    Stopwatch timer = new Stopwatch();
    timer.Start();
    Task<IEnumerable<INpcBase>> battleTask = this.GetBattleNpcs();
    Task<IEnumerable<INpcBase>> residentTask = this.GetResidentNpcs();
    \u003C\u003Ey__InlineArray2<Task<IEnumerable<INpcBase>>> buffer = new \u003C\u003Ey__InlineArray2<Task<IEnumerable<INpcBase>>>();
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task<IEnumerable<INpcBase>>>, Task<IEnumerable<INpcBase>>>(ref buffer, 0) = battleTask;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task<IEnumerable<INpcBase>>>, Task<IEnumerable<INpcBase>>>(ref buffer, 1) = residentTask;
    // ISSUE: reference to a compiler-generated method
    IEnumerable<INpcBase>[] npcBasesArray = await Task.WhenAll<IEnumerable<INpcBase>>(\u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray2<Task<IEnumerable<INpcBase>>>, Task<IEnumerable<INpcBase>>>(in buffer, 2));
    IEnumerable<INpcBase> npcBases = Enumerable.DistinctBy<INpcBase, (string, ushort, CustomizeContainer?, EquipmentContainer?)>(battleTask.Result.Concat<INpcBase>(residentTask.Result), (Func<INpcBase, (string, ushort, CustomizeContainer?, EquipmentContainer?)>) (npc => (npc.Name, npc.GetModelId(), npc.GetCustomize(), npc.GetEquipment())));
    timer.Stop();
    Ktisis.Ktisis.Log.Debug($"NPC list retrieved in {timer.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
    IEnumerable<INpcBase> npcList = npcBases;
    timer = (Stopwatch) null;
    battleTask = (Task<IEnumerable<INpcBase>>) null;
    residentTask = (Task<IEnumerable<INpcBase>>) null;
    return npcList;
  }

  private async Task<IEnumerable<INpcBase>> GetBattleNpcs()
  {
    await Task.Yield();
    Task<Dictionary<string, uint>> nameIndex = NpcService.GetNameIndex();
    ExcelSheet<BattleNpc> npcSheet = this._data.GetExcelSheet<BattleNpc>(new ClientLanguage?(), (string) null);
    ExcelSheet<BNpcName> namesSheet = this._data.GetExcelSheet<BNpcName>(new ClientLanguage?(), (string) null);
    Dictionary<string, uint> nameIndexDict = await nameIndex;
    IEnumerable<INpcBase> battleNpcs = ((IEnumerable<BattleNpc>) npcSheet).Skip<BattleNpc>(1).Select<BattleNpc, BattleNpc>((Func<BattleNpc, BattleNpc>) (row =>
    {
      string str1 = (string) null;
      uint num;
      if (nameIndexDict.TryGetValue(row.RowId.ToString(), out num) && namesSheet.HasRow(num))
      {
        BNpcName row1 = namesSheet.GetRow(num);
        ReadOnlySeString singular = ((BNpcName) ref row1).Singular;
        str1 = ((ReadOnlySeString) ref singular).ExtractText().FormatName(((BNpcName) ref row1).Article);
      }
      ref BattleNpc local = ref row;
      string str2 = str1;
      if (str2 == null)
        str2 = $"B:{row.RowId:D7}";
      local.Name = str2;
      return row;
    })).Cast<INpcBase>();
    npcSheet = (ExcelSheet<BattleNpc>) null;
    return battleNpcs;
  }

  private async Task<IEnumerable<INpcBase>> GetResidentNpcs()
  {
    await Task.Yield();
    return ((IEnumerable<ResidentNpc>) this._data.GetExcelSheet<ResidentNpc>(new ClientLanguage?(), (string) null)).Where<ResidentNpc>((Func<ResidentNpc, bool>) (npc => npc.Map > (byte) 0)).Cast<INpcBase>();
  }

  private static async Task<Dictionary<string, uint>> GetNameIndex()
  {
    Dictionary<string, uint> nameIndex;
    using (StreamReader reader = new StreamReader(NpcService.GetNameIndexStream()))
      nameIndex = JsonConvert.DeserializeObject<Dictionary<string, uint>>(await ((TextReader) reader).ReadToEndAsync()) ?? new Dictionary<string, uint>();
    return nameIndex;
  }

  private static Stream GetNameIndexStream()
  {
    Assembly executingAssembly = Assembly.GetExecutingAssembly();
    string message = executingAssembly.GetName().Name + ".Data.Library.bnpc-index.json";
    return executingAssembly.GetManifestResourceStream(message) ?? throw new FileNotFoundException(message);
  }
}
