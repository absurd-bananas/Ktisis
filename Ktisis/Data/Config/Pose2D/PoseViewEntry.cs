// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Pose2D.PoseViewEntry
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Data.Config.Pose2D;

public record PoseViewEntry()
{
  public string Name;
  public readonly List<string> Images;
  public readonly List<PoseViewBone> Bones;

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Name = ");
    builder.Append((object) this.Name);
    builder.Append(", Images = ");
    builder.Append((object) this.Images);
    builder.Append(", Bones = ");
    builder.Append((object) this.Bones);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(this.Images)) * -1521134295 + EqualityComparer<List<PoseViewBone>>.Default.GetHashCode(this.Bones);
  }

  [CompilerGenerated]
  public virtual bool Equals(PoseViewEntry? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<List<string>>.Default.Equals(this.Images, other.Images) && EqualityComparer<List<PoseViewBone>>.Default.Equals(this.Bones, other.Bones);
  }

  [CompilerGenerated]
  protected PoseViewEntry(PoseViewEntry original)
  {
    this.Name = original.Name;
    this.Images = original.Images;
    this.Bones = original.Bones;
  }
}
