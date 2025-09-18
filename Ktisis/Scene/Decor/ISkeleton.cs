// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Decor.ISkeleton
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;

#nullable disable
namespace Ktisis.Scene.Decor;

public interface ISkeleton
{
  unsafe Skeleton* GetSkeleton();

  unsafe hkaPose* GetPose(int index);
}
