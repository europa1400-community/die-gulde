using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Generators
{
    [IncludeMyAttributes]
    [Title("@$property.NiceName")]
    [FoldoutGroup("Settings", Order = 1)]
    [HideLabel]
    [PropertySpace(7.5f, 5f)]
    [NoGUIColor]
    public class SettingAttribute : Attribute
    {

    }
}