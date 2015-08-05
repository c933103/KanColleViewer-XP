﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NAudioStripped.CoreAudioApi;
using NAudioStripped.CoreAudioApi.Interfaces;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class Volume : NotificationObject, IAudioSessionEvents
	{
		private SimpleAudioVolume simpleAudioVolume;

		#region IsMute 変更通知プロパティ

		private bool _IsMute;

		public bool IsMute
		{
			get { return this._IsMute; }
			private set
			{
				if (this._IsMute != value)
				{
					this._IsMute = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Value 変更通知プロパティ

		private int _Value;

		public int Value
		{
			get { return this._Value; }
			private set
			{
				if (this._Value != value)
				{
					this._Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		private Volume() { }

		public static Volume GetInstance()
		{
			var volume = new Volume();
			var processId = Process.GetCurrentProcess().Id;

			var devenum = new MMDeviceEnumerator();
			var device = devenum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

			for (var i = 0; i < device.AudioSessionManager.Sessions.Count; i++)
			{
				var session = device.AudioSessionManager.Sessions[i];
				if (session.GetProcessID == processId)
				{
					volume.simpleAudioVolume = session.SimpleAudioVolume;
					volume.IsMute = session.SimpleAudioVolume.Mute;
					volume.Value = (int)(session.SimpleAudioVolume.Volume * 100);
					// ToDo: ↓ これ入れて通知受けるようにすると、通知が走ったタイミングでアプリが落ちる。意味不明。誰か助けて。
					//session.RegisterAudioSessionNotification(volume);
					return volume;
				}
			}

			throw new Exception("Session is not found.");
		}

		public void ToggleMute()
		{
			this.simpleAudioVolume.Mute = !this.simpleAudioVolume.Mute;
			this.IsMute = this.simpleAudioVolume.Mute;
		}

		public void SetVolume(int volume)
		{
			this.simpleAudioVolume.Volume = volume / 100.0f;
			this.Value = (int)(this.simpleAudioVolume.Volume * 100);
		}


		#region IAudioSessionEvents

		int IAudioSessionEvents.OnDisplayNameChanged(string newDisplayName, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnIconPathChanged(string newIconPath, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnSimpleVolumeChanged(float newVolume, bool newMute, ref Guid eventContext)
		{
			this.IsMute = newMute;
			this.Value = (int)(newVolume * 100);

			return 0;
		}

		int IAudioSessionEvents.OnChannelVolumeChanged(uint channelCount, IntPtr newChannelVolumeArray, uint changedChannel, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnGroupingParamChanged(ref Guid newGroupingParam, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnStateChanged(AudioSessionState newState)
		{
			return 0;
		}

		int IAudioSessionEvents.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
		{
			return 0;
		}

		#endregion
	}
}
