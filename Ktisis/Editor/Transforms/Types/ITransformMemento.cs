// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.Types.ITransformMemento
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Types;
using Ktisis.Common.Utility;

namespace Ktisis.Editor.Transforms.Types;

public interface ITransformMemento : IMemento {
	ITransformMemento Save();

	void SetTransform(Transform transform);

	void SetMatrix(Matrix4x4 matrix);

	void Dispatch();
}
