// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.Lights.LightSpawner
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Ktisis.Interop.Hooking;
using Ktisis.Structs.Camera;
using Ktisis.Structs.Common;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Modules.Lights;

public class LightSpawner : HookModule {
	private readonly HashSet<IntPtr> _created = new HashSet<IntPtr>();
	private readonly IFramework _framework;
	[Signature("E8 ?? ?? ?? ?? 48 89 84 FB ?? ?? ?? ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B C8")]
	private SceneLightCtorDelegate _sceneLightCtor;
	[Signature("E8 ?? ?? ?? ?? 48 8B 94 FB ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ??")]
	private SceneLightInitializeDelegate _sceneLightInit;
	[Signature("F6 41 38 01")]
	private SceneLightSetupDelegate _sceneLightSpawn;

	public LightSpawner(IHookMediator hook, IFramework framework)
		: base(hook) {
		this._framework = framework;
	}

	public void TryInitialize() {
		try {
			this.Initialize();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to initialize light spawner:\n{ex}", Array.Empty<object>());
		}
	}

	public unsafe SceneLight* Create() {
		var self = (SceneLight*)((IMemorySpace)(IntPtr)IMemorySpace.GetDefaultSpace()).Malloc<SceneLight>(8UL);
		var sceneLightPtr = this._sceneLightCtor(self);
		var num1 = this._sceneLightInit(self) ? 1 : 0;
		var num2 = this._sceneLightSpawn(self);
		var active = GameCameraEx.GetActive();
		if ((IntPtr)active != IntPtr.Zero) {
			self->Transform.Position = Vector3.op_Implicit(active->Position);
			self->Transform.Rotation = Quaternion.op_Implicit(active->CalcPointDirection());
		}
		var num3 = (IntPtr)self + new IntPtr(56);
		*(long*)num3 = *(long*)num3 | 2L;
		RenderLight* renderLight = self->RenderLight;
		if ((IntPtr)renderLight != IntPtr.Zero) {
			renderLight->Flags = LightFlags.Reflection;
			renderLight->LightType = LightType.PointLight;
			renderLight->Transform = &self->Transform;
			renderLight->Color = new ColorHDR();
			renderLight->ShadowNear = 0.1f;
			renderLight->ShadowFar = 15f;
			renderLight->FalloffType = FalloffType.Quadratic;
			renderLight->AreaAngle = Vector2.Zero;
			renderLight->Falloff = 1.1f;
			renderLight->LightAngle = 45f;
			renderLight->FalloffAngle = 0.5f;
			renderLight->Range = 100f;
			renderLight->CharaShadowRange = 100f;
		}
		this._created.Add((IntPtr)self);
		return self;
	}

	public unsafe void Destroy(SceneLight* light) {
		this._created.Remove((IntPtr)light);
		this._framework.RunOnFrameworkThread((Action)(() => this.InvokeDtor(light)));
	}

	private unsafe void DestroyAll() {
		if (this._framework.IsFrameworkUnloading)
			return;
		this._framework.RunOnFrameworkThread((Action)(() => {
			foreach (SceneLight* light in this._created)
				this.InvokeDtor(light);
			this._created.Clear();
		}));
	}

	private unsafe void InvokeDtor(SceneLight* light) {
		GetVirtualFunc<CleanupRenderDelegate>(light, 1)(light);
		GetVirtualFunc<DestructorDelegate>(light, 0)(light, false);
	}

	private unsafe static T GetVirtualFunc<T>(SceneLight* light, int index) => Marshal.GetDelegateForFunctionPointer<T>(light->_vf[index]);

	public override void Dispose() {
		base.Dispose();
		Ktisis.Ktisis.Log.Verbose("Disposing light spawn manager...", Array.Empty<object>());
		this.DestroyAll();
		GC.SuppressFinalize(this);
	}

	private unsafe delegate SceneLight* SceneLightCtorDelegate(SceneLight* self);

	private unsafe delegate bool SceneLightInitializeDelegate(SceneLight* self);

	private unsafe delegate IntPtr SceneLightSetupDelegate(SceneLight* self);

	private unsafe delegate void CleanupRenderDelegate(SceneLight* light);

	private unsafe delegate void DestructorDelegate(SceneLight* light, bool a2);
}
