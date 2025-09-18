// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.DllResolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using Ktisis.Core.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

#nullable enable
namespace Ktisis.Interop;

[Singleton]
public class DllResolver : IDisposable
{
  private readonly IDalamudPluginInterface _dpi;
  private readonly List<IntPtr> Handles = new List<IntPtr>();
  private AssemblyLoadContext? Context;

  public DllResolver(IDalamudPluginInterface dpi) => this._dpi = dpi;

  public void Create()
  {
    Ktisis.Ktisis.Log.Debug("Creating DLL resolver for unmanaged libraries", Array.Empty<object>());
    this.Context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
    if (this.Context == null)
      return;
    this.Context.ResolvingUnmanagedDll += new Func<Assembly, string, IntPtr>(this.ResolveUnmanaged);
  }

  private IntPtr ResolveUnmanaged(Assembly assembly, string library)
  {
    string directoryName = Path.GetDirectoryName(((FileSystemInfo) this._dpi.AssemblyLocation).FullName);
    if (directoryName == null)
    {
      Ktisis.Ktisis.Log.Warning("Failed to resolve location for native assembly!", Array.Empty<object>());
      return IntPtr.Zero;
    }
    string str = Path.Combine(directoryName, library);
    Ktisis.Ktisis.Log.Debug("Resolving native assembly path: " + str, Array.Empty<object>());
    IntPtr num;
    if (NativeLibrary.TryLoad(str, ref num) && num != IntPtr.Zero)
    {
      this.Handles.Add(num);
      Ktisis.Ktisis.Log.Debug($"Success, resolved library handle: {num:X}", Array.Empty<object>());
    }
    else
      Ktisis.Ktisis.Log.Warning("Failed to resolve native assembly path: " + str, Array.Empty<object>());
    return num;
  }

  public void Dispose()
  {
    Ktisis.Ktisis.Log.Debug("Disposing DLL resolver for unmanaged libraries", Array.Empty<object>());
    if (this.Context != null)
      this.Context.ResolvingUnmanagedDll -= new Func<Assembly, string, IntPtr>(this.ResolveUnmanaged);
    this.Context = (AssemblyLoadContext) null;
    this.Handles.ForEach(new Action<IntPtr>(this.FreeHandle));
    this.Handles.Clear();
  }

  private void FreeHandle(IntPtr handle)
  {
    Ktisis.Ktisis.Log.Debug($"Freeing library handle: {handle:X}", Array.Empty<object>());
    NativeLibrary.Free(handle);
  }
}
