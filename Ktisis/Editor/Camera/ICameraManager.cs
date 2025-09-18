// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.ICameraManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Ktisis.Editor.Camera.Types;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Camera;

public interface ICameraManager : IDisposable
{
  bool IsValid { get; }

  void Initialize();

  EditorCamera? Current { get; }

  IEnumerable<EditorCamera> GetCameras();

  void SetCurrent(EditorCamera camera);

  void SetNext();

  void SetPrevious();

  bool IsWorkCameraActive { get; }

  void SetWorkCameraMode(bool enabled);

  void ToggleWorkCameraMode();

  KtisisCamera Create(CameraFlags flags = CameraFlags.None);

  IGameObject? ResolveOrbitTarget(EditorCamera camera);
}
