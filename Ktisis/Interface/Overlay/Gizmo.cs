// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.Gizmo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Sections;
using Ktisis.ImGuizmo;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Overlay;

public class Gizmo : IGizmo
{
  private readonly GizmoConfig _cfg;
  private bool IsUsedPrev;
  private bool HasDrawn;
  private Matrix4x4 ViewMatrix = Matrix4x4.Identity;
  private Matrix4x4 ProjMatrix = Matrix4x4.Identity;

  public GizmoId Id { get; }

  public GizmoConfig Config => this._cfg;

  public Gizmo(GizmoConfig cfg, GizmoId id)
  {
    this._cfg = cfg;
    this.Id = id;
  }

  public float ScaleFactor { get; set; } = 0.5f;

  public Mode Mode { get; set; }

  public Operation Operation { get; set; } = Operation.UNIVERSAL;

  public bool AllowAxisFlip { get; set; } = true;

  public bool IsEnded { get; private set; }

  public void SetMatrix(Matrix4x4 view, Matrix4x4 proj)
  {
    this.ViewMatrix = view;
    this.ProjMatrix = proj;
  }

  public void BeginFrame(Vector2 pos, Vector2 size)
  {
    this.HasDrawn = false;
    Ktisis.ImGuizmo.Gizmo.SetDrawRect(pos.X, pos.Y, size.X, size.Y);
    if (this.Id != GizmoId.TransformEditor)
      this.ScaleFactor = !this.Operation.HasFlag((Enum) Operation.ROTATE) ? (!this.Operation.HasFlag((Enum) Operation.TRANSLATE) ? Math.Max(this.Config.ScaleGizmoScale, 0.05f) : Math.Max(this.Config.PositionGizmoScale, 0.05f)) : Math.Max(this.Config.RotationGizmoScale, 0.05f);
    Ktisis.ImGuizmo.Gizmo.ID = (int) this.Id;
    Ktisis.ImGuizmo.Gizmo.GizmoScale = this.ScaleFactor;
    Ktisis.ImGuizmo.Gizmo.AllowAxisFlip = this.AllowAxisFlip;
    Ktisis.ImGuizmo.Gizmo.Style = this._cfg.Style;
    Ktisis.ImGuizmo.Gizmo.BeginFrame();
    this.IsUsedPrev = Ktisis.ImGuizmo.Gizmo.IsUsing;
  }

  public unsafe void PushDrawList() => Ktisis.ImGuizmo.Gizmo.DrawList = (IntPtr) Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList().Handle;

  public bool Manipulate(ref Matrix4x4 mx, out Matrix4x4 delta)
  {
    delta = Matrix4x4.Identity;
    if (this.HasDrawn)
      return false;
    int num = Ktisis.ImGuizmo.Gizmo.Manipulate(this.ViewMatrix, this.ProjMatrix, this.Operation, this.Mode, ref mx, out delta) ? 1 : 0;
    this.HasDrawn = true;
    return num != 0;
  }

  public void EndFrame() => this.IsEnded = !Ktisis.ImGuizmo.Gizmo.IsUsing && this.IsUsedPrev;
}
