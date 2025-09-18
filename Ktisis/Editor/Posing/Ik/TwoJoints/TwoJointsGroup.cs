// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.TwoJoints.TwoJointsGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Ik.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Editor.Posing.Ik.TwoJoints;

public record TwoJointsGroup() : IIkGroup
{
  public TwoJointsMode Mode;
  public short FirstBoneIndex;
  public short FirstTwistIndex;
  public short SecondBoneIndex;
  public short SecondTwistIndex;
  public short EndBoneIndex;
  public float FirstBoneGain;
  public float SecondBoneGain;
  public float EndBoneGain;
  public float MaxHingeAngle;
  public float MinHingeAngle;
  public Vector3 HingeAxis;
  public bool EnforcePosition;
  public Vector3 TargetPosition;
  public bool EnforceRotation;
  public Quaternion TargetRotation;

  public bool IsEnabled { get; set; }

  public uint SkeletonId { get; set; }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("IsEnabled = ");
    builder.Append(this.IsEnabled.ToString());
    builder.Append(", SkeletonId = ");
    builder.Append(this.SkeletonId.ToString());
    builder.Append(", Mode = ");
    builder.Append(this.Mode.ToString());
    builder.Append(", FirstBoneIndex = ");
    builder.Append(this.FirstBoneIndex.ToString());
    builder.Append(", FirstTwistIndex = ");
    builder.Append(this.FirstTwistIndex.ToString());
    builder.Append(", SecondBoneIndex = ");
    builder.Append(this.SecondBoneIndex.ToString());
    builder.Append(", SecondTwistIndex = ");
    builder.Append(this.SecondTwistIndex.ToString());
    builder.Append(", EndBoneIndex = ");
    builder.Append(this.EndBoneIndex.ToString());
    builder.Append(", FirstBoneGain = ");
    builder.Append(this.FirstBoneGain.ToString());
    builder.Append(", SecondBoneGain = ");
    builder.Append(this.SecondBoneGain.ToString());
    builder.Append(", EndBoneGain = ");
    builder.Append(this.EndBoneGain.ToString());
    builder.Append(", MaxHingeAngle = ");
    builder.Append(this.MaxHingeAngle.ToString());
    builder.Append(", MinHingeAngle = ");
    builder.Append(this.MinHingeAngle.ToString());
    builder.Append(", HingeAxis = ");
    builder.Append(this.HingeAxis.ToString());
    builder.Append(", EnforcePosition = ");
    builder.Append(this.EnforcePosition.ToString());
    builder.Append(", TargetPosition = ");
    builder.Append(this.TargetPosition.ToString());
    builder.Append(", EnforceRotation = ");
    builder.Append(this.EnforceRotation.ToString());
    builder.Append(", TargetRotation = ");
    builder.Append(this.TargetRotation.ToString());
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return (((((((((((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.\u003CIsEnabled\u003Ek__BackingField)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(this.\u003CSkeletonId\u003Ek__BackingField)) * -1521134295 + EqualityComparer<TwoJointsMode>.Default.GetHashCode(this.Mode)) * -1521134295 + EqualityComparer<short>.Default.GetHashCode(this.FirstBoneIndex)) * -1521134295 + EqualityComparer<short>.Default.GetHashCode(this.FirstTwistIndex)) * -1521134295 + EqualityComparer<short>.Default.GetHashCode(this.SecondBoneIndex)) * -1521134295 + EqualityComparer<short>.Default.GetHashCode(this.SecondTwistIndex)) * -1521134295 + EqualityComparer<short>.Default.GetHashCode(this.EndBoneIndex)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.FirstBoneGain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.SecondBoneGain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.EndBoneGain)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.MaxHingeAngle)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.MinHingeAngle)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.HingeAxis)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.EnforcePosition)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.TargetPosition)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.EnforceRotation)) * -1521134295 + EqualityComparer<Quaternion>.Default.GetHashCode(this.TargetRotation);
  }

  [CompilerGenerated]
  public virtual bool Equals(TwoJointsGroup? other)
  {
    if ((object) this == (object) other)
      return true;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<bool>.Default.Equals(this.\u003CIsEnabled\u003Ek__BackingField, other.\u003CIsEnabled\u003Ek__BackingField) && EqualityComparer<uint>.Default.Equals(this.\u003CSkeletonId\u003Ek__BackingField, other.\u003CSkeletonId\u003Ek__BackingField) && EqualityComparer<TwoJointsMode>.Default.Equals(this.Mode, other.Mode) && EqualityComparer<short>.Default.Equals(this.FirstBoneIndex, other.FirstBoneIndex) && EqualityComparer<short>.Default.Equals(this.FirstTwistIndex, other.FirstTwistIndex) && EqualityComparer<short>.Default.Equals(this.SecondBoneIndex, other.SecondBoneIndex) && EqualityComparer<short>.Default.Equals(this.SecondTwistIndex, other.SecondTwistIndex) && EqualityComparer<short>.Default.Equals(this.EndBoneIndex, other.EndBoneIndex) && EqualityComparer<float>.Default.Equals(this.FirstBoneGain, other.FirstBoneGain) && EqualityComparer<float>.Default.Equals(this.SecondBoneGain, other.SecondBoneGain) && EqualityComparer<float>.Default.Equals(this.EndBoneGain, other.EndBoneGain) && EqualityComparer<float>.Default.Equals(this.MaxHingeAngle, other.MaxHingeAngle) && EqualityComparer<float>.Default.Equals(this.MinHingeAngle, other.MinHingeAngle) && EqualityComparer<Vector3>.Default.Equals(this.HingeAxis, other.HingeAxis) && EqualityComparer<bool>.Default.Equals(this.EnforcePosition, other.EnforcePosition) && EqualityComparer<Vector3>.Default.Equals(this.TargetPosition, other.TargetPosition) && EqualityComparer<bool>.Default.Equals(this.EnforceRotation, other.EnforceRotation) && EqualityComparer<Quaternion>.Default.Equals(this.TargetRotation, other.TargetRotation);
  }

  [CompilerGenerated]
  protected TwoJointsGroup(TwoJointsGroup original)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.\u003CIsEnabled\u003Ek__BackingField = original.\u003CIsEnabled\u003Ek__BackingField;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.\u003CSkeletonId\u003Ek__BackingField = original.\u003CSkeletonId\u003Ek__BackingField;
    this.Mode = original.Mode;
    this.FirstBoneIndex = original.FirstBoneIndex;
    this.FirstTwistIndex = original.FirstTwistIndex;
    this.SecondBoneIndex = original.SecondBoneIndex;
    this.SecondTwistIndex = original.SecondTwistIndex;
    this.EndBoneIndex = original.EndBoneIndex;
    this.FirstBoneGain = original.FirstBoneGain;
    this.SecondBoneGain = original.SecondBoneGain;
    this.EndBoneGain = original.EndBoneGain;
    this.MaxHingeAngle = original.MaxHingeAngle;
    this.MinHingeAngle = original.MinHingeAngle;
    this.HingeAxis = original.HingeAxis;
    this.EnforcePosition = original.EnforcePosition;
    this.TargetPosition = original.TargetPosition;
    this.EnforceRotation = original.EnforceRotation;
    this.TargetRotation = original.TargetRotation;
  }
}
