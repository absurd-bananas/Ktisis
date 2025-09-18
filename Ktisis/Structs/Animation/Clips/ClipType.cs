// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.Clips.ClipType
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Structs.Animation.Clips;

public enum ClipType : uint {
	BaseClip = 0,
	HavokAnimationClip = 6,
	ChildTimelineClip = 7,
	VfxClip = 9,
	VoiceClip = 41, // 0x00000029
	SoundClip = 52, // 0x00000034
	AutoShakeClip = 73 // 0x00000049
}
