// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.World.LightEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Common.Utility;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Modules.Lights;
using Ktisis.Scene.Types;
using Ktisis.Structs.Lights;
using System;

#nullable enable
namespace Ktisis.Scene.Entities.World;

public class LightEntity : WorldEntity, IDeletable
{
  public LightEntityFlags Flags { get; set; }

  public unsafe SceneLight* GetObject() => (SceneLight*) base.GetObject();

  public LightEntity(ISceneManager scene)
    : base(scene)
  {
    this.Type = EntityType.Light;
  }

  private LightModule GetModule() => this.Scene.GetModule<LightModule>();

  public unsafe void SetType(LightType type)
  {
    SceneLight* sceneLightPtr = this.GetObject();
    if ((IntPtr) sceneLightPtr == IntPtr.Zero || (IntPtr) sceneLightPtr->RenderLight == IntPtr.Zero)
      return;
    sceneLightPtr->RenderLight->LightType = type;
  }

  public override void Update()
  {
    if (!this.IsValid)
      return;
    if (this.Flags.HasFlag((Enum) LightEntityFlags.Update))
      this.GetModule().UpdateLightObject(this);
    base.Update();
  }

  public override void SetTransform(Transform trans)
  {
    base.SetTransform(trans);
    this.Flags |= LightEntityFlags.Update;
  }

  public bool Delete()
  {
    this.GetModule().Delete(this);
    return this.Address == IntPtr.Zero;
  }

  public unsafe bool IsObjectVisible => ((DrawObject) ref this.GetObject()->DrawObject).IsVisible;

  public unsafe void ToggleObjectVisibility()
  {
    ((DrawObject) ref this.GetObject()->DrawObject).IsVisible = !this.IsObjectVisible;
  }
}
