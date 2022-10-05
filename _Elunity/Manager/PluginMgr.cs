

/*
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using Mono.Cecil;

namespace Elang
{


    /// <summary>
    /// <br> Manages (native) plugins. </br>
    /// <br> Unlike PInvoke, this allows native libraries to be loaded and unloaded in runtime.</br>
    /// <br> This makes it easier to edit native libraries without having to restart the entire Unity Editor upon every change. </br>
    /// <br></br>
    /// <br> The default is to load all plugins via Plugin DefaultAssets (dll asset) then to iterate all types in target assemblies to find matching Plugin attributes. </br>
    /// <br> May implement load and unload </br>
    /// <br></br>
    /// <br> The code is just a little disorganized at the moment because it only supports loading Windows dll plugins. </br>
    /// <br> I have no idea how to modify it for other platforms. Will get around to it once the need is there. </br>
    /// </summary>
    public class PluginMgr : Singleton<PluginMgr> //, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DefaultAsset> _pluginAssets;
        [SerializeField]
        List<UnityEditorInternal.AssemblyDefinitionAsset> _targetAssemblyDefinitions;

        Dictionary<string, IntPtr> _plugins = new();

        bool _dllLoaded = false;

        protected override void Init() {
            gameObject.name = "[ Plugin Manager ]";
            LoadAll(); 
        }
        void OnDestroy() { UnloadAll(); }

        void LoadAll() {
            foreach (var obj in _pluginAssets) {
                var path = AssetDatabase.GetAssetPath(obj);
                var ext = Path.GetExtension(path);

#if UNITY_STANDALONE_WIN
                if (ext == ".dll") {
                    IntPtr lib = WindowsSystemKernal.LoadLibrary(path);
                    _plugins.Add(obj.name + ext, lib);
                    _dllLoaded = true;
                }
#endif
            }

            SyncAllLibraries();
        }
        void SyncAllLibraries() {
            if (_dllLoaded) {
                foreach (var obj in _targetAssemblyDefinitions) {
                    try {
                        SyncLibrary(Assembly.Load(obj.name));
                    } catch {
                        Debug.LogError("Assembly " + obj.name + " does not exist.");
                    }
                }
            }
        }
        // AUTHOR
        //   Forrest Smith
        void SyncLibrary(Assembly assembly) {
            // Loop over all types
            foreach (var type in assembly.GetTypes()) {
                // Get custom attributes for type
                var typeAttributes = type.GetCustomAttributes(typeof(Plugin), true);
                if (typeAttributes.Length > 0) {
                    Debug.Assert(typeAttributes.Length == 1); // should not be possible

                    var typeAttribute = typeAttributes[0] as Plugin;

                    // Remove all function ptr reference for plugins that no longer exist
                    var pluginName = typeAttribute.pluginName;
                    var ext = Path.GetExtension(pluginName);

#if UNITY_STANDALONE_WIN
                    if (ext == ".dll") {
                        SyncDLL(type, pluginName);
                    }
#endif
                }
            }
        }

#if UNITY_STANDALONE_WIN
        void SyncDLL(Type type, string pluginName) {
            var pluginHandle = IntPtr.Zero;
            if (!_plugins.TryGetValue(pluginName, out pluginHandle)) {
                throw new Exception(string.Format("Plugin [{0}] cannot be loaded", pluginName));
            }

            // Loop over fields in type
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                // Get custom attributes for field
                var fieldAttributes = field.GetCustomAttributes(typeof(PluginFunction), true);
                if (fieldAttributes.Length > 0) {
                    Debug.Assert(fieldAttributes.Length == 1); // should not be possible

                    // Get PluginFunctionAttr attribute
                    var fieldAttribute = fieldAttributes[0] as PluginFunction;
                    var functionName = fieldAttribute.functionName;

                    // Get function pointer
                    var fnPtr = WindowsSystemKernal.GetProcAddress(pluginHandle, functionName);
                    if (fnPtr == IntPtr.Zero) {
                        Debug.LogError(string.Format("Failed to find function [{0}] in plugin [{1}]. Err: [{2}]", functionName, pluginName, WindowsSystemKernal.GetLastError()));
                        continue;
                    }

                    // Get delegate pointer
                    var fnDelegate = Marshal.GetDelegateForFunctionPointer(fnPtr, field.FieldType);

                    // Set static field value
                    field.SetValue(null, fnDelegate);
                }
            }
        }
#endif

        void UnloadAll() {
            UnsyncAllLibraries();

            if (_plugins != null && _plugins.Count > 0) {
                foreach (var plugin in _plugins) {
                    var ext = Path.GetExtension(plugin.Key);

#if UNITY_STANDALONE_WIN
                    if (ext == ".dll") {
                        try {
                            WindowsSystemKernal.FreeLibrary(plugin.Value);
                        } catch {
                            Debug.LogError(string.Format("Error unloading dll plugin [{0}].", plugin.Key));
                        }
                    }
#endif
                }
                _plugins.Clear();
            }
        }

        void UnsyncAllLibraries() {
            if (_dllLoaded) {
                foreach (var obj in _targetAssemblyDefinitions) {
                    try {
                        UnsyncLibrary(Assembly.Load(obj.name));
                    } catch {
                        Debug.LogError("Assembly " + obj.name + " does not exist.");
                    }
                }
            }
        }

        void UnsyncLibrary(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                var typeAttributes = type.GetCustomAttributes(typeof(Plugin), true);
                if (typeAttributes.Length > 0) {
                    Debug.Assert(typeAttributes.Length == 1);

                    var typeAttribute = typeAttributes[0] as Plugin;
                    var pluginName = typeAttribute.pluginName;

                    var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var field in fields) {
                        var fieldAttributes = field.GetCustomAttributes(typeof(PluginFunction), true);
                        if (fieldAttributes.Length > 0) {
                            Debug.Assert(fieldAttributes.Length == 1);
                            field.SetValue(null, null);
                        }
                    }
                }
            }
        }

        // It is *strongly* recommended to set Editor->Preferences->Script Changes While Playing = Recompile After Finished Playing
        // Properly support reload of native assemblies requires extra work.
        // However the following code will re-fixup delegates.
        // More importantly, it prevents a dangling DLL which results in a mandatory Editor reboot
        //bool _reloadAfterDeserialize = false;
        //void ISerializationCallbackReceiver.OnBeforeSerialize() {
        //    UnloadAll();
        //    _reloadAfterDeserialize = true;
        //}

        //void ISerializationCallbackReceiver.OnAfterDeserialize() {
        //    if (_reloadAfterDeserialize) {
        //        LoadAll();
        //        _reloadAfterDeserialize = false;
        //    }
        //}
    }

#if UNITY_STANDALONE_WIN
    static class WindowsSystemKernal
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static public extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32")]
        static public extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        static public extern uint GetLastError();
    }
#endif

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Plugin : Attribute
    {
        public string pluginName { get; private set; }

        public Plugin(string pluginName) {
            this.pluginName = pluginName;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PluginFunction : Attribute
    {
        public string functionName { get; private set; }

        public PluginFunction(string functionName) {
            this.functionName = functionName;
        }
    }
}*/