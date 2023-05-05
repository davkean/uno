﻿// <autogenerated />
#pragma warning disable CS0114
#pragma warning disable CS0108
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Uno.UI;
using Uno.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Text;
using Uno.Extensions;
using Uno;
using Uno.UI.Helpers.Xaml;
using MyProject;

#if __ANDROID__
using _View = Android.Views.View;
#elif __IOS__
using _View = UIKit.UIView;
#elif __MACOS__
using _View = AppKit.NSView;
#elif UNO_REFERENCE_API || IS_UNIT_TESTS
using _View = Windows.UI.Xaml.UIElement;
#endif

namespace Uno.UI.Tests.Windows_UI_Xaml_Data.BindingTests.Controls
{
	partial class Binding_ElementName_In_Template : global::Windows.UI.Xaml.Controls.Page
	{
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
		private const string __baseUri_prefix_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca = "ms-appx:///TestProject/";
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
		private const string __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca = "ms-appx:///TestProject/";
				global::Windows.UI.Xaml.NameScope __nameScope = new global::Windows.UI.Xaml.NameScope();
		private void InitializeComponent()
		{
			InitializeComponent_B5A1302D();
		}
		private void InitializeComponent_B5A1302D()
		{
			NameScope.SetNameScope(this, __nameScope);
			var __that = this;
			base.IsParsing = true;
			// Source 0\Binding_ElementName_In_Template.xaml (Line 1:2)
			base.Content = 
			new global::Windows.UI.Xaml.Controls.Grid
			{
				IsParsing = true,
				// Source 0\Binding_ElementName_In_Template.xaml (Line 10:3)
				Children = 
				{
					new global::Windows.UI.Xaml.Controls.ContentControl
					{
						IsParsing = true,
						Name = "topLevel",
						Tag = @"42",
						ContentTemplate = 						new global::Windows.UI.Xaml.DataTemplate(this , __owner => 						new _Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_UnoUITestsWindows_UI_Xaml_DataBindingTestsControlsBinding_ElementName_In_TemplateSC0().Build(__owner)
						)						,
						// Source 0\Binding_ElementName_In_Template.xaml (Line 11:4)
					}
					.Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply((Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237ecaXamlApplyExtensions.XamlApplyHandler0)(c0 => 
					{
						__nameScope.RegisterName("topLevel", c0);
						__that.topLevel = c0;
						// FieldModifier public
						global::Uno.UI.FrameworkElementHelper.SetBaseUri(c0, __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca);
						c0.CreationComplete();
					}
					))
					,
				}
			}
			.Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply((Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237ecaXamlApplyExtensions.XamlApplyHandler1)(c1 => 
			{
				global::Uno.UI.FrameworkElementHelper.SetBaseUri(c1, __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca);
				c1.CreationComplete();
			}
			))
			;
			
			this
			.GenericApply(((c2) => 
			{
				// Source 0\Binding_ElementName_In_Template.xaml (Line 1:2)
				
				// WARNING Property c2.base does not exist on {http://schemas.microsoft.com/winfx/2006/xaml/presentation}Page, the namespace is http://www.w3.org/XML/1998/namespace. This error was considered irrelevant by the XamlFileGenerator
			}
			))
			.GenericApply(((c3) => 
			{
				// Class Uno.UI.Tests.Windows_UI_Xaml_Data.BindingTests.Controls.Binding_ElementName_In_Template
				global::Uno.UI.FrameworkElementHelper.SetBaseUri(c3, __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca);
				c3.CreationComplete();
			}
			))
			;
			OnInitializeCompleted();

		}
		partial void OnInitializeCompleted();
		private global::Windows.UI.Xaml.Data.ElementNameSubject _topLevelSubject = new global::Windows.UI.Xaml.Data.ElementNameSubject();
		public global::Windows.UI.Xaml.Controls.ContentControl topLevel
		{
			get
			{
				return (global::Windows.UI.Xaml.Controls.ContentControl)_topLevelSubject.ElementInstance;
			}
			set
			{
				_topLevelSubject.ElementInstance = value;
			}
		}
		private class _Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_UnoUITestsWindows_UI_Xaml_DataBindingTestsControlsBinding_ElementName_In_TemplateSC0
		{
			[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
			private const string __baseUri_prefix_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca = "ms-appx:///TestProject/";
			[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
			private const string __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca = "ms-appx:///TestProject/";
			global::Windows.UI.Xaml.NameScope __nameScope = new global::Windows.UI.Xaml.NameScope();
			public _View Build(object __ResourceOwner_1)
			{
				_View __rootInstance = null;
				var __that = this;
				__rootInstance = 
				new global::Windows.UI.Xaml.Controls.TextBlock
				{
					IsParsing = true,
					Name = "innerTextBlock",
					// Source 0\Binding_ElementName_In_Template.xaml (Line 14:7)
				}
				.Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply((Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237ecaXamlApplyExtensions.XamlApplyHandler2)(c4 => 
				{
					/* _isTopLevelDictionary:False */
					__that._component_0 = c4;
					__nameScope.RegisterName("innerTextBlock", c4);
					__that.innerTextBlock = c4;
					c4.SetBinding(
						global::Windows.UI.Xaml.Controls.TextBlock.TextProperty,
						new Windows.UI.Xaml.Data.Binding()
						{
							Path = @"Tag",
							ElementName = _topLevelSubject,
						}
					);
					global::Uno.UI.FrameworkElementHelper.SetBaseUri(c4, __baseUri_Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca);
					c4.CreationComplete();
				}
				))
				;
				if (__rootInstance is FrameworkElement __fe) 
				{
					var owner = this;
					__fe.Loading += delegate
					{
						_component_0.UpdateResourceBindings();
					}
					;
				}
				if (__rootInstance is DependencyObject d)
				{
					if (global::Windows.UI.Xaml.NameScope.GetNameScope(d) == null)
					{
						global::Windows.UI.Xaml.NameScope.SetNameScope(d, __nameScope);
						__nameScope.Owner = d;
					}
					global::Uno.UI.FrameworkElementHelper.AddObjectReference(d, this);
				}
				return __rootInstance;
			}
			private global::Windows.UI.Xaml.Markup.ComponentHolder _component_0_Holder  = new global::Windows.UI.Xaml.Markup.ComponentHolder(isWeak: true);
			private global::Windows.UI.Xaml.Controls.TextBlock _component_0
			{
				get
				{
					return (global::Windows.UI.Xaml.Controls.TextBlock)_component_0_Holder.Instance;
				}
				set
				{
					_component_0_Holder.Instance = value;
				}
			}
			private global::Windows.UI.Xaml.Data.ElementNameSubject _innerTextBlockSubject = new global::Windows.UI.Xaml.Data.ElementNameSubject();
			private global::Windows.UI.Xaml.Controls.TextBlock innerTextBlock
			{
				get
				{
					return (global::Windows.UI.Xaml.Controls.TextBlock)_innerTextBlockSubject.ElementInstance;
				}
				set
				{
					_innerTextBlockSubject.ElementInstance = value;
				}
			}
			private global::Windows.UI.Xaml.Data.ElementNameSubject _topLevelSubject = new global::Windows.UI.Xaml.Data.ElementNameSubject(isRuntimeBound: true, name: "topLevel");
		}

	}
}
namespace MyProject
{
	static class Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237ecaXamlApplyExtensions
	{
		public delegate void XamlApplyHandler0(global::Windows.UI.Xaml.Controls.ContentControl instance);
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static global::Windows.UI.Xaml.Controls.ContentControl Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply(this global::Windows.UI.Xaml.Controls.ContentControl instance, XamlApplyHandler0 handler)
		{
			handler(instance);
			return instance;
		}
		public delegate void XamlApplyHandler1(global::Windows.UI.Xaml.Controls.Grid instance);
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static global::Windows.UI.Xaml.Controls.Grid Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply(this global::Windows.UI.Xaml.Controls.Grid instance, XamlApplyHandler1 handler)
		{
			handler(instance);
			return instance;
		}
		public delegate void XamlApplyHandler2(global::Windows.UI.Xaml.Controls.TextBlock instance);
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public static global::Windows.UI.Xaml.Controls.TextBlock Binding_ElementName_In_Template_66bf0a54f1801c397a6fa4930a237eca_XamlApply(this global::Windows.UI.Xaml.Controls.TextBlock instance, XamlApplyHandler2 handler)
		{
			handler(instance);
			return instance;
		}
	}
}