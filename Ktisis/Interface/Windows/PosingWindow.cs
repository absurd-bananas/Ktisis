// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.PosingWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Ktisis.Data.Config.Pose2D;
using Ktisis.Data.Serialization;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Components.Posing;
using Ktisis.Interface.Components.Posing.Types;
using Ktisis.Interface.Types;
using Ktisis.Localization;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Services.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Windows;

public class PosingWindow : KtisisWindow
{
  private readonly IEditorContext _ctx;
  private readonly LocaleManager _locale;
  private readonly GPoseService _gpose;
  private readonly PoseViewRenderer _render;
  private PoseViewSchema? _schema;
  private PosingWindow.ViewEnum _view;
  private ActorEntity? _target;

  public PosingWindow(
    IEditorContext ctx,
    ITextureProvider tex,
    LocaleManager locale,
    GPoseService gpose)
    : base("Pose View")
  {
    this._ctx = ctx;
    this._locale = locale;
    this._gpose = gpose;
    this._render = new PoseViewRenderer(ctx.Config, tex);
  }

  public virtual void OnOpen() => this._schema = SchemaReader.ReadPoseView();

  public virtual void PreOpenCheck()
  {
    if (this._ctx.IsValid)
      return;
    Ktisis.Ktisis.Log.Verbose("Context for posing window is stale, closing...", Array.Empty<object>());
    this.Close();
  }

  public virtual void PreDraw()
  {
    Window.WindowSizeConstraints windowSizeConstraints;
    // ISSUE: explicit constructor call
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
    ((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(500f, 350f);
    this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
  }

  public virtual void Draw()
  {
    if (this._ctx.Config.Editor.UseLegacyPoseViewTabs)
      this.DrawLegacyTabs();
    else if (this._ctx.Config.Editor.UseLegacyWindowBehavior)
    {
      this.DrawLegacyTarget();
    }
    else
    {
      ActorEntity actorEntity = (ActorEntity) this._ctx.Selection.GetSelected().FirstOrDefault<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is ActorEntity));
      if (actorEntity != null && this._target != actorEntity)
        this._target = actorEntity;
      ActorEntity target = this._target;
      if (target == null || !target.IsValid)
        Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Select an actor to start editing its pose."));
      else
        this.DrawWindow(this._target);
    }
  }

  private IEnumerable<ActorEntity> GetValidTargets()
  {
    return this._ctx.Scene.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is ActorEntity)).Cast<ActorEntity>();
  }

  private void DrawLegacyTabs()
  {
    using (ImRaii.TabBar(ImU8String.op_Implicit("##pose_tabs")))
    {
      foreach (ActorEntity validTarget in this.GetValidTargets())
      {
        using (ImRaii.IEndObject iendObject = ImRaii.TabItem(ImU8String.op_Implicit(validTarget.Name)))
        {
          if (iendObject.Success)
          {
            Dalamud.Bindings.ImGui.ImGui.Spacing();
            this.DrawWindow(validTarget);
          }
        }
      }
    }
  }

  private void DrawLegacyTarget()
  {
    ushort? tarIndex = this._gpose.GPoseTarget?.ObjectIndex;
    if (this._target != null)
    {
      int objectIndex = (int) this._target.Actor.ObjectIndex;
      ushort? nullable1 = tarIndex;
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int valueOrDefault = nullable2.GetValueOrDefault();
      if (objectIndex == valueOrDefault & nullable2.HasValue)
        goto label_5;
    }
    if (tarIndex.HasValue)
    {
      ActorEntity actorEntity = this.GetValidTargets().FirstOrDefault<ActorEntity>((Func<ActorEntity, bool>) (actor =>
      {
        int objectIndex = (int) actor.Actor.ObjectIndex;
        ushort? nullable3 = tarIndex;
        int? nullable4 = nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?();
        int valueOrDefault = nullable4.GetValueOrDefault();
        return objectIndex == valueOrDefault & nullable4.HasValue;
      }));
      if (actorEntity != null)
        this._target = actorEntity;
    }
label_5:
    ActorEntity target = this._target;
    if (target == null || !target.IsValid)
      Ktisis.Ktisis.Log.Info("Targeted actor has no skeleton or is invalid.", Array.Empty<object>());
    else
      this.DrawWindow(this._target);
  }

  private void DrawWindow(ActorEntity target)
  {
    Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
    float num1 = contentRegionAvail.X * 0.9f;
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float num2 = ((ImGuiStylePtr) ref style).ItemSpacing.X * 2f;
    Vector2 region = contentRegionAvail with
    {
      X = num1 - num2
    };
    this.DrawView(target, region);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(num1);
    this.DrawSideMenu(target);
  }

  private void DrawSideMenu(ActorEntity target)
  {
    using (ImRaii.Group())
    {
      this.DrawViewSelect();
      for (int index = 0; index < 3; ++index)
        Dalamud.Bindings.ImGui.ImGui.Spacing();
      this.DrawImportExport(target);
    }
  }

  private void DrawViewSelect()
  {
    using (ImRaii.Group())
    {
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("View:"));
      foreach (PosingWindow.ViewEnum viewEnum in Enum.GetValues<PosingWindow.ViewEnum>())
      {
        if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit(viewEnum.ToString()), this._view == viewEnum))
          this._view = viewEnum;
      }
    }
  }

  private void DrawImportExport(ActorEntity target)
  {
    if (target.Pose == null)
      return;
    if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Import"), new Vector2()))
      this._ctx.Interface.OpenPoseImport(target);
    if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Export"), new Vector2()))
      return;
    this._ctx.Interface.OpenPoseExport(target.Pose);
  }

  private void DrawView(ActorEntity target, Vector2 region)
  {
    using (ImRaii.Child(ImU8String.op_Implicit("##viewFrame"), region, false, (ImGuiWindowFlags) 8))
    {
      IViewFrame frame = this._render.StartFrame();
      if (this._view == PosingWindow.ViewEnum.Body)
      {
        this.DrawView(frame, "Body", 0.35f);
        Dalamud.Bindings.ImGui.ImGui.SameLine();
        this.DrawView(frame, "Armor", 0.35f);
        Dalamud.Bindings.ImGui.ImGui.SameLine();
        using (ImRaii.Group())
        {
          this.DrawView(frame, "Hands", 0.3f, 0.6f);
          Dalamud.Bindings.ImGui.ImGui.Spacing();
          bool hasTail = false;
          bool isBunny = false;
          target.Pose?.CheckFeatures(out hasTail, out isBunny);
          IDictionary<string, string> template = this._render.BuildTemplate(target);
          float num;
          if (hasTail)
          {
            if (isBunny)
            {
              num = 0.15f;
              goto label_11;
            }
          }
          else if (!isBunny)
          {
            num = 0.0f;
            goto label_11;
          }
          num = 0.3f;
label_11:
          float width = num;
          if (hasTail)
          {
            this.DrawView(frame, "Tail", width, 0.4f);
            if (isBunny)
              Dalamud.Bindings.ImGui.ImGui.SameLine();
          }
          if (isBunny)
            this.DrawView(frame, "Ears", width, 0.4f, template);
        }
      }
      else
      {
        this.DrawView(frame, "Face", 0.65f);
        Dalamud.Bindings.ImGui.ImGui.SameLine();
        using (ImRaii.Group())
        {
          this.DrawView(frame, "Lips", 0.35f, 0.5f);
          this.DrawView(frame, "Mouth", 0.35f, 0.5f);
        }
      }
      if (target.Pose == null)
        return;
      frame.DrawBones(target.Pose);
    }
  }

  private void DrawView(
    IViewFrame frame,
    string name,
    float width = 1f,
    float height = 1f,
    IDictionary<string, string>? template = null)
  {
    PoseViewEntry entry;
    if (this._schema == (PoseViewSchema) null || !this._schema.Views.TryGetValue(name, out entry))
      return;
    frame.DrawView(entry, width, height, template);
  }

  private enum ViewEnum
  {
    Body,
    Face,
  }
}
