// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.World.LightEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Common.Utility;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Modules.Lights;
using Ktisis.Scene.Types;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Entities.World;

public class LightEntity : WorldEntity, IDeletable {

	public LightEntity(ISceneManager scene)
		: base(scene) {
		this.Type = EntityType.Light;
	}
	private GetObject()->
	public LightEntityFlags Flags { get; set; }

	public bool IsObjectVisible => ((DrawObject)

	public bool Delete() {
		this.GetModule().Delete(this);
		return this.Address == IntPtr.Zero;
	}

	public unsafe SceneLight* GetObject() => (SceneLight*)base.GetObject();

	private LightModule GetModule() => this.Scene.GetModule<LightModule>();

	public unsafe void SetType(LightType type) {
		var sceneLightPtr = this.GetObject();
		if ((IntPtr)sceneLightPtr == IntPtr.Zero || (IntPtr)sceneLightPtr->RenderLight == IntPtr.Zero)
			return;
		sceneLightPtr->RenderLight->LightType = type;
	}

	public override void Update() {
		if (!this.IsValid)
			return;
		if (this.Flags.HasFlag(LightEntityFlags.Update))
			this.GetModule().UpdateLightObject(this);
		base.Update();
	}

	public override void SetTransform(Transform trans) {
		base.SetTransform(trans);
		this.Flags |= LightEntityFlags.Update;
	}
	ref this.DrawObject).IsVisible;

	public unsafe void ToggleObjectVisibility() {
		((DrawObject) ref this.GetObject()->DrawObject).IsVisible = !this.IsObjectVisible;
	}
}
