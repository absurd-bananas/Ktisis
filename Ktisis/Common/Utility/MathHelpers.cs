// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.MathHelpers
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Common.Utility;

public static class MathHelpers {
	public readonly static float Deg2Rad = (float)Math.PI / 180f;
	public readonly static float Rad2Deg = 57.2957764f;

	public static Quaternion EulerAnglesToQuaternion(this Vector3 vec) {
		Vector3 vector3 = vec.NormalizeAngles() * Deg2Rad;
		var num1 = (double)vector3.X * 0.5;
		var w1 = MathF.Cos((float)num1);
		var x = MathF.Sin((float)num1);
		var num2 = (double)vector3.Y * 0.5;
		var w2 = MathF.Cos((float)num2);
		var y = MathF.Sin((float)num2);
		var num3 = (double)vector3.Z * 0.5;
		var w3 = MathF.Cos((float)num3);
		var z = MathF.Sin((float)num3);
		Quaternion quaternion1 = new Quaternion(x, 0.0f, 0.0f, w1);
		Quaternion quaternion2 = new Quaternion(0.0f, y, 0.0f, w2);
		return new Quaternion(0.0f, 0.0f, z, w3) * quaternion2 * quaternion1;
	}

	private static float NormalizeAngle(float angle) {
		if (angle > 360.0)
			angle = (float)(0.0 + angle % 360.0);
		else if (angle < -1.4012984643248171E-45)
			angle = (float)(360.0 - (360.0 - angle) % 360.0);
		return angle;
	}

	public static Vector3 NormalizeAngles(this Vector3 vec) => new Vector3(NormalizeAngle(vec.X), NormalizeAngle(vec.Y), NormalizeAngle(vec.Z));
}
