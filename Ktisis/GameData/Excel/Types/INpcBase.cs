// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.Types.INpcBase
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Ktisis.Structs.Characters;

#nullable enable
namespace Ktisis.GameData.Excel.Types;

public interface INpcBase
{
  string Name { get; set; }

  ushort GetModelId() => 0;

  CustomizeContainer? GetCustomize() => new CustomizeContainer?();

  EquipmentContainer? GetEquipment() => new EquipmentContainer?();

  WeaponModelId? GetMainHand() => new WeaponModelId?();

  WeaponModelId? GetOffHand() => new WeaponModelId?();
}
