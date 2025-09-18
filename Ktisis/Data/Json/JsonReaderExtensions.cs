// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.JsonReaderExtensions
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Data.Json;

public static class JsonReaderExtensions {
	public static void SkipIt(ref this BlockBufferJsonReader reader) {
		if (((Utf8JsonReader) ref reader.Reader ).TrySkip())
		return;
		if (((Utf8JsonReader) ref reader.Reader ).TokenType == 5)
		reader.Read();
		if (((Utf8JsonReader) ref reader.Reader ).TokenType != 1 && ((Utf8JsonReader) ref reader.Reader).TokenType != 3)
		return;
		int currentDepth = ((Utf8JsonReader) ref reader.Reader ).CurrentDepth;
		do {
			reader.Read();
		} while (((Utf8JsonReader) ref reader.Reader ).CurrentDepth > currentDepth);
	}
}
