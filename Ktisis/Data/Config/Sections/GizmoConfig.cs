// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.GizmoConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.ImGuizmo;
using System.Numerics;

#nullable disable
namespace Ktisis.Data.Config.Sections;

public class GizmoConfig
{
  public bool Visible = true;
  public Mode Mode;
  public Operation Operation = Operation.ROTATE;
  public bool MirrorRotation;
  public bool ParentBones = true;
  public bool RelativeBones = true;
  public bool AllowAxisFlip = true;
  public bool AllowRaySnap = true;
  public float PositionGizmoScale = 0.1f;
  public float RotationGizmoScale = 0.1f;
  public float ScaleGizmoScale = 0.1f;
  public Style Style = GizmoConfig.DefaultStyle;
  public static readonly Style DefaultStyle = new Style()
  {
    TranslationLineThickness = 3f,
    TranslationLineArrowSize = 6f,
    RotationLineThickness = 2f,
    RotationOuterLineThickness = 3f,
    ScaleLineThickness = 3f,
    ScaleLineCircleSize = 6f,
    HatchedAxisLineThickness = 6f,
    CenterCircleSize = 6f,
    ColorDirectionX = new Vector4(0.666f, 0.0f, 0.0f, 1f),
    ColorDirectionY = new Vector4(0.0f, 0.666f, 0.0f, 1f),
    ColorDirectionZ = new Vector4(0.0f, 0.0f, 0.666f, 1f),
    ColorPlaneX = new Vector4(0.666f, 0.0f, 0.0f, 0.38f),
    ColorPlaneY = new Vector4(0.0f, 0.666f, 0.0f, 0.38f),
    ColorPlaneZ = new Vector4(0.0f, 0.0f, 0.666f, 0.38f),
    ColorSelection = new Vector4(1f, 0.5f, 0.062f, 0.541f),
    ColorInactive = new Vector4(0.6f, 0.6f, 0.6f, 0.6f),
    ColorTranslationLine = new Vector4(0.666f, 0.666f, 0.666f, 0.666f),
    ColorScaleLine = new Vector4(0.25f, 0.25f, 0.25f, 1f),
    ColorRotationUsingBorder = new Vector4(1f, 0.5f, 0.062f, 1f),
    ColorRotationUsingFill = new Vector4(1f, 0.5f, 0.062f, 0.5f),
    ColorHatchedAxisLines = new Vector4(0.0f, 0.0f, 0.0f, 0.5f),
    ColorText = new Vector4(1f, 1f, 1f, 1f),
    ColorTextShadow = new Vector4(0.0f, 0.0f, 0.0f, 1f)
  };
}
