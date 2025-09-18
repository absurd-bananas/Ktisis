// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Pose2D.PoseViewSchema
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ktisis.Data.Config.Pose2D;

public record PoseViewSchema() {
public readonly Dictionary<string, PoseViewEntry> Views;

[CompilerGenerated]
protected virtual bool PrintMembers(StringBuilder builder) {
	RuntimeHelpers.EnsureSufficientExecutionStack();
	builder.Append("Views = ");
	builder.Append((object)this.Views);
	return true;
}

[CompilerGenerated]
public override int GetHashCode() {
	return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<Dictionary<string, PoseViewEntry>>.Default.GetHashCode(this.Views);
}

[CompilerGenerated]
public virtual bool Equals(PoseViewSchema? other) {
	if ((object)this == (object)other)
		return true;
	return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<Dictionary<string, PoseViewEntry>>.Default.Equals(this.Views, other.Views);
}

[CompilerGenerated]
protected PoseViewSchema(PoseViewSchema original) {
	return this.Views = original.Views;
}
}
