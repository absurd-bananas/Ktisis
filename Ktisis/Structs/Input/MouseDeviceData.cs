// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.MouseDeviceData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Numerics;

#nullable disable
namespace Ktisis.Structs.Input;

public struct MouseDeviceData
{
  public int PosX;
  public int PosY;
  public int ScrollDelta;
  public MouseButton Pressed;
  public MouseButton Clicked;
  public ulong Unk1;
  public int DeltaX;
  public int DeltaY;
  public uint Unk2;
  public bool IsFocused;

  public bool IsButtonHeld(MouseButton button) => (this.Pressed & button) != 0;

  public Vector2 GetDelta(bool consume = false)
  {
    Vector2 delta = new Vector2((float) this.DeltaX, (float) this.DeltaY);
    if (!consume)
      return delta;
    this.DeltaX = 0;
    this.DeltaY = 0;
    return delta;
  }
}
