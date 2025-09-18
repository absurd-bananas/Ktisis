// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.IItemSelect
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities;

namespace Ktisis.Interface.Overlay;

public interface IItemSelect {
	string Name { get; }

	SceneEntity Entity { get; }

	Vector2 ScreenPos { get; }

	float Distance { get; }

	bool IsHovered { get; set; }

	float OpacityMultiplier { get; set; }
}
