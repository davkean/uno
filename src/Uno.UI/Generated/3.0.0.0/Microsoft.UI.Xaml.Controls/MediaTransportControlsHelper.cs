#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Microsoft.UI.Xaml.Controls
{
	#if false || false || NET461 || false || __SKIA__ || __NETSTD_REFERENCE__ || false
	[global::Uno.NotImplemented("NET461", "__SKIA__", "__NETSTD_REFERENCE__")]
	#endif
	public  partial class MediaTransportControlsHelper 
	{
		#if false || false || NET461 || false || __SKIA__ || __NETSTD_REFERENCE__ || false
		[global::Uno.NotImplemented("NET461", "__SKIA__", "__NETSTD_REFERENCE__")]
		public static global::Microsoft.UI.Xaml.DependencyProperty DropoutOrderProperty { get; } = 
		Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(
			"DropoutOrder", typeof(int?), 
			typeof(global::Microsoft.UI.Xaml.Controls.MediaTransportControlsHelper), 
			new Microsoft.UI.Xaml.FrameworkPropertyMetadata(default(int?)));
		#endif
		// Forced skipping of method Microsoft.UI.Xaml.Controls.MediaTransportControlsHelper.DropoutOrderProperty.get
		#if false || false || NET461 || false || __SKIA__ || __NETSTD_REFERENCE__ || false
		[global::Uno.NotImplemented("NET461", "__SKIA__", "__NETSTD_REFERENCE__")]
		public static int? GetDropoutOrder( global::Microsoft.UI.Xaml.UIElement element)
		{
			return (int?)element.GetValue(DropoutOrderProperty);
		}
		#endif
		#if false || false || NET461 || false || __SKIA__ || __NETSTD_REFERENCE__ || false
		[global::Uno.NotImplemented("NET461", "__SKIA__", "__NETSTD_REFERENCE__")]
		public static void SetDropoutOrder( global::Microsoft.UI.Xaml.UIElement element,  int? value)
		{
			element.SetValue(DropoutOrderProperty, value);
		}
		#endif
	}
}
