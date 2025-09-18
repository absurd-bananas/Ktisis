// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Actions.ActionKeybind
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Ktisis.Actions.Binds;
using System;

#nullable enable
namespace Ktisis.Data.Config.Actions;

public class ActionKeybind
{
  public bool Enabled = true;
  public KeyCombo Combo = new KeyCombo((VirtualKey) 0, Array.Empty<VirtualKey>());
}
