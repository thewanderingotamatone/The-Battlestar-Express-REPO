using UnityEngine;
using UnityEngine.InputSystem;

public class AirshipController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction heightAction;  
    private Vector2 movementInput;
    public float maxSpeed = 20f;
    public float minSpeed = -10f;
    public float accelerationRate = 5f;
    public float decelerationRate = 5f;
    public float turnSpeed = 10f;
    public float heightSpeed = 10f;     
    public float maxHeight = 100f;     
    public float minHeight = 0f;       
    private float currentSpeed = 0f;

    void Start()
    {
        
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("2DVector(mode=2)")
            .With("Up", "<Keyboard>/w")     
            .With("Down", "<Keyboard>/s")   
            .With("Left", "<Keyboard>/a")    
            .With("Right", "<Keyboard>/d"); 
        moveAction.Enable();

    
        heightAction = new InputAction("Height", binding: "<Keyboard>/q,<Keyboard>/e");
        heightAction.AddCompositeBinding("1D Axis")  
            .With("Positive", "<Keyboard>/q")  
            .With("Negative", "<Keyboard>/e"); 
        heightAction.Enable();
    }

    void Update()
    {
      
        movementInput = moveAction.ReadValue<Vector2>();
        float horizontal = movementInput.x;
        float vertical = movementInput.y;

       
        float heightInput = heightAction.ReadValue<float>();  

        
        Debug.Log($"Horizontal: {horizontal}, Vertical: {vertical}, Height: {heightInput}, Current Speed: {currentSpeed}");

    
        if (vertical > 0.01f)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (vertical < -0.01f)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, minSpeed);
        }

        
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            transform.Rotate(Vector3.up, horizontal * turnSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(heightInput) > 0.01f)
        {
            float newHeight = transform.position.y + (heightInput * heightSpeed * Time.deltaTime);
          
            newHeight = Mathf.Clamp(newHeight, minHeight, maxHeight);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
    }

    void OnDestroy()
    {
        moveAction.Disable();
        heightAction.Disable();
    }
}