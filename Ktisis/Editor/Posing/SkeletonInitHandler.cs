// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.SkeletonInitHandler
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.Editor.Posing;

public unsafe delegate void SkeletonInitHandler(
	IGameObject owner,
	Skeleton* skeleton,
	ushort partialId
);
