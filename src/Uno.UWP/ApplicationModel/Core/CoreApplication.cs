#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding

using System;
using System.Collections.Generic;
using Uno.Helpers.Theming;

namespace Windows.ApplicationModel.Core;

public partial class CoreApplication
{
	private static CoreApplicationView _currentView;
	private static List<CoreApplicationView> _views;

	static CoreApplication()
	{
		_currentView = new CoreApplicationView();
	}

	/// <summary>
	/// Occurs when an app is resuming.
	/// </summary>
	public static event EventHandler<object> Resuming;

	/// <summary>
	/// Occurs when the app is suspending.
	/// </summary>
	public static event EventHandler<SuspendingEventArgs> Suspending;

	/// <summary>
	/// Fired when the app enters the running in the background state.
	/// </summary>
	public static event EventHandler<EnteredBackgroundEventArgs> EnteredBackground;

	/// <summary>
	/// Fired just before application UI becomes visible.
	/// </summary>
	public static event EventHandler<LeavingBackgroundEventArgs> LeavingBackground;

	/// <summary>
	/// Raises the <see cref="Resuming"/> event.
	/// </summary>
	internal static void RaiseResuming() => Resuming?.Invoke(null, null);

	/// <summary>
	/// Raises the <see cref="Suspending"/> event.
	/// </summary>
	/// <param name="args">Suspending event args.</param>
	internal static void RaiseSuspending(SuspendingEventArgs args) => Suspending?.Invoke(null, args);

	/// <summary>
	/// Raises the <see cref="EnteredBackground"/> event.
	/// </summary>
	/// <param name="args">Entered background event args.</param>
	internal static void RaiseEnteredBackground(EnteredBackgroundEventArgs args) => EnteredBackground?.Invoke(null, args);

	/// <summary>
	/// Raises the <see cref="LeavingBackground"/> event.
	/// </summary>
	/// <param name="args">Leaving background event args.</param>
	internal static void RaiseLeavingBackground(LeavingBackgroundEventArgs args) => LeavingBackground?.Invoke(null, args);

	public static CoreApplicationView GetCurrentView() => _currentView;

	public static CoreApplicationView MainView => _currentView;

	public static IReadOnlyList<CoreApplicationView> Views => _views ??= new List<CoreApplicationView>() { _currentView };

	/// <summary>
	/// This property is kept in sync with the Application.RequestedTheme to enable
	/// native UI elements in non Uno.UWP to resolve the currently set Application theme.
	/// </summary>
	internal static SystemTheme RequestedTheme { get; set; }
}
