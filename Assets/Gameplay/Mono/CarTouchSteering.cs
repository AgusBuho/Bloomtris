using UnityEngine;
using UnityEngine.EventSystems;

public class CarTouchSteering : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CarController carController;
    public float steerDirection = 1f; // 1 para derecha, -1 para izquierda
    public float steerMultiplier = 1f;

    private bool isPressed = false;
    private float pressTime = 0f;

    void Update()
    {
        if (isPressed)
        {
            pressTime += Time.deltaTime;
            float steerAmount = Mathf.Clamp01(pressTime) * steerMultiplier * steerDirection;
            carController.SetTurnInput(steerAmount);
            // Vibración
            Handheld.Vibrate(); // Solo en dispositivo real
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pressTime = 0f;
        carController.SetTurnInput(0f);
    }
}
