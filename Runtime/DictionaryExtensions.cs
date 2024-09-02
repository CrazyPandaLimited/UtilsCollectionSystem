using System;
using System.Collections.Generic;

namespace CrazyPanda.UnityCore.Utils
{
	public static class DictionaryExtensions
	{
		/// <summary>
		///     ��������� Action ��� ������� �������� �������
		/// </summary>
		public static void ForEach< TKey, TValue >( this IDictionary< TKey, TValue > items, Action< KeyValuePair< TKey, TValue > > action )
		{
			foreach( var item in items )
			{
				action( item );
			}
		}

		public static string ToPrettyString< TKey, TValue >( this IDictionary< TKey, TValue > source )
		{
			var result = "[";

			foreach( var v in source )
			{
				result += string.Format( "('{0}', '{1}'), ", v.Key, v.Value );
			}

			result += "]";
			return result.Replace( ", ]", "]" );
		}

		public static TValue GetValue< TKey, TValue >( this IDictionary< TKey, TValue > dictionary, TKey key )
		{
			TValue value;
			dictionary.TryGetValue( key, out value );
			return value;
		}


		public static int ValuesSum< TKey >( this IDictionary< TKey, int > dictionary )
		{
			var sum = 0;

			foreach( var pair in dictionary )
			{
				sum += pair.Value;
			}

			return sum;
		}

		public static float ValuesSum< TKey >( this IDictionary< TKey, float > dictionary )
		{
			float sum = 0;

			foreach( var pair in dictionary )
			{
				sum += pair.Value;
			}

			return sum;
		}

		public static double ValuesSum< TKey >( this IDictionary< TKey, double > dictionary )
		{
			double sum = 0;

			foreach( var pair in dictionary )
			{
				sum += pair.Value;
			}

			return sum;
		}

		public static bool EqualValues< TKey >( this IDictionary< TKey, int > dictionary1, IDictionary< TKey, int > dictionary2 )
		{
			if( dictionary1 == null && dictionary2 == null )
			{
				return true;
			}

			if( dictionary1 == null || dictionary2 == null )
			{
				return false;
			}

			foreach( var pair in dictionary1 )
			{
				if( !dictionary2.ContainsKey( pair.Key ) || dictionary2[ pair.Key ] != pair.Value )
				{
					return false;
				}
			}

			foreach( var pair in dictionary2 )
			{
				if( !dictionary1.ContainsKey( pair.Key ) || dictionary1[ pair.Key ] != pair.Value )
				{
					return false;
				}
			}

			return true;
		}

		public static bool EqualValues< TKey >( this IDictionary< TKey, float > dictionary1, IDictionary< TKey, float > dictionary2, float epsilon = float.Epsilon )
		{
			if( dictionary1 == null && dictionary2 == null )
			{
				return true;
			}

			if( dictionary1 == null || dictionary2 == null )
			{
				return false;
			}

			foreach( var pair in dictionary1 )
			{
				if( !dictionary2.ContainsKey( pair.Key ) )
				{
					return false;
				}
				else
				{
					var dif = dictionary1[ pair.Key ] - dictionary2[ pair.Key ];
					if( dif > epsilon || dif < -epsilon )
					{
						return false;
					}
				}
			}

			foreach( var pair in dictionary2 )
			{
				if( !dictionary1.ContainsKey( pair.Key ) )
				{
					return false;
				}
				else
				{
					var dif = dictionary1[ pair.Key ] - dictionary2[ pair.Key ];
					if( dif > epsilon || dif < -epsilon )
					{
						return false;
					}
				}
			}

			return true;
		}

		public static bool EqualValues< TKey >( this IDictionary< TKey, double > dictionary1, IDictionary< TKey, double > dictionary2, double epsilon = double.Epsilon )
		{
			if( dictionary1 == null && dictionary2 == null )
			{
				return true;
			}

			if( dictionary1 == null || dictionary2 == null )
			{
				return false;
			}

			foreach( var pair in dictionary1 )
			{
				if( !dictionary2.ContainsKey( pair.Key ) )
				{
					return false;
				}
				else
				{
					var dif = dictionary1[ pair.Key ] - dictionary2[ pair.Key ];
					if( dif > epsilon || dif < -epsilon )
					{
						return false;
					}
				}
			}

			foreach( var pair in dictionary2 )
			{
				if( !dictionary1.ContainsKey( pair.Key ) )
				{
					return false;
				}
				else
				{
					var dif = dictionary1[ pair.Key ] - dictionary2[ pair.Key ];
					if( dif > epsilon || dif < -epsilon )
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Subtracts subtrahend from minuend. If minuend doesn't have some key from subtrahend, method will check if default value isn't null. If it's null - throw exception, otherway add key with default value and subtract it
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="minuend"></param>
		/// <param name="subtrahend"></param>
		/// <param name="defaultValue"></param>
		public static void SubtractValues< TKey >( this IDictionary< TKey, int > minuend, IDictionary< TKey, int > subtrahend, int? defaultValue = null, bool removeKeyIfZero = false )
		{
			foreach( var pair in subtrahend )
			{
				if( defaultValue.HasValue && !minuend.ContainsKey( pair.Key ) )
				{
					minuend.Add( pair.Key, defaultValue.Value );
				}
				minuend[ pair.Key ] -= pair.Value;
				if( removeKeyIfZero && minuend[ pair.Key ] == 0 )
				{
					minuend.Remove( pair.Key );
				}
			}
		}

		/// <summary>
		/// Subtracts subtrahend from minuend. If minuend doesn't have some key from subtrahend, method will check if default value isn't null. If it's null - throw exception, otherway add key with default value and subtract it
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="minuend"></param>
		/// <param name="subtrahend"></param>
		/// <param name="defaultValue"></param>
		public static void SubtractValues< TKey >( this IDictionary< TKey, float > minuend, IDictionary< TKey, float > subtrahend, float? defaultValue = null )
		{
			foreach( var pair in subtrahend )
			{
				if( defaultValue.HasValue && !minuend.ContainsKey( pair.Key ) )
				{
					minuend.Add( pair.Key, defaultValue.Value );
				}
				minuend[ pair.Key ] -= pair.Value;
			}
		}

		/// <summary>
		/// Subtracts subtrahend from minuend. If minuend doesn't have some key from subtrahend, method will check if default value isn't null. If it's null - throw exception, otherway add key with default value and subtract it
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="minuend"></param>
		/// <param name="subtrahend"></param>
		/// <param name="defaultValue"></param>
		public static void SubtractValues< TKey >( this IDictionary< TKey, double > minuend, IDictionary< TKey, double > subtrahend, double? defaultValue = null )
		{
			foreach( var pair in subtrahend )
			{
				if( defaultValue.HasValue && !minuend.ContainsKey( pair.Key ) )
				{
					minuend.Add( pair.Key, defaultValue.Value );
				}
				minuend[ pair.Key ] -= pair.Value;
			}
		}

		public static void AddValues< TKey >( this IDictionary< TKey, int > term1, IDictionary< TKey, int > term2 )
		{
			foreach( var pair in term2 )
			{
				if( term1.ContainsKey( pair.Key ) )
				{
					term1[ pair.Key ] += pair.Value;
				}
				else
				{
					term1.Add( pair.Key, pair.Value );
				}
			}
		}

		public static void AddValues< TKey >( this IDictionary< TKey, float > term1, IDictionary< TKey, float > term2 )
		{
			foreach( var pair in term2 )
			{
				if( term1.ContainsKey( pair.Key ) )
				{
					term1[ pair.Key ] += pair.Value;
				}
				else
				{
					term1.Add( pair.Key, pair.Value );
				}
			}
		}

		public static void AddValues< TKey >( this IDictionary< TKey, double > term1, IDictionary< TKey, double > term2 )
		{
			foreach( var pair in term2 )
			{
				if( term1.ContainsKey( pair.Key ) )
				{
					term1[ pair.Key ] += pair.Value;
				}
				else
				{
					term1.Add( pair.Key, pair.Value );
				}
			}
		}
	}
}
