// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Game.GPoseService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Core.Attributes;
using Ktisis.Events;

namespace Ktisis.Services.Game;

[Singleton]
public class GPoseService : IDisposable {
	private readonly IClientState _clientState;
	private readonly IFramework _framework;
	private readonly Event<Action<GPoseService, bool>> _gposeEvent;
	private readonly ITargetManager _targets;
	private readonly Event<Action> _updateEvent;
	private bool _isActive;
	private bool _isSubscribed;

	public GPoseService(
		IClientState clientState,
		IFramework framework,
		ITargetManager targets,
		Event<Action> updateEvent,
		Event<Action<GPoseService, bool>> gposeEvent
	) {
		this._clientState = clientState;
		this._framework = framework;
		this._targets = targets;
		this._updateEvent = updateEvent;
		this._gposeEvent = gposeEvent;
	}

	public bool IsGPosing => this._clientState.IsGPosing;

	public IGameObject? GPoseTarget => this._targets.GPoseTarget;

	public void Dispose() {
		// ISSUE: method pointer
		this._framework.Update -= new IFramework.OnUpdateDelegate((object)this, __methodptr(OnFrameworkUpdate));
		this._isSubscribed = false;
	}

	public event Action Update {
		add => this._updateEvent.Add(value);
		remove => this._updateEvent.Remove(value);
	}

	public event GPoseStateHandler StateChanged {
		add => this._gposeEvent.Add(value.Invoke);
		remove => this._gposeEvent.Remove(value.Invoke);
	}

	public void Subscribe() {
		if (this._isSubscribed)
			return;
		// ISSUE: method pointer
		this._framework.Update += new IFramework.OnUpdateDelegate((object)this, __methodptr(OnFrameworkUpdate));
		this._isSubscribed = true;
	}

	public void Reset() => this._isActive = false;

	private void OnFrameworkUpdate(IFramework sender) {
		var isGposing = this.IsGPosing;
		if (this._isActive != isGposing) {
			this._isActive = isGposing;
			Ktisis.Ktisis.Log.Info($"GPose state changed: {isGposing}", Array.Empty<object>());
			this._gposeEvent.Invoke<GPoseService, bool>(this, isGposing);
		}
		if (!isGposing)
			return;
		this._updateEvent.Invoke();
	}
}
