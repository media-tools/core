using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using Core.Common;

namespace Core.Google.Auth
{
	public class DictionaryDataStore : IDataStore
	{
		private Dictionary<string,string> Store = new Dictionary<string, string> ();

		public DictionaryDataStore ()
		{
		}

		/// <summary>
		/// Stores the given value for the given key. It creates a new file (named <see cref="GenerateStoredKey"/>) in 
		/// <see cref="FolderPath"/>.
		/// </summary>
		/// <typeparam name="T">The type to store in the data store.</typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value to store in the data store.</param>
		public Task StoreAsync<T> (string key, T value)
		{
			if (string.IsNullOrEmpty (key)) {
				throw new ArgumentException ("Key MUST have a value");
			}

			var serialized = NewtonsoftJsonSerializer.Instance.Serialize (value);
			var configKey = GenerateStoredKey (key, typeof(T));
			Store [configKey] = serialized;
			return TaskEx.Delay (0);
		}

		/// <summary>
		/// Deletes the given key. It deletes the <see cref="GenerateStoredKey"/> named file in 
		/// <see cref="FolderPath"/>.
		/// </summary>
		/// <param name="key">The key to delete from the data store.</param>
		public Task DeleteAsync<T> (string key)
		{
			if (string.IsNullOrEmpty (key)) {
				throw new ArgumentException ("Key MUST have a value");
			}

			var configKey = GenerateStoredKey (key, typeof(T));
			Store.Remove (configKey);
			return TaskEx.Delay (0);
		}

		/// <summary>
		/// Returns the stored value for the given key or <c>null</c> if the matching file (<see cref="GenerateStoredKey"/>
		/// in <see cref="FolderPath"/> doesn't exist.
		/// </summary>
		/// <typeparam name="T">The type to retrieve.</typeparam>
		/// <param name="key">The key to retrieve from the data store.</param>
		/// <returns>The stored object.</returns>
		public Task<T> GetAsync<T> (string key)
		{
			if (string.IsNullOrEmpty (key)) {
				throw new ArgumentException ("Key MUST have a value");
			}

			TaskCompletionSource<T> tcs = new TaskCompletionSource<T> ();
			var configKey = GenerateStoredKey (key, typeof(T));
			if (Store.ContainsKey (configKey)) {
				try {
					var configValue = Store [configKey];
					tcs.SetResult (NewtonsoftJsonSerializer.Instance.Deserialize<T> (configValue));
				} catch (Exception ex) {
					tcs.SetException (ex);
				}
			} else {
				tcs.SetResult (default(T));
			}
			return tcs.Task;
		}

		/// <summary>
		/// Clears all values in the data store. This method deletes all files in <see cref="FolderPath"/>.
		/// </summary>
		public Task ClearAsync ()
		{
			Store.Clear ();

			return TaskEx.Delay (0);
		}

		public void Save (Dictionary<string,string> dictionary)
		{
			foreach (var pair in Store) {
				dictionary ["store_" + pair.Key] = pair.Value;
			}
			dictionary ["store_keys"] = Store.Keys.Join (";");
		}

		public void Load (Dictionary<string,string> dictionary)
		{
			string[] keys = dictionary ["store_keys"].Split (';');
			foreach (string key in keys) {
				Store [key] = dictionary ["store_" + key];
			}
		}

		/// <summary>Creates a unique stored key based on the key and the class type.</summary>
		/// <param name="key">The object key.</param>
		/// <param name="t">The type to store or retrieve.</param>
		public static string GenerateStoredKey (string key, Type t)
		{
			return string.Format ("{0}-{1}", t.FullName, key);
		}
	}
}

