// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Havok.TwoJointsIkSetup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Havok;

[StructLayout(LayoutKind.Explicit)]
public struct TwoJointsIkSetup {
	[FieldOffset(0)]
	public short m_firstJointIdx;
	[FieldOffset(2)]
	public short m_secondJointIdx;
	[FieldOffset(4)]
	public short m_endBoneIdx;
	[FieldOffset(6)]
	public short m_firstJointTwistIdx;
	[FieldOffset(8)]
	public short m_secondJointTwistIdx;
	[FieldOffset(16 /*0x10*/)]
	public Vector4 m_hingeAxisLS;
	[FieldOffset(32 /*0x20*/)]
	public float m_cosineMaxHingeAngle;
	[FieldOffset(36)]
	public float m_cosineMinHingeAngle;
	[FieldOffset(40)]
	public float m_firstJointIkGain;
	[FieldOffset(44)]
	public float m_secondJointIkGain;
	[FieldOffset(48 /*0x30*/)]
	public float m_endJointIkGain;
	[FieldOffset(64 /*0x40*/)]
	public Vector4 m_endTargetMS;
	[FieldOffset(80 /*0x50*/)]
	public Quaternion m_endTargetRotationMS;
	[FieldOffset(96 /*0x60*/)]
	public Vector4 m_endBoneOffsetLS;
	[FieldOffset(112 /*0x70*/)]
	public Quaternion m_endBoneRotationOffsetLS;
	[FieldOffset(128 /*0x80*/)]
	public bool m_enforceEndPosition;
	[FieldOffset(129)]
	public bool m_enforceEndRotation;
}
