// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.Lights.LightModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.Interop;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Types;
using Ktisis.Structs.GPose;
using Ktisis.Structs.Lights;
using System;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Scene.Modules.Lights;

public class LightModule : SceneModule
{
  private readonly GroupPoseModule _gpose;
  private readonly IFramework _framework;
  private readonly LightSpawner _spawner;
  [Signature("48 89 5C 24 ?? 57 48 83 EC 40 48 8B B9 ?? ?? ?? ??")]
  private LightModule.SceneLightUpdateCullingDelegate _sceneLightUpdateCulling;
  [Signature("40 53 48 83 EC 20 0F B6 81 ?? ?? ?? ?? 48 8B D9 A8 04 75 45 0C 04 B2 05")]
  private LightModule.SceneLightUpdateMaterialsDelegate _sceneLightUpdateMaterials;
  [Signature("48 83 EC 28 4C 8B C1 83 FA 03", DetourName = "ToggleLightDetour")]
  private Hook<LightModule.ToggleLightDelegate>? ToggleLightHook;

  public LightModule(
    IHookMediator hook,
    ISceneManager scene,
    GroupPoseModule gpose,
    IFramework framework)
    : base(hook, scene)
  {
    this._gpose = gpose;
    this._framework = framework;
    this._spawner = hook.Create<LightSpawner>();
  }

  public override void Setup()
  {
    this.EnableAll();
    this.BuildLightEntities();
    this._spawner.TryInitialize();
  }

  private unsafe void BuildLightEntities()
  {
    GPoseState* gposeState = this._gpose.GetGPoseState();
    if ((IntPtr) gposeState == IntPtr.Zero)
      return;
    Span<Pointer<SceneLight>> lights = gposeState->GetLights();
    for (int index = 0; index < lights.Length; ++index)
    {
      if ((IntPtr) lights[index].Value != IntPtr.Zero)
        this.AddLight(lights[index].Value, (uint) index);
    }
  }

  private unsafe void AddLight(SceneLight* light, uint index)
  {
    this.Scene.Factory.BuildLight().SetName($"Camera Light {index + 1U}").SetAddress(light).Add();
  }

  private unsafe void RemoveLight(SceneLight* light)
  {
    this.Scene.Children.FirstOrDefault<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is LightEntity lightEntity && lightEntity.Address == (IntPtr) light))?.Remove();
  }

  public unsafe void UpdateLightObject(LightEntity entity)
  {
    if (!this.IsInit || !entity.IsValid)
      return;
    SceneLight* self = entity.GetObject();
    if ((IntPtr) self != IntPtr.Zero)
    {
      this._sceneLightUpdateCulling(self);
      this._sceneLightUpdateMaterials(self);
    }
    entity.Flags &= ~LightEntityFlags.Update;
  }

  private unsafe bool ToggleLightDetour(GPoseState* state, uint index)
  {
    bool flag = false;
    try
    {
      int num = this.CheckValid() ? 1 : 0;
      SceneLight* light1 = num != 0 ? state->GetLight(index) : (SceneLight*) null;
      flag = this.ToggleLightHook.Original(state, index);
      if ((num & (flag ? 1 : 0)) != 0)
      {
        SceneLight* light2 = state->GetLight(index);
        if ((IntPtr) light2 != IntPtr.Zero && light2 != light1)
          this.AddLight(light2, index);
        else if ((IntPtr) light2 == IntPtr.Zero)
        {
          if ((IntPtr) light1 != IntPtr.Zero)
            this.RemoveLight(light1);
        }
      }
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle light toggle:\n{ex}", Array.Empty<object>());
    }
    return flag;
  }

  public async Task<LightEntity> Spawn()
  {
    LightModule lightModule = this;
    // ISSUE: reference to a compiler-generated method
    return await lightModule._framework.RunOnFrameworkThread<LightEntity>(new Func<LightEntity>(lightModule.\u003CSpawn\u003Eb__16_0));
  }

  private unsafe LightEntity? CreateLight()
  {
    SceneLight* pointer = this._spawner.Create();
    return (IntPtr) pointer == IntPtr.Zero ? (LightEntity) null : this.Scene.Factory.BuildLight().SetName("Light").SetAddress(pointer).Add();
  }

  public unsafe void Delete(LightEntity light)
  {
    SceneLight* address = (SceneLight*) light.Address;
    light.Address = IntPtr.Zero;
    light.Remove();
    if ((IntPtr) address == IntPtr.Zero)
      return;
    this._spawner.Destroy(address);
  }

  public override void Dispose()
  {
    base.Dispose();
    this._spawner.Dispose();
    GC.SuppressFinalize((object) this);
  }

  private unsafe delegate void SceneLightUpdateCullingDelegate(SceneLight* self);

  private unsafe delegate void SceneLightUpdateMaterialsDelegate(SceneLight* self);

  private unsafe delegate bool ToggleLightDelegate(GPoseState* state, uint index);
}
