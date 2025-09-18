// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.ResidentNpc
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Common.Extensions;
using Ktisis.GameData.Excel.Types;
using Ktisis.Structs.Characters;

namespace Ktisis.GameData.Excel;

[Sheet("ENpcResident", 4149192844)]
public struct ResidentNpc(uint row) : IExcelRow<ResidentNpc>, INpcBase {
	public uint RowId { get; } = row;

	public byte Map { get; init; } = 0;

	private RowRef<Ktisis.GameData.Excel.EventNpc> EventNpc { get; init; } = new RowRef<Ktisis.GameData.Excel.EventNpc>();

	static ResidentNpc IExcelRow<ResidentNpc>.Create(ExcelPage page, uint offset, uint row) {
		string name = page.ReadColumn<string>(0, offset);
		sbyte article = page.ReadColumn<sbyte>(7, offset);
		var residentNpc = new ResidentNpc(row);
		ref var local = ref residentNpc;
		var str = name.FormatName(article);
		if (str == null)
			str = $"E:{row:D7}";
		local.Name = str;
		residentNpc.Map = page.ReadColumn<byte>(9, offset);
		residentNpc.EventNpc = new RowRef<Ktisis.GameData.Excel.EventNpc>(page.Module, row, new Language?(page.Language));
		return residentNpc;
	}

	public string Name { get; set; } = string.Empty;

	public uint HashId { get; set; } = 0;

	public ushort GetModelId() => !this.EventNpc.IsValid ? ushort.MaxValue : this.EventNpc.Value.GetModelId();

	public CustomizeContainer? GetCustomize() => !this.EventNpc.IsValid ? new CustomizeContainer?() : this.EventNpc.Value.GetCustomize();

	public EquipmentContainer? GetEquipment() => !this.EventNpc.IsValid ? new EquipmentContainer?() : this.EventNpc.Value.GetEquipment();

	public WeaponModelId? GetMainHand() => !this.EventNpc.IsValid ? new WeaponModelId?() : this.EventNpc.Value.GetMainHand();

	public WeaponModelId? GetOffHand() => !this.EventNpc.IsValid ? new WeaponModelId?() : this.EventNpc.Value.GetOffHand();
}
