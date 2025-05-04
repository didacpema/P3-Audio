using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public AudioLowPassFilter filtroAgua; // El filtro que afectará al sonido
    public GameObject aguaVisual;         // GameObject que representa el agua subiendo
    public float alturaFinal = 5f;        // Altura máxima que sube el agua
    public float velocidadSubida = 1f;    // Velocidad a la que sube el agua
    public float frecuenciaLowPass = 1000f; // Frecuencia del filtro cuando estás bajo el agua

    private bool activado = false;
    private Vector3 alturaInicial;
    private bool subiendoAgua = false;
    private bool jugadorCerca = false;   // Para detectar si el jugador está cerca del botón

    void Start()
    {
        if (aguaVisual != null)
        {
            alturaInicial = aguaVisual.transform.position;
        }

        // Asegura que el filtro esté en estado normal al inicio
        if (filtroAgua != null)
        {
            filtroAgua.cutoffFrequency = 22000f; // Suena normal (sin efecto)
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F))
        {
            Inundacion();
        }
        if (subiendoAgua && aguaVisual != null)
        {
            // Lógica para subir el agua poco a poco
            Vector3 objetivo = new Vector3(
                alturaInicial.x,
                alturaInicial.y + alturaFinal,
                alturaInicial.z
            );

            aguaVisual.transform.position = Vector3.MoveTowards(
                aguaVisual.transform.position,
                objetivo,
                velocidadSubida * Time.deltaTime
            );

            // Aplicar el filtro cuando empieza a subir
            if (filtroAgua != null)
            {
                filtroAgua.cutoffFrequency = frecuenciaLowPass;
            }
        }
        else if (aguaVisual != null)
        {
            // Si el agua no está subiendo, vuelve a la posición inicial
            aguaVisual.transform.position = Vector3.MoveTowards(
                aguaVisual.transform.position,
                alturaInicial,
                velocidadSubida * Time.deltaTime
            );

            // Restablecer el filtro a su estado normal
            if (filtroAgua != null)
            {
                filtroAgua.cutoffFrequency = 22000f; // Suena normal (sin efecto)
            }
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
    // Este método lo puedes llamar desde un botón
    public void Inundacion()
    {
        activado = !activado;
        subiendoAgua = !subiendoAgua; 
        Debug.Log("¡Se ha activado la inundación, Jefe!");
        
    }
}
