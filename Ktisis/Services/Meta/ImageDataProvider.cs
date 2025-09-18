// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Meta.ImageDataProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.IO;

using GLib.Popups.ImFileDialog;
using GLib.Popups.ImFileDialog.Data;

using Ktisis.Core.Attributes;

namespace Ktisis.Services.Meta;

[Singleton]
public class ImageDataProvider {
	private readonly FileMetaHandler _handler;
	private readonly ITextureProvider _tex;

	public ImageDataProvider(ITextureProvider tex) {
		this._tex = tex;
		this._handler = new FileMetaHandler(tex);
	}

	public void Initialize() {
		this._handler.AddFileType("*", this.BuildMeta);
	}

	public void BindMetadata(FileDialog dialog) => dialog.WithMetadata(this._handler);

	public ISharedImmediateTexture GetFromFile(string path) => this._tex.GetFromFile(path);

	private FileMeta BuildMeta(string path) {
		ISharedImmediateTexture fromFile = this.GetFromFile(path);
		return new FileMeta(Path.GetFileName(path)) {
			Texture = fromFile
		};
	}
}
