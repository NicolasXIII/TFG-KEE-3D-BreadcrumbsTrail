using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;

public class ScreiptCtrlEviTipo_buscador_00 : MonoBehaviour {

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

    // Variables para gestionar el periodo refractario del triger y la activacion de los puntoros que nos disparan
    private bool enTriger; // Estamos dentro del triger (el ontrigerStay me daba problemas. Funciona mejor asi)
    private GameObject quienDispara;  // Para enviarlo al gestor del periodo refractario, que controla tambien si el puntero esta activado

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos todos los parametros del sistema
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-21
    /// Observaciones :
    /// 		Se debe llamar al inicio. Se ejecuta antes que cualquier funcion Star, por lo que todos los parametros
    ///		deben ser inicializados aqui en lugar de los Start, donde arrancamos con lo propio de cada game objet
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

        enTriger = false;
        quienDispara = null;

        // Generamos la baseDeEvi que lo contendra
        EviBase = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01);
        // Colocamos el evi base como hijo del muro activo
        EviBase.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        EviBase.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);

        // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
        EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00);
        //        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi_Especifico();

        // Ponemos el estado de evi en etipo asignado para que comience la carga
        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = ScriptCtrlBaseDeEvi.estadoEvi_asignadoTipo;

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

        // Ponemos un key y hst a pelo para pruebas PENDIENTE ( MAFG 2021-02-04)
        // host = "http://www.ideando.net/klw/dks_klw";
        host = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";  // Para que se adapte al host en el que se trabaja
        //  key = "gen_recMaq";
        //  key = "gen_ayudaInterfaz";
        //  key = "gen_recAyuIntf";
        //  key = "gen_tipoDeSinTechoTextoPlano";
        //  key = "gen_concepto";
        //  key = "gen_ParaConceptoNuevo";
        //  key = "gen_BuscadorKee";
        key = "gen_BuscadorKee_por_key_host";


        // host = "http://"+ConceptosConocidos.localizacionBase_001+"/klw/dks_desarrollo";  // Para que se adapte al host en el que se trabaja
        //        key = "gen_ventana";
        //        key = "gen_mikiDatosLudicos";
        //        key = "gen_miCasa";
        //        key = "gen_miki";
        //        key = "gen_prueba_001";






    } // FIn de - void Start ()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Update
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-21
    /// Observaciones :
    /// 		- Controla si este evi tiene solicitudes pendientes y las atiende (marillanos)
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

        if (enTriger)
        {
            if (Input.GetMouseButtonDown(0))
                {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Desde ScreiptCtrlEviTipo_buscador_00=> Update mouse pulsado en frame : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
                {
                    // Por ahora solo golicitamos desde el buscador el get details de un concepto mediante el Key y el host

                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde ScreiptCtrlEviTipo_buscador_00=> Update mouse pulsado despues de - gestionaEnPerRefracBotonMouse - en frame : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

                    // Realizamos la solicitud del get details
                    ctrlInterfaz.GetComponent<ScriptConexionDKS>().solicitaGetDetails(key, host, this.gameObject, this.gameObject);

                }  // Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
            }  //  FIn de - if (Input.GetMouseButtonDown(0))
        }  // Fin de - if (entriger)

    } // Fin de - void LateUpdate ()

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
    ///         Segun sea la naturaleza de la respuesta, generara un evi del subtipo que corresponda (fractal, lista, arbol. etc...). SI NO CONOCE EL CONCEPTO
    ///         QUIERE DECIR QUE NO ES UN CONCEPTO QUE SEPA PONER EN UN EVI ESPECIFICO, LUEGO CUALQUIER CONCEPTO DESCONOCIDO SE PONE COMO FRACTAL
    ///         - PENDIENTE MAFG 2021-02-13) Por ahora no analizamos el tipo de respuesta y consideramos que esta en un KDL de concepto, sin mirar nada, por lo que
    ///         generamos siempre un evi fractal
    /// </summary>
    public void procesaRespuestaSolicitud(GameObject solicitudAsociada)
    {
        // La respuesta que viene en la solicitud esta en la variable DOM "respuesta_Dom_solicitud"
        XmlDocument domRespuesta = solicitudAsociada.GetComponent<ClassSolicitud>().respuesta_Dom_solicitud;

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde ScreiptCtrlEviTipo_buscador_00=>procesaRespuestaSolicitud 278. He entrado  en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        // Ahora analizamos "domRespuesta" para ver si nos manda un concepto, una lista, u otra cosa

        // PENDIENTE. Por ahora asumimos que es un fractal

        // Hasta ahora, al DKS se acecde por el servicio de "GETDetails", el cual devuelve directamente el KDL del concepto. Pero en un futuro, al 
        // DKS se le podran realizar preguntas mas elavoradas que ontendran respuestas mas elaboradas (como una lista de conceptos, un arbol, una serie de ramas deo
        // conexion o un concepto simple). En esta situacion SERA NECESARIO PROCESAR EL KDL RECIBIDO DE RESPUESTA PARA VER SI LO QUE RECIBIMOS ES UN CONCEPTO
        // SIMPLE, UNA LISTA, UNA RAMA, U OTRA COSA. Y segun lo que se reciba, actuar en consecuencia a la hora de construir el EVI que debe mostrarlo, ya que segun se reciba
        // una u otra cosa, habra que generar :
        //          - un elemento de interfaz tipo EVI, subtipo Fractal, si es un concepto (o CUALQUIERA QUE INSTANCIA ALGO QUE NO CONOCEMOS)
        //          - un elemento de interfaz tipo EVI, subtipo lista, si es una lista
        //          - un elemento de interfaz tipo EVI, subtipo arbol, si es un arbol
        //
        // POR AHORA, no analizamos la respuesta (ya que solo pedimos un getdetails) y ponemos directamente un EVI subtipo fractal
        // PENDIENTE (MAFG 2021-02-12) habra que implementar esto para poner listas y arboles. Un primer paso puede ser hacer llamadas especificas como esta
        //  OJOOOO el evi Generador (fractal, por ejemplo) instancia un evi base al que el pone el subtipo que quiere. Por lo que puede ser el el que analice el KDL que ha llegado como respuesta


        // //////////////////////////////////////////////////////////
        // //////////////////////////////////////////////////////////
        // SI ES UN CONCEPTO QUE NO INSTANCIA NADA CONOCIDO (lista, arbol,...) GENERAMOS UN EVI FRACTAL
        
        GameObject eviContenidoFractal = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);
            // Ponemos el KDL que descruibe el concepto
        eviContenidoFractal.GetComponent<SctCtrlEviTipo_Fractal_01>().KdlConcepto = domRespuesta;
        eviContenidoFractal.GetComponent<SctCtrlEviTipo_Fractal_01>().nodo_E_eviFractal = domRespuesta.DocumentElement;  
        eviContenidoFractal.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_recibidoKDL;  // Le indicamos que ya tiene su kdl

        eviContenidoFractal.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalReferencia); // Indicamos que contiene una referencia a un concepto

        //  Anota la hora en la que comienza el interviniente 1 (RECEPTOR)
        solicitudAsociada.GetComponent<ClassSolicitud>().listaIntervinientes[1].hora_fin_intervencion = DateTime.Now;
        // Indicamos que intervencion en la solicitud ha terminado la solicitud ya ha sido atendida
        solicitudAsociada.GetComponent<ClassSolicitud>().listaIntervinientes[1].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
        ///
    }  // Fin de -  public void procesaRespuestaSolicitud(GameObject solicitudAsociada)



    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaEviTipo_buscador_00_Activado;
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScreiptCtrlEviTipo_buscador_00 => OnTriggerEnter - entro en el triger del buscador : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

        enTriger = true;
        quienDispara = other.gameObject;


    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaEviTipo_buscador_00;
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScreiptCtrlEviTipo_buscador_00 => OnTriggerExit - salgo del triger del buscador : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

        enTriger = false;
        quienDispara = null;

    } // Fin de - void OnTriggerEnter(Collider other) 

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// ///////////////////////////////////
    //   Vamos con la gestion de solicitudes

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void atiendeMariLlanos(GameObject solicitudAAtender)
    ///     Este metodo atiende las solicictudes en las que figura como receptor.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-06
    /// Ultima modificacion :
    ///         2021-08-12. La adapto al nuevo modelo de solicitud
    /// Variables de entrada :
    ///         - solicitudAAtender : es la solicitud que debe atender
    /// Variables de salida :
    ///         - No devuelve nada, solo atiende la solicitud obrando en consecuencia
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         TODOS LOS ELEMENTOS DE INTERFAZ QUE DEBEN ATENDER SOLICITUDES, deben tener un metodo con este nombre
    ///         cada tipo de elemento de interfaz implementara en el metodo una funcionalidad distina, ya que las naturalezas de las
    ///         solicitudes y sus respuestas seran diferentes.
    ///         El gestor de solicitudes "ScriptGestorSolicitudes" de "ctrlInterfaz", repasa periodicamente la lista de solicitudes 
    ///         "ListaSolicitudes" y segun la necesidad de atencion de la solicitud llamara al 
    ///         elemento de interfaz que corresponda segun "idElementIntf"
    /// </summary>
    public void atiendeMariLlanos(GameObject solicitudAAtender)
    {
        // el "tipoElementIntf" debe ser "tipoElemItf_solicitud"
        // Segun el subtipo (tipo de solicitud) habra que atenderla como proceda
        string subTipoElementIntf = solicitudAAtender.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
        string codigoDeSolicitud = solicitudAAtender.GetComponent<ClassSolicitud>().codigoDeSolicitud;


        // Si el "subTipoElementIntf" es "subTipoSolicitud_RespBtn_Consulta_Si_No" es una pregunta de si o no y se atiende como sigue
        if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)  // Si se ha recibido bien
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Estoy en ScreiptCtrlEviTipo_buscador_00 => atiendeMariLlanos en 447. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }

            //            string respuestaUsuario = solicitudAAtender.GetComponent<ClassSolicitud>().respuesta_txt_solicitud[0];  // Aqui viene la respuesta SI o No del usuario

            // Los codigos de solicitud para el tipo "subTipoSolicitud_consultaKdlADks". Estos son :
            //      - codigoDeSolicitud => "CodigoSol_consultaKdlADks_GetDetails"
            //      - Por ahora no hay mas (2021-08-12)
            if (codigoDeSolicitud == ClassSolicitud.CodigoSol_consultaKdlADks_GetDetails)  // Si se ha recibido bien
            {
                procesaRespuestaSolicitud(solicitudAAtender);

            }
            else if (solicitudAAtender.GetComponent<ClassSolicitud>().estado_solicitud == ClassSolicitud.estado_error)  // Si se ha producido algun error en la recepcion
            {
                if (DatosGlobal.niveDebug > 100)
                { Debug.Log("Estoy en ScreiptCtrlEviTipo_buscador_00 => atiendeMariLlanos redibido MAL con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
            }
            else
            {
                if (DatosGlobal.niveDebug > 100)
                { Debug.Log("Entro en ScreiptCtrlEviTipo_buscador_00 => atiendeMariLlanos y salgo por el else con estado : " + solicitudAAtender.GetComponent<ClassSolicitud>().estado_solicitud); }
            }
        } // FIn de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks) 
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Estoy en ScreiptCtrlEviTipo_buscador_00 => atiendeMariLlanos fuera del if con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
        }

    }  // FIn de - public void atiendeMariLlanos(GameObject solicitudAAtender)

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers


} // Fin de - public class ScreiptCtrlEviTipo_buscador_00 : MonoBehaviour {
