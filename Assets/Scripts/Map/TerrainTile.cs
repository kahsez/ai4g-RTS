using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Para que se pueda crear un TerrainTile desde el menú
// Tile hereda de TileBase, que a su vez hereda de ScriptableObject. 
[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "IADeJ/Terrain Tile")]
public class TerrainTile : Tile
{
    // Lo único que diferencia (de momento) a un Tile normal de un tile personalizado
    // es que nuestros tiles llevarán como información extra el tipo de terreno (TerrainType)

    // Para poder usar estos tiles, se puede usar el Tile Palette que trae Unity desde la versión
    // 2017.2
    // En un Tile Palette se pueden almacenar varios Tiles, siempre que hereden de TileBase.
    // Desde la paleta, se puede elegir cualquier tile que hayamos puesto y pintar sobre
    // la rejilla. Lo que estamos pintando realmente son estos TileBase (ScriptableObject).

    // La potencia de los tiles custom no radica únicamente en añadir información
    // estática (como el tipo de terreno, que es un enumerado). También se le podría
    // asociar un GameObject a cada Tile cuando aparezcan en tiempo de ejecución.

    [SerializeField] private TerrainType type;

    public TerrainType Type 
    {
        get { return type; }
    }
}
