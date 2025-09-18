// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.EventNpc
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ktisis.Common.Extensions;
using Ktisis.GameData.Excel.Types;
using Ktisis.Structs.Characters;
using Lumina.Data;
using Lumina.Excel;

#nullable enable
namespace Ktisis.GameData.Excel;

[Sheet("ENpcBase", 1537860006)]
public struct EventNpc(uint row) : IExcelRow<EventNpc>, INpcBase
{
  public uint RowId { get; } = row;

  public RowRef<Lumina.Excel.Sheets.ModelChara> ModelChara { get; init; } = new RowRef<Lumina.Excel.Sheets.ModelChara>();

  public CustomizeContainer Customize { get; init; } = new CustomizeContainer();

  public WeaponModelId MainHand { get; init; } = new WeaponModelId();

  public WeaponModelId OffHand { get; init; } = new WeaponModelId();

  public EquipmentContainer Equipment { get; init; } = new EquipmentContainer();

  static EventNpc IExcelRow<EventNpc>.Create(ExcelPage page, uint offset, uint row)
  {
    EventNpc eventNpc = new EventNpc(row);
    // ISSUE: explicit reference operation
    (^ref eventNpc).Name = $"E:{row:D7}";
    eventNpc.ModelChara = page.ReadRowRef<Lumina.Excel.Sheets.ModelChara>(35, offset);
    eventNpc.Customize = page.ReadCustomize(36, offset);
    eventNpc.MainHand = page.ReadWeapon(65, offset);
    eventNpc.OffHand = page.ReadWeapon(68, offset);
    eventNpc.Equipment = EventNpc.ReadEquipment(page, offset);
    return eventNpc;
  }

  private static EquipmentContainer ReadEquipment(ExcelPage page, uint offset)
  {
    ushort num = page.ReadColumn<ushort>(63 /*0x3F*/, offset);
    EquipmentContainer equipmentContainer = page.ReadEquipment(71, offset);
    if (num == (ushort) 0 || num == (ushort) 175)
      return equipmentContainer;
    RowRef<NpcEquipment> rowRef = new RowRef<NpcEquipment>(page.Module, (uint) num, new Language?(page.Language));
    if (!rowRef.IsValid)
      return equipmentContainer;
    for (uint index = 0; index < 10U; ++index)
    {
      EquipmentModelId equipmentModelId = rowRef.Value.Equipment[index];
      if (!equipmentModelId.Equals((object) null))
        equipmentContainer[index] = equipmentModelId;
    }
    return equipmentContainer;
  }

  public string Name { get; set; } = string.Empty;

  public ushort GetModelId() => (ushort) this.ModelChara.RowId;

  public CustomizeContainer? GetCustomize() => new CustomizeContainer?(this.Customize);

  public EquipmentContainer GetEquipment() => this.Equipment;

  public WeaponModelId? GetMainHand() => new WeaponModelId?(this.MainHand);

  public WeaponModelId? GetOffHand() => new WeaponModelId?(this.OffHand);
}
