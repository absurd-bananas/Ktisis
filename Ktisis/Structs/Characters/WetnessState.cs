// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Characters.WetnessState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Characters;

[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct WetnessState
{
  public float WeatherWetness;
  public float SwimmingWetness;
  public float WetnessDepth;
}
