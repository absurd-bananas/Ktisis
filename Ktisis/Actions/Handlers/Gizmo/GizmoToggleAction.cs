﻿// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Gizmo.GizmoToggleAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;
using Ktisis.Data.Config.Sections;

#nullable enable
namespace Ktisis.Actions.Handlers.Gizmo;

[Action("Gizmo_Toggle")]
public class GizmoToggleAction(IPluginContext ctx) : KeyAction(ctx)
{
  public override KeybindInfo BindInfo { get; } = new KeybindInfo()
  {
    Trigger = KeybindTrigger.OnDown,
    Default = new ActionKeybind()
    {
      Enabled = true,
      Combo = new KeyCombo((VirtualKey) 71, new VirtualKey[1]
      {
        (VirtualKey) 17
      })
    }
  };

  public override bool CanInvoke() => this.Context.Editor != null;

  public override bool Invoke()
  {
    if (!this.CanInvoke())
      return false;
    GizmoConfig gizmo = this.Context.Config.File.Gizmo;
    gizmo.Visible = !gizmo.Visible;
    return true;
  }
}
