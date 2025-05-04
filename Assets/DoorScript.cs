using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Transform puerta;         // Referencia al objeto que se moverá (la puerta)
    public float anguloApertura = 90f; // Grados de apertura
    public float velocidad = 2f;     // Velocidad de apertura/cierre
    public AudioSource doorSFX;   // Sonido de apertura/cierre de la puerta

    private bool jugadorCerca = false;
    private bool puertaAbierta = false;
    private Quaternion rotInicial;
    private Quaternion rotAbierta;

    void Start()
    {
        // Guarda la rotación inicial de la puerta
        rotInicial = puerta.rotation;
        // Calcula la rotación abierta (en el eje Y normalmente)
        rotAbierta = Quaternion.Euler(puerta.eulerAngles + new Vector3(0, anguloApertura, 0));
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
        {
            // Cambiar el estado de la puerta
            puertaAbierta = !puertaAbierta;
            PlayFX(); // Reproducir el sonido de la puerta
        }

        // Suavizar el movimiento de apertura/cierre
        if (puertaAbierta)
        {
            puerta.rotation = Quaternion.Lerp(puerta.rotation, rotAbierta, Time.deltaTime * velocidad);
        }
        else
        {
            puerta.rotation = Quaternion.Lerp(puerta.rotation, rotInicial, Time.deltaTime * velocidad);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Presiona F para abrir/cerrar la puerta.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
    private void PlayFX()
    {
        if (doorSFX != null && !doorSFX.isPlaying) // Verifica si el sonido no está ya reproduciéndose
        {
            doorSFX.Play();
        }
    }
}
