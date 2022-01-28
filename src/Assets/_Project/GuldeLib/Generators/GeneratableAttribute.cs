using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace GuldeLib.Generators
{
    [IncludeMyAttributes]
    [Title("@$property.NiceName")]
    [FoldoutGroup("Generatables", Order = 2)]
    [HideLabel]
    [PropertySpace(7.5f, 5f)]
    public class GeneratableAttribute : Attribute
    {

    }
}