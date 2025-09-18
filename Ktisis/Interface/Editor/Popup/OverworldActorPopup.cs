// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Popup.OverworldActorPopup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Types;
using GLib.Lists;
using Ktisis.Common.Extensions;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Modules.Actors;
using Ktisis.Services.Game;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Editor.Popup;

public class OverworldActorPopup : KtisisPopup
{
  private readonly ActorService _actors;
  private readonly IEditorContext _ctx;
  private readonly ListBox<IGameObject> _list;

  public OverworldActorPopup(ActorService actors, IEditorContext ctx)
    : base("##OverworldActorPopup")
  {
    this._actors = actors;
    this._ctx = ctx;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._list = new ListBox<IGameObject>("##OverworldActorList", OverworldActorPopup.\u003C\u003EO.\u003C0\u003E__DrawActorName ?? (OverworldActorPopup.\u003C\u003EO.\u003C0\u003E__DrawActorName = new ListBox<IGameObject>.DrawItemDelegate(OverworldActorPopup.DrawActorName)));
  }

  protected override void OnDraw()
  {
    if (!this._ctx.IsValid)
    {
      this.Close();
    }
    else
    {
      IGameObject selected;
      if (!this._list.Draw(this._actors.GetOverworldActors().ToList<IGameObject>(), out selected) || !selected.IsEnabled())
        return;
      this.AddActor(selected);
    }
  }

  private async void AddActor(IGameObject actor)
  {
    ActorEntity actorEntity = await this._ctx.Scene.GetModule<ActorModule>().AddFromOverworld(actor);
  }

  private static bool DrawActorName(IGameObject actor, bool isFocus)
  {
    return Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(actor.GetNameOrFallback()), isFocus, (ImGuiSelectableFlags) 0, new Vector2());
  }
}
