using System;
using Sirenix.OdinInspector;

namespace GuldeLib.Generators
{
    [IncludeMyAttributes]
    [Title("@$property.NiceName")]
    [ToggleGroup("HasNaming", "Naming", Order = 0)]
    [HideLabel]
    [PropertySpace(7.5f, 5f)]
    public class NamingAttribute : Attribute { }
}