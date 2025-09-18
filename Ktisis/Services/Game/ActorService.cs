// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Game.ActorService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Core.Attributes;

namespace Ktisis.Services.Game;

[Singleton]
public class ActorService {
	public const ushort GPoseIndex = 201;
	public const ushort GPoseCount = 42;
	private readonly IObjectTable _objectTable;

	public ActorService(IObjectTable objectTable) {
		this._objectTable = objectTable;
	}

	public IGameObject? GetIndex(int index) => this._objectTable[index];

	public IGameObject? GetAddress(IntPtr address) => this._objectTable.CreateObjectReference(address);

	public IEnumerable<IGameObject> GetGPoseActors() {
		for (ushort i = 201; i < 243; ++i) {
			IGameObject index = this.GetIndex(i);
			if (index != null)
				yield return index;
		}
	}

	public IEnumerable<IGameObject> GetOverworldActors() {
		for (var i = 0; i < 200; ++i) {
			IGameObject index = this.GetIndex(i);
			if (index != null && index.IsEnabled())
				yield return index;
		}
	}

	public unsafe IGameObject? GetSkeletonOwner(Skeleton* skeleton) {
		foreach (IGameObject gameObject in (IEnumerable<IGameObject>)this._objectTable) {
			GameObject* address = (GameObject*)gameObject.Address;
			if ((IntPtr)address != IntPtr.Zero && (IntPtr)address->DrawObject != IntPtr.Zero && ((FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object) ref address ->DrawObject->Object).GetObjectType() == 3 && gameObject.GetSkeleton() == skeleton)
			return gameObject;
		}
		return (IGameObject)null;
	}
}
