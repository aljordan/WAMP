﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WhisperingAudioMusicPlayer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SelectedLibraryName {
            get {
                return ((string)(this["SelectedLibraryName"]));
            }
            set {
                this["SelectedLibraryName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::WhisperingAudioMusicEngine.AudioOutput SelectedOutput {
            get {
                return ((global::WhisperingAudioMusicEngine.AudioOutput)(this["SelectedOutput"]));
            }
            set {
                this["SelectedOutput"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::WhisperingAudioMusicLibrary.Track CurrentTrack {
            get {
                return ((global::WhisperingAudioMusicLibrary.Track)(this["CurrentTrack"]));
            }
            set {
                this["CurrentTrack"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsRandom {
            get {
                return ((bool)(this["IsRandom"]));
            }
            set {
                this["IsRandom"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsRepeating {
            get {
                return ((bool)(this["IsRepeating"]));
            }
            set {
                this["IsRepeating"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int PlaybackTimeout {
            get {
                return ((int)(this["PlaybackTimeout"]));
            }
            set {
                this["PlaybackTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsVolumeEnabled {
            get {
                return ((bool)(this["IsVolumeEnabled"]));
            }
            set {
                this["IsVolumeEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("601")]
        public double CurrentVolume {
            get {
                return ((double)(this["CurrentVolume"]));
            }
            set {
                this["CurrentVolume"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SendPlaylistChangesToPlayer {
            get {
                return ((bool)(this["SendPlaylistChangesToPlayer"]));
            }
            set {
                this["SendPlaylistChangesToPlayer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsMemoryPlayEnabled {
            get {
                return ((bool)(this["IsMemoryPlayEnabled"]));
            }
            set {
                this["IsMemoryPlayEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsAcourateVolumeEnabled {
            get {
                return ((bool)(this["IsAcourateVolumeEnabled"]));
            }
            set {
                this["IsAcourateVolumeEnabled"] = value;
            }
        }
    }
}
