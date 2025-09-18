// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.KeyEvent
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Structs.Input;

public enum KeyEvent : byte
{
  None = 0,
  Pressed = 1,
  Released = 2,
  AnyKeyHeld = 4,
  Held = 8,
}
