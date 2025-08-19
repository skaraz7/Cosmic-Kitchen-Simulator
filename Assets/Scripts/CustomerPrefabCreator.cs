using UnityEngine;

public class CustomerPrefabCreator : MonoBehaviour
{
    public static GameObject CreateCustomerPrefab()
    {
        // Crear cubo cliente
        GameObject customer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        customer.name = "CustomerPrefab";
        
        // Configurar tamaño y posición
        customer.transform.localScale = new Vector3(0.8f, 1.6f, 0.8f); // Más parecido a persona
        
        // Color verde para identificar clientes
        var renderer = customer.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Standard"));
        material.color = Color.green;
        renderer.material = material;
        
        // Agregar CustomerAI
        var ai = customer.AddComponent<CustomerAI>();
        ai.moveSpeed = 2f;
        
        // Agregar Rigidbody para física
        var rb = customer.AddComponent<Rigidbody>();
        rb.freezeRotation = true; // No rotar al moverse
        
        return customer;
    }
}