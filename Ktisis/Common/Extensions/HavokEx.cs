// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.HavokEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.Havok.Common.Base.Container.Array;

#nullable enable
namespace Ktisis.Common.Extensions;

public static class HavokEx
{
  public static T[] Copy<T>(this hkArray<T> array) where T : unmanaged
  {
    int length = array.Length;
    T[] objArray = new T[length];
    for (int index = 0; index < length; ++index)
      objArray[index] = array[index];
    return objArray;
  }

  public static unsafe void Initialize<T>(hkArray<T>* array, T* data = null, int length = 0) where T : unmanaged
  {
    array->Data = data;
    array->Length = length;
    array->CapacityAndFlags = int.MinValue;
  }
}
