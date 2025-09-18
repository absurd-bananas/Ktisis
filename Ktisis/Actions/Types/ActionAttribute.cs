// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Attributes.ActionAttribute
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Actions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ActionAttribute : Attribute {
	public readonly string Name;

	public ActionAttribute(string name) {
		this.Name = name;
	}
}
