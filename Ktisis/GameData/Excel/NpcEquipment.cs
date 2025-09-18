// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.NpcEquipment
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Structs.Characters;

namespace Ktisis.GameData.Excel;

[Sheet("NpcEquip", 2125379932)]
public struct NpcEquipment(uint row) : IExcelRow<NpcEquipment> {
	public uint RowId { get; } = row;

	public WeaponModelId MainHand { get; private set; } = new WeaponModelId();

	public WeaponModelId OffHand { get; private set; } = new WeaponModelId();

	public EquipmentContainer Equipment { get; set; } = new EquipmentContainer();

	static NpcEquipment IExcelRow<NpcEquipment>.Create(ExcelPage page, uint offset, uint row) => new NpcEquipment(row) {
		MainHand = page.ReadWeapon(0, offset),
		OffHand = page.ReadWeapon(3, offset),
		Equipment = page.ReadEquipment(6, offset)
	};
}
