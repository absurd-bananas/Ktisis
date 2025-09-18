// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Posing.Types.IViewFrame
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Pose2D;
using Ktisis.Scene.Entities.Skeleton;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Interface.Components.Posing.Types;

public interface IViewFrame
{
  void DrawView(
    PoseViewEntry entry,
    float width,
    float height,
    IDictionary<string, string>? templates = null);

  void DrawBones(EntityPose pose);
}
