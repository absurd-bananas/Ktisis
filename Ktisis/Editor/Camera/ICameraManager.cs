// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.ICameraManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Editor.Camera.Types;

namespace Ktisis.Editor.Camera;

public interface ICameraManager : IDisposable {
	bool IsValid { get; }

	EditorCamera? Current { get; }

	bool IsWorkCameraActive { get; }

	void Initialize();

	IEnumerable<EditorCamera> GetCameras();

	void SetCurrent(EditorCamera camera);

	void SetNext();

	void SetPrevious();

	void SetWorkCameraMode(bool enabled);

	void ToggleWorkCameraMode();

	KtisisCamera Create(CameraFlags flags = CameraFlags.None);

	IGameObject? ResolveOrbitTarget(EditorCamera camera);
}
