// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Pose2D.PoseViewBone
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Ktisis.Data.Config.Pose2D;

public record PoseViewBone()
{
  public string Label;
  public string Name;
  public Vector2 Position;

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Label = ");
    builder.Append((object) this.Label);
    builder.Append(", Name = ");
    builder.Append((object) this.Name);
    builder.Append(", Position = ");
    builder.Append(this.Position.ToString());
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Label)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Position);
  }

  [CompilerGenerated]
  public virtual bool Equals(PoseViewBone? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.Label, other.Label) && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<Vector2>.Default.Equals(this.Position, other.Position);
  }

  [CompilerGenerated]
  protected PoseViewBone(PoseViewBone original)
  {
    this.Label = original.Label;
    this.Name = original.Name;
    this.Position = original.Position;
  }
}
