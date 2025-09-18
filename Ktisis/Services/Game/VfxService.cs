// Decompiled with JetBrains decompiler
// Type: Ktisis.Services.Game.VfxService
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Core.Attributes;
using Ktisis.Structs.Vfx.Apricot;

namespace Ktisis.Services.Game;

[Singleton]
public class VfxService {
	[Signature("E8 ?? ?? ?? ?? 48 8B 14 1E")]
	private GetApricotCoreDelegate? GetApricotCoreFunc;

	public VfxService(IGameInteropProvider interop) {
		interop.InitializeFromAttributes((object)this);
	}

	public unsafe ApricotCore* GetApricotCore() => this.GetApricotCoreFunc == null ? null : this.GetApricotCoreFunc();

	private unsafe delegate ApricotCore* GetApricotCoreDelegate();
}
