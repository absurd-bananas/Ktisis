// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.PlayerNameUtil
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Common.Utility;

public static class PlayerNameUtil {
	private readonly static string[] Single = new string[16 /*0x10*/] {
		"Zero",
		"One",
		"Two",
		"Three",
		"Four",
		"Five",
		"Six",
		"Seven",
		"Eight",
		"Nine",
		"Ten",
		"Eleven",
		"Twelve",
		"Thirteen",
		"Fourteen",
		"Fifteen"
	};
	private readonly static string[] Tens = new string[8] {
		"Twenty",
		"Thirty",
		"Forty",
		"Fifty",
		"Sixty",
		"Seventy",
		"Eighty",
		"Ninety"
	};

	public static string CalcActorName(ushort index, string firstName = "Actor") => $"{firstName} {CalcActorNameWords(index)}";

	private static string CalcActorNameWords(ushort index) {
		if (index >= 200)
			index -= 200;
		if (index < Single.Length)
			return Single[index];
		if (index < 20) {
			var index1 = index - 10;
			var str1 = Single[index1];
			if (str1.EndsWith('t')) {
				var str2 = str1;
				str1 = str2.Substring(0, str2.Length - 1);
			}
			return str1 + "teen";
		}
		var index2 = (int)Math.Floor((index - 20) / 10M);
		var index3 = index % 10;
		var ten = Tens[index2];
		return index3 == 0 ? ten : $"{ten}-{Single[index3].ToLowerInvariant()}";
	}
}
