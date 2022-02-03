using System;
using GuldeLib.Society;
using GuldeLib.TypeObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GuldeLib.Generators
{
    public class GeneratableTalent : GeneratableTypeObject<Talent>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Talent>();

            if (Value.StartingTalentPoints?.IsGenerated ?? false) Value.StartingTalentPoints.Generate();

            for (var i = 0; i < Value.StartingTalentPoints?.Value; i++)
            {
                var rand = Random.Range(0, 4);
                var talent = (TalentComponent.TalentType)rand;
                Value.TalentToPoints[talent] += 1;
            }
        }
    }
}