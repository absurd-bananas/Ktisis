// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.SaveModes
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Editor.Characters;

[Flags]
public enum SaveModes {
	None = 0,
	EquipmentGear = 1,
	EquipmentAccessories = 2,
	EquipmentWeapons = 4,
	AppearanceHair = 8,
	AppearanceFace = 16, // 0x00000010
	AppearanceBody = 32, // 0x00000020
	AppearanceExtended = 64, // 0x00000040
	Equipment = EquipmentAccessories | EquipmentGear, // 0x00000003
	Appearance = AppearanceBody | AppearanceFace | AppearanceHair, // 0x00000038
	All = Appearance | Equipment | EquipmentWeapons // 0x0000003F
}
