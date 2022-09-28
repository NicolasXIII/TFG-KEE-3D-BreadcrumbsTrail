using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreiptCtrlEviTipo_Rama_00 : MonoBehaviour {

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // Seccion comun a todos los EVIs, para generar su EVI base 
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public GameObject EviBase;

    public float pos_x;
    public float pos_y;
    public float pos_z;
    public Vector3 posicionEvi;

    // Key y host los hacemos publicos para poder cambiarlos desde el inspector, pero deben ser privados y modificables desde la interfaz de usuario del objeto buscador
    public string key;
    public string host;

    // Fin de - Seccion comun a todos los EVIs, para generar su EVI base 
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // Seccion especifica para EviTipo_Rama_00

    public GameObject ramaAsociada;

    // Fin de - Seccion especifica para EviTipo_Rama_00
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos todos los parametros del sistema
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-04
    /// Observaciones :
    /// 		Se debe llamar al inicio. Se ejecuta antes que cualquier funcion Star, por lo que todos los parametros
    ///		deben ser inicializados aqui en lugar de los Start, donde arrancamos con lo propio de cada game objet
    ///		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Awake()
    {
        // Averiguamos el tamaño del monitor donde se presenta la aplicacion

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // Seccion comun a todos los EVIs, para generar su EVI base 

        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // Generamos la baseDeEvi que lo contendra
        EviBase = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01);
        // Colocamos el evi base como hijo del muro activo
        EviBase.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        EviBase.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
        // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
        EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_rama);
        // Cargamos la ayuda de interfaz (que es la de un evi buscador)
        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi_Especifico();

        // Fin de - Seccion comun a todos los EVIs, para generar su EVI base 
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

    }  // Fin de -  void Awake()


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos los valores que no se inicializaron en el awake
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-21
    /// Observaciones :
    /// 		Se hace lo que no se hizo en el awake
    /// 		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Start()
    {
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // Seccion comun a todos los EVIs
        // En el awake hemos generado un eviBase, con su contenedor correpondiente
        // Este metodo corre en el script perteneciente a un objeto contenido en el contenedor de el eviBase que se ha creado
        // por lo que ahora colocamos en el contenedor y dimensionamos el contenido(objeto evi) que estamos creando dentro del 
        // contenedor del evi base que hemos creado para contenerlo

        // EL contenedor "ContenedorDeEvi_01" es el primer hijo "GetChild(0)" del gameobjet base del EVI (ver observaciones en "class ScriptCtrlBaseDeEvi")
        GameObject contenedorDeEsteEvi = EviBase.transform.GetChild(0).gameObject;

        gameObject.transform.SetParent(contenedorDeEsteEvi.transform);
        gameObject.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);  // Tambien tenemos que definir el tipo como elemento de interfaz (es un EVI)

        // ////////////////////////////////////////
        // Ponemos los parametros de nuestro objeto (el contenido del contenedor que esta en el EviBase). En este caso el objeto cubre todo el contenedorDeEsteEvi y esta situado en el mismo sitio
        // misma posicion que su padre "contenedorDeEsteEvi"
        posicionEvi = new Vector3(0, 0, 0);
        transform.localPosition = posicionEvi;
        // mismo tamaño que su padre "contenedorDeEsteEvi"
        Vector3 staEscalaEviContenido = new Vector3(1, 1, 1);
        transform.localScale = staEscalaEviContenido;

        // Fin de - Seccion comun a todos los EVIs
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

        ramaAsociada = null; // Definimos la rama asociada como null, ya que todabia no se ha generado

    } // FIn de - void Start ()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Update
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-21
    /// Observaciones :
    /// 		- Controla si este evi tiene solicitudes pendientes y las atiende (marillanos)
    /// 		- Por ahora los evis de rama no tienen solicitudes que atender, pero le implemento la posibilidad por si hace falta en un futuro (MAFG 2021-03-04)
    /// 		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Update()
    {
        // Miramos si tenemos soicitudes pendientes, y si es asi, se resuelven
        if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)
        {
            foreach (GameObject solicitud in GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes)
            {
                atiendeMariLlanos(solicitud);  // Llamamos a quien debe gestionar la solicitud
            }
            GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Clear(); // Debemos habre procesado todos los elementos de la lista y esta estará vacia
                                                                                   // No se pueden borrar en la funciones a las que se llama desde aqui porque entonces casca el foreach
                                                                                   // por modificar la lista mientras se ejecuta
                                                                                   // Si hubiera que eliminar solo algunos habria que generar una lista a parte con los que hubiera que 
                                                                                   // borrar y luego borrarlos uno a uno fuera de este foreach (creo) MAFG 2021-02-14
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)

    } // Fin de - void LateUpdate ()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void atiendeMariLlanos(GameObject solicitudAAtender)
    ///     Este metodo atiende las solicictudes en las que figura como receptor.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-06
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - solicitudAAtender : es la solicitud que debe atender
    /// Variables de salida :
    ///         - No devuelve nada, solo atiende la solicitud obrando en consecuencia
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         TODOS LOS ELEMENTOS DE INTERFAZ QUE DEBEN ATENDER SOLICITUDES (como receptor), deben tener un metodo con este nombre
    ///         cada tipo de elemento de interfaz implementara en el metodo una funcionalidad distina, ya que las naturalezas de las
    ///         solicitudes y sus respuestas seran diferentes.
    ///         El gestor de solicitudes "ScriptGestorSolicitudes" de "ctrlInterfaz", repasa periodicamente la lista de solicitudes 
    ///         "ListaSolicitudes" y segun la necesidad de atencion de la solicitud llamara al 
    ///         elemento de interfaz que corresponda segun "idElementIntf"
    ///         Este metodo atiende las solicictudes en las que figura como receptor.
    ///         - - La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    public void atiendeMariLlanos(GameObject solicitudAAtender)
    {
        if (DatosGlobal.niveDebug > 100)
        { Debug.Log("ScreiptCtrlEviTipo_Rama_00 => atiendeMariLlanos"); }
    }  // FIn de - public void atiendeMariLlanos(GameObject solicitudAAtender)


    /// ////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////
    /// ////////////////////////////////////////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos, lo indicamos al usuario modificando su tamaño
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaEviTipo_Rama_00_Activado;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // OJOOOOO este triger esta destinado a generar una nueva rama, o a colocarnos en la rama asociada si ya existe

        // ///////////
        // Si pulsamos el raton estando en el evi de rama, el usuario se coloca en la rama "ramaAsociada" si ya existe, o genera una nueva si no existia
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            {
                transform.parent.parent.GetComponent<SctExpandirEvi>().botonExpandeEvi();
            }  //  Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
        } // Fin de -if (Input.GetMouseButtonDown(0))
    }  // Fin de - void OnTriggerStay(Collider other)

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaEviTipo_Rama_00;
    } // Fin de - void OnTriggerEnter(Collider other) 



}  // Fin de - public class ScreiptCtrlEviTipo_Rama_00 : MonoBehaviour 
