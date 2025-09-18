// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.CustomizeService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Core.Attributes;
using Ktisis.Structs.Characters;

namespace Ktisis.Services.Data;

[Singleton]
public class CustomizeService {
	private readonly IDataManager _data;

	public CustomizeService(IDataManager data) {
		this._data = data;
	}

	public ushort CalcDataIdFor(Tribe tribe, Gender gender) {
		var flag = gender == Gender.Masculine;
		int num1;
		switch (tribe) {
			case Tribe.Midlander:
				num1 = flag ? 101 : 201;
				break;
			case Tribe.Highlander:
				num1 = flag ? 301 : 401;
				break;
			default:
				var race = (Race)(byte)Math.Floor(((byte)tribe + 1M) / 2M);
				int num2;
				switch (race) {
					case Race.Elezen:
						num2 = flag ? 501 : 601;
						break;
					case Race.Lalafell:
						num2 = flag ? 1101 : 1201;
						break;
					case Race.Miqote:
						num2 = flag ? 701 : 801;
						break;
					case Race.Roegadyn:
						num2 = flag ? 901 : 1001;
						break;
					default:
						num2 = 1301 + (int)(race - 6) * 200 + (flag ? 0 : 100);
						break;
				}
				num1 = num2;
				break;
		}
		return (ushort)num1;
	}

	public bool IsFaceIdValidFor(ushort dataId, int faceId) => this._data.FileExists(ResolveFacePath(dataId, faceId));

	public IEnumerable<byte> GetFaceTypes(ushort dataId) {
		for (var i = 0; i <= byte.MaxValue; ++i) {
			if (this.IsFaceIdValidFor(dataId, i))
				yield return (byte)i;
		}
	}

	public byte FindBestFaceTypeFor(ushort dataId, byte current) {
		var flag1 = false;
		for (var faceId = 0; faceId < byte.MaxValue; ++faceId) {
			var flag2 = this.IsFaceIdValidFor(dataId, faceId);
			if (!flag2 && faceId < 90)
				flag2 |= this.IsFaceIdValidFor(dataId, faceId + 100);
			if (flag2) {
				if (!flag1) {
					flag1 = true;
					if (faceId > current)
						return (byte)faceId;
				}
			} else if (flag1)
				return (byte)(faceId - 1);
		}
		return current;
	}

	public bool IsHairIdValidFor(ushort dataId, int hairId) => this._data.FileExists(ResolveHairPath(dataId, hairId));

	public IEnumerable<byte> GetHairTypes(ushort dataId) {
		for (var i = 0; i <= byte.MaxValue; ++i) {
			if (this.IsHairIdValidFor(dataId, i))
				yield return (byte)i;
		}
	}

	private static string ResolveFacePath(ushort dataId, int faceId) => string.Format("chara/human/c{0:D4}/obj/face/f{1:D4}/model/c{0:D4}f{1:D4}_fac.mdl", dataId, faceId);

	private static string ResolveHairPath(ushort dataId, int hairId) => string.Format("chara/human/c{0:D4}/obj/hair/h{1:D4}/model/c{0:D4}h{1:D4}_hir.mdl", dataId, hairId);
}
