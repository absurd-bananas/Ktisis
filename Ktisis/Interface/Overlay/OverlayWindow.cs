// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.OverlayWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Math;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Transforms.Types;
using Ktisis.ImGuizmo;
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities;
using Ktisis.Services.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Overlay;

public class OverlayWindow : KtisisWindow
{
  private const ImGuiWindowFlags WindowFlags = (ImGuiWindowFlags) 795563;
  private readonly IGameGui _gui;
  private readonly IEditorContext _ctx;
  private readonly IGizmo _gizmo;
  private readonly SceneDraw _sceneDraw;
  private ITransformMemento? Transform;

  public OverlayWindow(IGameGui gui, IEditorContext ctx, IGizmo gizmo, SceneDraw draw)
    : base("##KtisisOverlay", (ImGuiWindowFlags) 795563)
  {
    this._gui = gui;
    this._ctx = ctx;
    this._gizmo = gizmo;
    this._sceneDraw = draw;
    this._sceneDraw.SetContext(ctx);
    this.PositionCondition = (ImGuiCond) 1;
  }

  public virtual void PreOpenCheck()
  {
    if (this._ctx.IsValid)
      return;
    Ktisis.Ktisis.Log.Verbose("Context for overlay window is stale, closing...", Array.Empty<object>());
    this.Close();
  }

  public virtual void PreDraw()
  {
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    this.Size = new Vector2?(((ImGuiIOPtr) ref io).DisplaySize);
    this.Position = new Vector2?(Vector2.op_Implicit(Vector2.Zero));
  }

  public virtual void Draw()
  {
    if (!this._ctx.Config.Overlay.Visible)
      return;
    this._sceneDraw.DrawScene(this.DrawGizmo(), this._gizmo.IsEnded);
  }

  private bool DrawGizmo()
  {
    if (!this._ctx.Config.Gizmo.Visible)
      return false;
    ITransformTarget target = this._ctx.Transform.Target;
    Ktisis.Common.Utility.Transform transform = target?.GetTransform();
    if (target == null || transform == null)
      return false;
    Matrix4x4? viewMatrix = CameraService.GetViewMatrix();
    Matrix4x4? projectionMatrix = CameraService.GetProjectionMatrix();
    if (!viewMatrix.HasValue || !projectionMatrix.HasValue || !this.Size.HasValue)
      return false;
    Vector2 size = this.Size.Value;
    this._gizmo.SetMatrix(viewMatrix.Value, projectionMatrix.Value);
    this._gizmo.BeginFrame(Vector2.op_Implicit(Vector2.Zero), size);
    GizmoConfig gizmo = this._ctx.Config.Gizmo;
    this._gizmo.Mode = gizmo.Mode;
    this._gizmo.Operation = gizmo.Operation;
    this._gizmo.AllowAxisFlip = gizmo.AllowAxisFlip;
    Matrix4x4 matrix = transform.ComposeMatrix();
    if (this._gizmo.Manipulate(ref matrix, out Matrix4x4 _) | this.HandleShiftRaycast(ref matrix))
    {
      if (this.Transform == null)
        this.Transform = this._ctx.Transform.Begin(target);
      this.Transform.SetMatrix(matrix);
    }
    this._gizmo.EndFrame();
    if (this._gizmo.IsEnded)
    {
      this.Transform?.Dispatch();
      this.Transform = (ITransformMemento) null;
    }
    return true;
  }

  private bool HandleShiftRaycast(ref Matrix4x4 matrix)
  {
    Vector3 vector3;
    if (!this._ctx.Config.Gizmo.AllowRaySnap || !Dalamud.Bindings.ImGui.ImGui.IsKeyDown((ImGuiKey) 642) || !Ktisis.ImGuizmo.Gizmo.IsUsing || Ktisis.ImGuizmo.Gizmo.CurrentOperation != Operation.TRANSLATE || !this._gui.ScreenToWorld(Dalamud.Bindings.ImGui.ImGui.GetMousePos(), ref vector3, 100000f))
      return false;
    matrix.Translation = vector3;
    return true;
  }

  private void DrawDebug(Stopwatch t)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(((ImGuiStylePtr) ref style).WindowPadding.Y);
    for (int index = 0; index < 5; ++index)
      Dalamud.Bindings.ImGui.ImGui.Spacing();
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(12, 2);
    ((ImU8String) ref imU8String).AppendLiteral("Context: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(this._ctx.GetHashCode(), "X");
    ((ImU8String) ref imU8String).AppendLiteral(" (");
    ((ImU8String) ref imU8String).AppendFormatted<bool>(this._ctx.IsValid);
    ((ImU8String) ref imU8String).AppendLiteral(")");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(10, 2);
    ((ImU8String) ref imU8String).AppendLiteral("Scene: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(this._ctx.Scene.GetHashCode(), "X");
    ((ImU8String) ref imU8String).AppendLiteral(" ");
    ((ImU8String) ref imU8String).AppendFormatted<double>(this._ctx.Scene.UpdateTime, "00.00");
    ((ImU8String) ref imU8String).AppendLiteral("ms");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(12, 2);
    ((ImU8String) ref imU8String).AppendLiteral("Overlay: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(((object) this).GetHashCode());
    ((ImU8String) ref imU8String).AppendLiteral(" ");
    ((ImU8String) ref imU8String).AppendFormatted<double>(t.Elapsed.TotalMilliseconds, "00.00");
    ((ImU8String) ref imU8String).AppendLiteral("ms");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(13, 4);
    ((ImU8String) ref imU8String).AppendLiteral("Gizmo: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(this._gizmo.GetHashCode(), "X");
    ((ImU8String) ref imU8String).AppendLiteral(" ");
    ((ImU8String) ref imU8String).AppendFormatted<GizmoId>(this._gizmo.Id);
    ((ImU8String) ref imU8String).AppendLiteral(" (");
    ((ImU8String) ref imU8String).AppendFormatted<Operation>(this._gizmo.Operation);
    ((ImU8String) ref imU8String).AppendLiteral(", ");
    ((ImU8String) ref imU8String).AppendFormatted<bool>(Ktisis.ImGuizmo.Gizmo.IsUsing);
    ((ImU8String) ref imU8String).AppendLiteral(")");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    ITransformTarget target = this._ctx.Transform.Target;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(14, 4);
    ((ImU8String) ref imU8String).AppendLiteral("Target: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(target != null ? target.GetHashCode() : 0, "X7");
    ((ImU8String) ref imU8String).AppendLiteral(" ");
    ((ImU8String) ref imU8String).AppendFormatted<string>(target?.GetType().Name ?? "NULL");
    ((ImU8String) ref imU8String).AppendLiteral(" (");
    ref ImU8String local = ref imU8String;
    int? nullable;
    if (target == null)
    {
      nullable = new int?();
    }
    else
    {
      IEnumerable<SceneEntity> targets = target.Targets;
      nullable = targets != null ? new int?(targets.Count<SceneEntity>()) : new int?();
    }
    int valueOrDefault = nullable.GetValueOrDefault();
    ((ImU8String) ref local).AppendFormatted<int>(valueOrDefault);
    ((ImU8String) ref imU8String).AppendLiteral(", ");
    ((ImU8String) ref imU8String).AppendFormatted<string>(target?.Primary?.Name ?? "NULL");
    ((ImU8String) ref imU8String).AppendLiteral(")");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    IHistoryManager history = this._ctx.Actions.History;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(14, 3);
    ((ImU8String) ref imU8String).AppendLiteral("History: ");
    ((ImU8String) ref imU8String).AppendFormatted<int>(history.Count);
    ((ImU8String) ref imU8String).AppendLiteral(" (");
    ((ImU8String) ref imU8String).AppendFormatted<bool>(history.CanUndo);
    ((ImU8String) ref imU8String).AppendLiteral(", ");
    ((ImU8String) ref imU8String).AppendFormatted<bool>(history.CanRedo);
    ((ImU8String) ref imU8String).AppendLiteral(")");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
  }
}
