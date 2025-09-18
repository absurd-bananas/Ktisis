// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Types.IEquipmentEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using Ktisis.Editor.Characters.State;

namespace Ktisis.Editor.Characters.Types;

public interface IEquipmentEditor {
	void ApplyStateFlags();

	EquipmentModelId GetEquipIndex(EquipIndex index);

	void SetEquipIndex(EquipIndex index, EquipmentModelId model);

	void SetEquipIdVariant(EquipIndex index, ushort id, byte variant);

	void SetEquipStainId(EquipIndex index, byte stainId, int dyeIndex = 0);

	bool GetHatVisible();

	void SetHatVisible(bool visible);

	bool GetVisorToggled();

	void SetVisorToggled(bool toggled);

	ushort GetGlassesId(int index = 0);

	void SetGlassesId(int index, ushort id);

	WeaponModelId GetWeaponIndex(WeaponIndex index);

	void SetWeaponIndex(WeaponIndex index, WeaponModelId model);

	void SetWeaponIdBaseVariant(WeaponIndex index, ushort id, ushort second, byte variant);

	void SetWeaponStainId(WeaponIndex index, byte stainId, int dyeIndex = 0);

	bool GetWeaponVisible(WeaponIndex index);

	void SetWeaponVisible(WeaponIndex index, bool visible);

	void ApplyStateToGameObject();
}
