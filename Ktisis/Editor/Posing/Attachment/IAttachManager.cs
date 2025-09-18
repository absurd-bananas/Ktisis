// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Attachment.IAttachManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using Ktisis.Scene.Decor;
using System;

#nullable enable
namespace Ktisis.Editor.Posing.Attachment;

public interface IAttachManager : IDisposable
{
  void Attach(IAttachable child, IAttachTarget target);

  void Detach(IAttachable child);

  unsafe void Invalidate(Skeleton* parent);
}
