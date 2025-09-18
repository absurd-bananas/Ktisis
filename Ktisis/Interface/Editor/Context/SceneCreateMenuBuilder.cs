// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Context.SceneCreateMenuBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using GLib.Popups.Context;
using Ktisis.Common.Extensions;
using Ktisis.Data.Files;
using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Factory.Types;
using Ktisis.Structs.Lights;
using System;
using System.IO;

#nullable enable
namespace Ktisis.Interface.Editor.Context;

public class SceneCreateMenuBuilder
{
  private readonly IEditorContext _ctx;

  private IEntityFactory Factory => this._ctx.Scene.Factory;

  public SceneCreateMenuBuilder(IEditorContext ctx) => this._ctx = ctx;

  public ContextMenu Create()
  {
    return new ContextMenuBuilder().Group(new Action<ContextMenuBuilder>(this.BuildActorGroup)).Separator().Group(new Action<ContextMenuBuilder>(this.BuildLightGroup)).Separator().Group(new Action<ContextMenuBuilder>(this.BuildUtilityGroup)).Build($"##SceneCreateMenu_{this.GetHashCode():X}");
  }

  private void BuildActorGroup(ContextMenuBuilder sub)
  {
    sub.Action("Create new actor", (Action) (() => this.Factory.CreateActor().Spawn())).Action("Import actor from file", new Action(this.ImportCharaFromFile)).Action("Add overworld actor", new Action(this._ctx.Interface.OpenOverworldActorList));
  }

  private void BuildLightGroup(ContextMenuBuilder sub)
  {
    sub.SubMenu("Create new light", new Action<ContextMenuBuilder>(this.BuildLightMenu));
  }

  private void BuildLightMenu(ContextMenuBuilder sub)
  {
    sub.Action("Point", (Action) (() => SpawnLight(LightType.PointLight))).Action("Spot", (Action) (() => SpawnLight(LightType.SpotLight))).Action("Area", (Action) (() => SpawnLight(LightType.AreaLight))).Action("Sun", (Action) (() => SpawnLight(LightType.Directional)));

    void SpawnLight(LightType type) => this.Factory.CreateLight(type).Spawn();
  }

  private void BuildUtilityGroup(ContextMenuBuilder sub)
  {
    sub.Action("Add reference image", new Action(this.OpenReferenceImage));
  }

  private void ImportCharaFromFile()
  {
    this._ctx.Interface.OpenCharaFile((Action<string, CharaFile>) ((path, file) =>
    {
      string name = Path.GetFileNameWithoutExtension(path).Truncate(32 /*0x20*/);
      this.Factory.CreateActor().WithAppearance(file).SetName(name).Spawn();
    }));
  }

  private void OpenReferenceImage()
  {
    this._ctx.Interface.OpenReferenceImages((Action<string>) (path => this.Factory.BuildRefImage().SetPath(path).Add().Save()));
  }
}
