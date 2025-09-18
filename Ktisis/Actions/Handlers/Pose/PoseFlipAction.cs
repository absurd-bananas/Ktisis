// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Pose.PoseFlipAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;

namespace Ktisis.Actions.Handlers.Pose;

[Action("Pose_Flip")]
public class PoseFlipAction(IPluginContext ctx) : KeyAction(ctx) {
	public override KeybindInfo BindInfo { get; } = new KeybindInfo {
		Trigger = KeybindTrigger.OnRelease,
		Default = new ActionKeybind {
			Enabled = true,
			Combo = new KeyCombo((VirtualKey)0)
		}
	};

	public override bool CanInvoke() => this.Context.Editor != null && this.Context.Editor?.Posing.GetActorFromTarget(this.Context.Editor?.Transform.Target) != null;

	public override bool Invoke() {
		if (!this.CanInvoke())
			return false;
		this.Context.Editor.Cameras.Current?.SetOffsetPositionToTarget(this.Context.Editor, false);
		var actorFromTarget = this.Context.Editor.Posing.GetActorFromTarget(this.Context.Editor.Transform.Target);
		if (actorFromTarget != null)
			this.Context.Editor.Posing.ApplyPoseFlip(actorFromTarget);
		return true;
	}
}
