// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.IkNodeGroupBase
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Ik.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Types;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public abstract class IkNodeGroupBase : BoneNodeGroup, IIkNode
{
  public readonly IIkGroup Group;

  protected IkNodeGroupBase(ISceneManager scene, EntityPose pose, IIkGroup group)
    : base(scene, pose)
  {
    this.Group = group;
  }

  public bool IsEnabled => this.Group.IsEnabled;

  public virtual void Enable() => this.Group.IsEnabled = true;

  public virtual void Disable() => this.Group.IsEnabled = false;
}
