// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.PenumbraIpcProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;

namespace Ktisis.Interop.Ipc;

public class PenumbraIpcProvider {
	private readonly AddTemporaryMod _addTemporaryMod;
	private readonly AssignTemporaryCollection _assignTemporaryCollection;
	private readonly ICallGateSubscriber<string, string, (PenumbraApiEc, Guid Guid)> _createTemporaryCollection;
	private readonly DeleteTemporaryCollection _deleteTemporaryCollection;
	private readonly GetCollectionForObject _getCollectionForObject;
	private readonly GetCollections _getCollections;
	private readonly GetCutsceneParentIndex _getCutsceneParentIndex;
	private readonly RedrawObject _redrawObject;
	private readonly RemoveTemporaryMod _removeTemporaryMod;
	private readonly SetCollectionForObject _setCollectionForObject;
	private readonly SetCutsceneParentIndex _setCutsceneParentIndex;

	public PenumbraIpcProvider(IDalamudPluginInterface dpi) {
		this._getCollections = new GetCollections(dpi);
		this._getCollectionForObject = new GetCollectionForObject(dpi);
		this._setCollectionForObject = new SetCollectionForObject(dpi);
		this._getCutsceneParentIndex = new GetCutsceneParentIndex(dpi);
		this._setCutsceneParentIndex = new SetCutsceneParentIndex(dpi);
		this._assignTemporaryCollection = new AssignTemporaryCollection(dpi);
		this._createTemporaryCollection = dpi.GetIpcSubscriber<string, string, (PenumbraApiEc, Guid)>("Penumbra.CreateTemporaryCollection.V6");
		this._deleteTemporaryCollection = new DeleteTemporaryCollection(dpi);
		this._addTemporaryMod = new AddTemporaryMod(dpi);
		this._removeTemporaryMod = new RemoveTemporaryMod(dpi);
		this._redrawObject = new RedrawObject(dpi);
	}

	public Dictionary<Guid, string> GetCollections() => this._getCollections.Invoke();

	public (Guid Id, string Name) GetCollectionForObject(IGameObject gameObject) => this._getCollectionForObject.Invoke((int)gameObject.ObjectIndex).EffectiveCollection;

	public bool SetCollectionForObject(IGameObject gameObject, Guid id) {
		Ktisis.Ktisis.Log.Verbose($"Setting collection for '{gameObject.Name}' ({gameObject.ObjectIndex}) to '{id}'", Array.Empty<object>());
		var penumbraApiEc = this._setCollectionForObject.Invoke((int)gameObject.ObjectIndex, id).Item1;
		var num = penumbraApiEc == PenumbraApiEc.Success ? 1 : 0;
		if (num != 0)
			return num != 0;
		Ktisis.Ktisis.Log.Warning($"Penumbra collection set failed with return code: {penumbraApiEc}", Array.Empty<object>());
		return num != 0;
	}

	public int GetAssignedParentIndex(IGameObject gameObject) => this._getCutsceneParentIndex.Invoke((int)gameObject.ObjectIndex);

	public void AssignTemporaryCollection(Guid collectionId, int actorIndex) {
		var num = (int)this._assignTemporaryCollection.Invoke(collectionId, actorIndex);
	}

	public Guid CreateTemporaryCollection(string name) => this._createTemporaryCollection.InvokeFunc(name, name).Guid;

	public void DeleteTemporaryCollection(Guid collectionId) {
		var num = (int)this._deleteTemporaryCollection.Invoke(collectionId);
	}

	public bool SetAssignedParentIndex(IGameObject gameObject, int index) {
		Ktisis.Ktisis.Log.Verbose($"Setting assigned parent for '{gameObject.Name}' ({gameObject.ObjectIndex}) to {index}", Array.Empty<object>());
		var penumbraApiEc = this._setCutsceneParentIndex.Invoke((int)gameObject.ObjectIndex, index);
		var num = penumbraApiEc == PenumbraApiEc.Success ? 1 : 0;
		if (num != 0)
			return num != 0;
		Ktisis.Ktisis.Log.Warning($"Penumbra parent set failed with return code: {penumbraApiEc}", Array.Empty<object>());
		return num != 0;
	}

	public void AssignTemporaryMods(Guid id, Guid collectionId, Dictionary<string, string> paths) {
		Ktisis.Ktisis.Log.Info($"{this._removeTemporaryMod.Invoke("MareChara_Files", collectionId, 0)} {this._addTemporaryMod.Invoke("MareChara_Files", collectionId, paths, string.Empty, 0)}", Array.Empty<object>());
	}

	public void AssignManipulationData(Guid id, Guid collectionId, string manipData) {
		var num = (int)this._addTemporaryMod.Invoke("MareChara_Meta", collectionId, new Dictionary<string, string>(), manipData, 0);
	}

	public void Redraw(int index) => this._redrawObject.Invoke(index);
}
