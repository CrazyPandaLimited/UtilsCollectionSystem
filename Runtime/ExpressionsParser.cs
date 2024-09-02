// http://wiki.unity3d.com/index.php/ExpressionParser

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CrazyPanda.UnityCore.Utils.ExpressionParser
{
	public interface IValue
	{
		double Value { get; }
	}

	public class Number : IValue
	{
		private double m_Value;

		public double Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

		public Number( double aValue )
		{
			m_Value = aValue;
		}

		public override string ToString()
		{
			return "" + m_Value + "";
		}
	}

	public class OperationSum : IValue
	{
		private IValue[ ] m_Values;

		public double Value
		{
			get { return m_Values.Select( v => v.Value ).Sum(); }
		}

		public OperationSum( params IValue[ ] aValues )
		{
			// collapse unnecessary nested sum operations.
			var v = new List< IValue >( aValues.Length );
			foreach( var I in aValues )
			{
				var sum = I as OperationSum;
				if( sum == null )
				{
					v.Add( I );
				}
				else
				{
					v.AddRange( sum.m_Values );
				}
			}
			m_Values = v.ToArray();
		}

		public override string ToString()
		{
			return "( " + string.Join( " + ", m_Values.Select( v => v.ToString() ).ToArray() ) + " )";
		}
	}

	public class OperationProduct : IValue
	{
		private IValue[ ] m_Values;

		public double Value
		{
			get { return m_Values.Select( v => v.Value ).Aggregate( ( v1, v2 ) => v1 * v2 ); }
		}

		public OperationProduct( params IValue[ ] aValues )
		{
			m_Values = aValues;
		}

		public override string ToString()
		{
			return "( " + string.Join( " * ", m_Values.Select( v => v.ToString() ).ToArray() ) + " )";
		}
	}

	public class OperationPower : IValue
	{
		private IValue m_Value;
		private IValue m_Power;

		public double Value
		{
			get { return Math.Pow( m_Value.Value, m_Power.Value ); }
		}

		public OperationPower( IValue aValue, IValue aPower )
		{
			m_Value = aValue;
			m_Power = aPower;
		}

		public override string ToString()
		{
			return "( " + m_Value + "^" + m_Power + " )";
		}
	}

	public class OperationNegate : IValue
	{
		private IValue m_Value;

		public double Value
		{
			get { return -m_Value.Value; }
		}

		public OperationNegate( IValue aValue )
		{
			m_Value = aValue;
		}

		public override string ToString()
		{
			return "( -" + m_Value + " )";
		}
	}

	public class OperationReciprocal : IValue
	{
		private IValue m_Value;

		public double Value
		{
			get { return 1.0 / m_Value.Value; }
		}

		public OperationReciprocal( IValue aValue )
		{
			m_Value = aValue;
		}

		public override string ToString()
		{
			return "( 1/" + m_Value + " )";
		}
	}

	public class MultiParameterList : IValue
	{
		private IValue[ ] m_Values;

		public IValue[ ] Parameters
		{
			get { return m_Values; }
		}

		public double Value
		{
			get { return m_Values.Select( v => v.Value ).FirstOrDefault(); }
		}

		public MultiParameterList( params IValue[ ] aValues )
		{
			m_Values = aValues;
		}

		public override string ToString()
		{
			return string.Join( ", ", m_Values.Select( v => v.ToString() ).ToArray() );
		}
	}

	public class CustomFunction : IValue
	{
		private IValue[ ] m_Params;
		private Func< double[ ], double > m_Delegate;
		private string m_Name;

		public double Value
		{
			get
			{
				if( m_Params == null )
				{
					return m_Delegate( null );
				}
				return m_Delegate( m_Params.Select( p => p.Value ).ToArray() );
			}
		}

		public CustomFunction( string aName, Func< double[ ], double > aDelegate, params IValue[ ] aValues )
		{
			m_Delegate = aDelegate;
			m_Params = aValues;
			m_Name = aName;
		}

		public override string ToString()
		{
			if( m_Params == null )
			{
				return m_Name;
			}
			return m_Name + "( " + string.Join( ", ", m_Params.Select( v => v.ToString() ).ToArray() ) + " )";
		}
	}

	public class Parameter : Number
	{
		public string Name { get; private set; }

		public Parameter( string aName ) : base( 0 )
		{
			Name = aName;
		}

		public override string ToString()
		{
			return Name + "[" + base.ToString() + "]";
		}
	}

	public class Expression : IValue
	{
		public Dictionary< string, Parameter > Parameters = new Dictionary< string, Parameter >();

		public IValue ExpressionTree { get; set; }

		public double Value
		{
			get { return ExpressionTree.Value; }
		}

		public double[ ] MultiValue
		{
			get
			{
				var t = ExpressionTree as MultiParameterList;
				if( t != null )
				{
					var res = new double[ t.Parameters.Length ];
					for( var i = 0; i < res.Length; i++ )
					{
						res[ i ] = t.Parameters[ i ].Value;
					}
					return res;
				}
				return null;
			}
		}

		public override string ToString()
		{
			return ExpressionTree.ToString();
		}

		public ExpressionDelegate ToDelegate( params string[ ] aParamOrder )
		{
			var parameters = new List< Parameter >( aParamOrder.Length );
			for( var i = 0; i < aParamOrder.Length; i++ )
			{
				if( Parameters.ContainsKey( aParamOrder[ i ] ) )
				{
					parameters.Add( Parameters[ aParamOrder[ i ] ] );
				}
				else
				{
					parameters.Add( null );
				}
			}
			var parameters2 = parameters.ToArray();

			return p => Invoke( p, parameters2 );
		}

		public MultiResultDelegate ToMultiResultDelegate( params string[ ] aParamOrder )
		{
			var parameters = new List< Parameter >( aParamOrder.Length );
			for( var i = 0; i < aParamOrder.Length; i++ )
			{
				if( Parameters.ContainsKey( aParamOrder[ i ] ) )
				{
					parameters.Add( Parameters[ aParamOrder[ i ] ] );
				}
				else
				{
					parameters.Add( null );
				}
			}
			var parameters2 = parameters.ToArray();


			return p => InvokeMultiResult( p, parameters2 );
		}

		public static Expression Parse( string aExpression )
		{
			return new ExpressionParser().EvaluateExpression( aExpression );
		}

		double Invoke( double[ ] aParams, Parameter[ ] aParamList )
		{
			var count = Math.Min( aParamList.Length, aParams.Length );
			for( var i = 0; i < count; i++ )
			{
				if( aParamList[ i ] != null )
				{
					aParamList[ i ].Value = aParams[ i ];
				}
			}
			return Value;
		}

		double[ ] InvokeMultiResult( double[ ] aParams, Parameter[ ] aParamList )
		{
			var count = Math.Min( aParamList.Length, aParams.Length );
			for( var i = 0; i < count; i++ )
			{
				if( aParamList[ i ] != null )
				{
					aParamList[ i ].Value = aParams[ i ];
				}
			}
			return MultiValue;
		}

		public class ParameterException : Exception
		{
			public ParameterException( string aMessage ) : base( aMessage )
			{
			}
		}
	}

	public delegate double ExpressionDelegate( params double[ ] aParams );

	public delegate double[ ] MultiResultDelegate( params double[ ] aParams );

	public class ExpressionParser
	{
		private List< string > m_BracketHeap = new List< string >();
		private Dictionary< string, Func< double > > m_Consts = new Dictionary< string, Func< double > >();
		private Dictionary< string, Func< double[ ], double > > m_Funcs = new Dictionary< string, Func< double[ ], double > >();
		private Expression m_Context;

		public ExpressionParser()
		{
			var rnd = new Random();
			m_Consts.Add( "PI", () => Math.PI );
			m_Consts.Add( "e", () => Math.E );
			m_Funcs.Add( "sqrt", p => Math.Sqrt( p.FirstOrDefault() ) );
			m_Funcs.Add( "abs", p => Math.Abs( p.FirstOrDefault() ) );
			m_Funcs.Add( "ln", p => Math.Log( p.FirstOrDefault() ) );
			m_Funcs.Add( "floor", p => Math.Floor( p.FirstOrDefault() ) );
			m_Funcs.Add( "ceiling", p => Math.Ceiling( p.FirstOrDefault() ) );
			m_Funcs.Add( "ceil", p => Math.Ceiling( p.FirstOrDefault() ) );
			m_Funcs.Add( "round", p => Math.Round( p.FirstOrDefault() ) );

			m_Funcs.Add( "sin", p => Math.Sin( p.FirstOrDefault() ) );
			m_Funcs.Add( "cos", p => Math.Cos( p.FirstOrDefault() ) );
			m_Funcs.Add( "tan", p => Math.Tan( p.FirstOrDefault() ) );

			m_Funcs.Add( "asin", p => Math.Asin( p.FirstOrDefault() ) );
			m_Funcs.Add( "acos", p => Math.Acos( p.FirstOrDefault() ) );
			m_Funcs.Add( "atan", p => Math.Atan( p.FirstOrDefault() ) );
			m_Funcs.Add( "atan2", p => Math.Atan2( p.FirstOrDefault(), p.ElementAtOrDefault( 1 ) ) );

			//System.Math.Floor
			m_Funcs.Add( "min", p => Math.Min( p.FirstOrDefault(), p.ElementAtOrDefault( 1 ) ) );
			m_Funcs.Add( "max", p => Math.Max( p.FirstOrDefault(), p.ElementAtOrDefault( 1 ) ) );
			m_Funcs.Add( "rnd", p =>
			{
				if( p.Length == 2 )
				{
					return p[ 0 ] + rnd.NextDouble() * ( p[ 1 ] - p[ 0 ] );
				}
				if( p.Length == 1 )
				{
					return rnd.NextDouble() * p[ 0 ];
				}
				return rnd.NextDouble();
			} );
		}

		public void AddFunc( string aName, Func< double[ ], double > aMethod )
		{
			if( m_Funcs.ContainsKey( aName ) )
			{
				m_Funcs[ aName ] = aMethod;
			}
			else
			{
				m_Funcs.Add( aName, aMethod );
			}
		}

		public void AddConst( string aName, Func< double > aMethod )
		{
			if( m_Consts.ContainsKey( aName ) )
			{
				m_Consts[ aName ] = aMethod;
			}
			else
			{
				m_Consts.Add( aName, aMethod );
			}
		}

		public void RemoveFunc( string aName )
		{
			if( m_Funcs.ContainsKey( aName ) )
			{
				m_Funcs.Remove( aName );
			}
		}

		public void RemoveConst( string aName )
		{
			if( m_Consts.ContainsKey( aName ) )
			{
				m_Consts.Remove( aName );
			}
		}

		public Expression EvaluateExpression( string aExpression )
		{
			var val = new Expression();
			m_Context = val;
			val.ExpressionTree = Parse( aExpression );
			m_Context = null;
			m_BracketHeap.Clear();
			return val;
		}

		public double Evaluate( string aExpression )
		{
			return EvaluateExpression( aExpression ).Value;
		}

		public static double Eval( string aExpression )
		{
			return new ExpressionParser().Evaluate( aExpression );
		}

		int FindClosingBracket( ref string aText, int aStart, char aOpen, char aClose )
		{
			var counter = 0;
			for( var i = aStart; i < aText.Length; i++ )
			{
				if( aText[ i ] == aOpen )
				{
					counter++;
				}
				if( aText[ i ] == aClose )
				{
					counter--;
				}
				if( counter == 0 )
				{
					return i;
				}
			}
			return -1;
		}

		void SubstitudeBracket( ref string aExpression, int aIndex )
		{
			var closing = FindClosingBracket( ref aExpression, aIndex, '(', ')' );
			if( closing > aIndex + 1 )
			{
				var inner = aExpression.Substring( aIndex + 1, closing - aIndex - 1 );
				m_BracketHeap.Add( inner );
				var sub = "&" + ( m_BracketHeap.Count - 1 ) + ";";
				aExpression = aExpression.Substring( 0, aIndex ) + sub + aExpression.Substring( closing + 1 );
			}
			else
			{
				throw new ParseException( "Bracket not closed!" );
			}
		}

		IValue Parse( string aExpression )
		{
			aExpression = aExpression.Trim();
			var index = aExpression.IndexOf( '(' );
			while( index >= 0 )
			{
				SubstitudeBracket( ref aExpression, index );
				index = aExpression.IndexOf( '(' );
			}
			if( aExpression.Contains( ',' ) )
			{
				var parts = aExpression.Split( ',' );
				var exp = new List< IValue >( parts.Length );
				for( var i = 0; i < parts.Length; i++ )
				{
					var s = parts[ i ].Trim();
					if( !string.IsNullOrEmpty( s ) )
					{
						exp.Add( Parse( s ) );
					}
				}
				return new MultiParameterList( exp.ToArray() );
			}
			if( aExpression.Contains( '+' ) )
			{
				var parts = aExpression.Split( '+' );
				var exp = new List< IValue >( parts.Length );
				for( var i = 0; i < parts.Length; i++ )
				{
					var s = parts[ i ].Trim();
					if( !string.IsNullOrEmpty( s ) )
					{
						exp.Add( Parse( s ) );
					}
				}
				if( exp.Count == 1 )
				{
					return exp[ 0 ];
				}
				return new OperationSum( exp.ToArray() );
			}
			if( aExpression.Contains( '-' ) )
			{
				var parts = aExpression.Split( '-' );
				var exp = new List< IValue >( parts.Length );
				if( !string.IsNullOrEmpty( parts[ 0 ].Trim() ) )
				{
					exp.Add( Parse( parts[ 0 ] ) );
				}
				for( var i = 1; i < parts.Length; i++ )
				{
					var s = parts[ i ].Trim();
					if( !string.IsNullOrEmpty( s ) )
					{
						exp.Add( new OperationNegate( Parse( s ) ) );
					}
				}
				if( exp.Count == 1 )
				{
					return exp[ 0 ];
				}
				return new OperationSum( exp.ToArray() );
			}
			if( aExpression.Contains( '*' ) )
			{
				var parts = aExpression.Split( '*' );
				var exp = new List< IValue >( parts.Length );
				for( var i = 0; i < parts.Length; i++ )
				{
					exp.Add( Parse( parts[ i ] ) );
				}
				if( exp.Count == 1 )
				{
					return exp[ 0 ];
				}
				return new OperationProduct( exp.ToArray() );
			}
			if( aExpression.Contains( '/' ) )
			{
				var parts = aExpression.Split( '/' );
				var exp = new List< IValue >( parts.Length );
				if( !string.IsNullOrEmpty( parts[ 0 ].Trim() ) )
				{
					exp.Add( Parse( parts[ 0 ] ) );
				}
				for( var i = 1; i < parts.Length; i++ )
				{
					var s = parts[ i ].Trim();
					if( !string.IsNullOrEmpty( s ) )
					{
						exp.Add( new OperationReciprocal( Parse( s ) ) );
					}
				}
				return new OperationProduct( exp.ToArray() );
			}
			if( aExpression.Contains( '^' ) )
			{
				var pos = aExpression.IndexOf( '^' );
				var val = Parse( aExpression.Substring( 0, pos ) );
				var pow = Parse( aExpression.Substring( pos + 1 ) );
				return new OperationPower( val, pow );
			}
			var pPos = aExpression.IndexOf( "&" );
			if( pPos > 0 )
			{
				var fName = aExpression.Substring( 0, pPos );
				foreach( var M in m_Funcs )
				{
					if( fName == M.Key )
					{
						var inner = aExpression.Substring( M.Key.Length );
						var param = Parse( inner );
						var multiParams = param as MultiParameterList;
						IValue[ ] parameters;
						if( multiParams != null )
						{
							parameters = multiParams.Parameters;
						}
						else
						{
							parameters = new[ ]
							{
								param
							};
						}
						return new CustomFunction( M.Key, M.Value, parameters );
					}
				}
			}
			foreach( var C in m_Consts )
			{
				if( aExpression == C.Key )
				{
					return new CustomFunction( C.Key, p => C.Value(), null );
				}
			}
			var index2a = aExpression.IndexOf( '&' );
			var index2b = aExpression.IndexOf( ';' );
			if( index2a >= 0 && index2b >= 2 )
			{
				var inner = aExpression.Substring( index2a + 1, index2b - index2a - 1 );
				int bracketIndex;
				if( int.TryParse( inner, out bracketIndex ) && bracketIndex >= 0 && bracketIndex < m_BracketHeap.Count )
				{
					return Parse( m_BracketHeap[ bracketIndex ] );
				}
				throw new ParseException( "Can't parse substitude token" );
			}
			double doubleValue;
			if( double.TryParse( aExpression, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue ) )
			{
				return new Number( doubleValue );
			}
			if( ValidIdentifier( aExpression ) )
			{
				if( m_Context.Parameters.ContainsKey( aExpression ) )
				{
					return m_Context.Parameters[ aExpression ];
				}
				var val = new Parameter( aExpression );
				m_Context.Parameters.Add( aExpression, val );
				return val;
			}

			throw new ParseException( "Reached unexpected end within the parsing tree" );
		}

		private bool ValidIdentifier( string aExpression )
		{
			aExpression = aExpression.Trim();
			if( string.IsNullOrEmpty( aExpression ) )
			{
				return false;
			}
			if( aExpression.Contains( " " ) )
			{
				return false;
			}
			if( !"abcdefghijklmnopqrstuvwxyz§$".Contains( char.ToLowerInvariant( aExpression[ 0 ] ) ) )
			{
				return false;
			}
			if( m_Consts.ContainsKey( aExpression ) )
			{
				return false;
			}
			if( m_Funcs.ContainsKey( aExpression ) )
			{
				return false;
			}
			return true;
		}

		public class ParseException : Exception
		{
			public ParseException( string aMessage ) : base( aMessage )
			{
			}
		}
	}
}

/*

Examples:

var parser = new ExpressionParser();
Expression exp = parser.EvaluateExpression("(5+3)*8^2-5*(-2)");
Debug.Log("Result: " + exp.Value);  // prints: "Result: 522"
 
Debug.Log("Result: " + parser.Evaluate("ln(e^5)"));  // prints: "Result: 5"
 
 
// unknown identifiers are simply translated into parameters:
 
Expression exp2 = parser.EvaluateExpression("sin(x*PI/180)");
exp2.Parameters["x"].Value = 45; // set the named parameter "x"
Debug.Log("sin(45°): " + exp2.Value); // prints "sin(45°): 0.707106781186547" 
 
 
// convert the expression into a delegate:
 
var sinFunc = exp2.ToDelegate("x");
Debug.Log("sin(90°): " + sinFunc(90)); // prints "sin(90°): 1" 
 
 
// It's possible to return multiple values, but it generates extra garbage for each call due to the return array:
 
var exp3 = parser.EvaluateExpression("sin(angle/180*PI) * length,cos(angle/180*PI) * length");
var f = exp3.ToMultiResultDelegate("angle", "length");
double[] res = f(45,2);  // res contains [1.41421356237309, 1.4142135623731]
 
 
// To add custom functions to the parser, you just have to add it before parsing an expression:
 
parser.AddFunc("test", (p) => {
    Debug.Log("TEST: " + p.Length);
    return 42;
});
Debug.Log("Result: "+parser.Evaluate("2*test(1,5)")); // prints "TEST: 2" and "Result: 84"
 
 
// NOTE: functions without parameters are not supported, use "constants" instead:
parser.AddConst("meaningOfLife", () => 42);
Console.WriteLine("Result: " + parser.Evaluate("2*meaningOfLife")); // prints "Result: 84"
 
 */
