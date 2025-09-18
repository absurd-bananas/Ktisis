// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Selection.ISelectManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

using Ktisis.Scene.Entities;

namespace Ktisis.Editor.Selection;

public interface ISelectManager {

	int Count { get; }
	event SelectChangedHandler Changed;

	void Update();

	IEnumerable<SceneEntity> GetSelected();

	SceneEntity? GetFirstSelected();

	bool IsSelected(SceneEntity entity);

	void Select(SceneEntity entity, SelectMode mode = SelectMode.Default);

	void Unselect(SceneEntity entity);

	void Clear();
}
