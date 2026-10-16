using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIFull : MonoBehaviour
{
    [Header("Componentes")]
    private Animator animator;
    private NavMeshAgent agente;
    public Transform jugador; // Dejar vac�o para buscar la c�mara VR autom�ticamente

    [Header("Configuraci�n de Rutas y Velocidad")]
    public Transform[] waypoints;
    private int indiceWaypointActual = 0;
    public float velocidadCaminar = 2f;
    public float velocidadCorrer = 5f;

    [Header("Tiempos de Pausa (Idle)")]
    [Tooltip("Tiempo breve que se quedar� quieto en cada waypoint")]
    public float minTiempoIdle = 2f;
    public float maxTiempoIdle = 4f;

    [Header("Configuraci�n de Detecci�n")]
    public float distanciaDeteccion = 10f;
    public float distanciaPerdida = 15f;

    [Header("Estado Interno (Solo lectura)")]
    public bool estaEnAlerta = false;
    private bool esperandoEnWaypoint = false;
    private Coroutine corrutinaPausa;

    void Start()
    {
        animator = GetComponent<Animator>();
        agente = GetComponent<NavMeshAgent>();

        // 1. Configuraci�n de VR: Buscar la c�mara principal si no hay jugador asignado
        if (jugador == null && Camera.main != null)
        {
            jugador = Camera.main.transform;
        }

        agente.isStopped = true; // Empieza quieto
    }

    void Update()
    {
        // Si no estamos en Chase, el enemigo se queda quieto
        if (GameManager.Instance.currentState != GameState.Chase)
        {
            agente.isStopped = true;
            animator.SetInteger("estadoMovimiento", 0); // Idle
            return;
        }

        // Cuando entramos en Chase vuelve a moverse
        agente.isStopped = false;

        // Si por alguna raz�n no hay jugador en la escena, no hacemos nada
        if (jugador == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (estaEnAlerta)
        {
            // --- ESTADO: PERSIGUIENDO ---
            agente.SetDestination(jugador.position);

            if (distanciaAlJugador > distanciaPerdida)
            {
                PerderJugador();
            }
        }
        else
        {
            // --- ESTADO: PATRULLANDO / IDLE ---
            if (distanciaAlJugador <= distanciaDeteccion)
            {
                DetectarJugador();
            }
            else if (!esperandoEnWaypoint)
            {
                // Si no est� persiguiendo, no est� esperando, y ya lleg� a su waypoint
                if (!agente.pathPending && agente.remainingDistance < 0.5f)
                {
                    // Inicia su pausa antes de ir al siguiente punto
                    corrutinaPausa = StartCoroutine(PausaEnWaypoint());
                }
            }
        }
    }

    // --- M�TODOS DE ANIMACI�N Y ESTADO ---
    public void ActivarIA()
    {
        animator.SetTrigger("Chase");
        agente.isStopped = false;

        if (waypoints.Length > 0)
        {
            IrAlSiguienteWaypoint();
        }
    }

    public void DetectarJugador()
    {
        if (!estaEnAlerta)
        {
            estaEnAlerta = true;
            esperandoEnWaypoint = false; // Cancela el estado de espera

            // Detenemos la corrutina de Idle si estaba pausado en un waypoint
            if (corrutinaPausa != null) StopCoroutine(corrutinaPausa);

            agente.isStopped = false;    // Asegurarnos de que el agente puede moverse
            agente.speed = velocidadCorrer; // Aumentar velocidad

            // Animaciones
            animator.SetBool("Running", true);
            // Opcional: asegurarnos de que no se quede en estado de animaci�n 0
            animator.SetInteger("estadoMovimiento", 1);

            Debug.Log("�Jugador detectado! Corriendo.");
        }
    }

    public void PerderJugador()
    {
        if (estaEnAlerta)
        {
            estaEnAlerta = false;

            agente.speed = velocidadCaminar; // Vuelve a velocidad normal

            // Animaciones
            animator.SetBool("Running", false);
            animator.SetInteger("estadoMovimiento", 1); // Vuelve a caminar

            Debug.Log("Jugador perdido. Volviendo a la patrulla normal.");

            // Retoma su ruta
            IrAlSiguienteWaypoint();
        }
    }

    // --- M�TODOS DE RUTA ---

    void IrAlSiguienteWaypoint()
    {
        if (waypoints.Length == 0) return;

        agente.isStopped = false; // Permite que se mueva
        agente.SetDestination(waypoints[indiceWaypointActual].position);

        // Animaci�n de caminar
        animator.SetInteger("estadoMovimiento", 1);

        // Preparamos el �ndice para el siguiente punto
        indiceWaypointActual = (indiceWaypointActual + 1) % waypoints.Length;
    }

    IEnumerator PausaEnWaypoint()
    {
        esperandoEnWaypoint = true;

        // Frenar al agente y poner animaci�n de Idle
        agente.isStopped = true;
        animator.SetInteger("estadoMovimiento", 0);

        // Esperar tiempo aleatorio
        float tiempoQuieto = Random.Range(minTiempoIdle, maxTiempoIdle);
        yield return new WaitForSeconds(tiempoQuieto);

        // Si mientras esperaba detect� al jugador, la corrutina se habr� cortado, 
        // pero por seguridad comprobamos que no est� en alerta antes de seguir.
        if (!estaEnAlerta)
        {
            esperandoEnWaypoint = false;
            IrAlSiguienteWaypoint();
        }
    }

    // Visualizar rangos en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaPerdida);
    }
}