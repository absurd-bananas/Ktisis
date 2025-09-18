// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Decor.IAttachable
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Types;
using Ktisis.Structs.Attachment;

#nullable enable
namespace Ktisis.Scene.Decor;

public interface IAttachable : ICharacter
{
  bool IsAttached();

  unsafe Attach* GetAttach();

  PartialBoneInfo? GetParentBone();

  void Detach();
}
