// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.QueueEntry
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Input;

[StructLayout(LayoutKind.Explicit)]
public struct QueueEntry {
	[FieldOffset(0)]
	public KeyEvent Event;
	[FieldOffset(1)]
	public byte KeyCode;
	[FieldOffset(4)]
	public byte Unknown;

	public VirtualKey VirtualKey => (VirtualKey)(int)this.KeyCode;
}
