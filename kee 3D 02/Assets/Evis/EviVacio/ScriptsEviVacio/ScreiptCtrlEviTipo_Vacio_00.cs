using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control para un EVI Vacio
/// Construye y gestiona la parte general de un evi vacio, el cual habra que adaptar para que cumpla una funcion concreta
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-02-19
/// Observaciones :
/// 		- Este Objeto es una base para generar sobre el otros evis con funciones concertas (listas, busquedas, fractales, arboles, etc...)
/// 		- Este evi ha sido generado sobre la estructura que ofrece el game Object "EviTipo_Vacio_00"
///			- "OJOOO CAMBIAR" Donde aparece como parte de un comentario "OJOOO CAMBIAR", se debe adaptar el codigo para el caso concreto al que se vaya a dedicar el evi (lista, busqueda, etc...)
///			incluido estos comentarios y la documentacion en general
///			- Todos los evis que puedan recibir o enviar solicitudes, deben imlementar las funciones "atiendeMariLlanos" y "procesaRespuestaSolicitud".
/// </summary>
public class ScreiptCtrlEviTipo_Vacio_00 : MonoBehaviour {

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // 1 - Seccion comun a todos los EVIs, para declaracion de variables

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

    // Fin de - 1 - Seccion comun a todos los EVIs, para declaracion de variables
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // A partir de aqui declaracion de  variables especificas de este tipo de evi




    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos todos los parametros del sistema
    /// Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-02-19
    /// Observaciones :
    /// 		Se debe llamar al inicio. Se ejecuta antes que cualquier funcion Star, por lo que todos los parametros
    ///		deben ser inicializados aqui en lugar de los Start, donde arrancamos con lo propio de cada game objet
    ///		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Awake()
    {
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // 2 - Seccion comun a todos los EVIs, asignacion de variables

        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // Generamos la baseDeEvi que lo contendra
        EviBase = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01);
        // Colocamos el evi base como hijo del muro activo
        EviBase.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        EviBase.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
        // Cargamos la ayuda de interfaz (que es la de un evi buscador)
        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi_Especifico(); // OJOOO CAMBIAR (si es un evi especifico hay que definirlo en ScriptCtrlBaseDeEvi y hacer un metodo con sus datos, imagen, icono, etc...)


        // Fin de - 2 - Seccion comun a todos los EVIs, asignacion de variables
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////


        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Awake()"

        // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
        EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00); // OJOOO CAMBIAR (hay que poner el tio de evi que sea)

        // OJO, en este awake no se puede hacer referencia a nada del gameobject "EviTipo_Vacio_00", ya que este no se genera hasta 
        // que se ejecuta es start


    } // FIn de - void Awake()


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos los valores que no se inicializaron en el awake
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-19
    /// Observaciones :
    /// 		Se hace lo que no se hizo en el awake
    /// 		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Start()
    {
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // 3 - Seccion comun a todos los EVIs, para generar su EVI base y ahijarse en su contenedor
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
        Vector3 staEscalaEviBuscador = new Vector3(1, 1, 1);
        transform.localScale = staEscalaEviBuscador;

        // Fin de - 3 - Seccion comun a todos los EVIs, para generar su EVI base y ahijarse en su contenedor
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Start()"

    } // FIn de - void Start ()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Update
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-19
    /// Observaciones :
    /// 		- Controla si este evi tiene solicitudes pendientes y las atiende (marillanos)
    /// 		- Si este tipo de evis tiene solicitudes que atenderhay ue implemetar los metodos correspondientes dependiendo de la 
    /// 		funcionalidad de las solicitudes en un futuro (MAFG 2021-03-04)
    /// 		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// </summary>
    void Update()
    {
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // 4 - Seccion comun a todos los EVIs, para generar atender las solicitudes pendientes

        // Miramos si tenemos soicitudes pendientes, y si es asi, se resuelven
        if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)
        {
            foreach (GameObject solicitud in GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes)
            {
                atiendeMariLlanos(solicitud);  // Llamamos a quien debe gestionar la solicitud
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScreiptCtrlEviTipo_Vacio_00 => update con tamaño de lista de solicitudes : "); }
            }
            GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Clear(); // Debemos habre procesado todos los elementos de la lista y esta estará vacia
                                                                                   // No se pueden borrar en la funciones a las que se llama desde aqui porque entonces casca el foreach
                                                                                   // por modificar la lista mientras se ejecuta
                                                                                   // Si hubiera que eliminar solo algunos habria que generar una lista a parte con los que hubiera que 
                                                                                   // borrar y luego borrarlos uno a uno fuera de este foreach (creo) MAFG 2021-02-14
        } // Fin de - if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)

        // FIn de - 4 - Seccion comun a todos los EVIs, para generar atender las solicitudes pendientes
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui otro código especificos de este tipo de evi en el "void Update()"

    } // Fin de - void Update()

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
    /// </summary>
    public void atiendeMariLlanos(GameObject solicitudAAtender)
    {
        if (DatosGlobal.niveDebug > 100)
        { Debug.Log("Entro en ScreiptCtrlEviTipo_Vacio_00 => atiendeMariLlanos"); }
    }  // FIn de - public void atiendeMariLlanos(GameObject solicitudAAtender)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : procesaRespuestaSolicitud 
    /// Observaciones : Este metodo procesa la respuesta de una solicitud realizada (normlmente a un DKS).
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-13
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject solicitudAsociada : es la solicitud que genero el buscador. Esta ya esta resuelta, con lo que contiene los datos de respuesta del DKS
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         Esta funcion debe analizar el tipo de respuesta que recibe para saber si es un KDL directamente de conceto, o una lista a un arbol o lo que sea
    ///         Segun sea la naturaleza de la respuesta, generara un vevi del subtipo que corresponda (fractal, lista, arbol. etc...)
    ///         - NDIENTE MAFG 2021-02-13) Por ahora no analizamos el tipo de respuesta y consideramos que esta en un KDL de concepto, sin mirar nada, por lo que
    ///         generamos siempre un evi fractal
    /// </summary>
    public void procesaRespuestaSolicitud(GameObject solicitudAsociada)
    {

    }  // Fin de -  public void procesaRespuestaSolicitud(GameObject solicitudAsociada)
}
