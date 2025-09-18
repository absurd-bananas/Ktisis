// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Transforms.TransformTableFlags
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Interface.Components.Transforms;

[Flags]
public enum TransformTableFlags
{
  None = 0,
  Position = 1,
  Rotation = 2,
  Scale = 4,
  Operation = 8,
  UseAvailable = 16, // 0x00000010
  Default = Operation | Scale | Rotation | Position, // 0x0000000F
}
