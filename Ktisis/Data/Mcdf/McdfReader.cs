// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Mcdf.McdfReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using K4os.Compression.LZ4.Legacy;

namespace Ktisis.Data.Mcdf;

public sealed class McdfReader : IDisposable {
	private const uint MareMagic = 1178878797;
	private readonly McdfHeader _header;
	private readonly LZ4Stream _lz4;
	private readonly FileStream _stream;

	private McdfReader(FileStream stream, LZ4Stream lz4, McdfHeader header) {
		this._stream = stream;
		this._lz4 = lz4;
		this._header = header;
	}

	public void Dispose() {
		this._lz4.Dispose();
		this._stream.Dispose();
	}

	public static McdfReader FromPath(string path) {
		var fileStream = File.OpenRead(path);
		var lz4 = new LZ4Stream(fileStream, LZ4StreamMode.Decompress, LZ4StreamFlags.HighCompression);
		McdfHeader header = ReadHeader(path, lz4);
		if (header == (McdfHeader)null)
			throw new Exception($"'{Path.GetFileName(path)}' is not a valid MCDF file.");
		return new McdfReader(fileStream, lz4, header);
	}

	private static McdfHeader? ReadHeader(string path, LZ4Stream lz4) {
		var binaryReader = new BinaryReader(lz4);
		if (binaryReader.ReadUInt32() != 1178878797U)
			return (McdfHeader)null;
		var num1 = binaryReader.ReadByte();
		if (num1 != 1)
			return (McdfHeader)null;
		var num2 = binaryReader.ReadInt32();
		var str = Encoding.UTF8.GetString(binaryReader.ReadBytes(num2));
		return new McdfHeader {
			Version = num1,
			FilePath = path,
			Data = JsonSerializer.Deserialize<McdfData>(str, (JsonSerializerOptions)null)
		};
	}

	public McdfData GetData() => this._header.Data;

	public Dictionary<string, string> Extract(string dir) {
		using (var binaryReader = new BinaryReader(this._lz4)) {
			var dictionary = new Dictionary<string, string>();
			foreach (McdfData.FileData file in this._header.Data.Files) {
				var str = Path.Combine(dir, $"ktisis_{file.Hash}.tmp");
				using (var fileStream = File.OpenWrite(str)) {
					using (var binaryWriter = new BinaryWriter(fileStream)) {
						var numArray = binaryReader.ReadBytes(file.Length);
						binaryWriter.Write(numArray);
						binaryWriter.Flush();
						foreach (string gamePath in file.GamePaths) {
							dictionary[gamePath] = str;
							Ktisis.Ktisis.Log.Debug($"{gamePath} => {Path.GetFileName(str)}", Array.Empty<object>());
						}
					}
				}
			}
			return dictionary;
		}
	}
}
