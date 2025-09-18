// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.ResourceUtil
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.IO;
using System.Reflection;

namespace Ktisis.Common.Utility;

public static class ResourceUtil {
	public static Stream GetManifestResource(string path) {
		var executingAssembly = Assembly.GetExecutingAssembly();
		path = $"{executingAssembly.GetName().Name}.{path}";
		return executingAssembly.GetManifestResourceStream(path) ?? throw new FileNotFoundException(path);
	}
}
