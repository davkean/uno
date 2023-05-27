#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Composition
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class CompositionObject : global::System.IDisposable,global::Windows.UI.Composition.IAnimationObject
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.Compositor Compositor
		{
			get
			{
				throw new global::System.NotImplementedException("The member Compositor CompositionObject.Compositor is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=Compositor%20CompositionObject.Compositor");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Core.CoreDispatcher Dispatcher
		{
			get
			{
				throw new global::System.NotImplementedException("The member CoreDispatcher CompositionObject.Dispatcher is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=CoreDispatcher%20CompositionObject.Dispatcher");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.CompositionPropertySet Properties
		{
			get
			{
				throw new global::System.NotImplementedException("The member CompositionPropertySet CompositionObject.Properties is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=CompositionPropertySet%20CompositionObject.Properties");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.ImplicitAnimationCollection ImplicitAnimations
		{
			get
			{
				throw new global::System.NotImplementedException("The member ImplicitAnimationCollection CompositionObject.ImplicitAnimations is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=ImplicitAnimationCollection%20CompositionObject.ImplicitAnimations");
			}
			set
			{
				global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "ImplicitAnimationCollection CompositionObject.ImplicitAnimations");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string Comment
		{
			get
			{
				throw new global::System.NotImplementedException("The member string CompositionObject.Comment is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=string%20CompositionObject.Comment");
			}
			set
			{
				global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "string CompositionObject.Comment");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.System.DispatcherQueue DispatcherQueue
		{
			get
			{
				throw new global::System.NotImplementedException("The member DispatcherQueue CompositionObject.DispatcherQueue is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=DispatcherQueue%20CompositionObject.DispatcherQueue");
			}
		}
		#endif
		// Forced skipping of method Windows.UI.Composition.CompositionObject.Compositor.get
		// Forced skipping of method Windows.UI.Composition.CompositionObject.Dispatcher.get
		// Forced skipping of method Windows.UI.Composition.CompositionObject.Properties.get
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StartAnimation( string propertyName,  global::Windows.UI.Composition.CompositionAnimation animation)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StartAnimation(string propertyName, CompositionAnimation animation)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StopAnimation( string propertyName)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StopAnimation(string propertyName)");
		}
		#endif
		// Forced skipping of method Windows.UI.Composition.CompositionObject.Comment.get
		// Forced skipping of method Windows.UI.Composition.CompositionObject.Comment.set
		// Forced skipping of method Windows.UI.Composition.CompositionObject.ImplicitAnimations.get
		// Forced skipping of method Windows.UI.Composition.CompositionObject.ImplicitAnimations.set
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StartAnimationGroup( global::Windows.UI.Composition.ICompositionAnimationBase value)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StartAnimationGroup(ICompositionAnimationBase value)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StopAnimationGroup( global::Windows.UI.Composition.ICompositionAnimationBase value)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StopAnimationGroup(ICompositionAnimationBase value)");
		}
		#endif
		// Forced skipping of method Windows.UI.Composition.CompositionObject.DispatcherQueue.get
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.AnimationController TryGetAnimationController( string propertyName)
		{
			throw new global::System.NotImplementedException("The member AnimationController CompositionObject.TryGetAnimationController(string propertyName) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=AnimationController%20CompositionObject.TryGetAnimationController%28string%20propertyName%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void Dispose()
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.Dispose()");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void PopulatePropertyInfo( string propertyName,  global::Windows.UI.Composition.AnimationPropertyInfo propertyInfo)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.PopulatePropertyInfo(string propertyName, AnimationPropertyInfo propertyInfo)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static void StartAnimationWithIAnimationObject( global::Windows.UI.Composition.IAnimationObject target,  string propertyName,  global::Windows.UI.Composition.CompositionAnimation animation)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StartAnimationWithIAnimationObject(IAnimationObject target, string propertyName, CompositionAnimation animation)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static void StartAnimationGroupWithIAnimationObject( global::Windows.UI.Composition.IAnimationObject target,  global::Windows.UI.Composition.ICompositionAnimationBase animation)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.CompositionObject", "void CompositionObject.StartAnimationGroupWithIAnimationObject(IAnimationObject target, ICompositionAnimationBase animation)");
		}
		#endif
		// Processing: System.IDisposable
		// Processing: Windows.UI.Composition.IAnimationObject
	}
}
