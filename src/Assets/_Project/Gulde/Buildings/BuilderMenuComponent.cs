using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gulde.Buildings
{
    public class BuilderMenuComponent : MonoBehaviour
    {
        [SerializeField] Building _buildingHouse;
        [SerializeField] Building _buildingCompany;

        [SerializeField] BuilderComponent _builderComponent;

        void Start()
        {
            var buttonHouse = transform.GetChild(0).GetComponent<Button>();
            var buttonCompany = transform.GetChild(1).GetComponent<Button>();

            buttonHouse.onClick.AddListener(() => _builderComponent.OnButtonBuilding(_buildingHouse));
            buttonCompany.onClick.AddListener(() => _builderComponent.OnButtonBuilding(_buildingCompany));
        }
    }
}
