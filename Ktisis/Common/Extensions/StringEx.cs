// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.StringEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;

namespace Ktisis.Common.Extensions;

public static class StringEx {
	public static string Truncate(this string str, int len, bool ellipsis = true) {
		if (str.Length <= len)
			return str;
		var length1 = Math.Min(len, str.Length);
		var count = Math.Min(len - 2, 3);
		if (count <= 1 || !ellipsis)
			return str.Substring(0, length1);
		var length2 = length1 - count;
		return str.Substring(0, length2) + new string('.', count);
	}

	public static string FitToWidth(this string str, float width, bool ellipsis = true) {
		var width1 = str;
		var length1 = width1.Length;
		var flag = false;
		for (; length1 > 0 && (double)Dalamud.Bindings.ImGui.ImGui.CalcTextSize(ImU8String.op_Implicit(width1), false, -1f).X > width; width1 = width1.Substring(0, length1)) {
			flag = true;
			--length1;
		}
		if (ellipsis & flag && length1 >= 5) {
			var length2 = length1 - 3;
			width1 = width1.Substring(0, length2) + new string('.', 3);
		}
		return width1;
	}

	public static string? FormatName(this string name, sbyte article) {
		if (StringExtensions.IsNullOrEmpty(name))
			return null;
		return article != 1
			? string.Join<string>(' ', name.Split(' ', StringSplitOptions.None).Select((word, index) => {
				var flag1 = word.Length <= 1;
				if (!flag1) {
					var flag2 = index > 0;
					if (flag2)
						flag2 = word == "of" || word == "the" || word == "and";
					flag1 = flag2;
				}
				if (flag1)
					return word;
				var upper = word[0].ToString().ToUpper();
				var str1 = word;
				var str2 = str1.Substring(1, str1.Length - 1);
				return upper + str2;
			}))
			: name;
	}
}
