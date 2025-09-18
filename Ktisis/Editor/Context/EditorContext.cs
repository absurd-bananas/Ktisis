// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Context.EditorContext
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Core.Types;
using Ktisis.Data.Config;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interface;
using Ktisis.Interface.Editor.Types;
using Ktisis.Localization;
using Ktisis.Scene.Types;
using Ktisis.Services.Game;
using System;

#nullable enable
namespace Ktisis.Editor.Context;

public class EditorContext : IEditorContext, IDisposable
{
  private readonly GPoseService _gpose;
  private EditorState? _state;

  public bool IsValid
  {
    get
    {
      EditorState state = this._state;
      return state != null && state.IsValid;
    }
  }

  public EditorContext(GPoseService gpose, IPluginContext plugin)
  {
    this._gpose = gpose;
    this.Plugin = plugin;
  }

  public bool IsGPosing => this._gpose.IsGPosing;

  public IPluginContext Plugin { get; }

  private EditorState State
  {
    get
    {
      return this._state != null ? this._state : throw new Exception("Attempting to access invalid context.");
    }
  }

  public void Setup(EditorState state)
  {
    this._state = this._state == null ? state : throw new Exception("Attempted double initialization of editor context!");
  }

  public Configuration Config => this.Plugin.Config.File;

  public GuiManager Gui => this.Plugin.Gui;

  public LocaleManager Locale => this.Gui.Locale;

  public IActionManager Actions => this.State.Actions;

  public IAnimationManager Animation => this.State.Animation;

  public ICharacterManager Characters => this.State.Characters;

  public ICameraManager Cameras => this.State.Cameras;

  public IEditorInterface Interface => this.State.Interface;

  public IPosingManager Posing => this.State.Posing;

  public ISceneManager Scene => this.State.Scene;

  public ISelectManager Selection => this.State.Selection;

  public ITransformHandler Transform => this.State.Transform;

  public void Initialize() => this._state?.Initialize();

  public void Update() => this._state?.Update();

  public void Dispose()
  {
    try
    {
      this._state?.Dispose();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to destroy editor context:\n{ex}", Array.Empty<object>());
    }
    finally
    {
      this._state = (EditorState) null;
    }
    GC.SuppressFinalize((object) this);
  }
}
