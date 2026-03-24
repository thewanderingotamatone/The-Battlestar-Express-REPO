using UnityEngine;
using UnityEngine.InputSystem;
using Magic;  

public class AirshipController : MonoBehaviour
{
    // Variables
    private InputAction moveAction;
    private InputAction heightAction;
    private Vector2 movementInput;
    
    [Header("Spin Module Settings")]
    public Magic.Modules.SpinModule spin; 
    public float spinSensitivity = 10f;     
    public bool useSpinModule = true;

    [Header("Slider Module Settings")]
    public Magic.Modules.SliderModule slider;
    public float sliderSensitivity = 1f;
    public bool useSliderModule = true;
    
    [Header("Movement Settings")]
    public float maxSpeed = 25f;
    public float minSpeed = 5f;
    public float accelerationRate = 5f;
    public float decelerationRate = 5f;
    public float keyboardTurnSpeed = 90f;  
    public float heightSpeed = 10f;     
    public float maxHeight = 100f;     
    public float minHeight = 0f;       
    private float currentSpeed = 5f;

    void Start()
    {
        // Move Action (WASD)
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("2DVector(mode=2)")
            .With("Up", "<Keyboard>/w")     
            .With("Down", "<Keyboard>/s")   
            .With("Left", "<Keyboard>/a")    
            .With("Right", "<Keyboard>/d"); 
        moveAction.Enable();

        // Height Action (Q/E)
        heightAction = new InputAction("Height");
        heightAction.AddCompositeBinding("1DAxis")
            .With("Positive", "<Keyboard>/q")  
            .With("Negative", "<Keyboard>/e"); 
        heightAction.Enable();
        
        //  Spin Module
        if (useSpinModule && spin == null)
        {
           Debug.LogWarning("Spin Module not assigned! Keyboard controls will be used instead.");
            useSpinModule = false;
        }

        if (useSliderModule && slider == null)
        {
           Debug.LogWarning("Slider Module not assigned! Keyboard controls will be used instead.");
            useSliderModule = false;
        }

    }
    void Update()
    {
        // Keyboard inputs
        movementInput = moveAction.ReadValue<Vector2>();
        float keyboardVertical = movementInput.y;
        float heightInput = heightAction.ReadValue<float>();
        float keyboardHorizontal = movementInput.x;

        // SPIN MODULE 
        float spinHorizontal = 0f;
        if (useSpinModule && spin != null)
        {
            spinHorizontal = Mathf.Clamp(spin.rotation / 1000f, -1f, 1f) * spinSensitivity;
        }

        float sliderVertical = 0f;
        if (useSliderModule && slider != null)
        {
            sliderVertical = Mathf.Clamp(slider.position / 1000f, -1f, 1f) * sliderSensitivity;
        }

        float finalVerticalInput = sliderVertical + keyboardVertical;
        finalVerticalInput = Mathf.Clamp(finalVerticalInput, -1f, 1f);


       
        float finalHorizontalInput = spinHorizontal + keyboardHorizontal;
    
        
        finalHorizontalInput = Mathf.Clamp(finalHorizontalInput, -1f, 1f);

        Debug.Log($"Slider: {sliderVertical:F2} | W/S: {keyboardVertical:F2} | Throttle: {finalVerticalInput:F2} | Spin: {finalHorizontalInput:F2} | Speed: {currentSpeed:F1}");

        // Throttle control 
        if (finalVerticalInput > 0.01f)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (finalVerticalInput < -0.01f)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, minSpeed);
        }
        else if (Mathf.Abs(finalVerticalInput) < 0.01f && Mathf.Abs(currentSpeed) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, decelerationRate * Time.deltaTime * 0.5f);
        }

        // Movement 
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

        if (Mathf.Abs(finalHorizontalInput) > 0.01f)
        {
            float turnSpeed = useSpinModule ? keyboardTurnSpeed * 1.5f : keyboardTurnSpeed;
            transform.Rotate(Vector3.up, finalHorizontalInput * turnSpeed * Time.deltaTime, Space.Self);
        }

        if (Mathf.Abs(finalHorizontalInput) > 0.01f)
        {
            float turnSpeed = keyboardTurnSpeed;
            if (useSpinModule && spin != null) turnSpeed *= 1.5f;  
            transform.Rotate(Vector3.up, finalHorizontalInput * turnSpeed * Time.deltaTime, Space.Self);
        }

        // Height control
        if (Mathf.Abs(heightInput) > 0.01f)
        {
            float newHeight = transform.position.y + (heightInput * heightSpeed * Time.deltaTime);
            newHeight = Mathf.Clamp(newHeight, minHeight, maxHeight);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
    }
    void OnDestroy()
    {
        moveAction?.Disable();
        heightAction?.Disable();
    }
}