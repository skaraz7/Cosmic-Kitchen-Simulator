using UnityEngine;

public class RestaurantBuilder
{
    // Medidas globales (en metros)
    static readonly float L = 20f;  // largo (eje X, -10..+10)
    static readonly float W = 16f;  // ancho (eje Z, -8..+8)
    static readonly float H = 3f;   // altura (eje Y)

    // Grosor de paredes
    static readonly float T = 0.2f;

    // Colores de zonas
    static readonly Color COL_CLIENTES = new Color(0.70f, 0.85f, 1.00f); // azul clarito
    static readonly Color COL_COCINA   = new Color(0.90f, 0.98f, 0.90f); // blanco verdoso suave
    static readonly Color COL_ALMACEN  = new Color(0.82f, 0.82f, 0.82f); // gris claro
    static readonly Color COL_SUELO    = new Color(0.28f, 0.28f, 0.28f); // gris oscuro

    public Vector3 BuildRestaurant()
    {
        // Piso base del restaurante
        CreateBox("Floor", new Vector3(0, -0.1f, 0), new Vector3(L, 0.2f, W), COL_SUELO);
        
        // Suelo extendido para evitar caer al vacío
        CreateBox("ExtendedFloor", new Vector3(0, -0.2f, 0), new Vector3(L * 3f, 0.1f, W * 3f), new Color(0.2f, 0.2f, 0.2f));

        // Techo fino
        CreateBox("Roof", new Vector3(0, H, 0), new Vector3(L, 0.2f, W), new Color(0.35f, 0.35f, 0.38f));

        // Paredes exteriores con colores
        BuildFrontWallWithEntrance();
        CreateBox("BackWall",  new Vector3(0, H/2f,  +W/2f), new Vector3(L, H, T), new Color(0.9f, 0.9f, 0.7f)); // Amarillo claro
        CreateBox("LeftWall",  new Vector3(-L/2f, H/2f, 0),  new Vector3(T, H, W), new Color(0.8f, 0.9f, 0.8f)); // Verde claro
        CreateBox("RightWall", new Vector3(+L/2f, H/2f, 0),  new Vector3(T, H, W), new Color(0.9f, 0.8f, 0.8f)); // Rosa claro

        // División horizontal con mostrador
        BuildCounterWall();

        // División vertical servicios
        BuildKitchenStorageWall();

        // Overlays de suelo por zona
        CreateBox("FloorOverlay_Clientes", new Vector3(0, 0.01f, -4f), new Vector3(L, 0.02f, 8f), COL_CLIENTES);
        CreateBox("FloorOverlay_Cocina",   new Vector3(-5f, 0.01f, +4f), new Vector3(10f, 0.02f, 8f), COL_COCINA);
        CreateBox("FloorOverlay_Almacen",  new Vector3(+5f, 0.01f, +4f), new Vector3(10f, 0.02f, 8f), COL_ALMACEN);

        // Luces por sala
        CreatePointLight("Light_Clientes", new Vector3(0,   2.7f, -4f), 15f, 2f);
        CreatePointLight("Light_Cocina",   new Vector3(-5f, 2.7f, +4f), 15f, 2f);
        CreatePointLight("Light_Almacen",  new Vector3(+5f, 2.7f, +4f), 15f, 2f);

        // Luz direccional suave
        var sun = new GameObject("Sun");
        var dl = sun.AddComponent<Light>();
        dl.type = LightType.Directional;
        dl.intensity = 0.6f;
        sun.transform.rotation = Quaternion.Euler(50, -30, 0);

        // Retornar posición del mostrador para clientes
        return new Vector3(0, 0.5f, 0);
    }

    void BuildFrontWallWithEntrance()
    {
        float z = -W/2f;
        float doorW = 4f; // Entrada ampliada al doble (de 2m a 4m)
        float doorH = 2.5f;
        float sideW = (L - doorW) / 2f;

        CreateBox("FrontWall_L", new Vector3(- (doorW/2f + sideW/2f), H/2f, z),
                  new Vector3(sideW, H, T), new Color(0.8f, 0.8f, 1.0f)); // Azul claro entrada
        CreateBox("FrontWall_R", new Vector3(  (doorW/2f + sideW/2f), H/2f, z),
                  new Vector3(sideW, H, T), new Color(0.8f, 0.8f, 1.0f)); // Azul claro entrada
        
        float topH = H - doorH;
        if (topH > 0.01f)
        {
            CreateBox("FrontWall_TOP", new Vector3(0, doorH + topH/2f, z),
                      new Vector3(doorW, topH, T), new Color(0.8f, 0.8f, 1.0f)); // Azul claro entrada
        }
    }

    void BuildCounterWall()
    {
        float z = 0f;
        float gap = (L * 2f) / 3f;
        float side = (L - gap) / 2f;

        CreateBox("CounterWall_L", new Vector3(- (gap/2f + side/2f), H/2f, z),
                  new Vector3(side, H, T), new Color(0.7f, 0.5f, 0.3f)); // Marrón mostrador
        CreateBox("CounterWall_R", new Vector3(  (gap/2f + side/2f), H/2f, z),
                  new Vector3(side, H, T), new Color(0.7f, 0.5f, 0.3f)); // Marrón mostrador

        CreateBox("Counter_BAR", new Vector3(0, 0.5f, z),
                  new Vector3(gap, 1.0f, T), new Color(0.6f, 0.4f, 0.2f)); // Marrón oscuro mostrador
    }

    void BuildKitchenStorageWall()
    {
        float x = 0f;
        float doorW = 4f;
        float sideLen = (8f - doorW) / 2f;

        CreateBox("KS_Wall_Zmin", new Vector3(x, H/2f, 0f + sideLen/2f),
                  new Vector3(T, H, sideLen), new Color(0.9f, 0.7f, 0.5f)); // Naranja cocina/almacén
        CreateBox("KS_Wall_Zmax", new Vector3(x, H/2f, 8f - sideLen/2f),
                  new Vector3(T, H, sideLen), new Color(0.9f, 0.7f, 0.5f)); // Naranja cocina/almacén
    }

    GameObject CreateBox(string name, Vector3 pos, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;

        var rend = go.GetComponent<Renderer>();
        const string urpShaderName = "Universal Render Pipeline/Lit";
        const string standardShaderName = "Standard";
        var shader = Shader.Find(urpShaderName);
        if (shader == null) shader = Shader.Find(standardShaderName);
        var mat = new Material(shader);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        rend.material = mat;

        return go;
    }

    void CreatePointLight(string name, Vector3 pos, float range, float intensity)
    {
        var go = new GameObject(name);
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.range = range;
        l.intensity = intensity;
        go.transform.position = pos;
    }
}