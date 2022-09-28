using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;
using TMPro;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control para un EVI fractum de referencia, instancia o sin techo (una de las partes de un evi fractal de referencia)
/// Construye y gestiona un evi fractum de referencia de un evi fractal de referencia
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-04-10
/// Observaciones :
/// 		- Este se genera para ser una de las partes de un evi fractal de referencia, instancia o sin techo (cada enlace de referencia, instancia o sin techo de un evi fractal
/// 		tiene un evi fractum que lo desarrolla y visualiza)
///     		- Los fractum de referencia solo tienen una referencia al concepto
/// 	    	- Los evi de instancia tienen una referencia al concepto que instancian y la descripcion de la instanciacion. Esta descripcion sera a su vez un 
/// 	    	conjunto de fractum contenidos en el fractun de la instancia 
/// 	    	- Los evi sin techo, tienen una referencia al tipo de sintecho y el contenido del sin techo
///			- Todos los evis que puedan recibir o enviar solicitudes, deben imlementar las funciones "atiendeMariLlanos" y "procesaRespuestaSolicitud".
///			
/// </summary>
public class SctCtrlEviTipo_01Fractum : MonoBehaviour
{
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

    // Fin de - 1 - Seccion comun a todos los EVIs, para declaracion de variables
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////

    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////
    // A partir de aqui declaracion de  variables especificas de este tipo de evi


    // /////////////////////////////////////////////////////////
    // Datos de identificacion del ENLACE que visualiza
    public string tipoEnlace; // Referencia, Instancia o SinTecho

    // Key y host los hacemos publicos para poder cambiarlos desde el inspector, pero deben ser privados y modificables desde la interfaz de usuario del objeto buscador
    public string key; // Key (KDL:I => K ) del ENLACE que visualiza
    public string host; // host (KDL:I => H ) del ENLACE que visualiza
    public string cualificador; // Cualificador (KDL:I => Q ) del ENLACE que visualiza, conceptos, efimeros, etc..

    public string ordinal; // Ordianl (KDL:F=> O) del ENLACE que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
    public string fechUltMod; // Ultima modificacion (KDL:F=> M) del ENLACE que visualiza. Es un instante concreto del tiempo global

    public string idioma_AyuIntf;               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
    public string imagen_AyuIntf;               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL)
    public string icono_AyuIntf;               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    public string audio_AyuIntf;               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_nombre_AyuIntf;          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_rotulo_AyuIntf;          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_descripcion_AyuIntf;     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)

    public string datoSinTecho;                 // Para el caso de fractums con tipo de enlace Sin techo (Z) es el dato del sin techo, contenido en (T)
        public static string datoSinTechoSInDato = "Sin dato SIN TECHO";                 // Para el caso de fractums con tipo de enlace Sin techo (Z) es el dato del sin techo, contenido en (T)

    public XmlDocument KdlConcepto; // este debe ser un DOM, pero para ir desarrollando pongo el string xml PENDIENTE (2021-01-30 Miguel AFG)

    public XmlNode nodo_E_Fractum;  // es elnodo enlace "E", que el fractum debe monitorizar en pantalla

    // ////////////////////////////
    // //// Vamos con los componentes de la ayuda a interfaz

    // Para la caja de texto que contiene la ayuda a interfaz : nombre, descripcion,...
    // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  "Script_BaseDeEvi_N1_Info"

    // Para la caja de imagen que contiene la imagen de ayuda a interfaz (La imagen pequeña que acompaña añ icono NO la grande del contenedor)
    public GameObject ImgAyuIntfBaseDeEvi;
    public Vector3 escalaImgAyuIntfBaseDeEvi;
    public Vector3 posicionImgAyuIntfBaseDeEvi;
    protected float factorEscalaImgAyuIntfBaseDeEvi;

    // Para las texturas Imagenes que contiene asociadas a la ayuda a interfaz
    public Material material_imagen_AyuIntf;
    public Texture textura_imagen_AyuIntf;

    public Material material_ImgAyuIntfBaseDeEvi;
    public Texture textura_ImgAyuIntfBaseDeEvi;

    public Material material_icono_AyuIntf;
    public Texture textura_icono_AyuIntf;

    public AudioClip AudioClip_AyuIntf;  // Para el audio del concepto


    public string estadoImgAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
    public string estadoIconoAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
    public string estadoAudioAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
                                // Sin inidiar
                                // enCarga
                                // Cargada

 //   protected string enResources_ImgAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz
 //   protected string enResources_IconoAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz
 //   protected string enResources_AudioAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz

    // ////////////////////////////
    // //// Vamos con los componentes del fractum
    public GameObject Opciones_FractumRef;
        public GameObject Opciones_FractumRef_Info;
        public GameObject Opciones_FractumRef_Individualizar;

    // Vamos con el canvas y su texto
    // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  " el que corresponda"

    public GameObject Contenedor_FractumRef;

    public Material material_fondoFractum; // COlor de fondo del fractum (Referencia = blanco; Instancia = azul; SinTecho = rojo)
//    public Material material_fondoReferencia;
//    public Material material_fondoInstancia;
//    public Material material_fondoSinTecho;


    // Estados operativos dee EVI contenedor
    public string estOpFractum; // indica los estados de carga y operativos
                                   // Sus valores posibles son :
        public static string estOpFractum_sinIniciar = "sinIniciar";  // El evi ha sido generado, pero estamos aun en el AWAKE. No hemos hecho nada mas
        public static string estOpFractum_nodeoEnlaceDisponible = "nodeoEnlaceDisponible";  // El evi ha sido generado, pero estamos aun en el AWAKE. No hemos hecho nada mas
        public static string estOpFractum_enCarga = "enCarga";  // El evi ha sido generado, pero estamos aun en el AWAKE. No hemos hecho nada mas
        public static string estOpFractum_generandoDescFractal = "generandoDescFractal";  // El evi esta generando su estructura fractal interna. Osea generando los gameobject internos con la descripcion del concepto
        public static string estOpFractum_listo = "listo";  // EL evi esta listo, con la interfaz cargada y si procede, con el desarrollo fractal generado
        public static string estOpFractum_operativo = "operativo";  // Estara operativo cuando lo este todo el evi que lo contiene, incluidos progenitores anteriores hasta el evi base

    // ////////////////////////////////////////////
    // Miscelaneos
    public bool contenidoCumplimentado;
    public bool maximizado;


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos todos los parametros del sistema
    /// Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-02-19
    /// Observaciones :
    /// 		Todos los fractum son generados por el contenedor padre, que ya tiene el KDL del concepto disponible y es este quien ajusta 
    /// 		los parametros de escala y localizacion de los fractum, ya que es el que construye y coloca el puzle de fractums atendiendo a su numero 
    /// 		y el espacio disponible en el contenedor
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

        // Fin de - 2 - Seccion comun a todos los EVIs, asignacion de variables
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////


        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Awake()"

        // /////////////////////////////////////////////////////////
        // Colocamos los elementos del fractus. Estos son 
        //      Hijo(0) = "Opciones_FractumRef" 
        //          Hijo(0) =>  = Hijo(0) "Opciones_FractumRef_Info"
        //          Hijo(0) =>  = Hijo(1) "Opciones_FractumRef_Individualizar"
        //      Hijo(1) ="Contenedor_FractumRef"

        //  Hijo(0) = "Opciones_FractumRef"
        Opciones_FractumRef = transform.GetChild(0).gameObject; // Es el primer hijo de este gameobjet en el que estamos "EviTipo_RF01_FractumRef"
        Opciones_FractumRef_Info = Opciones_FractumRef.gameObject.transform.GetChild(0).gameObject; // Es el primer hijo de "Opciones_FractumRef"
        Opciones_FractumRef_Individualizar = Opciones_FractumRef.gameObject.transform.GetChild(1).gameObject; // Es el segundo hijo de "Opciones_FractumRef"

            // Hijo(1) ="Contenedor_FractumRef"
        Contenedor_FractumRef = gameObject.transform.GetChild(1).gameObject; // Es el tercer hijo de este gameobjet en el que estamos "EviTipo_RF01_FractumRef"

        // Vamos con el menu de opciones 
        Vector3 escala_Opciones_FractumRef = new Vector3(0.1f, 1f, 1.1f); // X un decimo, y igual y z un pelin mas grueso para que se superponga y lo veamos
        Opciones_FractumRef.transform.localScale = escala_Opciones_FractumRef;
        Vector3 localiz_Opciones_FractumRef = new Vector3(-0.5f, 0f, 0f); // X a la izquierda, y centrado y z centrado
        Opciones_FractumRef.transform.localPosition = localiz_Opciones_FractumRef;
        // Vamos con el boton de Icono de concepto
        Vector3 escala_Opciones_FractumRef_Info = new Vector3(0.9f, 0.1f, 1.1f); // X dejando un margen, y para hacerlo cuadrado (la x se redujo a 0.1 en el padre) y z un pelin mas grueso para que se superponga y lo veamos
        Opciones_FractumRef_Info.transform.localScale = escala_Opciones_FractumRef_Info;
        Vector3 localiz_Opciones_FractumRef_Info = new Vector3(0f, 0.4f, 0f); // X centrado, y arriba y z centrado
        Opciones_FractumRef_Info.transform.localPosition = localiz_Opciones_FractumRef_Info;
        // Vamos con el boton de Sacar (que genera un evi de referencia fractal del concepto
        Vector3 escala_Opciones_FractumRef_Individualizar = new Vector3(0.9f, 0.1f, 1.1f);  // X dejando un margen, y para hacerlo cuadrado (la x se redujo a 0.1 en el padre) y z un pelin mas grueso para que se superponga y lo veamos
        Opciones_FractumRef_Individualizar.transform.localScale = escala_Opciones_FractumRef_Individualizar;
        Vector3 localiz_Opciones_FractumRef_Individualizar = new Vector3(0f, 0.3f, 0f);  // X centrado, y arriba despues del boton de icono y z centrado
        Opciones_FractumRef_Individualizar.transform.localPosition = localiz_Opciones_FractumRef_Individualizar;

        // Vamos con el canvas y su texto
        // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  " el que corresponda"
        //       

        // Vamos con el contenedor del FractumRef
        Vector3 escala_Contenedor_FractumRef = new Vector3(0.8f, 0.9f, 1.1f); // X un noveno (le falta el menu de opciones para cubrir al padre, y 0.9 para dejar un margen y z un pelin mas grueso para que se superponga y lo veamos
        Contenedor_FractumRef.transform.localScale = escala_Contenedor_FractumRef;
        Vector3 localiz_Contenedor_FractumRef = new Vector3(0.05f, 0f, 0f); // X a la izquierda justo despues de las opciones, y centrado y z centrado
        Contenedor_FractumRef.transform.localPosition = localiz_Contenedor_FractumRef;

        // Generamos los materiales por defecto
        material_imagen_AyuIntf = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo a la imagen de fondoobjeto
        material_ImgAyuIntfBaseDeEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo a la imagen pequeña que acompaña al icono del objeto
        material_icono_AyuIntf = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo al icono del objeto


        key = "Sin dato KEY"; // Key (KDL:I => K ) del ENLACE que visualiza
        host = "Sin dato HOST"; // host (KDL:I => H ) del ENLACE que visualiza
        cualificador = "Sin dato cualificador"; // Cualificador (KDL:I => Q ) del ENLACE que visualiza, conceptos, efimeros, etc..
        ordinal = "Sin dato ordinal"; // Ordianl (KDL:F=> O) del ENLACE que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        fechUltMod = "Sin dato fechUltMod"; // Ultima modificacion (KDL:F=> M) del ENLACE que visualiza. Es un instante concreto del tiempo global

        idioma_AyuIntf = "Sin dato idioma";               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
        imagen_AyuIntf = "Sin dato imagen";               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL)
        icono_AyuIntf = "Sin dato icon";               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
        audio_AyuIntf = "Sin dato audio";               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
        txt_nombre_AyuIntf = "Sin dato nombre";          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
        txt_rotulo_AyuIntf = "Sin dato rotulo";          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
        txt_descripcion_AyuIntf = "Sin dato descripcion";     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
        datoSinTecho = datoSinTechoSInDato; 

        // /////////////////////////////////////////////////////////
        // Definimos las variables de estado
        estOpFractum = estOpFractum_sinIniciar;

        // OJO, en este awake no se puede hacer referencia a nada del gameobject "EviTipo_Vacio_00", ya que este no se genera hasta 
        // que se ejecuta es start


    } // FIn de - void Awake()


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////  Error al obtener imagen_AyuIntf
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
        { Debug.Log("Dede SctCtrlEviTipo_01Fractum =>Start. He llegado en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }


        // Fin de - 3 - Seccion comun a todos los EVIs, para generar su EVI base y ahijarse en su contenedor
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////

        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // /////////////////////////////////////////////////////////
        // A partir de aqui la asignacion de variables y otro código especificos de este tipo de evi en el "void Start()"

        // Ponemos el material de fondo del fractum, segun su tipo. Hay que cargarlos en el start ya que en el aweak todabia el tipo de enlace no esta cargado

        if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia))
        {
            material_fondoFractum = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().material_fondoInstancia);
        }
        else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Referencia))
        {
            material_fondoFractum = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().material_fondoReferencia);
        }
        else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_SinTecho))
        {
            material_fondoFractum = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().material_fondoSinTecho);
        }
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Error desde SctCtrlEviTipo_01Fractum => Awake. Tipo de enlace desconocido. Tipo enlace = " + tipoEnlace); }
            material_fondoFractum = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().material_fondoError);
        }
        this.gameObject.GetComponent<MeshRenderer>().material = material_fondoFractum;


        //        StartCoroutine(generaEviCompleto());  // Hay que construir el evi mediante una corrutina porque este cuadro, el evi base no esta todabia disponibel

    } // FIn de - void Start ()

    // Update is called once per frame
    void Update()
    {

        //    Debug.Log(" Desde  SctCtrlEviTipo_01Fractum = > Update antes de kdl disponible - En cuadro n =" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame);
        if (estOpFractum != estOpFractum_listo) // SI el fractum todabia no esta terminado de generar
        {
            if (estOpFractum == estOpFractum_nodeoEnlaceDisponible) // Tenemos disponible el nodo enlace, cargamos sus datos segun corresponda
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" Desde  SctCtrlEviTipo_01Fractum = > Update con KdlConcepto disponible = " + KdlConcepto.ToString() + " - En fr¡cuadro n =" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }
                // /////////////////////////////////////////////
                // Primero cargamos los datos de ayuda a interfaz
                cargaDatosAIFractum();

            }  // Fin de - if (estOpFractum == estOpFractum_nodeoEnlaceDisponible)
        }  //  Fin de - if ((estOpFractum != estOpFractum_listo) & (estOpFractum != estOpFractum_listo))
    }  // FIn de - void Update()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cargaDatosAIFractum
    /// Observaciones : Este metodo carga los datos  de ayuda a interfaz 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-18
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject solicitudAsociada : es la solicitud que genera la carga de datos. COntiene la informacion necesaria para cargar los datos del EVI y gestionar la solicitud
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         Como es un fractum, los datos nos vienen en el enlace, por lo que no hay que buscarlos fuera (ya se los trajo alguien (el evi contenedor) antes)
    ///         Para cargar los datos actuamos como sigue:
    ///             1.) Si es un enlace de instancia o de referencia (tipoEnlace es (A) o (R)), tomamos los datos de I, F y Q
    ///                Recordamos
    ///                      => E (enlace)
    ///                           => R (referencia) o A (instancia)
    ///                                => I (identificador)
    ///                                => F (control de configuracion)
    ///                                => P (Ayuda a interfaz) (opcional)
    ///             2.) SI es un enlace SinTecho (Z), tendremos una referencia (R) que indica el tipo de dato, y un elemento dato (T) que contiene el dato sin techo en si
    ///                Recordamos
    ///                      => E (enlace)
    ///                           => Z (referencia) o A (instancia)
    ///                              => R (referencia) o A (instancia)
    ///                                    => I (identificador)
    ///                                    => F (control de configuracion)
    ///                                    => P (Ayuda a interfaz) (opcional)
    ///                              => T (SIn Techo) Contiene un string de texto con el dato sin techo
    ///                              
    ///                     2.1.) En este caso, tomamos los datos de interfaz del tipo de dato que esta en la referencia (R) y los ponemos en la ayuda a interfaz del fractum sin techo
    ///                         (OJO por ahora 2021-04-27, el DKS nos nos envia la ayuda a interfaz de la referencia del tipo de dato (P) de (R), por lo que le ponemos uno por defecto
    ///                         PENDIENTE MAFG 2021-04-27 hay que hacer que el DKS mande la ayuda a interfaz (P) de la referencia del tipo de dato (R))
    ///                     2.2.) Despues colocamos el texto del sin techo en el texto de sin techo del canvas general. El cual se activa al entrar en el contenedor del fractum sin techo
    ///                         y se desactiva al saleir de el
    ///             
    /// </summary>
    public void cargaDatosAIFractum()
    {

        estOpFractum = estOpFractum_enCarga;

        // Los datos que buscamos estaran ya en el enlace que esta almacenado en la variable "XmlNode nodo_E_Fractum"

        // XmlNode KDL_Raiz = domPropio.DocumentElement;
        string[] identificadorConcepto_K_H_Q;
        string[] ctrlCof_O_M;
        string[] ayudaIntf;


        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        // Vamos a cargar los datos de ayuda a interfaz y otros del enlace E
        // En "enlaceKdlDelFractum", lo que tenemos es el nodo del enlace (E). Primero obtenemos el nodo de la Inarancia (A), referencia (R) o sin techo(Z),
        // que sera el primer hijo del nodo enlace E

        XmlNode nodo_R_A_Z_DelFractum = nodo_E_Fractum.FirstChild;

        // Obtenemos lo datos basicos de ayuda a interfaz del enlace E

        // 1.) Si es un enlace de instancia o de referencia (tipoEnlace es (A) o (R)), tomamos los datos de I, F y Q
        if ((tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia)) ||
            (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Referencia)))
        {
            // Vamos con el identificador
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("cargaDatosAIFractum con KdlConcepto = " + KdlConcepto.ToString()); }

            identificadorConcepto_K_H_Q = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(KdlConcepto, nodo_R_A_Z_DelFractum);
            // Asignamos a las variables de este game object
            key = identificadorConcepto_K_H_Q[0];                            // key: es el key del concepto (K en KDL - esta en I en KDL)
            host = identificadorConcepto_K_H_Q[1];                         // host : es el host del concepto (H en KDL - esta en I en KDL)
            cualificador = identificadorConcepto_K_H_Q[2];                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)

            // Vamos con los datos de control de configuracion
            ctrlCof_O_M = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameCtrlConfDeNodo(KdlConcepto, nodo_R_A_Z_DelFractum);
            ordinal = ctrlCof_O_M[0];                         // ordinal : es el ordinal del concepto (O en KDL - esta en F en KDL)
            fechUltMod = ctrlCof_O_M[1];                         // fechUltMod : es la fecha de ultima modificacion del concepto (M en KDL - esta en F en KDL)

            // Vamos con los datos de ayuda a interfaz
            ayudaIntf = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameAyudaIntDeNodo(KdlConcepto, nodo_R_A_Z_DelFractum);
            idioma_AyuIntf = ayudaIntf[0];               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
            txt_nombre_AyuIntf = ayudaIntf[1];          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
            txt_descripcion_AyuIntf = ayudaIntf[2];     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
            txt_rotulo_AyuIntf = ayudaIntf[3];          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
            icono_AyuIntf = ayudaIntf[4];               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                        // TENGO QUE PEDIRLO FUERA
            audio_AyuIntf = ayudaIntf[5];               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                        // TENGO QUE PEDIRLO FUERA
            imagen_AyuIntf = ayudaIntf[6];               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                         // TENGO QUE PEDIRLO FUERA
        }
        else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_SinTecho))
        {
            // 2.1.) En este caso, tomamos los datos de interfaz del tipo de dato que esta en la referencia (R) y los ponemos en la ayuda a interfaz del fractum sin techo
            // La referencia (R) del tipo de dato es el primer hijo del nodo sin techo ()Z 
            XmlNode nodo_R_DeTipoDeDato = nodo_R_A_Z_DelFractum.FirstChild;
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("desde 439 con nodo_R_A_Z_DelFractum.Name = " + nodo_R_A_Z_DelFractum.Name +
                "con nodo_R_DeTipoDeDato.Name = " + nodo_R_DeTipoDeDato.Name); }

            // Vamos con el identificador
            identificadorConcepto_K_H_Q = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(KdlConcepto, nodo_R_DeTipoDeDato);
            // Asignamos a las variables de este game object
            key = identificadorConcepto_K_H_Q[0];                            // key: es el key del concepto (K en KDL - esta en I en KDL)
            host = identificadorConcepto_K_H_Q[1];                         // host : es el host del concepto (H en KDL - esta en I en KDL)
            cualificador = identificadorConcepto_K_H_Q[2];                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)

            // Vamos con los datos de control de configuracion
            ctrlCof_O_M = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameCtrlConfDeNodo(KdlConcepto, nodo_R_DeTipoDeDato);
            ordinal = ctrlCof_O_M[0];                         // ordinal : es el ordinal del concepto (O en KDL - esta en F en KDL)
            fechUltMod = ctrlCof_O_M[1];                         // fechUltMod : es la fecha de ultima modificacion del concepto (M en KDL - esta en F en KDL)


            //      (OJO por ahora 2021-04-27, el DKS nos nos envia la ayuda a interfaz de la referencia del tipo de dato (P) de (R), por lo que le ponemos uno por defecto
            //      PENDIENTE MAFG 2021-04-27 hay que hacer que el DKS mande la ayuda a interfaz (P) de la referencia del tipo de dato (R))

            // Vamos con los datos de ayuda a interfaz
            //            ayudaIntf = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameAyudaIntDeNodo(KdlConcepto, nodo_R_DeTipoDeDato);
            //            idioma_AyuIntf = ayudaIntf[0];               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
            //            txt_nombre_AyuIntf = ayudaIntf[1];          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
            //            txt_descripcion_AyuIntf = ayudaIntf[2];     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
            //            txt_rotulo_AyuIntf = ayudaIntf[3];          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
            //            icono_AyuIntf = ayudaIntf[4];               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
            // TENGO QUE PEDIRLO FUERA
            //            audio_AyuIntf = ayudaIntf[5];               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
            // TENGO QUE PEDIRLO FUERA
            //            imagen_AyuIntf = ayudaIntf[6];               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
            // TENGO QUE PEDIRLO FUERA
            // Ponemos esto por defecto, hasta que el DKS nos mande el elemento P del tipo de dato que esta en R


            idioma_AyuIntf = "Idioma de Tipo de dato sin techo por defecto";  // por defecto hasta que el DKS nos de el P del R
            txt_nombre_AyuIntf = "Tipo de dato sin techo por defecto";  // por defecto hasta que el DKS nos de el P del R
            txt_descripcion_AyuIntf = "Descripcion del tipo de dato sin techo por defecto  con key : " + key + " -y host : " + host;  // por defecto hasta que el DKS nos de el P del R
            txt_rotulo_AyuIntf = "Rotulo de dato sin techo por defecto";  //  por defecto hasta que el DKS nos de el P del R
            icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/iconoImgPorDefecto";  //   por defecto hasta que el DKS nos de el P del R
            audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudioPorDefecto";  // por defecto hasta que el DKS nos de el P del R
            imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/sinFoto";  // por defecto hasta que el DKS nos de el P del R

            // 2.2.) Despues colocamos el texto del sin techo en el texto de sin techo del canvas general. El cual se activa al entrar en el contenedor del fractum sin techo
            //      y se desactiva al saleir de el

            // Obtenemos el dato SIN TECHO  No es un elemento adecuado T
            datoSinTecho = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameDatoSinTecho(KdlConcepto, nodo_R_A_Z_DelFractum);
        }

        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Error desde SctCtrlEviTipo_01Fractum => cargaDatosAIFractum. Tipo de enlace desconocido. Tipo enlace = " + tipoEnlace); }

        }

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Hola desde cargaDatosBaseDeEvi Desde domCargado - 1 - txt_nombre_AyuIntf = " + txt_nombre_AyuIntf +
            " - 2 - txt_descripcion_AyuIntf = " + txt_descripcion_AyuIntf +
            " - 3 - txt_rotulo_AyuIntf = " + txt_rotulo_AyuIntf +
            " - 4 - icono_AyuIntf = " + icono_AyuIntf +
            " - 5 - audio_AyuIntf = " + audio_AyuIntf +
            " - 6 - imagen_AyuIntf = " + imagen_AyuIntf +
            " - desde el nodo enlace = " + nodo_R_A_Z_DelFractum.Name);
        }

        // Marcamos la solicitud porque ya no requiere atencion. Cuando finalicemos la generacion total del gameObject, la marcaremos como finalizada

        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        //  YA TENEMOS LOS DATOS O LAS REFERENCIAS DE LA INFORMACION DE AYUDA A INTEFAZ
        // Ponemos los datos de ayuda a interfaz en el gameObjet de la base del evi que contiene el concepto

        // ///////////////////////////////////////////////////////////////////////////////////
        //  Primero los que ya tenemos y no hay que ir a buscar fuera

        // Vamos con el texto de ayuda a interfaz
        // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  "el que corresponda"

        // ///////////////////////////////////////////////////////////////////////////////////
        //  Ahora los que tenemos el sitio donde buscarlos, pero que hay que bajar de internet
        //  Para obtenerlos de internet y cargarlos utilizaremos corrutinas definidas en esta clase (AHORA No usamos solicitudes )

        // Vamos a por la imagen de descripcion del concepto (la grande que aparece de fondo en el contenedor del concepto)
        string locImgAyuIntf = host + ScriptConexionDKS.sufijoAccesoAImagenesDks + "/" + imagen_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeTextura_imagen_AyuIntf(locImgAyuIntf));
        // En el update cargamos la textura donde corresponde cuando la corrutina ha terminado. Se chequea mediante "estadoImgAyudaIntf"

        // Vamos a por el icono del concepto
        string locIconoAyuIntf = host + ScriptConexionDKS.sufijoAccesoAIconosDks + "/" + icono_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeTextura_icono_AyuIntf(locIconoAyuIntf));
        // En el update cargamos la textura donde corresponde cuando la corrutina ha terminado. Se chequea mediante "estadoIconoAyudaIntf"


        // Vamos a por el audio del concepto
        string locAudioAyuIntf = host + ScriptConexionDKS.sufijoAccesoAAudiossDks + "/" + audio_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeAudio__AyuIntf(locAudioAyuIntf));

        // *************************************
        // *************************************
        // Cargamos ahora el fracal del concepto con la descripcion y los datos del DOM

        // El evi ya esta cargando sus datos básicos, por lo que lo indicamos en el estado
//        estOpFractum = estOpFractum_enCarga;

    }  // Fin de -  public void cargaDatosBaseDeEvi(GameObject solicitudAsociada)


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////  
    /// ///////////  reemplaza_aspecto_AyuIntf_fractum() : esta funcion cambia las imagenes iconos y audios de la ayuda a interfaz de este fractum
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string este_locImgAyuIntf
    ///         - string este_locIconoAyuIntf
    ///         - string este_locAudioAyuIntf
    /// Variables de salida :
    ///     No devuelve nada, solo reemplaza el aspecto de la ayuda a interfaz
    /// Observaciones:
    ///     - OJOOO, los datos de los sistentos scripts asociados al EVI deben cambiarse por otro lado y anteriormente a la llamada de este metodo.
    ///     aqui solo se cambian las texturas, audios, etc para que sean visualizadas en el evi, no los valores de los parametros que contienen sta informacion
    /// </summary>
    public void reemplaza_aspecto_AyuIntf_fractum(string este_locImgAyuIntf, string este_locIconoAyuIntf, string este_locAudioAyuIntf)
    {
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Estoy en SctCtrlEviTipo_01Fractum => reemplaza_aspecto_AyuIntf_fractum : " +
              "\n - Con este_locImgAyuIntf : " + este_locImgAyuIntf +
              "\n - Con este_locIconoAyuIntf : " + este_locIconoAyuIntf +
              "\n - Con este_locAudioAyuIntf : " + este_locAudioAyuIntf +
              "\n - Con name : " + this.name
              );
        }

        StartCoroutine(traeTextura_imagen_AyuIntf(este_locImgAyuIntf));
        StartCoroutine(traeTextura_icono_AyuIntf(este_locIconoAyuIntf));
        StartCoroutine(traeAudio__AyuIntf(este_locAudioAyuIntf));

    }  // FIn de - public void reemplaza_aspecto_AyuIntf_fractum(string este_locImgAyuIntf, string este_locIconoAyuIntf, string este_locAudioAyuIntf)

    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////


    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   CORRUTINAS
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita una imagen de ayuda a interfaz a un servidor externo Y la pone como textura en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : "http://www.ideando.net/klw/dks_desarrollo")
    /// Variables de salida :
    /// Observaciones:
    ///         - OJO  pone como textura de DOS GAMEOBJECT en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    ///         - OJOOO estamos en un fractum, no en el evi base. Para gestionar la ayuda a interfaz del evi base HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "ScriptCtrlBaseDeEvi"
    /// </summary>
    IEnumerator traeTextura_imagen_AyuIntf(string origenDeLaImagen)
    {
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Estoy en SctCtrlEviTipo_01Fractum => traeTextura_imagen_AyuIntf : " +
              "\n - Con origenDeLaImagen : " + origenDeLaImagen +
              "\n - Con name : " + this.name
              );
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(origenDeLaImagen);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log("Error desde traeTextura_imagen_AyuIntf : " + www.error + " -Al intentar traer la textura : " + origenDeLaImagen); }
        }
        else
        {
            textura_imagen_AyuIntf = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // Ponemos la textura recibida y almacenada en "textura_imagen_AyuIntf" en el material correspondiente a la imagen de fondo
            material_imagen_AyuIntf.SetTexture("_MainTex", textura_imagen_AyuIntf);  // Le asinamos la textura al material
                                                                                     // Ponemos el material en el objeto contenedor, Que es quien contiene la imagen de fondo
            this.gameObject.GetComponent<SctCtrlEviTipo_01Fractum>().Contenedor_FractumRef.GetComponent<MeshRenderer>().material = material_imagen_AyuIntf;

            // Vamos con la imagen que acompaña al icono (Es la misma fuente que la anterior, pero se visualiza mas pequeña cuando marcamos el boton de información)
            textura_ImgAyuIntfBaseDeEvi = textura_imagen_AyuIntf; // Utilizamos la misma imagen para el fondo y para la que acompaña al icono
            material_ImgAyuIntfBaseDeEvi.SetTexture("_MainTex", textura_ImgAyuIntfBaseDeEvi);  // Le asinamos la textura al material
            Contenedor_FractumRef.GetComponent<MeshRenderer>().material = material_ImgAyuIntfBaseDeEvi; // Asignamos el material al objeto

            // Modificamos el estado para actuar en consecuencia
            estadoImgAyudaIntf = "Cargada";
        }
    }  // Fin de - IEnumerator traeTextura_imagen_AyuIntf(string origenDeLaImagen)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita una imagen de icono de concepto a un servidor externo
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : "http://www.ideando.net/klw/dks_desarrollo")
    /// Variables de salida :
    /// Observaciones:
    ///         - OJOOO estamos en un fractum, no en el evi base. Para gestionar la ayuda a interfaz del evi base HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "ScriptCtrlBaseDeEvi"
    /// </summary>
    IEnumerator traeTextura_icono_AyuIntf(string origenDeLaImagen)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(origenDeLaImagen);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log("Error desde  traeTextura_icono_AyuIntf : " + www.error + " -Al intentar traer la textura : " + origenDeLaImagen); }
        }
        else
        {
            textura_icono_AyuIntf = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // Ponemos la textura recibida y almacenada en "textura_icono_AyuIntf" en el material correspondiente al icono del concepto
            material_icono_AyuIntf.SetTexture("_MainTex", textura_icono_AyuIntf);  // Le asinamos la textura al material
            Opciones_FractumRef_Info.GetComponent<MeshRenderer>().material = material_icono_AyuIntf; // Asignamos el material al objeto

            // Modificamos el estado para actuar en consecuencia
            estadoIconoAyudaIntf = "Cargada";
        }
    }  // Fin de - IEnumerator  traeTextura_icono_AyuIntf(string origenDeLaImagen)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita audio de ayuda a interfaz de un concepto a un servidor externo
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDelAudio  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : "http://www.ideando.net/klw/dks_desarrollo")
    /// Variables de salida :
    /// Observaciones:
    ///         - OJOOO estamos en un fractum, no en el evi base. Para gestionar la ayuda a interfaz del evi base HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "ScriptCtrlBaseDeEvi"
    /// </summary>
    IEnumerator traeAudio__AyuIntf(string origenDelAudio)
    {
        // OJOOOOO habria que mirar la extension del fichero de audio, para ver el formato. O mejor, que en el formato KDL, el icono de audio tubiera dos campos, uno
        // de nombre del fichero y otro de tipo de fichero. Por ahora entendemos solo ficheros de audio tipo WAV. PENDIENTE MAFG 2021-02-12
        AudioType formatoAudio = AudioType.WAV;

        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(origenDelAudio, formatoAudio);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log("Error desde  traeAudio__AyuIntf : " + www.error + " -Al intentar traer el audio : " + origenDelAudio + " -COn formato :" + formatoAudio); }
        }
        else
        {
            AudioClip_AyuIntf = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;

            //Aplicamos el audio clip al audiosource
            this.gameObject.GetComponent<SctCtrlEviTipo_01Fractum>().AudioClip_AyuIntf = AudioClip_AyuIntf;

            // Modificamos el estado para actuar en consecuencia
            estadoIconoAyudaIntf = "Cargada";
        }
    }  // Fin de - IEnumerator  traeTextura_icono_AyuIntf(string origenDeLaImagen)


}
