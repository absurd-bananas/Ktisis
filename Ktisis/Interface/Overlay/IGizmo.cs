// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.IGizmo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Sections;
using Ktisis.ImGuizmo;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Overlay;

public interface IGizmo
{
  GizmoId Id { get; }

  GizmoConfig Config { get; }

  float ScaleFactor { get; set; }

  Mode Mode { get; set; }

  Operation Operation { get; set; }

  bool AllowAxisFlip { get; set; }

  bool IsEnded { get; }

  void SetMatrix(Matrix4x4 view, Matrix4x4 proj);

  void BeginFrame(Vector2 pos, Vector2 size);

  void PushDrawList();

  bool Manipulate(ref Matrix4x4 mx, out Matrix4x4 delta);

  void EndFrame();
}
