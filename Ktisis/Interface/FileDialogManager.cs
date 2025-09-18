// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.FileDialogManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.IO;
using System.Linq;

using GLib.Popups.ImFileDialog;
using GLib.Popups.ImFileDialog.Data;

using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Files;
using Ktisis.Data.Json;
using Ktisis.Services.Meta;

namespace Ktisis.Interface;

[Singleton]
public class FileDialogManager {
	private readonly ConfigManager _cfg;
	private readonly ImageDataProvider _img;
	private readonly JsonFileSerializer _serializer = new JsonFileSerializer();
	private readonly FileDialogOptions ImageOptions = new FileDialogOptions {
		Flags = FileDialogFlags.OpenMode,
		Filters = "Images{.png,.jpg,.jpeg}"
	};
	private FileDialogLocation? AutoSaveLoc;

	public FileDialogManager(ConfigManager cfg, ImageDataProvider img) {
		this._cfg = cfg;
		this._img = img;
	}

	public event Action<FileDialog>? OnOpenDialog;

	public void Initialize() => this._img.Initialize();

	private T OpenDialog<T>(T dialog) where T : FileDialog {
		string path;
		if (this._cfg.File.File.LastOpenedPaths.TryGetValue(dialog.Title, out path))
			dialog.Open(path);
		else
			dialog.Open();
		var onOpenDialog = this.OnOpenDialog;
		if (onOpenDialog != null)
			onOpenDialog(dialog);
		return dialog;
	}

	private void SaveDialogState(FileDialog dialog) {
		if (dialog.ActiveDirectory == null)
			return;
		this._cfg.File.File.LastOpenedPaths[dialog.Title] = dialog.ActiveDirectory;
	}

	public FileDialog OpenFile(string name, Action<string> handler, FileDialogOptions? options = null) {
		if ((object)options == null)
			options = new FileDialogOptions();
		this.PopulateOptions(options);
		return this.OpenDialog(new FileDialog(name, (FileDialogConfirmHandler)((sender, paths) => {
			this.SaveDialogState(sender);
			var str = paths.FirstOrDefault<string>();
			if (StringExtensions.IsNullOrEmpty(str))
				return;
			handler(str);
		}), options with { Flags = FileDialogFlags.OpenMode }));
	}

	public FileDialog OpenFile<T>(string name, Action<string, T> handler, FileDialogOptions? options = null) where T : JsonFile {
		return this.OpenFile(name, path => {
			T obj = this._serializer.Deserialize<T>(File.ReadAllText(path));
			if (obj == null)
				return;
			handler(path, obj);
		}, options);
	}

	public FileDialog SaveFile(string name, string content, FileDialogOptions? options = null) {
		if ((object)options == null)
			options = new FileDialogOptions();
		this.PopulateOptions(options);
		return this.OpenDialog(new FileDialog(name, (FileDialogConfirmHandler)((sender, paths) => {
			this.SaveDialogState(sender);
			var str = paths.FirstOrDefault<string>();
			if (StringExtensions.IsNullOrEmpty(str))
				return;
			File.WriteAllText(str, content);
		}), options));
	}

	public FileDialog SaveFile<T>(string name, T file, FileDialogOptions? options = null) where T : JsonFile {
		var content = this._serializer.Serialize(file);
		return this.SaveFile(name, content, options);
	}

	public FileDialog OpenImage(string name, Action<string> handler) {
		var dialog = new FileDialog(name, (sender, paths) => {
			foreach (var path in paths)
				handler(path);
		}, this.ImageOptions);
		this._img.BindMetadata(dialog);
		return this.OpenDialog<FileDialog>(dialog);
	}

	private void PopulateOptions(FileDialogOptions options) {
		var filePath = this._cfg.File.AutoSave.FilePath;
		if (this.AutoSaveLoc == null)
			this.AutoSaveLoc = new FileDialogLocation("AutoSave", filePath);
		else if (this.AutoSaveLoc.FullPath != filePath)
			this.AutoSaveLoc.FullPath = filePath;
		if (options.Locations.Contains(this.AutoSaveLoc))
			return;
		options.Locations.Add(this.AutoSaveLoc);
		Ktisis.Ktisis.Log.Info("Added autosave: " + filePath, Array.Empty<object>());
	}
}
