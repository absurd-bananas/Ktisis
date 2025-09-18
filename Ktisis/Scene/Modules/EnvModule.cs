// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.EnvModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Timer;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Types;
using Ktisis.Structs.Env;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Scene.Modules;

public class EnvModule(IHookMediator hook, ISceneManager scene) : 
  SceneModule(hook, scene),
  IEnvModule,
  IHookModule,
  IDisposable
{
  [Signature("E8 ?? ?? ?? ?? 49 3B F5 75 0D")]
  private EnvModule.EnvStateCopyDelegate EnvStateCopy;
  [Signature("E8 ?? ?? ?? ?? 49 3B F5 75 0D", DetourName = "EnvStateCopyDetour")]
  private Hook<EnvModule.EnvStateCopyDelegate> EnvStateCopyHook;
  [Signature("40 53 48 83 EC 30 48 8B 05 ?? ?? ?? ?? 48 8B D9 0F 29 74 24 ??", DetourName = "EnvUpdateDetour")]
  private Hook<EnvModule.EnvManagerUpdateDelegate> EnvUpdateHook;
  [Signature("48 89 5C 24 ?? 57 48 83 EC 30 4C 8B 15 ?? ?? ?? ??", DetourName = "UpdateTimeDetour")]
  private Hook<EnvModule.UpdateTimeDelegate> UpdateTimeHook;

  protected override bool OnInitialize()
  {
    this.EnableAll();
    return true;
  }

  public EnvOverride Override { get; set; }

  public float Time { get; set; }

  public int Day { get; set; }

  public byte Weather { get; set; }

  private unsafe void ApplyState(EnvState* dest, EnvState state)
  {
    foreach (EnvOverride envOverride in ((IEnumerable<EnvOverride>) Enum.GetValues<EnvOverride>()).Where<EnvOverride>((Func<EnvOverride, bool>) (flag => flag > EnvOverride.TimeWeather && this.Override.HasFlag((Enum) flag))))
    {
      switch (envOverride)
      {
        case EnvOverride.SkyId:
          dest->SkyId = state.SkyId;
          continue;
        case EnvOverride.Lighting:
          dest->Lighting = state.Lighting;
          continue;
        case EnvOverride.Stars:
          dest->Stars = state.Stars;
          continue;
        case EnvOverride.Fog:
          dest->Fog = state.Fog;
          continue;
        case EnvOverride.Clouds:
          dest->Clouds = state.Clouds;
          continue;
        case EnvOverride.Rain:
          dest->Rain = state.Rain;
          continue;
        case EnvOverride.Dust:
          dest->Dust = state.Dust;
          continue;
        case EnvOverride.Wind:
          dest->Wind = state.Wind;
          continue;
        default:
          continue;
      }
    }
  }

  private unsafe IntPtr EnvStateCopyDetour(EnvState* dest, EnvState* src)
  {
    EnvState? nullable = new EnvState?();
    if (this.Scene.IsValid && this.Override != EnvOverride.None)
      nullable = new EnvState?(*dest);
    IntPtr num = this.EnvStateCopyHook.Original(dest, src);
    if (!nullable.HasValue)
      return num;
    this.ApplyState(dest, nullable.Value);
    return num;
  }

  private unsafe IntPtr EnvUpdateDetour(EnvManagerEx* env, float a2, float a3)
  {
    if (this.Scene.IsValid && this.Override.HasFlag((Enum) EnvOverride.TimeWeather))
    {
      env->_base.DayTimeSeconds = this.Time;
      env->_base.ActiveWeather = this.Weather;
    }
    return this.EnvUpdateHook.Original(env, a2, a3);
  }

  private unsafe void UpdateTimeDetour(IntPtr a1)
  {
    if (this.Scene.IsValid && this.Override.HasFlag((Enum) EnvOverride.TimeWeather))
    {
      long num = (long) ((double) (this.Day * 86400) + (double) this.Time);
      ClientTime* clientTimePtr = &FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->ClientTime;
      clientTimePtr->EorzeaTime = num;
      clientTimePtr->EorzeaTimeOverride = num;
    }
    this.UpdateTimeHook.Original(a1);
  }

  private unsafe delegate IntPtr EnvStateCopyDelegate(EnvState* dest, EnvState* src);

  private unsafe delegate IntPtr EnvManagerUpdateDelegate(EnvManagerEx* env, float a2, float a3);

  private delegate void UpdateTimeDelegate(IntPtr a1);
}
