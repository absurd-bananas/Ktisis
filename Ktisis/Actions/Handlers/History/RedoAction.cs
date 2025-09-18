﻿// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.History.RedoAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Keys;
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Context.Types;

#nullable enable
namespace Ktisis.Actions.Handlers.History;

[Action("History_Redo")]
public class RedoAction(IPluginContext ctx) : KeyAction(ctx)
{
  public override KeybindInfo BindInfo { get; } = new KeybindInfo()
  {
    Trigger = KeybindTrigger.OnDown,
    Default = new ActionKeybind()
    {
      Enabled = true,
      Combo = new KeyCombo((VirtualKey) 90, new VirtualKey[2]
      {
        (VirtualKey) 17,
        (VirtualKey) 16 /*0x10*/
      })
    }
  };

  public override bool CanInvoke()
  {
    IEditorContext editor = this.Context.Editor;
    if (editor != null)
    {
      IActionManager actions = editor.Actions;
      if (actions != null)
      {
        IHistoryManager history = actions.History;
        if (history != null)
          return history.CanRedo;
      }
    }
    return false;
  }

  public override bool Invoke()
  {
    if (!this.CanInvoke())
      return false;
    this.Context.Editor.Actions.History.Redo();
    return true;
  }
}
