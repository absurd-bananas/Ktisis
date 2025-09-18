// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.Apricot.InstanceContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Vfx.Apricot;

[StructLayout(LayoutKind.Explicit, Size = 136)]
public struct InstanceContainer {
	public const int Size = 136;
	[FieldOffset(0)]
	public unsafe ApricotInstance* Instance;
}
