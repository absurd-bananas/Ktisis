// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.GameObjectEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Ktisis.Common.Extensions;

public static class GameObjectEx
{
  public static string GetNameOrFallback(this IGameObject gameObject)
  {
    string textValue = gameObject.Name.TextValue;
    if (!StringExtensions.IsNullOrEmpty(textValue))
      return textValue;
    return $"Actor #{gameObject.ObjectIndex}";
  }

  public static unsafe DrawObject* GetDrawObject(this IGameObject gameObject)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    return (IntPtr) address == IntPtr.Zero ? (DrawObject*) null : address->DrawObject;
  }

  public static unsafe Skeleton* GetSkeleton(this IGameObject gameObject)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    if ((IntPtr) address == IntPtr.Zero || (IntPtr) address->DrawObject == IntPtr.Zero)
      return (Skeleton*) null;
    DrawObject* drawObject = address->DrawObject;
    return ((FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object) ref drawObject->Object).GetObjectType() != 3 ? (Skeleton*) null : ((CharacterBase*) drawObject)->Skeleton;
  }

  public static unsafe bool IsDrawing(this IGameObject gameObject)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    return (IntPtr) address != IntPtr.Zero && address->RenderFlags == 0;
  }

  public static unsafe bool IsEnabled(this IGameObject gameObject)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    return (IntPtr) address != IntPtr.Zero && (address->RenderFlags & 2) == 0;
  }

  public static unsafe void SetWorld(this IGameObject gameObject, ushort world)
  {
    FFXIVClientStructs.FFXIV.Client.Game.Character.Character* address = (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*) gameObject.Address;
    if ((IntPtr) address == IntPtr.Zero || !((GameObject) ref address->GameObject).IsCharacter())
      return;
    address->CurrentWorld = world;
    address->HomeWorld = world;
  }

  public static unsafe void SetName(this IGameObject gameObject, string name)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    if ((IntPtr) address == IntPtr.Zero)
      return;
    byte[] array = ((IEnumerable<byte>) Encoding.UTF8.GetBytes(name)).Append<byte>((byte) 0).ToArray<byte>();
    for (int index = 0; index < array.Length; ++index)
      ((GameObject) (IntPtr) address).Name[index] = array[index];
  }

  public static unsafe void SetTargetable(this IGameObject gameObject, bool targetable)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    if ((IntPtr) address == IntPtr.Zero)
      return;
    if (targetable)
    {
      ref ObjectTargetableFlags local = ref address->TargetableStatus;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(sbyte&) ref local = (sbyte) ((int) ^(byte&) ref local | 2);
    }
    else
    {
      ref ObjectTargetableFlags local = ref address->TargetableStatus;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(sbyte&) ref local = (sbyte) ((int) ^(byte&) ref local & 253);
    }
  }

  public static unsafe void SetGPoseTarget(this IGameObject gameObject)
  {
    if (!gameObject.IsValid())
      return;
    TargetSystem* targetSystemPtr = TargetSystem.Instance();
    if ((IntPtr) targetSystemPtr == IntPtr.Zero || (IntPtr) targetSystemPtr->GPoseTarget == IntPtr.Zero)
      return;
    targetSystemPtr->GPoseTarget = (GameObject*) gameObject.Address;
  }

  public static unsafe void Redraw(this IGameObject gameObject)
  {
    GameObject* address = (GameObject*) gameObject.Address;
    if ((IntPtr) address == IntPtr.Zero)
      return;
    ((GameObject) (IntPtr) address).DisableDraw();
    ((GameObject) (IntPtr) address).EnableDraw();
  }
}
