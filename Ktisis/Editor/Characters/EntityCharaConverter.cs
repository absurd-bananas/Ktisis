// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.EntityCharaConverter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Data.Files;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.GameData.Excel.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;
using Ktisis.Structs.Characters;

namespace Ktisis.Editor.Characters;

public class EntityCharaConverter {
	private readonly ICustomizeEditor _custom;
	private readonly ActorEntity _entity;
	private readonly IEquipmentEditor _equip;

	public EntityCharaConverter(ActorEntity entity, ICustomizeEditor custom, IEquipmentEditor equip) {
		this._entity = entity;
		this._custom = custom;
		this._equip = equip;
	}

	public void Apply(CharaFile file, SaveModes modes = SaveModes.All) {
		this.ApplyEquipment(file, modes);
		this.PrepareCustomize(file, modes).Apply();
		this.ApplyMisc(file);
	}

	public CharaFile Save() {
		var file = new CharaFile {
			Nickname = this._entity.Name
		};
		this.WriteCustomize(file);
		this.WriteEquipment(file);
		this.WriteMisc(file);
		return file;
	}

	public void Apply(INpcBase npc, SaveModes modes = SaveModes.All) {
		this.ApplyEquipment(npc, modes);
		this.PrepareCustomize(npc, modes).Apply();
	}

	private ICustomizeBatch PrepareCustomize(INpcBase npc, SaveModes modes = SaveModes.All) {
		var customizeBatch = this._custom.Prepare();
		var flag1 = modes.HasFlag(SaveModes.AppearanceFace);
		var flag2 = modes.HasFlag(SaveModes.AppearanceBody);
		var flag3 = modes.HasFlag(SaveModes.AppearanceHair);
		if (!flag1 && !flag2 && !flag3)
			return customizeBatch;
		if (flag2) {
			var modelId = npc.GetModelId();
			if (modelId != ushort.MaxValue)
				customizeBatch.SetModelId(modelId);
		}
		var customize = npc.GetCustomize();
		if (!customize.HasValue)
			return customizeBatch;
		var flag4 = true;
		for (uint index = 0; index < 26U; ++index) {
			flag4 &= customize.Value[index] == 0;
			if (!flag4)
				break;
		}
		if (flag4)
			return customizeBatch;
		foreach (int num in Enum.GetValues<CustomizeIndex>()) {
			CustomizeIndex index = (CustomizeIndex)num;
			bool flag5;
			if (index >= 12) {
				if (index > 20) {
					if (index > 23) {
						if (index - 24 > 1)
							goto label_24;
					} else
						goto label_23;
				}
			} else if (index >= 0) {
				if (index > 4) {
					switch (index - 5) {
						case 0:
							break;
						case 1:
						case 2:
						case 5:
						case 6:
							flag5 = flag3;
							goto label_25;
						default:
							goto label_24;
					}
				} else
					goto label_23;
			} else
				goto label_24;
			flag5 = flag1;
			goto label_25;
			label_23:
			flag5 = flag1 | flag2;
			goto label_25;
			label_24:
			flag5 = flag2;
			label_25:
			if (flag5)
				customizeBatch.SetCustomization(index, customize.Value[(uint)index]);
		}
		return customizeBatch;
	}

	private ICustomizeBatch PrepareCustomize(CharaFile file, SaveModes modes = SaveModes.All) {
		var customizeBatch1 = this._custom.Prepare();
		var num = modes.HasFlag(SaveModes.AppearanceFace) ? 1 : 0;
		var flag = modes.HasFlag(SaveModes.AppearanceBody);
		if (modes.HasFlag(SaveModes.AppearanceHair)) {
			var enableHighlights = file.EnableHighlights;
			var nullable = !enableHighlights.HasValue ? new byte?() : enableHighlights.GetValueOrDefault() ? (byte)128 /*0x80*/ : (byte)0;
			customizeBatch1.SetIfNotNull((CustomizeIndex)6, file.Hair).SetIfNotNull((CustomizeIndex)10, file.HairTone).SetIfNotNull((CustomizeIndex)11, file.Highlights).SetIfNotNull((CustomizeIndex)7, nullable);
		}
		if ((num | (flag ? 1 : 0)) != 0) {
			var customizeBatch2 = customizeBatch1;
			var race = file.Race;
			var nullable1 = race.HasValue ? (byte)race.GetValueOrDefault() : new byte?();
			var customizeBatch3 = customizeBatch2.SetIfNotNull((CustomizeIndex)0, nullable1);
			var tribe = file.Tribe;
			var nullable2 = tribe.HasValue ? (byte)tribe.GetValueOrDefault() : new byte?();
			var customizeBatch4 = customizeBatch3.SetIfNotNull((CustomizeIndex)4, nullable2);
			Gender? gender = file.Gender;
			var nullable3 = gender.HasValue ? (byte)gender.GetValueOrDefault() : new byte?();
			var customizeBatch5 = customizeBatch4.SetIfNotNull((CustomizeIndex)1, nullable3);
			Age? age = file.Age;
			var nullable4 = age.HasValue ? (byte)age.GetValueOrDefault() : new byte?();
			customizeBatch5.SetIfNotNull((CustomizeIndex)2, nullable4);
		}
		if (num != 0) {
			var customizeBatch6 = customizeBatch1.SetIfNotNull((CustomizeIndex)5, file.Head).SetIfNotNull((CustomizeIndex)16 /*0x10*/, file.Eyes).SetIfNotNull((CustomizeIndex)9, file.REyeColor).SetIfNotNull((CustomizeIndex)15, file.LEyeColor)
				.SetIfNotNull((CustomizeIndex)14, file.Eyebrows).SetIfNotNull((CustomizeIndex)17, file.Nose).SetIfNotNull((CustomizeIndex)18, file.Jaw).SetIfNotNull((CustomizeIndex)19, file.Mouth)
				.SetIfNotNull((CustomizeIndex)20, file.LipsToneFurPattern).SetIfNotNull((CustomizeIndex)13, file.LimbalEyes);
			var facialFeatures = file.FacialFeatures;
			var nullable = facialFeatures.HasValue ? (byte)facialFeatures.GetValueOrDefault() : new byte?();
			customizeBatch6.SetIfNotNull((CustomizeIndex)12, nullable).SetIfNotNull((CustomizeIndex)24, file.FacePaint).SetIfNotNull((CustomizeIndex)25, file.FacePaintColor);
		}
		if (flag)
			customizeBatch1.SetIfNotNull((CustomizeIndex)3, file.Height).SetIfNotNull((CustomizeIndex)8, file.Skintone).SetIfNotNull((CustomizeIndex)21, file.EarMuscleTailSize).SetIfNotNull((CustomizeIndex)22, file.TailEarsType)
				.SetIfNotNull((CustomizeIndex)23, file.Bust).SetModelId(file.ModelType);
		return customizeBatch1;
	}

	private void ApplyEquipment(INpcBase npc, SaveModes modes = SaveModes.All) {
		if (modes.HasFlag(SaveModes.EquipmentWeapons)) {
			WeaponModelId? mainHand = npc.GetMainHand();
			if (mainHand.HasValue)
				this._equip.SetWeaponIndex(WeaponIndex.MainHand, mainHand.GetValueOrDefault());
			WeaponModelId? offHand = npc.GetOffHand();
			if (offHand.HasValue)
				this._equip.SetWeaponIndex(WeaponIndex.OffHand, offHand.GetValueOrDefault());
		}
		var flag1 = modes.HasFlag(SaveModes.EquipmentGear);
		var flag2 = modes.HasFlag(SaveModes.EquipmentAccessories);
		if (!flag1 && !flag2)
			return;
		var equipment = npc.GetEquipment();
		if (!equipment.HasValue)
			return;
		var flag3 = true;
		for (uint index = 0; index < 10U; ++index) {
			flag3 &= equipment.Value[index].Value == 0UL;
			if (!flag3)
				break;
		}
		if (flag3)
			return;
		foreach (EquipIndex index in Enum.GetValues<EquipIndex>()) {
			if (index > EquipIndex.Feet || flag1) {
				if (index >= EquipIndex.Earring && !flag2)
					break;
				this._equip.SetEquipIndex(index, equipment.Value[(uint)index]);
			}
		}
	}

	private void ApplyEquipment(CharaFile file, SaveModes modes = SaveModes.All) {
		if (modes.HasFlag(SaveModes.EquipmentWeapons))
			this.SetWeaponIndex(file, WeaponIndex.MainHand).SetWeaponIndex(file, WeaponIndex.OffHand);
		var flag1 = modes.HasFlag(SaveModes.EquipmentGear);
		var flag2 = modes.HasFlag(SaveModes.EquipmentAccessories);
		if (!flag1 && !flag2)
			return;
		foreach (EquipIndex index in Enum.GetValues<EquipIndex>()) {
			if (index > EquipIndex.Feet || flag1) {
				if (index < EquipIndex.Earring || flag2) {
					EquipmentModelId? equipModelId = GetEquipModelId(file, index);
					if (equipModelId.HasValue) {
						EquipmentModelId valueOrDefault = equipModelId.GetValueOrDefault();
						this._equip.SetEquipIndex(index, valueOrDefault);
					}
				} else
					break;
			}
		}
		var charaFile = file;
		if (charaFile.Glasses == null) {
			CharaFile.GlassesSave glassesSave;
			charaFile.Glasses = glassesSave = new CharaFile.GlassesSave();
		}
		this._equip.SetGlassesId(0, file.Glasses.GlassesId);
	}

	private EntityCharaConverter SetWeaponIndex(CharaFile file, WeaponIndex index) {
		CharaFile.WeaponSave weaponSave1;
		switch (index) {
			case WeaponIndex.MainHand:
				weaponSave1 = file.MainHand;
				break;
			case WeaponIndex.OffHand:
				weaponSave1 = file.OffHand;
				break;
			default:
				weaponSave1 = null;
				break;
		}
		var weaponSave2 = weaponSave1;
		if (weaponSave2 == null)
			return this;
		this._equip.SetWeaponIndex(index, new WeaponModelId {
			Id = weaponSave2.ModelSet,
			Type = weaponSave2.ModelBase,
			Variant = weaponSave2.ModelVariant,
			Stain0 = (byte)weaponSave2.DyeId,
			Stain1 = (byte)weaponSave2.DyeId2
		});
		return this;
	}

	private static EquipmentModelId? GetEquipModelId(CharaFile file, EquipIndex index) {
		CharaFile.ItemSave itemSave1;
		switch (index) {
			case EquipIndex.Head:
				itemSave1 = file.HeadGear;
				break;
			case EquipIndex.Chest:
				itemSave1 = file.Body;
				break;
			case EquipIndex.Hands:
				itemSave1 = file.Hands;
				break;
			case EquipIndex.Legs:
				itemSave1 = file.Legs;
				break;
			case EquipIndex.Feet:
				itemSave1 = file.Feet;
				break;
			case EquipIndex.Earring:
				itemSave1 = file.Ears;
				break;
			case EquipIndex.Necklace:
				itemSave1 = file.Neck;
				break;
			case EquipIndex.Bracelet:
				itemSave1 = file.Wrists;
				break;
			case EquipIndex.RingRight:
				itemSave1 = file.RightRing;
				break;
			case EquipIndex.RingLeft:
				itemSave1 = file.LeftRing;
				break;
			default:
				itemSave1 = null;
				break;
		}
		var itemSave2 = itemSave1;
		if (itemSave2 == null)
			return new EquipmentModelId?();
		return new EquipmentModelId?(new EquipmentModelId {
			Id = itemSave2.ModelBase,
			Variant = itemSave2.ModelVariant,
			Stain0 = itemSave2.DyeId,
			Stain1 = itemSave2.DyeId2
		});
	}

	private unsafe void ApplyMisc(CharaFile file) {
		FFXIVClientStructs.FFXIV.Client.Game.Character.Character* character = this._entity.Character;
		if ((IntPtr)character == IntPtr.Zero)
			return;
		var transparency = file.Transparency;
		if (!transparency.HasValue)
			return;
		var valueOrDefault = transparency.GetValueOrDefault();
		((CharacterEx*)character)->Opacity = valueOrDefault;
	}

	private void WriteCustomize(CharaFile file) {
		file.ModelType = this._custom.GetModelId();
		file.Hair = this._custom.GetCustomization((CustomizeIndex)6);
		file.HairTone = this._custom.GetCustomization((CustomizeIndex)10);
		file.Highlights = this._custom.GetCustomization((CustomizeIndex)11);
		file.EnableHighlights = (this._custom.GetCustomization((CustomizeIndex)7) & 128U /*0x80*/) > 0U;
		file.Race = (CharaFile.AnamRace)this._custom.GetCustomization((CustomizeIndex)0);
		file.Tribe = (CharaFile.AnamTribe)this._custom.GetCustomization((CustomizeIndex)4);
		file.Gender = (Gender)this._custom.GetCustomization((CustomizeIndex)1);
		file.Age = (Age)this._custom.GetCustomization((CustomizeIndex)2);
		file.Head = this._custom.GetCustomization((CustomizeIndex)5);
		file.Eyes = this._custom.GetCustomization((CustomizeIndex)16 /*0x10*/);
		file.REyeColor = this._custom.GetCustomization((CustomizeIndex)9);
		file.LEyeColor = this._custom.GetCustomization((CustomizeIndex)15);
		file.Eyebrows = this._custom.GetCustomization((CustomizeIndex)14);
		file.Nose = this._custom.GetCustomization((CustomizeIndex)17);
		file.Jaw = this._custom.GetCustomization((CustomizeIndex)18);
		file.Mouth = this._custom.GetCustomization((CustomizeIndex)19);
		file.LipsToneFurPattern = this._custom.GetCustomization((CustomizeIndex)20);
		file.LimbalEyes = this._custom.GetCustomization((CustomizeIndex)13);
		file.FacialFeatures = (CharaFile.AnamFacialFeature)this._custom.GetCustomization((CustomizeIndex)12);
		file.FacePaint = this._custom.GetCustomization((CustomizeIndex)24);
		file.FacePaintColor = this._custom.GetCustomization((CustomizeIndex)25);
		file.Height = this._custom.GetCustomization((CustomizeIndex)3);
		file.Skintone = this._custom.GetCustomization((CustomizeIndex)8);
		file.EarMuscleTailSize = this._custom.GetCustomization((CustomizeIndex)21);
		file.TailEarsType = this._custom.GetCustomization((CustomizeIndex)22);
		file.Bust = this._custom.GetCustomization((CustomizeIndex)23);
	}

	private void WriteEquipment(CharaFile file) {
		file.MainHand = this.SaveWeapon(WeaponIndex.MainHand);
		file.OffHand = this.SaveWeapon(WeaponIndex.OffHand);
		file.HeadGear = this.SaveItem(EquipIndex.Head);
		file.Body = this.SaveItem(EquipIndex.Chest);
		file.Hands = this.SaveItem(EquipIndex.Hands);
		file.Legs = this.SaveItem(EquipIndex.Legs);
		file.Feet = this.SaveItem(EquipIndex.Feet);
		file.Ears = this.SaveItem(EquipIndex.Earring);
		file.Neck = this.SaveItem(EquipIndex.Necklace);
		file.Wrists = this.SaveItem(EquipIndex.Bracelet);
		file.LeftRing = this.SaveItem(EquipIndex.RingLeft);
		file.RightRing = this.SaveItem(EquipIndex.RingRight);
		file.Glasses = this.SaveGlasses();
	}

	private CharaFile.WeaponSave SaveWeapon(WeaponIndex index) => new CharaFile.WeaponSave(this._equip.GetWeaponIndex(index));

	private CharaFile.ItemSave SaveItem(EquipIndex index) => new CharaFile.ItemSave(this._equip.GetEquipIndex(index));

	private CharaFile.GlassesSave SaveGlasses() => new CharaFile.GlassesSave(this._equip.GetGlassesId());

	private unsafe void WriteMisc(CharaFile file) {
		FFXIVClientStructs.FFXIV.Client.Game.Character.Character* character = this._entity.Character;
		if ((IntPtr)character == IntPtr.Zero)
			return;
		file.Transparency = ((CharacterEx*)character)->Opacity;
		file.HeightMultiplier = character->GameObject.Height;
	}
}
