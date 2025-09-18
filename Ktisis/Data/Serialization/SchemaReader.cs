// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Serialization.SchemaReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Common.Utility;
using Ktisis.Data.Config.Sections;

namespace Ktisis.Data.Serialization;

public static class SchemaReader {
	private const string CategorySchemaPath = "Data.Schema.Categories.xml";
	private const string ViewSchemaPath = "Data.Schema.Views.xml";

	public static CategoryConfig ReadCategories() => CategoryReader.ReadStream(ResourceUtil.GetManifestResource("Data.Schema.Categories.xml"));

	public static PoseViewSchema ReadPoseView() => PoseViewReader.ReadStream(ResourceUtil.GetManifestResource("Data.Schema.Views.xml"));
}
