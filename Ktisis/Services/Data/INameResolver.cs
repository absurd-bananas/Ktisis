// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Data.INameResolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
namespace Ktisis.Services.Data;

public interface INameResolver {
	string? GetWeaponName(ushort id, ushort secondId, ushort variant);
}
