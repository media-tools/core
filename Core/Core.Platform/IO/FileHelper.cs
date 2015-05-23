using System;
using System.IO;
using System.Reflection;
using Core.Common;
using Core.Platform.Linux;

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
			return ((File.GetAttributes (path) & FileAttributes.ReadOnly) == 0);
		}

		public virtual bool Delete (string path)
		{
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
			return false;
		}

		public virtual bool Exists (string path)
		{
			return (File.Exists (path) || Directory.Exists (path));
		}

		public virtual bool IsDirectory (string path)
		{
			return Directory.Exists (path);
		}

		public virtual bool IsFile (string path)
		{
			return File.Exists (path);
		}

		public virtual long LastModified (string path)
		{
			if (IsFile (path)) {
				var info2 = new FileInfo (path);
				return info2.Exists ? info2.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
			} else if (IsDirectory (path)) {
				var info = new DirectoryInfo (path);
				return info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
			}
			return 0;
		}

		public virtual long Length (string path)
		{
			// If you call .Length on a file that doesn't exist, an exception is thrown
			var info2 = new FileInfo (path);
			return info2.Exists ? info2.Length : 0;
		}

		public virtual void MakeDirWritable (string path)
		{
			foreach (string file in Directory.GetFiles (path)) {
				MakeFileWritable (file);
			}
			foreach (string subdir in Directory.GetDirectories (path)) {
				MakeDirWritable (subdir);
			}
		}

		public virtual void MakeFileWritable (string file)
		{
			FileAttributes fileAttributes = File.GetAttributes (file);
			if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
				fileAttributes &= ~FileAttributes.ReadOnly;
				File.SetAttributes (file, fileAttributes);
			}
		}

		public virtual bool RenameTo (string path, string name)
		{
			try {
				File.Move (path, name);
				return true;
			} catch {
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
			} catch {
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
				Log.Error (ex);
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

		public virtual string GetOwner (string path)
		{
			string user = File.GetAccessControl (path).GetOwner (typeof(System.Security.Principal.NTAccount)).ToString ();
			return user;
		}
	}

	[Flags ()]
	public enum LinuxFileAccessPermissions
	{
		UserReadWriteExecute = 448,
		UserRead = 256,
		UserWrite = 128,
		UserExecute = 64,
		GroupReadWriteExecute = 56,
		GroupRead = 32,
		GroupWrite = 16,
		GroupExecute = 8,
		OtherReadWriteExecute = 7,
		OtherRead = 4,
		OtherWrite = 2,
		OtherExecute = 1,
		DefaultPermissions = 438,
		AllPermissions = 511
	}

	public struct _UserInfo
	{
		public _GroupInfo Group { get; }

		public long GroupId  { get; }

		public string GroupName  { get; }

		public string HomeDirectory { get; }

		public string Password  { get; }

		public string RealName { get; }

		public long UserId { get; }

		public string UserName { get; }
	}

	public struct _GroupInfo
	{
		public long GroupId  { get; }

		public string GroupName  { get; }

		public string Password  { get; }

	}
}

