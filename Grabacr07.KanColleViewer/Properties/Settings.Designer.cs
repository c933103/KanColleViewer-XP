﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.0
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grabacr07.KanColleViewer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.dmm.com/netgame_s/kancolle/")]
        public global::System.Uri KanColleUrl {
            get {
                return ((global::System.Uri)(this["KanColleUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("333")]
        public double UIContentHight {
            get {
                return ((double)(this["UIContentHight"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int FeatureBrowserEmulation {
            get {
                return ((int)(this["FeatureBrowserEmulation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<style type=\"text/css\">html { touch-action: none; }</style>")]
        public string TagNoTouchAction {
            get {
                return ((string)(this["TagNoTouchAction"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/")]
        public global::System.Uri KanColleGamePage {
            get {
                return ((global::System.Uri)(this["KanColleGamePage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<script type=""text/javascript"">(function(quality, wmode) {{
  var origEmbedFlash = window.gadgets.flash.embedFlash;
  gadgets.flash.embedFlash = function(url, container, version, params) {{
    if(typeof(params) == ""undefined"") params = {{}};
    if(quality.toLowerCase() != ""default"") params.quality = quality;
    if(wmode.toLowerCase() != ""default"") params.wmode = wmode;
    origEmbedFlash(url, container, version, params);
  }};
}})(""{0}"", ""{1}"");</script>")]
        public string TagQualityShim {
            get {
                return ((string)(this["TagQualityShim"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<style type=\"text/css\">\nbody {\n  margin:0;\n  overflow:hidden;\n}\n\n#ntg-recommend {" +
            "\n  display: none;\n}\n\n#game_frame {\n  position:fixed;\n  left:50%;\n  top:-16px;\n  " +
            "margin-left:-450px;\n  z-index:1;\n}\n</style>")]
        public string TagOverrideStylesheet {
            get {
                return ((string)(this["TagOverrideStylesheet"]));
            }
        }
    }
}
