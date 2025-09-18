// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Types.ICustomizeBatch
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Enums;

#nullable enable
namespace Ktisis.Editor.Characters.Types;

public interface ICustomizeBatch
{
  ICustomizeBatch SetCustomization(CustomizeIndex index, byte value);

  ICustomizeBatch SetIfNotNull(CustomizeIndex index, byte? value);

  ICustomizeBatch SetModelId(uint id);

  void Apply();
}
