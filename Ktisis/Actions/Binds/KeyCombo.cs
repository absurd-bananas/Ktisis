// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Binds.KeyCombo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Actions.Binds;

public class KeyCombo
{
  public VirtualKey Key;
  public VirtualKey[] Modifiers;

  public KeyCombo(VirtualKey key = 0, params VirtualKey[] mods)
  {
    this.Key = key;
    this.Modifiers = mods;
  }

  public string GetShortcutString()
  {
    return string.Join(" + ", ((IEnumerable<VirtualKey>) this.Modifiers).Append<VirtualKey>(this.Key).Select<VirtualKey, string>((Func<VirtualKey, string>) (key => VirtualKeyExtensions.GetFancyName(key))));
  }

  public void AddModifier(VirtualKey key)
  {
    this.Modifiers = ((IEnumerable<VirtualKey>) this.Modifiers).Append<VirtualKey>(key).ToArray<VirtualKey>();
  }
}
