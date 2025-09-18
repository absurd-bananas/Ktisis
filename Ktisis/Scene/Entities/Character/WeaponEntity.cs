// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Character.WeaponEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Character;

public class WeaponEntity : CharaEntity {
	public WeaponEntity(ISceneManager scene, IPoseBuilder pose)
		: base(scene, pose) {
		this.Type = EntityType.Weapon;
	}
}
