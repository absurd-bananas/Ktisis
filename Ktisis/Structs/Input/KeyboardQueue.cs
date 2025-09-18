// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.KeyboardQueue
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Structs.Input;

public struct KeyboardQueue {
	public unsafe fixed ulong _data[66];

	public unsafe QueueEntry this[int i] {
		get {
			fixed (ulong* numPtr = this._data)
				return ((QueueEntry*)numPtr)[i];
		}
	}
}
