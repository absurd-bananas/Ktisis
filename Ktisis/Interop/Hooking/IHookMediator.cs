// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.IHookMediator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.Interop.Hooking;

public interface IHookMediator {
	bool IsValid { get; }

	T Create<T>(params object[] param) where T : HookModule;

	bool Init(HookModule module);

	bool Remove(HookModule module);
}
