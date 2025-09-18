// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Files.CharaFile
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Data.Json.Converters;

namespace Ktisis.Data.Files;

public class CharaFile : JsonFile {

	[Flags]
	public enum AnamFacialFeature : byte {
		None = 0,
		First = 1,
		Second = 2,
		Third = 4,
		Fourth = 8,
		Fifth = 16, // 0x10
		Sixth = 32, // 0x20
		Seventh = 64, // 0x40
		LegacyTattoo = 128 // 0x80
	}

	public enum AnamRace : byte {
		Hyur = 1,
		Elezen = 2,
		Lalafel = 3,
		Miqote = 4,
		Roegadyn = 5,
		AuRa = 6,
		Hrothgar = 7,
		Viera = 8
	}

	public enum AnamTribe : byte {
		Midlander = 1,
		Highlander = 2,
		Wildwood = 3,
		Duskwight = 4,
		Plainsfolk = 5,
		Dunesfolk = 6,
		SeekerOfTheSun = 7,
		KeeperOfTheMoon = 8,
		SeaWolf = 9,
		Hellsguard = 10, // 0x0A
		Raen = 11, // 0x0B
		Xaela = 12, // 0x0C
		Helions = 13, // 0x0D
		TheLost = 14, // 0x0E
		Rava = 15, // 0x0F
		Veena = 16 // 0x10
	}

	public const int CurrentVersion = 1;

	public new string FileExtension { get; set; } = ".chara";

	public new string TypeName { get; set; } = "Ktisis Character File";

	[DeserializerDefault(1)]
	public new int FileVersion { get; set; } = 1;

	public string? Nickname { get; set; }

	public uint ModelType { get; set; }

	public ObjectKind ObjectKind { get; set; }

	public AnamRace? Race { get; set; }

	public Ktisis.Structs.Characters.Gender? Gender { get; set; }

	public Ktisis.Structs.Characters.Age? Age { get; set; }

	public byte? Height { get; set; }

	public AnamTribe? Tribe { get; set; }

	public byte? Head { get; set; }

	public byte? Hair { get; set; }

	public bool? EnableHighlights { get; set; }

	public byte? Skintone { get; set; }

	public byte? REyeColor { get; set; }

	public byte? HairTone { get; set; }

	public byte? Highlights { get; set; }

	public AnamFacialFeature? FacialFeatures { get; set; }

	public byte? LimbalEyes { get; set; }

	public byte? Eyebrows { get; set; }

	public byte? LEyeColor { get; set; }

	public byte? Eyes { get; set; }

	public byte? Nose { get; set; }

	public byte? Jaw { get; set; }

	public byte? Mouth { get; set; }

	public byte? LipsToneFurPattern { get; set; }

	public byte? EarMuscleTailSize { get; set; }

	public byte? TailEarsType { get; set; }

	public byte? Bust { get; set; }

	public byte? FacePaint { get; set; }

	public byte? FacePaintColor { get; set; }

	public WeaponSave? MainHand { get; set; }

	public WeaponSave? OffHand { get; set; }

	public ItemSave? HeadGear { get; set; }

	public ItemSave? Body { get; set; }

	public ItemSave? Hands { get; set; }

	public ItemSave? Legs { get; set; }

	public ItemSave? Feet { get; set; }

	public ItemSave? Ears { get; set; }

	public ItemSave? Neck { get; set; }

	public ItemSave? Wrists { get; set; }

	public ItemSave? LeftRing { get; set; }

	public ItemSave? RightRing { get; set; }

	public GlassesSave? Glasses { get; set; }

	public Vector3? BustScale { get; set; }

	public float? Transparency { get; set; }

	public float? HeightMultiplier { get; set; }

	[Serializable]
	public class WeaponSave {
		public WeaponSave() { }

		public WeaponSave(WeaponModelId from) {
			this.ModelSet = from.Id;
			this.ModelBase = from.Type;
			this.ModelVariant = from.Variant;
			this.DyeId = (ushort)from.Stain0;
			this.DyeId2 = (ushort)from.Stain1;
		}

		public Vector3 Color { get; set; }

		public Vector3 Scale { get; set; }

		public ushort ModelSet { get; set; }

		public ushort ModelBase { get; set; }

		public ushort ModelVariant { get; set; }

		public ushort DyeId { get; set; }

		public ushort DyeId2 { get; set; }
	}

	[Serializable]
	public class ItemSave {
		public ItemSave() { }

		public ItemSave(EquipmentModelId from) {
			this.ModelBase = from.Id;
			this.ModelVariant = from.Variant;
			this.DyeId = from.Stain0;
			this.DyeId2 = from.Stain1;
		}

		public ushort ModelBase { get; set; }

		public byte ModelVariant { get; set; }

		public byte DyeId { get; set; }

		public byte DyeId2 { get; set; }
	}

	[Serializable]
	public class GlassesSave {
		public GlassesSave() { }

		public GlassesSave(ushort id) {
			this.GlassesId = id;
		}

		public ushort GlassesId { get; set; }
	}
}
