// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.GizmoManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.IO;

using Ktisis.Data.Config;

namespace Ktisis.Interface.Overlay;

public class GizmoManager {
	private const string ImGuiVersion = "1.88";
	private readonly Configuration _cfg;
	private readonly IDalamudPluginInterface _dpi;
	private bool IsInit;

	public GizmoManager(IDalamudPluginInterface dpi, Configuration cfg) {
		this._dpi = dpi;
		this._cfg = cfg;
	}

	public unsafe void Initialize() {
		if (this.IsInit)
			return;
		var flag = false;
		try {
			string version = Dalamud.Bindings.ImGui.ImGui.GetVersion();
			if (version != "1.88")
				throw new Exception($"ImGui version mismatch! Expected {"1.88"}, got {version ?? "NULL"} instead.");
			// ISSUE: cast to a function pointer type
			__FnPtr <void*  (UIntPtr,  void* )> zero1 = (__FnPtr <void*  (UIntPtr,  void* )>) IntPtr.Zero;
			// ISSUE: cast to a function pointer type
			__FnPtr <void  (void* ,  void* )> zero2 = (__FnPtr <void  (void* ,  void* )>) IntPtr.Zero;
			var allocUD = (void*)null;
			Dalamud.Bindings.ImGui.ImGui.GetAllocatorFunctions(&zero1, &zero2, &allocUD);
			ImGuiContextPtr currentContext = Dalamud.Bindings.ImGui.ImGui.GetCurrentContext();
			Ktisis.ImGuizmo.Gizmo.Load(((FileSystemInfo)this._dpi.AssemblyLocation.Directory).FullName);
			Ktisis.ImGuizmo.Gizmo.Initialize((IntPtr)currentContext.Handle, (IntPtr)zero1, (IntPtr)zero2, (IntPtr)allocUD);
			flag = true;
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to initialize gizmo:\n{ex}", Array.Empty<object>());
		}
		Ktisis.Ktisis.Log.Verbose($"Completed gizmo init (success: {flag})", Array.Empty<object>());
		this.IsInit = flag;
	}

	public Gizmo Create(GizmoId id) {
		if (!this.IsInit)
			throw new Exception("Can't create gizmo as ImGuizmo is not initialized.");
		return new Gizmo(this._cfg.Gizmo, id);
	}
}
