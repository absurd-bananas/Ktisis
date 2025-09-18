// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.GroupPoseModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility.Signatures;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Types;
using Ktisis.Structs.GPose;

#nullable enable
namespace Ktisis.Scene.Modules;

public class GroupPoseModule(IHookMediator hook, ISceneManager scene) : SceneModule(hook, scene)
{
  [Signature("E8 ?? ?? ?? ?? 0F B7 56 3C")]
  private GroupPoseModule.GetGPoseStateDelegate? _getGPoseState;

  public unsafe GPoseState* GetGPoseState()
  {
    return this._getGPoseState == null ? (GPoseState*) null : this._getGPoseState();
  }

  public bool IsPrimaryActor(ActorEntity actor)
  {
    bool flag;
    switch (actor.Actor.ObjectIndex)
    {
      case 200:
      case 201:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  private unsafe delegate GPoseState* GetGPoseStateDelegate();
}
