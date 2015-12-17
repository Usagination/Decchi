﻿using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Decchi.ParsingModule
{
	public sealed class AlsongSongInfo : SongInfo
	{
		public override string Client { get { return "알송"; } }
		public override string ClientIcon { get { return "/Decchi;component/ParsingModule/IconImages/AlSong.png"; } }

		public override bool GetCurrentPlayingSong( )
		{
			var str = NativeMethods.GetWindowTitle("ALSong_Class", null);

			return str != null ? this.Parse(str) : false;
		}

		private bool Parse(string str)
		{
			var alsongFormat = Registry.GetValue(@"HKEY_CURRENT_USER\Software\ESTsoft\ALSong\Param\Option\Title", "DescFormat", null) as string;
			if (alsongFormat == null) return false;
			
			alsongFormat = alsongFormat.Replace("%가수%", "(?<가수>.+)");
			alsongFormat = alsongFormat.Replace("%제목%", "(?<제목>.+)");
			alsongFormat = alsongFormat.Replace("%앨범%", "(?<앨범>.+)");
			alsongFormat = alsongFormat.Replace("%년도%", "(?<년도>.+)");
			alsongFormat = alsongFormat.Replace("%설명%", "(?<설명>.+)");
			alsongFormat = alsongFormat.Replace("%장르%", "(?<장르>.+)");
			alsongFormat = alsongFormat.Replace("%파일%", "(?<파일>.+)");
			alsongFormat = alsongFormat.Replace("%경로%", "(?<경로>.+)");
			alsongFormat = alsongFormat.Replace("%트랙%", "(?<트랙>.+)");
			
			try
			{
				var match = Regex.Match(str, alsongFormat, RegexOptions.IgnoreCase);
				if (!match.Success) return false;

				Group g;

				g = match.Groups["가수"];
				this.Artist	= g != null ? g.Value : null;

				g = match.Groups["제목"];
				this.Title	= g != null ? g.Value : null;

				g = match.Groups["앨범"];
				this.Album	= g != null ? g.Value : null;

				this.Loaded	= true;

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}