// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.PosingManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Actions.Types;
using Ktisis.Common.Extensions;
using Ktisis.Data.Config;
using Ktisis.Data.Files;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.AutoSave;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Ik;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Editor.Posing;

public class PosingManager : IPosingManager, IDisposable
{
  private readonly IEditorContext _context;
  private readonly HookScope _scope;
  private readonly IFramework _framework;
  private readonly PoseAutoSave AutoSave;
  private readonly Dictionary<ushort, PosingManager.PoseState> _savedPoses = new Dictionary<ushort, PosingManager.PoseState>();

  public bool IsValid => this._context.IsValid;

  public IAttachManager Attachments { get; }

  public PosingManager(
    IEditorContext context,
    HookScope scope,
    IFramework framework,
    IAttachManager attach,
    PoseAutoSave autoSave)
  {
    this._context = context;
    this._scope = scope;
    this._framework = framework;
    this.Attachments = attach;
    this.AutoSave = autoSave;
  }

  public bool IsSolvingIk { get; set; }

  private PosingModule? PoseModule { get; set; }

  private IkModule? IkModule { get; set; }

  public void Initialize()
  {
    try
    {
      this.PoseModule = this._scope.Create<PosingModule>((object) this);
      this.PoseModule.Initialize();
      this.IkModule = this._scope.Create<IkModule>((object) this);
      this.IkModule.Initialize();
      this.AutoSave.Initialize(this._context.Config);
      this.Subscribe();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to initialize posing manager:\n{ex}", Array.Empty<object>());
    }
  }

  private void Subscribe()
  {
    this.PoseModule.OnSkeletonInit += new SkeletonInitHandler(this.OnSkeletonInit);
    this.PoseModule.OnDisconnect += new Action(this.OnDisconnect);
    this._context.Characters.OnDisableDraw += new DisableDrawHandler(this.OnDisableDraw);
    this._context.Plugin.Config.OnSaved += new OnConfigSaved(this.AutoSave.Configure);
  }

  private unsafe void OnSkeletonInit(IGameObject gameObject, FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton, ushort partialId)
  {
    this.RestorePoseFor(gameObject.ObjectIndex, skeleton, partialId);
  }

  private void OnDisconnect()
  {
    if (!this._context.Config.AutoSave.OnDisconnect)
      return;
    Ktisis.Ktisis.Log.Verbose("Disconnected, triggering pose save.", Array.Empty<object>());
    this.AutoSave.Save();
  }

  private unsafe void OnDisableDraw(IGameObject gameObject, DrawObject* drawObject)
  {
    Ktisis.Ktisis.Log.Verbose($"Preserving state for {gameObject.Name} ({gameObject.ObjectIndex})", Array.Empty<object>());
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = gameObject.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    this.Attachments.Invalidate(skeleton);
    this.PreservePoseFor(gameObject.ObjectIndex, skeleton);
  }

  public bool IsEnabled
  {
    get
    {
      PosingModule poseModule = this.PoseModule;
      return poseModule != null && poseModule.IsEnabled;
    }
  }

  public void SetEnabled(bool enable)
  {
    if (enable && !this.IsValid)
      return;
    this.PoseModule?.SetEnabled(enable);
  }

  public IIkController CreateIkController() => this.IkModule.CreateController();

  private unsafe void PreservePoseFor(ushort objectIndex, FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton)
  {
    PoseContainer poseContainer = new PoseContainer();
    poseContainer.Store(skeleton);
    this._savedPoses[objectIndex] = new PosingManager.PoseState()
    {
      Pose = poseContainer
    };
  }

  private unsafe void RestorePoseFor(ushort objectIndex, FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton, ushort partialId)
  {
    PosingManager.PoseState poseState;
    if (!this._savedPoses.TryGetValue(objectIndex, out poseState))
      return;
    poseState.Pose.ApplyToPartial(skeleton, (int) partialId, PoseTransforms.Rotation | PoseTransforms.PositionRoot);
  }

  public Task ApplyReferencePose(EntityPose pose)
  {
    return this._framework.RunOnFrameworkThread((Action) (() =>
    {
      EntityPoseConverter converter = new EntityPoseConverter(pose);
      PoseContainer poseContainer1 = converter.Save();
      converter.LoadReferencePose();
      PoseContainer poseContainer2 = converter.Save();
      this._context.Actions.History.Add((IMemento) new PoseMemento(converter)
      {
        Transforms = (PoseTransforms.Rotation | PoseTransforms.Position),
        Bones = (List<PartialBoneInfo>) null,
        Initial = poseContainer1,
        Final = poseContainer2
      });
    }));
  }

  public Task ApplyPoseFile(
    EntityPose pose,
    PoseFile file,
    PoseTransforms transforms = PoseTransforms.Rotation,
    bool selectedBones = false,
    bool anchorGroups = false,
    bool selectedBonesIncludeChildren = false,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    return this._framework.RunOnFrameworkThread((Action) (() =>
    {
      if (file.Bones == null)
        return;
      EntityPoseConverter converter = new EntityPoseConverter(pose);
      PoseContainer pose1 = converter.Save();
      List<IMemento> mementos = new List<IMemento>();
      PoseContainer poseContainer = new PoseContainer();
      switch (boneTypeInclusion)
      {
        case BoneTypeInclusion.Both:
          selectedBones = false;
          if (file.Bones.ContainsKey("j_f_face"))
          {
            transforms = transforms.HasFlag((Enum) PoseTransforms.Scale) ? PoseTransforms.Rotation | PoseTransforms.Position | PoseTransforms.Scale : PoseTransforms.Rotation | PoseTransforms.Position;
            converter.Load(file.Bones, transforms, BoneTypeInclusion.Face);
            converter.Load(file.Bones, transforms, BoneTypeInclusion.Body);
            poseContainer = converter.Save();
            break;
          }
          converter.Load(file.Bones, transforms, BoneTypeInclusion.Body);
          poseContainer = file.Bones;
          break;
        case BoneTypeInclusion.Face:
          transforms = transforms.HasFlag((Enum) PoseTransforms.Scale) ? PoseTransforms.Rotation | PoseTransforms.Position | PoseTransforms.Scale : PoseTransforms.Rotation | PoseTransforms.Position;
          if (selectedBones)
          {
            converter.Load(file.Bones, transforms, BoneTypeInclusion.Face);
            converter.LoadUnselectedBones(pose1, transforms, selectedBonesIncludeChildren, BoneTypeInclusion.Face);
          }
          else
            converter.Load(file.Bones, transforms, BoneTypeInclusion.Face);
          converter.Load(pose1, transforms, BoneTypeInclusion.Body);
          poseContainer = converter.Save();
          break;
        default:
          if (selectedBones)
          {
            converter.LoadSelectedBones(file.Bones, transforms, selectedBonesIncludeChildren, BoneTypeInclusion.Body);
            break;
          }
          converter.Load(file.Bones, transforms, BoneTypeInclusion.Body);
          poseContainer = converter.Save();
          break;
      }
      mementos.Add((IMemento) new PoseMemento(converter)
      {
        Transforms = transforms,
        Bones = (selectedBones ? converter.GetSelectedBones(selectedBonesIncludeChildren).ToList<PartialBoneInfo>() : (List<PartialBoneInfo>) null),
        Initial = (selectedBones ? converter.FilterSelectedBones(pose1, selectedBonesIncludeChildren) : pose1),
        Final = (selectedBones ? converter.FilterSelectedBones(file.Bones, selectedBonesIncludeChildren) : poseContainer)
      });
      if (selectedBones & anchorGroups && transforms.HasFlag((Enum) PoseTransforms.Position))
      {
        List<PartialBoneInfo> list = converter.GetSelectedBones(false).ToList<PartialBoneInfo>();
        converter.LoadBones(pose1, (IEnumerable<PartialBoneInfo>) list, PoseTransforms.Position);
        mementos.Add((IMemento) new PoseMemento(converter)
        {
          Transforms = PoseTransforms.Position,
          Bones = list,
          Initial = converter.FilterSelectedBones(file.Bones, false),
          Final = converter.FilterSelectedBones(pose1, false)
        });
      }
      this._context.Actions.History.Add((IMemento) new MultipleMemento((IReadOnlyList<IMemento>) mementos));
    }));
  }

  public Task<PoseFile> SavePoseFile(EntityPose pose)
  {
    return this._framework.RunOnFrameworkThread<PoseFile>((Func<PoseFile>) (() => new EntityPoseConverter(pose).SaveFile()));
  }

  public ActorEntity? GetActorFromTarget(ITransformTarget? target)
  {
    if (target != null && target.Primary != null)
    {
      if (target.Primary.Type == EntityType.Actor)
        return target.Primary as ActorEntity;
      if (target.Primary is SkeletonNode primary)
      {
        EntityPose pose = primary.Pose;
        return this._context.Scene.Children.FirstOrDefault<SceneEntity>((Func<SceneEntity, bool>) (x => x is ActorEntity actorEntity && actorEntity.Pose == pose)) as ActorEntity;
      }
    }
    return (ActorEntity) null;
  }

  public Task ApplyPoseFlip(ActorEntity? target)
  {
    return this._framework.RunOnFrameworkThread((Action) (() =>
    {
      if (target == null || target.Pose == null)
        return;
      EntityPoseConverter converter = new EntityPoseConverter(target.Pose);
      PoseContainer poseContainer1 = converter.Save();
      PoseContainer poseContainer2 = converter.FlipBones(this._context.Config.Editor.FlipYawCorrection, this._context.Config.Editor.FlipRotationCorrection);
      this._context.Actions.History.Add((IMemento) new PoseMemento(converter)
      {
        Transforms = PoseTransforms.Rotation,
        Bones = (List<PartialBoneInfo>) null,
        Initial = poseContainer1,
        Final = poseContainer2
      });
    }));
  }

  public void Dispose()
  {
    try
    {
      this.PoseModule?.Dispose();
      this.PoseModule = (PosingModule) null;
      this.IkModule?.Dispose();
      this.IkModule = (IkModule) null;
      this.Attachments.Dispose();
      this._context.Plugin.Config.OnSaved -= new OnConfigSaved(this.AutoSave.Configure);
      this.AutoSave.Dispose();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to dispose posing manager:\n{ex}", Array.Empty<object>());
    }
    GC.SuppressFinalize((object) this);
  }

  private record PoseState()
  {
    public required PoseContainer Pose;

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Pose = ");
      builder.Append((object) this.Pose);
      return true;
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<PoseContainer>.Default.GetHashCode(this.Pose);
    }

    [CompilerGenerated]
    public virtual bool Equals(PosingManager.PoseState? other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<PoseContainer>.Default.Equals(this.Pose, other.Pose);
    }

    [CompilerGenerated]
    [SetsRequiredMembers]
    protected PoseState(PosingManager.PoseState original) => this.Pose = original.Pose;
  }
}
