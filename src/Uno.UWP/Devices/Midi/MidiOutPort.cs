﻿using System;
using Uno.Devices.Enumeration.Internal;

namespace Windows.Devices.Midi
{
	/// <summary>
	/// Represents a port used to send MIDI messages to a MIDI device.
	/// </summary>
	public sealed partial class MidiOutPort : IDisposable
	{
		private readonly static string MidiOutAqsFilter =
			"System.Devices.InterfaceClassGuid:=\"{" + DeviceClassGuids.MidiOut + "}\" AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True";

		/// <summary>
		/// Gets a query string that can be used to enumerate all MidiOutPort objects on the system.
		/// </summary>
		/// <returns>The query string used to enumerate the MidiOutPort objects on the system.</returns>
		public static string GetDeviceSelector() => MidiOutAqsFilter;
	}
}
