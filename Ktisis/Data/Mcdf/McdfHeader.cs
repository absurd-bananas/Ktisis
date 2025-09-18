// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Mcdf.McdfHeader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ktisis.Data.Mcdf;

public record McdfHeader() {
public byte Version {
	get;
	set;
}
public required string FilePath {
	get;
	set;
}
public required McdfData Data {
get;
set;
}
[CompilerGenerated]
protected virtual bool PrintMembers(StringBuilder builder) {
	RuntimeHelpers.EnsureSufficientExecutionStack();
	builder.Append("Version = ");
	builder.Append(this.Version.ToString());
	builder.Append(", FilePath = ");
	builder.Append((object)this.FilePath);
	builder.Append(", Data = ");
	builder.Append((object)this.Data);
	return true;
}

[CompilerGenerated]
public override int GetHashCode() {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(this.\u003CVersion\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<string>.Default.GetHashCode(this.\u003CFilePath\u003Ek__BackingField)) * -1521134295 + EqualityComparer<McdfData>.Default.GetHashCode(this.\u003CData\u003Ek__BackingField);
}

[CompilerGenerated]
public virtual bool Equals(McdfHeader? other) {
	if ((object)this == (object)other)
		return true;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<byte>.Default.Equals(this.\u003CVersion\u003Ek__BackingField, other.\u003CVersion\u003Ek__BackingField) &&
		EqualityComparer<string>.Default.Equals(this.\u003CFilePath\u003Ek__BackingField, other.\u003CFilePath\u003Ek__BackingField) &&
		EqualityComparer<McdfData>.Default.Equals(this.\u003CData\u003Ek__BackingField, other.\u003CData\u003Ek__BackingField);
}

[CompilerGenerated]
[SetsRequiredMembers]
protected McdfHeader(McdfHeader original) {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CVersion\u003Ek__BackingField = original.\u003CVersion\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CFilePath\u003Ek__BackingField = original.\u003CFilePath\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CData\u003Ek__BackingField = original.\u003CData\u003Ek__BackingField;
}
}
