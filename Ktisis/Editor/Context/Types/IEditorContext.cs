// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Context.Types.IEditorContext
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Core.Types;
using Ktisis.Data.Config;
using Ktisis.Editor.Actions;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Selection;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interface.Editor.Types;
using Ktisis.Localization;
using Ktisis.Scene.Types;
using System;

#nullable enable
namespace Ktisis.Editor.Context.Types;

public interface IEditorContext : IDisposable
{
  bool IsValid { get; }

  IPluginContext Plugin { get; }

  bool IsGPosing { get; }

  Configuration Config { get; }

  LocaleManager Locale { get; }

  IActionManager Actions { get; }

  IAnimationManager Animation { get; }

  ICharacterManager Characters { get; }

  ICameraManager Cameras { get; }

  IEditorInterface Interface { get; }

  IPosingManager Posing { get; }

  ISceneManager Scene { get; }

  ISelectManager Selection { get; }

  ITransformHandler Transform { get; }

  void Initialize();

  void Update();
}
