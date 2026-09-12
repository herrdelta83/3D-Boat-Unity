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
    public Transform targetGoal; 
    public Transform forwardReference; //NUEVO: Usaremos el SensorFrontal como brújula verdadera

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.localPosition;
        startingRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = startingPosition;
        transform.localRotation = startingRotation;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 directionToTarget = (targetGoal.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(directionToTarget.x);
        sensor.AddObservation(directionToTarget.z);
        
        // CORRECCIÓN: Usamos la flecha azul del sensor para saber a dónde apuntamos
        sensor.AddObservation(Vector3.Dot(forwardReference.forward.normalized, directionToTarget));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float acceleration = actions.ContinuousActions[0];
        float steering = actions.ContinuousActions[1];
        MoveBoat(acceleration, steering);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    private void MoveBoat(float acceleration, float steering)
    {
        // CORRECCIÓN: Propulsión hacia la flecha azul del SensorFrontal
        Vector3 force = forwardReference.forward * acceleration * motorPower;
        rb.AddForce(force, ForceMode.Acceleration);

        if (Mathf.Abs(acceleration) > 0.1f || rb.linearVelocity.magnitude > 0.5f)
        {
            // CORRECCIÓN: Giro usando la flecha verde del SensorFrontal
            Vector3 torque = forwardReference.up * steering * turnPower;
            if (acceleration < 0) torque *= -1;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("OutOfBounds"))
        {
            AddReward(-1.0f); 
            EndEpisode();     
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(1.0f); 
            EndEpisode();    
        }
    }
}