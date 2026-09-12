using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : Agent
{
    private Rigidbody rb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    [Header("Boat Specs")]
    public float motorPower = 50f;
    public float turnPower = 15f;

    [Header("Environment")]
    public Transform targetGoal; // Arrastraremos el objeto Goal aquí en el Inspector

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        // Guardamos la posición inicial para cuando el bote choque y deba reiniciar
        startingPosition = transform.localPosition;
        startingRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        // 1. Detener inercia
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2. Regresar el bote a la línea de salida
        transform.localPosition = startingPosition;
        transform.localRotation = startingRotation;

        // Opcional: Aquí podrías agregar código para mover los obstáculos o la meta aleatoriamente
    }

    // Le damos al agente un "GPS" interno para que sepa dónde está la meta
    public override void CollectObservations(VectorSensor sensor)
    {
        // Calculamos la dirección hacia la meta (normalizada)
        Vector3 directionToTarget = (targetGoal.localPosition - transform.localPosition).normalized;
        
        // Agregamos 3 observaciones numéricas: (Eje X, Eje Z, y qué tan alineado está el bote con la meta)
        sensor.AddObservation(directionToTarget.x);
        sensor.AddObservation(directionToTarget.z);
        sensor.AddObservation(Vector3.Dot(transform.forward.normalized, directionToTarget));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float acceleration = actions.ContinuousActions[0];
        float steering = actions.ContinuousActions[1];
        MoveBoat(acceleration, steering);

        // Recompensa pequeña por acercarse a la meta (incentiva a moverse rápido)
        // AddReward(-1f / MaxStep); // Descomenta esto si agregas un MaxStep en el editor
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    private void MoveBoat(float acceleration, float steering)
    {
        Vector3 force = transform.forward * acceleration * motorPower;
        rb.AddForce(force, ForceMode.Acceleration);

        if (Mathf.Abs(acceleration) > 0.1f || rb.linearVelocity.magnitude > 0.5f)
        {
            Vector3 torque = transform.up * steering * turnPower;
            if (acceleration < 0) torque *= -1;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    // Usamos OnCollisionEnter para colisiones físicas (obstáculos)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("OutOfBounds"))
        {
            AddReward(-1.0f); // Castigo por chocar
            EndEpisode();     // Terminar y reiniciar
        }
    }

    // Usamos OnTriggerEnter para objetos etéreos (la meta)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(1.0f); // Premio por ganar
            EndEpisode();    // Terminar y reiniciar
        }
    }
}