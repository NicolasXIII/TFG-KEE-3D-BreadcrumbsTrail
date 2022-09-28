using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;


/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control para un EVI fractal de referencia
/// Construye y gestiona un evi fractal de referencia
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-03-16
/// Observaciones :
/// 		- Este evi ha sido generado sobre la estructura que ofrece el game Object "EviTipo_Vacio_00"
///			- Todos los evis que puedan recibir o enviar solicitudes, deben imlementar las funciones "atiendeMariLlanos" y "procesaRespuestaSolicitud".
///			
///         - OJOOO el gameObnect "EviTipo_RefFractal_01" no tiene un componente MeshRender, Esto es porque en principio es solo un contenedor de cosas 
///         que si deben verse, pero el contenedor no tiene que verse, Ademas, si se le pone, oculta la imagen de fondo (imagen de ayuda a interfaz, propia 
///         de la base del evi que tiene la imagen de fondo del concepto
/// </summary>
public class SctCtrlEviTipo_Fractal_01 : MonoBehaviour {

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // 1 - Seccion comun a todos los EVIs, para declaracion de variables

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public GameObject EviBase;
    public GameObject EviTipo_RF01_FractumRef;

    public float pos_x;
    public float pos_y;
    public float pos_z;
    public Vector3 posicionEvi;

    // Fin de - 1 - Seccion comun a todos los EVIs, para declaracion de variables
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // A partir de aqui declaracion de  variables especificas de este tipo de evi


        // Datos de identificacion del concepto que visualiza
        // Key y host los hacemos publicos para poder cambiarlos desde el inspector, pero deben ser privados y modificables desde la interfaz de usuario del objeto buscador
    public string key; // Key (KDL:I => K ) del concepto que visualiza
    public string host; // host (KDL:I => H ) del concepto que visualiza
    public string cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
    public string ordinalConf; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
    public DateTime ultiModConf; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

    public XmlDocument KdlConcepto; // Esto es el KDL del concepto general al que pretenece el evi (OJO el evi puede serlo de un enlace de instancia o un sin techo que pertenece a un concepto)
                                    //  este KDL lo es, NO SOLO del enlace de esta instancia o sin techo, si no del concepto completo. Si es referencia es el KDL del concepto asociado al evi completo
    public XmlNode nodo_E_eviFractal; // Este es el enlace, DENTRO DE KdlConcepto, que corresponde a la instancia o al sin techo que visualiza este evi fractal. Hay que tener en cuenta que este evi
                                      // puede visualizad un a instancia o sin techo que no es un concepto, si no un enlace dentro de la descripcion de este. En este caso
                                      //      "KdlConcepto" es el kdl del concepto completo donde la instancia o el sin techo es una rama 
                                      //      "nodo_E_eviFractal" es el el enlace (dentro de "KdlConcepto"), asociado a la instancia o el sin techo que visualiza este evi
                                      // OJOO cada fractum, contiene otro enlace que se almacena en "SctCtrlEviTipo_01Fractum - nodo_E_Fractum" que corresponde al enlace que visualiza el fractum

    // Estados operativos dee EVI contenedor
    public string estOpContenedor; // indica los estados de carga y operativos
                                   // Sus valores posibles son :
        public static string estOpCont_sinIniciar = "sinIniciar";  // El evi ha sido generado, pero estamos aun en el AWAKE. No hemos hecho nada mas
        public static string estOpCont_antesDeSolicitarKDL = "antesDeSolicitarKDL";  // El evi esta listo para solicitar el KDL de la descripcion que visualiza l DKS correspondiente
        public static string estOpCont_solicitadoKDL = "solicitadoKDL";  // Se ha cursado la slicitud Get Details al DKS, pero todabia no se ha obtenido un a respuesta KDL
        public static string estOpCont_recibidoKDL = "recibidoKDL";  // El KDL de respuesta a la solicitud Get Details ya ha sido recibido, por lo que el evi esta listao para cargar los datos del KDL
        public static string estOpCont_enCarga = "enCarga";  // El evi esta realizando el proceso de lleer los datos del KDL para ponerlos como informacion en el gameobject (evi del concepto fractal)
        public static string estOpCont_errorEnCargaKDL = "errorEnCargaKDL";
        public static string estOpCont_generandoDescFractal = "generandoDescFractal";  // El evi esta generando su estructura fractal interna. Osea generando los gameobject internos con la descripcion del concepto
        public static string estOpCont_cargado = "cargado";
        public static string estOpCont_listo = "listo";


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

        // Cargamos ahora el subtipo en el evi base
        EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinDefinir);

        // Ponemos el estado de evi en etipo asignado para que comience la carga
        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = ScriptCtrlBaseDeEvi.estadoEvi_asignadoTipo;

        // Fin de - 2 - Seccion comun a todos los EVIs, asignacion de variables
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////


        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Awake()"

        // DATOS DEL EVI CONTENEDOR (dentro del evi base)
        // Ponemos el tipo y el subtipo de este evi (un evi contenedor refFractal_01, que a su vez quedara contenido en su evi base)
        GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);  // Tambien tenemos que definir el tipo como elemento de interfaz (es un EVI)
        GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinDefinir);
        
        estOpContenedor = estOpCont_sinIniciar;

        GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinDefinir);

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

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Dede SctCtrlEviTipo_Fractal_01 =>Start. He llegado en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame);

            Debug.Log("Dede SctCtrlEviTipo_Fractal_01 =>Start. mi naturaleza es  = " + GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf());
        }

        // EL contenedor "ContenedorDeEvi_01" es el primer hijo "GetChild(0)" del gameobjet base del EVI (ver observaciones en "class ScriptCtrlBaseDeEvi")
        GameObject contenedorDeEsteEvi = EviBase.transform.GetChild(0).gameObject;

        gameObject.transform.SetParent(contenedorDeEsteEvi.transform);
        gameObject.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);  // Tambien tenemos que definir el tipo como elemento de interfaz (es un EVI)
        // El subtipo de evi debe ponerlo quien lo genera, que es quien sabe el subtipo de evi que esta generando (referencia, instancia, fractal,...)

        // ////////////////////////////////////////
        // Ponemos los parametros de nuestro objeto (el contenido del contenedor que esta en el EviBase). En este caso el objeto cubre todo el contenedorDeEsteEvi y esta situado en el mismo sitio
        // misma posicion que su padre "contenedorDeEsteEvi"
        posicionEvi = new Vector3(0, 0, 0);
        transform.localPosition = posicionEvi;
        // mismo tamaño que su padre "contenedorDeEsteEvi"
        Vector3 estaEscalaEvi_RefFractal_01 = new Vector3(1, 1, 1);
        transform.localScale = estaEscalaEvi_RefFractal_01;

        // Definimos las rotaciones
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Ponemos el tipo de evi en el evi base para que se configure como procede (color de botones, etc)
        string miNaturaleza = GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();

        if (miNaturaleza == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalReferencia)
        { EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal); }
        else if (miNaturaleza == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalInstancia)
        { EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal); }  
        else if (miNaturaleza == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho)
        {
            EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00);

            // Los evi Sin Techo tienen una forma distinta, ya que no albergan un fractal, sino solo el tipo de dato y el dato, asi como la funcionalidad de 
            // ejecutar la accion que proceda segun el tipo de dato (texto, url, fichero de imagen,, de audio, etc...)
            // El tamaño que hay que cambiar es el del contenedor, que es el padre de este evi (que es el cotenido)
                // La informacion necesaria para ajustar el tamaño del contenedor esta en el evi base (abuelo de este evi)
            float escalaAncho = this.transform.parent.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoContenedorDeEvi_01;
            float escalaAlto = this.transform.parent.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoContenedorDeEvi_01;
            float escalaProfundo = this.transform.parent.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaProfundoContenedorDeEvi_01;
            // Ajustamos con los valores de escala
            escalaAncho = escalaAncho * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().razon_escalaLocal_x_ContenedorEviSinTecho;
            escalaAlto = escalaAlto * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().razon_escalaLocal_y_ContenedorEviSinTecho;
            escalaProfundo = escalaProfundo * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().razon_escalaLocal_z_ContenedorEviSinTecho;
                // Ponemos la escala
            this.transform.parent.transform.localScale = new Vector3(escalaAncho, escalaAlto, escalaProfundo);
            // Para la posicion, queremos ponerlo  a la derecha de la botonera
            float escalaBoton = this.transform.parent.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoBoton;
//            this.transform.parent.transform.localPosition = new Vector3((escalaAncho / 2f + escalaBoton), (escalaAlto / 2f - escalaBoton), 0f);
            this.transform.parent.transform.localPosition = new Vector3((escalaAncho / 2f + escalaBoton), escalaBoton, 0f);
        }
        else if (miNaturaleza == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinDefinir)
        { EviBase.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_sinDefinir); }
        else {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Error desde generaEviCompleto, naturaleza del contenido desconocida - con naturalezaDeContenido = " + miNaturaleza); }
        }

        // Fin de - 3 - Seccion comun a todos los EVIs, para generar su EVI base y ahijarse en su contenedor
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Start()"

        StartCoroutine(generaEviCompleto());  // Hay que construir el evi mediante una corrutina porque este cuadro, el evi base no esta todabia disponibel

    } // FIn de - void Start ()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Update
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-19
    /// Observaciones :
    /// 		- Controla si este evi tiene solicitudes pendientes y las atiende (marillanos)
    /// 		- Si este tipo de evis tiene solicitudes que atender hay que implemetar los metodos correspondientes dependiendo de la 
    /// 		funcionalidad de las solicitudes en un futuro (MAFG 2021-03-04)
    /// 		- La copio directamente de el mismo metodo de "ScreiptCtrlEviTipo_buscador_00  2020-03-21 "
    /// 		
    /// 		- GESTIONA LA AUTOCARGA DEL CONCEPTO A VISUALIZAR. Para ello realiza los pasos siguientes
    /// 		    1.) Si esta en ESTADO PENDIENTE DE CARGA "estOpContenedor == estOpCont_antesDeSolicitarKDL" es que todabia no tenemos el KDL del concepto.
    /// 		        por lo que lo pedimos al servidor con una solicitud de getDetailas
    /// 		        - Cuando ha llegado el KDL de descripcion del concepto pasamos a ESTADO DE KDL RECIBIDO "estOpContenedor == estOpCont_recibidoKDL"
    /// 		    2.) Si estamos en ESTADO DE KDL RECIBIDO "estOpContenedor == estOpCont_recibidoKDL" cumplimentamos la informacion del cncepto con la estructura adecuada
    /// 		        2.1.) Cargamos los datos básicos de ayuda a interfaz del concepto
    /// 		        2.2.) Generamos la estructura fractal del concepto con sus elementos de descripción
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
                { Debug.Log("Estoy en SctCtrlEviTipo_Fractal_01 => update con tamaño de lista de solicitudes : " + GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count); }
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

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // Si la carga del concepto esta pendiente, procedemos a cargarlo
        if (estOpContenedor == estOpCont_antesDeSolicitarKDL)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Dede SctCtrlEviTipo_Fractal_01 =>Update. Iniciamos la corrutina en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }
            StartCoroutine(obtenKdlDeGetDetails());
            //            estOpContenedor = estOpCont_solicitadoKDL;

        }  // FIn de - if (estOpContenedor == estOpCont_antesDeSolicitarKDL)
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // Si ya tenemos KDL, procedemos a cargar la informacion del conceotp en este evi fractal de referencia
//        else if (estOpContenedor == estOpCont_recibidoKDL)
        if (estOpContenedor == estOpCont_recibidoKDL)
        {
            EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio = KdlConcepto;
            EviBase.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi = nodo_E_eviFractal;
            EviBase.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = ScriptCtrlBaseDeEvi.estadoEvi_domRecibido;  // Indicamos al evi base que ya tiene los datos listos

            estOpContenedor = estOpCont_enCarga; // Cambiamos el estado para no volver a cargar los datos de evi base
        }  // FIn de - else if (estOpContenedor == estOpCont_recibidoKDL)
           // /////////////////////////////////////////////////////////
           // /////////////////////////////////////////////////////////
           // Cuando ya hemos lanzado la carga de los datos del evi base, arrancamos con la generacion de la descripcion fractal
           //        else if (estOpContenedor == estOpCont_enCarga)
        if (estOpContenedor == estOpCont_enCarga)
        {
            estOpContenedor = estOpCont_generandoDescFractal; // Cambiamos el estado para no volver a cargar los datos de evi base
            string tipoDeFractal = GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();

            if (tipoDeFractal == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalReferencia)
            {
                XmlNode KDL_Nodo_C = EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio.DocumentElement; // COmo lo vemos como un get details, la raiz del KDL que hemos obtenido es la raiz del concepto
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().desarrollaFractal(EviBase, this.gameObject, KdlConcepto, KDL_Nodo_C);
            }
            else if (tipoDeFractal == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalInstancia)
            {
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().desarrollaFractal(EviBase, this.gameObject, KdlConcepto, nodo_E_eviFractal);
            }
            else if (tipoDeFractal == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho)
            {
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().construyeContenidoSinTecho(EviBase, this.gameObject, KdlConcepto, nodo_E_eviFractal);
            }
            else
            {
                if (DatosGlobal.niveDebug > 100)
                { Debug.Log("SctCtrlEviTipo_Fractal_01 => update tipoDeFractal desconocido = " + tipoDeFractal); }
            }

        }  // FIn de - else if (estOpContenedor == estOpCont_enCarga)
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // Cuando ya hemos desarrollado por completo la estructura fractal, el contenedor esta listo, si la base del evi esta lista, ponemos el evi com ooperativo
//        else if (estOpContenedor == estOpCont_listo)   //////  OJOOOOO  // debia ser  (estOpContenedor == estOpCont_listo) esto esta pra pruebas hasta que funcione la generacion del fractal
        if (estOpContenedor == estOpCont_listo)   //////  OJOOOOO  // debia ser  (estOpContenedor == estOpCont_listo) esto esta pra pruebas hasta que funcione la generacion del fractal
        {
            if (transform.parent.parent.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi == ScriptCtrlBaseDeEvi.estadoEvi_baseLista)
            {
                transform.parent.parent.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = ScriptCtrlBaseDeEvi.estadoEvi_operativo;
            }
        }  // FIn de - else if (estOpContenedor == estOpCont_enCarga)

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
        { Debug.Log("Entro en SctCtrlEviTipo_Fractal_01 => atiendeMariLlanos"); }
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


    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    //         ITERADORES


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// obtenKdlDeGetDetails : Metodo (corrutina) : envia un KDL de consulta GetDetails al DKS y cuando llega modifica el estado de este EVI para que este prosiga 
    ///                         visualizando la informacion en el evi conforme corresponde
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-20
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     (en esta clase ya tiene lo que necesita : key, host, cualificador, ordinalConf, ultiModConf 
    ///     Recordamos
    ///         => E (enlace)
    ///             => R (referencia)
    ///                 => I (identificador)
    ///                     => F (control de configuracion)
    ///                     => P (Ayuda a interfaz) (opcional)
    ///                     
    /// Variables de salida :
    ///     No devuelve nada, cuando finaliza pone el evi es estado "estOpCont_recibidoKDL" si todo ha ido bien o en estado "estOpCont_errorEnCargaKDL" si ha habido error.
    /// Observaciones:
    ///         Para llevarla a cabo se realizan los pasos siguientes:
    ///             1.) Se genera el KDL de consulta GetDetails para enviar al DKS
    ///             2.) Se realiza la solicitud getDetails al DKS
    ///             3.) Cuando ha llegado el KDL, se guarda en la variable correspondiente y se cambia el estado para que el evi pueda continual 
    ///                 con el proceso de generar el evi fractal de referencia
    /// </summary>

    IEnumerator obtenKdlDeGetDetails()
    {
        // ///////////////////////////////
        // 1.) Se genera el KDL de consulta GetDetails para enviar al DKS
        // Esto es una consulta KDL a un DKS. Por lo que hay que enviar el KDL de solicitud, mediante el POST
        // Preparamos el KDL de consulta getDetails, que seran los datos que iran en le POST

        string kdlConsulta = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameConceptoBuskedaKeyHost(key, host);

        // ///////////////////////////////
        // 2.) Se realiza la solicitud getDetails al DKS
        // Realizamos la solicitud al DKS
        string urlDks = host + ScriptConexionDKS.sufijoAccesoASwDks; // Para solicitud sin post, devuelve lo que le mandamos por el post

        WWWForm formDatosPost = new WWWForm();
        formDatosPost.AddField("KDL", kdlConsulta);

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde SctCtrlEviTipo_Fractal_01 => obtenKdlDeGetDetails. Con  = " + urlDks + " -  y con  = " + kdlConsulta); }

        // Preparamos la conexion por POST con el DKS
        UnityWebRequest ObjWebReqPost = UnityWebRequest.Post(urlDks, formDatosPost);

        estOpContenedor = estOpCont_solicitadoKDL;
        yield return ObjWebReqPost.SendWebRequest(); // Quedamos a la espera de la resuesta

        // ///////////////////////////////
        //  3.) Cuando ha llegado el KDL, se guarda en la variable correspondiente y se cambia el estado para que el evi pueda continual 
        //      con el proceso de generar el evi fractal de referencia

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Dede SctCtrlEviTipo_Fractal_01 =>obtenKdlDeGetDetails. Salimos del if en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

        if (ObjWebReqPost.isNetworkError)
        {
            // Si se produce un error, debe enviarse un KDL de error. PENDIENTE (MAFG 2021-02-04) Por ahora solo anotamos en el log
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(" Desde SctCtrlEviTipo_Fractal_01 => obtenKdlDeGetDetails con error = " + ObjWebReqPost.error); }
            estOpContenedor = estOpCont_errorEnCargaKDL;
        }
        else
        {
            // Tomamos la respuesta que sera un fichero KDL, osea un XML en texto
            string respuestaKDL = ObjWebReqPost.downloadHandler.text;
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log(" Desde SctCtrlEviTipo_Fractal_01 => obtenKdlDeGetDetailscon respuestaKDL  = " + respuestaKDL); }
            // Construimos el DOM del XML recibido
            XmlDocument domDeKdl = new XmlDocument();
            domDeKdl.PreserveWhitespace = true;
            try
            {
                domDeKdl.LoadXml(respuestaKDL);
                // Aqui habria que validarlo con el esquema de KDL. PENDIENTE (MAFG 2021-01-31)
                KdlConcepto = domDeKdl;
                EviBase.gameObject.transform.GetComponent<ScriptCtrlBaseDeEvi>().domPropio = domDeKdl;
                // SI estamos trayendo el KDL del concepto para u evi fractal, hay que poner el nodo" nodo_E_eviFractal" el nodo "C" del KDL que hemos obtenido
//                string subTipoElementIntfDeEviBase = EviBase.gameObject.transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
//                if (subTipoElementIntfDeEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                string subTipoElementIntfDeEviContenedor = gameObject.transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
                if (subTipoElementIntfDeEviContenedor == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalReferencia) 
                {
                    nodo_E_eviFractal = domDeKdl.DocumentElement;
                }
                estOpContenedor = estOpCont_recibidoKDL;
            } // FIn de - try
            catch (System.IO.FileNotFoundException)
            {
                if (DatosGlobal.niveDebug > 100)
                { Debug.Log("Error al generar el DOM desde - con respuestaConPost = " + respuestaKDL); }
                estOpContenedor = estOpCont_errorEnCargaKDL;
            }  // Fin de -  catch (System.IO.FileNotFoundException)
        }  // Fin de - else de -if (ObjWebReqPost.isNetworkError)

    }  // Fin de - IEnumerator obtenKdlDeGetDetails()


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviCompleto : Metodo (corrutina) : Hemos generado el contenido (fractal) que va a tener el contenedor del evi. Pero en este cuadro no 
    ///                         tenemos la base del evi para poder cumplimentarla debidamente, por lo que hacemos esta corrutina, para esperar
    ///                         al cuadro siguiente, donde ya tengamos la base de evi disponible para poder ir rellenandola pertinentemente
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-09
    /// Ultima modificacion :
    /// Variables de entrada : 
    /// Variables de salida :
    /// Observaciones:
    /// </summary>

    IEnumerator generaEviCompleto()
    {
        yield return null; // Esperamos un cuadro a que el evi base este listo
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);

        yield return null; // Esperamos un cuadro a que el evi base este listo
        yield return null; // Esperamos un cuadro a que el evi base este listo
        // Cargamos el DOM en el "domPropio" del EVI  generado. Ahi tendra toda la informacion necesaria para generarse
//        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio = KdlConcepto;
        // Marcamos el estado adecuado del evi fractal
//        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = "generandoContenido";
//        EviBase.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi();

        yield return null; // Esperamos un cuadro a para no mezcalr procesos que da la lata
//        desarrollaFractal();

    }  // Fin de - IEnumerator generaEviCompleto()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviCompleto : Metodo (corrutina) : Hemos generado el contenido (fractal) que va a tener el contenedor del evi. Pero en este cuadro no 
    ///                         tenemos la base del evi para poder cumplimentarla debidamente, por lo que hacemos esta corrutina, para esperar
    ///                         al cuadro siguiente, donde ya tengamos la base de evi disponible para poder ir rellenandola pertinentemente
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-09
    /// Ultima modificacion :
    /// Variables de entrada : 
    /// Variables de salida :
    /// Observaciones:
    /// </summary>

    IEnumerator esperaYactivacarga()
    {
        yield return null; // Esperamos un cuadro a que el evi base este listo
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
        // Estamos cargando un evi de concepto. Luego cargamos un fractal
        //        GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_RefFractal); // Debe ser un evi fractal PENDIENTE

        yield return null; // Esperamos un cuadro a que el evi base este listo
        yield return null; // Esperamos un cuadro a que el evi base este listo
                           // Cargamos el DOM en el "domPropio" del EVI  generado. Ahi tendra toda la informacion necesaria para generarse
                           //        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio = KdlConcepto;
                           // Marcamos el estado adecuado del evi fractal
                           //        EviBase.GetComponent<ScriptCtrlBaseDeEvi>().estado_evi = "generandoContenido";
                           //        EviBase.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi();

        yield return null; // Esperamos un cuadro a para no mezcalr procesos que da la lata
                           //        desarrollaFractal();
       estOpContenedor = estOpCont_enCarga; // Cambiamos el estado para no volver a cargar los datos de evi base

    }  // Fin de - IEnumerator generaEviCompleto()

}  // Fin de - public class SctCtrlEviTipo_Fractal_01 : MonoBehaviour {
