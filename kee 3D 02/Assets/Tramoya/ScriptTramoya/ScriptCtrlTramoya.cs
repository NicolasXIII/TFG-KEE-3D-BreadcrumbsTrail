using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control de la tramoya, dondep se mueven los telones
// Basicamente muueve la tramoya para que acompañe al usuario
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2021-05-27
// Observaciones :
// 		- Lo acompañan la camara, el foco de luz y el muro de usuario. Todos viajan como uno solo con el usuario
//		- El objetivo es que la camara lo encuadre siempre de forma que los telones puedan bajar y subir en su 
//      recorrido correspondiente, ocupando el lugar que se defina

public class ScriptCtrlTramoya : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    GameObject Telon_mochila_01; // l game objet para el telon de la mochila (por ahora solo un telon)
    public bool Telon_mochila_01_activado;  // Cuando esta variable es true, el gameobject "Telon_mochila_01" baja y sube cuando se le requiere
                                            // Cuando esta variable es false, el gameobject "Telon_mochila_01" no aparece aunque se le requiera
    public string Telon_mochila_01_estado;  // Indica si el "Telon_mochila_01" esta bajado, oculto, en transito, operativo
        public static string Telon_mochila_01_est_oculto = "oculto";  // El "Telon_mochila_01" esta oculto, y por tanto no operativo
        public static string Telon_mochila_01_est_EnTransito = "EnTransito";  // El "Telon_mochila_01" esta subiendo o bajando, y por tanto no operativo
        public static string Telon_mochila_01_est_operativo = "operativo";  // El "Telon_mochila_01" esta bajado, y por tanto visible y operativo

    float velocidadTelon;  // Es el parametro que nos permite controlar la velocidad de bajada del telon
    float sueloDeTelon;  // Es el parametro que nos permite definir hasta que punto de Y, en coordenadas de la tramoya, baja el telos (su parte de abajo)

    float ratioFranjaActivaTelon;  // Es el parametro que nos permite definir la dimension de la franja superior, donde si el telon esta activo lo hacemos visible

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // ////////////////////////////////////////
        // Asignamos los parametros de la tramoya
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTramoya;
        // Para la orientacion espacial
        this.transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().giroOrientacionTramoya;
        // Para la posicion
        transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTramoya;

        // Generamos el game objet para el telon de la mochila (por ahora solo un telon)
        Telon_mochila_01 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().telon);
        Telon_mochila_01.transform.SetParent(this.gameObject.transform);

        Telon_mochila_01.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTelones;
        // Para la orientacion espacial
        Telon_mochila_01.transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().giroOrientacionTelones;
        // Para la posicion
        Telon_mochila_01.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTelones;

        Telon_mochila_01_activado = false; // Inicializamos la variable
        Telon_mochila_01_estado = Telon_mochila_01_est_oculto;

        velocidadTelon = 1f/40f;
        sueloDeTelon = 0f;  // Para que ocupe toda la tramoya
        ratioFranjaActivaTelon = 1f / 10f;

    }  // Fin de - void Start()

    void Update()
    {
        // Controlamos los telones, decidiendo cual debe estar abajo y cual arriba
        // En principio solo tenemos un telon de mochila, por lo que solo controlamos este. Luego deberemos 
        // poner mas, para definir mochilas mas especificas (de personas, de localizaciones, mas recientes, etc...) PENDIENTE MAFG 2021-05-28

        // Gestion del telon de mochila general. 
        // Si "Telon_mochila_01_activado" esta a true, 
        //      - Cuando el raton se acerca a la parte superior de la pantalla
        //          - Si el telon esta arriba baja (el raton quedara dentro del telon). Y no sube hasta que el raton sale del telon por abajo
        //          - Si el telon esta abao, permanece bajado  (el raton quedara dentro del telon). Y no sube hasta que el raton sale del telon por abajo


        float mouse_Y = Input.mousePosition.y;
        float pixels_y_Pantalla = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().pixels_y_Pantalla;
        float inicioFranjaActivaTelon = pixels_y_Pantalla - (pixels_y_Pantalla * ratioFranjaActivaTelon);
        float sueloTramoya = pixels_y_Pantalla - (pixels_y_Pantalla * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ratioOcupacionTramoya);

        // Si el telon esta activado pero todabia no ha bajado
        if ((Telon_mochila_01_activado == true) & (Telon_mochila_01_estado == Telon_mochila_01_est_oculto))
        {
            if (mouse_Y > inicioFranjaActivaTelon)
            {
                StartCoroutine(muestraTelon(Telon_mochila_01));
                // ajustamos el estado de los punteros, para que en la tramoya actue el puntero de la tramoya y no el de usuario
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroTramoya.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_NoActivaEvis;
            }
        }
        // Si el telon esta activado pero esta arriba porque nos hemos salido de este con anterioridad
        if ((Telon_mochila_01_activado == true) & (Telon_mochila_01_estado == Telon_mochila_01_est_operativo))
            {
            if (mouse_Y < sueloTramoya)
            {
                StartCoroutine(ocultaTelon(Telon_mochila_01));
                // ajustamos el estado de los punteros, fuera de la tramoya el que trabaja es el puntero de usuario
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroTramoya.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_desactivado;
            }
        }
        // Si hemos desactovado el telon, lo ocultamos en cualquier caso. Cuando este desactivado no debe bajarse
        if ((Telon_mochila_01_activado == false) & (Telon_mochila_01_estado == Telon_mochila_01_est_operativo)){StartCoroutine(ocultaTelon(Telon_mochila_01));}

    } // Fin de - void Update()


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// //////  void Awake()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-06-02 ( lo cojo de "ScriptCtrlMuroTrabajo")
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     - GameObject ObjetoPadre : Es el objeto de la tramoya, donde debe situarse el elemento que lo llama)
    /// Variables de salida :
    /// Observaciones:
    /// </summary>
    public Vector3 damePosicionIniLocal(GameObject ObjetoPadre)
    {
        // Por ahora, como en la tramoya solo tenemos el telon de la mochila, pasamos la posicion independientemente del padre
        // OJOOOO PENDIENTE MAFG 2021-06-02, cuando haya mas elementos en la tramoya habra que generar la posicion en consecuencia
        // ////////////////////////////////////////
        // Para la posicion en el muro
        float margen = 20f; // es el tanto por ciento que dejo de margen para que no se me vallan los evis al borde, que no puedo manejarlos
        float hastaDonde = 0.5f - (0.5f * margen / 100f);

        float pos_x = Random.Range((-1) * hastaDonde, hastaDonde);
        float pos_y = Random.Range((-1) * hastaDonde, hastaDonde);
        float pos_z = 0.0f;
        Vector3 posicionLocalAObjetoPadre = new Vector3(pos_x, pos_y, pos_z);

        return posicionLocalAObjetoPadre;
    } // Fin de - void Update ()


    /// //////////////////////////////////////////////////////////////////////////////////////  
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ////////  Corrutinas


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  muestraTelon : Baja el telon que se le indique hasta ocupar la tramoya
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-05-28
    /// Inputs;
    ///     -  GameObject telonAMostrar : es el telon que hay que bajar
    /// Observaciones :
    /// <summary>
    //    public void muestraTelon(GameObject telonABajar)
    IEnumerator muestraTelon(GameObject telonAMostrar)
    {
        telonAMostrar.SetActive(true);
        Vector3 incrementoEnY = new Vector3(0.0f, velocidadTelon, 0.0f); // A la distancia correspondiente del usuario y ocupando la parte superior de la pantalla

        while (telonAMostrar.transform.localPosition.y > sueloDeTelon)
        {
            telonAMostrar.transform.localPosition = telonAMostrar.transform.localPosition - incrementoEnY;
            yield return null;
        }
        Telon_mochila_01_estado = Telon_mochila_01_est_operativo;
    }  // Fin de - public void iniciaTransicion(int direccion, float distancia)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  muestraTelon : Baja el telon que se le indique hasta ocupar la tramoya
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-05-28
    /// Inputs;
    ///     -  GameObject telonAOcultar : es el telon que hay que ocultar
    /// Observaciones :
    /// <summary>
    //    public void muestraTelon(GameObject telonABajar)
    IEnumerator ocultaTelon(GameObject telonAOcultar)
    {
        Telon_mochila_01_estado = Telon_mochila_01_est_oculto;
        Vector3 incrementoEnY = new Vector3(0.0f, velocidadTelon, 0.0f); // A la distancia correspondiente del usuario y ocupando la parte superior de la pantalla
        float posicionTelosArriba = sueloDeTelon + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_Telones;

        while (telonAOcultar.transform.localPosition.y < posicionTelosArriba)
        {
            telonAOcultar.transform.localPosition = telonAOcultar.transform.localPosition + incrementoEnY;
            yield return null;
        }
        telonAOcultar.SetActive(false);
    }  // Fin de - public void iniciaTransicion(int direccion, float distancia)

}  // Fin de - public class ScriptCtrlTramoya : MonoBehaviour
