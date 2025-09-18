// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.WeaponPropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Character;

namespace Ktisis.Interface.Editor.Properties;

public class WeaponPropertyList : ObjectPropertyList {
	public override void Invoke(IPropertyListBuilder builder, SceneEntity entity) {
		var weaponEntity = entity as WeaponEntity;
	}
}
