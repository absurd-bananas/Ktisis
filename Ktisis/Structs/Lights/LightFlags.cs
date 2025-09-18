// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Lights.LightFlags
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Structs.Lights;

[Flags]
public enum LightFlags : uint
{
  Reflection = 1,
  Dynamic = 2,
  CharaShadow = 4,
  ObjectShadow = 8,
}
