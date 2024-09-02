using System;
using System.Text;
using static CrazyPanda.UnityCore.Utils.ExtensionsUtils;
namespace CrazyPanda.UnityCore.Utils
{
    public static class ExceptionExtensions
    {
        private const char StackTraceDelimiter = ';';

        public static string ToLongTitle( this Exception exception, int stackTraceLines = 0 )
        {
            if( exception == null )
            {
                return "Exception is null";
            }

            try
            {
                _stringBuilder.Clear();

                BuildTitle(exception, _stringBuilder);

                if (stackTraceLines > 0)
                {
                    int stackTraceLinesCounter = 0;
                    AddStackTraceLinesToTitle(exception, stackTraceLines, ref stackTraceLinesCounter, _stringBuilder);
                }

                string result = _stringBuilder.ToString();
                _stringBuilder.Clear();
                return result;
            }
            catch (Exception e)
            {
                return "Got exception while Exception.ToLongTitle(): " + e.GetType().Name + ": " + e.Message;
            }
        }

        private static void BuildTitle( Exception exception, StringBuilder stringBuilder )
        {
            const char spaceSymbol = ' ';
            
            stringBuilder.Append( exception.GetType().FullName );

            if( !string.IsNullOrEmpty( exception.Message ) )
            {
                stringBuilder.Append( ':' ).Append( spaceSymbol ).Append( exception.Message );
            }

            if( exception is AggregateException aggregateException )
            {
                stringBuilder.Append(spaceSymbol)
                    .Append("Total: ")
                    .AppendInt(aggregateException.InnerExceptions.Count)
                    .Append(':');

                for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
                {
                    stringBuilder.Append(spaceSymbol)
                        .Append($"[{i}] ---> ");
                    BuildTitle(aggregateException.InnerExceptions[i], stringBuilder);
                    stringBuilder.Append(";");
                }
            } 
            else if( exception.InnerException != null )
            {
                stringBuilder.Append( spaceSymbol ).Append( "--->" ).Append( spaceSymbol );
                BuildTitle( exception.InnerException, stringBuilder );
            }
        }

        private static void AddStackTraceLinesToTitle( Exception exception, int maxStackTraceLines, ref int stackTraceLinesCounter, StringBuilder stringBuilder )
        {
            if( stackTraceLinesCounter >= maxStackTraceLines )
            {
                return;
            }

            string stackTrace = exception.StackTrace;

            if( !string.IsNullOrEmpty( stackTrace ) )
            {
                if( stringBuilder[ stringBuilder.Length - 1 ] != StackTraceDelimiter && 
                   stringBuilder[ stringBuilder.Length - 2 ] != StackTraceDelimiter )
                {
                    stringBuilder.Append( StackTraceDelimiter );
                }
                
                for( int i = 0; i < stackTrace.Length; ++i )
                {
                    if( stackTraceLinesCounter >= maxStackTraceLines )
                    {
                        return;
                    }
                
                    char stackTraceItem = stackTrace[i];

                    if( stackTraceItem == '\r' )
                    {
                        continue;
                    }
                    
                    if( stackTraceItem == '\n' )
                    {
                        ++stackTraceLinesCounter;

                        if( stackTraceLinesCounter < maxStackTraceLines )
                        {
                            stringBuilder.Append( StackTraceDelimiter );
                        }

                        continue;
                    }

                    stringBuilder.Append( stackTrace[ i ] );
                }
            }
            
            if( exception.InnerException != null )
            {
                AddStackTraceLinesToTitle( exception.InnerException, maxStackTraceLines, ref stackTraceLinesCounter, stringBuilder );
            }
        }
    }
}