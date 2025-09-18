// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.CameraManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Editor.Camera.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Interop.Hooking;

namespace Ktisis.Editor.Camera;

public class CameraManager : ICameraManager, IDisposable {
	private readonly IEditorContext _context;
	private readonly HookScope _scope;
	private readonly List<EditorCamera> CameraList = new List<EditorCamera>();

	public CameraManager(IEditorContext context, HookScope scope) {
		this._context = context;
		this._scope = scope;
	}

	private CameraModule? Module { get; set; }

	private EditorCamera? Active { get; set; }

	private EditorCamera? Default { get; set; }

	private WorkCamera? WorkCamera { get; set; }

	public bool IsValid => this._context.IsValid;

	public void Initialize() {
		Ktisis.Ktisis.Log.Verbose("Initializing camera manager...", Array.Empty<object>());
		try {
			this.SetupCameras();
			this.Module = this._scope.Create<CameraModule>(this);
			if (!this.Module.Initialize())
				return;
			this.Module.Setup();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to initialize camera manager:\n{ex}", Array.Empty<object>());
		}
	}

	public bool IsWorkCameraActive { get; private set; }

	public EditorCamera? Current {
		get {
			if (this.IsWorkCameraActive) {
				var workCamera = this.WorkCamera;
				if (workCamera != null && workCamera.IsValid)
					return workCamera;
			}
			var active = this.Active;
			if (active != null && active.IsValid)
				return active;
			var editorCamera = this.Default;
			return editorCamera != null && editorCamera.IsValid ? editorCamera : null;
		}
	}

	public IEnumerable<EditorCamera> GetCameras() => this.CameraList;

	public void SetCurrent(EditorCamera camera) {
		if (!camera.IsValid)
			throw new Exception("Attempting to set invalid camera as current.");
		if (this.Active == camera)
			return;
		this.Active = camera;
		this.Module?.ChangeCamera(camera);
		if (camera == this.WorkCamera)
			return;
		this.IsWorkCameraActive = false;
	}

	public void SetNext() {
		if (this.Current == null || !this.CameraList.Contains(this.Current))
			return;
		this.SetCurrent(this.CameraList[(this.CameraList.IndexOf(this.Current) + 1) % this.CameraList.Count]);
	}

	public void SetPrevious() {
		if (this.Current == null || !this.CameraList.Contains(this.Current))
			return;
		var num = this.CameraList.IndexOf(this.Current);
		var index = (num > 0 ? num : this.CameraList.Count) - 1;
		if (index >= this.CameraList.Count)
			return;
		this.SetCurrent(this.CameraList[index]);
	}

	public void SetWorkCameraMode(bool enabled) {
		if (this.IsWorkCameraActive == enabled)
			return;
		if (enabled) {
			this.SetupWorkCamera();
			this.Module?.ChangeCamera(this.WorkCamera);
			this.IsWorkCameraActive = true;
		} else {
			this.IsWorkCameraActive = false;
			var active = this.Active;
			if (active == null || !active.IsValid)
				return;
			this.Module?.ChangeCamera(active);
		}
	}

	public void ToggleWorkCameraMode() => this.SetWorkCameraMode(!this.IsWorkCameraActive);

	public KtisisCamera Create(CameraFlags flags = CameraFlags.None) {
		var ktisisCamera = new KtisisCamera(this);
		ktisisCamera.Name = this.GetNextAvailableName();
		ktisisCamera.Flags = flags;
		var camera = ktisisCamera;
		if (camera.Address == IntPtr.Zero)
			throw new Exception("Failed to allocate camera.");
		if (!this.CopyOntoCamera(camera))
			throw new Exception("Failed to setup new camera.");
		this.CameraList.Add(camera);
		this.SetCurrent(camera);
		return camera;
	}

	public IGameObject? ResolveOrbitTarget(EditorCamera camera) => this.Module?.ResolveOrbitTarget(camera);

	public void Dispose() {
		try {
			this.Module?.Dispose();
			this.ResetState();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to dispose camera manager!\n{ex}", Array.Empty<object>());
		}
		GC.SuppressFinalize(this);
	}

	private unsafe void SetupCameras() {
		FFXIVClientStructs.FFXIV.Client.Game.Camera* activeCamera = ((CameraManager)(IntPtr)CameraManager.Instance()).GetActiveCamera();
		if ((IntPtr)activeCamera == IntPtr.Zero)
			return;
		var editorCamera1 = new EditorCamera(this);
		editorCamera1.Name = "Main Camera";
		editorCamera1.Address = (IntPtr)activeCamera;
		editorCamera1.Flags = CameraFlags.DefaultCamera;
		var editorCamera2 = editorCamera1;
		this.Default = editorCamera1;
		this.Active = editorCamera2;
		this.CameraList.Add(this.Default);
	}

	private void SetupWorkCamera() {
		if (this.WorkCamera == null) {
			var workCamera = new WorkCamera(this._context, this);
			workCamera.Name = "Work Camera";
			this.WorkCamera = workCamera;
		}
		if (!this.CopyOntoCamera(this.WorkCamera))
			throw new Exception("Failed to setup work camera.");
	}

	private unsafe bool CopyOntoCamera(EditorCamera camera) {
		var current = this.Current;
		if (current == null || !current.IsValid || current == camera)
			return false;
		camera.OrbitTarget = current.OrbitTarget;
		camera.FixedPosition = current.FixedPosition;
		camera.RelativeOffset = current.RelativeOffset;
		*camera.GameCamera = *current.GameCamera;
		if (camera is WorkCamera workCamera) {
			Vector3 pos = current.GetPosition().Value;
			Vector3 rot = current.Camera->CalcRotation();
			workCamera.SetInitialPosition(pos, rot);
		} else
			camera.Flags = current.Flags & ~CameraFlags.DefaultCamera;
		camera.OrthographicZoom = current.OrthographicZoom;
		return true;
	}

	private string GetNextAvailableName() {
		for (var index = this.CameraList.Count + 1; index <= 100; ++index) {
			var name = $"Camera #{index}";
			if (!this.CameraList.Any(camera => camera.Name == name))
				return name;
		}
		return "New Camera";
	}

	private void ResetState() {
		this.Default?.ResetState();
		this.Active = null;
		this.WorkCamera?.Dispose();
		this.WorkCamera = null;
		this.CameraList.ForEach(cam => {
			if (!(cam is KtisisCamera ktisisCamera2))
				return;
			ktisisCamera2.Dispose();
		});
		this.CameraList.Clear();
	}
}
