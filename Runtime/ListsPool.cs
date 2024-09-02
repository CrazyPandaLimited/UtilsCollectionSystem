using System;
using System.Collections.Generic;

namespace CrazyPanda.UnityCore.Utils
{
    public static class ListsPool< TComponent >
    {
        private static readonly Stack< List< TComponent > > _poolStack = new();

        /// <summary>
        /// Get list from pool.
        /// </summary>
        /// <returns>new or pooled list</returns>
        public static List< TComponent > UseList() => _poolStack.Count != 0 ? _poolStack.Pop() : new List< TComponent >();

        /// <summary>
        /// Release list from using.
        /// </summary>
        /// <param name="components">list.</param>
        public static void ReleaseList( List< TComponent > components )
        {
            if( components == null )
            {
                throw new ArgumentNullException( nameof( components ) );
            }

            components.Clear();
            _poolStack.Push( components );
        }

        /// <summary>
        /// Start pool transaction.
        /// </summary>
        /// <param name="list">list of components.</param>
        /// <returns>pool use transaction.</returns>
        public static ListDispose CreateTransaction( out List< TComponent > list )
        {
            list = UseList();
            return new ListDispose( list );
        }

        /// <summary>
        /// Start pool transaction.
        /// </summary>
        /// <param name="initData">init data for </param>
        /// <param name="list">list of components.</param>
        /// <returns>pool use transaction.</returns>
        public static ListDispose CreateTransaction( IEnumerable< TComponent > initData, out List< TComponent > list )
        {
            list = UseList();

            if( initData != null )
            {
                list.AddRange( initData );
            }

            return new ListDispose( list );
        }

        /// <summary>
        /// Start pool transaction.
        /// </summary>
        /// <param name="initData">init data for </param>
        /// <param name="list">list of components.</param>
        /// <returns>pool use transaction.</returns>
        public static ListDispose CreateTransaction( TComponent[] initData, out List< TComponent > list )
        {
            list = UseList();

            if( initData != null )
            {
                foreach( var item in initData )
                {
                    list.Add( item );
                }
            }

            return new ListDispose( list );
        }

        public static ListDispose CreateTransaction()
        {
            List< TComponent > list = UseList();
            return new ListDispose( list );
        }

        public static ListDispose CreateTransaction( TComponent[] initData )
        {
            List< TComponent > list = UseList();
            if( initData != null )
            {
                foreach( var item in initData )
                {
                    list.Add( item );
                }
            }

            return new ListDispose( list );
        }

        public readonly struct ListDispose : IDisposable
        {
            public readonly List< TComponent > List;
            public ListDispose( List< TComponent > list ) => List = list;
            public void Add( TComponent val ) => List.Add( val );
            public void Dispose() => ReleaseList( List );

            public List< TComponent >.Enumerator GetEnumerator() => List.GetEnumerator();

            public static implicit operator List< TComponent >( ListDispose val ) => val.List;
        }
    }
}
