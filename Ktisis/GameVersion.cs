// Decompiled with JetBrains decompiler
// Type: Ktisis.GameVersion
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis;

public static class GameVersion {
	public const string Validated = "2023.11.09.0000.0000";

	public unsafe static string GetCurrent() {
		FFXIVClientStructs.FFXIV.Client.System.Framework.Framework* frameworkPtr = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
		return (IntPtr)frameworkPtr == IntPtr.Zero ? string.Empty : ((FFXIVClientStructs.FFXIV.Client.System.Framework.Framework)(IntPtr)frameworkPtr).GameVersionString;
	}
}
