// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.Transform
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using Ktisis.Common.Extensions;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Ktisis.Common.Utility;

[StructLayout(LayoutKind.Explicit)]
public class Transform
{
  [FieldOffset(0)]
  public Vector3 Position;
  [FieldOffset(16 /*0x10*/)]
  public Quaternion Rotation;
  [FieldOffset(32 /*0x20*/)]
  public Vector3 Scale;

  public Transform()
  {
    this.Position = Vector3.Zero;
    this.Rotation = Quaternion.Identity;
    this.Scale = Vector3.One;
  }

  public Transform(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    this.Position = pos;
    this.Rotation = rot;
    this.Scale = scale;
  }

  public Transform(hkQsTransformf hk)
  {
    this.Position = hk.Translation.ToVector3();
    this.Rotation = hk.Rotation.ToQuaternion();
    this.Scale = hk.Scale.ToVector3();
  }

  public Transform(Transform trans)
  {
    this.Position = Vector3.op_Implicit(trans.Position);
    this.Rotation = Quaternion.op_Implicit(trans.Rotation);
    this.Scale = Vector3.op_Implicit(trans.Scale);
  }

  public Transform(Matrix4x4 mx) => this.DecomposeMatrix(mx);

  public Matrix4x4 ComposeMatrix()
  {
    Matrix4x4 scale = Matrix4x4.CreateScale(this.Scale);
    Matrix4x4 fromQuaternion = Matrix4x4.CreateFromQuaternion(this.Rotation);
    Matrix4x4 translation = Matrix4x4.CreateTranslation(this.Position);
    Matrix4x4 matrix4x4 = fromQuaternion;
    return scale * matrix4x4 * translation;
  }

  public void DecomposeMatrix(Matrix4x4 mx)
  {
    Vector3 scale;
    Quaternion rotation;
    Vector3 translation;
    Matrix4x4.Decompose(mx, out scale, out rotation, out translation);
    this.Position = translation;
    this.Rotation = rotation;
    this.Scale = scale;
  }

  public Transform Set(Transform t)
  {
    this.Position = t.Position;
    this.Rotation = t.Rotation;
    this.Scale = t.Scale;
    return this;
  }

  public static implicit operator Transform(Transform trans)
  {
    return new Transform()
    {
      Position = Vector3.op_Implicit(trans.Position),
      Rotation = Quaternion.op_Implicit(trans.Rotation),
      Scale = Vector3.op_Implicit(trans.Scale)
    };
  }
}
