// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Input.MouseButton
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Structs.Input;

[Flags]
public enum MouseButton
{
  None = 0,
  Left = 1,
  Middle = 2,
  Right = 4,
  Mouse4 = 8,
  Mouse5 = 16, // 0x00000010
}
