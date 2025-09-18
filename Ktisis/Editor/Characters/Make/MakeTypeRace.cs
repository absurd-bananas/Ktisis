// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Make.MakeTypeRace
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Enums;
using Ktisis.GameData.Chara;
using Ktisis.Structs.Characters;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Characters.Make;

public class MakeTypeRace(Tribe tribe, Gender gender)
{
  public Tribe Tribe = tribe;
  public Gender Gender = gender;
  public readonly Dictionary<CustomizeIndex, MakeTypeFeature> Customize = new Dictionary<CustomizeIndex, MakeTypeFeature>();
  public readonly Dictionary<byte, uint[]> FaceFeatureIcons = new Dictionary<byte, uint[]>();
  public TribeColors Colors;

  public bool HasFeature(CustomizeIndex index) => this.Customize.ContainsKey(index);

  public MakeTypeFeature? GetFeature(CustomizeIndex index)
  {
    return CollectionExtensions.GetValueOrDefault<CustomizeIndex, MakeTypeFeature>((IReadOnlyDictionary<CustomizeIndex, MakeTypeFeature>) this.Customize, index);
  }
}
