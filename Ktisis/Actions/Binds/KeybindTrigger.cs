// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Binds.KeybindTrigger
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Actions.Binds;

[Flags]
public enum KeybindTrigger
{
  None = 0,
  OnDown = 1,
  OnHeld = 2,
  OnRelease = 4,
}
