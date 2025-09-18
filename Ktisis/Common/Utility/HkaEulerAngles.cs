// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.HkaEulerAngles
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Common.Utility;

internal static class HkaEulerAngles {
	internal const float Deg2Rad = 0.0174532924f;
	internal const float Rad2Deg = 57.2957764f;
	private readonly static int EulFrmS = 0;
	private readonly static int EulFrmR = 1;
	private readonly static int EulRepNo = 0;
	private readonly static int EulRepYes = 1;
	private readonly static int EulParEven = 0;
	private readonly static int EulParOdd = 1;
	private readonly static int[] EulSafe = new int[4] { 0, 1, 2, 0 };
	private readonly static int[] EulNext = new int[4] { 1, 2, 0, 1 };
	private readonly static int Order = EulOrd(Axis.Z, EulParEven, EulRepNo, EulFrmS);

	private static void EulGetOrd(
		int ord,
		out int i,
		out int j,
		out int k,
		out int h,
		out int n,
		out int s,
		out int f
	) {
		var num1 = ord;
		f = num1 & 1;
		var num2 = num1 >> 1;
		s = num2 & 1;
		var num3 = num2 >> 1;
		n = num3 & 1;
		var num4 = num3 >> 1;
		i = EulSafe[num4 & 3];
		j = EulNext[i + n];
		k = EulNext[i + 1 - n];
		h = s == 1 ? k : i;
	}

	private static int EulOrd(Axis i, int p, int r, int f) => ((((int)i << 1) + p << 1) + r << 1) + f;

	internal static Vector3 MatrixToEuler(Matrix4x4 m) {
		Vector3 vector3 = new Vector3();
		var numArray = new float[4, 4] {
			{
				m.M11,
				m.M12,
				m.M13,
				m.M14
			}, {
				m.M21,
				m.M22,
				m.M23,
				m.M24
			}, {
				m.M31,
				m.M32,
				m.M33,
				m.M34
			}, {
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
		EulGetOrd(Order, out i, out j, out k, out var _, out n, out s, out f);
		if (s == EulRepYes) {
			var num = MathF.Sqrt((float)(numArray[i, j] * (double)numArray[i, j] + numArray[i, k] * (double)numArray[i, k]));
			if (num > 2.2420775429197073E-44) {
				vector3.X = MathF.Atan2(numArray[i, j], numArray[i, k]);
				vector3.Y = MathF.Atan2(num, numArray[i, i]);
				vector3.Z = MathF.Atan2(numArray[j, i], -numArray[k, i]);
			} else {
				vector3.X = MathF.Atan2(-numArray[j, k], numArray[j, j]);
				vector3.Y = MathF.Atan2(num, numArray[i, i]);
				vector3.Z = 0.0f;
			}
		} else {
			var num = MathF.Sqrt((float)(numArray[i, i] * (double)numArray[i, i] + numArray[j, i] * (double)numArray[j, i]));
			if (num > 2.2420775429197073E-44) {
				vector3.X = MathF.Atan2(numArray[k, j], numArray[k, k]);
				vector3.Y = MathF.Atan2(-numArray[k, i], num);
				vector3.Z = MathF.Atan2(numArray[j, i], numArray[i, i]);
			} else {
				vector3.X = MathF.Atan2(-numArray[j, k], numArray[j, j]);
				vector3.Y = MathF.Atan2(-numArray[k, i], num);
				vector3.Z = 0.0f;
			}
		}
		if (n == EulParOdd) {
			vector3.X = -vector3.X;
			vector3.Y = -vector3.Y;
			vector3.Z = -vector3.Z;
		}
		if (f == EulFrmR) {
			float x = vector3.X;
			vector3.X = vector3.Z;
			vector3.Z = x;
		}
		return new Vector3(vector3.Y, vector3.Z, vector3.X) * 57.2957764f;
	}

	internal static Matrix4x4 EulerToMatrix(Vector3 v) {
		Vector3 vector3 = new Vector3(v.Z, v.X, v.Y) * ((float)Math.PI / 180f);
		int i;
		int j;
		int k;
		int n;
		int s;
		int f;
		EulGetOrd(Order, out i, out j, out k, out var _, out n, out s, out f);
		if (f == EulFrmR) {
			float x = vector3.X;
			vector3.X = vector3.Z;
			vector3.Z = x;
		}
		if (n == EulParOdd) {
			vector3.X = -vector3.X;
			vector3.Y = -vector3.Y;
			vector3.Z = -vector3.Z;
		}
		var numArray = new float[4, 4];
		var x1 = (double)vector3.X;
		float y = vector3.Y;
		float z = vector3.Z;
		var num1 = MathF.Cos((float)x1);
		var num2 = MathF.Cos(y);
		var num3 = MathF.Cos(z);
		var num4 = MathF.Sin((float)x1);
		var num5 = MathF.Sin(y);
		var num6 = MathF.Sin(z);
		var num7 = num1 * num3;
		var num8 = num1 * num6;
		var num9 = num4 * num3;
		var num10 = num4 * num6;
		if (s == EulRepYes) {
			numArray[i, i] = num2;
			numArray[i, j] = num5 * num4;
			numArray[i, k] = num5 * num1;
			numArray[j, i] = num5 * num6;
			numArray[j, j] = -num2 * num10 + num7;
			numArray[j, k] = -num2 * num8 - num9;
			numArray[k, i] = -num5 * num3;
			numArray[k, j] = num2 * num9 + num8;
			numArray[k, k] = num2 * num7 - num10;
		} else {
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
		return new Matrix4x4(numArray[0, 0], numArray[0, 1], numArray[0, 2], numArray[0, 3], numArray[1, 0], numArray[1, 1], numArray[1, 2], numArray[1, 3], numArray[2, 0], numArray[2, 1], numArray[2, 2], numArray[2, 3], numArray[3, 0],
			numArray[3, 1], numArray[3, 2], numArray[3, 3]);
	}

	internal static Vector3 ToEuler(Quaternion q) {
		var num1 = (float)((double)q.X * (double)q.X + (double)q.Y * (double)q.Y + (double)q.Z * (double)q.Z + (double)q.W * (double)q.W);
		var num2 = num1 > 0.0 ? 2f / num1 : 0.0f;
		var num3 = q.X * num2;
		var num4 = q.Y * num2;
		var num5 = q.Z * num2;
		var num6 = q.W * num3;
		var num7 = q.W * num4;
		var num8 = q.W * num5;
		var num9 = q.X * num3;
		var num10 = q.X * num4;
		var num11 = q.X * num5;
		var num12 = q.Y * num4;
		var num13 = q.Y * num5;
		var num14 = q.Z * num5;
		return MatrixToEuler(new Matrix4x4((float)(1.0 - (num12 + (double)num14)), num10 - num8, num11 + num7, 0.0f, num10 + num8, (float)(1.0 - (num9 + (double)num14)), num13 - num6, 0.0f, num11 - num7, num13 + num6,
			(float)(1.0 - (num9 + (double)num12)), 0.0f, 0.0f, 0.0f, 0.0f, 1f)).NormalizeAngles();
	}

	public static Quaternion ToQuaternion(Vector3 v) {
		Vector3 vector3 = new Vector3(v.Z, v.X, v.Y) * ((float)Math.PI / 180f);
		Quaternion quaternion = new Quaternion();
		var numArray = new float[3];
		int i;
		int j;
		int k;
		int n;
		int s;
		int f;
		EulGetOrd(Order, out i, out j, out k, out var _, out n, out s, out f);
		if (f == EulFrmR) {
			float x = vector3.X;
			vector3.X = vector3.Z;
			vector3.Z = x;
		}
		if (n == EulParOdd)
			vector3.Y = -vector3.Y;
		var num1 = vector3.X * 0.5f;
		var num2 = vector3.Y * 0.5f;
		var num3 = (double)vector3.Z * 0.5;
		var num4 = MathF.Cos(num1);
		var num5 = MathF.Cos(num2);
		var num6 = MathF.Cos((float)num3);
		var num7 = MathF.Sin(num1);
		var num8 = MathF.Sin(num2);
		var num9 = MathF.Sin((float)num3);
		var num10 = num4 * num6;
		var num11 = num4 * num9;
		var num12 = num7 * num6;
		var num13 = num7 * num9;
		if (s == EulRepYes) {
			numArray[i] = num5 * (num11 + num12);
			numArray[j] = num8 * (num10 + num13);
			numArray[k] = num8 * (num11 - num12);
			quaternion.W = num5 * (num10 - num13);
		} else {
			numArray[i] = (float)(num5 * (double)num12 - num8 * (double)num11);
			numArray[j] = (float)(num5 * (double)num13 + num8 * (double)num10);
			numArray[k] = (float)(num5 * (double)num11 - num8 * (double)num12);
			quaternion.W = (float)(num5 * (double)num10 + num8 * (double)num13);
		}
		if (n == EulParOdd)
			numArray[j] = -numArray[j];
		quaternion.X = numArray[0];
		quaternion.Y = numArray[1];
		quaternion.Z = numArray[2];
		return quaternion;
	}

	private enum Axis {
		X,
		Y,
		Z,
		W
	}
}
