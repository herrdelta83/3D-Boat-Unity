using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Boat Specs")]
    public float motorPower = 50f;
    public float turnPower = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // El agente de Python (RL) llamará a esta función en cada paso de la simulación
    // acceleration: valor entre -1 (reversa) y 1 (adelante)
    // steering: valor entre -1 (izquierda) y 1 (derecha)
    public void MoveBoat(float acceleration, float steering)
    {
        // 1. Aplicar fuerza de propulsión hacia adelante/atrás
        Vector3 force = transform.forward * acceleration * motorPower;
        rb.AddForce(force, ForceMode.Acceleration);

        // 2. Aplicar torque para girar
        // Solo permitimos girar si el bote se está moviendo un poco (como un timón real)
        if (Mathf.Abs(acceleration) > 0.1f || rb.linearVelocity.magnitude > 0.5f)
        {
            Vector3 torque = transform.up * steering * turnPower;
            
            // Si vamos en reversa, invertimos el giro para que sea intuitivo
            if (acceleration < 0) torque *= -1;
            
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }
}