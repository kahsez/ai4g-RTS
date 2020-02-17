using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour {

    public Tilemap tilemap;
    public Light mouseLight;
    public TextMeshPro texto;

    private void Update() {
        // Obtenemos la posición del ratón en el mundo
        // Esto es instantáneo porque la cámara es ortográfica (sin profundidad)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mousePosInt = new Vector3Int((int)mousePos.x, (int)mousePos.y, (int)mousePos.z);
        // Efecto de luz sobre el ratón (se puede quitar)
        mouseLight.transform.position = new Vector3(mousePos.x, mousePos.y, mouseLight.transform.position.z);
        if (Input.GetMouseButtonDown(0)) {
            // Cuando se pulse el clic izquierdo, obtenemos el tile de nuestro Tilemap, que sabemos
            // que está relleno con TerrainTile (nuestros tiles custom)
            TerrainTile tile = tilemap.GetTile<TerrainTile>(new Vector3Int(mousePosInt.x, mousePosInt.y, 0));
            string tipoTerreno = "Ninguno";
            if(tile != null) {
                tipoTerreno = tile.Type.ToString() + "("+mousePosInt.x+", "+mousePosInt.y+")";
            }
            // Lo mostramos gráficamente con un texto de TextMeshPro
            texto.gameObject.SetActive(true);
            texto.SetText(tipoTerreno);
            
        }
    }
}
