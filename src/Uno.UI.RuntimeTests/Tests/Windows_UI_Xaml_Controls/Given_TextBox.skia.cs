﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MUXControlsTestApp.Utilities;
using Uno.Extensions;
using Uno.UI.RuntimeTests.Helpers;
using static Private.Infrastructure.TestServices;

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml_Controls
{
	/// <summary>
	/// This partial is for testing the skia-based TextBox implementation.
	/// Most tests here should set UseOverlayOnSkia to false and HideCaret
	/// to true and then set them back at the end of the test.
	/// </summary>
	public partial class Given_TextBox
	{
		[TestMethod]
		public async Task When_Basic_Input()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var text = "Hello world";
			foreach (var c in text)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(text, SUT.Text);
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Basic_Input_With_ArrowKeys()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			for (int i = 1; i <= 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(5 - i, SUT.SelectionStart);
				Assert.AreEqual(0, SUT.SelectionLength);
			}

			foreach (var c in "Hello ")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Basic_Input_With_Home_End()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			foreach (var c in "Hello ")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Hello world", SUT.Text);
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Selection_With_Keyboard_NoMod_And_Shift()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "Hello world")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();

			for (var i = 1; i <= 11; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(11 - i, SUT.SelectionStart);
				Assert.AreEqual(i, SUT.SelectionLength);
			}

			for (var i = 1; i <= 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(i, SUT.SelectionStart);
				Assert.AreEqual(11 - i, SUT.SelectionLength);
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(11, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(11, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Ctrl_A()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.Control, unicodeKey: 'a'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Selection_With_Keyboard_NoMod_Ctrl_And_Shift()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "Hello &(%&^( w0.rld")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(15, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(16, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(19, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Text_Bigger_Than_TextBox()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			foreach (var c in "This should be a lot longer than the width of the TextBox.")
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}
			await WindowHelper.WaitForIdle();

			var sv = SUT.FindVisualChildByType<ScrollViewer>();
			Assert.IsTrue(sv.HorizontalOffset > 0);
			Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			Assert.IsTrue(sv.ScrollableWidth > 0);
			Assert.AreEqual(0, sv.HorizontalOffset);
		}

		[TestMethod]
		public async Task When_KeyDown_Bubbles_Out()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 100,
				Text = "Hello world"
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.End, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(2, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(3, keyDownCount);


			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(2, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Up, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(4, keyDownCount);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Down, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(5, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(6, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(0, 0);
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.Select(SUT.Text.Length, 0);
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Delete, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(7, keyDownCount);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.None, unicodeKey: 'A'));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(8, keyDownCount);
		}

		[TestMethod]
		public async Task When_Selection_Initial_Then_Text_Changed()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "Initial"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Text = "Changed";

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_ReadOnly()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "Initial",
				IsReadOnly = true
			};

			var keyDownCount = 0;
			SUT.KeyDown += (_, _) => keyDownCount++;

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.A, VirtualKeyModifiers.None, unicodeKey: 'A'));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("Initial", SUT.Text);
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(1, keyDownCount);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(2, keyDownCount);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
			Assert.AreEqual(2, keyDownCount);
		}

		[TestMethod]
		public async Task When_Long_Text_Unfocused()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 80,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			var btn = new Button();

			var sp = new StackPanel
			{
				Children =
				{
					SUT,
					btn
				}
			};


			WindowHelper.WindowContent = sp;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			// DeleteButton Takes space to the right of sv
			Assert.IsTrue(LayoutInformation.GetLayoutSlot(SUT).Right - LayoutInformation.GetLayoutSlot(sv).Right > 10);

			btn.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();
			Assert.IsTrue(LayoutInformation.GetLayoutSlot(SUT).Right - LayoutInformation.GetLayoutSlot(sv).Right < 10);
		}

		[TestMethod]
		public async Task When_Scrolling_Updates_With_Movement()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 40,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);

			for (var i = 0; i < 6; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(sv.ScrollableWidth, sv.HorizontalOffset);
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.IsTrue(sv.HorizontalOffset < sv.ScrollableWidth);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Home, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, sv.HorizontalOffset);

			for (var i = 0; i < 6; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(0, sv.HorizontalOffset);
			}

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.None));
			await WindowHelper.WaitForIdle();
			Assert.IsTrue(sv.HorizontalOffset > 0);
		}

		[TestMethod]
		public async Task When_Scrolling_Updates_After_Backspace()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var sv = SUT.FindVisualChildByType<ScrollViewer>();

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.Select(SUT.Text.Length, 0);
			await WindowHelper.WaitForIdle();
			// DeleteButton Takes space to the right of sv
			Assert.IsTrue(LayoutInformation.GetLayoutSlot(SUT).Right - LayoutInformation.GetLayoutSlot(sv).Right > 10);

			var svRight = LayoutInformation.GetLayoutSlot(SUT).Right;
			for (var i = 0; i < 5; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.None));
				await WindowHelper.WaitForIdle();
				Assert.AreEqual(svRight, LayoutInformation.GetLayoutSlot(SUT).Right);
			}

			for (var i = 0; i < 10; i++)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Back, VirtualKeyModifiers.Control));
			}
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, sv.ScrollableWidth);
		}

		[TestMethod]
		public async Task When_Pointer_Tap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_Hold_Drag()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(-50, 0);

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(9, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Pointer_Hold_Drag_OutOfBounds()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(-150, 0);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(10, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_LongText_Pointer_Hold_Drag_OutOfBounds()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "This should be a lot longer than the width of the TextBox."
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(-150, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(10, SUT.SelectionLength);

			mouse.MoveBy(0, 50);
			mouse.MoveBy(600, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 10, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_DoubleTapped()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			// the selection should start on the left
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(4, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_DoubleTapHeld()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(6, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			mouse.MoveBy(-40, 0);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);

			// the selection should start on the right
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(1, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Chunk_TripleTapped()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			// double tap
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length, SUT.SelectionLength);

			// the selection should start on the left
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_NonAscii_Characters()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox();

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var text = "صباح الخير";
			foreach (var c in text)
			{
				SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.None, VirtualKeyModifiers.None, unicodeKey: c));
			}

			await WindowHelper.WaitForIdle();
			Assert.AreEqual(text, SUT.Text);
		}

		[TestMethod]
		public async Task When_Copy_Paste()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150
			};
			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			var dp = new DataPackage();
			var text = "copied content";
			dp.SetText(text);
			Clipboard.SetContent(dp);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(text, SUT.Text);

			SUT.Select(2, 4);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.C, VirtualKeyModifiers.Control, unicodeKey: 'c'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(SUT.Text.Substring(2, 4), await Clipboard.GetContent()!.GetTextAsync());
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(4, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length - 1, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("copied contenpiedt", SUT.Text);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(6, 3);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("copiedpiedntenpiedt", SUT.Text);
			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Cut_Paste()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 150,
				Text = "Hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Select(2, 4);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.X, VirtualKeyModifiers.Control, unicodeKey: 'x'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("llo ", await Clipboard.GetContent()!.GetTextAsync());
			Assert.AreEqual("Heworld", SUT.Text);
			Assert.AreEqual(2, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(SUT.Text.Length - 1, 0);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Heworlllo d", SUT.Text);
			Assert.AreEqual(SUT.Text.Length - 1, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.Select(6, 3);
			await WindowHelper.WaitForIdle();
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.V, VirtualKeyModifiers.Control, unicodeKey: 'v'));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("Heworlllo  d", SUT.Text);
			Assert.AreEqual(10, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "hello";
			await WindowHelper.WaitForIdle();

			var height = SUT.ActualHeight;

			SUT.Text = "hello\rworld";
			await WindowHelper.WaitForIdle();

			Assert.IsTrue(SUT.ActualHeight > 1.5 * height);
		}

		[TestMethod]
		public async Task When_Multiline_LineFeed()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "lorem\nipsum\r\ndolor";

			await WindowHelper.WaitForIdle();
			Assert.AreEqual("lorem\ripsum\rdolor", SUT.Text);
		}

		[TestMethod]
		public async Task When_Multiline_Return_Selected()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Text = "hello\rworld";
			SUT.Select(4, 0);
			await WindowHelper.WaitForIdle();

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("o\r", SUT.SelectedText);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));
			await WindowHelper.WaitForIdle();

			Assert.AreEqual("o\rw", SUT.SelectedText);
		}

		[TestMethod]
		public async Task When_Multiline_Keyboard_Chunking()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text =
				"""
				Lorem 
				     
				
				ipsum

				&&^
				    
				
				
				"""
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(7, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(12, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(13, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(19, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(20, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(21, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(24, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(25, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(29, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(30, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift | VirtualKeyModifiers.Control));
			await WindowHelper.WaitForIdle();
			Assert.AreEqual(0, SUT.SelectionStart);
			Assert.AreEqual(31, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Text_Ends_In_Return()
		{
			var SUT = new TextBox
			{
				AcceptsReturn = true,
				Text = "hello world"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(SUT);
			await WindowHelper.WaitForIdle();

			var height = SUT.ActualHeight;

			SUT.Text += "\r";

			Assert.IsTrue(SUT.ActualHeight > height * 1.5);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_Tap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "Lorem\ripsum dolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(38, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);

			mouse.MoveTo(bounds.GetCenter() - new Point(40, 10));
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(17, SUT.SelectionStart);
			Assert.AreEqual(0, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_DoubleTap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "Lorem\ripsum dolor sit\ramet consectetur\radipiscing"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			await WindowHelper.WaitForIdle();

			mouse.MoveTo(bounds.GetCenter() - new Point(40, 10));
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(6, SUT.SelectionLength);

			mouse.MoveBy(-40, 10);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(15, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Left, VirtualKeyModifiers.Shift));

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(22, SUT.SelectionStart);
			Assert.AreEqual(5, SUT.SelectionLength);

			mouse.MoveBy(40, -10);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(12, SUT.SelectionStart);
			Assert.AreEqual(15, SUT.SelectionLength);

			SUT.SafeRaiseEvent(UIElement.KeyDownEvent, new KeyRoutedEventArgs(SUT, VirtualKey.Right, VirtualKeyModifiers.Shift));

			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);
		}

		[TestMethod]
		public async Task When_Multiline_Pointer_TripleTap()
		{
			using var _ = new TextBoxFeatureConfigDisposable();

			var SUT = new TextBox
			{
				Width = 250,
				AcceptsReturn = true,
				Text = "elit aliquam\rullamcorper\rcommodoprimis\rornare himenaeos"
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForIdle();
			await WindowHelper.WaitForLoaded(SUT);

			SUT.Focus(FocusState.Programmatic);
			await WindowHelper.WaitForIdle();

			var injector = InputInjector.TryCreate() ?? throw new InvalidOperationException("Failed to init the InputInjector");
			using var mouse = injector.GetMouse();

			var bounds = SUT.GetAbsoluteBounds();
			mouse.MoveTo(bounds.GetCenter());
			await WindowHelper.WaitForIdle();

			mouse.Press();
			mouse.Release();
			mouse.Press();
			mouse.Release();
			mouse.Press();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(25, SUT.SelectionStart);
			Assert.AreEqual(14, SUT.SelectionLength);

			mouse.MoveBy(0, 20);
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(25, SUT.SelectionStart);
			Assert.AreEqual(30, SUT.SelectionLength);

			mouse.MoveBy(0, -35);
			mouse.Release();
			await WindowHelper.WaitForIdle();

			Assert.AreEqual(13, SUT.SelectionStart);
			Assert.AreEqual(26, SUT.SelectionLength);
		}

		private class TextBoxFeatureConfigDisposable : IDisposable
		{
			private bool _useOverlay;
			private bool _hideCaret;

			public TextBoxFeatureConfigDisposable()
			{
				_useOverlay = FeatureConfiguration.TextBox.UseOverlayOnSkia;
				_hideCaret = FeatureConfiguration.TextBox.HideCaret;

				FeatureConfiguration.TextBox.UseOverlayOnSkia = false;
				FeatureConfiguration.TextBox.HideCaret = true;
			}

			public void Dispose()
			{
				FeatureConfiguration.TextBox.UseOverlayOnSkia = _useOverlay;
				FeatureConfiguration.TextBox.HideCaret = _hideCaret;
			}
		}
	}
}
