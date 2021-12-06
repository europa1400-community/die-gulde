using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Generators
{
    [IncludeMyAttributes]
    [InlineProperty]
    [Title("@$property.NiceName")]
    [FoldoutGroup("Settings")]
    [HideLabel]
    [PropertySpace(7.5f, 5f)]
    public class SettingAttribute : Attribute
    {

    }
}