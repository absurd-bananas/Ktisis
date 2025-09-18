// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.IEnvModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

using Ktisis.Interop.Hooking;

namespace Ktisis.Scene.Modules;

public interface IEnvModule : IHookModule, IDisposable {
	EnvOverride Override { get; set; }

	float Time { get; set; }

	int Day { get; set; }

	byte Weather { get; set; }
}
