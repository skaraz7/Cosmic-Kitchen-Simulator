using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;  // Prefab del cliente
    public Transform spawnPoint;       // Punto de entrada del restaurante
    public float spawnInterval = 10f;  // Cada 10 segundos aparece un cliente

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }
    
    void Start()
    {
        Debug.Log("CustomerSpawner iniciado. Intervalo: " + spawnInterval + " segundos");
        InvokeRepeating(nameof(DebugTimer), 1f, 1f);
    }
    
    void DebugTimer()
    {
        Debug.Log("Timer: " + timer.ToString("F1") + "/" + spawnInterval);
    }

    void SpawnCustomer()
    {
        if (customerPrefab != null && spawnPoint != null)
        {
            var customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
            customer.name = "Customer_" + Time.time.ToString("F0");
            customer.SetActive(true); // Activar el cliente para que sea visible
            Debug.Log("\u00a1CLIENTE CREADO! Nombre: " + customer.name + " en posición: " + spawnPoint.position + " - Activo: " + customer.activeInHierarchy);
        }
        else
        {
            Debug.LogError("ERROR: CustomerPrefab=" + (customerPrefab != null) + ", SpawnPoint=" + (spawnPoint != null));
        }
    }
}
