#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;
using CrazyPanda.UnityCore.Utils.GUIHelper;
using CrazyPanda.UnityCore.Utils.Reflection;
using UnityObject = UnityEngine.Object;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
	public class EditorGUIHelper
	{
		public static readonly Color lightBlue = new Color( 0.8f, 0.8f, 1 );
		public static readonly Color lightOrange = new Color( 1, 0.9f, 0.4f );
		public static readonly Color lightRed = new Color( 1, 0.5f, 0.5f, 0.8f );

		private static readonly List< int > _layerNumbers = new List< int >();
		private static readonly Dictionary< object, bool > _registeredEditorFoldouts = new Dictionary< object, bool >();
		private static readonly Dictionary< Color, Texture2D > _textures = new Dictionary< Color, Texture2D >();
		private static Texture2D _tex;

		private static Texture2D tex
		{
			get
			{
				if( _tex == null )
				{
					_tex = new Texture2D( 1, 1 );
					_tex.hideFlags = HideFlags.HideAndDontSave;
				}
				return _tex;
			}
		}

		///Get a colored 1x1 texture
		public static Texture2D GetTexture( Color color )
		{
			if( _textures.ContainsKey( color ) )
			{
				return _textures[ color ];
			}

			var newTexture = new Texture2D( 1, 1 );
			newTexture.SetPixel( 0, 0, color );
			newTexture.Apply();
			_textures[ color ] = newTexture;
			return newTexture;
		}

		//a cool label :-P (for headers)
		public static void CoolLabel( string text )
		{
			GUI.skin.label.richText = true;
			GUI.color = lightOrange;
			GUILayout.Label( "<b><size=14>" + text + "</size></b>" );
			GUI.color = Color.white;
			GUILayout.Space( 2 );
		}

		//a thin separator
		public static void Separator()
		{
			GUI.backgroundColor = Color.black;
			GUILayout.Box( "", GUILayout.MaxWidth( Screen.width ), GUILayout.Height( 2 ) );
			GUI.backgroundColor = Color.white;
		}

		//A thick separator similar to ngui. Thanks
		public static void BoldSeparator()
		{
			var lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space( 14 );
			GUI.color = new Color( 0, 0, 0, 0.25f );
			GUI.DrawTexture( new Rect( 0, lastRect.yMax + 6, Screen.width, 4 ), tex );
			GUI.DrawTexture( new Rect( 0, lastRect.yMax + 6, Screen.width, 1 ), tex );
			GUI.DrawTexture( new Rect( 0, lastRect.yMax + 9, Screen.width, 1 ), tex );
			GUI.color = Color.white;
		}

		//Combines the rest functions for a header style label
		public static void TitledSeparator( string title )
		{
			GUILayout.Space( 1 );
			BoldSeparator();
			CoolLabel( title + " ▼" );
			Separator();
		}

		//Just a fancy ending for inspectors
		public static void EndOfInspector()
		{
			var lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space( 8 );
			GUI.color = new Color( 0, 0, 0, 0.4f );
			GUI.DrawTexture( new Rect( 0, lastRect.yMax + 6, Screen.width, 4 ), tex );
			GUI.DrawTexture( new Rect( 0, lastRect.yMax + 4, Screen.width, 1 ), tex );
			GUI.color = Color.white;
		}

		//Used just after a textfield with no prefix to show an italic transparent text inside when empty
		public static void TextFieldComment( string check, string comment = "Comments..." )
		{
			if( string.IsNullOrEmpty( check ) )
			{
				var lastRect = GUILayoutUtility.GetLastRect();
				GUI.color = new Color( 1, 1, 1, 0.3f );
				GUI.Label( lastRect, " <i>" + comment + "</i>" );
				GUI.color = Color.white;
			}
		}

		//Show an automatic editor gui for arbitrary objects, taking into account custom attributes
		public static void ShowAutoEditorGUI( object o )
		{
			foreach( var field in o.GetType().GetFields( BindingFlags.Instance | BindingFlags.Public ) )
			{
				field.SetValue( o, GenericField( field.Name, field.GetValue( o ), field.FieldType, field, o ) );
				GUI.backgroundColor = Color.white;
			}
		}

		//--
		//For generic automatic editors. Passing a MemberInfo will also check for attributes
		public static object GenericField( string name, object value, Type t, MemberInfo member = null, object instance = null, Type subType = null, bool addElem = true )
		{
			GUI.backgroundColor = Color.white;

			if( t == null )
			{
				GUILayout.Label( "NO TYPE PROVIDED!" );
				return value;
			}

			//Preliminary Hides
			if( typeof( Delegate ).IsAssignableFrom( t ) )
			{
				return value;
			}

			//

			if( member != null )
			{
				//Hide class?
				if( t.GetCustomAttributes( typeof( HideInInspector ), true ).FirstOrDefault() != null || t.GetCustomAttributes( typeof( HideClassInInspectorAttribute ), true ).FirstOrDefault() != null )
				{
					return value;
				}

				//Hide field?
				if( member.GetCustomAttributes( typeof( HideInInspector ), true ).FirstOrDefault() != null )
				{
					return value;
				}

				//Is required?
				if( member.GetCustomAttributes( typeof( RequiredFieldAttribute ), true ).FirstOrDefault() != null )
				{
					if( value == null || value.Equals( null ) || t == typeof( string ) && string.IsNullOrEmpty( ( string ) value ) )
					{
						GUI.backgroundColor = lightRed;
					}
				}
			}

			if( member != null )
			{
				GenericFieldColor( name, value, t, member, instance );
			}

			name = name.SplitCamelCase();

			if( member != null )
			{
				var nameAtt = member.GetCustomAttributes( typeof( NameAttribute ), true ).FirstOrDefault() as NameAttribute;
				if( nameAtt != null )
				{
					name = nameAtt.name;
				}

				if( instance != null )
				{
					var showAtt = member.GetCustomAttributes( typeof( ShowIfAttribute ), true ).FirstOrDefault() as ShowIfAttribute;
					if( showAtt != null )
					{
						var targetField = instance.GetType().GetField( showAtt.fieldName );
						if( targetField == null || targetField.FieldType != typeof( bool ) )
						{
							GUILayout.Label( string.Format( "[ShowIf] Error: bool \"{0}\" does not exist.", showAtt.fieldName ) );
						}
						else
						{
							if( ( bool ) targetField.GetValue( instance ) != showAtt.show )
							{
								return value;
							}
						}
					}
				}
			}

			//Then check UnityObjects
			if( typeof( UnityObject ).IsAssignableFrom( t ) )
			{
				if( t == typeof( Component ) && ( Component ) value != null )
				{
					return ComponentField( name, ( Component ) value, typeof( Component ) );
				}
				return EditorGUILayout.ObjectField( name, ( UnityObject ) value, t, true );
			}

			//Restricted popup values?
			if( member != null )
			{
				var popAtt = member.GetCustomAttributes( typeof( PopupFieldAttribute ), true ).FirstOrDefault() as PopupFieldAttribute;
				if( popAtt != null )
				{
					if( popAtt.staticPath != null )
					{
						try
						{
							var typeName = popAtt.staticPath.Substring( 0, popAtt.staticPath.LastIndexOf( "." ) );
							var type = ReflectionTools.GetType( typeName );
							var start = popAtt.staticPath.LastIndexOf( "." ) + 1;
							var end = popAtt.staticPath.Length;
							var propName = popAtt.staticPath.Substring( start, end - start );
							var prop = type.GetProperty( propName, BindingFlags.Static | BindingFlags.Public );
							var propValue = prop.GetValue( null, null );
							var values = ( ( IEnumerable ) propValue ).Cast< object >().ToList();
							return Popup( name, value, values, false );
						}
						catch
						{
							EditorGUILayout.LabelField( name, "[PopupField] attribute error!" );
							return value;
						}
					}
					return Popup( name, value, popAtt.values.ToList(), false );
				}
			}

			//Check abstract
			if( value != null && value.GetType().IsAbstract || value == null && t.IsAbstract )
			{
				EditorGUILayout.LabelField( name, string.Format( "Abstract ({0})", t.FriendlyName() ) );
				return value;
			}

			//Create instance for some types
			if( value == null && !t.IsAbstract && !t.IsInterface && ( t.IsValueType || t.GetConstructor( Type.EmptyTypes ) != null || t.IsArray ) )
			{
				if( t.IsArray )
				{
					value = Array.CreateInstance( t.GetElementType(), 0 );
				}
				else
				{
					value = Activator.CreateInstance( t );
				}
			}

			if( member != null )
			{
				var context = new ContextGuiHelper( name, value, t, member, GenericField );
				var customAttribute = GetAttribute< IAttributeGuiHelper >( member );
				if( customAttribute != null && customAttribute.AttributeTarget == t )
				{
					try
					{
						if( customAttribute.DrawGenericField( context ) )
						{
							return context.Value;
						}
					}
					catch( Exception exception )
					{
						throw new ArgumentException( "Exception draw: " + customAttribute.GetType().ToString(), exception );
					}
				}
			}


			//..............
			if( t == typeof( string ) )
			{
				if( member != null )
				{
					if( member.GetCustomAttributes( typeof( MessageFieldAttribute ), true ).FirstOrDefault() != null )
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField( name, GUILayout.Width( 120 ) );
							EditorGUILayout.LabelField( ( string ) value, GUI.skin.box, GUILayout.ExpandWidth( true ) );
						}
						EditorGUILayout.EndHorizontal();
						return ( string ) value;
					}

					if( member.GetCustomAttributes( typeof( TagFieldAttribute ), true ).FirstOrDefault() != null )
					{
						return EditorGUILayout.TagField( name, ( string ) value );
					}

					var areaAtt = member.GetCustomAttributes( typeof( TextAreaFieldAttribute ), true ).FirstOrDefault() as TextAreaFieldAttribute;
					if( areaAtt != null )
					{
						GUILayout.Label( name );
						var areaStyle = new GUIStyle( GUI.skin.GetStyle( "TextArea" ) );
						areaStyle.wordWrap = true;
						var s = EditorGUILayout.TextArea( ( string ) value, areaStyle, GUILayout.Height( areaAtt.height ) );
						return s;
					}

					var pathAtt = member.GetCustomAttributes( typeof( PathSelectorAttribute ), true ).FirstOrDefault() as PathSelectorAttribute;
					if( pathAtt != null )
					{
						var result = value;

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField( name, GUILayout.Width( 120 ) );

							EditorGUILayout.LabelField( value == null ? string.Empty : value.ToString(), GUI.skin.textArea );

							if( GUILayout.Button( "...", GUILayout.Width( 30 ) ) )
							{
								var absolutePath = EditorUtility.OpenFolderPanel( "Path...", Application.dataPath, "" );

								if( pathAtt.AbsolutePath )
								{
									result = absolutePath;
								}
								else
								{
									if( absolutePath.StartsWith( Application.dataPath ) )
									{
										result = "Assets" + absolutePath.Substring( Application.dataPath.Length );
									}
									else
									{
										Debug.Log( "Path is not in project" );
									}
								}
							}
						}
						EditorGUILayout.EndHorizontal();

						return result;
					}
				}
				return EditorGUILayout.TextField( name, ( string ) value );
			}


			if( t == typeof( bool ) )
			{
				return EditorGUILayout.Toggle( name, ( bool ) value );
			}
			if( t == typeof( int ) )
			{
				if( member != null )
				{
					var sField = member.GetCustomAttributes( typeof( SliderFieldAttribute ), true ).FirstOrDefault() as SliderFieldAttribute;
					if( sField != null )
					{
						return ( int ) EditorGUILayout.Slider( name, ( int ) value, ( int ) sField.left, ( int ) sField.right );
					}
					if( member.GetCustomAttributes( typeof( LayerFieldAttribute ), true ).FirstOrDefault() != null )
					{
						return EditorGUILayout.LayerField( name, ( int ) value );
					}
				}
				return EditorGUILayout.IntField( name, ( int ) value );
			}

			if( t == typeof( Int64 ) )
			{
				return EditorGUILayout.LongField( name, ( long ) value );
			}

			if( t == typeof( float ) )
			{
				if( member != null )
				{
					var sField = member.GetCustomAttributes( typeof( SliderFieldAttribute ), true ).FirstOrDefault() as SliderFieldAttribute;
					if( sField != null )
					{
						return EditorGUILayout.Slider( name, ( float ) value, sField.left, sField.right );
					}
				}
				return EditorGUILayout.FloatField( name, ( float ) value );
			}

			if( t == typeof( Vector2 ) )
			{
				return EditorGUILayout.Vector2Field( name, ( Vector2 ) value );
			}

			if( t == typeof( Vector3 ) )
			{
				return EditorGUILayout.Vector3Field( name, ( Vector3 ) value );
			}

			if( t == typeof( Vector4 ) )
			{
				return EditorGUILayout.Vector4Field( name, ( Vector4 ) value );
			}

			if( t == typeof( Quaternion ) )
			{
				var quat = ( Quaternion ) value;
				var vec4 = new Vector4( quat.x, quat.y, quat.z, quat.w );
				vec4 = EditorGUILayout.Vector4Field( name, vec4 );
				return new Quaternion( vec4.x, vec4.y, vec4.z, vec4.w );
			}

			if( t == typeof( Color ) )
			{
				return EditorGUILayout.ColorField( name, ( Color ) value );
			}

			if( t == typeof( Rect ) )
			{
				return EditorGUILayout.RectField( name, ( Rect ) value );
			}

			if( t == typeof( AnimationCurve ) )
			{
				return EditorGUILayout.CurveField( name, ( AnimationCurve ) value );
			}

			if( t == typeof( Bounds ) )
			{
				return EditorGUILayout.BoundsField( name, ( Bounds ) value );
			}

			if( t == typeof( LayerMask ) )
			{
				return LayerMaskField( name, ( LayerMask ) value );
			}

			if( t.IsSubclassOf( typeof( Enum ) ) )
			{
				return EditorGUILayout.EnumPopup( name, ( Enum ) value );
			}

			if( typeof( IList ).IsAssignableFrom( t ) )
			{
				return ListEditor( name, ( IList ) value, t, member, subType, addElem );
			}

			if( typeof( IDictionary ).IsAssignableFrom( t ) )
			{
				return DictionaryEditor( name, ( IDictionary ) value, t, member );
			}

			//show nested class members recursively
			if( value != null && !t.IsEnum && !t.IsInterface )
			{
				GUILayout.BeginVertical();
				EditorGUILayout.LabelField( name, t.FriendlyName() );
				EditorGUI.indentLevel++;

				foreach( var field in value.GetType().GetFields( BindingFlags.Instance | BindingFlags.Public ) )
				{
					field.SetValue( value, GenericField( field.Name, field.GetValue( value ), field.FieldType, field ) );
				}

				foreach( var property in value.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
				{
					if( property.GetIndexParameters().Length != 0) continue;
					if( property.GetCustomAttributes( typeof( HideInInspector ), true ).FirstOrDefault() == null )
					{
						var changedValue = GenericField( property.Name, property.GetValue( value, null ), property.PropertyType, property );
						if( property.CanWrite )
						{
							property.SetValue( value, changedValue, null );
						}
					}
				}

				EditorGUI.indentLevel--;
				GUILayout.EndVertical();
			}
			else
			{
				EditorGUILayout.LabelField( name, string.Format( "({0})", t.FriendlyName() ) );
			}

			return value;
		}

		public static void GenericFieldColor( string name, object value, Type t, MemberInfo member = null, object instance = null )
		{
			var headerAttr = GetAttribute< HeaderGuiAttribute >( member );
			if( headerAttr != null )
			{
				GUI.color = Color.green;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( headerAttr.header );
				GUI.color = Color.white;
			}

			var descriptionAttr = GetAttribute< DescriptionGuiAttribute >( member );
			if( descriptionAttr != null )
			{
				GUI.color = descriptionAttr.color;
				EditorGUILayout.LabelField( descriptionAttr.description );
				GUI.color = Color.white;
			}
		}

		public static LayerMask LayerMaskField( string prefix, LayerMask layerMask, params GUILayoutOption[ ] layoutOptions )
		{
			var layers = InternalEditorUtility.layers;
			_layerNumbers.Clear();

			for( var i = 0; i < layers.Length; i++ )
			{
				_layerNumbers.Add( LayerMask.NameToLayer( layers[ i ] ) );
			}

			var maskWithoutEmpty = 0;
			for( var i = 0; i < _layerNumbers.Count; i++ )
			{
				if( ( ( 1 << _layerNumbers[ i ] ) & layerMask.value ) > 0 )
				{
					maskWithoutEmpty |= 1 << i;
				}
			}

			if( !string.IsNullOrEmpty( prefix ) )
			{
				maskWithoutEmpty = EditorGUILayout.MaskField( prefix, maskWithoutEmpty, layers, layoutOptions );
			}
			else
			{
				maskWithoutEmpty = EditorGUILayout.MaskField( maskWithoutEmpty, layers, layoutOptions );
			}

			var mask = 0;
			for( var i = 0; i < _layerNumbers.Count; i++ )
			{
				if( ( maskWithoutEmpty & ( 1 << i ) ) > 0 )
				{
					mask |= 1 << _layerNumbers[ i ];
				}
			}
			layerMask.value = mask;
			return layerMask;
		}

		//An IList editor (List<T> and Arrays)
		public static IList ListEditor( string prefix, IList list, Type listType, MemberInfo memberInfo = null, Type subType = null, bool addElem = true )
		{
			var argType = listType.IsArray ? listType.GetElementType() : listType.GetGenericArguments()[ 0 ];

			//register foldout
			if( !_registeredEditorFoldouts.ContainsKey( list ) )
			{
				_registeredEditorFoldouts[ list ] = false;
			}

			GUILayout.BeginVertical();
			if( memberInfo != null && !Foldout( memberInfo, prefix ) )
			{
				GUILayout.EndVertical();
				return list;
			}

			if( list.Equals( null ) )
			{
				GUILayout.Label( "Null List" );
				GUILayout.EndVertical();
				return list;
			}
			EditorGUI.indentLevel++;

			for( var i = 0; i < list.Count; i++ )
			{
				if( memberInfo == null )
				{
					var elem = list[ i ];
					if( elem != null )
					{
						if( !_registeredEditorFoldouts.ContainsKey( elem ) )
						{
							_registeredEditorFoldouts[ elem ] = false;
						}

						var foldoutElem = _registeredEditorFoldouts[ elem ];
						foldoutElem = EditorGUILayout.Foldout( foldoutElem, elem + " : " + elem.GetType().Name );
						_registeredEditorFoldouts[ elem ] = foldoutElem;

						if( !foldoutElem )
						{
							continue;
						}
					}
				}


				GUILayout.BeginVertical( GUI.skin.box );
				GUI.color = Color.white;
				GUILayout.BeginHorizontal();
				list[ i ] = GenericField( "Element " + i, list[ i ], list[ i ].GetType(), memberInfo );

				GUI.color = Color.green;
				if( GUILayout.Button( "CC", GUILayout.Width( 28 ), GUILayout.Height( 24 ) ) )
				{
					var copy = CreateObjectReplica( list[ i ], list[ i ].GetType() );
					list.Add( CustomCopyAttributes( copy ) );
				}

				GUI.color = Color.red;
				if( GUILayout.Button( "X", GUILayout.Width( 24 ), GUILayout.Height( 24 ) ) )
				{
					var confirmation = EditorUtility.DisplayDialog( "Delete item!", "Are sure you want to delete an item?", "Yes, delete!", "Cancel" );
					if( confirmation )
					{
						if( listType.IsArray )
						{
							list = ResizeArray( ( Array ) list, list.Count - 1 );
							_registeredEditorFoldouts[ list ] = true;
						}
						else
						{
							list.RemoveAt( i );
						}
					}
				}

				GUILayout.EndHorizontal();

				GUI.color = Color.white;
				GUILayout.EndVertical();
			}

			var lType = subType ?? argType;
			
			GUI.color = Color.green;
			if( addElem && GUILayout.Button( "Add Element" ) )
			{
				if( lType != null )
				{
					if( lType == typeof( string ) )
					{
						list.Add( string.Empty );
					}
					else
					{
						list.Add( Activator.CreateInstance( lType ) );
					}
				}
				else if( listType.IsArray )
				{
					list = ResizeArray( ( Array ) list, list.Count + 1 );
					_registeredEditorFoldouts[ list ] = true;
				}
				else
				{
					list.Add( argType.IsValueType ? Activator.CreateInstance( argType ) : null );
				}
			}
			GUI.color = Color.white;

			EditorGUI.indentLevel--;
			Separator();

			GUILayout.EndVertical();
			return list;
		}

		public static List< Type > GetAllSubclass( Type subType )
		{
			var types = subType.IsAbstract ? new List< Type >() : new List< Type >
			{
				subType
			};
			types.AddRange( Assembly.GetAssembly( subType ).GetTypes().Where( t => t.IsSubclassOf( subType ) ) );
			return types;
		}

		//A dictionary editor
		public static IDictionary DictionaryEditor( string prefix, IDictionary dict, Type dictType, MemberInfo member )
		{
			var keyType = dictType.GetGenericArguments()[ 0 ];
			var valueType = dictType.GetGenericArguments()[ 1 ];

			//register foldout
			if( !_registeredEditorFoldouts.ContainsKey( dict ) )
			{
				_registeredEditorFoldouts[ dict ] = false;
			}

			GUILayout.BeginVertical();

			var foldout = _registeredEditorFoldouts[ dict ];
			foldout = EditorGUILayout.Foldout( foldout, prefix );
			_registeredEditorFoldouts[ dict ] = foldout;

			if( !foldout )
			{
				GUILayout.EndVertical();
				return dict;
			}

			if( dict.Equals( null ) )
			{
				GUILayout.Label( "Null Dictionary" );
				GUILayout.EndVertical();
				return dict;
			}

			var keys = dict.Keys.Cast< object >().ToList();
			var values = dict.Values.Cast< object >().ToList();

			if( GUILayout.Button( "Add Element" ) )
			{
				if( !typeof( UnityObject ).IsAssignableFrom( keyType ) )
				{
					object newKey = null;
					if( keyType == typeof( string ) )
					{
						newKey = string.Empty;
					}
					else
					{
						newKey = Activator.CreateInstance( keyType );
					}
					if( dict.Contains( newKey ) )
					{
						Debug.LogWarning( string.Format( "Key '{0}' already exists in Dictionary", newKey ) );
						return dict;
					}

					keys.Add( newKey );
				}
				else
				{
					Debug.LogWarning( "Can't add a 'null' Dictionary Key" );
					return dict;
				}

				values.Add( valueType.IsValueType ? Activator.CreateInstance( valueType ) : null );
			}

			//clear before reconstruct
			dict.Clear();
			EditorGUILayout.Space();

			for( var i = 0; i < keys.Count; i++ )
			{
				var isRemovedElem = false;
				GUILayout.BeginHorizontal();

				if( member != null && member.GetCustomAttributes( typeof( IAttributeGuiHelper ), true ).FirstOrDefault() != null )
				{
					//CustomAttributeBuilder 
					keys[ i ] = GenericField( "Key:", keys[ i ], keyType, member );
					values[ i ] = GenericField( "Value:", values[ i ], valueType, null );
				}
				else
				{
					keys[ i ] = GenericField( "Key:", keys[ i ], keyType, null );
					values[ i ] = GenericField( "Value:", values[ i ], valueType, null );
				}
				GUI.color = Color.red;

				if( GUILayout.Button( "X", GUILayout.Width( 24 ), GUILayout.Height( 24 ) ) )
				{
					isRemovedElem = true;
				}
				GUI.color = Color.white;


				GUILayout.EndHorizontal();

				try
				{
					if( !isRemovedElem )
						dict.Add( keys[ i ], values[ i ] );
				}
				catch
				{
					Debug.Log( "Dictionary Key removed due to duplicate found" );
				}
			}

			Separator();

			GUILayout.EndVertical();
			return dict;
		}

		//An editor field where if the component is null simply shows an object field, but if its not, shows a dropdown popup to select the specific component
		//from within the gameobject
		public static Component ComponentField( string prefix, Component comp, Type type, bool allowNone = true )
		{
			if( !comp )
			{
				if( !string.IsNullOrEmpty( prefix ) )
				{
					comp = EditorGUILayout.ObjectField( prefix, comp, type, true, GUILayout.ExpandWidth( true ) ) as Component;
				}
				else
				{
					comp = EditorGUILayout.ObjectField( comp, type, true, GUILayout.ExpandWidth( true ) ) as Component;
				}

				return comp;
			}

			var allComp = new List< Component >( comp.GetComponents( type ) );
			var compNames = new List< string >();

			foreach( var c in allComp.ToArray() )
			{
				if( c == null )
				{
					continue;
				}
				compNames.Add( c.GetType().FriendlyName() + " (" + c.gameObject.name + ")" );
			}

			if( allowNone )
			{
				compNames.Add( "|NONE|" );
			}

			int index;
			if( !string.IsNullOrEmpty( prefix ) )
			{
				index = EditorGUILayout.Popup( prefix, allComp.IndexOf( comp ), compNames.ToArray(), GUILayout.ExpandWidth( true ) );
			}
			else
			{
				index = EditorGUILayout.Popup( allComp.IndexOf( comp ), compNames.ToArray(), GUILayout.ExpandWidth( true ) );
			}

			if( allowNone && index == compNames.Count - 1 )
			{
				return null;
			}

			return allComp[ index ];
		}

		public static string StringPopup( string selected, List< string > options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[ ] GUIOptions )
		{
			return StringPopup( string.Empty, selected, options, showWarning, allowNone, GUIOptions );
		}

		//a popup that is based on the string rather than the index
		public static string StringPopup( string prefix, string selected, List< string > options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[ ] GUIOptions )
		{
			EditorGUILayout.BeginVertical();
			if( options.Count == 0 && showWarning )
			{
				EditorGUILayout.HelpBox( "There are no options to select for '" + prefix + "'", MessageType.Warning );
				EditorGUILayout.EndVertical();
				return null;
			}

			if( allowNone )
			{
				options.Insert( 0, "|NONE|" );
			}

			int index;

			if( options.Contains( selected ) )
			{
				index = options.IndexOf( selected );
			}
			else
			{
				index = allowNone ? 0 : -1;
			}

			if( !string.IsNullOrEmpty( prefix ) )
			{
				index = EditorGUILayout.Popup( prefix, index, options.ToArray(), GUIOptions );
			}
			else
			{
				index = EditorGUILayout.Popup( index, options.ToArray(), GUIOptions );
			}

			if( index == -1 || allowNone && index == 0 )
			{
				if( showWarning )
				{
					if( !string.IsNullOrEmpty( selected ) )
					{
						EditorGUILayout.HelpBox( "The previous selection '" + selected + "' has been deleted or changed. Please select another", MessageType.Warning );
					}
					else
					{
						EditorGUILayout.HelpBox( "Please make a selection", MessageType.Warning );
					}
				}
			}

			EditorGUILayout.EndVertical();
			if( allowNone )
			{
				return index == 0 ? string.Empty : options[ index ];
			}

			return index == -1 ? string.Empty : options[ index ];
		}

		///Generic Popup for selection of any element within a list
		public static T Popup< T >( string prefix, T selected, List< T > options, bool addNoneDefault = true, params GUILayoutOption[ ] GUIOptions )
		{
			if( addNoneDefault )
			{
				//add default "NONE" option
				options.Insert( 0, default( T ) );
			}

			//		EditorGUILayout.BeginVertical();
			int index;

			if( options.Contains( selected ) )
			{
				index = options.IndexOf( selected );
			}
			else
			{
				index = -1;
			}

			var stringedOptions = options.Select( o => o != null ? o.ToString() : "|NONE|" ).ToArray();

			if( !string.IsNullOrEmpty( prefix ) )
			{
				index = EditorGUILayout.Popup( prefix, index, stringedOptions, GUIOptions );
			}
			else
			{
				index = EditorGUILayout.Popup( index, stringedOptions, GUIOptions );
			}

			//		EditorGUILayout.EndVertical();
			return index == -1 ? options[ 0 ] : options[ index ];
		}

        private static object CustomCopyAttributes( object Object )
		{
			var fields = Object.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );
			foreach( FieldInfo fieldInfo in fields )
			{
				var customCopyAttribute = GetAttribute< IAttributeCustomCopy >( fieldInfo );
				if( customCopyAttribute != null )
				{
					var value = fieldInfo.GetValue( Object );
					var newValue = customCopyAttribute.CopyObject( value );
					fieldInfo.SetValue( Object, newValue );
				}
			}

			var properies = Object.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );
			foreach( var propertyInfo in properies )
			{
				var customCopyAttribute = GetAttribute< IAttributeCustomCopy >( propertyInfo );
				if( customCopyAttribute != null )
				{
					var value = propertyInfo.GetValue( Object, null );
					var newValue = customCopyAttribute.CopyObject( value );
					propertyInfo.SetValue( Object, newValue, null );
				}
			}
			return Object;
		}

		private static object CreateObjectReplica( object patternObject, Type nObjectType )
		{
			//взять все проперти и филды исходника
			//найти такие же поля в созданном объекте
			//посетать одно в другое

			var replicaObject = Activator.CreateInstance( nObjectType );

			var patternFields = patternObject.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );
			var replicaFielsd = replicaObject.GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );

			foreach( FieldInfo patternFieldInfo in patternFields )
			{
				foreach( FieldInfo replicaFieldInfo in replicaFielsd )
				{
					bool checkType = patternFieldInfo.FieldType == replicaFieldInfo.FieldType;
					bool checkName = patternFieldInfo.Name == replicaFieldInfo.Name;

					if( checkName && checkType )
					{
						var value = patternFieldInfo.GetValue( patternObject );
						replicaFieldInfo.SetValue( replicaObject, value );
					}
				}
			}

			var patternProperties = patternObject.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );
			var replicaProperties = replicaObject.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );

			foreach( PropertyInfo patternPropertyInfo in patternProperties )
			{
				foreach( PropertyInfo replicaProperyInfo in replicaProperties )
				{
					bool checkType = patternPropertyInfo.PropertyType == replicaProperyInfo.PropertyType;
					bool checkName = patternPropertyInfo.Name == replicaProperyInfo.Name;

					if( checkName && checkType )
					{
						var value = patternPropertyInfo.GetValue( patternObject, null );
						if( replicaProperyInfo.GetSetMethod( true ) != null )
						{
							replicaProperyInfo.SetValue( replicaObject, value, null );
						}
					}
				}
			}
			return replicaObject;
		}

		private static bool Foldout( object elem, string prefix = "", bool defaultOpen = false )
		{
			if( !_registeredEditorFoldouts.ContainsKey( elem ) )
			{
				_registeredEditorFoldouts[ elem ] = false;
			}

			var foldout = _registeredEditorFoldouts[ elem ];
			foldout = EditorGUILayout.Foldout( foldout, prefix );
			_registeredEditorFoldouts[ elem ] = foldout;
			return foldout;
		}

		private static T GetAttribute< T >( MemberInfo member )
		{
			try
			{
				if( member.GetCustomAttributes( typeof( T ), true ).FirstOrDefault() != null )
				{
					return ( T ) member.GetCustomAttributes( typeof( T ), true ).FirstOrDefault();
				}
			}
			catch( Exception ex )
			{
				Debug.LogException( new ArgumentException( "Can't get attribute.", ex ) );
			}

			return default( T );
		}

		private static Array ResizeArray( Array oldArray, int newSize )
		{
			var oldSize = oldArray.Length;
			var elementType = oldArray.GetType().GetElementType();
			var newArray = Array.CreateInstance( elementType, newSize );
			var preserveLength = Math.Min( oldSize, newSize );
			if( preserveLength > 0 )
			{
				Array.Copy( oldArray, newArray, preserveLength );
			}
			return newArray;
		}


		//Get all base derived types in the current loaded assemplies, excluding the base type itself
	}
}

#endif