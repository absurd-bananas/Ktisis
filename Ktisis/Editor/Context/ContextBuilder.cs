// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Context.ContextBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Core.Attributes;
using Ktisis.Core.Types;
using Ktisis.Data.Mcdf;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Actions.Input;
using Ktisis.Editor.Animation;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Characters;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.AutoSave;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms;
using Ktisis.Interface.Editor;
using Ktisis.Interop;
using Ktisis.Scene;
using Ktisis.Scene.Factory;
using Ktisis.Services.Data;
using Ktisis.Services.Game;

namespace Ktisis.Editor.Context;

[Singleton]
public class ContextBuilder {
	private readonly IDataManager _data;
	private readonly IDalamudPluginInterface _dpi;
	private readonly FormatService _format;
	private readonly IFramework _framework;
	private readonly GPoseService _gpose;
	private readonly InteropService _interop;
	private readonly IKeyState _keyState;
	private readonly McdfManager _mcdf;
	private readonly NamingService _naming;

	public ContextBuilder(
		IDalamudPluginInterface dpi,
		GPoseService gpose,
		InteropService interop,
		IFramework framework,
		IDataManager data,
		IKeyState keyState,
		NamingService naming,
		FormatService format,
		McdfManager mcdf
	) {
		this._dpi = dpi;
		this._gpose = gpose;
		this._interop = interop;
		this._framework = framework;
		this._data = data;
		this._keyState = keyState;
		this._naming = naming;
		this._format = format;
		this._mcdf = mcdf;
	}

	public IEditorContext Create(IPluginContext state) {
		var editorContext = new EditorContext(this._gpose, state);
		var scope = this._interop.CreateScope();
		var input = new InputManager(editorContext, scope, this._keyState);
		var action = new ActionManager(editorContext, input);
		var factory = new EntityFactory(editorContext, this._naming);
		var select = new SelectManager(editorContext);
		var attach = new AttachManager();
		var autoSave = new PoseAutoSave(editorContext, this._framework, this._format);
		var state1 = new EditorState(editorContext, scope) {
			Actions = action,
			Animation = new AnimationManager(editorContext, scope, this._data, this._framework),
			Cameras = new CameraManager(editorContext, scope),
			Characters = new CharacterManager(editorContext, scope, this._framework, this._mcdf),
			Interface = new EditorInterface(this._dpi, editorContext, state.Gui),
			Posing = new PosingManager(editorContext, scope, this._framework, attach, autoSave),
			Scene = new SceneManager(editorContext, scope, factory, this._framework),
			Selection = select,
			Transform = new TransformHandler(editorContext, action, select)
		};
		editorContext.Setup(state1);
		return editorContext;
	}
}
