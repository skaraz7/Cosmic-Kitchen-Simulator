using UnityEngine;
using System.Collections;

public class CustomerAI : MonoBehaviour
{
    public Transform mostrador;   // Asigna el mostrador desde el Boot
    public float moveSpeed = 2f;

    private bool hasArrived = false;

    void Update()
    {
        if (!hasArrived && mostrador != null)
        {
            // Ir exactamente al punto del mostrador manteniendo altura correcta
            Vector3 target = new Vector3(mostrador.position.x, 1.2f, mostrador.position.z);
            float distanceToTarget = Vector3.Distance(transform.position, target);
            
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // Hacer que mire hacia donde camina
            Vector3 dir = target - transform.position;
            if (dir != Vector3.zero)
                transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * 5f);

            // Debug continuo de distancia
            Debug.Log("Cliente " + gameObject.name + " distancia al mostrador: " + distanceToTarget.ToString("F2"));
            
            // Revisar si ya llegó
            if (distanceToTarget < 1f)
            {
                hasArrived = true;
                // Al llegar, mirar directamente hacia la pared del mostrador (Z positivo)
                transform.forward = Vector3.forward;
                Debug.Log("¡¡¡Cliente " + gameObject.name + " LLEGÓ al mostrador y mira la pared!!!");
                StartCoroutine(WaitAndLeave());
            }
        }
    }

    IEnumerator WaitAndLeave()
    {
        Debug.Log("Cliente " + gameObject.name + " esperando 3 segundos en el mostrador");
        // Espera 3 segundos en el mostrador
        yield return new WaitForSeconds(3f);

        Debug.Log("Cliente " + gameObject.name + " desapareciendo");
        // Elimina al cliente
        Destroy(gameObject);
    }
}
