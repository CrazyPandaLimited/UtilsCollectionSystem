using System;
using System.IO;

namespace CrazyPanda.UnityCore.Utils
{
	public static class PathUtils
	{
		/// <summary>
		/// Заменяет '\' на '/' и убирает дублирующиеся разделители
		/// </summary>
		public static string Normalize(string path)
		{
			if (path.IsBlank())
				return "";
			path = path.Replace("\\", "/");
			string prevPath;
			do
			{
				prevPath = path;
				path = prevPath.Replace("//", "/");
			} while (prevPath != path);
			return path;
		}

		/// <summary>
		/// Создаёт путь к path относительно root. Если path и так относительный, то возвращает его самого.
		/// Если root не абсолютный, генерит исключение ArgumentException.
		/// </summary>
		public static string MakeRelative(string root, string path)
		{
			if (!Path.IsPathRooted(root))
				throw new ArgumentException("root path is not absolute : '" + root + "'");
			var relativePath = Normalize(path);
			if (Path.IsPathRooted(relativePath))
			{
				var pathUri = new Uri(Normalize(path), UriKind.Absolute);
				var rootUri = new Uri(Normalize(root) + "/fake", UriKind.Absolute);
				relativePath = rootUri.MakeRelativeUri(pathUri).ToString();
			}
			return relativePath;
		}

		/// <summary>
		/// Создаёт из относительного пути абсолютный. Если исходный путь абсолютный, возвращает его самого.
		/// </summary>
		public static string MakeFull(string root, string path)
		{
			var normalized = Normalize(path);
			return Path.IsPathRooted(normalized) ? normalized : Path.Combine(Normalize(root), normalized);
		}

		/// <summary>
		/// Собирает путь из кусков пропуская пустые строки и нормализуя результат
		/// </summary>
		public static string Combine(params string[] chunks)
		{
			if (chunks == null || chunks.Length == 0)
				return "";
			string rv = null;
			foreach (var chunk in chunks)
			{
				if (!string.IsNullOrEmpty(chunk))
					rv = !string.IsNullOrEmpty(rv) ? Path.Combine(rv, chunk) : chunk;
			}
			return Normalize(rv);
		}

		/// <summary>
		/// Заменить расширение файла в указанном пути
		/// </summary>
		public static string ReplaceExtension(string file, string ext)
		{
			ext = ext.TrimStart('.');
			var idx = file.LastIndexOf('.');
			return ((idx != -1) ? file.Substring(0, idx) : file) + (ext.IsNotBlank() ? '.' + ext : "");
		}

		/// <summary>
		/// Ищет в пути первую с конца существующую директорию
		/// </summary>
		public static string GetExistingDirectoryName(string path)
		{
			while (path.IsNotEmpty() && !Directory.Exists(path))
				path = Path.GetDirectoryName(path);
			return path;
		}
	}
}
