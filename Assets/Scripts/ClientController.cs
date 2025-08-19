using UnityEngine;

public class ClientController : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 target;

    public void Init(Vector3 destination)
    {
        target = destination;
    }

    void Update()
    {
        // Movimiento simple hacia el mostrador
        Vector3 dir = target - transform.position;
        dir.y = 0; // mantener en el suelo

        if (dir.magnitude > 0.1f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
