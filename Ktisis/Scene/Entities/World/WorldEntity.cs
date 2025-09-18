// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.World.WorldEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Common.Utility;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.World;

public class WorldEntity(ISceneManager scene) : SceneEntity(scene), ITransform, IVisibility {
	public IntPtr Address { get; set; }

	public unsafe virtual bool IsObjectValid => (IntPtr)this.GetObject() != IntPtr.Zero;

	public unsafe Transform? GetTransform() {
		var objectPtr = this.GetObject();
		return (IntPtr)objectPtr == IntPtr.Zero ? null : new Transform(Vector3.op_Implicit(objectPtr->Position), Quaternion.op_Implicit(objectPtr->Rotation), Vector3.op_Implicit(objectPtr->Scale));
	}

	public unsafe virtual void SetTransform(Transform trans) {
		var objectPtr = this.GetObject();
		if ((IntPtr)objectPtr == IntPtr.Zero)
			return;
		objectPtr->Position = Vector3.op_Implicit(trans.Position);
		objectPtr->Rotation = Quaternion.op_Implicit(trans.Rotation);
		objectPtr->Scale = Vector3.op_Implicit(trans.Scale);
	}

	public bool Visible { get; set; }

	public unsafe virtual object* GetObject() => (object*)this.Address;

	public virtual void Setup() => this.Clear();

	public override void Update() {
		if (!this.IsObjectValid)
			return;
		this.UpdateChildren();
		base.Update();
	}

	private unsafe void UpdateChildren() {
		var objectPtr1 = this.GetObject();
		if ((IntPtr)objectPtr1 == IntPtr.Zero)
			return;
		var numList = new List<IntPtr>();
		object* childObject = objectPtr1->ChildObject;
		var objectPtr2 = childObject;
		while ((IntPtr)objectPtr2 != IntPtr.Zero) {
			numList.Add((IntPtr)objectPtr2);
			objectPtr2 = objectPtr2->NextSiblingObject;
			if (objectPtr2 == childObject)
				break;
		}
		foreach (var worldEntity in this.Children.Where(x => x is WorldEntity).Cast<WorldEntity>().ToList()) {
			if (numList.Contains(worldEntity.Address))
				numList.Remove(worldEntity.Address);
			else
				worldEntity.Remove();
		}
		foreach (object* ptr in numList)
			this.CreateObjectEntity(ptr);
	}

	private unsafe void CreateObjectEntity(object* ptr) {
		Ktisis.Ktisis.Log.Verbose($"Creating object entity for {(IntPtr)ptr:X}", Array.Empty<object>());
		this.Scene.Factory.BuildObject().SetAddress(ptr).Add(this);
	}
}
