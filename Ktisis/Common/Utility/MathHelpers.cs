// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.MathHelpers
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Numerics;

#nullable disable
namespace Ktisis.Common.Utility;

public static class MathHelpers
{
  public static readonly float Deg2Rad = (float) Math.PI / 180f;
  public static readonly float Rad2Deg = 57.2957764f;

  public static Quaternion EulerAnglesToQuaternion(this Vector3 vec)
  {
    Vector3 vector3 = vec.NormalizeAngles() * MathHelpers.Deg2Rad;
    double num1 = (double) vector3.X * 0.5;
    float w1 = MathF.Cos((float) num1);
    float x = MathF.Sin((float) num1);
    double num2 = (double) vector3.Y * 0.5;
    float w2 = MathF.Cos((float) num2);
    float y = MathF.Sin((float) num2);
    double num3 = (double) vector3.Z * 0.5;
    float w3 = MathF.Cos((float) num3);
    float z = MathF.Sin((float) num3);
    Quaternion quaternion1 = new Quaternion(x, 0.0f, 0.0f, w1);
    Quaternion quaternion2 = new Quaternion(0.0f, y, 0.0f, w2);
    return new Quaternion(0.0f, 0.0f, z, w3) * quaternion2 * quaternion1;
  }

  private static float NormalizeAngle(float angle)
  {
    if ((double) angle > 360.0)
      angle = (float) (0.0 + (double) angle % 360.0);
    else if ((double) angle < -1.4012984643248171E-45)
      angle = (float) (360.0 - (360.0 - (double) angle) % 360.0);
    return angle;
  }

  public static Vector3 NormalizeAngles(this Vector3 vec)
  {
    return new Vector3(MathHelpers.NormalizeAngle(vec.X), MathHelpers.NormalizeAngle(vec.Y), MathHelpers.NormalizeAngle(vec.Z));
  }
}
