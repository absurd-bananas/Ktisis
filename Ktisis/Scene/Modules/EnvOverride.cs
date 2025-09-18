// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.EnvOverride
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Scene.Modules;

[Flags]
public enum EnvOverride
{
  None = 0,
  TimeWeather = 1,
  SkyId = 2,
  Lighting = 4,
  Stars = 8,
  Fog = 16, // 0x00000010
  Clouds = 32, // 0x00000020
  Rain = 64, // 0x00000040
  Dust = 128, // 0x00000080
  Wind = 256, // 0x00000100
}
