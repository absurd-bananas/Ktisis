// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Handlers.EquipmentEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.Scene.Entities.Game;

namespace Ktisis.Editor.Characters.Handlers;

public class EquipmentEditor(ActorEntity actor) : IEquipmentEditor {
	public void ApplyStateFlags() {
		this.UpdateWeaponVisibleState(WeaponIndex.MainHand);
		this.UpdateWeaponVisibleState(WeaponIndex.OffHand);
		if (actor.Appearance.VisorToggled == EquipmentToggle.None)
			return;
		this.SetVisorToggled(actor.Appearance.VisorToggled == EquipmentToggle.On);
	}

	public unsafe EquipmentModelId GetEquipIndex(EquipIndex index) {
		if (!actor.IsValid)
			return new EquipmentModelId();
		if (actor.Appearance.Equipment.IsSet(index))
			return actor.Appearance.Equipment[index];
		return (IntPtr)actor.CharacterBaseEx == IntPtr.Zero ? new EquipmentModelId() : actor.CharacterBaseEx->Equipment[(uint)index];
	}

	public unsafe void SetEquipIndex(EquipIndex index, EquipmentModelId model) {
		if (!actor.IsValid)
			return;
		actor.Appearance.Equipment[index] = model;
		CharacterBase* character = actor.GetCharacter();
		if ((IntPtr)character == IntPtr.Zero)
			return;
		var num = (long)((CharacterBase)(IntPtr)character).FlagSlotForUpdate((uint)index, &model);
	}

	public void SetEquipIdVariant(EquipIndex index, ushort id, byte variant) {
		EquipmentModelId equipIndex = this.GetEquipIndex(index);
		equipIndex.Id = id;
		equipIndex.Variant = variant;
		this.SetEquipIndex(index, equipIndex);
	}

	public void SetEquipStainId(EquipIndex index, byte stainId, int dyeIndex = 0) {
		EquipmentModelId equipIndex = this.GetEquipIndex(index);
		if (dyeIndex == 1)
			equipIndex.Stain1 = stainId;
		else
			equipIndex.Stain0 = stainId;
		this.SetEquipIndex(index, equipIndex);
	}

	public unsafe bool GetHatVisible() {
		return actor.IsValid && (IntPtr)actor.Character != IntPtr.Zero && actor.Appearance.CheckHatVisible(!((DrawDataContainer) ref actor.Character->DrawData).IsHatHidden);
	}

	public unsafe void SetHatVisible(bool visible) {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return;
		this.SetStateIfNotTracked(EquipIndex.Head);
		actor.Appearance.HatVisible = visible ? EquipmentToggle.On : EquipmentToggle.Off;
		((DrawDataContainer) ref actor.Character->DrawData).HideHeadgear(0U, !visible);
		if (!visible)
			return;
		this.ForceUpdateEquipIndex(EquipIndex.Head);
	}

	public unsafe bool GetVisorToggled() {
		return actor.IsValid && (IntPtr)actor.Character != IntPtr.Zero && actor.Appearance.CheckVisorToggled(((DrawDataContainer) ref actor.Character->DrawData).IsVisorToggled);
	}

	public unsafe void SetVisorToggled(bool toggled) {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return;
		actor.Appearance.VisorToggled = toggled ? EquipmentToggle.On : EquipmentToggle.Off;
		((DrawDataContainer) ref actor.Character->DrawData).SetVisor(toggled);
	}

	public unsafe ushort GetGlassesId(int index) {
		return (IntPtr)actor.Character == IntPtr.Zero ? (ushort)0 : ((DrawDataContainer) ref actor.Character ->DrawData).GlassesIds[index];
	}

	public unsafe void SetGlassesId(int index, ushort id) {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return;
		((DrawDataContainer) ref actor.Character->DrawData).SetGlasses(index, id);
	}

	public unsafe WeaponModelId GetWeaponIndex(WeaponIndex index) {
		if (!actor.IsValid)
			return new WeaponModelId();
		if (actor.Appearance.Weapons.IsSet(index))
			return actor.Appearance.Weapons[index];
		DrawObjectData* weaponData = GetWeaponData(actor, index);
		return (IntPtr)weaponData == IntPtr.Zero ? new WeaponModelId() : weaponData->ModelId;
	}

	public unsafe void SetWeaponIndex(WeaponIndex index, WeaponModelId model) {
		if (!actor.IsValid)
			return;
		actor.Appearance.Weapons[index] = model;
		FFXIVClientStructs.FFXIV.Client.Game.Character.Character* character = actor.Character;
		if ((IntPtr)character == IntPtr.Zero)
			return;
		((DrawDataContainer) ref character->DrawData).LoadWeapon((DrawDataContainer.WeaponSlot)(int)index, model, (byte)0, (byte)0, (byte)0, (byte)0);
	}

	public void SetWeaponIdBaseVariant(WeaponIndex index, ushort id, ushort second, byte variant) {
		WeaponModelId weaponIndex = this.GetWeaponIndex(index);
		weaponIndex.Id = id;
		weaponIndex.Type = second;
		weaponIndex.Variant = (ushort)variant;
		this.SetWeaponIndex(index, weaponIndex);
	}

	public void SetWeaponStainId(WeaponIndex index, byte stainId, int dyeIndex = 0) {
		WeaponModelId weaponIndex = this.GetWeaponIndex(index);
		if (dyeIndex == 1)
			weaponIndex.Stain1 = stainId;
		else
			weaponIndex.Stain0 = stainId;
		this.SetWeaponIndex(index, weaponIndex);
	}

	public unsafe bool GetWeaponVisible(WeaponIndex index) {
		DrawObjectData* weaponData = GetWeaponData(actor, index);
		return (IntPtr)weaponData != IntPtr.Zero && actor.Appearance.Weapons.CheckVisible(index, !((DrawObjectData)(IntPtr)weaponData).IsHidden);
	}

	public unsafe void SetWeaponVisible(WeaponIndex index, bool visible) {
		actor.Appearance.Weapons.SetVisible(index, visible);
		DrawObjectData* weaponData = GetWeaponData(actor, index);
		if ((IntPtr)weaponData == IntPtr.Zero)
			return;
		((DrawObjectData)(IntPtr)weaponData).IsHidden = !visible;
	}

	public unsafe void ApplyStateToGameObject() {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return;
		foreach (EquipIndex index in Enum.GetValues<EquipIndex>()) {
			EquipmentModelId equipIndex = this.GetEquipIndex(index);
			((DrawDataContainer) ref actor.Character->DrawData).LoadEquipment((DrawDataContainer.EquipmentSlot)(int)index, &equipIndex, true);
		}
	}

	private void SetStateIfNotTracked(EquipIndex index) {
		if (!actor.IsValid || actor.Appearance.Equipment.IsSet(index))
			return;
		actor.Appearance.Equipment[index] = this.GetEquipIndex(index);
	}

	private unsafe void ForceUpdateEquipIndex(EquipIndex index) {
		if (!actor.IsValid)
			return;
		CharacterBase* character = actor.GetCharacter();
		if ((IntPtr)character == IntPtr.Zero)
			return;
		EquipmentModelId equipIndex = this.GetEquipIndex(index);
		var num = (long)((CharacterBase)(IntPtr)character).FlagSlotForUpdate((uint)index, &equipIndex);
	}

	private unsafe static DrawObjectData* GetWeaponData(ActorEntity actor, WeaponIndex index) {
		if (!actor.IsValid || (IntPtr)actor.Character == IntPtr.Zero)
			return (DrawObjectData*)null;
		fixed (DrawObjectData* weaponData = &((DrawDataContainer) ref actor.Character ->DrawData).WeaponData[(int)index])
		return weaponData;
	}

	private void UpdateWeaponVisibleState(WeaponIndex index) {
		var visible = actor.Appearance.Weapons.GetVisible(index);
		if (visible == EquipmentToggle.None)
			return;
		this.SetWeaponVisible(index, visible == EquipmentToggle.On);
	}
}
