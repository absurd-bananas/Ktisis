// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.HkaEulerAngles
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Common.Utility;

internal static class HkaEulerAngles
{
  internal const float Deg2Rad = 0.0174532924f;
  internal const float Rad2Deg = 57.2957764f;
  private static int EulFrmS = 0;
  private static int EulFrmR = 1;
  private static int EulRepNo = 0;
  private static int EulRepYes = 1;
  private static int EulParEven = 0;
  private static int EulParOdd = 1;
  private static int[] EulSafe = new int[4]{ 0, 1, 2, 0 };
  private static int[] EulNext = new int[4]{ 1, 2, 0, 1 };
  private static int Order = HkaEulerAngles.EulOrd(HkaEulerAngles.Axis.Z, HkaEulerAngles.EulParEven, HkaEulerAngles.EulRepNo, HkaEulerAngles.EulFrmS);

  private static void EulGetOrd(
    int ord,
    out int i,
    out int j,
    out int k,
    out int h,
    out int n,
    out int s,
    out int f)
  {
    int num1 = ord;
    f = num1 & 1;
    int num2 = num1 >> 1;
    s = num2 & 1;
    int num3 = num2 >> 1;
    n = num3 & 1;
    int num4 = num3 >> 1;
    i = HkaEulerAngles.EulSafe[num4 & 3];
    j = HkaEulerAngles.EulNext[i + n];
    k = HkaEulerAngles.EulNext[i + 1 - n];
    h = s == 1 ? k : i;
  }

  private static int EulOrd(HkaEulerAngles.Axis i, int p, int r, int f)
  {
    return ((((int) i << 1) + p << 1) + r << 1) + f;
  }

  internal static Vector3 MatrixToEuler(Matrix4x4 m)
  {
    Vector3 vector3 = new Vector3();
    float[,] numArray = new float[4, 4]
    {
      {
        m.M11,
        m.M12,
        m.M13,
        m.M14
      },
      {
        m.M21,
        m.M22,
        m.M23,
        m.M24
      },
      {
        m.M31,
        m.M32,
        m.M33,
        m.M34
      },
      {
        m.M41,
        m.M42,
        m.M43,
        m.M44
      }
    };
    int i;
    int j;
    int k;
    int n;
    int s;
    int f;
    HkaEulerAngles.EulGetOrd(HkaEulerAngles.Order, out i, out j, out k, out int _, out n, out s, out f);
    if (s == HkaEulerAngles.EulRepYes)
    {
      float num = MathF.Sqrt((float) ((double) numArray[i, j] * (double) numArray[i, j] + (double) numArray[i, k] * (double) numArray[i, k]));
      if ((double) num > 2.2420775429197073E-44)
      {
        vector3.X = MathF.Atan2(numArray[i, j], numArray[i, k]);
        vector3.Y = MathF.Atan2(num, numArray[i, i]);
        vector3.Z = MathF.Atan2(numArray[j, i], -numArray[k, i]);
      }
      else
      {
        vector3.X = MathF.Atan2(-numArray[j, k], numArray[j, j]);
        vector3.Y = MathF.Atan2(num, numArray[i, i]);
        vector3.Z = 0.0f;
      }
    }
    else
    {
      float num = MathF.Sqrt((float) ((double) numArray[i, i] * (double) numArray[i, i] + (double) numArray[j, i] * (double) numArray[j, i]));
      if ((double) num > 2.2420775429197073E-44)
      {
        vector3.X = MathF.Atan2(numArray[k, j], numArray[k, k]);
        vector3.Y = MathF.Atan2(-numArray[k, i], num);
        vector3.Z = MathF.Atan2(numArray[j, i], numArray[i, i]);
      }
      else
      {
        vector3.X = MathF.Atan2(-numArray[j, k], numArray[j, j]);
        vector3.Y = MathF.Atan2(-numArray[k, i], num);
        vector3.Z = 0.0f;
      }
    }
    if (n == HkaEulerAngles.EulParOdd)
    {
      vector3.X = -vector3.X;
      vector3.Y = -vector3.Y;
      vector3.Z = -vector3.Z;
    }
    if (f == HkaEulerAngles.EulFrmR)
    {
      float x = vector3.X;
      vector3.X = vector3.Z;
      vector3.Z = x;
    }
    return new Vector3(vector3.Y, vector3.Z, vector3.X) * 57.2957764f;
  }

  internal static Matrix4x4 EulerToMatrix(Vector3 v)
  {
    Vector3 vector3 = new Vector3(v.Z, v.X, v.Y) * ((float) Math.PI / 180f);
    int i;
    int j;
    int k;
    int n;
    int s;
    int f;
    HkaEulerAngles.EulGetOrd(HkaEulerAngles.Order, out i, out j, out k, out int _, out n, out s, out f);
    if (f == HkaEulerAngles.EulFrmR)
    {
      float x = vector3.X;
      vector3.X = vector3.Z;
      vector3.Z = x;
    }
    if (n == HkaEulerAngles.EulParOdd)
    {
      vector3.X = -vector3.X;
      vector3.Y = -vector3.Y;
      vector3.Z = -vector3.Z;
    }
    float[,] numArray = new float[4, 4];
    double x1 = (double) vector3.X;
    float y = vector3.Y;
    float z = vector3.Z;
    float num1 = MathF.Cos((float) x1);
    float num2 = MathF.Cos(y);
    float num3 = MathF.Cos(z);
    float num4 = MathF.Sin((float) x1);
    float num5 = MathF.Sin(y);
    float num6 = MathF.Sin(z);
    float num7 = num1 * num3;
    float num8 = num1 * num6;
    float num9 = num4 * num3;
    float num10 = num4 * num6;
    if (s == HkaEulerAngles.EulRepYes)
    {
      numArray[i, i] = num2;
      numArray[i, j] = num5 * num4;
      numArray[i, k] = num5 * num1;
      numArray[j, i] = num5 * num6;
      numArray[j, j] = -num2 * num10 + num7;
      numArray[j, k] = -num2 * num8 - num9;
      numArray[k, i] = -num5 * num3;
      numArray[k, j] = num2 * num9 + num8;
      numArray[k, k] = num2 * num7 - num10;
    }
    else
    {
      numArray[i, i] = num2 * num3;
      numArray[i, j] = num5 * num9 - num8;
      numArray[i, k] = num5 * num7 + num10;
      numArray[j, i] = num2 * num6;
      numArray[j, j] = num5 * num10 + num7;
      numArray[j, k] = num5 * num8 - num9;
      numArray[k, i] = -num5;
      numArray[k, j] = num2 * num4;
      numArray[k, k] = num2 * num1;
    }
    return new Matrix4x4(numArray[0, 0], numArray[0, 1], numArray[0, 2], numArray[0, 3], numArray[1, 0], numArray[1, 1], numArray[1, 2], numArray[1, 3], numArray[2, 0], numArray[2, 1], numArray[2, 2], numArray[2, 3], numArray[3, 0], numArray[3, 1], numArray[3, 2], numArray[3, 3]);
  }

  internal static Vector3 ToEuler(Quaternion q)
  {
    float num1 = (float) ((double) q.X * (double) q.X + (double) q.Y * (double) q.Y + (double) q.Z * (double) q.Z + (double) q.W * (double) q.W);
    float num2 = (double) num1 > 0.0 ? 2f / num1 : 0.0f;
    float num3 = q.X * num2;
    float num4 = q.Y * num2;
    float num5 = q.Z * num2;
    float num6 = q.W * num3;
    float num7 = q.W * num4;
    float num8 = q.W * num5;
    float num9 = q.X * num3;
    float num10 = q.X * num4;
    float num11 = q.X * num5;
    float num12 = q.Y * num4;
    float num13 = q.Y * num5;
    float num14 = q.Z * num5;
    return HkaEulerAngles.MatrixToEuler(new Matrix4x4((float) (1.0 - ((double) num12 + (double) num14)), num10 - num8, num11 + num7, 0.0f, num10 + num8, (float) (1.0 - ((double) num9 + (double) num14)), num13 - num6, 0.0f, num11 - num7, num13 + num6, (float) (1.0 - ((double) num9 + (double) num12)), 0.0f, 0.0f, 0.0f, 0.0f, 1f)).NormalizeAngles();
  }

  public static Quaternion ToQuaternion(Vector3 v)
  {
    Vector3 vector3 = new Vector3(v.Z, v.X, v.Y) * ((float) Math.PI / 180f);
    Quaternion quaternion = new Quaternion();
    float[] numArray = new float[3];
    int i;
    int j;
    int k;
    int n;
    int s;
    int f;
    HkaEulerAngles.EulGetOrd(HkaEulerAngles.Order, out i, out j, out k, out int _, out n, out s, out f);
    if (f == HkaEulerAngles.EulFrmR)
    {
      float x = vector3.X;
      vector3.X = vector3.Z;
      vector3.Z = x;
    }
    if (n == HkaEulerAngles.EulParOdd)
      vector3.Y = -vector3.Y;
    float num1 = vector3.X * 0.5f;
    float num2 = vector3.Y * 0.5f;
    double num3 = (double) vector3.Z * 0.5;
    float num4 = MathF.Cos(num1);
    float num5 = MathF.Cos(num2);
    float num6 = MathF.Cos((float) num3);
    float num7 = MathF.Sin(num1);
    float num8 = MathF.Sin(num2);
    float num9 = MathF.Sin((float) num3);
    float num10 = num4 * num6;
    float num11 = num4 * num9;
    float num12 = num7 * num6;
    float num13 = num7 * num9;
    if (s == HkaEulerAngles.EulRepYes)
    {
      numArray[i] = num5 * (num11 + num12);
      numArray[j] = num8 * (num10 + num13);
      numArray[k] = num8 * (num11 - num12);
      quaternion.W = num5 * (num10 - num13);
    }
    else
    {
      numArray[i] = (float) ((double) num5 * (double) num12 - (double) num8 * (double) num11);
      numArray[j] = (float) ((double) num5 * (double) num13 + (double) num8 * (double) num10);
      numArray[k] = (float) ((double) num5 * (double) num11 - (double) num8 * (double) num12);
      quaternion.W = (float) ((double) num5 * (double) num10 + (double) num8 * (double) num13);
    }
    if (n == HkaEulerAngles.EulParOdd)
      numArray[j] = -numArray[j];
    quaternion.X = numArray[0];
    quaternion.Y = numArray[1];
    quaternion.Z = numArray[2];
    return quaternion;
  }

  private enum Axis
  {
    X,
    Y,
    Z,
    W,
  }
}
