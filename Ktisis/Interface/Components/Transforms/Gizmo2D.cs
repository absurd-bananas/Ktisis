// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Transforms.Gizmo2D
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ktisis.ImGuizmo;
using Ktisis.Interface.Overlay;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Components.Transforms;

public class Gizmo2D
{
  public const float ScaleFactor = 0.5f;
  private readonly IGizmo Gizmo;

  public Gizmo2D(IGizmo gizmo)
  {
    this.Gizmo = gizmo;
    this.Gizmo.Operation = Operation.ROTATE;
    this.Gizmo.ScaleFactor = 0.5f;
  }

  public Mode Mode
  {
    get => this.Gizmo.Mode;
    set => this.Gizmo.Mode = value;
  }

  public bool IsEnded => this.Gizmo.IsEnded;

  public void SetLookAt(Vector3 cameraPos, Vector3 targetPos, float fov, float aspect = 1f)
  {
    Matrix4x4 perspectiveFieldOfView = Matrix4x4.CreatePerspectiveFieldOfView(fov, 1f, 0.1f, 100f);
    this.Gizmo.SetMatrix(Matrix4x4.CreateLookAt(cameraPos, targetPos, Vector3.UnitY), perspectiveFieldOfView);
  }

  public void Begin(Vector2 rectSize)
  {
    using (ImRaii.PushStyle((ImGuiStyleVar) 10, Vector2.Zero, true))
    {
      Dalamud.Bindings.ImGui.ImGui.BeginChildFrame((uint) (873568 + this.Gizmo.Id), rectSize);
      Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
      Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
      ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowPos(Vector2.Zero);
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowSize(((ImGuiIOPtr) ref io).DisplaySize);
      Dalamud.Bindings.ImGui.ImGui.Begin(ImU8String.op_Implicit("##Gizmo2D"), (ImGuiWindowFlags) 16777263 /*0x0100002F*/);
      float num = Math.Min(contentRegionAvail.X, contentRegionAvail.Y);
      Vector2 size = new Vector2(num, num);
      Vector2 vector2 = (contentRegionAvail - size) / 2f;
      Vector2 pos = cursorScreenPos + vector2;
      this.Gizmo.BeginFrame(pos, size);
      this.Gizmo.PushDrawList();
      Gizmo2D.DrawGizmoCircle(pos, size, size.X);
    }
  }

  private static void DrawGizmoCircle(Vector2 pos, Vector2 size, float width)
  {
    ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
    ((ImDrawListPtr) ref windowDrawList).AddCircleFilled(pos + size / 2f, (float) ((double) width * 0.5 / 2.0499999523162842), 3474989088U);
  }

  public bool Manipulate(ref Matrix4x4 matrix, out Matrix4x4 delta)
  {
    return this.Gizmo.Manipulate(ref matrix, out delta);
  }

  public void End()
  {
    this.Gizmo.EndFrame();
    Dalamud.Bindings.ImGui.ImGui.End();
    Dalamud.Bindings.ImGui.ImGui.EndChildFrame();
  }
}
