// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Types.ISceneManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Threading.Tasks;

using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Data;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Modules;

namespace Ktisis.Scene.Types;

public interface ISceneManager : IComposite, IDisposable {
	bool IsValid { get; }

	IEditorContext Context { get; }

	IEntityFactory Factory { get; }

	double UpdateTime { get; }

	T GetModule<T>() where T : SceneModule;

	bool TryGetModule<T>(out T? module) where T : SceneModule;

	void Initialize();

	void Update();

	void Refresh();

	ActorEntity? GetEntityForActor(IGameObject actor);

	Task ApplyLightFile(
		LightEntity lightEntity,
		SceneManager.LightFile file,
		PoseTransforms transforms = PoseTransforms.Rotation | PoseTransforms.Scale,
		bool importLighting = true,
		bool importColor = true,
		bool importShadows = true
	);

	Task<SceneManager.LightFile> SaveLightFile(LightEntity lightEntity);
}
