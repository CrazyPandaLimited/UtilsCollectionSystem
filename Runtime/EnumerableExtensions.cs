using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     "Добавляет" элемент к перечислению. Как Concat, но для одного элемента.
        /// </summary>
        public static IEnumerable< T > Append< T >( this IEnumerable< T > source, T element )
        {
            foreach( var e in source ) yield return e;
            yield return element;
        }

        /// <summary>
        ///     "Добавляет" элемент в начало перечисления.
        /// </summary>
        public static IEnumerable< T > Prepend< T >( this IEnumerable< T > source, T element )
        {
            yield return element;
            foreach( var e in source ) yield return e;
        }

        /// <summary>
        ///     Показывает IEnumerable в виде [элемент1, элемент2]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ShowAsString< T >( this IEnumerable< T > source )
        {
            return ShowAsString( source, ",", "[", "]" );
        }

        /// <summary>
        ///     Строит строку из IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ShowAsString< T >( this IEnumerable< T > source, string sep, string bgn = null, string end = null )
        {
            if( source == null )
            {
                return null;
            }
            var result = new StringBuilder();
            if( bgn != null )
            {
                result.Append( bgn );
            }
            var it = source.GetEnumerator();
            if( it.MoveNext() )
            {
                result.Append( it.Current );
                while( it.MoveNext() ) result.Append( sep ).Append( it.Current );
            }
            if( end != null )
            {
                result.Append( end );
            }
            return result.ToString();
        }

        public static T GetRandomElement< T >( this IEnumerable< T > source )
        {
            if( source == null || !source.Any() )
                return default( T );
            
            return source.ElementAt( new Random().Next( 0, source.Count() ) );
        }
        
        /// <summary>
        /// Validate collection. Must be not null and has no null elements.
        /// </summary>
        /// <typeparam name="T">collection type</typeparam>
        /// <param name="collection">collection ref</param>
        /// <param name="name">collection name</param>
        /// <returns>same collection</returns>
        /// <exception cref="ArgumentNullException">collection is null</exception>
        /// <exception cref="ArgumentException">one of elements is null</exception>
        public static IEnumerable< T > ValidateCollection< T >( this IEnumerable< T > collection, string name )
            where T : class
        {
            collection.CheckArgumentForNull( name );
            
            foreach( T value in collection )
            {
                if( ReferenceEquals( value, null ) )
                {
                    throw new ArgumentException( @"One of element is null", name );
                }
            }

            return collection;
        }
    }
}
