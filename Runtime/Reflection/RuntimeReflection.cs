//.................................................................................................................//
//Reflection system class for runtime...............................................................................//
//.................................................................................................................//
//.................................................................................................................//
//.................................................................................................................//
//.................................................................................................................//
#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

public static class RuntimeReflection
{
    //.........................................................................................................//
    //Interface block
    //.........................................................................................................//


    public static object CallPrivate<T>( string methodName, T objectRef, params object[] parameters ) where T : class
    {
        MethodInfo tempInfo = typeof(T).GetMethod( methodName, _baseFlags );
        return tempInfo.Invoke( objectRef, parameters );
    }

    public static T GetValue<T>( string valueName, object objectRef )
    {
        FieldInfo tempInfo = objectRef.GetType().GetField( valueName, _baseFlags );
        return ( T )tempInfo.GetValue( objectRef );
    }

    public static TReturn GetValue<TReturn, TInstance>( string valueName, TInstance objectRef )
    {
        FieldInfo tempInfo = typeof(TInstance).GetField( valueName, _baseFlags );
        return ( TReturn )tempInfo.GetValue( objectRef );
    }

    public static TReturn GetProp<TReturn, TInstance>( string propName, TInstance objectRef )
    {
        PropertyInfo tempInfo = typeof(TInstance).GetProperty( propName, _baseFlags );
        return ( TReturn )tempInfo.GetValue( objectRef, null );
    }

    public static T GetProp<T>( string valueName, object objectRef )
    {
        PropertyInfo tempInfo = objectRef.GetType().GetProperty( valueName, _baseFlags );
        return ( T )tempInfo.GetValue( objectRef, null );
    }

    public static void SetProp<TObject, TValue>( string propName, TObject objectRef, TValue value )
    {
        PropertyInfo tempInfo = typeof(TObject).GetProperty( propName, _baseFlags );
        tempInfo.SetValue( objectRef, value, new object[] { } );
    }

    public static void SetValue( string valueName, object objectRef, object parameter )
    {
        FieldInfo tempInfo = objectRef.GetType().GetField( valueName, _baseFlags );
        tempInfo.SetValue( objectRef, parameter );
    }

    public static void SetStaticValue<T>( string valueName, object parameter )
    {
        FieldInfo tempInfo = typeof(T).GetField( valueName, _baseFlags );
        tempInfo.SetValue( null, parameter );
    }

    public static void CallAwake<T>( T parameter ) where T : Object
    {
        CallPrivate( AwakeName, parameter );
    }

    public static void CallStart<T>( T parameter ) where T : Object
    {
        CallPrivate( StartName, parameter );
    }

    public static void CallUpdate<T>( T parameter ) where T : Object
    {
        CallPrivate( UpdateName, parameter );
    }

    public static void CallFixedUpdate<T>( T parameter ) where T : Object
    {
        CallPrivate( FixedUpdateName, parameter );
    }

    public static void CallOnEnamble<T>( T parameter ) where T : Object
    {
        CallPrivate( OnEnambleName, parameter );
    }

    public static void CallOnDisable<T>( T parameter ) where T : Object
    {
        CallPrivate( OnDisableName, parameter );
    }

    public static void CallOnTriggerEnter2D<T>( T parameter, Collider2D collider ) where T : Object
    {
        CallPrivate( OnTriggerEnter2DName, parameter, collider );
    }

    public static void CallOnTriggerStay2D<T>( T parameter, Collider2D collider ) where T : Object
    {
        CallPrivate( OnTriggerStay2DName, parameter, collider );
    }

    public static void CallOnTriggerExit2D<T>( T parameter, Collider2D collider ) where T : Object
    {
        CallPrivate( OnTriggerExit2DName, parameter, collider );
    }

    public static void CallOnCollisionEnter2D<T>( T parameter, Collision2D collision ) where T : Object
    {
        CallPrivate( OnCollisionEnter2DName, parameter, collision );
    }

    public static void CallOnCollisionStay2D<T>( T parameter, Collision2D collision ) where T : Object
    {
        CallPrivate( OnCollisionStay2DName, parameter, collision );
    }

    public static void CallOnCollisionExit2D<T>( T parameter, Collision2D collision ) where T : Object
    {
        CallPrivate( OnCollisionExit2DName, parameter, collision );
    }

    public static void CallOnDestroy<T>( T parameter ) where T : Object
    {
        CallPrivate( OnDestroyName, parameter );
    }

    public static void CallOnApplicationQuit<T>( T parameter ) where T : Object
    {
        CallPrivate( OnApplicationQuitName, parameter );
    }


    //.........................................................................................................//
    //Parameters Block
    //.........................................................................................................//  


    private const string AwakeName = @"Awake";
    private const string StartName = @"Start";
    private const string UpdateName = @"Update";
    private const string FixedUpdateName = @"FixedUpdate";

    private const string OnEnambleName = @"OnEnable";
    private const string OnDisableName = @"OnDisable";

    private const string OnTriggerEnter2DName = @"OnTriggerEnter2D";
    private const string OnTriggerStay2DName = @"OnTriggerStay2D";
    private const string OnTriggerExit2DName = @"OnTriggerExit2D";

    private const string OnCollisionEnter2DName = @"OnCollisionEnter2D";
    private const string OnCollisionStay2DName = @"OnCollisionStay2D";
    private const string OnCollisionExit2DName = @"OnCollisionExit2D";

    private const string OnDestroyName = @"OnDestroy";
    private const string OnApplicationQuitName = @"OnApplicationQuit";

    private const BindingFlags _baseFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

}
#endif