// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.BattleNpc
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.GameData.Excel.Types;
using Ktisis.Structs.Characters;

namespace Ktisis.GameData.Excel;

[Sheet("BNpcBase", 3587712534)]
public struct BattleNpc(uint row) : IExcelRow<BattleNpc>, INpcBase {
	public uint RowId { get; } = row;

	public float Scale { get; init; } = 0.0f;

	private RowRef<Lumina.Excel.Sheets.ModelChara> ModelChara { get; init; } = new RowRef<Lumina.Excel.Sheets.ModelChara>();

	private RowRef<BNpcCustomize> Customize { get; init; } = new RowRef<BNpcCustomize>();

	private RowRef<NpcEquipment> Equipment { get; init; } = new RowRef<NpcEquipment>();

	static BattleNpc IExcelRow<BattleNpc>.Create(ExcelPage page, uint offset, uint row) => new BattleNpc(row) {
		Scale = page.ReadColumn<float>(4, offset),
		ModelChara = page.ReadRowRef<Lumina.Excel.Sheets.ModelChara>(5, offset),
		Customize = page.ReadRowRef<BNpcCustomize>(6, offset),
		Equipment = page.ReadRowRef<NpcEquipment>(7, offset)
	};

	public string Name { get; set; } = string.Empty;

	public ushort GetModelId() => (ushort)this.ModelChara.RowId;

	public CustomizeContainer? GetCustomize() {
		RowRef<BNpcCustomize> customize = this.Customize;
		return !customize.IsValid || customize.RowId == 0U ? new CustomizeContainer?() : this.Customize.Value.Customize;
	}

	public EquipmentContainer? GetEquipment() => !this.Equipment.IsValid ? new EquipmentContainer?() : this.Equipment.Value.Equipment;

	public WeaponModelId? GetMainHand() => !this.Equipment.IsValid ? new WeaponModelId?() : new WeaponModelId?(this.Equipment.Value.MainHand);

	public WeaponModelId? GetOffHand() => !this.Equipment.IsValid ? new WeaponModelId?() : new WeaponModelId?(this.Equipment.Value.OffHand);

	[Sheet("BNpcCustomize", 418406612)]
	private struct BNpcCustomize(uint row) : IExcelRow<BNpcCustomize> {
		public uint RowId { get; } = row;

		public CustomizeContainer Customize { get; private init; } = new CustomizeContainer();

		static BNpcCustomize IExcelRow<BNpcCustomize>.Create(
			ExcelPage page,
			uint offset,
			uint row
		) => new BNpcCustomize(row) {
			Customize = page.ReadCustomize(0, offset)
		};
	}
}
