// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Mcdf.McdfManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ktisis.Common.Extensions;
using Ktisis.Core.Attributes;
using Ktisis.Interop.Ipc;

namespace Ktisis.Data.Mcdf;

[Singleton]
public sealed class McdfManager : IDisposable {
	private readonly IFramework _framework;
	private readonly IpcManager _ipc;

	public McdfManager(IFramework framework, IpcManager ipc) {
		this._framework = framework;
		this._ipc = ipc;
	}

	public void Dispose() {
		var tempPath = GetTempPath(false);
		if (!Directory.Exists(tempPath))
			return;
		Directory.Delete(tempPath, true);
	}

	public void LoadAndApplyTo(string path, IGameObject actor) {
		this.LoadAndApplyToAsync(path, actor).ContinueWith(task => {
			if (task.Exception == null)
				return;
			Ktisis.Ktisis.Log.Error($"Failed to load MCDF:\n{task.Exception.InnerException}", Array.Empty<object>());
		}, (TaskContinuationOptions)327680 /*0x050000*/);
	}

	private async Task LoadAndApplyToAsync(string path, IGameObject actor) {
		McdfData data;
		Dictionary<string, string> extracted;
		using (var reader = McdfReader.FromPath(path)) {
			var tempPath = GetTempPath(true);
			Ktisis.Ktisis.Log.Debug("Reading and extracting MCDF file", Array.Empty<object>());
			data = reader.GetData();
			extracted = reader.Extract(tempPath);
			var dictionary = Enumerable.ToDictionary<string, string>((IEnumerable<KeyValuePair<string, string>>)extracted);
			foreach ((string, string) valueTuple in data.FileSwaps.SelectMany<McdfData.FileSwap, string, (string, string)>((Func<McdfData.FileSwap, IEnumerable<string>>)(x => (IEnumerable<string>)x.GamePaths),
				(Func<McdfData.FileSwap, string, (string, string)>)((k, p) => (p, k.FileSwapPath))))
				dictionary[valueTuple.Item1] = valueTuple.Item2;
			Ktisis.Ktisis.Log.Debug("Applying MCDF data", Array.Empty<object>());
			var collectionId = this.ApplyPenumbraMods(actor, data, dictionary);
			this.ApplyGlamourerData(actor, data);
			await this.RedrawAndWait(actor);
			if (collectionId.HasValue)
				this._ipc.GetPenumbraIpc().DeleteTemporaryCollection(collectionId.Value);
			this.ApplyCustomizeData(actor, data);
			Ktisis.Ktisis.Log.Debug("Cleaning up extracted files", Array.Empty<object>());
			foreach (var str in extracted.Values)
				File.Delete(str);
		}
		data = (McdfData)null;
		extracted = null;
	}

	private void ApplyCustomizeData(IGameObject actor, McdfData data) {
		if (!this._ipc.IsCustomizeActive)
			return;
		var customizeIpc = this._ipc.GetCustomizeIpc();
		string customizePlusData = data.CustomizePlusData;
		var str = !StringExtensions.IsNullOrEmpty(customizePlusData) ? Encoding.UTF8.GetString(Convert.FromBase64String(customizePlusData)) : "{}";
		Ktisis.Ktisis.Log.Info(str, Array.Empty<object>());
		var objectIndex = (int)actor.ObjectIndex;
		var profileJson = str;
		customizeIpc.SetTemporaryProfile((ushort)objectIndex, profileJson);
	}

	private void ApplyGlamourerData(IGameObject actor, McdfData data) {
		if (!this._ipc.IsGlamourerActive)
			return;
		this._ipc.GetGlamourerIpc().ApplyState(data.GlamourerData, (int)actor.ObjectIndex);
	}

	private Guid? ApplyPenumbraMods(
		IGameObject actor,
		McdfData data,
		Dictionary<string, string> files
	) {
		if (!this._ipc.IsPenumbraActive)
			return new Guid?();
		var penumbraIpc = this._ipc.GetPenumbraIpc();
		var temporaryCollection = penumbraIpc.CreateTemporaryCollection($"KtisisMCDF_{actor.ObjectIndex}");
		penumbraIpc.AssignTemporaryCollection(temporaryCollection, (int)actor.ObjectIndex);
		var id = Guid.NewGuid();
		penumbraIpc.AssignTemporaryMods(id, temporaryCollection, files);
		penumbraIpc.AssignManipulationData(id, temporaryCollection, data.ManipulationData);
		return temporaryCollection;
	}

	private async Task RedrawAndWait(IGameObject actor) {
		actor.Redraw();
		var start = DateTime.Now;
		do {
			if (!await this._framework.RunOnFrameworkThread<bool>(new Func<bool>(((GameObjectEx)actor).IsDrawing)))
				await Task.Delay(100);
			else
				goto label_2;
		} while (actor.IsValid() && (DateTime.Now - start).TotalMilliseconds < 20000.0);
		goto label_5;
		label_2:
		return;
		label_5:
		Ktisis.Ktisis.Log.Warning($"Timed out waiting for '{actor.Name}' to redraw!", Array.Empty<object>());
	}

	private static string GetTempPath(bool create) {
		string tempPath = Path.Join(Path.GetTempPath(), "Ktisis");
		if (create && !Directory.Exists(tempPath))
			Directory.CreateDirectory(tempPath);
		return tempPath;
	}
}
