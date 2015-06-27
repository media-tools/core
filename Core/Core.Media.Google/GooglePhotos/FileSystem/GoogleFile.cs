using System;
using Core.Media.Common;
using Core.Shell.Common.FileSystems;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos.FileSystem
{
	public class GoogleFile : VirtualFile
	{
		public GoogleFile (Path path)
			: base (path)
		{
		}

		#region implemented abstract members of VirtualNode

		public override bool Validate (bool throwExceptions)
		{
			return Path.VirtualPath.Length == 3;
		}

		public override string PermissionsString {
			get {
				return "none";
			}
		}

		public override string OwnerName {
			get {
				return Path.VirtualPath [0];
			}
		}

		public override string GroupName {
			get {
				return "google";
			}
		}

		#endregion

		#region implemented abstract members of VirtualFile

		public override VirtualFileReader OpenReader ()
		{
			return null;
		}

		public override VirtualFileWriter OpenWriter ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}

}
