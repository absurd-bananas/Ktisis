// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Env.Weather.EnvWind
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Env.Weather;

[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct EnvWind {
	[FieldOffset(0)]
	public float Direction;
	[FieldOffset(4)]
	public float Angle;
	[FieldOffset(8)]
	public float Speed;
}
