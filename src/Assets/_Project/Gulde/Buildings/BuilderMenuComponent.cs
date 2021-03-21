using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gulde.Buildings
{
    public class BuilderMenuComponent : MonoBehaviour
    {
        [SerializeField] BuildingLayout _buildingLayoutHouse;
        [SerializeField] BuildingLayout _buildingLayoutCompany;

        [SerializeField] BuilderComponent _builderComponent;

        void Start()
        {
            var buttonHouse = transform.GetChild(0).GetComponent<Button>();
            var buttonCompany = transform.GetChild(1).GetComponent<Button>();

            buttonHouse.onClick.AddListener(() => _builderComponent.OnButtonBuilding(_buildingLayoutHouse));
            buttonCompany.onClick.AddListener(() => _builderComponent.OnButtonBuilding(_buildingLayoutCompany));
        }
    }
}
