// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Objects.PropertyEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Editor.Properties;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Scene.Entities;

namespace Ktisis.Interface.Components.Objects;

[Transient]
public class PropertyEditor {
	private readonly PropertyListBuilder _builder = new PropertyListBuilder();
	private readonly DIBuilder _di;
	private readonly List<ObjectPropertyList> _editors = new List<ObjectPropertyList>();

	public PropertyEditor(DIBuilder di) {
		this._di = di;
	}

	public void Prepare(IEditorContext ctx) {
		this.Create<ActorPropertyList>(ctx).Create<BasePropertyList>().Create<PosePropertyList>(ctx).Create<LightPropertyList>().Create<ImagePropertyList>(ctx).Create<WeaponPropertyList>();
	}

	private PropertyEditor Create<T>(params object[] parameters) where T : ObjectPropertyList {
		Ktisis.Ktisis.Log.Verbose("Creating property editor: " + typeof(T).Name, Array.Empty<object>());
		this._editors.Add(this._di.Create<T>(parameters));
		return this;
	}

	public void Draw(SceneEntity entity) {
		this._builder.Clear();
		foreach (var editor in this._editors)
			editor.Invoke(this._builder, entity);
		foreach (var propertyHeader in this._builder.Build()) {
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(propertyHeader.Name), (ImGuiTreeNodeFlags)0)) {
				try {
					propertyHeader.Callback();
				} catch (Exception ex) {
					Ktisis.Ktisis.Log.Error($"Error on '{propertyHeader.Name}':\n{ex.Message}", Array.Empty<object>());
					Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Encountered a UI error!\nPlease submit a bug report."));
				}
				Dalamud.Bindings.ImGui.ImGui.Spacing();
			}
		}
	}

	private class PropertyListBuilder : IPropertyListBuilder {
		private readonly List<PropertyHeader> _headers = new List<PropertyHeader>();

		public void AddHeader(string name, Action callback, int priority = -2147483648 /*0x80000000*/) {
			this._headers.Add(new PropertyHeader {
				Name = name,
				Callback = callback,
				Priority = priority == int.MinValue ? this._headers.Count : priority
			});
		}

		public void Clear() => this._headers.Clear();

		public IReadOnlyList<PropertyHeader> Build() {
			this._headers.Sort((a, b) => a.Priority - b.Priority);
			return this._headers.AsReadOnly();
		}

		public class PropertyHeader {
			public required Action Callback;
			public required string Name;
			public required int Priority;
		}
	}
}
