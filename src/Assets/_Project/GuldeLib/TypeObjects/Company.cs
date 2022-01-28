using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.Producing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Companies/Company")]
    public class Company : TypeObject<Company>
    {
        /// <summary>
        /// Gets or sets the cost of hiring emloyees.
        /// </summary>
        [Required]
        [Setting]
        [OdinSerialize]
        public int HiringCost { get; set; }

        /// <summary>
        /// Gets or sets the cost of hiring carts.
        /// </summary>
        [Required]
        [Setting]
        [OdinSerialize]
        public int CartCost { get; set; }

        /// <summary>
        /// Gets or sets the cost of wages per hour worked per employee.
        /// </summary>
        [Required]
        [Setting]
        [OdinSerialize]
        public float WagePerHour { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employees</see> of the company.
        /// </summary>
        [Required]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableEmployee> Employees { get; set; } = new List<GeneratableEmployee>();

        /// <summary>
        /// Gets or sets the <see cref = "CartComponent">Carts</see> of the company.
        /// </summary>
        [Required]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableCart> Carts { get; set; } = new List<GeneratableCart>();

        /// <summary>
        /// Gets or sets the <see cref = "LocationComponent">Location</see> of the company.
        /// </summary>
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocation Location { get; set; } = new GeneratableLocation();

        /// <summary>
        /// Gets or sets the <see cref = "ProductionComponent">ProductionComponent</see> of the company.
        /// </summary>
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableProduction Production { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "MasterComponent">Master</see> of the company.
        /// </summary>
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableMaster Master { get; set; }

        public override bool SupportsNaming => false;
    }
}