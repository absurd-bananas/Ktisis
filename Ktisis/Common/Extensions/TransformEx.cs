// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.TransformEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.Havok.Common.Base.Math.Quaternion;
using FFXIVClientStructs.Havok.Common.Base.Math.Vector;
using Ktisis.Common.Utility;
using System.Numerics;

#nullable enable
namespace Ktisis.Common.Extensions;

public static class TransformEx
{
  public static Vector3 ModelToWorldPos(this Vector3 target, Transform offset)
  {
    return Vector3.Transform(target, offset.Rotation) * offset.Scale;
  }

  public static Vector3 WorldToModelPos(this Vector3 target, Transform offset)
  {
    return Vector3.Transform(target - offset.Position, System.Numerics.Quaternion.Inverse(offset.Rotation)) / offset.Scale;
  }

  public static Vector3 ToVector3(this hkVector4f hkVec) => new Vector3(hkVec.X, hkVec.Y, hkVec.Z);

  public static System.Numerics.Quaternion ToQuaternion(this hkQuaternionf hkQuat)
  {
    return new System.Numerics.Quaternion(hkQuat.X, hkQuat.Y, hkQuat.Z, hkQuat.W);
  }

  public static hkQuaternionf ToHavok(this System.Numerics.Quaternion quat)
  {
    return new hkQuaternionf()
    {
      X = quat.X,
      Y = quat.Y,
      Z = quat.Z,
      W = quat.W
    };
  }
}
