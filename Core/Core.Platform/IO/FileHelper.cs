using System;
using System.IO;
using System.Reflection;
using Core.Common;
using Core.Platform.Linux;
using Core.Portable;

namespace Core.IO
{
	public class FileHelper
	{
		internal static string LinuxFullClass = "Core.Platform.Linux.LinuxFileHelper";
		internal static string LinuxAssemblyName = "Core.Platform.Linux";

		public static FileHelper Instance { get; set; }

		static FileHelper ()
		{
			if (Environment.OSVersion.Platform.ToString ().StartsWith ("Win"))
				Instance = new FileHelper ();
			else {
				Instance = new LinuxFileHelper ();
			}
		}

		public virtual bool CanExecute (string path)
		{
			return false;
		}

		public virtual bool CanWrite (string path)
		{
			try {
				return ((File.GetAttributes (path) & FileAttributes.ReadOnly) == 0);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual bool Delete (string path)
		{
			try {
				if (Directory.Exists (path)) {
					if (Directory.GetFileSystemEntries (path).Length != 0)
						return false;
					MakeDirWritable (path);
					Directory.Delete (path, true);
					return true;
				} else if (File.Exists (path)) {
					MakeFileWritable (path);
					File.Delete (path);
					return true;
				}
			} catch (Exception ex) {
				Log.Warning (ex);
			}
			return false;
		}

		public virtual bool Exists (string path)
		{
			try {
				return (File.Exists (path) || Directory.Exists (path));
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual bool IsDirectory (string path)
		{
			try {
				return Directory.Exists (path);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual bool IsFile (string path)
		{
			try {
				return File.Exists (path);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual long LastModified (string path)
		{
			try {
				if (IsFile (path)) {
					var info2 = new FileInfo (path);
					return info2.Exists ? info2.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
				} else if (IsDirectory (path)) {
					var info = new DirectoryInfo (path);
					return info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
				}
			} catch (Exception ex) {
				Log.Warning (ex);
			}
			return 0;
		}

		public virtual long Length (string path)
		{
			try {
				// If you call .Length on a file that doesn't exist, an exception is thrown
				var info2 = new FileInfo (path);
				return info2.Exists ? info2.Length : 0;
			} catch (Exception ex) {
				Log.Warning (ex);
				return 0;
			}
		}

		public virtual void MakeDirWritable (string path)
		{
			try {
				foreach (string file in Directory.GetFiles (path)) {
					MakeFileWritable (file);
				}
				foreach (string subdir in Directory.GetDirectories (path)) {
					MakeDirWritable (subdir);
				}
			} catch (Exception ex) {
				Log.Warning (ex);
			}
		}

		public virtual void MakeFileWritable (string file)
		{
			try {
				FileAttributes fileAttributes = File.GetAttributes (file);
				if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
					fileAttributes &= ~FileAttributes.ReadOnly;
					File.SetAttributes (file, fileAttributes);
				}
			} catch (Exception ex) {
				Log.Warning (ex);
			}
		}

		public virtual bool RenameTo (string path, string name)
		{
			try {
				File.Move (path, name);
				return true;
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual bool SetExecutable (string path, bool exec)
		{
			return false;
		}

		public virtual bool SetReadOnly (string path)
		{
			try {
				var fileAttributes = File.GetAttributes (path) | FileAttributes.ReadOnly;
				File.SetAttributes (path, fileAttributes);
				return true;
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public virtual bool SetLastModified (string path, long milis)
		{
			try {
				DateTime utcDateTime = DateTimeExtensions.MillisToDateTimeOffset (milis, 0L).UtcDateTime;
				if (IsFile (path)) {
					var info2 = new FileInfo (path);
					info2.LastWriteTimeUtc = utcDateTime;
					return true;
				} else if (IsDirectory (path)) {
					var info = new DirectoryInfo (path);
					info.LastWriteTimeUtc = utcDateTime;
					return true;
				}
			} catch (Exception ex) {
				Log.Warning (ex);
			}
			return false;
		}

		public virtual string PermissionsString (string path)
		{
			return string.Empty;
		}

		public virtual bool IsSymLink (string path)
		{
			return false;
		}

		public virtual bool CreateSymLink (string target, string symLink)
		{
			return false;
		}

		public virtual string ReadSymLink (string path)
		{
			return null;
		}

		public virtual string GetOwnerName (string path)
		{
			try {
				string userName = File.GetAccessControl (path).GetOwner (typeof(System.Security.Principal.NTAccount)).ToString ();
				return userName.Replace ("\\", "/").Replace (PlatformInfo.User.HostName + "/", "");
			} catch (Exception ex) {
				Log.Warning (ex);
				return "invalid";
			}
		}

		public virtual string GetGroupName (string path)
		{
			try {
				string groupName = File.GetAccessControl (path).GetGroup (typeof(System.Security.Principal.NTAccount)).ToString ();
				return groupName.Replace ("\\", "/").Replace (PlatformInfo.User.HostName + "/", "");
			} catch (Exception ex) {
				Log.Warning (ex);
				return "invalid";
			}
		}
	}
}

