using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;  
    public float rotationSpeed = 20f;          
    private float currentRotationAngle = 0f;   
    public float maxRotationAngle = 180f;      

    void Update()
    {

        if (currentRotationAngle < maxRotationAngle)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            
            float remainingRotation = maxRotationAngle - currentRotationAngle;
            float rotationAmount = Mathf.Min(rotationThisFrame, remainingRotation);
            
            transform.Rotate(rotationAxis, rotationAmount);

            currentRotationAngle += rotationAmount;
        }
    }
}