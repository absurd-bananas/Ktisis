// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.SkeletonNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton;

public abstract class SkeletonNode(ISceneManager scene) : SceneEntity(scene) {
	public EntityPose Pose { get; protected init; }

	public int SortPriority { get; set; }

	public void OrderByPriority() {
		this.GetChildren().Sort((_a, _b) => {
			int num;
			if (_a is SkeletonGroup) {
				if (_b is SkeletonGroup) {
					skeletonNode3 = (SkeletonNode)_a;
					skeletonNode4 = (SkeletonNode)_b;
				} else {
					num = -1;
					goto label_9;
				}
			} else if (!(_b is SkeletonGroup)) {
				if (!(_a is SkeletonNode skeletonNode3) || !(_b is SkeletonNode skeletonNode4)) {
					num = 0;
					goto label_9;
				}
			} else {
				num = 1;
				goto label_9;
			}
			num = skeletonNode3.SortPriority - skeletonNode4.SortPriority;
			label_9:
			return num;
		});
	}
}
