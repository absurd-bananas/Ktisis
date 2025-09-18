// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.World.WorldEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Math;
using Ktisis.Common.Utility;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Scene.Entities.World;

public class WorldEntity(ISceneManager scene) : SceneEntity(scene), ITransform, IVisibility
{
  public IntPtr Address { get; set; }

  public bool Visible { get; set; }

  public virtual unsafe Object* GetObject() => (Object*) this.Address;

  public virtual unsafe bool IsObjectValid => (IntPtr) this.GetObject() != IntPtr.Zero;

  public virtual void Setup() => this.Clear();

  public override void Update()
  {
    if (!this.IsObjectValid)
      return;
    this.UpdateChildren();
    base.Update();
  }

  private unsafe void UpdateChildren()
  {
    Object* objectPtr1 = this.GetObject();
    if ((IntPtr) objectPtr1 == IntPtr.Zero)
      return;
    List<IntPtr> numList = new List<IntPtr>();
    Object* childObject = objectPtr1->ChildObject;
    Object* objectPtr2 = childObject;
    while ((IntPtr) objectPtr2 != IntPtr.Zero)
    {
      numList.Add((IntPtr) objectPtr2);
      objectPtr2 = objectPtr2->NextSiblingObject;
      if (objectPtr2 == childObject)
        break;
    }
    foreach (WorldEntity worldEntity in this.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (x => x is WorldEntity)).Cast<WorldEntity>().ToList<WorldEntity>())
    {
      if (numList.Contains(worldEntity.Address))
        numList.Remove(worldEntity.Address);
      else
        worldEntity.Remove();
    }
    foreach (Object* ptr in numList)
      this.CreateObjectEntity(ptr);
  }

  private unsafe void CreateObjectEntity(Object* ptr)
  {
    Ktisis.Ktisis.Log.Verbose($"Creating object entity for {(IntPtr) ptr:X}", Array.Empty<object>());
    this.Scene.Factory.BuildObject().SetAddress(ptr).Add((IComposite) this);
  }

  public unsafe Transform? GetTransform()
  {
    Object* objectPtr = this.GetObject();
    return (IntPtr) objectPtr == IntPtr.Zero ? (Transform) null : new Transform(Vector3.op_Implicit(objectPtr->Position), Quaternion.op_Implicit(objectPtr->Rotation), Vector3.op_Implicit(objectPtr->Scale));
  }

  public virtual unsafe void SetTransform(Transform trans)
  {
    Object* objectPtr = this.GetObject();
    if ((IntPtr) objectPtr == IntPtr.Zero)
      return;
    objectPtr->Position = Vector3.op_Implicit(trans.Position);
    objectPtr->Rotation = Quaternion.op_Implicit(trans.Rotation);
    objectPtr->Scale = Vector3.op_Implicit(trans.Scale);
  }
}
