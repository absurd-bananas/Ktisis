// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.IIkController
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Config.Bones;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Posing.Ik.Types;
using Ktisis.Scene.Decor;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Posing.Ik;

public interface IIkController
{
  void Setup(ISkeleton skeleton);

  int GroupCount { get; }

  IEnumerable<(string name, IIkGroup group)> GetGroups();

  bool TrySetupGroup(string name, CcdGroupParams param, out CcdGroup? group);

  bool TrySetupGroup(string name, TwoJointsGroupParams param, out TwoJointsGroup? group);

  void Solve(bool frozen = false);

  void Destroy();
}
