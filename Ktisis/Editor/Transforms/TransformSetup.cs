// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformSetup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Sections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Editor.Transforms;

public record TransformSetup()
{
  public bool MirrorRotation;
  public bool ParentBones;
  public bool RelativeBones;

  public void Configure(GizmoConfig cfg)
  {
    this.MirrorRotation = cfg.MirrorRotation;
    this.ParentBones = cfg.ParentBones;
    this.RelativeBones = cfg.RelativeBones;
  }

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("MirrorRotation = ");
    builder.Append(this.MirrorRotation.ToString());
    builder.Append(", ParentBones = ");
    builder.Append(this.ParentBones.ToString());
    builder.Append(", RelativeBones = ");
    builder.Append(this.RelativeBones.ToString());
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.MirrorRotation)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.ParentBones)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.RelativeBones);
  }

  [CompilerGenerated]
  public virtual bool Equals(TransformSetup? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<bool>.Default.Equals(this.MirrorRotation, other.MirrorRotation) && EqualityComparer<bool>.Default.Equals(this.ParentBones, other.ParentBones) && EqualityComparer<bool>.Default.Equals(this.RelativeBones, other.RelativeBones);
  }

  [CompilerGenerated]
  protected TransformSetup(TransformSetup original)
  {
    this.MirrorRotation = original.MirrorRotation;
    this.ParentBones = original.ParentBones;
    this.RelativeBones = original.RelativeBones;
  }
}
