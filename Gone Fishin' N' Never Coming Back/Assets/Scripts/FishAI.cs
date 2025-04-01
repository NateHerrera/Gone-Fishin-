using UnityEngine;

public class FishAI : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float turnSpeed = 2f;
    public float swimRadius = 5f;
    public float decisionInterval = 3f;

    private Vector3 swimTarget;
    private float decisionTimer;

    void Start()
    {
        SetNewSwimTarget();
    }

    void Update()
    {
        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f)
        {
            SetNewSwimTarget();
        }

        MoveTowardsTarget(swimTarget);
    }

    private void SetNewSwimTarget()
    {
        Vector3 randomPoint = Random.insideUnitSphere * swimRadius;
        randomPoint.y = 0f; 
        swimTarget = transform.position + randomPoint;
        decisionTimer = decisionInterval;
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * swimSpeed * Time.deltaTime;
    }
}
