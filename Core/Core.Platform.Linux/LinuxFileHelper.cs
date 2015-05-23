using System;
using System.IO;
using Core.Common;
using Core.IO;
using Mono.Unix;

namespace Core.Platform.Linux
{
	class LinuxFileHelper : FileHelper
	{
		static UnixFileSystemInfo GetUnixFileInfo (string path)
		{
			try {
				return UnixFileSystemInfo.GetFileSystemEntry (path);
			} catch (DirectoryNotFoundException ex) {
				// If we have a file /foo/bar and probe the path /foo/bar/baz, we get a DirectoryNotFound exception
				// because 'bar' is a file and therefore 'baz' cannot possibly exist. This is annoying.
				var inner = ex.InnerException as UnixIOException;
				if (inner != null && inner.ErrorCode == Mono.Unix.Native.Errno.ENOTDIR)
					return null;
				throw;
			}
		}

		public override bool CanExecute (string path)
		{
			UnixFileInfo fi = new UnixFileInfo (path);
			if (!fi.Exists)
				return false;
			return 0 != (fi.FileAccessPermissions & (FileAccessPermissions.UserExecute | FileAccessPermissions.GroupExecute | FileAccessPermissions.OtherExecute));
		}

		public override bool CanWrite (string path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.CanAccess (Mono.Unix.Native.AccessModes.W_OK);
		}

		public override bool Delete (string path)
		{
			var info = GetUnixFileInfo (path);
			if (info != null && info.Exists) {
				try {
					info.Delete ();
					return true;
				} catch {
					// If the directory is not empty we return false. JGit relies on this
					return false;
				}
			}
			return false;
		}

		public override bool Exists (string path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists;
		}

		public override bool IsDirectory (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists && info.FileType == FileTypes.Directory;
			} catch (DirectoryNotFoundException) {
				// If the file /foo/bar exists and we query to see if /foo/bar/baz exists, we get a
				// DirectoryNotFound exception for Mono.Unix. In this case the directory definitely
				// does not exist.
				return false;
			}
		}

		public override bool IsFile (string path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists && (info.FileType == FileTypes.RegularFile || info.FileType == FileTypes.SymbolicLink);
		}

		public override long LastModified (string path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
		}

		public override long Length (string path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists ? info.Length : 0;
		}

		public override void MakeFileWritable (string file)
		{
			var info = GetUnixFileInfo (file);
			if (info != null)
				info.FileAccessPermissions |= (FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
		}

		public override bool RenameTo (string path, string name)
		{
			var symlink = GetUnixFileInfo (path) as UnixSymbolicLinkInfo;
			if (symlink != null) {
				var newFile = new UnixSymbolicLinkInfo (name);
				newFile.CreateSymbolicLinkTo (symlink.ContentsPath);
				return true;
			} else {
				// This call replaces the file if it already exists.
				// File.Move throws an exception if dest already exists
				return Mono.Unix.Native.Stdlib.rename (path, name) == 0;
			}
		}

		public override bool SetExecutable (string path, bool exec)
		{
			UnixFileInfo fi = new UnixFileInfo (path);
			FileAccessPermissions perms = fi.FileAccessPermissions;
			if (exec) {
				if (perms.HasFlag (FileAccessPermissions.UserRead))
					perms |= FileAccessPermissions.UserExecute;
				if (perms.HasFlag (FileAccessPermissions.OtherRead))
					perms |= FileAccessPermissions.OtherExecute;
				if ((perms.HasFlag (FileAccessPermissions.GroupRead)))
					perms |= FileAccessPermissions.GroupExecute;
			} else {
				if (perms.HasFlag (FileAccessPermissions.UserRead))
					perms &= ~FileAccessPermissions.UserExecute;
				if (perms.HasFlag (FileAccessPermissions.OtherRead))
					perms &= ~FileAccessPermissions.OtherExecute;
				if ((perms.HasFlag (FileAccessPermissions.GroupRead)))
					perms &= ~FileAccessPermissions.GroupExecute;
			}
			fi.FileAccessPermissions = perms;
			return true;
		}

		public override bool SetLastModified (string path, long milis)
		{
			// How can the last write time be set on a symlink?
			return base.SetLastModified (path, milis);
		}

		public override bool SetReadOnly (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				if (info != null)
					info.FileAccessPermissions &= ~(FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
				return true;
			} catch {
				return false;
			}
		}

		public LinuxFileAccessPermissions Permissions (string path)
		{
			/*
			ufi.CanAccess (AccessModes.F_OK); // is a file/directory
			ufi.CanAccess (AccessModes.R_OK); // accessible for reading
			ufi.CanAccess (AccessModes.W_OK); // accessible for writing
			ufi.CanAccess (AccessModes.X_OK); // accessible for executing
			FileSpecialAttributes sa = ufi.FileSpecialAttributes; //setuid, setgid and sticky bits
			*/
			var ufi = new UnixFileInfo (path);
			FileAccessPermissions fa = ufi.FileAccessPermissions;
			return (LinuxFileAccessPermissions)fa;
		}

		public override string PermissionsString (string path)
		{
			return formatPermissions (Permissions (path: path));
		}

		string formatPermissions (LinuxFileAccessPermissions p)
		{
			char ur = (p & LinuxFileAccessPermissions.UserRead) > 0 ? 'r' : '-';
			char uw = (p & LinuxFileAccessPermissions.UserRead) > 0 ? 'w' : '-';
			char ux = (p & LinuxFileAccessPermissions.UserRead) > 0 ? 'x' : '-';
			string result = $"{ur}{uw}{ux}";
			return result;
		}
	}
}