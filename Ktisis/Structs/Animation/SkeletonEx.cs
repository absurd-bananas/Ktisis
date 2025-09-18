// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.SkeletonEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using Ktisis.Structs.Attachment;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 256 /*0x0100*/)]
public struct SkeletonEx
{
  [FieldOffset(0)]
  public Skeleton Skeleton;
  [FieldOffset(136)]
  public unsafe Ktisis.Structs.Attachment.ElementParam* ElementParam;
  [FieldOffset(144 /*0x90*/)]
  public unsafe Matrix4x4* ElementMatrix;
  [FieldOffset(152)]
  public unsafe ushort* ElementBoneMap;
  [FieldOffset(160 /*0xA0*/)]
  public uint ElementCount;

  public bool TryGetBoneIndexForElementId(uint id, out ushort index)
  {
    return this.TryGetBoneIndexForElementId((ElementId) id, out index);
  }

  public unsafe bool TryGetBoneIndexForElementId(ElementId id, out ushort index)
  {
    index = ushort.MaxValue;
    for (int index1 = 0; (long) index1 < (long) this.ElementCount; ++index1)
    {
      if (this.ElementParam[index1].ElementId == id)
      {
        index = this.ElementBoneMap[index1];
        return true;
      }
    }
    return false;
  }
}
