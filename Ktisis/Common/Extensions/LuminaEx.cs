// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.LuminaEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Structs.Characters;

namespace Ktisis.Common.Extensions;

public static class LuminaEx {
	public static RowRef<T> ReadRowRef<T>(this ExcelPage page, int columnIndex, uint offset) where T : struct, IExcelRow<T> {
		var andReadColumn = GetAndReadColumn(page, columnIndex, offset);
		return new RowRef<T>(page.Module, Convert.ToUInt32(andReadColumn), new Language?(page.Language));
	}

	public static T ReadColumn<T>(this ExcelPage page, int columnIndex, uint offset) => (T)GetAndReadColumn(page, columnIndex, offset);

	private static object GetAndReadColumn(ExcelPage page, int columnIndex, uint offset) {
		ExcelColumnDefinition column = page.Sheet.Columns[columnIndex];
		switch ((int)column.Type) {
			case 0:
				ReadOnlySeString readOnlySeString = page.ReadString((UIntPtr)((uint)column.Offset + offset), (UIntPtr)offset);
				return (object)((ReadOnlySeString) ref readOnlySeString ).ExtractText();
			case 1:
				return (object)page.ReadBool((UIntPtr)((uint)column.Offset + offset));
			case 2:
				return (object)page.ReadInt8((UIntPtr)((uint)column.Offset + offset));
			case 3:
				return (object)page.ReadUInt8((UIntPtr)((uint)column.Offset + offset));
			case 4:
				return (object)page.ReadInt16((UIntPtr)((uint)column.Offset + offset));
			case 5:
				return (object)page.ReadUInt16((UIntPtr)((uint)column.Offset + offset));
			case 6:
				return (object)page.ReadInt32((UIntPtr)((uint)column.Offset + offset));
			case 7:
				return (object)page.ReadUInt32((UIntPtr)((uint)column.Offset + offset));
			case 9:
				return (object)page.ReadFloat32((UIntPtr)((uint)column.Offset + offset));
			case 10:
				return (object)page.ReadInt64((UIntPtr)((uint)column.Offset + offset));
			case 11:
				return (object)page.ReadUInt64((UIntPtr)((uint)column.Offset + offset));
			case 25:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)0);
			case 26:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)1);
			case 27:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)2);
			case 28:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)3);
			case 29:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)4);
			case 30:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)5);
			case 31 /*0x1F*/:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)6);
			case 32 /*0x20*/:
				return (object)page.ReadPackedBool((UIntPtr)((uint)column.Offset + offset), (byte)7);
			default:
				throw new Exception($"Unknown type: {column.Type}");
		}
	}

	public static CustomizeContainer ReadCustomize(this ExcelPage parser, int index, uint offset) {
		var customizeContainer = new CustomizeContainer();
		for (var index1 = 0; index1 < 26; ++index1)
			customizeContainer[(uint)index1] = parser.ReadColumn<byte>(index + index1, offset);
		return customizeContainer;
	}

	public static WeaponModelId ReadWeapon(this ExcelPage parser, int index, uint offset) {
		Quad quad = Quad.op_Explicit(parser.ReadColumn<ulong>(index, offset));
		byte num1 = parser.ReadColumn<byte>(index + 1, offset);
		byte num2 = parser.ReadColumn<byte>(index + 2, offset);
		return new WeaponModelId {
			Id = ((Quad) ref quad).A,
			Type = ((Quad) ref quad).B,
			Variant = ((Quad) ref quad).C,
			Stain0 = num1,
			Stain1 = num2
		};
	}

	public static EquipmentModelId ReadEquipItem(this ExcelPage parser, int index, uint offset) {
		uint num1 = parser.ReadColumn<uint>(index, offset);
		byte num2 = parser.ReadColumn<byte>(index + 1, offset);
		byte num3 = parser.ReadColumn<byte>(index + 2, offset);
		return new EquipmentModelId {
			Id = (ushort)num1,
			Variant = (byte)(num1 >> 16 /*0x10*/),
			Stain0 = num2,
			Stain1 = num3
		};
	}

	public static EquipmentContainer ReadEquipment(this ExcelPage parser, int index, uint offset) {
		var equipmentContainer = new EquipmentContainer();
		for (var index1 = 0; index1 < 10; ++index1)
			equipmentContainer[(uint)index1] = parser.ReadEquipItem(index + index1 * 3 + (index1 > 0 ? 2 : 0), offset);
		return equipmentContainer;
	}
}
