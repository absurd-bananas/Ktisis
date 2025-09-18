// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.NamingService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Ktisis.Core.Attributes;
using Ktisis.GameData.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Services.Data;

[Singleton]
public class NamingService : INameResolver
{
  private readonly IDataManager _data;

  public NamingService(IDataManager data) => this._data = data;

  public string? GetWeaponName(ushort id, ushort secondId, ushort variant)
  {
    if (id == (ushort) 0)
      return (string) null;
    ItemSheet itemSheet = this.GetWeapons().FirstOrDefault<ItemSheet>((Func<ItemSheet, bool>) (wep =>
    {
      if (wep.Model.Matches(id, secondId, variant))
        return true;
      return wep.SubModel.Id != (ushort) 0 && wep.SubModel.Matches(id, secondId, variant);
    }));
    return StringExtensions.IsNullOrEmpty(itemSheet.Name) ? (string) null : itemSheet.Name;
  }

  private IEnumerable<ItemSheet> GetWeapons()
  {
    return ((IEnumerable<ItemSheet>) this._data.GetExcelSheet<ItemSheet>(new ClientLanguage?(), (string) null)).Where<ItemSheet>((Func<ItemSheet, bool>) (item => item.IsWeapon()));
  }
}
