// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Mcdf.McdfData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ktisis.Data.Mcdf;

public record McdfData() {
public string Description {
	get;
	set;
}
public string GlamourerData {
	get;
	set;
}
public string CustomizePlusData {
	get;
	set;
}
public string ManipulationData {
	get;
	set;
}
public List < McdfData.FileData > Files {
	get;
	set;
}
public List < McdfData.FileSwap > FileSwaps {
	get;
	set;
}

[CompilerGenerated]
protected virtual bool PrintMembers(StringBuilder builder) {
	RuntimeHelpers.EnsureSufficientExecutionStack();
	builder.Append("Description = ");
	builder.Append((object)this.Description);
	builder.Append(", GlamourerData = ");
	builder.Append((object)this.GlamourerData);
	builder.Append(", CustomizePlusData = ");
	builder.Append((object)this.CustomizePlusData);
	builder.Append(", ManipulationData = ");
	builder.Append((object)this.ManipulationData);
	builder.Append(", Files = ");
	builder.Append((object)this.Files);
	builder.Append(", FileSwaps = ");
	builder.Append((object)this.FileSwaps);
	return true;
}

[CompilerGenerated]
public override int GetHashCode() {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.\u003CDescription\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<string>.Default.GetHashCode(this.\u003CGlamourerData\u003Ek__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.\u003CCustomizePlusData\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<string>.Default.GetHashCode(this.\u003CManipulationData\u003Ek__BackingField)) * -1521134295 + EqualityComparer<List<McdfData.FileData>>.Default.GetHashCode(this.\u003CFiles\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<List<McdfData.FileSwap>>.Default.GetHashCode(this.\u003CFileSwaps\u003Ek__BackingField);
}

[CompilerGenerated]
public virtual bool Equals(McdfData? other) {
	if ((object)this == (object)other)
		return true;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.\u003CDescription\u003Ek__BackingField, other.\u003CDescription\u003Ek__BackingField) &&
		EqualityComparer<string>.Default.Equals(this.\u003CGlamourerData\u003Ek__BackingField, other.\u003CGlamourerData\u003Ek__BackingField) &&
		EqualityComparer<string>.Default.Equals(this.\u003CCustomizePlusData\u003Ek__BackingField, other.\u003CCustomizePlusData\u003Ek__BackingField) &&
		EqualityComparer<string>.Default.Equals(this.\u003CManipulationData\u003Ek__BackingField, other.\u003CManipulationData\u003Ek__BackingField) &&
		EqualityComparer<List<McdfData.FileData>>.Default.Equals(this.\u003CFiles\u003Ek__BackingField, other.\u003CFiles\u003Ek__BackingField) &&
		EqualityComparer<List<McdfData.FileSwap>>.Default.Equals(this.\u003CFileSwaps\u003Ek__BackingField, other.\u003CFileSwaps\u003Ek__BackingField);
}

[CompilerGenerated]
protected McdfData(McdfData original) {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CDescription\u003Ek__BackingField = original.\u003CDescription\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CGlamourerData\u003Ek__BackingField = original.\u003CGlamourerData\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CCustomizePlusData\u003Ek__BackingField = original.\u003CCustomizePlusData\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CManipulationData\u003Ek__BackingField = original.\u003CManipulationData\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CFiles\u003Ek__BackingField = original.\u003CFiles\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CFileSwaps\u003Ek__BackingField = original.\u003CFileSwaps\u003Ek__BackingField;
}

public record FileData() {
public string[] GamePaths {
	get;
	set;
}
public int Length {
	get;
	set;
}
public string Hash {
	get;
	set;
}

[CompilerGenerated]
protected virtual bool PrintMembers(StringBuilder builder) {
	RuntimeHelpers.EnsureSufficientExecutionStack();
	builder.Append("GamePaths = ");
	builder.Append((object)this.GamePaths);
	builder.Append(", Length = ");
	builder.Append(this.Length.ToString());
	builder.Append(", Hash = ");
	builder.Append((object)this.Hash);
	return true;
}

[CompilerGenerated]
public override int GetHashCode() {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(this.\u003CGamePaths\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<int>.Default.GetHashCode(this.\u003CLength\u003Ek__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.\u003CHash\u003Ek__BackingField);
}

[CompilerGenerated]
public virtual bool Equals(McdfData.FileData? other) {
	if ((object)this == (object)other)
		return true;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string[]>.Default.Equals(this.\u003CGamePaths\u003Ek__BackingField, other.\u003CGamePaths\u003Ek__BackingField) &&
		EqualityComparer<int>.Default.Equals(this.\u003CLength\u003Ek__BackingField, other.\u003CLength\u003Ek__BackingField) && EqualityComparer<string>.Default.Equals(this.\u003CHash\u003Ek__BackingField, other.\u003CHash\u003Ek__BackingField);
}

[CompilerGenerated]
protected FileData(McdfData.FileData original) {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CGamePaths\u003Ek__BackingField = original.\u003CGamePaths\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CLength\u003Ek__BackingField = original.\u003CLength\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CHash\u003Ek__BackingField = original.\u003CHash\u003Ek__BackingField;
}
}
public record FileSwap() {
public string[] GamePaths {
	get;
	set;
}
public string FileSwapPath {
	get;
	set;
}

[CompilerGenerated]
protected virtual bool PrintMembers(StringBuilder builder) {
	RuntimeHelpers.EnsureSufficientExecutionStack();
	builder.Append("GamePaths = ");
	builder.Append((object)this.GamePaths);
	builder.Append(", FileSwapPath = ");
	builder.Append((object)this.FileSwapPath);
	return true;
}

[CompilerGenerated]
public override int GetHashCode() {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(this.\u003CGamePaths\u003Ek__BackingField)) * -1521134295 +
		EqualityComparer<string>.Default.GetHashCode(this.\u003CFileSwapPath\u003Ek__BackingField);
}

[CompilerGenerated]
public virtual bool Equals(McdfData.FileSwap? other) {
	if ((object)this == (object)other)
		return true;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	return (object)other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string[]>.Default.Equals(this.\u003CGamePaths\u003Ek__BackingField, other.\u003CGamePaths\u003Ek__BackingField) &&
		EqualityComparer<string>.Default.Equals(this.\u003CFileSwapPath\u003Ek__BackingField, other.\u003CFileSwapPath\u003Ek__BackingField);
}

[CompilerGenerated]
protected FileSwap(McdfData.FileSwap original) {
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CGamePaths\u003Ek__BackingField = original.\u003CGamePaths\u003Ek__BackingField;
	// ISSUE: reference to a compiler-generated field
	// ISSUE: reference to a compiler-generated field
	this.\u003CFileSwapPath\u003Ek__BackingField = original.\u003CFileSwapPath\u003Ek__BackingField;
}
}
}
