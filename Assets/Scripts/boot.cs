using UnityEngine;

public class Boot : MonoBehaviour
{
    [Header("Customer System")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;

    private Vector3 counterPos;
    private static bool hasInitialized = false;

    void Start()
    {
        // Evitar múltiples inicializaciones
        if (hasInitialized) 
        {
            Destroy(gameObject);
            return;
        }
        hasInitialized = true;

        // Quitar Main Camera por defecto si existe
        if (Camera.main != null) Destroy(Camera.main.gameObject);

        // Construir restaurante
        RestaurantBuilder builder = new RestaurantBuilder();
        counterPos = builder.BuildRestaurant();

        // Crear jugador
        CreatePlayer();

        // Configurar sistema de clientes
        SetupCustomerSystem();
    }

    void CreatePlayer()
    {
        // Constantes para configuración del jugador
        const float PLAYER_HEIGHT = 1.8f;
        const float PLAYER_RADIUS = 0.3f;
        const float CAMERA_EYE_LEVEL = 1.6f;
        const float SPAWN_HEIGHT = 1.0f;

        var player = new GameObject("Player");
        var cc = player.AddComponent<CharacterController>();
        cc.height = PLAYER_HEIGHT;
        cc.radius = PLAYER_RADIUS;
        cc.center = new Vector3(0, cc.height/2f, 0);

        // Agregar controlador de jugador
        var playerController = player.AddComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Failed to add PlayerController component");
            return;
        }

        // Cámara hija
        var camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        camObj.transform.SetParent(player.transform, worldPositionStays:false);
        camObj.transform.localPosition = new Vector3(0, CAMERA_EYE_LEVEL, 0);
        camObj.transform.localRotation = Quaternion.identity;
        camObj.AddComponent<Camera>();

        // Posición inicial en zona clientes (dentro del restaurante)
        player.transform.position = new Vector3(0, SPAWN_HEIGHT, -4f); // Centro zona clientes
    }

    void SetupCustomerSystem()
    {
        /* MAPA DE POSICIONES DEL RESTAURANTE:
         * 
         * COORDENADAS IMPORTANTES:
         * - Restaurante: 20m (X: -10 a +10) x 16m (Z: -8 a +8) x 3m altura
         * - Suelo principal: Y = 0 (Floor en Y = -0.1)
         * - Zona clientes: Z = -8 a 0 (frente del restaurante)
         * - Zona servicios: Z = 0 a +8 (atrás del restaurante)
         * - Puerta entrada: Z = -8 (pared frontal), ancho 2m
         * - Mostrador: Z = 0 (división horizontal), Y = 0.5 (altura barra)
         * 
         * PUNTOS CLAVE:
         * - SpawnPoint (ENTRADA): (0, 0.9, -9) - Fuera del restaurante, altura suelo
         * - CounterPoint (MOSTRADOR): (-5, 0.9, -1) - Mostrador izquierda, 5m del centro
         * - ExitPoint (SALIDA): (0, 0.9, -9) - Misma que entrada
         */
        
        // Crear puntos de referencia con flechas visuales
        if (spawnPoint == null)
        {
            var spawnGO = new GameObject("SpawnPoint");
            spawnGO.transform.position = new Vector3(0, 1.2f, -9f); // ENTRADA: Más alto para superar el escalón
            spawnPoint = spawnGO.transform;
            
            // Flecha VERDE para entrada
            CreateArrow(spawnPoint.position, Color.green, "ENTRADA");
        }

        if (counterPoint == null)
        {
            var counterGO = new GameObject("CounterPoint");
            counterGO.transform.position = new Vector3(-5f, 0.9f, -0.1f); // MOSTRADOR: Lado izquierdo, pegado a la pared
            counterPoint = counterGO.transform;
            
            // Flecha VERDE para mostrador
            CreateArrow(counterPoint.position, Color.green, "MOSTRADOR");
        }

        if (exitPoint == null)
        {
            var exitGO = new GameObject("ExitPoint");
            exitGO.transform.position = new Vector3(0, 0.9f, -9f); // SALIDA: Misma que entrada
            exitPoint = exitGO.transform;
        }

        // Crear prefab de cliente directamente
        if (customerPrefab == null)
        {
            Debug.Log("Creando customerPrefab directamente...");
            
            // Crear cubo cliente
            customerPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            customerPrefab.name = "CustomerPrefab";
            customerPrefab.transform.localScale = new Vector3(0.8f, 1.6f, 0.8f);
            
            // Color verde
            var renderer = customerPrefab.GetComponent<Renderer>();
            var material = new Material(Shader.Find("Standard"));
            material.color = Color.green;
            renderer.material = material;
            
            // Agregar CustomerAI
            var ai = customerPrefab.AddComponent<CustomerAI>();
            ai.moveSpeed = 2f;
            ai.mostrador = counterPoint;
            
            // Agregar Rigidbody
            var rb = customerPrefab.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            
            // Desactivar el prefab para que no aparezca en escena
            customerPrefab.SetActive(false);
            
            Debug.Log("CustomerPrefab creado y configurado exitosamente");
        }

        // Añadir CustomerSpawner solo si no existe
        if (GetComponent<CustomerSpawner>() == null)
        {
            var spawner = gameObject.AddComponent<CustomerSpawner>();
            spawner.customerPrefab = customerPrefab;
            spawner.spawnPoint = spawnPoint;
            spawner.spawnInterval = 10f;
            
            Debug.Log("CustomerSpawner configurado. Prefab asignado: " + (customerPrefab != null));
        }
    }
    
    void CreateArrow(Vector3 position, Color color, string label)
    {
        // Crear esfera verde incrustada en la pared
        var arrow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        arrow.name = "Marker_" + label;
        
        // Posición incrustada en la pared
        if (label == "MOSTRADOR")
        {
            arrow.transform.position = new Vector3(position.x, position.y, 0.05f); // Incrustada en pared del mostrador
        }
        else
        {
            arrow.transform.position = position; // Entrada normal
        }
        
        arrow.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f); // Esfera pequeña
        
        // Eliminar colisiones
        var collider = arrow.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        
        // Color usando método alternativo
        var renderer = arrow.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Unlit/Color"));
        renderer.material.SetColor("_Color", Color.cyan); // CYAN para que se vea
        
        Debug.Log("Esfera VERDE " + label + " creada en: " + arrow.transform.position);
    }
}