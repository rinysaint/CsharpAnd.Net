﻿#pragma checksum "..\..\TimeSetting.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "21FEF82428AFCCD944B9947FE23CE7056A0A2EAC"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using SkyWellAGaugeApp;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SkyWellAGaugeApp {
    
    
    /// <summary>
    /// TimeSetting
    /// </summary>
    public partial class TimeSetting : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label_Title;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_hour_up;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_minute_up;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox_hour;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox_minute;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_hour_down;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_minute_down;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\TimeSetting.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock_Time;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SkyWellAGaugeApp;component/timesetting.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TimeSetting.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.label_Title = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.button_hour_up = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\TimeSetting.xaml"
            this.button_hour_up.Click += new System.Windows.RoutedEventHandler(this.button_hour_up_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.button_minute_up = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\TimeSetting.xaml"
            this.button_minute_up.Click += new System.Windows.RoutedEventHandler(this.button_minute_up_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.textbox_hour = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\TimeSetting.xaml"
            this.textbox_hour.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.numtextboxchanged);
            
            #line default
            #line hidden
            
            #line 25 "..\..\TimeSetting.xaml"
            this.textbox_hour.SelectionChanged += new System.Windows.RoutedEventHandler(this.textbox_hour_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.textbox_minute = ((System.Windows.Controls.TextBox)(target));
            
            #line 27 "..\..\TimeSetting.xaml"
            this.textbox_minute.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.numtextboxchanged);
            
            #line default
            #line hidden
            
            #line 27 "..\..\TimeSetting.xaml"
            this.textbox_minute.SelectionChanged += new System.Windows.RoutedEventHandler(this.textbox_hour_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.button_hour_down = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\TimeSetting.xaml"
            this.button_hour_down.Click += new System.Windows.RoutedEventHandler(this.button_hour_down_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.button_minute_down = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\TimeSetting.xaml"
            this.button_minute_down.Click += new System.Windows.RoutedEventHandler(this.button_minute_down_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.textBlock_Time = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

