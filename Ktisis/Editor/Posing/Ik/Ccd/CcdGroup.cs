// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.Ccd.CcdGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Ik.Types;
using System.Numerics;

#nullable disable
namespace Ktisis.Editor.Posing.Ik.Ccd;

public class CcdGroup : IIkGroup
{
  public short StartBoneIndex = -1;
  public short EndBoneIndex = -1;
  public int Iterations = 8;
  public float Gain = 0.5f;
  public Vector3 TargetPosition;

  public bool IsEnabled { get; set; }

  public uint SkeletonId { get; set; }
}
