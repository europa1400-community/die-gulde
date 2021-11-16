using System.Collections.Generic;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Producing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Companies
{
    public class Company : SerializedScriptableObject
    {
        /// <summary>
        /// Gets or sets the cost of hiring emloyees.
        /// </summary>
        [Required]
        [OdinSerialize]
        public int HiringCost { get; set; }

        /// <summary>
        /// Gets or sets the cost of hiring carts.
        /// </summary>
        [Required]
        [OdinSerialize]
        public int CartCost { get; set; }

        /// <summary>
        /// Gets or sets the cost of wages per hour worked per employee.
        /// </summary>
        [Required]
        [OdinSerialize]
        public float WagePerHour { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employees</see> of the company.
        /// </summary>
        [Required]
        [OdinSerialize]
        public List<Employee> Employees { get; } = new List<Employee>();

        /// <summary>
        /// Gets or sets the <see cref = "CartComponent">Carts</see> of the company.
        /// </summary>
        [Required]
        [OdinSerialize]
        public List<Cart> Carts { get; } = new List<Cart>();

        /// <summary>
        /// Gets or sets the <see cref = "LocationComponent">Location</see> of the company.
        /// </summary>
        [Required]
        [OdinSerialize]
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "ProductionComponent">ProductionComponent</see> of the company.
        /// </summary>
        [Optional]
        [OdinSerialize]
        public Production Production { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "MasterComponent">Master</see> of the company.
        /// </summary>
        [Optional]
        [OdinSerialize]
        public Master Master { get; private set; }
    }
}