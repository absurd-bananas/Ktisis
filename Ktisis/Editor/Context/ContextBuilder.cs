// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Context.ContextBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ktisis.Core.Attributes;
using Ktisis.Core.Types;
using Ktisis.Data.Mcdf;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Actions.Input;
using Ktisis.Editor.Animation;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Characters;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.AutoSave;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interface.Editor;
using Ktisis.Interface.Editor.Types;
using Ktisis.Interop;
using Ktisis.Interop.Hooking;
using Ktisis.Scene;
using Ktisis.Scene.Factory;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using Ktisis.Services.Data;
using Ktisis.Services.Game;

#nullable enable
namespace Ktisis.Editor.Context;

[Singleton]
public class ContextBuilder
{
  private readonly IDalamudPluginInterface _dpi;
  private readonly GPoseService _gpose;
  private readonly InteropService _interop;
  private readonly IFramework _framework;
  private readonly IDataManager _data;
  private readonly IKeyState _keyState;
  private readonly NamingService _naming;
  private readonly FormatService _format;
  private readonly McdfManager _mcdf;

  public ContextBuilder(
    IDalamudPluginInterface dpi,
    GPoseService gpose,
    InteropService interop,
    IFramework framework,
    IDataManager data,
    IKeyState keyState,
    NamingService naming,
    FormatService format,
    McdfManager mcdf)
  {
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

  public IEditorContext Create(IPluginContext state)
  {
    EditorContext editorContext = new EditorContext(this._gpose, state);
    HookScope scope = this._interop.CreateScope();
    InputManager input = new InputManager((IEditorContext) editorContext, scope, this._keyState);
    ActionManager action = new ActionManager((IEditorContext) editorContext, (IInputManager) input);
    EntityFactory factory = new EntityFactory((IEditorContext) editorContext, (INameResolver) this._naming);
    SelectManager select = new SelectManager((IEditorContext) editorContext);
    AttachManager attach = new AttachManager();
    PoseAutoSave autoSave = new PoseAutoSave((IEditorContext) editorContext, this._framework, this._format);
    EditorState state1 = new EditorState((IEditorContext) editorContext, scope)
    {
      Actions = (IActionManager) action,
      Animation = (IAnimationManager) new AnimationManager((IEditorContext) editorContext, scope, this._data, this._framework),
      Cameras = (ICameraManager) new CameraManager((IEditorContext) editorContext, scope),
      Characters = (ICharacterManager) new CharacterManager((IEditorContext) editorContext, scope, this._framework, this._mcdf),
      Interface = (IEditorInterface) new EditorInterface(this._dpi, (IEditorContext) editorContext, state.Gui),
      Posing = (IPosingManager) new PosingManager((IEditorContext) editorContext, scope, this._framework, (IAttachManager) attach, autoSave),
      Scene = (ISceneManager) new SceneManager((IEditorContext) editorContext, scope, (IEntityFactory) factory, this._framework),
      Selection = (ISelectManager) select,
      Transform = (ITransformHandler) new TransformHandler((IEditorContext) editorContext, (IActionManager) action, (ISelectManager) select)
    };
    editorContext.Setup(state1);
    return (IEditorContext) editorContext;
  }
}
