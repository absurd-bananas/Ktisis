// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.CharacterEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Actors;

[StructLayout(LayoutKind.Explicit, Size = 8928)]
public struct CharacterEx
{
  public const int AnimationOffset = 2592;
  [FieldOffset(0)]
  public FFXIVClientStructs.FFXIV.Client.Game.Character.Character Character;
  [FieldOffset(224 /*0xE0*/)]
  public Vector3 DrawObjectOffset;
  [FieldOffset(304)]
  public Vector3 CameraOffsetSmooth;
  [FieldOffset(384)]
  public Vector3 CameraOffset;
  [FieldOffset(1568)]
  public unsafe IntPtr* _emoteControllerVf;
  [FieldOffset(1568)]
  public EmoteController EmoteController;
  [FieldOffset(3298)]
  public CombatFlags CombatFlags;
  [FieldOffset(2592)]
  public AnimationContainer Animation;
  [FieldOffset(8920)]
  public float Opacity;
  [FieldOffset(9044)]
  public byte Mode;
  [FieldOffset(9045)]
  public EmoteModeEnum EmoteMode;

  public bool IsGPose
  {
    get
    {
      ushort objectIndex = this.Character.ObjectIndex;
      return objectIndex >= (ushort) 201 && objectIndex <= (ushort) 243;
    }
  }
}
