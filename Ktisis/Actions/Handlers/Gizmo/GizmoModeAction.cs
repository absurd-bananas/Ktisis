// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Gizmo.GizmoModeAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;
using Ktisis.ImGuizmo;

#nullable enable
namespace Ktisis.Actions.Handlers.Gizmo;

[Action("Gizmo_ToggleMode")]
public class GizmoModeAction(IPluginContext ctx) : KeyAction(ctx)
{
  public override KeybindInfo BindInfo { get; } = new KeybindInfo()
  {
    Trigger = KeybindTrigger.OnDown,
    Default = new ActionKeybind()
    {
      Enabled = true,
      Combo = new KeyCombo((VirtualKey) 88, new VirtualKey[1]
      {
        (VirtualKey) 17
      })
    }
  };

  public override bool Invoke()
  {
    if (this.Context.Editor == null || this.Context.Editor.Selection.Count == 0)
      return false;
    this.Context.Config.File.Gizmo.Mode ^= Mode.World;
    return true;
  }
}
