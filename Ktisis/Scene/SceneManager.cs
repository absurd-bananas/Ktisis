// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.SceneManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ktisis.Actions.Types;
using Ktisis.Common.Utility;
using Ktisis.Data.Files;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Data;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Modules;
using Ktisis.Scene.Modules.Actors;
using Ktisis.Scene.Modules.Lights;
using Ktisis.Scene.Types;
using Ktisis.Structs.Common;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene;

public class SceneManager : SceneModuleContainer, ISceneManager, IComposite, IDisposable {
	private readonly IFramework _framework;
	private readonly SceneRoot Root;
	private bool IsDisposing;

	public SceneManager(
		IEditorContext context,
		HookScope scope,
		IEntityFactory factory,
		IFramework framework
	)
		: base(scope) {
		this.Context = context;
		this.Factory = factory;
		this.Root = new SceneRoot(this);
		this._framework = framework;
	}

	public bool IsValid => this.Context.IsValid && !this.IsDisposing;

	public IEditorContext Context { get; }

	public IEntityFactory Factory { get; }

	public void Initialize() {
		Ktisis.Ktisis.Log.Info("Initializing scene...", Array.Empty<object>());
		this.SetupModules();
	}

	public double UpdateTime { get; private set; }

	public void Update() {
		if (!this.IsValid)
			return;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		this.UpdateModules();
		this.Root.Update();
		stopwatch.Stop();
		this.UpdateTime = stopwatch.Elapsed.TotalMilliseconds;
	}

	public void Refresh() {
		foreach (var configurable in this.Root.Recurse().Where(entity => entity is IConfigurable).Cast<IConfigurable>())
			configurable.Refresh();
	}

	public SceneEntity? Parent {
		get => this.Root.Parent;
		set => this.Root.Parent = value;
	}

	public IEnumerable<SceneEntity> Children => this.Root.Children;

	public bool Add(SceneEntity entity) => this.Root.Add(entity);

	public bool Remove(SceneEntity entity) => this.Root.Remove(entity);

	public IEnumerable<SceneEntity> Recurse() => this.Root.Recurse();

	public ActorEntity? GetEntityForActor(IGameObject actor) {
		return this.Children.ToList().Where(entity => entity is ActorEntity actorEntity && actorEntity.IsValid).Cast<ActorEntity>().FirstOrDefault((Func<ActorEntity, bool>)(entity => (int)entity.Actor.ObjectIndex == (int)actor.ObjectIndex));
	}

	public void Dispose() {
		this.IsDisposing = true;
		try {
			this.Root.Clear();
			this.DisposeModules();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to dispose scene!\n{ex}", Array.Empty<object>());
		}
		GC.SuppressFinalize(this);
	}

	public Task ApplyLightFile(
		LightEntity lightEntity,
		LightFile file,
		PoseTransforms transforms = PoseTransforms.Rotation | PoseTransforms.Scale,
		bool importLighting = true,
		bool importColor = true,
		bool importShadows = true
	) {
		return this._framework.RunOnFrameworkThread((Action)(() => {
			if (file == null)
				return;
			var converter = new EntityLightConverter(lightEntity);
			var lightFile1 = converter.Save();
			var lightFile2 = new LightFile();
			var source = this.Context.Scene.Children.Where(entity => entity is ActorEntity).Cast<ActorEntity>();
			var actor = (ActorEntity)null;
			if (source.Count() > 0)
				actor = source.FirstOrDefault(x => x.IsSelected) ?? source.FirstOrDefault(x => this.GetModule<GroupPoseModule>().IsPrimaryActor(x));
			converter.Load(file, transforms, importLighting, importColor, importShadows, actor);
			var lightFile3 = converter.Save();
			this.Context.Actions.History.Add(new LightMemento(converter) {
				Transforms = transforms,
				Lighting = importLighting,
				Color = importColor,
				Shadows = importShadows,
				Initial = lightFile1,
				Final = lightFile3
			});
		}));
	}

	public unsafe Task<LightFile> SaveLightFile(LightEntity lightEntity) {
		return this._framework.RunOnFrameworkThread<LightFile>((Func<LightFile>)(() => {
			var lightFile = new LightFile();
			var sceneLightPtr = lightEntity.GetObject();
			RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
			if ((IntPtr)renderLight == IntPtr.Zero)
				return lightFile;
			var source = this.Context.Scene.Children.Where(entity => entity is ActorEntity).Cast<ActorEntity>();
			var actorEntity = (ActorEntity)null;
			if (source.Count() > 0)
				actorEntity = source.FirstOrDefault(x => x.IsSelected) ?? source.FirstOrDefault(x => this.GetModule<GroupPoseModule>().IsPrimaryActor(x));
			var transform1 = lightEntity.GetTransform();
			lightFile.Transform = transform1;
			if (actorEntity != null && transform1 != null && lightFile.Transform != null) {
				var transform2 = actorEntity.Pose?.FindBoneByName("n_hara")?.GetTransform();
				var transform3 = actorEntity.GetTransform();
				if (transform2 == null)
					transform2 = transform3;
				var transform4 = transform2;
				if (transform4 != null) {
					lightFile.Transform.Position = Vector3.Transform(transform1.Position - transform4.Position, Quaternion.Inverse(transform4.Rotation));
					lightFile.Transform.Rotation = Quaternion.Inverse(transform4.Rotation) * transform1.Rotation;
				}
			}
			lightFile.LightType = renderLight->LightType;
			lightFile.Flags = renderLight->Flags;
			lightFile.LightAngle = renderLight->LightAngle;
			lightFile.FalloffAngle = renderLight->FalloffAngle;
			lightFile.AreaAngle = renderLight->AreaAngle;
			lightFile.FalloffType = renderLight->FalloffType;
			lightFile.Falloff = renderLight->Falloff;
			lightFile.Color = renderLight->Color;
			lightFile.Range = renderLight->Range;
			lightFile.CharaShadowRange = renderLight->CharaShadowRange;
			lightFile.ShadowNear = renderLight->ShadowNear;
			lightFile.ShadowFar = renderLight->ShadowFar;
			return lightFile;
		}));
	}

	private void SetupModules() {
		var groupPoseModule = this.AddModule<GroupPoseModule>();
		this.AddModule<ActorModule>(groupPoseModule);
		this.AddModule<LightModule>(groupPoseModule);
		this.AddModule<EnvModule>();
		this.InitializeModules();
		this.SetupSavedState();
	}

	private void SetupSavedState() {
		foreach (ReferenceImage.SetupData referenceImage in this.Context.Config.Editor.ReferenceImages)
			this.Factory.BuildRefImage().FromData(referenceImage).Add();
	}

	public class LightFile : JsonFile {
		public const int CurrentVersion = 2;

		public new string FileExtension { get; set; } = ".light";

		public new string TypeName { get; set; } = "Ktisis Light";

		public Transform? Transform { get; set; }

		public LightType LightType { get; set; }

		public LightFlags Flags { get; set; }

		public float LightAngle { get; set; }

		public float FalloffAngle { get; set; }

		public Vector2 AreaAngle { get; set; }

		public FalloffType FalloffType { get; set; }

		public float Falloff { get; set; }

		public ColorHDR Color { get; set; }

		public float Range { get; set; }

		public float CharaShadowRange { get; set; }

		public float ShadowNear { get; set; }

		public float ShadowFar { get; set; }
	}

	public class EntityLightConverter(LightEntity lightEntity) {
		public bool IsLightValid => lightEntity.IsValid;

		public unsafe LightFile Save() {
			var lightFile = new LightFile();
			var sceneLightPtr = lightEntity.GetObject();
			RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
			if ((IntPtr)renderLight == IntPtr.Zero)
				return lightFile;
			lightFile.Transform = lightEntity.GetTransform();
			lightFile.LightType = renderLight->LightType;
			lightFile.Flags = renderLight->Flags;
			lightFile.LightAngle = renderLight->LightAngle;
			lightFile.FalloffAngle = renderLight->FalloffAngle;
			lightFile.AreaAngle = renderLight->AreaAngle;
			lightFile.FalloffType = renderLight->FalloffType;
			lightFile.Falloff = renderLight->Falloff;
			lightFile.Range = renderLight->Range;
			lightFile.Color = renderLight->Color;
			lightFile.CharaShadowRange = renderLight->CharaShadowRange;
			lightFile.ShadowNear = renderLight->ShadowNear;
			lightFile.ShadowFar = renderLight->ShadowFar;
			return lightFile;
		}

		public unsafe void Load(
			LightFile file,
			PoseTransforms transforms,
			bool doLighting,
			bool doColor,
			bool doShadows,
			ActorEntity? actor,
			bool history = false
		) {
			var sceneLightPtr = lightEntity.GetObject();
			RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
			if ((IntPtr)renderLight == IntPtr.Zero)
				return;
			var transform1 = lightEntity.GetTransform();
			if (transform1 != null && file.Transform != null) {
				var transform2 = (Transform)null;
				if (actor != null) {
					var transform3 = actor.Pose?.FindBoneByName("n_hara")?.GetTransform();
					var transform4 = actor.GetTransform();
					if (transform3 == null)
						transform3 = transform4;
					transform2 = transform3;
				}
				if (transforms.HasFlag(PoseTransforms.Position)) {
					if (history)
						transform1.Position = file.Transform.Position;
					else if (transform2 != null) {
						Vector3 vector3 = Vector3.Transform(file.Transform.Position, transform2.Rotation);
						transform1.Position = transform2.Position + vector3;
					}
				}
				if (transforms.HasFlag(PoseTransforms.Rotation))
					transform1.Rotation = !(transform2 == null | history) ? transform2.Rotation * file.Transform.Rotation : file.Transform.Rotation;
				if (transforms.HasFlag(PoseTransforms.Scale))
					transform1.Scale = file.Transform.Scale;
				lightEntity.SetTransform(transform1);
			}
			if (doLighting) {
				renderLight->LightType = file.LightType;
				renderLight->Flags = file.Flags;
				renderLight->LightAngle = file.LightAngle;
				renderLight->FalloffAngle = file.FalloffAngle;
				renderLight->AreaAngle = file.AreaAngle;
				renderLight->FalloffType = file.FalloffType;
				renderLight->Falloff = file.Falloff;
				renderLight->Range = file.Range;
			}
			if (doColor)
				renderLight->Color = file.Color;
			if (!doShadows)
				return;
			renderLight->Flags = file.Flags;
			renderLight->CharaShadowRange = file.CharaShadowRange;
			renderLight->ShadowNear = file.ShadowNear;
			renderLight->ShadowFar = file.ShadowFar;
		}
	}

	public class LightMemento(EntityLightConverter converter) : IMemento {
		public required PoseTransforms Transforms { get; init; }

		public required bool Lighting { get; init; }

		public required bool Color { get; init; }

		public required bool Shadows { get; init; }

		public required LightFile Initial { get; init; }

		public required LightFile Final { get; init; }

		public void Restore() => this.Apply(this.Initial);

		public void Apply() => this.Apply(this.Final);

		private void Apply(LightFile file) {
			if (!converter.IsLightValid)
				return;
			converter.Load(file, this.Transforms, this.Lighting, this.Color, this.Shadows, null, true);
		}
	}
}
