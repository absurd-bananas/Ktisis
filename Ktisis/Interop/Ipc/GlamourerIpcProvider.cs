// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.GlamourerIpcProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Glamourer.Api.IpcSubscribers;

namespace Ktisis.Interop.Ipc;

public class GlamourerIpcProvider {
	private readonly ApplyState _applyState;

	public GlamourerIpcProvider(IDalamudPluginInterface dpi) {
		this._applyState = new ApplyState(dpi);
	}

	public void ApplyState(string state, int index) {
		var num = (int)this._applyState.Invoke(state, index);
	}
}
