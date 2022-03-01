using System;
using System.Text;
using System.Collections.Generic;

namespace TextEditorFormApp.Util
{
	public static class Extensions
	{
		public static T[] InsertArray<T>(this T[] array, int start, T[] array1)
		{
			List<T> c = new();

			for (int i = 0; i < start; i++)
			{
				c.Add(array[i]);
			}

			for (int i = 0; i < array1.Length; i++)
			{
				c.Add(array1[i]);
			}

			for (int i = start; i < array.Length; i++)
			{
				c.Add(array[i]);
			}

			return c.ToArray();
		}

		public static T[] CutArray<T>(this T[] array, int start, int end)
		{
			List<T> c = new();

			if (array.Length >= (end - start))
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i < start || i > end)
					{
						c.Add(array[i]);
					}
				}

				return c.ToArray();
			}

			return array;
		}

		public static string[] MySplit(this string s, params char[] delimiters)
		{
			List<string> elements = new();
			if (s.IsNullOrEmpty())
				return elements.ToArray();
			char[] arr = s.ToCharArray();
			string substring;
			int start = 0;

			for (int i = 0; i < arr.Length; i++)
			{
				for (int j = 0; j < delimiters.Length; j++)
				{
					if (arr[i] == delimiters[j])
					{
						int len = i - start;
						substring = s.Substring(start, len);
						if (substring.Length != 0)
						{
							elements.Add(substring);
						}
						start = i + 1;
					}
				}
			}
			substring = s[start..];
			if (substring.Length != 0)
			{
				elements.Add(substring);
			}
			return elements.ToArray();
		}

		public static int Occurences(this string s, char target)
		{
			int value = 0;

			foreach (char c in s.ToCharArray())
			{
				if (c.Equals(target))
				{
					value++;
				}
			}

			return value;
		}

		public static int Occurences<T>(this IEnumerable<T> ie, Predicate<T> predicate)
		{
			int value = 0;

			foreach (T element in ie)
			{
				if (predicate(element))
				{
					value++;
				}
			}

			return value;
		}

		public static string PathToFileName(this string path)
		{
			string[] split = path.Split("\\");

			if (split.Length > 0)
			{
				return split[^1];
			}
			else
			{
				return split[0];
			}
		}

		public static string GetDirectory(this string path)
		{
			string[] src = path.Split("\\");

			if (src.Length > 1)
			{
				string[] dest = new string[src.Length - 1];
				System.Array.Copy(src, dest, dest.Length);
				return string.Join("\\", dest);
			}

			return null;
		}

		public static string ReplaceFirst(this string s, char target)
		{
			char[] array = s.ToCharArray();
			int index = s.IndexOf(target);

			if (index != -1)
			{
				var builder = new StringBuilder();

				for (int i = 0; i < array.Length; i++)
				{
					if (i == index)
					{
						continue;
					}

					builder.Append(array[i]);
				}

				return builder.ToString();
			}
			else
			{
				return s;
			}
		}

		public static bool IsNullOrEmpty(this string s)
		{
			return (s == null) || (s.Equals(string.Empty));
		}

		public static bool IsNotNullOrEmpty(this string s)
		{
			return (s != null) && !(s.Equals(string.Empty));
		}
	}
}
