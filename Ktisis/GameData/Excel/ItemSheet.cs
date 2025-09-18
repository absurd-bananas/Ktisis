// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.ItemSheet
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.GameData.Excel;

[Sheet("Item", 3919789213)]
public struct ItemSheet : IExcelRow<ItemSheet> {
	public uint RowId { get; }

	public string Name { get; }

	public ushort Icon { get; }

	public ItemModel Model { get; }

	public ItemModel SubModel { get; }

	private RowRef<EquipSlotCategoryRow> EquipSlotCategory { get; }

	public bool IsEquippable() => this.EquipSlotCategory.IsValid && this.EquipSlotCategory.RowId > 0U;

	public bool IsEquippable(EquipSlot slot) {
		var flag = this.IsEquippable() && this.EquipSlotCategory.Value.IsEquippable(slot);
		if (slot == EquipSlot.MainHand)
			flag |= this.EquipSlotCategory.Value.IsEquippable(EquipSlot.OffHand);
		return flag;
	}

	public bool IsWeapon() => this.IsEquippable(EquipSlot.MainHand) || this.IsEquippable(EquipSlot.OffHand);

	public ItemSheet(ExcelPage page, uint offset, uint row) {
		this.Model = null;
		this.SubModel = null;
		this.RowId = row;
		this.Name = page.ReadColumn<string>(9, offset);
		this.Icon = page.ReadColumn<ushort>(10, offset);
		this.EquipSlotCategory = page.ReadRowRef<EquipSlotCategoryRow>(17, offset);
		var isWep = this.IsWeapon();
		this.Model = new ItemModel(page.ReadColumn<ulong>(47, offset), isWep);
		this.SubModel = new ItemModel(page.ReadColumn<ulong>(48 /*0x30*/, offset), isWep);
	}

	static ItemSheet IExcelRow<ItemSheet>.Create(ExcelPage page, uint offset, uint row) => new ItemSheet(page, offset, row);

	[Sheet("EquipSlotCategory")]
	private struct EquipSlotCategoryRow(uint row) : IExcelRow<EquipSlotCategoryRow> {
		public uint RowId { get; } = row;

		private bool[] Slots { get; set; } = new bool[14];

		public bool IsEquippable(EquipSlot slot) {
			bool slot1;
			switch (slot) {
				case EquipSlot.MainHand:
					slot1 = this.Slots[1];
					break;
				case EquipSlot.OffHand:
					slot1 = this.Slots[0];
					break;
				default:
					slot1 = this.Slots[(int)slot];
					break;
			}
			return slot1;
		}

		static EquipSlotCategoryRow IExcelRow<EquipSlotCategoryRow>.Create(
			ExcelPage page,
			uint offset,
			uint row
		) {
			var flagArray = new bool[14];
			for (var columnIndex = 0; columnIndex < 14; ++columnIndex)
				flagArray[columnIndex] = page.ReadColumn<sbyte>(columnIndex, offset) != 0;
			return new EquipSlotCategoryRow(row) {
				Slots = flagArray
			};
		}
	}
}
