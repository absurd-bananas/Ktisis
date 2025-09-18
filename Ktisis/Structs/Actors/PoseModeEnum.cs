// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.PoseModeEnum
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Structs.Actors;

public enum PoseModeEnum : byte {
	Idle = 0,
	Battle = 1,
	SitChair = 2,
	SitGround = 3,
	Sleeping = 4,
	None = 255 // 0xFF
}
