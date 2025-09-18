// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.IkNodeGroup`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Ik.Types;
using Ktisis.Scene.Types;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public class IkNodeGroup<T> : IkNodeGroupBase where T : IIkGroup
{
  public readonly T Group;

  public IkNodeGroup(ISceneManager scene, EntityPose pose, T group)
    : base(scene, pose, (IIkGroup) group)
  {
    this.Group = group;
  }
}
