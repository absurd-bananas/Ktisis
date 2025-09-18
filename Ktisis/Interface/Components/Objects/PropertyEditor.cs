// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Objects.PropertyEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Editor.Properties;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Scene.Entities;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Interface.Components.Objects;

[Transient]
public class PropertyEditor
{
  private readonly DIBuilder _di;
  private readonly PropertyEditor.PropertyListBuilder _builder = new PropertyEditor.PropertyListBuilder();
  private readonly List<ObjectPropertyList> _editors = new List<ObjectPropertyList>();

  public PropertyEditor(DIBuilder di) => this._di = di;

  public void Prepare(IEditorContext ctx)
  {
    this.Create<ActorPropertyList>((object) ctx).Create<BasePropertyList>().Create<PosePropertyList>((object) ctx).Create<LightPropertyList>().Create<ImagePropertyList>((object) ctx).Create<WeaponPropertyList>();
  }

  private PropertyEditor Create<T>(params object[] parameters) where T : ObjectPropertyList
  {
    Ktisis.Ktisis.Log.Verbose("Creating property editor: " + typeof (T).Name, Array.Empty<object>());
    this._editors.Add((ObjectPropertyList) this._di.Create<T>(parameters));
    return this;
  }

  public void Draw(SceneEntity entity)
  {
    this._builder.Clear();
    foreach (ObjectPropertyList editor in this._editors)
      editor.Invoke((IPropertyListBuilder) this._builder, entity);
    foreach (PropertyEditor.PropertyListBuilder.PropertyHeader propertyHeader in (IEnumerable<PropertyEditor.PropertyListBuilder.PropertyHeader>) this._builder.Build())
    {
      if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(propertyHeader.Name), (ImGuiTreeNodeFlags) 0))
      {
        try
        {
          propertyHeader.Callback();
        }
        catch (Exception ex)
        {
          Ktisis.Ktisis.Log.Error($"Error on '{propertyHeader.Name}':\n{ex.Message}", Array.Empty<object>());
          Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Encountered a UI error!\nPlease submit a bug report."));
        }
        Dalamud.Bindings.ImGui.ImGui.Spacing();
      }
    }
  }

  private class PropertyListBuilder : IPropertyListBuilder
  {
    private readonly List<PropertyEditor.PropertyListBuilder.PropertyHeader> _headers = new List<PropertyEditor.PropertyListBuilder.PropertyHeader>();

    public void Clear() => this._headers.Clear();

    public void AddHeader(string name, Action callback, int priority = -2147483648 /*0x80000000*/)
    {
      this._headers.Add(new PropertyEditor.PropertyListBuilder.PropertyHeader()
      {
        Name = name,
        Callback = callback,
        Priority = priority == int.MinValue ? this._headers.Count : priority
      });
    }

    public IReadOnlyList<PropertyEditor.PropertyListBuilder.PropertyHeader> Build()
    {
      this._headers.Sort((Comparison<PropertyEditor.PropertyListBuilder.PropertyHeader>) ((a, b) => a.Priority - b.Priority));
      return (IReadOnlyList<PropertyEditor.PropertyListBuilder.PropertyHeader>) this._headers.AsReadOnly();
    }

    public class PropertyHeader
    {
      public required string Name;
      public required Action Callback;
      public required int Priority;
    }
  }
}
