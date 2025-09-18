// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Types.KtisisWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Events;

namespace Ktisis.Interface.Types;

public abstract class KtisisWindow : Window {

	public delegate void ClosedDelegate(KtisisWindow window);
	private readonly Event<Action<KtisisWindow>> _closedEvent = new Event<Action<KtisisWindow>>();

	protected KtisisWindow(string name, ImGuiWindowFlags flags = 0, bool forceMainWindow = false)
		: base(name, flags, forceMainWindow) {
		this.RespectCloseHotkey = false;
	}

	public event ClosedDelegate Closed {
		add => this._closedEvent.Add(value.Invoke);
		remove => this._closedEvent.Remove(value.Invoke);
	}

	public void Open() => this.IsOpen = true;

	public void Close() {
		try {
			if (this.IsOpen)
				return;
			base.OnClose();
		} finally {
			this.IsOpen = false;
		}
	}

	public virtual void OnCreate() { }

	public virtual void OnClose() => this._closedEvent.Invoke(this);
}
