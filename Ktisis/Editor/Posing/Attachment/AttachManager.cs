// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Attachment.AttachManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using Ktisis.Scene.Decor;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Posing.Attachment;

public class AttachManager : IAttachManager, IDisposable
{
  private readonly HashSet<IAttachable> Attachments = new HashSet<IAttachable>();

  public void Attach(IAttachable child, IAttachTarget target)
  {
    Ktisis.Ktisis.Log.Info($"Attaching {child} {child.GetHashCode():X}", Array.Empty<object>());
    if (!child.IsValid || !target.TryAcceptAttach(child))
      return;
    this.Attachments.Add(child);
  }

  public void Detach(IAttachable child)
  {
    if (!child.IsValid)
      return;
    Ktisis.Ktisis.Log.Info($"Detaching {child} {child.GetHashCode():X}", Array.Empty<object>());
    try
    {
      child.Detach();
    }
    finally
    {
      this.Attachments.RemoveWhere((Predicate<IAttachable>) (item => item.Equals((object) child)));
    }
  }

  public unsafe void Invalidate(Skeleton* parent)
  {
    foreach (IAttachable child in this.Attachments.Where<IAttachable>((Func<IAttachable, bool>) (x => x.IsValid)).ToList<IAttachable>())
    {
      Ktisis.Structs.Attachment.Attach* attach = child.GetAttach();
      if ((IntPtr) attach != IntPtr.Zero && attach->Parent == parent)
        this.Detach(child);
    }
  }

  private void Clear()
  {
    foreach (IAttachable attachment in this.Attachments)
      attachment.Detach();
    this.Attachments.Clear();
  }

  public void Dispose()
  {
    try
    {
      this.Clear();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to clear attachments:\n{ex}", Array.Empty<object>());
    }
    GC.SuppressFinalize((object) this);
  }
}
