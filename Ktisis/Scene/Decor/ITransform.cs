// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Decor.ITransform
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Common.Utility;

namespace Ktisis.Scene.Decor;

public interface ITransform {
	Transform? GetTransform();

	void SetTransform(Transform trans);

	Matrix4x4? GetMatrix() => this.GetTransform()?.ComposeMatrix();

	void SetMatrix(Matrix4x4 mx) => this.SetTransform(new Transform(mx));
}
