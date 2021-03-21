using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gulde.Pathfinding
{
    public class NavComponent : MonoBehaviour
    {
        [SerializeField] Tilemap _mapBackground;
        [SerializeField] Tilemap _mapBuildSpaces;
        [SerializeField] Tile _tileNavGreen;
        [SerializeField] Tile _tileNavRed;

        Tilemap Tilemap { get; set; }

        void Start()
        {
            Tilemap = GetComponent<Tilemap>();
            GenerateNavMap();
        }

        [ContextMenu("Generate Nav Map")]
        void GenerateNavMap()
        {
            foreach (var cellPosition in _mapBackground.cellBounds.allPositionsWithin)
            {
                if (_mapBuildSpaces.HasTile(cellPosition))
                {
                    Tilemap.SetTile(cellPosition, _tileNavRed);
                }
                else
                {
                    Tilemap.SetTile(cellPosition, _tileNavGreen);
                }
            }
        }
    }
}
