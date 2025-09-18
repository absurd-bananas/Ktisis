// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.EditorState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Editor.Actions;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interface.Editor.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Types;

namespace Ktisis.Editor;

public class EditorState : IDisposable {
	private readonly IEditorContext _context;
	private readonly HookScope _scope;
	private bool IsDisposing;
	private bool IsInit;

	public EditorState(IEditorContext context, HookScope scope) {
		this._context = context;
		this._scope = scope;
	}

	public bool IsValid => this.IsInit && this._context.IsGPosing && !this.IsDisposing;

	public required IActionManager Actions { get; init; }

	public required IAnimationManager Animation { get; init; }

	public required ICameraManager Cameras { get; init; }

	public required ICharacterManager Characters { get; init; }

	public required IEditorInterface Interface { get; init; }

	public required IPosingManager Posing { get; init; }

	public required ISceneManager Scene { get; init; }

	public required ISelectManager Selection { get; init; }

	public required ITransformHandler Transform { get; init; }

	public void Dispose() {
		this.IsDisposing = true;
		this._scope.Dispose();
		this.Scene.Dispose();
		this.Posing.Dispose();
		this.Cameras.Dispose();
		GC.SuppressFinalize(this);
	}

	public void Initialize() {
		try {
			this.IsInit = true;
			this.Actions.Initialize();
			this.Animation.Initialize();
			this.Characters.Initialize();
			this.Cameras.Initialize();
			this.Posing.Initialize();
			this.Scene.Initialize();
		} catch {
			this.Dispose();
			throw;
		}
		try {
			this.Interface.Prepare();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Error preparing interface:\n{ex}", Array.Empty<object>());
		}
	}

	public void Update() {
		this.Scene.Update();
		this.Selection.Update();
	}
}
