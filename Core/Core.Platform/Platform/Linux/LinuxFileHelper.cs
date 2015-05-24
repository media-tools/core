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
			try {
				UnixFileInfo fi = new UnixFileInfo (path);
				if (!fi.Exists)
					return false;
				return 0 != (fi.FileAccessPermissions & (FileAccessPermissions.UserExecute | FileAccessPermissions.GroupExecute | FileAccessPermissions.OtherExecute));
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool CanWrite (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.CanAccess (Mono.Unix.Native.AccessModes.W_OK);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool Delete (string path)
		{
			try {
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
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool Exists (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists;
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool IsDirectory (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists && info.FileType == FileTypes.Directory;
			} catch (DirectoryNotFoundException ex) {
				Log.Debug (ex);
				// If the file /foo/bar exists and we query to see if /foo/bar/baz exists, we get a
				// DirectoryNotFound exception for Mono.Unix. In this case the directory definitely
				// does not exist.
				return false;
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool IsFile (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists && (info.FileType == FileTypes.RegularFile || info.FileType == FileTypes.SymbolicLink);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override long LastModified (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch () : 0;
			} catch (Exception ex) {
				Log.Warning (ex);
				return 0;
			}
		}

		public override long Length (string path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists ? info.Length : 0;
			} catch (Exception ex) {
				Log.Warning (ex);
				return 0;
			}
		}

		public override void MakeFileWritable (string file)
		{
			try {
				var info = GetUnixFileInfo (file);
				if (info != null)
					info.FileAccessPermissions |= (FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
			} catch (Exception ex) {
				Log.Warning (ex);
			}
		}

		public override bool RenameTo (string path, string name)
		{
			try {
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
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool SetExecutable (string path, bool exec)
		{
			try {
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
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool SetLastModified (string path, long milis)
		{
			try {
				// How can the last write time be set on a symlink?
				return base.SetLastModified (path, milis);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool SetReadOnly (string path)
		{
			try {
				try {
					var info = GetUnixFileInfo (path);
					if (info != null)
						info.FileAccessPermissions &= ~(FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
					return true;
				} catch {
					return false;
				}
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool IsSymLink (string path)
		{
			try {
				UnixSymbolicLinkInfo i = new UnixSymbolicLinkInfo (path);
				switch (i.FileType) {
				case FileTypes.SymbolicLink:
					return true;
				case FileTypes.Fifo:
				case FileTypes.Socket:
				case FileTypes.BlockDevice:
				case FileTypes.CharacterDevice:
				case FileTypes.Directory:
				case FileTypes.RegularFile:
				default:
					return false;
				}
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override bool CreateSymLink (string target, string symLink)
		{
			try {
				UnixFileInfo targetFile = new UnixFileInfo (target);
				targetFile.CreateSymbolicLink (symLink);
				return true;
			} catch (Exception ex) {
				Log.Error ("Failed to create symbolic link from '", symLink, "' to '", target, "'");
				Log.Error (ex);
				return false;
			}
		}

		public override string ReadSymLink (string path)
		{
			UnixSymbolicLinkInfo i = new UnixSymbolicLinkInfo (path);
			switch (i.FileType) {
			case FileTypes.SymbolicLink:
				try {
					return i.GetContents ().FullName;
				} catch (Exception ex) {
					Log.Error ("Failed to read symbolic link: '", path, "'");
					Log.Error (ex);
					return null;
				}
			case FileTypes.Fifo:
			case FileTypes.Socket:
			case FileTypes.BlockDevice:
			case FileTypes.CharacterDevice:
			case FileTypes.Directory:
			case FileTypes.RegularFile:
			default:
				return null;
			}
		}

		public FileAccessPermissions Permissions (string path)
		{
			/*
			ufi.CanAccess (AccessModes.F_OK); // is a file/directory
			ufi.CanAccess (AccessModes.R_OK); // accessible for reading
			ufi.CanAccess (AccessModes.W_OK); // accessible for writing
			ufi.CanAccess (AccessModes.X_OK); // accessible for executing
			FileSpecialAttributes sa = ufi.FileSpecialAttributes; //setuid, setgid and sticky bits
			*/

			try {
				var ufi = new UnixFileInfo (path);
				FileAccessPermissions fa = ufi.FileAccessPermissions;
				return (FileAccessPermissions)fa;
			} catch (Exception ex) {
				Log.Warning (ex);
				return (FileAccessPermissions)0;
			}
		}

		public override string PermissionsString (string path)
		{
			return formatPermissions (Permissions (path: path));
		}

		string formatPermissions (FileAccessPermissions p)
		{
			char ur = (p & FileAccessPermissions.UserRead) > 0 ? 'r' : '-';
			char uw = (p & FileAccessPermissions.UserWrite) > 0 ? 'w' : '-';
			char ux = (p & FileAccessPermissions.UserExecute) > 0 ? 'x' : '-';
			char gr = (p & FileAccessPermissions.GroupRead) > 0 ? 'r' : '-';
			char gw = (p & FileAccessPermissions.GroupWrite) > 0 ? 'w' : '-';
			char gx = (p & FileAccessPermissions.GroupExecute) > 0 ? 'x' : '-';
			char or = (p & FileAccessPermissions.OtherRead) > 0 ? 'r' : '-';
			char ow = (p & FileAccessPermissions.OtherWrite) > 0 ? 'w' : '-';
			char ox = (p & FileAccessPermissions.OtherExecute) > 0 ? 'x' : '-';
			string result = $"{ur}{uw}{ux}{gr}{gw}{gx}{or}{ow}{ox}";
			return result;
		}

		public override string GetOwnerName (string path)
		{
			return GetOwner_Linux (path)?.UserName ?? "invalid";
		}

		public UnixUserInfo GetOwner_Linux (string path)
		{
			try {
				var ufi = new UnixFileInfo (path);
				return ufi.OwnerUser;
			} catch (Exception ex) {
				Log.Warning (ex);
				return null;
			}
		}

		public override string GetGroupName (string path)
		{
			return GetGroup_Linux (path)?.GroupName ?? "invalid";
		}

		public UnixGroupInfo GetGroup_Linux (string path)
		{
			try {
				var ufi = new UnixFileInfo (path);
				return ufi.OwnerGroup;
			} catch (Exception ex) {
				Log.Warning (ex);
				return null;
			}
		}
	}
}
