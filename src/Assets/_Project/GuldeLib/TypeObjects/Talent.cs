using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Society;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "talent", menuName = "Society/Talent")]
    public class Talent : TypeObject<Talent>
    {
        public override bool SupportsNaming => false;

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableInt StartingTalentPoints { get; set; } = new GeneratableRangedInt();

        public Dictionary<TalentComponent.TalentType, int> TalentToPoints { get; } = new Dictionary<TalentComponent.TalentType, int>
        {
            { TalentComponent.TalentType.Negotiation, 0 },
            { TalentComponent.TalentType.Handicraft, 0 },
            { TalentComponent.TalentType.Stealth, 0 },
            { TalentComponent.TalentType.Combat, 0 },
            { TalentComponent.TalentType.Rhetoric, 0 },
        };
    }
}