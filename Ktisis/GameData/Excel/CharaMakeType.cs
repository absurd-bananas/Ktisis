// Decompiled with JetBrains decompiler
// Type: Ktisis.GameData.Excel.CharaMakeType
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.GameData.Excel;

[Sheet("CharaMakeType", 2161630061)]
public struct CharaMakeType(uint row) : IExcelRow<CharaMakeType> {
	public uint RowId { get; } = row;

	public CharaMakeStructStruct[] CharaMakeStruct { get; private set; } = null;

	public byte[] VoiceStruct { get; private set; } = null;

	public int[,] FacialFeatureOption { get; private set; } = null;

	public EquipmentStruct[] Equipment { get; private set; } = null;

	public RowRef<Lumina.Excel.Sheets.Race> Race { get; private set; } = new RowRef<Lumina.Excel.Sheets.Race>();

	public RowRef<Lumina.Excel.Sheets.Tribe> Tribe { get; private set; } = new RowRef<Lumina.Excel.Sheets.Tribe>();

	public sbyte Gender { get; private set; } = 0;

	static CharaMakeType IExcelRow<CharaMakeType>.Create(ExcelPage page, uint offset, uint row) {
		var makeStructStructArray = new CharaMakeStructStruct[28];
		for (var index1 = 0; index1 < 28; ++index1) {
			makeStructStructArray[index1].Menu = new RowRef<Lobby>(page.Module, page.ReadUInt32((UIntPtr)(offset + (ushort)(index1 * 428))), new Language?(page.Language));
			makeStructStructArray[index1].SubMenuMask = page.ReadUInt32((UIntPtr)(offset + (ushort)(index1 * 428 + 4)));
			makeStructStructArray[index1].Customize = page.ReadUInt32((UIntPtr)(offset + (ushort)(index1 * 428 + 8)));
			makeStructStructArray[index1].SubMenuParam = new uint[100];
			for (var index2 = 0; index2 < 100; ++index2)
				makeStructStructArray[index1].SubMenuParam[index2] = page.ReadUInt32((UIntPtr)(offset + (ushort)(index1 * 428 + 12 + index2 * 4)));
			makeStructStructArray[index1].InitVal = page.ReadUInt8((UIntPtr)(offset + (ushort)(index1 * 428 + 412)));
			makeStructStructArray[index1].SubMenuType = page.ReadUInt8((UIntPtr)(offset + (ushort)(index1 * 428 + 413)));
			makeStructStructArray[index1].SubMenuNum = page.ReadUInt8((UIntPtr)(offset + (ushort)(index1 * 428 + 414)));
			makeStructStructArray[index1].LookAt = page.ReadUInt8((UIntPtr)(offset + (ushort)(index1 * 428 + 415)));
			makeStructStructArray[index1].SubMenuGraphic = new byte[10];
			for (var index3 = 0; index3 < 10; ++index3)
				makeStructStructArray[index1].SubMenuGraphic[index3] = page.ReadUInt8((UIntPtr)(offset + (ushort)(index1 * 428 + 416 + index3)));
		}
		var numArray1 = new byte[12];
		for (var index = 0; index < 12; ++index)
			numArray1[index] = page.ReadUInt8((UIntPtr)(offset + (ushort)(11984 + index)));
		var numArray2 = new int[8, 7];
		for (var index4 = 0; index4 < 8; ++index4) {
			for (var index5 = 0; index5 < 7; ++index5)
				numArray2[index4, index5] = page.ReadInt32((UIntPtr)(offset + (ushort)(11996 + index4 * 28 + index5 * 4)));
		}
		var equipmentStructArray = new EquipmentStruct[3];
		for (var index = 0; index < 3; ++index) {
			equipmentStructArray[index].Helmet = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12224)));
			equipmentStructArray[index].Top = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12232)));
			equipmentStructArray[index].Gloves = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12240)));
			equipmentStructArray[index].Legs = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12248)));
			equipmentStructArray[index].Shoes = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12256)));
			equipmentStructArray[index].Weapon = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12264)));
			equipmentStructArray[index].SubWeapon = page.ReadUInt64((UIntPtr)(offset + (ushort)(index * 56 + 12272)));
		}
		return new CharaMakeType(row) {
			CharaMakeStruct = makeStructStructArray,
			VoiceStruct = numArray1,
			FacialFeatureOption = numArray2,
			Equipment = equipmentStructArray,
			Race = page.ReadRowRef<Lumina.Excel.Sheets.Race>(0, offset),
			Tribe = page.ReadRowRef<Lumina.Excel.Sheets.Tribe>(1, offset),
			Gender = page.ReadColumn<sbyte>(2, offset)
		};
	}

	public struct CharaMakeStructStruct {
		public RowRef<Lobby> Menu { get; internal set; }

		public uint SubMenuMask { get; internal set; }

		public uint Customize { get; internal set; }

		public uint[] SubMenuParam { get; internal set; }

		public byte InitVal { get; internal set; }

		public byte SubMenuType { get; internal set; }

		public byte SubMenuNum { get; internal set; }

		public byte LookAt { get; internal set; }

		public byte[] SubMenuGraphic { get; internal set; }
	}

	public struct EquipmentStruct {
		public ulong Helmet { get; internal set; }

		public ulong Top { get; internal set; }

		public ulong Gloves { get; internal set; }

		public ulong Legs { get; internal set; }

		public ulong Shoes { get; internal set; }

		public ulong Weapon { get; internal set; }

		public ulong SubWeapon { get; internal set; }
	}
}
