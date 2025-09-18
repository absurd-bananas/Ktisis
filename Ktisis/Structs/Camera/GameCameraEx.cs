// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Camera.GameCameraEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Game.Control;
using Ktisis.Common.Utility;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Camera;

[StructLayout(LayoutKind.Explicit)]
public struct GameCameraEx
{
  [FieldOffset(0)]
  public FFXIVClientStructs.FFXIV.Client.Game.Camera GameCamera;
  [FieldOffset(96 /*0x60*/)]
  public Vector3 Position;
  [FieldOffset(292)]
  public float Distance;
  [FieldOffset(296)]
  public float DistanceMin;
  [FieldOffset(300)]
  public float DistanceMax;
  [FieldOffset(316)]
  public float Zoom;
  [FieldOffset(320)]
  public Vector2 Angle;
  [FieldOffset(348)]
  public float YMin;
  [FieldOffset(344)]
  public float YMax;
  [FieldOffset(352)]
  public Vector2 Pan;
  [FieldOffset(368)]
  public float Rotation;
  [FieldOffset(536)]
  public Vector2 DistanceCollide;

  public Quaternion CalcPointDirection()
  {
    return (new Vector3((float) -((double) this.Angle.Y + (double) this.Pan.Y), (float) (((double) this.Angle.X + 3.1415901184082031) % 6.28318977355957) - this.Pan.X, 0.0f) * MathHelpers.Rad2Deg).EulerAnglesToQuaternion();
  }

  public Vector3 CalcRotation()
  {
    return new Vector3(this.Angle.X - this.Pan.X, -this.Angle.Y - this.Pan.Y, this.Rotation);
  }

  public unsafe RenderCameraEx* RenderEx
  {
    get => (RenderCameraEx*) this.GameCamera.CameraBase.SceneCamera.RenderCamera;
  }

  public static unsafe GameCameraEx* GetActive()
  {
    return (GameCameraEx*) ((CameraManager) (IntPtr) CameraManager.Instance()).GetActiveCamera();
  }
}
