#if UNITY_EDITOR

using System;
using UnityEngine;

namespace CrazyPanda.UnityCore.Utils.Editor.GUIHelper
{
    [ AttributeUsage( AttributeTargets.Field | AttributeTargets.Property ) ]
    public class HeaderGuiAttribute : Attribute
    {
        public string header;
        public Color color = Color.green;

        public HeaderGuiAttribute( string header )
        {
            this.header = header;
        }
    }

    [ AttributeUsage( AttributeTargets.Field | AttributeTargets.Property ) ]
    public class DescriptionGuiAttribute : Attribute
    {
        public  string description;
        public  Color color;

        public DescriptionGuiAttribute(string description, TypeColor colorType)
        {
            this.description = description;
            switch( colorType )
            {
                case TypeColor.red:
                    color = Color.red;
                    break;
                case TypeColor.green:
                    color = Color.green;
                    break;
                case TypeColor.yellow:
                    color = Color.yellow;
                    break;
                default:
                    color = Color.white;
                    break;
            }
        }
        public DescriptionGuiAttribute( string description )
        {
            this.description = description;
            color = Color.white;
        }
    }

    public enum TypeColor
    {
        white,
        red,
        green,
        yellow,
    }
}

#endif