using System.Collections.Generic;
using UnityEngine;

namespace Gulde.Core
{
    public class ProductionComponent : MonoBehaviour
    {
        [SerializeField]
        public ExchangeComponent exchangeComponent;

        [SerializeField]
        public AssignmentComponent assignmentComponent;

        [SerializeField]
        public List<ProductionProgress> productionProgresses;
    }
}