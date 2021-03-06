﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pixellation.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Pixellation.Properties.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string CaptionError {
            get {
                return ResourceManager.GetString("CaptionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Information.
        /// </summary>
        internal static string CaptionInformation {
            get {
                return ResourceManager.GetString("CaptionInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning.
        /// </summary>
        internal static string CaptionWarning {
            get {
                return ResourceManager.GetString("CaptionWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must fill all input fields!.
        /// </summary>
        internal static string ErrorEmptyInput {
            get {
                return ResourceManager.GetString("ErrorEmptyInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value {0} is not a number!.
        /// </summary>
        internal static string ErrorIsNotANumber {
            get {
                return ResourceManager.GetString("ErrorIsNotANumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exporting to image failed! Error: {0}.
        /// </summary>
        internal static string ErrorWhileExportingImage {
            get {
                return ResourceManager.GetString("ErrorWhileExportingImage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading image failed! Error: {0}.
        /// </summary>
        internal static string ErrorWhileLoadingImage {
            get {
                return ResourceManager.GetString("ErrorWhileLoadingImage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading project failed! Error: {0}.
        /// </summary>
        internal static string ErrorWhileLoadingProject {
            get {
                return ResourceManager.GetString("ErrorWhileLoadingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saving project failed! Error: {0}.
        /// </summary>
        internal static string ErrorWhileSavingProject {
            get {
                return ResourceManager.GetString("ErrorWhileSavingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have unsaved changes!.
        /// </summary>
        internal static string WarningUnsavedChanges {
            get {
                return ResourceManager.GetString("WarningUnsavedChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Would you like to save your project before proceeding with the operation?.
        /// </summary>
        internal static string WouldYouLikeToSave {
            get {
                return ResourceManager.GetString("WouldYouLikeToSave", resourceCulture);
            }
        }
    }
}
