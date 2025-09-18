// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.ActorPropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using GLib.Widgets;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Localization;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Editor.Properties;

public class ActorPropertyList : ObjectPropertyList
{
  private readonly IEditorContext _ctx;
  private readonly LocaleManager _locale;
  private const string ImportOptsPopupId = "##KtisisCharaImportOptions";

  public ActorPropertyList(IEditorContext ctx, LocaleManager locale)
  {
    this._ctx = ctx;
    this._locale = locale;
  }

  public override void Invoke(IPropertyListBuilder builder, SceneEntity entity)
  {
    SceneEntity sceneEntity;
    switch (entity)
    {
      case BoneNode boneNode:
        sceneEntity = boneNode.Pose.Parent;
        break;
      case EntityPose entityPose:
        sceneEntity = entityPose.Parent;
        break;
      default:
        sceneEntity = entity;
        break;
    }
    ActorEntity actor = sceneEntity as ActorEntity;
    if (actor == null)
      return;
    builder.AddHeader("Actor", (Action) (() => this.DrawActorTab(actor)), 0);
  }

  private void DrawActorTab(ActorEntity actor)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    bool positionLockEnabled = this._ctx.Animation.PositionLockEnabled;
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._locale.Translate("actors.pos_lock")), ref positionLockEnabled))
      this._ctx.Animation.PositionLockEnabled = positionLockEnabled;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (Buttons.IconButton((FontAwesomeIcon) 61508))
      this._ctx.Interface.OpenActorEditor(actor);
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Edit actor appearance"));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Import"), new Vector2()))
      this._ctx.Interface.OpenCharaImport(actor);
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Export"), new Vector2()))
      return;
    this._ctx.Interface.OpenCharaExport(actor);
  }

  private void DrawGazeTab()
  {
  }
}
