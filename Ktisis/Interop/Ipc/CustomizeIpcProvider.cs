// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.CustomizeIpcProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

namespace Ktisis.Interop.Ipc;

public class CustomizeIpcProvider {
	private readonly ICallGateSubscriber<ushort, (int, Guid?)> _getActiveProfileId;
	private readonly ICallGateSubscriber<(int, int)> _getApiVersion;
	private readonly ICallGateSubscriber<Guid, (int, string?)> _getProfileByUId;
	private readonly ICallGateSubscriber<IList<(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled)>> _getProfileList;
	private readonly ICallGateSubscriber<int, int, int> _setCsParentIndex;
	private readonly ICallGateSubscriber<ushort, string, (int, Guid?)> _setTemporaryProfile;
	private readonly ICallGateSubscriber<ushort, int> _unsetTemporaryProfile;

	public CustomizeIpcProvider(IDalamudPluginInterface dpi) {
		this._getApiVersion = dpi.GetIpcSubscriber<(int, int)>("CustomizePlus.General.GetApiVersion");
		this._getProfileList = dpi.GetIpcSubscriber<IList<(Guid, string, string, List<(string, ushort, byte, ushort)>, int, bool)>>("CustomizePlus.Profile.GetList");
		this._getActiveProfileId = dpi.GetIpcSubscriber<ushort, (int, Guid?)>("CustomizePlus.Profile.GetActiveProfileIdOnCharacter");
		this._getProfileByUId = dpi.GetIpcSubscriber<Guid, (int, string)>("CustomizePlus.Profile.GetByUniqueId");
		this._setTemporaryProfile = dpi.GetIpcSubscriber<ushort, string, (int, Guid?)>("CustomizePlus.Profile.SetTemporaryProfileOnCharacter");
		this._unsetTemporaryProfile = dpi.GetIpcSubscriber<ushort, int>("CustomizePlus.Profile.DeleteTemporaryProfileOnCharacter");
		this._setCsParentIndex = dpi.GetIpcSubscriber<int, int, int>("CustomizePlus.GameState.SetCutsceneParentIndex");
	}

	public bool IsCompatible() {
		(int, int) tuple = this._getApiVersion.InvokeFunc();
		var num = tuple.Item1;
		return num > 5 || num == 5 && tuple.Item2 >= 1;
	}

	public IList<(Guid UniqueId, string Name, string VirtualPath, List<(string Name, ushort WorldId, byte CharacterType, ushort CharacterSubType)> Characters, int Priority, bool IsEnabled)> GetProfileList() => this._getProfileList.InvokeFunc();

	public (int, Guid? Id) GetActiveProfileId(ushort gameObjectIndex) => this._getActiveProfileId.InvokeFunc(gameObjectIndex);

	public (int, string? Data) GetProfileByUniqueId(Guid id) => this._getProfileByUId.InvokeFunc(id);

	public (int, Guid? Id) SetTemporaryProfile(ushort gameObjectIndex, string profileJson) => this._setTemporaryProfile.InvokeFunc(gameObjectIndex, profileJson);

	public int DeleteTemporaryProfile(ushort gameObjectIndex) => this._unsetTemporaryProfile.InvokeFunc(gameObjectIndex);

	public int SetCutsceneParentIndex(int copyIndex, int newParentIndex) => this._setCsParentIndex.InvokeFunc(copyIndex, newParentIndex);
}
