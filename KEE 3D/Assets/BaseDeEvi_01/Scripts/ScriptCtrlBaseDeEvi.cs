using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control GENERAL de los EVIs 
/// Construye y gestiona la parte general de todos los EVIs
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-03-30
/// Observaciones :
/// 		TODOS los evis tienen una serie de datos y metodos en comun.  Si esto fuera un proyecto de codigo orientado a 
///		objetos, esto seria una clase de la que heredarian el resto de los EVIs especificos. EL fractal, el de lista,
///		else delegate busqueda u otros especificos delegate menus u otros funciones o basadas en graficos de cuerpo 
///		humano, planos, esquemas, planos o cualquier otro
///		Peor como UNITY ha optado por un desarrollo basado en componentes, en lugar de la filisofia de objetos
///		pues esto es un componente, tipo String, que deben llevar todos los EVIs del tipo que sean para implementar
///		los datos y las funciones generales que son:
/// 
///      La funcionalidad de los EVIs, se implementa dentro del hijo "ContenedorDeEvi_01" del evi base. Todos los evis tienen una base de EVI
///      que es igual en todos con su menu y la ayuda a interfaz propia del EVI correspondiente de forma que todos los EVIS tienen la misma información 
///      base de ayuda a interfaz (cada cual la de su ayuda a interfaz) y la mismo funcionalidad. Despues, cada EVI especifico (fractal, buscador, lista, etc...)
///      implementa su funcionalidad especifica en el gameobjet "ContenedorDeEvi_01" hijo de evi base
///
///		DATOS GENERALES :
///			- public Vector3posicionEvi - Indica la posicion del evi (siempre referida a su localizacion dentro de su padre osea el plano de trabajo)
///			- public float pos_x - Indica la posicion del evi en el eje x (siempre referida a su localizacion dentro de su padre osea el plano de trabajo)
///			- public float pos_y - Indica la posicion del evi en el eje y (siempre referida a su localizacion dentro de su padre osea el plano de trabajo)
///			- public float pos_z - Indica la posicion del evi en el eje z (siempre referida a su localizacion dentro de su padre osea el plano de trabajo)
///			- public bool contenidoCumplimentado - INdica si el EVI ha cargado sus datos desde la fuente que lo suministra (su DKS)
///          - Y mas en las propiedades, que me he candsado de ponerlos aqui
///		METODOS GENERALES :
///			-
/// </summary>

public class ScriptCtrlBaseDeEvi : MonoBehaviour {

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    // ///////////////////////////////////////////////////////////////////
    //      VALORES DE SUBTIPO para este tipo (EVI) de elemento de interfaz que es : "tipoElemItf_solicitud" ( ver "ScriptDatosInterfaz")
    // osea posibles valores de "subTipoElementIntf" para elementos de interfaz del tipo "tipoElemItf_solicitud"
//    public static string subTipoEvi_Base = "Base"; //  Base : Es un evi que se crea con todo por defecto. Para generar un evi desnudo sin pedir nada al exterior
//    public static string subTipoEvi_enCarga = "enCarga"; //  enCarga : Todo evi que tiene que cargar datos de fuera se genera inidialmente como evi "enCarga", cuando recibe la 
                                                        // informacion necesaria para generarse (KDL de getDetails, lista, arbol, u otros) su subTipo se modificara segun corresponda
//    public static string subTipoEvi_EviPrue_001 = "EviPrue_001"; // EviPrue_001 : Es un evi para ppruebas, Realizara llamadas bien testeadas al exterior

//    public static string subTipoEvi_EviFractal_001 = "EviFractal_001"; // EviPrue_001 : Es un evi para ppruebas, Realizara llamadas bien testeadas al exterior
//    public static string subTipoEvi_Buscador_001 = "Buscador_001"; //  Buscador_001 : Es el subTipo para el buscador mas basico (por Key host, <Debe evolucionar MAFG 2021-02-07)
//public static string subTipoEviTipo_Rama_00 = "Rama_000"; //  Buscador_001 : Es el subTipo para el buscador mas basico (por Key host, <Debe evolucionar MAFG 2021-02-07)

    // ///////////////////////////////////////////////////////////////////      ScriptCtrlBaseDeEvi>().estado
    // ///////////////////////////////////////////////////////////////////
    // Datos propios de todos los EVIs

    public string estado_evi;   // identifica el estado en el que se encuentra el elemento. Los elementos posibles son los que siguen
        public static string estadoEvi_sinIniciar = "sinIniciar";
        public static string estadoEvi_asignadoTipo = "asignadoTipo";  // Se ha definido el subtipo del evi, pero nada mas
        public static string estadoEvi_esperandoDom = "esperandoDom"; //   enCarga (cuando el game objet esta a la espera de informacion en la web para terminar de generarse)
        public static string estadoEvi_domRecibido = "domRecibido"; //   enCarga (cuando el game objet esta a la espera de informacion en la web para terminar de generarse)
        public static string estadoEvi_enCarga = "enCarga"; // El evi base ya tiene sus datos de ayuda a interfaz, pero los esta cargando 
        public static string estadoEvi_errorGenerando = "errorGenerando"; // La generacion del EVI ha fallado (OJOOO un concepto de error procedente del DKS puede y debe generar un evi sin errores)
        public static string estadoEvi_baseLista = "baseLista"; // La base del evi se ha generado por completo satisfactoriamente (ya tiene cargados sus  datos de ayuda a interfaz)
        public static string estadoEvi_operativo = "operativo"; // El evi se ha generado por completo satisfactoriamente, tanto la base como el contenedor

    public string estadoImgAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
    public string estadoIconoAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
    public string estadoAudioAyudaIntf;   // identifica el estado en el que se encuentra la imagen de ayuda a interfaz del concepto
                                          // para los tres estados anteriores, las posibilidades son:
                                              // SinIniciar
                                              // SolicotadoADks
                                              // RecibidoOk
                                              // RecibidoError
                                              // InstaladoEnEvi

    public float pos_x;
    public float pos_y;
    public float pos_z;
    // Hasta aqui ****

    // /////////////////////////////////////////////////////////////////// 
    // ///////////////////////////////////////////////////////////////////
    // DATOS PROPIOS DE TODOS LOS ELEMENTOS TIPO EVI  (especificos para este script)

    // Todos los evis tieme una parte comun "baseDeEVI" y una especifica en el contenedor
    // En este script, se genera el EVI y se contruye la parte general que es comun a todos,
    // esto es : botones de control, informacion de ayuda a interfaz, contenedor, etc..

    public XmlDocument domPropio; // este debe ser un DOM, pero para ir desarrollando pongo el string xml PENDIENTE (2021-01-30 Miguel AFG)
    public XmlNode nodo_E_ContEnBaseDeEvi;  // Nodo del elemento (dentro del KDL del concepto total) que este evi visualiza. EL contenedor lo 
                                            // tiene en "nodo_E_eviFractal" y lo copia aqui cuando se genera este evi base

    public ClassFichero miFicheroAsociado; // Para los SIn Techo, con un tipo de datos que necesiten asociarlos a un fichero, en este objeto de la clase "ClassFichero"
                                           // se almacena la informacion de dicho fichero
                                           // - OJO, el fichero puede no estar en el KEE, puede estar en el DKS, en ese caso, cuando se expande el sin techo, el KEE
                                           // solicita al DKS que se lo envie
//    public List<ClassFichero> misFicherosAGrabar; // Si soy un sin techo tendre un fichero asociado, pero si soy otro tipo de EVI y me han editado, lo normal es que de entre
                                                  // los Sin Techo que contengo, algunos de ellos hayan modificado su fichero asociado. Si ha sido asi, los ficheros asociados que 
                                                  // hay que subir deben estar anotados en esta lista de objetos, por lo que en la operacion de alta, despues de mandar el 
                                                  // KDL de solicitud, tendre que revisar esta lista y enviar al DKS los ficheros que encuentre en ella, para que queden 
                                                  // almacenados en el DKS y relacionados como datos de los elementos Sin Techo que corresponda

    public Canvas este_CanvasEviBase;
        public TMP_Text caja_texInfoCanvasEviBase;  // Caja para el texto de informacion del evi base (descripcion para referencias e instancias, y tipo de dato para sin techo)
            public string textoInfoCanvasEviBase;  // El texto de informacion del evi base (descripcion para referencias e instancias, y tipo de dato para sin techo)
        public TMP_Text caja_Tex_T_deSinTechoCanvasEviBase;  // Caja para el texto del elemento T de del KDL de los elementos sin techo (solo aparecera en los sin techo
            public string texto_T_deSinTechoCanvasEviBase;  // El texto del elemento T del sin techo (si es un sin techo)
        public GameObject este_Panel_Input_Text_SinTecho;  // Panel que almacena el imput de texto y los botones para la edicion de los sin techo
            public TMP_InputField este_Input_Text_T_deSinTecho;// Calja para introducir el nuevo texto (elemento T del KDL) en los sin techo en edicion
              //                public tmp TextArea;
            public Button este_Btn_Guardar_Text_T_deSinTecho;  // Boton de Guardar
            public Button este_Btn_Cancelar_Text_T_deSinTecho;  // Boton de cancelar


    public GameObject Tramoya;
    public GameObject estePunteroUsuario;
    public GameObject estePunteroTramoya;

    public Vector3 posicionEvi;

    public bool arrastrando;  // Si esta a 1 es que estamos arrastrando desde el boton de desplazar
    public bool en_espera_resp_edicion;  // Si esta a 1 es que esperamos una respuesta del usuario (normalmente gruardar o cancelar edicion)
                                         // OJO dependiendo del tipo de evi, la respuesta a la edicion tiene una respuesta distinta
                                         //      - Si es sin techo en edicion puede preguntarse por
                                         //          - Grabar o cancelar cambio de tipo de dato
                                         //          - Grabar o cancelar seleccion de fichero (para los tipos de datos asociados a fichero)
                                         //          - OJO en los datos sin techo   tipo texto y otros, la respuesta a grabar o cancelar edicion se
                                         //              realiza desde "Panel_Input_Text_SinTecho"
    public GameObject evi_pendiente_cambio_tipo_dato_sinTecho;  // Cuando un evi de concepto se superpone en edicion a un sintecho, el evi de concepto queda asociado a la espera
                                                                // de que el usuario conteste si quiere realmente o no cambiar su tipo de dato por el del concepto superpuesto
                                                                // durante este periodo, en esta variable se almacena una referencia a dicho evi de dato.
                                                                // Si no hay ningun sin techo asociado, esta variable estara a null

    // ///////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////
    // //// Vamos con el SphereCollider

    public float SphereCollider_radio;

    // ///////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////
    // //// Vamos con el contenedor

    public GameObject ContenedorDeEvi_01;
    public Vector3 escalaContenedorDeEvi_01;
    public Vector3 posicionContenedorDeEvi_01;

    public float escalaAnchoContenedorDeEvi_01;
    public float escalaAltoContenedorDeEvi_01;
    public float escalaProfundoContenedorDeEvi_01;

    // ////////////////////////////
    // //// Vamos con los componentes de la ayuda a interfaz

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

    // //////   Datos estandar (especificos de cada EVI concreto)
    // Estos datos son los datos propios de la Ayuda Interfaz y otros que todos los EVIs deben tener en el mismo formato, para hacerlos
    // accesibles en cualquier caso, pero que son especificos de cada evi. Estos datos son:

    public string key;                            // key: es el key del concepto (K en KDL - esta en I en KDL)
    public string host;                         // host : es el host del concepto (H en KDL - esta en I en KDL)
    public string cualificador;                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)

    public string ordinal;                         // ordinal : es el ordinal del concepto (O en KDL - esta en F en KDL)
    public string fechUltMod;                         // fechUltMod : es la fecha de ultima modificacion del concepto (M en KDL - esta en F en KDL)

    public string idioma_AyuIntf;               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
    public string imagen_AyuIntf;               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL)
    public string icono_AyuIntf;               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    public string audio_AyuIntf;               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_nombre_AyuIntf;          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_rotulo_AyuIntf;          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
    public string txt_descripcion_AyuIntf;     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)


    public string enResources_ImgAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz
    public string enResources_IconoAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz
    public string enResources_AudioAyuIntf;  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz

    // ///////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////
    // /// Vamos con los Botones principales

    // Declaramos los game objet de los botnes
    public GameObject Btn_Evi_Info;
    public GameObject Btn_Evi_Maxi_Mini;
    public GameObject Btn_Evi_Desplazar;
    public GameObject Btn_Evi_Opciones;
    public GameObject Btn_Evi_Caja_opciones;

    /* **************
    public Material material_icono_AyuIntf;  // Para el boton de Btn_Evi_Info;
    public Texture textura_icono_AyuIntf;

    public Material material_BtnMaxi_Mini;
    public Texture textura_BtnMaxi_Mini;

    public Material material_BtnDesplazar;
    public Texture textura_BtnDesplazar;

    public Material material_BtnOpciones;
    public Texture textura_BtnOpciones;
**************** */

    // escalas y posiciones botones 
    // Botones principalews
    // Declaramos los parametros para escalar los botones
    public float escalaAnchoBoton;
    public float escalaAltoBoton;
    public float escalaProfundoBoton;

    // Para los botones dentro de la caja de popciones
    public Vector3 escalaBtn_BaseDeEvi_N1_Desactivado;  // El tamaño es el mismo para todos los Btn_BaseDeEvi_N2_1
    public Vector3 escalaBtn_BaseDeEvi_N1_Activado;

    protected float posicionAnchoBaseBotones;
    protected float posicionAltoBaseBotones;
    protected float posicionProfundoBaseBotones;

    // Posicion de cada boton
    public Vector3 positionBtn_Evi_Info;
    public Vector3 positionBtn_Evi_Maxi_Mini;
    public Vector3 positionBtn_Evi_Desplazar;
    public Vector3 positionBtn_Evi_Opciones;

    // Los botones de opciones se declaran en sus respectivos scripts


    // ////////////////////////////////////////////
    // Miscelaneos
    public bool contenidoCumplimentado;
    public bool maximizado; // True: el evi esta maximizado. False : el evi esta minimizado

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  Awake()
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-xx-xx
    /// Ultima modificacion :
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    /// </summary>
    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        Tramoya = GameObject.FindWithTag("Tramoya");

        SphereCollider_radio = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi;

        //       misFicherosAGrabar = new List<ClassFichero>(); // Son las solicitudes que el elemento de inerfaz debe atender. 

        este_CanvasEviBase = transform.GetChild(7).gameObject.GetComponent<Canvas>();

        caja_texInfoCanvasEviBase = este_CanvasEviBase.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        caja_Tex_T_deSinTechoCanvasEviBase = este_CanvasEviBase.gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

        este_Panel_Input_Text_SinTecho = este_CanvasEviBase.gameObject.transform.GetChild(2).gameObject;

        este_Input_Text_T_deSinTecho = este_Panel_Input_Text_SinTecho.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>();
            //   public tmp TextArea;
        este_Btn_Guardar_Text_T_deSinTecho = este_Panel_Input_Text_SinTecho.gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        este_Btn_Cancelar_Text_T_deSinTecho = este_Panel_Input_Text_SinTecho.gameObject.transform.GetChild(2).gameObject.GetComponent<Button>();

        texto_T_deSinTechoCanvasEviBase = "no soy un sin techo";  // Lo ponemos por defecto, el sin techo lo sustituye y cualquier otro no lo enseña

        // Por si fuera un sin techo asociado a un fichero, instanciamos el objeto asociado al fichero correspondiente
        miFicheroAsociado = new ClassFichero();
        miFicheroAsociado.nombre_origen = "";
        miFicheroAsociado.nombre_enKee = "";  // Cuando nombre_Unity = ""; esto quiere decir que no se ha realizado ninguna operacion con el fichero. Esto es que no se ha seleccionado
                                                // ningun fichero del sistema (para asociarlo a este sin techo), y que tampoco se ha descargado el fichero del DKS
        miFicheroAsociado.modificado = false;  // Cuando modificado = false; esto quiere decir que NO se modificado por edicion
                                               // Cuando modificado = true; esto quiere decir que SI se modificado por edicion
        
        //        esteCanvasEviBase = this.GetComponent<Canvas>();

        // Valores por defecto
        // Salvo que alguien diga otra cosa, el evi arranca en modo navegacion
        this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_navegacion;
        estado_evi = estadoEvi_sinIniciar; // Se ha generado el evi, pero nada mas

        estadoImgAyudaIntf = "SinIniciar";  
        estadoIconoAyudaIntf = "SinIniciart";
        estadoAudioAyudaIntf = "SinIniciar";

        enResources_ImgAyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/sinFoto";
        enResources_IconoAyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/iconoImgPorDefecto";  // Localizacion del fichero enel DKS
        enResources_AudioAyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudioPorDefecto";  // Contiene el path, con nombre incluido para localizar el fichero en el directorio "Resources" de esta interfaz

        material_imagen_AyuIntf = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo a la imagen de fondoobjeto
        material_ImgAyuIntfBaseDeEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo a la imagen pequeña que acompaña al icono del objeto
        material_icono_AyuIntf = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialGenerico);  // Instanciamos un nuevo material para asignarselo al icono del objeto


        // ponemos el DOM por defecto, para que tenga un valor por defecto
        string funcionQueLLama = "ScriptCtrlBaseDeEvi";
        int codigo = 0;
        string mensajeDeDOMPorDefecto = "Este es el DOM por defecto  o desde ScriptCtrlBaseDeEvi";

        domPropio = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().generaDomPorDefecto(funcionQueLLama, codigo, mensajeDeDOMPorDefecto);

        //       estePunteroUsuario = Usuario.GetComponent<ScriptDatosInterfaz>().PunteroUsuario;
        estePunteroUsuario = GameObject.FindWithTag("PunteroUsuario");
        // Obtenemos ahora el puntero de la tramoya
        for (int i = 0; i < estePunteroUsuario.transform.childCount; i++)
        {
            GameObject child = estePunteroUsuario.transform.GetChild(i).gameObject;
            if (child.name == "PunteroTramoya"){ estePunteroTramoya = child;}
            //Do something with child
        }

        arrastrando = false;  // En principio el evi no esta en arrastre
        en_espera_resp_edicion = false;  // En principio no se espera respuesta alguna
        evi_pendiente_cambio_tipo_dato_sinTecho = null; // En principio no hay evi ce concepto pendiente para cambio de tipo de dato

        contenidoCumplimentado = false; // En principio el evi esta sin contenido
//        this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.SetActive(false);
//        this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.SetActive(false);
//        maximizado = false; // En principio el evi esta minimizado


        // Puesto que creamos un nuevo evi, aumentamos el indice que controla los que llevamos
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numEvis++;
    }  // Fin de - void Awake()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  Start ()
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-xx-xx
    /// Ultima modificacion :
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    /// </summary>
    void Start ()
	{

        // ////////////////////////////////////////
        // Para la posicion inicial del EVI :
        // Si estamos en un muro le pedimos al muro donde esta colocado el evi, que le de la posicion inicial
        if (transform.parent.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
        {
            transform.localPosition = transform.parent.GetComponent<ScriptCtrlMuroTrabajo>().damePosicionIniLocal();
        }
        else  // Si no estamos en un muro, por ahora nos colocamos en la posicion 0,0,0. PENDIENTE MAFG 2021-03-24
        {
            // pos_x = Random.Range (-5.0f, 5.0f);
            // pos_y = 0.0f;
            // pos_z = Random.Range (-5.0f, 5.0f);

            pos_x = 0f;
            pos_y = 0f;
            pos_z = 0f;
            posicionEvi = new Vector3 (pos_x, pos_y, pos_z);
            transform.localPosition = posicionEvi;
        }

        // Definimos el giro correspondiente
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Para el tamaño
        transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().estaEscalaBaseDeEvi_01;

        // Ponemos los datos propios de este EVI como elemento de interfaz general
                // EL identificador "idElementIntf" ya se cargo al generarlo en "ScriptDatosInterfaz"
                // Cargamos ahora el tipo de elemento y el identificador dentro del tipo
        GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi); // Ponemos el identificador de tipo
                // El subtipo "subTipoElementIntf" deben ponerse cuando el evi se haya cargado

        gameObject.name = "Evi_" + GetComponent<ScriptDatosElemenItf>().dameIdElementIntf(); // Ponemos un nombre para identificarlo en el entorno de trabajo

        // ///////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////
        // /// Vamos calculando los tamaños principales

        // Tamaño y posición para los botones del EVI
        // Definimos los parametros a utilizar
        escalaAnchoBoton = 0.5f;
        escalaAltoBoton = escalaAnchoBoton;
        escalaProfundoBoton = 1.1f;

        posicionAnchoBaseBotones = escalaAnchoBoton/2; 
        posicionAltoBaseBotones =  escalaAltoBoton/2;
        posicionProfundoBaseBotones = 0f;

        factorEscalaImgAyuIntfBaseDeEvi = 4f;


        // Calculamos escalas de botones
        escalaBtn_BaseDeEvi_N1_Desactivado = new Vector3(escalaAnchoBoton, escalaAltoBoton, escalaProfundoBoton); // En la escala (ancho, alto , profundo)
        escalaBtn_BaseDeEvi_N1_Activado = new Vector3(escalaAnchoBoton * 2, escalaAltoBoton * 2, escalaProfundoBoton);
            // Calculamos posiciones de botones
        positionBtn_Evi_Info = new Vector3(-posicionAnchoBaseBotones, -posicionAltoBaseBotones, posicionProfundoBaseBotones);
        positionBtn_Evi_Maxi_Mini = new Vector3(-posicionAnchoBaseBotones, posicionAltoBaseBotones, posicionProfundoBaseBotones);
        positionBtn_Evi_Desplazar = new Vector3(posicionAnchoBaseBotones, -posicionAltoBaseBotones, posicionProfundoBaseBotones);
        positionBtn_Evi_Opciones = new Vector3(posicionAnchoBaseBotones, posicionAltoBaseBotones, posicionProfundoBaseBotones);

        //        positionBtn_Evi_Info = new Vector3(-posicionAnchoBaseBotones, posicionProfundoBaseBotones, posicionAltoBaseBotones); // En posición (ancho, profundo, alto)
        //        positionBtn_Evi_Maxi_Mini = new Vector3(posicionAnchoBaseBotones, posicionProfundoBaseBotones, -posicionAltoBaseBotones);
        //        positionBtn_Evi_Desplazar = new Vector3(-posicionAnchoBaseBotones, posicionProfundoBaseBotones, -posicionAltoBaseBotones);
        //        positionBtn_Evi_Opciones = new Vector3(posicionAnchoBaseBotones, posicionProfundoBaseBotones, posicionAltoBaseBotones);

        // ///////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////
        // /// Generamos el contenedor

        // Parametros de ContenedorDeEvi_01
        escalaAnchoContenedorDeEvi_01 = escalaAnchoBoton * 10f;  // El contenedor es 4 veces el tamaño del oto (el concepto minimizado
        escalaAltoContenedorDeEvi_01 = escalaAnchoContenedorDeEvi_01;
        escalaProfundoContenedorDeEvi_01 = 1f;

        escalaContenedorDeEvi_01 = new Vector3(escalaAnchoContenedorDeEvi_01, escalaAltoContenedorDeEvi_01, escalaProfundoContenedorDeEvi_01); // En la escala (ancho, alto , profundo)
        posicionContenedorDeEvi_01 = new Vector3(-(escalaAnchoBoton + (escalaAnchoContenedorDeEvi_01 / 2)), (escalaAltoBoton + (escalaAnchoContenedorDeEvi_01 / 2)), 0);

        //        ContenedorDeEvi_01 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ContenedorDeEvi_01);
        ContenedorDeEvi_01.transform.SetParent(gameObject.transform);
        ContenedorDeEvi_01.transform.localScale = escalaContenedorDeEvi_01;
        ContenedorDeEvi_01.transform.localPosition = posicionContenedorDeEvi_01;

       // Vamos con la imagen que acompaña al icono
                //        escalaImgAyuIntfBaseDeEvi = new Vector3((escalaAnchoContenedorDeEvi_01 / 2f), (escalaAltoContenedorDeEvi_01 / 2f), escalaProfundoContenedorDeEvi_01); // En la escala (ancho, alto , profundo)
        escalaImgAyuIntfBaseDeEvi = new Vector3((escalaAnchoBoton * factorEscalaImgAyuIntfBaseDeEvi), (escalaAnchoBoton * factorEscalaImgAyuIntfBaseDeEvi), escalaProfundoBoton); // En la escala (ancho, alto , profundo)
        posicionImgAyuIntfBaseDeEvi = new Vector3(escalaAnchoBoton + (escalaImgAyuIntfBaseDeEvi[0] / 2f), (escalaImgAyuIntfBaseDeEvi[1] / 2f), 0f); 

//        ImgAyuIntfBaseDeEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ImgAyuIntfBaseDeEvi);
        ImgAyuIntfBaseDeEvi.transform.SetParent(gameObject.transform);
        ImgAyuIntfBaseDeEvi.transform.localScale = escalaImgAyuIntfBaseDeEvi;
        ImgAyuIntfBaseDeEvi.transform.localPosition = posicionImgAyuIntfBaseDeEvi;
                // Inicialmente desactivamos el texto de ayuda a interfaz que se activara al pasar por encima del botod de info
        ImgAyuIntfBaseDeEvi.SetActive(false);

        // Los datos de ayuda interfaz se cargan en el EVI mediante "cargaDatosBaseDeEvi()"

        // ///////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////
        // /// Generamos los botones del control general del EVI

            // Vamos generando botones y asignandoles escalas y posiciones 
            // con los botones priincipales
//        Btn_Evi_Info = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Info);
        Btn_Evi_Info.transform.SetParent(this.transform);
        Btn_Evi_Info.transform.localScale = escalaBtn_BaseDeEvi_N1_Desactivado;
        Btn_Evi_Info.transform.localPosition = positionBtn_Evi_Info;

//        Btn_Evi_Maxi_Mini = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Maxi_Mini);
        Btn_Evi_Maxi_Mini.transform.SetParent(this.transform);
        Btn_Evi_Maxi_Mini.transform.localScale = escalaBtn_BaseDeEvi_N1_Desactivado;
        Btn_Evi_Maxi_Mini.transform.localPosition = positionBtn_Evi_Maxi_Mini;

//        Btn_Evi_Desplazar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Desplazar);
        Btn_Evi_Desplazar.transform.SetParent(this.transform);
        Btn_Evi_Desplazar.transform.localScale = escalaBtn_BaseDeEvi_N1_Desactivado;
        Btn_Evi_Desplazar.transform.localPosition = positionBtn_Evi_Desplazar;

//        Btn_Evi_Opciones = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Opciones);
        Btn_Evi_Opciones.transform.SetParent(this.transform);
        Btn_Evi_Opciones.transform.localScale = escalaBtn_BaseDeEvi_N1_Desactivado;
        Btn_Evi_Opciones.transform.localPosition = positionBtn_Evi_Opciones;

            // Declaramos la caja de opcionas, la dimensionaremos y localizaremos al generarla
//        Btn_Evi_Caja_opciones = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Caja_opciones);
        Btn_Evi_Caja_opciones.transform.SetParent(Btn_Evi_Opciones.transform);
        Btn_Evi_Caja_opciones.transform.localScale = escalaBtn_BaseDeEvi_N1_Desactivado;
        Btn_Evi_Caja_opciones.transform.localPosition = positionBtn_Evi_Opciones;

        // ///////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////
        // /// onfiguramos el camvas que acompaña al eviBase "CanvasEviBase" para el estado inicial (solo es visible el texto sin techo si el evi lo es)
        // OJOOO esto hay que hacerlo en el update cuando los datos del DOM ya estan cargados

 //       este_CanvasEviBase.gameObject.SetActive(true);

        // El resto hay que configurarlo mediante una corrutina, ya que el contenedor todabia no esta generado por completo
        StartCoroutine(configuraInicioCanvasEviBase());




        //        Panel_Input_Text_SinTecho.SetActive(false);

        /* *****************************

        CanvasEviBase = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasEviBase);
        CanvasEviBase.transform.SetParent(this.transform);  // El panel de edicion es hijo del canvas general (gerarquit¡a Unity)
                                                            //        this.transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Add(CanvasEviBase.gameObject);  // El panel es hijo del evi base, ya que debe moverse con el, borrarse con el, etc...
                                                            //          // Esto lo quito porque me casca al revisar la list de hijos, diciendo que esto no es un objeto MAFG 2021-10-05

        // AL hacer el canvas hijo del evi Base, Unity modifica los parametros de escala y los ejes de rectTransform, porque los recalcula
        // considerando que el canvas se queda donde lo ha instanciado, por esto tenemos que volver a definirlos segun nuestras necesidades
        // Lo escalamos
        Vector3 esc_CanvasEviBase = new Vector3(1f, 1f, 1f);
        CanvasEviBase.transform.localScale = esc_CanvasEviBase;
        // Lo ajustamos al marco del rectTransform del padre (EviBase)

//        CanvasEviBase.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(GetComponent<RectTransform>().rect.xMin, 0f, 0f);

        CanvasEviBase.GetComponent<RectTransform>().rect.xMin.Equals(0f);



        CanvasEviBase.GetComponent<RectTransform>().ForceUpdateRectTransforms();

        //        Vector2 unVector2 = new Vector2(0.73f, 0.84f);

        //       CanvasEviBase.GetComponent<RectTransform>().anchoredPosition = unVector2;

        // CanvasEviBase.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(Left, 2f, 1f);
        CanvasEviBase.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(GetComponent<RectTransform>().rect.xMin, 0f, 0f);


        //CanvasEviBase.GetComponent<RectTransform>().rect.xMin.Equals(5f);

        //        Vector3 pos_CanvasEviBase = new Vector3(100f, 100f, 0f);
        //        Quaternion rot_CanvasEviBase = new Quaternion.Euler(0, 0, 0);
        //        CanvasEviBase.GetComponent<RectTransform>().SetPositionAndRotation(pos_CanvasEviBase, Quaternion.Euler(0, 0, 0));
        //        CanvasEviBase.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(GetComponent<RectTransform>().rect.xMin, 0f, 0f );  // El panel de edicion es hijo del canvas general (gerarquit¡a Unity)
        //        CanvasEviBase.transform.localPosition = pos_CanvasEviBase;


        // Lo escalamos
        float esc_x_CanvasEviBase = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaLocal_x_BaseDeEvi_01 *
                                                this.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoContenedorDeEvi_01 *
                                                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().razon_escalaLocal_x_ContenedorEviSinTecho;
        float esc_y_CanvasEviBase = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaLocal_y_BaseDeEvi_01 *
                                                this.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoContenedorDeEvi_01 *
                                                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().razon_escalaLocal_y_ContenedorEviSinTecho;
        float esc_z_CanvasEviBase = 1f;

        Vector3 esc_CanvasEviBase = new Vector3(esc_x_CanvasEviBase, esc_y_CanvasEviBase, esc_z_CanvasEviBase);
        CanvasEviBase.transform.localScale = esc_CanvasEviBase;
        // Definimos el giro correspondiente
        CanvasEviBase.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Lo localizamos
        Vector3 coordConocidas = this.transform.localPosition; // Estas son las coordenadas del EVI base, tenemos que ajustarlas para irnos al centro del contenedor
        Vector3 coordConocidasAjustada = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().ajustaLocPanelEdicionSinTecho(coordConocidas, CanvasEviBase.gameObject);
        // "coordConocidas" son las coordenadas del Evi referenciadas al muro en el que reside, tenemos que transformarlas para referirlas al Canva
        // ya que el "Panel_Input_Text_SinTecho" es hijo del canvas y por tanto debe referenciarse en sus coordenadas (ver la funcion que lo hace "convierteCoordenadas")
        CanvasEviBase.transform.localPosition = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().convierteCoordenadas(coordConocidasAjustada, ScriptLibGestorEvis.convCoord_muro_a_canvas);
        *************************** */

    } // FIn de - void Start ()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  Update() 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-xx-xx
    /// Ultima modificacion :2021-07-08, incluyo la gestion del cambio de modo
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    ///         1.) Gestionamos el cambio de modo. Para ello atendemos a la naturaleza del padre de este eviBase, en la gerarquia de unity
    ///             Por ahora actuamos igual para todos los subtipos de EVI
    ///             Veamos las posibilidades que contemplamos:
    ///                 1.1.) Si existe el padre de este evi base, cambiamos el modo atendiendo a donde se aloja en evi:
    ///                         1.1.1.)  Si esta alojado en la tramoya (el padre es la ramoya ) => modo = modoElemItf_enTramoya
    ///                         1.1.2.)  Si esta alojado en un muro (el padre es el muro ) 
    ///                                 1.1.2.1.)  SI el muro esta en modo navegacion => el evi se pone modo = modoElemItf_navegacion
    ///                                 1.1.2.2.) SI el muro esta en modo edicion => el evi se pone modo = modoElemItf_edicion
    ///         2.) Gestionamos las solicitudes pendientes de este objeto
    ///         3.) Manejamos el arrastre
    ///         4.) Gestionamos la carga de datos del evi
    /// </summary>
    void Update ()
	{

        // //////////////////////////////////////////////////////////////////
        // 1.) Gestionamos el cambio de modo.
        // Si hemos cambiado de modo, actuamos en consecuencia, ajustustando la configuracion de los botones segun proceda
        if (this.gameObject.GetComponent<ScriptDatosElemenItf>().modoAnterior != this.gameObject.GetComponent<ScriptDatosElemenItf>().modo)
        {
            this.Btn_Evi_Caja_opciones.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().configuraBotonesOpcionesEvi(this.gameObject.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf, this.gameObject.GetComponent<ScriptDatosElemenItf>().modo);
            // No actualizamos el modo anterior ya que hay que actualizarlo para todas las funciones del objeto, al final de todos los updates. (Se hace en "ScriptDatosElemenItf => LateUpdate()")
            // El cambio de modo se hace atendiendo al la naturaleza del padre donde este albergado (muro de navegacion, edicion, tramoya,...) ( se hace en "ScriptDatosElemenItf => Update()")

        }

        // //////////////////////////////////////////////////////////////////
        // 2.) Gestionamos las solicitudes pendientes de este objeto. Miramos si tenemos soicitudes pendientes, y si es asi, se resuelven
        if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)
        {
            // Las solicitudes que pueden llegar son :
            //  1.) Respuesta a la pregunta que se hizo al usuario sobre si quiere abandonar la edicion
            //  2.) Respuesta a la pregunta que se hizo al usuario sobre si quiere grabar en el DKS los cambios realizados durante la edicion
            foreach (GameObject solicitud in GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScreiptCtrlEviTipo_buscador_00 => update antes de atiende mari llanos. Con tamaño de lista de solicitudes : " + GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count); }
                atiendeMariLlanos(solicitud);  // Llamamos a quien debe gestionar la solicitud
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScreiptCtrlEviTipo_buscador_00 => update con tamaño de lista de solicitudes : " + GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count); }
            }
            GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Clear(); // Debemos habre procesado todos los elementos de la lista y esta estará vacia
                                                                                   // No se pueden borrar en la funciones a las que se llama desde aqui porque entonces casca el foreach
                                                                                   // por modificar la lista mientras se ejecuta
                                                                                   // Si hubiera que eliminar solo algunos habria que generar una lista a parte con los que hubiera que 
                                                                                   // borrar y luego borrarlos uno a uno fuera de este foreach (creo) MAFG 2021-02-14
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)


        // //////////////////////////////////////////////////////////////////
        // 3.) Arrastres

        // //////////////////////////////////////////////////////////////////
        // 3.1.) Manejamos el arrastre. Si estamos desplazando, vamos moviendo el EVI
        if (arrastrando)
        {
            // Movemos el evi base, se moveran todos los descendientes del objeto
            Vector3 posicionDeEviEnPadre = calculaPosicionEvi(); 
            this.transform.localPosition = posicionDeEviEnPadre;

            // Puede haber elementos hijos del evi que no son descendiente del gameobect (evi base) y sin embargo tienen que desplazarse con el
            // Estos son :
            //      - "Panel_Input_Text_SinTecho" Si teiene asociado un panel de entrada de texto para edicion "Panel_Input_Text_SinTecho", este es descendiente del canvas y no lo es 
            //          del game object, por lo que debemos moverlo aparte sobre el canvas para que se mueva asociado al evi base

            foreach (GameObject hijo in transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if (hijo.tag == "Panel_Input_Text_SinTecho")
                {
                    Vector3 posicionAjustada = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().ajustaLocPanelEdicionSinTecho(posicionDeEviEnPadre, hijo);
                    Vector3 posicionPanelEnCanvas = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().convierteCoordenadas(posicionAjustada, ScriptLibGestorEvis.convCoord_muro_a_canvas);
                    hijo.transform.localPosition = posicionPanelEnCanvas;
                } // FIn de - 
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)


        }  // Fin de - if (arrastrando)


        // //////////////////////////////////////////////////////////////////
        // 3.2.) Manejamos el arrastre del pendiente de tipo de dato. 
        //      - Si somos un sin techo en edicion, y se nos ha superpuesto un concepto y estamos esperando que el usuario conteste
        //      si realmente quiere cambiar el tipo de dato de este sin techo. Lo dejamos al lado nuestro y acompañandonos hasta que el usuario conteste

        if ((this.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &&
            (this.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &&
            (this.gameObject.transform.parent.gameObject == ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo) &&
            (this.gameObject.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion) &&
            (evi_pendiente_cambio_tipo_dato_sinTecho != null) &&
            (en_espera_resp_edicion))
        {
            // Movemos el evi base, se moveran todos los descendientes del objeto

            /////////////////
            Vector3 posicionDelSinTecho = this.transform.localPosition;
            float distancia_a_mantener = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi * 1.1f * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;
            Vector3 desplazamient_tipo_dato_en_espera = new Vector3(distancia_a_mantener, -distancia_a_mantener, 0); // Lo desplazamos para que quede abajo a la derecha del sin techo
            Vector3 posicion_tipo_dato_en_espera = posicionDelSinTecho + desplazamient_tipo_dato_en_espera;
            evi_pendiente_cambio_tipo_dato_sinTecho.transform.localPosition = posicion_tipo_dato_en_espera;


        }  // Fin de - if ((this.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &&...

        // //////////////////////////////////////////////////////////////////
        // 4.) Gestionamos la carga de datos del evi. si la base del evi esta lista es que ya tiene todo listo y no hay que mirar mas. SERA EL CONTENEDOR QUIEN CUANDO ESTE LISTO PONGA EL EVI OPERATIVO
        if (estado_evi != estadoEvi_baseLista)
        {
            // Cargamos los datos basicos del EVI (datos de ayuda a interfaz) dependiendo del subtipo de este 
            if (estado_evi == estadoEvi_asignadoTipo) // OJOOO debe estar cargado el contenido en el contenedor
            {
                string subtipo = GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
                // Se cargan con datos especificos (no necesitan el kdl, si no que se cargan con ayuda interfaz predeterminas) los evis subtipo:
                //      - subTipoElemItf_evi_rama
                //      - subTipoElemItf_evi_buscador_00
                //      - subTipoElemItf_evi_lista_00
                //      - subTipoElemItf_evi_camino_00
                //      - subTipoElemItf_evi_arbol_00
                //      - subTipoElemItf_evi_EviPrue_001
                if ((subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_rama) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_EviPrue_001))
                {
                    estado_evi = estadoEvi_enCarga;  // Indiacmos que el evi ya tiene sus datos basicos y los esta cargando
                    cargaDatosBaseDeEvi_Especifico();  // Cargamos los datos basicos del evi, dependiendo del sustipo que sea
                }
                // Se gargan con los datos del KDL de concepto los evis de subtipo :
                //      - subTipoElemItf_evi_RefFractal
                //      - subTipoElemItf_evi_InstFractal
                //      - subTipoElemItf_evi_sinTecho_00
                else if ((subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal) ||
                    (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00))
                {
                    estado_evi = estadoEvi_esperandoDom;  // Indiacmos que el evi ya tiene sus datos basicos y los esta cargando
                    // Poner una configuracion de espera par que quede mas informativo (PENDIENTE MAFG 2021-04-15)
                }
                else if (subtipo == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
                {
                    // Los datos deben estar cargados, ya que se cargan al instanciarlo en "" utilizando los datos del elemento de interfaz al que hace referencia
                    estado_evi = estadoEvi_enCarga;  // Indiacmos que el evi ya tiene sus datos basicos y los esta cargando
                    // Poner una configuracion de espera par que quede mas informativo (PENDIENTE MAFG 2021-04-15)
                }
                else
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde ScriptCtrlBaseDeEvi => Update => if (estado_evi == estadoEvi_asignadoTipo). Con SUbtipo desconocido = " + subtipo); }
                }
            }  // FIn de - if (estado_evi == estadoEvi_asignadoTipo)

            // Si hemos recibido los datos base del EVI (normalmente porque se solicitaron a un DKS externo), estaran en el DOM. Y el evi estara en el 
            // estado correspondiente y habra que cargar los datos en la variable "domPropio"
            if (estado_evi == estadoEvi_domRecibido) // Si ya tenemos el domPropio, cargamos los datos en el gameObect
            {
                cargaDatosBaseDeEvi();

                // Ponemos el estado de minimizado por defecto
                // Lo hacemos aqui porque si desactivamos el contenedor antes de finalizar la carga, hace cosas raritas
                this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.SetActive(false);
                maximizado = false; // En principio el evi esta minimizado

                // ///////////////////////////////////////////////////////
                // ///////////////////////////////////////////////////////
                // /// onfiguramos el camvas que acompaña al eviBase "CanvasEviBase" para el estado inicial (solo es visible el texto sin techo si el evi lo es)

//                este_CanvasEviBase.gameObject.SetActive(false);

                // El resto hay que configurarlo mediante una corrutina, ya que el contenedor todabia no esta generado por completo
                StartCoroutine(configuraInicioCanvasEviBase());

            }
        }  // Fin de - if (estado_evi != estadoEvi_baseLista)

        // ES EL CONTENEDOR, QUIEN AL ESTAR LISTO Y TENER LA BASE DEL EVI LISTA, DEBE PONER EL EVI EN ESTADO OPERATIVO
        //            if (estado_evi == estadoEvi_baseLista Y CONTENEDOR OPERATIVO) { estado_evi = estadoEvi_operativo; } 


        // //////////////////////////////////////////////////////////////////
        // Si hemos terminado de recibir el concepto que debe contener, pasamos a cargarlo
        //      Recordamos que el concepto se carga del DKS mediante una corrutina, por lo que es posible que se reciba varios frames despues de crear el EVI
        if ((estadoImgAyudaIntf == "texturaCargada") &
            (estadoIconoAyudaIntf == "texturaCargada") &
            (estadoAudioAyudaIntf == "audioListoParaCargar") ) // OJOOO debe estar cargado el contenido en el contenedor
        {
            estado_evi = estadoEvi_baseLista;
        }  // Fin de - if (estado_evi == "domCargado")

    } // Fin de - void LateUpdate ()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cargaDatosBaseDeEvi 
    /// Observaciones : Este metodo carga los datos estandar el concepto, referencia, instancia o sin techo, en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del concepto, referencia, instancia, o en el caso del sin techo, del tipo de dato que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-06
    /// Ultima modificacion :
    ///         - 2021-05-16  Para incluir los datos de informacion de los evis base asociados a instancias y sin techo
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         - Esta funcion carga los datos de ayuda a interfaz y basicos de EVIS estandar de la interfaz. Sean evis de referencias, instancia o sin techo
    ///         - Cuando un EVI contiene informacion que viene de fuera (EviFractal de getdetails, u otros)
    ///         el EVI puede haberse cargado primero con unos datos por defecto (En carga), hasta que se obtiene la informacion de los dksS CORRESPONDIENTES
    ///         entonces se darga desde el Update de esta misma clase, atendiendo a los estados (en carga, Xmlrecibido, etc...
    ///         segun vaya llegando la informacion se va sobreescribiendo sobre la que hay por defecto en el EVI.
    ///         - Si el evi lo es de instancia o de sin techo, normalmente, el KDL asociado ya estara disponible localmente, puesto que normalmente este evi sera
    ///         el resultado de la expansion de un evi que ya tenemos, o el de una extraccion de la instancia de un evi de referenci del que tambien debemos tener
    ///         su KDL
    ///         
    ///         PASOS DEL PROCESO
    ///         1.) Analisis del tipo  y subtipo del evi base
    ///         - OJOOOO los datos que carga el evi base dependen de la naturaleza del contenedor que porta. Esta informacion esta en el tipo "tipoElementIntf" y 
    ///         subtipo "subTipoElementIntf" de elemento de interfaz (en "ScriptDatosElemenItf")
    ///             1.1.) Tipo : usuario, rama, muro, solicitud, agente... Por ahora estamos desarrollando elementos de interfaz tipo EVI (MAFG 2021-05-16)
    ///             Por ahora nos cupamos solo de los elementos TIPO EVI.
    ///                 tipoElementIntf = tipoElemItf_evi  ( en "ScriptDatosElemenItf")
    ///             1.2. ) Los subtipo de elementos tipo evi pueden ser de varios tipos. Nosotros aqui, por ahora nos ocupamos solo de los que estan en la familia de 
    ///                     los fractales para evi base (MAFG 2021-05-16), que son, segun el valor de subTipoElementIntf ( en "ScriptDatosElemenItf" ) los que siguen : 
    ///                 1.2.1.) subTipoElementIntf = subTipoElemItf_evi_baseRefFractal  => Para los evis que muestran un concepto completo (procedente normalmente de una busqueda o la expansion de una referencia o una extraccion)
    ///                          En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto referenciado
    ///                 1.2.2.) subTipoElementIntf = subTipoElemItf_evi_baseInstFractal  => Para los evis que muestran una instancia (procedente normalmente de la expansion de una referencia o una extraccion)
    ///                          En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto instanciado
    ///                 1.2.3.) subTipoElementIntf = subTipoElemItf_evi_baseSinTecho_00  => Para los evis que muestran un sin techo (procedente normalmente de la expansion de una referencia o una extraccion)
    ///                          En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto asociado al tipo de datos del sin techo
    ///                          
    ///         2.) Carga de los datos correspondientes
    /// 
    /// 
    /// </summary>
    public void cargaDatosBaseDeEvi()
    {
        // La respuesta que viene en la solicitud sera el DOM del evi en que estamos cargando 

        XmlNode KDL_Raiz = domPropio.DocumentElement;

        XmlNode nodoDeInformacion = KDL_Raiz;  // Este es el nodo del que tomaremos la informacion para ponerla en el evi base (por defecto el raiz "C"), segun el caso :
                                               //   1.2.1.) subTipoElementIntf = subTipoElemItf_evi_baseRefFractal  => Para los evis que muestran un concepto. En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto referenciado
                                               //   1.2.2.) subTipoElementIntf = subTipoElemItf_evi_baseInstFractal  => Para los evis que muestran una instancia. En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto instanciado
                                               //   1.2.3.) subTipoElementIntf = subTipoElemItf_evi_baseSinTecho_00  => Para los evis que muestran un sin techo. En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto asociado al tipo de datos del sin techo

//        GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_RefFractal);  // Ponemos a pelo que es un evi fractal para hacer la carga como procede
//        GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal);  // Ponemos a pelo que es un evi fractal para hacer la carga como procede

        // ///////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////
        // 1. Analizamos el tipo y subtipo del evi base

        // El tipo y el subtipo de todos los evis esta en la decalracion de variables de "ScriptDatosElemenItf". segun sea el tipo y el subtipo, los datos del evi base
        // son distintos, por ejemplo
        //  Tipo : usuario, rama, muro, solicitud, agente

        // Hasta ahora, al DKS se acecde por el servicio de "GETDetails", el cual devuelve directamente el KDL del concepto. Pero en un futuro, al 
        // DKS se le podran realizar preguntas mas elavoradas que ontendran respuestas mas elaboradas (como una lista de conceptos, un arbol, una serie de ramas deo
        // conexion o un concepto simple). En esta situacion SERA NECESARIO PROCESAR EL KDL RECIBIDO DE RESPUESTA PARA VER SI LO QUE RECIBIMOS ES UN CONCEPTO
        // SIMPLE, UNA LISTA, UNA RAMA, U OTRA COSA. Y segun lo que se reciba, actuar en consecuencia a la hora de construir el EVI que debe mostrarlo, ya que segun se reciba
        // una u otra cosa, habra que generar :
        //          - un elemento de interfaz tipo EVI, subtipo Fractal, si es un concepto
        //          - un elemento de interfaz tipo EVI, subtipo lista, si es una lista
        //          - un elemento de interfaz tipo EVI, subtipo arbol, si es un arbol
        //
        // POR AHORA, no analizamos la respuesta (ya que solo pedimos un getdetails) y ponemos directamente un EVI subtipo fractal
        // PENDIENTE (MAFG 2021-02-12) habra que implementar esto para poner listas y arboles. Un primer paso puede ser hacer llamadas especificas como esta
        //  OJOOOO el evi Generador (fractal, por ejemplo) instancia un evi base al que el pone el subtipo que quiere. Por lo que puede ser el el que analice el KDL que ha llegado como respuesta
        string TipoElementIntf = GetComponent<ScriptDatosElemenItf>().dameTipoElementIntf();
        string subTipoElementIntf = GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();

        //  1.1.) Tipo : usuario, rama, muro, solicitud, agente... Por ahora estamos desarrollando elementos de interfaz tipo EVI (MAFG 2021-05-16)
        if (TipoElementIntf != ScriptDatosElemenItf.tipoElemItf_evi)
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Hola desde ScriptCtrlBaseDeEvi -  Desde domCargado - cargaDatosBaseDeEvi, intentamos cargar dato en a un elemento de interfaz que no es un evi con cargaDatosBaseDeEvi = " +
                GetComponent<ScriptDatosElemenItf>().tipoElementIntf +
                " - y con subTipoElementIntf : " + GetComponent<ScriptDatosElemenItf>().subTipoElementIntf);
            }
            return;
        }


        // 1.2. ) Los subtipo de elementos tipo evi pueden ser de varios tipos. Nosotros aqui, por ahora nos ocupamos solo de los que estan en la familia de los fractales para evi base (MAFG 2021-05-16), 
        //     que son, segun el valor de subTipoElementIntf ( en "ScriptDatosElemenItf" ) los que siguen :

        if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
        {

            /* ******************  2022-09-30 ********** */
            /* ******************  2022-09-30  ************** */
            if (cualificador != ScriptDatosInterfaz.Cualificador_efimero)
            {
                //   1.2.1.) subTipoElementIntf = subTipoElemItf_evi_baseRefFractal  => Para los evis que muestran un concepto completo (procedente normalmente de una busqueda o la expansion de una referencia o una extraccion)
                //           En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto referenciado
                nodoDeInformacion = KDL_Raiz; // Como es un get details (una referencia), la raiz del KDL que hemos obtenido es la raiz del concepto
//                XmlNode KDL_Nodo_C = EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio.DocumentElement; // COmo lo vemos como un get details, la raiz del KDL que hemos obtenido es la raiz del concepto
//                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().desarrollaFractal(EviBase, this.gameObject, KdlConcepto, KDL_Nodo_C);
            }
            else
            {
                //                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().desarrollaFractal(EviBase, this.gameObject, KdlConcepto, nodo_E_eviFractal);
                nodoDeInformacion = nodo_E_ContEnBaseDeEvi.FirstChild;  // Convenia ponerlo mas lelegante atendiendo al nombre del nodo (PENDIENTE MAFG 2022-11-26)
                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log("Hola desde ScriptCtrlBaseDeEvi -  Desde domCargado - cargaDatosBaseDeEvi, intentamos cargar dato en un evi efimero con = " +
                    GetComponent<ScriptDatosElemenItf>().tipoElementIntf +
                    " - y con subTipoElementIntf : " + GetComponent<ScriptDatosElemenItf>().subTipoElementIntf +
                    "con nodo_E_ContEnBaseDeEvi.FirstChild.name " + nodo_E_ContEnBaseDeEvi.FirstChild.Name +
                    "con nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.name " + nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.Name +
                    "con nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.FirstChild.name " + nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.FirstChild.Name );
                }
            }

        }
        else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
        {
            //   1.2.2.) subTipoElementIntf = subTipoElemItf_evi_baseInstFractal  => Para los evis que muestran una instancia (procedente normalmente de la expansion de una referencia o una extraccion)
            //           En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto instanciado
                    // El nodod de interes es el asociado a la instancia que esta en el evi "EviTipo_RefFractal_01" hijo de "ContenedorDeEvi_01" hojo de esta base de evi
            nodoDeInformacion = nodo_E_ContEnBaseDeEvi.FirstChild;  // Debemos enviar el nodo instancia "A", no el enlace "E"
                                                                    // Convenia ponerlo mas lelegante atendiendo al nombre del nodo (PENDIENTE MAFG 2022-11-26)
        }
        else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
        {
            //   1.2.3.) subTipoElementIntf = subTipoElemItf_evi_baseSinTecho_00  => Para los evis que muestran un sin techo (procedente normalmente de la expansion de una referencia o una extraccion)
            //           En este caso la ayuda a interfaz muestra la ayuda a interfaz del concepto asociado al tipo de datos del sin techo
                    // El nodod de interes es el asociado al sin techo que esta en el evi "EviTipo_RefFractal_01" hijo de "ContenedorDeEvi_01" hojo de esta base de evi
            nodoDeInformacion = nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild;  // Debemos enviar el nodo sin techo "Z", no el enlace "E"
                                                                               // Convenia ponerlo mas lelegante atendiendo al nombre del nodo (PENDIENTE MAFG 2022-11-26)
        }
        else 
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Hola desde ScriptCtrlBaseDeEvi -  Desde domCargado - cargaDatosBaseDeEvi, intentamos cargar dato con i con tipoElementIntfsubTipoElementIntf desconociso = " +
                TipoElementIntf +
                " - y con subTipoElementIntf desconociso : " + subTipoElementIntf);
            }
            return;
        }


        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        // 2.) CArga de los datos correspondientes

        // Obtenemos lo datos basicos de ayuda a interfaz del DOM del concepto

        // Vamos con el identificador
        string[] identificadorConcepto_K_H_Q = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(domPropio, nodoDeInformacion);
        // Asignamos a las variables de este game object
        key = identificadorConcepto_K_H_Q[0];                            // key: es el key del concepto (K en KDL - esta en I en KDL)
        host = identificadorConcepto_K_H_Q[1];                         // host : es el host del concepto (H en KDL - esta en I en KDL)
        cualificador = identificadorConcepto_K_H_Q[2];                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)

        // Vamos con los datos de control de configuracion
        string[] ctrlCof_O_M = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameCtrlConfDeNodo(domPropio, nodoDeInformacion);
        ordinal = ctrlCof_O_M[0];                         // ordinal : es el ordinal del concepto (O en KDL - esta en F en KDL)
        fechUltMod = ctrlCof_O_M[1];                         // fechUltMod : es la fecha de ultima modificacion del concepto (M en KDL - esta en F en KDL)

        // Vamos con los datos de ayuda a interfaz
        string[] ayudaIntf = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameAyudaIntDeNodo(domPropio, nodoDeInformacion);
        idioma_AyuIntf = ayudaIntf[0];               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
        txt_nombre_AyuIntf = ayudaIntf[1];          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
        txt_descripcion_AyuIntf = ayudaIntf[2];     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
                                                    // Aqui incluimos el key y el host para poder identificar el Concepto si hiciera falta
        txt_descripcion_AyuIntf = txt_descripcion_AyuIntf + " - Key = " + key + " - Host = " + host + " - Cualificador = " + cualificador;
        txt_rotulo_AyuIntf = ayudaIntf[3];          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
        icono_AyuIntf = ayudaIntf[4];               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                    // TENGO QUE PEDIRLO FUERA
        audio_AyuIntf = ayudaIntf[5];               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                    // TENGO QUE PEDIRLO FUERA
        imagen_AyuIntf = ayudaIntf[6];               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL). Solo ha llegado el nombre del ficherp
                                                     // TENGO QUE PEDIRLO FUERA

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Hola desde cargaDatosBaseDeEvi Desde domCargado - 1 - txt_nombre_AyuIntf = " + txt_nombre_AyuIntf +
            " - 2 - key = " + key +
            " - 2 - host = " + host +
            " - 2 - txt_descripcion_AyuIntf = " + txt_descripcion_AyuIntf +
            " - 3 - txt_rotulo_AyuIntf = " + txt_rotulo_AyuIntf +
            " - 4 - icono_AyuIntf = " + icono_AyuIntf +
            " - 5 - audio_AyuIntf = " + audio_AyuIntf +
            " - 6 - imagen_AyuIntf = " + imagen_AyuIntf);
        }

        // ///////////////////////////////////////////////////////////////////////////////////
        // ///////////////////////////////////////////////////////////////////////////////////
        //  YA TENEMOS LOS DATOS O LAS REFERENCIAS DE LA INFORMACION DE AYUDA A INTEFAZ
        // Ponemos los datos de ayuda a interfaz en el gameObjet de la base del evi que contiene el concepto

        // ///////////////////////////////////////////////////////////////////////////////////
        //  Primero los que ya tenemos y no hay que ir a buscar fuera

        // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  "Script_BaseDeEvi_N1_Info"

        // ///////////////////////////////////////////////////////////////////////////////////
        //  Ahora los que tenemos el sitio donde buscarlos, pero que hay que bajar de internet
        //  Para obtenerlos de internet y cargarlos utilizaremos corrutinas definidas en esta clase (AHORA No usamos solicitudes )

        // Vamos a por la imagen de descripcion del concepto (la grande que aparece de fondo en el contenedor del concepto)
        string locImgAyuIntf = host + ScriptConexionDKS.sufijoAccesoAImagenesDks + imagen_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeTextura_imagen_AyuIntf(locImgAyuIntf));
        // En el update cargamos la textura donde corresponde cuando la corrutina ha terminado. Se chequea mediante "estadoImgAyudaIntf"

        // Vamos a por el icono del concepto
        string locIconoAyuIntf = host + ScriptConexionDKS.sufijoAccesoAIconosDks + icono_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeTextura_icono_AyuIntf(locIconoAyuIntf));
        // En el update cargamos la textura donde corresponde cuando la corrutina ha terminado. Se chequea mediante "estadoIconoAyudaIntf"

        // Vamos a por el audio del concepto
        string locAudioAyuIntf = host + ScriptConexionDKS.sufijoAccesoAAudiossDks + audio_AyuIntf;  // Localizacion del fichero enel DKS
        StartCoroutine(traeAudio__AyuIntf(locAudioAyuIntf));

        // *************************************
        // *************************************
        // Configuramos la base del evi conforme corresponda atendiendo a su naturaleza
        string miTipoDeEviBase = GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();

        if (miTipoDeEviBase ==  ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
        {
            // si es instancia ponemos los botones de instancia (azules)
            Btn_Evi_Maxi_Mini.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimizaInstancia;
            Btn_Evi_Desplazar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazarInstancia;
            Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesInstancia;
        }  // Fin de - if (miTipoDeEviBase ==  ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
        else if (miTipoDeEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) 
        {
            // si es sin techo ponemos los botones de sin techo (rojos)
            Btn_Evi_Maxi_Mini.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimizaSinTecho;
            Btn_Evi_Desplazar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazarSinTecho;
            Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesSinTecho;
        }
        //  else if (miTipoDeEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal) - Si es un evi de referencia, se queda con lo que esta
        // else  - Si no es ninguno de este tipo, se queda con lo que esta

        // El evi ya esta cargando sus datos básicos, por lo que lo indicamos en el estado
        estado_evi = estadoEvi_enCarga;

    }  // Fin de -  public void cargaDatosBaseDeEvi(GameObject solicitudAsociada)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cargaDatosBaseDeEvi_Especifico
    /// Observaciones : Este metodo carga los datos estandar el conceoto en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunis otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-06
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string eviTxt : es la informacion de entrada. Debia de ser un DOM (PENDIENTE MAFG 2021-02-07)
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         Esta funcion carga los datos de ayuda a interfaz y basicos de EVIS estandar de la interfaz.
    ///         Esto es EVIs como Buscador, En carga, Preubas, Evi por defecto u otros
    ///         Cuando un EVI contiene informacion que viene de fuera (EviFractal de getdetails, u otros)
    ///         el EVI primero se carga con unos datos por defecto (En carga), hasta que se obtiene la informacion de los dksS CORRESPONDIENTES
    ///         entonces se darga desde el Update de esta misma clase, atendiendo a los estados (en carga, Xmlrecibido, etc...
    ///         segun vaya llegando la informacion se va sobreescribiendo sobre la que hay por defecto en el EVI
    /// </summary>
    public void cargaDatosBaseDeEvi_Especifico()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosBaseDeEvi_Especifico donde tipoDeEvi es : " + GetComponent<ScriptDatosElemenItf>().dameTipoElementIntf()); }
        //        Debug.Log("Estoy en cargaDatosBaseDeEvi_Especifico donde eviTxt es : " + eviTxt);

        // CArgamos los datos del evi que no dependen del XML

        // ///////////////////////////////
        // Segun el tipo de EVI
        //      Buscador
        //      De concepto
        //          Fractal

        // /////////////////////////
        // Obtenemos del Daus los datos del concepto
        //  Los tipos de EVIs se describen en la cabecera de este fichero

        //if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == subTipoEvi_Base)
        //{
        //    cargaDatosEviBase();
        //    // El evi ya esta cargado y configurado. Lo ponemos operativo
        //    estado_evi = "operativo";
        //}
        //else if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == subTipoEvi_enCarga)
        //{ cargaDatosEviEnCarga(); }

        if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
        {
            cargaDatosEviBuscador_001();
            // El evi ya esta cargado y configurado. Lo ponemos operativo
            estado_evi = estadoEvi_baseLista;
        }
        else if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
        {
            cargaDatosEviTipo_Rama_00();
            // El evi ya esta cargado y configurado. Lo ponemos operativo
            estado_evi = estadoEvi_baseLista;
        }
        else if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_EviPrue_001)
        {
            cargaDatosEviPrue_001();
            // El evi ya esta cargado y configurado. Lo ponemos operativo
            estado_evi = estadoEvi_baseLista;

        }
        else if (GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
        {
            // Los datos ya estaran cargados, porque los cargo el elemento al que este EviRefElemen representa       
            // El evi ya esta cargado y configurado. Lo ponemos operativo
            estado_evi = estadoEvi_baseLista;

        }
        //       else if (tipoDeEvi == "Lista_001")
        //       { }
        else
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Tipo de EVI desconocido. Donde tipoDeEvi es : " + GetComponent<ScriptDatosElemenItf>().dameTipoElementIntf()); }
        }

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosBaseDeEvi_Especifico donde this.imagen_AyuIntf es : " + this.imagen_AyuIntf); }

        // ///////////////////////////////////////////////////
        // ///////////////////////////////////////////////////
        // //////   Como ya tenemos los datos de ayuda a interfaz, construimos y colocamos el gameobjet que los contiene
        // //////   Cargasmos datos etandar (especificos de cada EVI congreto) En sus contenedores (gameobjet) correspondientes

        // La imagen de descripcion del concepto (la grande que aparece de fondo en el contenedor del concepto) (el material generico lo cargamos en el metodo "awake")
        textura_imagen_AyuIntf = Resources.Load<Texture>(imagen_AyuIntf);  // Generamos la textura
        material_imagen_AyuIntf.SetTexture("_MainTex", textura_imagen_AyuIntf);  // Le asinamos la textura al material
//        this.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.GetComponent<MeshRenderer>().material = material_imagen_AyuIntf;
        this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.GetComponent<MeshRenderer>().material = material_imagen_AyuIntf;

        // El icono del concepto (el material generico lo cargamos en el metodo "awake")
        textura_icono_AyuIntf = Resources.Load<Texture>(icono_AyuIntf);  // Generamos la textura
        material_icono_AyuIntf.SetTexture("_MainTex", textura_icono_AyuIntf);  // Le asinamos la textura al material
//        this.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<MeshRenderer>().material = material_icono_AyuIntf;
        this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<MeshRenderer>().material = material_icono_AyuIntf;

        // Vamos con la imagen que acompaña al icono (el material generico lo cargamos en el metodo "awake")
        textura_ImgAyuIntfBaseDeEvi = Resources.Load<Texture>(imagen_AyuIntf);  // Generamos la textura
        material_ImgAyuIntfBaseDeEvi.SetTexture("_MainTex", textura_ImgAyuIntfBaseDeEvi);  // Le asinamos la textura al material
//        this.GetComponent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.GetComponent<MeshRenderer>().material = material_ImgAyuIntfBaseDeEvi;
        this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.GetComponent<MeshRenderer>().material = material_ImgAyuIntfBaseDeEvi;


        // El audio del concepto
        //Leemos el audio clip del directorio correspondiente en Resources
        AudioClip_AyuIntf = Resources.Load<AudioClip>(audio_AyuIntf);
                //Aplicamos el audio clip al audiosource
        this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<Script_BaseDeEvi_N1_Info>().audio_AyuIntf = AudioClip_AyuIntf;

        // Vamos con el texto de ayuda a interfaz
        // los textos de ayuda a interfaz los hacemos aparecen mediante el "OnTriggerEnter" del script  "Script_BaseDeEvi_N1_Info"


        // Las variables : tipoEvi; IdEnInterfaz; key; host; txt_nombre_AyuIntf; txt_rotulo_AyuIntf; txt_descripcion_AyuIntf; 
        // son variables de script que debe haber cargado "obtenerDatosEstandar()" y no hay que asignarlas a ningun componente

    }  // Fin de - public void cargaDatosBaseDeEvi_Especifico(string tipoDeEvi, string xmlDeEvi)


    // /////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////
    // /////////////////////////////////////////////////////////////////
    //  METODOS PARA CARGA DE EVIs ESPECIFICOS



    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : private void cargaDatosEviBase(string xmlDeEvi)
    /// Observaciones : Este metodo carga los datos estandar para una caja de concpto base (sin concepto concreto)en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-21
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// </summary>
    private void cargaDatosEviBase()
    {

        key = "0";
        host = "host por defecto";
        imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/sinFoto";
        icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/iconoImgPorDefecto";
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudioPorDefecto";
        txt_nombre_AyuIntf = "txt_nombre_AyuIntf por defecto";
        // txt_nombre_AyuIntf = "ayuda a interfaz: nombre";
        txt_rotulo_AyuIntf = "txt_rotulo_AyuIntf poe defecto";
        txt_descripcion_AyuIntf = "txt_descripcion_AyuIntf por defecto" + " -  numero de evi = " + GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo();

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosEviBase 222 donde this.imagen_AyuIntf es : " + imagen_AyuIntf); }

    }  // Fin de - private void cargaDatosEviBase(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : private void cargaDatosEviBase(string xmlDeEvi)
    /// Observaciones : Este metodo carga los datos estandar para una caja de concpto base (sin concepto concreto)en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-21
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// </summary>
    private void cargaDatosEviEnCarga()
    {

        key = "0";
        host = "host por defecto";
        imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/sinFoto";
        icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/iconoImgPorDefecto";
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudioPorDefecto";
        //        txt_nombre_AyuIntf = "txt_nombre_AyuIntf por defecto";
        txt_nombre_AyuIntf = "ayuda a interfaz: en carga";
        txt_rotulo_AyuIntf = "rotulo en carga";
        txt_descripcion_AyuIntf = "elemento en carga" + " -  numero de evi = " + GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo();

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosEviEnCarga 222 donde this.imagen_AyuIntf es : " + imagen_AyuIntf); }

    }  // Fin de - private void cargaDatosEviBase(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : private void cargaDatosEviBuscador_001(string xmlDeEvi)
    /// Observaciones : Este metodo carga los datos estandar para una caja de concpto Buscador_001 (evi de interfaz de busqueda)en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-21
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// </summary>
    private void cargaDatosEviBuscador_001()
    {

        key = "0";
        host = "host por defecto";
        imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/imagen_Buscador";
        icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/IconoImgBusqueda";
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudioBuscador";
        //        txt_nombre_AyuIntf = "txt_nombre_AyuIntf por defecto";
        txt_nombre_AyuIntf = "Buscador 001";
        txt_rotulo_AyuIntf = "Buscador rotulo";
        txt_descripcion_AyuIntf = "EVI para facilitar la búsqueda al usuario"+" -  numero de evi = "+ GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo();

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosEviBuscador_001 333 donde this.imagen_AyuIntf es : " + imagen_AyuIntf); }

    }  // Fin de - private void cargaDatosEviBuscador_001(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : private void cargaDatosEviTipo_Rama_00()
    /// Observaciones : Este metodo carga los datos estandar para una caja de Rama (evi de interfaz de gestion de rama)en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-04
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// </summary>
    private void cargaDatosEviTipo_Rama_00()
    {

        key = "0";
        host = "host por defecto";
        imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesKlw + "/imagen_arbol";
        icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosKlw + "/iconoExpandir";
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosKlw + "/IconoAudio_esp_gestioRama";
        //        txt_nombre_AyuIntf = "txt_nombre_AyuIntf por defecto";
        txt_nombre_AyuIntf = "Acceso a rama";
        txt_rotulo_AyuIntf = "Acceso a rama rotulo";
        txt_descripcion_AyuIntf = "EVI para acceder a una rama" + " -  numero de evi = " + GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo();

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosEviTipo_Rama_00 donde this.imagen_AyuIntf es : " + imagen_AyuIntf); }

    }  // Fin de - private void cargaDatosEviBuscador_001(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : private void cargaDatosEviPrue_001(string xmlDeEvi)
    /// Observaciones : Este metodo carga los datos estandar para una caja de concepto EviPrue_001 (evi de interfaz de busqueda)en las propiedades correspondientes de este objetoo
    ///         Los datos estandar son los datos de ayuda a interfaz y algunos otros como el identificador del evi en la 
    ///         entidad, el key y host del conceto que contiene, etc...
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-12-21
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones :
    ///     Por ahora es una funcion prototipo provisional, en la que se ponen los datos a pelo y aleatoriamente (2020-12-23)
    /// </summary>
    private void cargaDatosEviPrue_001()
    {

        key = "0";
        host = "host por defecto";
        imagen_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locImagenesExternos + "/imagen_gen_casa";
        icono_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locIconosExternos + "/IconoImgCasa";
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locAudiosExternos + "/IconoAudioCasa";
        //        txt_nombre_AyuIntf = "txt_nombre_AyuIntf por defecto";
        txt_nombre_AyuIntf = "fractal 001. Prototipo de pruebas";
        txt_rotulo_AyuIntf = "fractal 001. Prototipo de pruebas";
        txt_descripcion_AyuIntf = "EVI fractal 001 prototipo provisional. " + " -  numero de evi = " + GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo();

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en cargaDatosEviPrue_001 444 donde this.imagen_AyuIntf es : " + imagen_AyuIntf); }

    }  // Fin de - private void cargaDatosEviPrue_001(string xmlDeEvi)


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////  
    /// ///////////  reemplaza_aspecto_AyuIntf() : esta funcion cambia las imagenes iconos y audios de la ayuda a interfaz de un evi
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-11
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string locImgAyuIntf
    ///         - string locIconoAyuIntf
    ///         - string locAudioAyuIntf
    /// Variables de salida :
    ///     No devuelve nada, solo reemplaza el aspecto de la ayuda a interfaz
    /// Observaciones:
    ///     - OJOOO, los datos de los sistentos scripts asociados al EVI deben cambiarse por otro lado y anteriormente a la llamada de este metodo.
    ///     aqui solo se cambian las texturas, audios, etc para que sean visualizadas en el evi, no los valores de los parametros que contienen sta informacion
    /// </summary>
    public void reemplaza_aspecto_AyuIntf(string este_locImgAyuIntf, string este_locIconoAyuIntf, string este_locAudioAyuIntf)
    {
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Estoy en ScriptCtrlBaseDeEvi => reemplaza_aspecto_AyuIntf : " +
              "\n - Con este_locImgAyuIntf : " + este_locImgAyuIntf +
              "\n - Con este_locIconoAyuIntf : " + este_locIconoAyuIntf +
              "\n - Con este_locAudioAyuIntf : " + este_locAudioAyuIntf +
              "\n - Con name : " + this.name
              );
        }

        StartCoroutine(traeTextura_imagen_AyuIntf(este_locImgAyuIntf));
        StartCoroutine(traeTextura_icono_AyuIntf(este_locIconoAyuIntf));
        StartCoroutine(traeAudio__AyuIntf(este_locAudioAyuIntf));

        // el contenedor que gestiona la carga de los recursos de ayuda a interfaz del concepto es:
        //      "EviBase_de_sinTecho" => "ContenedorDeEvi_01" => "EviTipo_Fractal_01"
        GameObject ContenedorDeEvi_01 = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(this.gameObject, "ContenedorDeEvi_01");
        GameObject EviTipo_Fractal_01 = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(ContenedorDeEvi_01, "EviTipo_Fractal_01");
        GameObject EviTipo_RF01_Fractum = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(EviTipo_Fractal_01, "EviTipo_RF01_FractumRef");  // OJO el nombre del gameobjet no es el mismo que su tag (o lo corrijo por si se lia)

        // Actibamos los gameobjet, porque si no no arrancan los metodos asociados a estos. Ademas me viene bien que aparezcan en pantalla para que 
        // el usuario sepa que ha cambiado el tipo de dato
        ContenedorDeEvi_01.SetActive(true);
        EviTipo_Fractal_01.SetActive(true);
        EviTipo_RF01_Fractum.SetActive(true);

        EviTipo_RF01_Fractum.GetComponent<SctCtrlEviTipo_01Fractum>().reemplaza_aspecto_AyuIntf_fractum(este_locImgAyuIntf, este_locIconoAyuIntf, este_locAudioAyuIntf); 

    }  // Fin de - public void reemplaza_aspecto_AyuIntf(string este_locImgAyuIntf, string este_locIconoAyuIntf, string este_locAudioAyuIntf)



    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public Vector3 calculaPosicionEvi()
    ///     Calcula la posicion del evi, dependiendo de en que elemento este ahijado
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2020-06-16
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     - Vector3 posicionEvi : 
    /// Observaciones :
    ///     - Hay que tener en cuenta que como el evi puede ser hijo de distintos elementos de interfaz (muro, tramoya,...), la posicion de este
    ///     debe calcularse para que se ajuste como proceda a la escala del elemento que lo contiene (su padre)
    ///     - Conviene reseñar tambien nosrmalmente los evis se arrastran siguiendo algun puntero ( de usuario, de tramoya,...) y estos punteros
    ///     son hijos del usuario y del puntero de usuario, por lo que estos se mueven en las coordenadas de su padre (elemento que los contiene, generalmente usuario)
    ///     por lo que para ajustar el movimiento del evi (en la escala del muro, tramoya,...) al movimiento del puntero (en la escala del usuario,..) hay que hacer 
    ///     los ajustes de ejes de coordenadas correspondientes (se pueden ver en el codigo de esta funcion)
    /// </summary>
    public Vector3 calculaPosicionEvi()
    {
        Vector3 posicionEvi;

        // Para arrastrar el evi, CUANDO ESTA EN EL MURO DE TRABAJO ACTIVO, con el puntero del usuario "PunteroUsuario", lo que hacemos es colocar el evi en la posición donde esta el puntero
        // Hay que tener en cuenta :
        //      - El puntero es hijo del usuario (el usuario es escala (1,1,1) por lo que su hijo se mueve en una escala (1,1,1)
        //          (escala_x_Usuario, escala_y_Usuario, escala_z_Usuario) no se definen en ScriptDatosInterfaz por lo que es (1,1,1))
        //      - El Evi es hijo del muro, la escala de muro se define mediante  escalaMuroTrabajo = (escala_x_MuroTrabajo, escala_y_MuroTrabajo, escala_z_MuroTrabajo) se 
        //          definen en funcion de "escalaGeneralMuroTrabajo" en "ScriptDatosInterfaz"  por lo que una unidad en el usuario son "escalaGeneralMuroTrabajo" unidades
        //          en el muro por lo que hay que dividir las coordenadas del puntero por este valor para escalar correctamente las coordenadas
        Vector3 posicionDePunteroUsuario = estePunteroUsuario.transform.localPosition;
        pos_x = (posicionDePunteroUsuario.x / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_MuroTrabajo); // es - porque el boton de desplazar esta abajo a la derecha
        pos_y = (posicionDePunteroUsuario.y / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_MuroTrabajo);
        pos_z = 0f;

        // Si el puntero activo es el "PunteroTramoya" y 
        // SI el abuelo del evi base es la tramoya (evi base - hijo de algun telon - hijo de la tramoya)
        // entonces ajustamos el movimiento del evi para que funcione dentro de la tramoya
        if ((ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroTramoya.GetComponent<ClassPuntero>().estadoPuntero == ClassPuntero.estPuntero_activo) & 
            (transform.parent.transform.parent.name == Tramoya.name))
            {
                // Para arrastrar el evi dentro de la tramoya, lo que hacemos es colocar el evi ajustando su posicion en la tramoya, del mismo modo que hicimos con el puntero de tramoya
                // Hay que tener en cuenta :
                //      - El puntero de tramoya es hijo del puntero de usuario que es hijo del usuario. Por lo que al ajustarlo en "ScriptCtrlPuntTramoya" teniamos que ajustar su posicion
                //          referenciada al puntero de usuario. El Evi "EviRefElemen" CUANDO ESTA EN LA TRAMOYA, es hijo de algun telon, que será hijo de la tramoya, por lo que su posicion hay
                //          que referenciarla al telos (por esto el calculo es distinto que el del puntero de tramoya)
                //      - Para realizar el calculo del evi "EviRefElemen" CUANDO ESTA EN LA TRAMOYA, simplemente lo hacemos por triangulacion.
                //              - EL pntero de usuario esta en el muro de usuario
                //              - La tramoya esta entre el muro de usuario (puntero de usuario) y la camara y se ajusta al cono de esta, por lo que como son dos triangulos rectangulos (el del puntero 
                //                      de usuario con la camara y el de el evi "EviRefElemen" que esta en la tramoya con la camara), el cateto (puntero usuario), dividido por la distancia del usuario
                //                      a la camara, debe ser igual al cateto equivalente (posicion del "EviRefElemen" que esta en la tramoya) dividido por la distancia de la tramoya a la camara.
                //                      Y de ahi despejamos el cateto equivalente (posicion del "EviRefElemen" que esta en la tramoya)
                //      -  Hay que aplicar tambien el ajuste debido a la escala de la tramoya "escalaTramoya"
                //      -  Hay que tener en cuenta tambien que la tramoya ocupa solo parte de el espacio y esta colocada en la parte superior, por lo que hay que introducir una correccion en el 
                //          eje Y : - (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTramoya.y * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_Tramoya));

            float PU_x = posicionDePunteroUsuario.x;
            float PU_y = posicionDePunteroUsuario.y;
            float DC = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario;
            float DTC = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario - ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_PuntTramoya; // Distancia de la camara a la tramoya
                                                                                                                                                                               // realizamos el calculo. Lo dividimos por la escala del puntero de usuario, (que es su padre), para ajustar la escala del calculo
            pos_x = ((PU_x * DTC) / DC) * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_Tramoya);
            pos_y = ((PU_y * DTC) / DC) * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_Tramoya);
            pos_y = pos_y - (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTramoya.y * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_Tramoya));

        }  // FIn de -  if ((ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroTramoya.GetComponent<ClassPuntero>().estadoPuntero == ScriptDatosInterfaz.estPuntero_activo) & 

        // Hay que tener en cuenta :
        //      - Para que el puntero se coloque en el centro del boton de desplazar, en lugar de colocarse en el centro del objeto "BaseDeEvi_01", tenemos que 
        //          sumar en x "posicionAnchoBaseBotones" y en y "posicionAltoBaseBotones". Como estos estan dentro del EVI base, hay que corregir con
        //          el factor de escala correspondiente "factorEscalaBaseDeEvi"
        pos_x = pos_x - posicionAnchoBaseBotones * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi; // es - porque el boton de desplazar esta abajo a la derecha
        pos_y = pos_y + posicionAltoBaseBotones * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;

        //  Por fin, definimos la posicion del evi base
        posicionEvi = new Vector3(pos_x, pos_y, pos_z);

        return posicionEvi;

    }  // Fin de - private void cargaDatosEviPrue_001(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : edicion_salir
    /// Observaciones : Este metodo saca el evi de modo edicion y lo pone en modo navegacion. Borra la rama de edicion (esta se pierde y no puede recuerarse)
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-08-10
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo saca el evi de modo edicion
    /// Observaciones :
    ///     - La salida del modo de edicion depende de el contenido del evi como sigue
    ///           0.) Anteriormente ya se ha consltado al usuario si realmente quiere salir y ha dicho que SI
    ///           1.)  Eliminamos la rama en edicion
    ///           2.)  Sacamos el concepto de modo edicion
    /// </summary>
    private void edicion_salir()
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en edicion_salir 1320"); }

        // 1.)  Eliminamos la rama en edicion
        GameObject ramaAEliminar = null;
            // Miramos cual es su rama de edicion: forEach
            // Buscamos una rama hija que este en modo edicion de este evi base.
            //  OJOO la lista de hijos esta en el EVI base, donde estamos
        if (transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama) & (hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)) 
                {
                    // si es rama, es la rama asociada a la expansion del evi (OJOOO ESTO IMPLICA QUE CADA EVI, SOLO PUEDE TENER UNA RAMA ASOCIADA)                               
                    ramaAEliminar = hijo;
                } // FIn de - 
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)

            // Ya sabemos cual es la rama en edicion a borrar. 
                // Primero la borramos de la lista de hijos del evi que estaba en edicion
        transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Remove(ramaAEliminar);
                // Ahora  la eliminamos
        ramaAEliminar.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();

        // 2.)  Sacamos el concepto de modo edicion
        transform.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_navegacion;

    }  // Fin de - private void cargaDatosEviPrue_001(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : edicion_grabar
    ///         Este metodo graba en el DKS el concepto en edicion enviando el KDL que corresponde al estado de la edicion
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-08-10
    /// Ultima modificacion : 
    ///         - 2021-10-17. He estado trabajando la visualizacion y edicion de los sin techo, Cosa qu me ha llevado bastante trabajo
    ///         Continuo ahora con la generacion del KDL de solicitud de alta de concepto
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, CUANDO LO PROGRAME DEBE DEVOLVER UNA REFERENCIA AL CONCEPTO QUE SE HA DADO DE ALTA
    /// Observaciones :
    ///      - Ya se ha preguntado al usuario si realmente desea grabar el concepto en edicion (y ha contestado que si)
    ///      - Para grabar el concepto (evi) que esta en edicion, damos los pasos siguientes:
    ///      1.) Construimos con esta descripcion del concepto, la solicitud de alta de nuevo concepto al DKS pertinente
    ///         1.1.) Generamos el concepto
    ///         1.2.) Definimos el espacio de nombres
    ///      2.) Añadimos el nodo identificador (para el concepto SOLICITUD DE ALTA DE NUEVO CONCEPTO)
    ///             generamos los datos del identificador del concepto efimero para solicitid de alta de concepto que vamos 
    ///             a generar (Ver ScriptDatosInterfaz => IDENTIFICACION CONCEPTO GENERRADO POR ESTE KEE)
    ///      3.) Añadimos el nodo de control de configuracion      
    ///      4.) Generamos el nodo (D) raiz de este concepto. Donde ira la descripcion de este. Esto es una instancia "gen_solicitudADks" que contendra una 
    ///             instancia de "gen_solicitud_altaConcepto" en cuya descripcion (D:gen_solicitud_altaConcepto), pondremos la descripcion del concepto a dar de 
    ///             alta en el DKS
    ///      5.) Añadimos  la instancia de gen_solicitud_altaConcepto (ella se encarga de instanciar a gen_solicitudADks) y nos devuelve el 
    ///             elemento (D:gen_D => D:gen_solicitud_altaConcepto), donde debemos insertar la descripcion del concepto a dar de alta en el DKS
    ///      6.) Obetenemos la descripcion del concepto a dar de alta (que esta definida en la rama de edicion del concepto editado en la interfaz KEE) y la ingertamos 
    ///             como descripcion de la instancia "(D:gen_D => D:gen_solicitud_altaConcepto"       
    ///             - OJOO, es el DKS quien proporciona los elementos I y F del nuevo concepto que se va a dar de alta en el DKS
    ///      
    ///      7.) Enviamos la solicitud al DKS y quedamos a la espera de recibir la respuesta
    ///      
    ///      8.) Enviamos al DKS los ficheros que se hayan asociado a datos Sin Techo en el proceso de edicion
    ///      
    ///      9.) La respuesta de la solicitud que enviamos al DKS debe ser :
    ///          9.1.)
    ///              - Un error. En este caso mostramos el concepto de error en el muro donde esta el evi raiz de la edcion que nos ocupa
    ///          9.2.)
    ///              - Si el alta ha finalizado con exito. EL DKS respondera con un concepto respuesta en el que indicara que todo esta OK y
    ///            llevará tambien una referencia al concepto que terminamos de dar de alta. Mostrando el concepto recibido
    ///            indicamos que el alta ha finalizado con exito. El evi de respuesta debe llevar una referencia al concepto recien dado 
    ///            de alta, donde el usuario puede pinchar, para solicitar el concepto y visualizarlo en la interfaz KEE
    /// </summary>
    private void edicion_grabar()
    {
        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => edicion_grabar 1320"); }

        XmlElement KDL_C_elemento;
        XmlElement KDL_D_raiz;  // es el nodo D inicial de este concepto efimero que es el que vamos a generar para realizar la solicitud de alta
        XmlElement KDL_D_para_concepto_a_dar_de_alta; // es el elemento D que cuelga de la instancia "", osea, el nodo donde debemos poner la 
                                                      // descripcion del concepto que vamos a dar de alta (la que obtendremos de la rama de edicion del concepto editado en la interfaz de KEE)

        XmlDocument kdlSolAltaNuevoConcepto = new XmlDocument();

        List<ClassFichero> FicherosAGrabar = new List<ClassFichero>(); // Si soy un sin techo tendre un fichero asociado, pero si soy otro tipo de EVI y me han editado, lo normal es que de entre
                                                                       // los Sin Techo que contengo, algunos de ellos hayan modificado su fichero asociado. Si ha sido asi, los ficheros asociados que 
                                                                       // hay que subir deben estar anotados en esta lista de objetos, por lo que en la operacion de alta, despues de mandar el 
                                                                       // KDL de solicitud, tendre que revisar esta lista y enviar al DKS los ficheros que encuentre en ella, para que queden 
                                                                       // almacenados en el DKS y relacionados como datos de los elementos Sin Techo que corresponda

        // 1.) Construimos con esta descripcion del concepto, la solicitud de alta de nuevo concepto al DKS pertinente
        // 1.1.) Generamos el concepto
        kdlSolAltaNuevoConcepto.LoadXml(ScriptLibConceptosXml.BaseConceptoKDL);
        KDL_C_elemento = kdlSolAltaNuevoConcepto.DocumentElement;

            // 1.2.) Definimos el espacio de nombres
        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(kdlSolAltaNuevoConcepto.NameTable);
        manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);

        // 2.) Añadimos el nodo identificador (para el concepto SOLICITUD DE ALTA DE NUEVO CONCEPTO)
        // generamos los datos del identificador del concepto efimero para solicitid de alta de concepto que vamos a generar (Ver ScriptDatosInterfaz => IDENTIFICACION CONCEPTO GENERRADO POR ESTE KEE)
        int num_concepto = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().dameNuevo_numDeConceptosGenerados();
        string key_conceptoConsulta = num_concepto.ToString();
        string host_conceptoConsulta = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().host_efimero_de_interfaz;
        string cualificador_conceptoConsulta = "1";  // 0 es almacenado. 1 es efimero (ver descripcion de esquema KDL)
        string resultIngerIdentificador;
        resultIngerIdentificador = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingertaIdentificadorDeNodo(kdlSolAltaNuevoConcepto, KDL_C_elemento, key_conceptoConsulta, host_conceptoConsulta, cualificador_conceptoConsulta);

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_grabar _ despues del identificador de concepto. Con . resultIngerIdentificador : " + resultIngerIdentificador);
            string Xml_en_string = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameStringDeXmlDocument(kdlSolAltaNuevoConcepto);
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_grabar _ despues del identificador de concepto. Con . Xml_en_string : " + Xml_en_string);
        }
         
        // 3.) Añadimos el nodo de control de configuracion
        string resultIngerCtrlConfig;
        DateTime dateActual = DateTime.Now;
        double dateActual_ms = dateActual.Millisecond;
        string ordinal = "0";  // es un efimero y por tanto, normalmente el primero y ultimo de su estirpe
        string ultimaModificacion = dateActual_ms.ToString();

        resultIngerCtrlConfig = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingertaCtrlConfDeNodo(kdlSolAltaNuevoConcepto, KDL_C_elemento, ordinal, ultimaModificacion);

        // 4.) Generamos el nodo (D) raiz de este concepto. Donde ira la descripcion de este. Esto es una instancia "gen_solicitudADks" que contendra una 
        //      instancia de "gen_solicitud_altaConcepto" en cuya descripcion (D:gen_solicitud_altaConcepto), pondremos la descripcion del concepto a dar de 
        //      alta en el DKS
        KDL_D_raiz = kdlSolAltaNuevoConcepto.CreateElement(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.letra_Descripcion, ScriptLibConceptosXml.DnsKdl);
        KDL_C_elemento.AppendChild(KDL_D_raiz);


        // //////////////////////
        // 5.) Añadimos  la instancia de gen_solicitud_altaConcepto (ella se encarga de instanciar a gen_solicitudADks) y nos devuelve el elemento (D:gen_solicitud_altaConcepto => E => A => "gen_D" => D), 
        //      donde debemos insertar la descripcion del concepto a dar de alta en el DKS

        KDL_D_para_concepto_a_dar_de_alta = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingerta_solicitud_altaConcepto(kdlSolAltaNuevoConcepto, KDL_D_raiz);

        if (DatosGlobal.niveDebug > 50)
        {
            string Xml_en_string = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameStringDeXmlDocument(kdlSolAltaNuevoConcepto);
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_grabar - desspues de ingerta_solicitud_altaConcepto. con Xml_en_string : " + Xml_en_string);
        }

        // /////////////////////
        // 6.) Obetenemos la descripcion del concepto a dar de altaa (que esta definida en la rama de edicion del concepto editado en la interfaz KEE) y la ingertamos 
        //      como descripcion de la instancia "gen_solicitud_altaConcepto"
        //     - OJOO, es el DKS quien proporciona los elementos I y F del nuevo concepto que se va a dar de alta en el DKS


        string resultadoDelIngertoDescripcion = "resultadoDelIngertoDescripcion por defecto";

        try
        {
            resultadoDelIngertoDescripcion = edicion_ingertaDescripcion(kdlSolAltaNuevoConcepto, KDL_D_para_concepto_a_dar_de_alta, this.gameObject, FicherosAGrabar);
        }
//        catch (System.IO.FileNotFoundException)
        catch
        {
            if (DatosGlobal.niveDebug > 50)
            { Debug.Log("Error al generar el DOM de EDICION desde edicion_grabar - con resultadoDelIngertoDescripcion = " + resultadoDelIngertoDescripcion); }

            // PENDIENTE MAFG 2022-01-02 OJOO por ahora solo llamamos a un concepto error del DKS. HAY que generar un evi de erros, qe tenga informacion del error que se produce en la interfaz KEE

            // Generamos los parametros globales para generar los evis en el muro activo
            string cualificador_DeError = "0";
            string ordinalConf_DeError = "0";
            DateTime ultiModConf_DeError = new DateTime(0);
            // Para obtener el objeto de telon donde van a ir los evis en pruebas
            // OJOOOO esto puede hacerse mientras no tengamos mas que un telon. POr ahora lo hacemos asi para simplificar (PENDIENTE MAFG 2022-01-01)
            GameObject elemDestino_DeError = GetComponent<ScriptDatosInterfaz>().muro_Activo; ;  // Los evis iran a la tramoya 

            // Vamos generando cada evi de los que nos hacen falta

            // Generamos un evi de "gen_recAyuIntf" de "dks_klw"
            string key_DeError = "gen_errorKLW";
            string host_DeError = ConceptosConocidos.gen_dks_klw_host;
            ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeError, host_DeError, cualificador_DeError, ordinalConf_DeError, ultiModConf_DeError, elemDestino_DeError);

        }  // Fin de -  catch (System.IO.FileNotFoundException)


        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_grabar - Con el concepto de solicitud de alta a DKS completo. Con resultadoDelIngertoDescripcion : " + resultadoDelIngertoDescripcion);
            string Xml_en_string = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameStringDeXmlDocument(kdlSolAltaNuevoConcepto);
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_grabar - Con el concepto de solicitud de alta a DKS completo. con Xml_en_string : " + Xml_en_string);
        }

        //  7.) Enviamos la solicitud al DKS y quedamos a la espera de recibir la respuesta
        enviaSolicitudADks(kdlSolAltaNuevoConcepto);

        // 8.) Enviamos al DKS los ficheros que se hayan asociado a datos Sin Techo en el proceso de edicion
        enviaFicherosADks(FicherosAGrabar);

        ///      9.) La respuesta de la solicitud que enviamos al DKS debe ser :
        ///          9.1.)
        ///              - Un error. En este caso mostramos el concepto de error en el muro donde esta el evi raiz de la edcion que nos ocupa
        ///          9.2.)
        ///              - Si el alta ha finalizado con exito. EL DKS respondera con un concepto respuesta en el que indicara que todo esta OK y
        ///            llevará tambien una referencia al concepto que terminamos de dar de alta. Mostrando el concepto recibido
        ///            indicamos que el alta ha finalizado con exito. El evi de respuesta debe llevar una referencia al concepto recien dado 
        ///            de alta, donde el usuario puede pinchar, para solicitar el concepto y visualizarlo en la interfaz KEE
        ///         

    }  // Fin de - private void cargaDatosEviPrue_001(string xmlDeEvi)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : edicion_ingertaDescripcion
    ///         Este metodo inserta la descripcion de un concepto en edicion (atendiendo al contenido de su rama de edicion), normlmente con el fin
    ///         de anexarlo a una solicitud KDL  de alta o modificacion de un concepto en un DKS para grabar un concepto nuevo o modificar uno ya exisatente
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-08-17
    /// Ultima modificacion :
    ///         - 2021-08-30. Hasta ahora solo tenia un prototipo. Me pongo ahora a programarla en serio
    ///         - 2021-10-22. He arreglado la edicion de ls SinTecho. La ajusto para integrar los cambios realizados
    /// Variables de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general de solicitud al DKS
    ///      - XmlElement elemento_D_recibido : Nodo D en que que tenemos que realizar el ingerto
    ///      - GameObject eviQueDescribe : es el gameobject del evi del cual vamos a ingertar la descripcion "elemento_D_recibido" en el KDL que nos mandan (domKDL)
    /// Variables de salida :
    ///     resultadoDelIngertoDescripcion : Solo es un string que indica si el proceso se ha realizado correctamente
    /// Observaciones :
    ///      - Ya se ha preguntado al usuario si realmente desea grabar el concepto en edicion (y ha contestado que si)
    ///      - OJOOO Quien la llama DEBE ATENDER LA EXCEPCION : throw new ArgumentException("ERROR. No es posoible ingertar un elemento instancia (E=>A) fuera de un elemento descripcion (D)");
    ///      
    ///      - Para grabar el concepto (evi) que esta en edicion, damos los pasos siguientes:
    ///         1.) Controlamos que el nodo que nos han pasado es un elemento "D", ya que la descripcion del concepto debe ir en un elemento de este tipo. Si no es asi, devolvemos un error
    ///         2.) Controlamos que el EVI esta en modo edicion. Si no es asi, devolvemos un error
    ///         3.) EN EL EVI BASE. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" del EVI, una rama que este en modo edicion. Debe haber una y solo una
    ///         4.) EN LA RAMA BASE DE EDICION. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" un muro que este en modo edicion. Debe haber uno y solo uno
    ///         5.) Ya tenemos el muro de la rama base de edicion.
    ///                 EN EL MURO DE LA RAMA BASE DE EDICION. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" que configuran el primer nivel de descripcion del concepto que 
    ///             estamos editando. Estos seran o REFERENCIAS, o SINTECHO o INSTANCIAS. Los iremos recorriendo todos y segun su naturaleza actuamos como sigue:
    ///             
    ///             5.1.) Para el caso de un evi SIN TECHO (subTipoElementIntf = subTipoElemItf_evi_baseSinTecho_00):
    ///                     Ingertamos el elemento sin techoòndiente(E => Z) mediante "ingerta_sinTecho"
    ///                     - OJO recordamos que los elementos Sin Techo NO tienen descripcion, por lo que esta rama muere aqui
    ///                     
    ///             5.2.) Para el caso de un evi REFERENCIA (subTipoElementIntf = subTipoElemItf_evi_baseRefFractal):
    ///                     Ingertamos el elemento referencia correspòndiente(E => R) mediante "ingertaElementoReferencia"
    ///                     - OJO si el sin techo esta asociado a un fichero hay que gestionar (segun proceda o no) el envio de este fichero al DKS
    ///                     - OJO recordamos que la referencia NO tiene descripcion (salvo en el DKS de origen), por lo que esta rama muere aqui
    ///                     
    ///             5.3.) Para el caso de un evi INSTANCIA (subTipoElementIntf = subTipoElemItf_evi_baseInstFractal)
    ///                     Ingertamos el elemento instancia correspòndiente(E => A) mediante "ingertaElementoInstancia"
    ///                     - OJO recordamos que los elementos instancia SI tienen descripcion, por lo que esta rama debe ingertar la descripcion de la instancia
    ///                     recursivamente hasta recorrer toda la rama de descripcion de la instancia
    ///         
    ///         6.) Si todo ha ido bien, devolvemos OK
    ///         
    /// </summary>
    public string edicion_ingertaDescripcion(XmlDocument domKDL, XmlElement elemento_D_recibido, GameObject eviQueDescribe, List<ClassFichero> FicherosAGrabar)
    {
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - Entro en el metodo ");
        }
        string resultadoDelIngertoDescripcion = "resultadoDelIngertoDescripcion por defecto";

        GameObject ramaBaseDeEdicion = null;
        GameObject muroDeRamaBaseDeEdicion = null;
        
        // 1.) Controlamos que el nodo que nos han pasado es un elemento "D", ya que la descripcion del concepto debe ir en un elemento de eswte tipo. Si no es asi, devolvemos un error
        bool esAdecuado = elemento_D_recibido.Name == ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + "D";

        // 2.) Controlamos que el EVI esta en modo edicion (tambien nos vale el modo cabeza de edicion). Si no es asi, devolvemos un error
        esAdecuado = (esAdecuado & 
            ((GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion) || (GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_cabezaEdicion)));
        if (!esAdecuado)
        {
            resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion el ingerto no ha podido realizarse, el nodo madre, no es D. EL nodo recibido es elemento_D_recibido.Name = "
                 + elemento_D_recibido.Name + 
                 " - de tipo = " + elemento_D_recibido.NodeType +
                 " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo;

            if (DatosGlobal.niveDebug > 50)
            {
                Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - NO ES ADCUADO. - con resultadoDelIngertoDescripcion : " + resultadoDelIngertoDescripcion);
            }

            throw new ArgumentException(resultadoDelIngertoDescripcion);
        }

        // 3.) EN EL EVI BASE. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" del EVI, una rama que este en modo edicion. Debe haber una y solo una
        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos alguna solicitud pendiente
        {
            int numeroDeRamasBaseDeEdicion = 0;
            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama) & 
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    ramaBaseDeEdicion = hijo.gameObject;
                    numeroDeRamasBaseDeEdicion++;
                }
            }  // FIn de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            if (numeroDeRamasBaseDeEdicion != 1)
            {
                resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion el EVI no posee una y solo una rama de edicion. EL nodo recibido es elemento_D_recibido.Name = "
                     + elemento_D_recibido.Name +
                     " - de tipo = " + elemento_D_recibido.NodeType +
                     " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo +
                     " - numeroDeRamasBaseDeEdicion = " + numeroDeRamasBaseDeEdicion;

                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - ERROR en ramas de edicion con numeroDeRamasBaseDeEdicion = " + numeroDeRamasBaseDeEdicion + ". - con resultadoDelIngertoDescripcion : " + resultadoDelIngertoDescripcion);
                }
                throw new ArgumentException(resultadoDelIngertoDescripcion);
            }
        }  // FIn de - if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)

        // 4.) EN LA RAMA BASE DE EDICION. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" un muro que este en modo edicion. Debe haber uno y solo uno
        if (ramaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos alguna solicitud pendiente
        {
            int numeroDeMurosEnRamasBaseDeEdicion = 0;
            foreach (GameObject hijo in ramaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro) &
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    muroDeRamaBaseDeEdicion = hijo.gameObject;
                    numeroDeMurosEnRamasBaseDeEdicion++;
                }
            }  // FIn de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            if (numeroDeMurosEnRamasBaseDeEdicion != 1)
            {
                resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion la rama base no posee uno y solo un muro en edicion. EL nodo recibido es elemento_D_recibido.Name = "
                     + elemento_D_recibido.Name +
                     " - de tipo = " + elemento_D_recibido.NodeType +
                     " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo +
                     " - numeroDeMurosEnRamasBaseDeEdicion = " + numeroDeMurosEnRamasBaseDeEdicion;

                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - ERROR en muros de edicion con numeroDeMurosEnRamasBaseDeEdicion = " + numeroDeMurosEnRamasBaseDeEdicion + ". - con resultadoDelIngertoDescripcion : " + resultadoDelIngertoDescripcion);
                }

                throw new ArgumentException(resultadoDelIngertoDescripcion);
            }
        }  // FIn de - if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - Hasta el muro de edicion todo correcto.");
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - Nombre del muro = " + muroDeRamaBaseDeEdicion.name);
            Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - Numero de hijos del muro = " + muroDeRamaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count);
        }
        ///         5.) Ya tenemos el muro de la rama base de edicion.
        ///                 EN EL MURO DE LA RAMA BASE DE EDICION. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" que configuran el primer nivel de descripcion del concepto que 
        ///             estamos editando. Estos seran o REFERENCIAS, o SINTECHO o INSTANCIAS. Los iremos recorriendo todos y segun su naturaleza actuamos como sigue:
        if (muroDeRamaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos alguna solicitud pendiente
        {
            int i = 0;
            foreach (GameObject hijo in muroDeRamaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - inicio del ciclo del foreach con i = " + i
                                + "\n Con subTipoElementIntf " + hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf);
                }

                // 5.1.) Para el caso de un evi SIN TECHO (subTipoElementIntf = subTipoElemItf_evi_baseSinTecho_00):
                //        Ingertamos el elemento referencia correspòndiente(E => R) mediante "ingertaElementoReferencia"
                //        - OJO recordamos que la referencia NO tiene descripcion (salvo en el DKS de origen), por lo que esta rama muere aqui
                if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    // los datos del tipo de dato sin techo lo tenemos en el objeto "EviTipo_Fractal_01" que en la gerarquia de objetos viene como gigue:
                    //  - EVI_xx
                    //      - ContenedorDeEvi_01
                    //          - EviTipo_Fractal_01
                    // Por lo que tenemos que ir hijos abajo hasta encontrarlo
                    GameObject EviTipo_Fractal_01_tipo = hijo.transform.GetChild(0).transform.GetChild(0).gameObject;

                    string key_tipo = EviTipo_Fractal_01_tipo.GetComponent<SctCtrlEviTipo_Fractal_01>().key;
                    string host_tipo = EviTipo_Fractal_01_tipo.GetComponent<SctCtrlEviTipo_Fractal_01>().host;
                    string cualificador_tipo = EviTipo_Fractal_01_tipo.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador;
                    string ordinal_tipo = EviTipo_Fractal_01_tipo.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf;
                    DateTime ultiModConf = EviTipo_Fractal_01_tipo.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf;
                    int fechUltMod_tipo_int = ultiModConf.Millisecond;
                    string fechUltMod_tipo = fechUltMod_tipo_int.ToString();

                    // los datos del sin techo lo tenemos en el objeto "EviTipo_RF01_Fractum" que en la gerarquia de objetos viene como gigue:
                    //  - EVI_xx
                    //      - ContenedorDeEvi_01
                    //          - EviTipo_Fractal_01
                    //              - EviTipo_RF01_Fractum
                    // Por lo que tenemos que ir hijos abajo hasta encontrarlo
                    GameObject EviTipo_RF01_Fractum_tipo = hijo.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

                    string textoSinTecho = EviTipo_RF01_Fractum_tipo.GetComponent<SctCtrlEviTipo_01Fractum>().datoSinTecho;

                    // Analizamos el objeto de la clase "ClassFichero" para ver si hay que enviar el fichero asociado al DKS
                    // Para esto empleamos la lista de ficheros a enviar al DKS "misFicherosAGrabar"

                    // Si "hijo.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.modificado"es true quiere decir que se ha asociado un fichero a este Sin techo 
                    // (al hoijo que estamos procesando) y que hay que enviarlo al DKS, por lo que
                    // lo anotamos en "misFicherosAGrabar" para que este proceso los envie todos cuando terminemos de generar y enviar esta solicitud de alta al DKS
                    if (hijo.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.modificado)
                    {
                        FicherosAGrabar.Add(hijo.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado);
                        // Ponemos el nombre del fichero como dato del sin techo, para que se puedan relacionar el fichero y el sin techo, cuando ambos lleguen al DKS
                        textoSinTecho = hijo.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.nombre_enKee;
                    }

                    string resultadoDelIngerto;
                    resultadoDelIngerto = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingerta_sinTecho(domKDL, elemento_D_recibido, textoSinTecho, key_tipo, host_tipo, cualificador_tipo, ordinal_tipo, fechUltMod_tipo);
                }  // Fin de - if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &

                // 5.2.) Para el caso de un evi REFERENCIA (subTipoElementIntf = subTipoElemItf_evi_baseRefFractal)
                //        Ingertamos el elemento sin techoòndiente(E => Z) mediante "ingerta_sinTecho"
                //        - OJO recordamos que los elementos Sin Techo NO tienen descripcion, por lo que esta rama muere aqui
                else if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal) &
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    string key_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().key;
                    string host_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().host;
                    string cualificador_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().cualificador;
                    string ordinal_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().ordinal;
                    string fechUltMod_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().fechUltMod;
                    string resultadoDelIngerto;
                    resultadoDelIngerto = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingertaElementoReferencia(domKDL, elemento_D_recibido, key_hijo, host_hijo, cualificador_hijo, ordinal_hijo, fechUltMod_hijo);
                }  // Fin de - else if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal) &

                // 5.3.) Para el caso de un evi REFERENCIA  a elemento de interfaz (subTipoElementIntf = subTipoElemItf_evi_baseRefFractal)
                //        - No tenemos que ingertar nada, ya que los evis de referencia a elemento de interfaz, no son parte de la descripcion del concepto.
                //              tan solo tenemos que borrarlo. OJO, el borrado de un evi de referencia a elemento debe hacerse con cuidado. este puede estar
                //              referenciado como hijo de la panera, o en en otro muro, o en la tramoya... Por lo que para borrarlo hay que contar con todos sus 
                //              progenitores logicos (nuestra gerarquia, esta en la que estamos) y como obgetos de juego (gerarquia de unity)
                else if (hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
                {
                    // NO HAY QUE HACER NADA. No lo borramos, ya lo borrara su padre cuando lo eliminen al eliminar el arbol de edicion
                    // hijo.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();

                }  // Fin de - else if (hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)

                // 5.4.) Para el caso de un evi INSTANCIA (subTipoElementIntf = subTipoElemItf_evi_baseInstFractal)
                //        Ingertamos el elemento instancia correspòndiente(E => A) mediante "ingertaElementoInstancia"
                //        - OJO recordamos que los elementos instancia SI tienen descripcion, por lo que esta rama debe ingertar la descripcion de la instancia
                //        recrsivamente hasta recorres toda la rama de descripcion de la instancia
                else if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal) &
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    string key_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().key;
                    string host_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().host;
                    string cualificador_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().cualificador;
                    string ordinal_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().ordinal;
                    string fechUltMod_hijo = hijo.GetComponent<ScriptCtrlBaseDeEvi>().fechUltMod;
                    XmlElement elemento_D_ADescribir = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().ingertaElementoInstancia(domKDL, elemento_D_recibido, key_hijo, host_hijo, cualificador_hijo, ordinal_hijo, fechUltMod_hijo);
                    // XmlElement ingertaElementoInstancia(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador, string ordinal, string ultimaModificacion)
                    // Aqui habra que llamar recursivamente a esta misma funcion "", pero hay que ver bien como hacerlo
                    string resultadoDelIngerto;
                    resultadoDelIngerto = gestionaDescripcionDeInstanciaEnEdicion(domKDL, elemento_D_ADescribir, hijo.gameObject, FicherosAGrabar);
                }  // Fin de - else if ((hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal) &
                // Si no es ni Instancia, ni Referencia ni Sin Techo, es porque se ha producido un error
                else
                {
                    resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion. El elemento no es Sin Techo, ni referencia ni instancia elemento_D_recibido.Name = "
                         + elemento_D_recibido.Name +
                         " - de tipo = " + elemento_D_recibido.NodeType +
                         " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo +
                         " - subTipoElementIntf = " + hijo.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf;

                    throw new ArgumentException(resultadoDelIngertoDescripcion);
                }  // Fin de - else

                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - final del ciclo del foreach con resultadoDelIngertoDescripcion = " + resultadoDelIngertoDescripcion);
                    string Xml_en_string = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameStringDeXmlDocument(domKDL);
                    Debug.Log(" Desde ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion - final del ciclo del foreach con i = "+i+". - con Xml_en_string : " + Xml_en_string);
                }
                i++;

            }  // FIn de - foreach (GameObject hijo in muroDeRamaBaseDeEdicion.GetComponent<ScriptDatosElemenItf>().listaDeHijos)

        }  // FIn de - if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)


        // 6.) Si todo ha ido bien, devolvemos OK
        resultadoDelIngertoDescripcion = "OK";
        return resultadoDelIngertoDescripcion;


    }  // Fin de - private void cargaDatosEviPrue_001(string xmlDeEvi)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   getionaDescripcionDeInstanciaEnEdicion : Obtiene la descripcion de una instancia en edicion dependiendo de si esta esta expandida o no
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-10-23
    /// Variables de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general de solicitud al DKS
    ///      - XmlElement elemento_D_recibido : Nodo D en que que tenemos que realizar el ingerto
    ///      - GameObject eviDeInstanciaQueDescribe : es el gameobject del evi del cual vamos a ingertar la descripcion "elemento_D_recibido" en el KDL que nos mandan (domKDL)
    /// Variables de salida :
    ///     resultadoDelIngertoDescripcion : Solo es un string que indica si el proceso se ha realizado correctamente
    /// Observaciones :
    ///      - Nos envian en "eviDeInstanciaQueDescribe" un evi de instancia fractal que debe encontrarse en una rama de edicion de un concepto.
    ///      - Este evi de instancia fractal puede estar en dos dituaciones:
    ///         a.) El evi esta en edición, pero NO HA SIDO EXPANDIDO. Esto quiere decir que el usuario NO quiere realizar modificacion alguna sobre la configuracion de la instancia
    ///                 - En este caso, la configuracion de la instancia, no esta expandida mediante sus correspondientes ramas de edicion en el arbol de edicion, Pero 
    ///                 SI SE ENCUENTRA almacenada en el kdl que esta almacenado en el evi de instancia fractal correspondiente, concretamente en "nodo_E_ContEnBaseDeEvi".
    ///                 Por lo que sera ese "nodo_E_ContEnBaseDeEvi" el que tiene que ir a describir la descripción de la instancia.
    ///         b.) El evi esta en edición, Y SI HA SIDO EXPANDIDO. Esto quiere decir que el usuario SI quiere realizar alguna modificacion sobre la configuracion de lainstancia
    ///                 - En este caso, la configuracion de la instancia, se describe por las rama de edición de la instancia, osea por el arbol de evis que haya generado el usuario
    ///                 para describir esta instanciación concreta.
    ///                 Por lo que habra que ir recorriendo la el arbol de edicion que surge de la instancia para obtener la información que describe el concepto editado.
    ///      
    ///      - Para grabar el concepto (evi) que esta en edicion, damos los pasos siguientes:
    ///         1.) Controlamos que el nodo que nos han pasado es un elemento "D", ya que la descripcion del concepto debe ir en un elemento de este tipo. Si no es asi, devolvemos un error
    ///         2.) Controlamos que el EVI esta en modo edicion. Si no es asi, devolvemos un error
    ///         3.) EN EL EVI BASE. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" del EVI, una rama que este en modo edicion. Pueden darse dos casos
    ///             3.1.) NO HAY NINGUNA RAMA en modo edición hija del evi de instancia fractal. Eso quiere decir que el usuario no modifica la configuración de la instancia.
    ///                 Por lo que sera la descripcion que hay en "nodo_E_ContEnBaseDeEvi", la que debe ir al KDL de edicion del concepto
    ///             3.2.) Hay UNA Y SOLO UNA RAMA  en modo edicion hija del evi de instancia fractal. Eso quiere decir que el usuario ha modificado la descripción de esta instancia especifica
    ///                 Por lo que hay que seguir recorriendo el arbol de descripción de la instancia (modificada por el usuario), para obtener la descripcion de la instancia que debe ir 
    ///                 al KDL de edicion del concepto. Para realizar esto, volvemos a llamar recursivamente a la funcion "edicion_ingertaDescripcion"
    ///             3.3.) Hay MAS DE UNA RAMA en edicion. Esto no debe ser asi, por lo que devolvemos un error
    ///         
    ///         4.) Si todo ha ido bien, devolvemos OK
    ///         
    /// </summary>
    public string gestionaDescripcionDeInstanciaEnEdicion(XmlDocument domKDL, XmlElement elemento_D_recibido, GameObject eviDeInstanciaQueDescribe, List<ClassFichero> FicherosAGrabar)
    {
        string resultadoDelIngertoDescripcion = "resultadoDelIngertoDescripcion por defecto";

        GameObject ramaBaseDeEdicion = null;
        GameObject muroDeRamaBaseDeEdicion = null;

        // 1.) Controlamos que el nodo que nos han pasado es un elemento "D", ya que la descripcion del concepto debe ir en un elemento de eswte tipo. Si no es asi, devolvemos un error
        bool esAdecuado = elemento_D_recibido.Name == ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + "D";

        // 2.) Controlamos que el EVI esta en modo edicion (tambien nos vale el modo cabeza de edicion). Si no es asi, devolvemos un error
        esAdecuado = (esAdecuado &
            ((eviDeInstanciaQueDescribe.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion) || (eviDeInstanciaQueDescribe.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_cabezaEdicion)));
        if (!esAdecuado)
        {
            resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => gestionaDescripcionDeInstanciaEnEdicion el ingerto no ha podido realizarse, el nodo madre, no es D. EL nodo recibido es elemento_D_recibido.Name = "
                 + elemento_D_recibido.Name +
                 " - de tipo = " + elemento_D_recibido.NodeType +
                 " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo;

            throw new ArgumentException(resultadoDelIngertoDescripcion);
        }

        // 3.) EN EL EVI BASE. Buscamos entre los hijos "ScriptDatosElemenItf.listaDeHijos" del EVI, una rama que este en modo edicion. Pueden darse dos casos
        if (eviDeInstanciaQueDescribe.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos alguna solicitud pendiente
        {
            int numeroDeRamasBaseDeEdicion = 0;
            foreach (GameObject hijo in eviDeInstanciaQueDescribe.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama) &
                    ((hijo.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)))
                {
                    ramaBaseDeEdicion = hijo.gameObject;
                    numeroDeRamasBaseDeEdicion++;
                }
            }  // FIn de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

            // Como hemos dicho en 3.) PUEDEN DARSE DOS CASOS:

            //  3.1.) NO HAY NINGUNA RAMA en modo edición hija del evi de instancia fractal. Eso quiere decir que el usuario no modifica la configuración de la instancia.
            //      Por lo que sera la descripcion que hay en "nodo_E_ContEnBaseDeEvi", la que debe ir al KDL de edicion del concepto
            if (numeroDeRamasBaseDeEdicion == 0)
            {
                // Generamos una copia del nodo de de este evi de instancia fractal que estamos recorriendo
                //  OJOOO Estamos trabajando con dos DOM (2 ficheros XML)
                //      - Uno "domKDL" es el documento que estamos generando. El KDL del concepto modificado que estamos generando. (este es "domKDL")
                //      - El otro es el documento DOM del "eviDeInstanciaQueDescribe" que es el KDL del que cada evi tiene asociado
                //          su nodo correspondiente en el KDL del concepto global (este es "eviDeInstanciaQueDescribe => ScriptCtrlBaseDeEvi => domPropio")
                //
                //      Tendremos que cojer el elemento D de "nodo_E_ContEnBaseDeEvi" que esta en "eviDeInstanciaQueDescribe => ScriptCtrlBaseDeEvi => domPropio" y ponerlo 
                //      en "domKDL => elemento_D_recibido"

//                XmlNode nodo_E_ContenidoEnBaseDeEvi_queRecorremos = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.CloneNode(true);
//                XmlNode nodo_E_ContenidoEnBaseDeEvi_queRecorremos_En_domKDL = domKDL.ImportNode(nodo_E_ContenidoEnBaseDeEvi_queRecorremos, true);  // true es para que inporte los hijos del nodo
//                elemento_D_recibido.AppendChild(nodo_E_ContenidoEnBaseDeEvi_queRecorremos_En_domKDL);

                // Accedemos al nodo  "nodo_E_ContEnBaseDeEvi" que es donde esta el elemento de la instancia que nos ocupa y hacemos una copia para trabajar con ella
                XmlNode nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.CloneNode(true);

                // Buscamos el nodo D
                // Obtenemos el elemento descripción del nodo nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos
                    //Declaramos del Espacio de Nombres, necesario para xPath
                XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
                manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);

                // Segun estemos en un nodo E o C, el nodo D estara en un sitio distinto:
                string xPathString_para_D = "";
                if (nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos.Name == ScriptLibConceptosXml.xPathString_name_E)
                {
                    xPathString_para_D = "." + ScriptLibConceptosXml.xPathString_para_A + ScriptLibConceptosXml.xPathString_para_D;
                }
                else if (nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos.Name == ScriptLibConceptosXml.xPathString_name_C)
                {
                    xPathString_para_D = "." + ScriptLibConceptosXml.xPathString_para_D;
                }
                else
                {
                    if (DatosGlobal.niveDebug > 50)
                    {
                        string nodo_E_ContEnBaseDeEvi_0 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.Name;
                        string nodo_E_ContEnBaseDeEvi_1 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.Name;
                        string nodo_E_ContEnBaseDeEvi_2 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.FirstChild.Name;

                        Debug.Log(" ERROR Desde ScriptLibConceptosXml =>gestionaDescripcionDeInstanciaEnEdicion SIN RAMA DE EDICION (no es nodo E o C) con  : " +
                                    "\n -xPathString_para_D = " + xPathString_para_D +
                                    "\n - elemento_D_recibido = " + elemento_D_recibido.Name +
                                    "\n - nodo_E_ContEnBaseDeEvi = " + eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.Name +
                                    "\n - nodo_E_ContEnBaseDeEvi_0 = " + nodo_E_ContEnBaseDeEvi_0 +
                                    "\n - nodo_E_ContEnBaseDeEvi_1 = " + nodo_E_ContEnBaseDeEvi_1 +
                                    "\n - nodo_E_ContEnBaseDeEvi_2 = " + nodo_E_ContEnBaseDeEvi_2
                                    );
                    }
                }  // Fin de - else - de -  if (nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos.Name == ScriptLibConceptosXml.xPathString_name_D)

                // Ya sabemos donde esta el nodo D. Lo obtenemos para trabajar con el 
                XmlNode nodo_D_descripcion_a_ingertar = nodo_E_o_C_ContenidoEnBaseDeEvi_queRecorremos.SelectSingleNode(xPathString_para_D, manejadorEspNomb);

                // Importamos el nodo D al domKDL
                XmlNode nodo_D_descripcion_a_ingertar_En_domKDL = domKDL.ImportNode(nodo_D_descripcion_a_ingertar, true);  // true es para que inporte los hijos del nodo

                // Lo hacemos hijo de el nodo A que es el padre del nodo D que nos han enviado
                // Eliminado el nodo D que nos han enviado para que solo haya u nodo de descripcion
                XmlNode nodo_A_padre = elemento_D_recibido.ParentNode;
                nodo_A_padre.ReplaceChild(nodo_D_descripcion_a_ingertar_En_domKDL, elemento_D_recibido);


                if (DatosGlobal.niveDebug > 50)
                {
                    string nodo_E_ContEnBaseDeEvi_0 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.Name;
                    string nodo_E_ContEnBaseDeEvi_1 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.Name;
                    string nodo_E_ContEnBaseDeEvi_2 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.FirstChild.Name;

                    Debug.Log(" Desde ScriptLibConceptosXml =>gestionaDescripcionDeInstanciaEnEdicion SIN RAMA DE EDICION desde el if con  : " +
                                "\n - elemento_D_recibido = " + elemento_D_recibido.Name +
                                "\n - nodo_E_ContEnBaseDeEvi = " + eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.Name +
                                "\n - nodo_E_ContEnBaseDeEvi_0 = " + nodo_E_ContEnBaseDeEvi_0 +
                                "\n - nodo_E_ContEnBaseDeEvi_1 = " + nodo_E_ContEnBaseDeEvi_1 +
                                "\n - nodo_E_ContEnBaseDeEvi_2 = " + nodo_E_ContEnBaseDeEvi_2 
                                );
                }
            }  // Fin de - if (numeroDeRamasBaseDeEdicion == 0)
            //  3.2.) Hay UNA Y SOLO UNA RAMA  en modo edicion hija del evi de instancia fractal. Eso quiere decir que el usuario ha modificado la descripción de esta instancia especifica
            //      Por lo que hay que seguir recorriendo el arbol de descripción de la instancia, para obtener la descripcion de la instancia que debe ir al KDL de edicion del concepto.
            //       Para realizar esto, volvemos a llamar recursivamente a la funcion "edicion_ingertaDescripcion"
            else if (numeroDeRamasBaseDeEdicion == 1)
            {
                string otraInsercionDeInstancia;
                otraInsercionDeInstancia = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().edicion_ingertaDescripcion(domKDL, elemento_D_recibido, eviDeInstanciaQueDescribe, FicherosAGrabar);


                if (DatosGlobal.niveDebug > 1000)
                {
                    string nodo_E_ContEnBaseDeEvi_0 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.Name;
                    string nodo_E_ContEnBaseDeEvi_1 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.Name;
                    string nodo_E_ContEnBaseDeEvi_2 = eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.FirstChild.FirstChild.Name;

                    Debug.Log(" Desde ScriptLibConceptosXml =>gestionaDescripcionDeInstanciaEnEdicion CON UNA RAMA DE EDICION desde el if con  : " +
                                "\n - elemento_D_recibido = " + elemento_D_recibido.Name +
                                "\n - nodo_E_ContEnBaseDeEvi = " + eviDeInstanciaQueDescribe.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.Name +
                                "\n - nodo_E_ContEnBaseDeEvi_0 = " + nodo_E_ContEnBaseDeEvi_0 +
                                "\n - nodo_E_ContEnBaseDeEvi_1 = " + nodo_E_ContEnBaseDeEvi_1 +
                                "\n - nodo_E_ContEnBaseDeEvi_2 = " + nodo_E_ContEnBaseDeEvi_2 
                                );
                }
            }
            // 3.3.) Hay MAS DE UNA RAMA en edicion. Esto no debe ser asi, por lo que devolvemos un error
            else
            {
                resultadoDelIngertoDescripcion = "ERROR, desde  ScriptCtrlBaseDeEvi => edicion_ingertaDescripcion el EVI no posee una y solo una rama de edicion. EL nodo recibido es elemento_D_recibido.Name = "
                     + elemento_D_recibido.Name +
                     " - de tipo = " + elemento_D_recibido.NodeType +
                     " - de modo = " + GetComponent<ScriptDatosElemenItf>().modo +
                     " - numeroDeRamasBaseDeEdicion = " + numeroDeRamasBaseDeEdicion;

                throw new ArgumentException(resultadoDelIngertoDescripcion);
            }
        }  // FIn de - if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)

        //       
        //  4.) Si todo ha ido bien, devolvemos OK
        resultadoDelIngertoDescripcion = "OK";
        return resultadoDelIngertoDescripcion;

    } // Fin de -  public string gestionaDescripcionDeInstanciaEnEdicion(XmlDocument domKDL, XmlElement elemento_D_recibido, GameObject eviDeInstanciaQueDescribe)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  enviaSolicitudADks(XmlDocument kdlSolAltaNuevoConcepto)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-10-24
    /// Variables de entrada :
    ///      - XmlDocument kdlSolAltaNuevoConcepto : Dom del KDL de solicitud al DKS
    /// Variables de salida :
    ///     resultadoDelIngertoDescripcion : Solo es un string que indica si el proceso se ha realizado correctamente
    /// Observaciones :
    /// 		- Pasos a realizar :
    /// 		1.) Convertimos en DON en string XML que es lo que viaja en la solicitud HTTP
    /// 		2.) Generamos una solicitud para tramitar el proceso de alta entre el KEE y el DKS. 
    /// 		    2.1.) Generamos los intervinientes
    /// 		        2.1.1.)
    /// 		
    /// </summary>
    public void enviaSolicitudADks(XmlDocument kdlSolAltaNuevoConcepto)
    {
        // //////////////////////////////////////////////////////////
        // //////////////////////////////////////////////////////////
        //	1.) Convertimos en DON en string XML que es lo que viaja en la solicitud HTTP
        string kdlSolAltaNuevoConcepto_en_string = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameStringDeXmlDocument(kdlSolAltaNuevoConcepto);

        // //////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        //	2.) Generamos una solicitud para tramitar el proceso de alta entre el KEE y el DKS. 
        // Generamos el elemento solicitud para gestionar la solicitud al DKS
        // INICIO SOLICITUD ///////////////////////////////////////////

        GameObject solicitud = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Solicitud);
        solicitud.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_solicitud);
        solicitud.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks);
        // idSolicitud;  Se ha puesto mediante ponSubTipoElementIntf
        // hora_solicitud - Se pone en el awake de la solicitud
        // hora_respuesta - Todabia sin valor
        // hora_fin - Todabia sin valor
        // tiempoDeVida - lo incrementa "ScriptGestorSolicitudes"
        solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_consultaKdlADks_AltaConcepto; // Estamos en el boton de salir de edicion de concepto
        solicitud.GetComponent<ClassSolicitud>().subTipo_solicitud = solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();   // Usamos esta variable para hacer el tipo mas accesible
        solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso;   // La solicitud ya esta en curso. EN ESTE ESTADO ALGUN INTERVINIENTE PUEDE REQUERIR ATENCION
                                                                                                        // XmlDocument datos_Dom_solicitud - No procede en este caso

        // Esto que sigue es provisional. Debe ir todo el la consulta en DOM que es mas elegante PENDIENE MAFG 2021-08-11
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrKey); // Es el key del usuario que realiza la solicitud
        //        string hostUsr = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host;  // Para que se adapte al host en el que se trabaja
        string hostUsr = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host;  // Para que se adapte al host en el que se trabaja
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(hostUsr); // Es el host del usuario que realiza la solicitud. Por ahora (2021-10-24) solo se permiten altas y modificaciones en el DKS en el que nos conectamos inicialmente
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(kdlSolAltaNuevoConcepto_en_string); // Es el KDL de solicitud 
                                                                                                             // XmlDocument respuesta_Dom_solicitud - No procede en este caso
                                                                                                             // List<string> respuesta_txt_solicitud - Se sabrá mas tarde. C0ntendra la respuesta del usuario que sera SI o NO
                                                                                                             // Hasta aqui lo PENDIENTE

        // //////////////////////////////////////////////////////////
        //    2.1.) Generamos los intervinientes
        //   public List<ClassInterviniente> listaIntervinientes;  // Es una lista que contiene sus hijos de primer nivel

        // //////////////////////////////////////
        // INTERVINIENTES EN LA SOLICITUD :
        // 0.) este "BaseDeEvi_01" (es el evi que esta en edicion   lo llamo aqui "BaseDeEvi_01_eviEnEdicion")
        //        - Genera la solicitud
        //        - Envia la peticion al DKS. Y cuando esta reciba la respuesta hara que la solicitud requiera al siguiente 
        //          interviniente, que será este mismo "BaseDeEvi_01"
        //    Siguiente interviniente : este mismo "BaseDeEvi_01"

        ClassInterviniente interviniente_BaseDeEvi_01_eviEnEdicion_ord_0 = new ClassInterviniente();
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.Interviniente = this.gameObject; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.ordinal_Interviniente = 0; ; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // Es quien realiza la solicitud al DKS.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;   // esta intervencion realiza la solicitud al DKS y eso ya esta hecho cuando termine este metodo
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.hora_ini_intervencion = DateTime.Now;
        solicitud.GetComponent<ClassSolicitud>().listaIntervinientes.Add(interviniente_BaseDeEvi_01_eviEnEdicion_ord_0); // Es una lista que contiene sus hijos de primer nivel

        // 1.) este "BaseDeEvi_01" (es el evi que esta en edicion   lo llamo aqui "BaseDeEvi_01_eviEnEdicion") 
        //        - Atiende la solicitud. Sera requerido cuando se haya recibido la respuesta del DKS
        //        - Con la respuesta del DKS ...
        //        - Pone el estado de la intervencion en finalizado para que "ScriptGestorSolicitudes" finalice la solicitud

        ClassInterviniente interviniente_BaseDeEvi_01_eviEnEdicion_ord_1 = new ClassInterviniente();
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_1.Interviniente = this.gameObject; // Es este mismo evi quien debe recibir la respuesta del DKS.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_1.ordinal_Interviniente = 1; ; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_0.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_receptor; // Es quien debe recibir la respuesta del DKS.
        interviniente_BaseDeEvi_01_eviEnEdicion_ord_1.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_enEjecucion;   // en ejecucion a la espera de que llegue la respuesta del DKS
        solicitud.GetComponent<ClassSolicitud>().listaIntervinientes.Add(interviniente_BaseDeEvi_01_eviEnEdicion_ord_1);  // Es una lista que contiene sus hijos de primer nivel

        ctrlInterfaz.GetComponent<ScriptConexionDKS>().solicitudConKdlAlDks(solicitud);
    } // Fin de - public void enviaSolicitudADks(XmlDocument kdlSolAltaNuevoConcepto)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : procesaRespuestaSolicitud_ADks_AltaConcepto 
    /// Observaciones : Este metodo procesa la respuesta de una solicitud de alta de concepto realizada (normlmente a un DKS).
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-10-24
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject solicitudAsociada : es la solicitud que genero el buscador. Esta ya esta resuelta, con lo que contiene los datos de respuesta del DKS
    /// Variables de salida :
    ///     No devuelve nada, solo actua en consecuencia al KDL recibido como respuesta
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         Esta funcion debe analizar el tipo de respuesta que recibe para saber si es un KDL directamente de conceto, o una lista a un arbol o lo que sea
    ///         Segun sea la naturaleza de la respuesta, generara un evi del subtipo que corresponda (fractal, lista, arbol. etc...). SI NO CONOCE EL CONCEPTO
    ///         QUIERE DECIR QUE NO ES UN CONCEPTO QUE SEPA PONER EN UN EVI ESPECIFICO, LUEGO CUALQUIER CONCEPTO DESCONOCIDO SE PONE COMO FRACTAL
    ///         - PENDIENTE MAFG 2021-02-13) Por ahora no analizamos el tipo de respuesta y consideramos que esta en un KDL de concepto, sin mirar nada, por lo que
    ///         generamos siempre un evi fractal
    ///         
    ///     Pasos de ejecucion:
    ///         1.) Primero tenemos que mirar en solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add("") que debe poner OK
    /// </summary>
    public void procesaRespuestaSolicitud_ADks_AltaConcepto(GameObject solicitudAsociada)
    {

        // //////////////////////////////////////////////
        // 1.) Primero tenemos que mirar en solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add("") que debe poner OK, si no hay un erro ry hay que indicarlo
        if (solicitudAsociada.GetComponent<ClassSolicitud>().respuesta_txt_solicitud[0] != "OK")
        {
            // Aqui habria que generar un evi de error con la informacion del error que se ha producido. PENDIENTE MAFG 2021-10-24
            // por ahora solo ponemos un log
            if (DatosGlobal.niveDebug > 50)
            { Debug.Log(" Desde ScriptCtrlBaseDeEvi => procesaRespuestaSolicitud_ADks_AltaConcepto. ERROR en la solicitud = " + solicitudAsociada.name +
                        " - Info error : " + solicitudAsociada.GetComponent<ClassSolicitud>().respuesta_txt_solicitud[0]); }
        }  // Fin de - if (solicitudAsociada.GetComponent<ClassSolicitud>().respuesta_txt_solicitud[0] != "OK")


        // La respuesta que viene en la solicitud esta en la variable DOM "respuesta_Dom_solicitud"
        XmlDocument domRespuesta = solicitudAsociada.GetComponent<ClassSolicitud>().respuesta_Dom_solicitud;

        if (DatosGlobal.niveDebug > 50)
        { Debug.Log(" Desde ScriptCtrlBaseDeEvi => procesaRespuestaSolicitud_ADks_AltaConcepto. He entrado  en el cuadro = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

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
    }  // Fin de -  public void procesaRespuestaSolicitud_ADks_AltaConcepto(GameObject solicitudAsociada)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : ponBotonesDeInstanciaFractal . Pone los botones de desplazar y maxinizar en color azul (para convertr un evi de referencia en uno de instancia  (Solo en edicion))
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-15
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo actua en consecuencia al KDL recibido como respuesta
    /// Observaciones:
    ///     - Pone los botones de desplazar y maxinizar en color azul para convertr un evi de referencia en uno de instancia
    ///    ´- OJO. En principio, un evi de referencia pasa a ser una instancia Solo en edicion (2021-11-15)
    /// </summary>
    public void ponBotonesDeInstanciaFractal()
    {
        // si es instancia ponemos los botones de instancia (azules)
        Btn_Evi_Maxi_Mini.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimizaInstancia;
        Btn_Evi_Desplazar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazarInstancia;
        Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesInstancia;

    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : enviaFicherosADks . Envia cada uno de los ficheros que hay en la lista "" al DKS que corresponde
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-03-05
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo envia los ficheros al DKS, Si se produjera algun erros, sera el DKS  quien lo indique
    /// Observaciones:
    ///     - OJOO. Cada evi base del arbol de descrip
    /// </summary>
    public void  enviaFicherosADks(List<ClassFichero> FicherosAGrabar)
    {
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Desde enviaFicherosADks - antes del foreach con FicherosAGrabar.Count : " + FicherosAGrabar.Count);
        }
        if (FicherosAGrabar.Count != 0)
        {
            int contador_ficheros = 0;
            foreach (ClassFichero fichero in FicherosAGrabar)
            {
                contador_ficheros++ ;
                string nombre_origen = fichero.nombre_origen;
                string extension = fichero.extension;
                string nombre_enKee = fichero.nombre_enKee;
                string path_enKee = fichero.path_enKee;
                string codigo_de_envio_Kee = "_fich_"+ contador_ficheros;

                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log("Desde enviaFicherosADks - con nombre_origen " + nombre_origen
                          + " - extension : " + extension
                          + " - nombre_enKee : " + nombre_enKee
                           + " - path_enKee : " + path_enKee);
                }

                StartCoroutine(IEnu_enviaFichero(nombre_origen, extension, nombre_enKee, path_enKee, codigo_de_envio_Kee)); // Llamamos a la corrutina que enviara el fichero
            }
            FicherosAGrabar.Clear(); // Debemos habre procesado todos los elementos de la lista y esta estará vacia
        }

    }  // Fin de - public void enviaFicherosADks()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void atiendeMariLlanos(GameObject solicitudAAtender)
    ///     Este metodo atiende las solicictudes en las que figura como receptor.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-06-23
    /// Ultima modificacion :
    ///         - 2021-08-07. MAFG, para atender las solicitudes de "Btn_Consulta_Si_No", como respuesta a cerrar o grabar el evi en edicion
    ///         - 2021-08-12. MAFG, para adaptarlo a la modificacion de las dolicitude (que la he simplificado bastante)
    ///         - 2021-10-24. MAFG, para incluir las solicitudes correspondientes a el alta de conceptos (desde la edicion)
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
    ///         
    ///         - Este metodo atiende las solicictudes en las que figura como interviniente, por lo que las solicitudes recibidas deben ser :
    ///             a.) ScriptDatosElemenItf => "tipoElementIntf" == tipoElemItf_solicitud
    ///             b.) Los tipos de solicitud que aqui se atienden son :
    ///                     1. "subTipoSolicitud_RespBtn_Consulta_Si_No"
    ///                     2. "subTipoSolicitud_consultaKdlADks"
    ///                     
    ///             
    ///         - Tipos de solicitud y sus pasos para procesarlas        
    /// 
    ///                 1.) Tipo de solicitud "subTipoSolicitud_RespBtn_Consulta_Si_No". (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No)
    ///                         Los pasos a recorrer son:
    ///                         
    ///                         1.1.) tipo_sol_resp_sino_btn_Editar_Salir :
    ///                                 Esta solicitud corresponde al caso en el que un objeto "Btn_Consulta_Si_No" ha consultado al usuario sobre su intencion de salir de la edicion de 
    ///                                 un evi (asociado a este evi base). En este caso el usuario ha contestado con la respuesta pertinente, el objeto "Btn_Consulta_Si_No" nos manda la respuesta
    ///                                 que sera si o no y aqui debemos actuar en consecuencia
    ///                                     1.1.)
    ///                                     - "respUsr_Si". Hay que eliminar la rama de edicion y modificar el "modo" de este elemento de interfaz, desde "modoElemItf_edicion" a "modoElemItf_navegacion"
    ///                                     1.2.)
    ///                                     - "respUsr_No". No hacemos nada. El usuario no desea abandonar la edicion
    ///                         
    ///                         1.2.) tipo_sol_resp_sino_btn_Editar_Grabar :
    ///                                 Esta solicitud corresponde al caso en el que un objeto "Btn_Consulta_Si_No" ha consultado al usuario sobre su intencion de grabar en el DKS el 
    ///                                 resultado de la edicion del evi (contenido a este evi base). En este caso el usuario ha contestado con la respuesta pertinente, el 
    ///                                 objeto "Btn_Consulta_Si_No" nos manda la respuesta que sera si o no y aqui debemos actuar en consecuencia
    ///                                     2.1.)
    ///                                     - "respUsr_No". No hacemos nada. El usuario no desea grabar el estado de la edicion en el DKS. El evi sigue en edicion
    ///                                     2.2.)
    ///                                     - "respUsr_Si". Llamamos al metodo grabar nuevo concepto en DKS
    ///                                             La respuesta de la solicitud que enviamos al DKS debe ser :
    ///                                             2.2.1.)
    ///                                                 - Un error. En este caso mostramos el concepto de error en el muro donde esta el evi raiz de la edcion que nos ocupa
    ///                                             2.2.2.)
    ///                                                 - Si el alta ha finalizado con exito. EL DKS respondera con un concepto respuesta en el que indicara que todo esta OK y
    ///                                                 llevará tambien una referencia al concepto que terminamos de dar de alta. Mostrando el concepto recibido
    ///                                                 indicamos que el alta ha finalizado con exito. El evi de respuesta debe llevar una referencia al concepto recien dado 
    ///                                                 de alta, donde el usuario puede pinchar, para solicitar el concepto y visualizarlo en la interfaz KEE
    /// 
    ///                        1.3.) Eliminamos la solicitud (no hay mas intervinientes)
    ///                                     
    ///                 2.) Tipo de solicitud "subTipoSolicitud_consultaKdlADks". (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)
    ///                         Los pasos a recorrer son:
    ///                         
    ///                         1.1.) tipo_sol_resp_sino_btn_Editar_Salir :
    ///                                 Esta solicitud corresponde al caso en el que un objeto "Btn_Consulta_Si_No" ha consultado al usuario sobre su intencion de salir de la edicion de 
    ///                                 un evi (asociado a este evi base). En este caso el usuario ha contestado con la respuesta pertinente, el objeto "Btn_Consulta_Si_No" nos manda la respuesta
    ///                                 que sera si o no y aqui debemos actuar en consecuencia
    ///                                     1.1.)
    ///                                     - "respUsr_Si". Hay que eliminar la rama de edicion y modificar el "modo" de este elemento de interfaz, desde "modoElemItf_edicion" a "modoElemItf_navegacion"
    ///                                     1.2.)
    ///                                     - "respUsr_No". No hacemos nada. El usuario no desea abandonar la edicion
    ///                         
    ///                         1.2.) tipo_sol_resp_sino_btn_Editar_Grabar :
    ///                                 Esta solicitud corresponde al caso en el que un objeto "Btn_Consulta_Si_No" ha consultado al usuario sobre su intencion de grabar en el DKS el 
    ///                                 resultado de la edicion del evi (contenido a este evi base). En este caso el usuario ha contestado con la respuesta pertinente, el 
    ///                                 objeto "Btn_Consulta_Si_No" nos manda la respuesta que sera si o no y aqui debemos actuar en consecuencia
    ///                                     2.1.)
    ///                                     - "respUsr_No". No hacemos nada. El usuario no desea grabar el estado de la edicion en el DKS. El evi sigue en edicion
    ///                                     2.2.)
    ///                                     - "respUsr_Si". Llamamos al metodo grabar nuevo concepto en DKS
    ///                                             La respuesta de la solicitud que enviamos al DKS debe ser :
    ///                                             2.2.1.)
    ///                                                 - Un error. En este caso mostramos el concepto de error en el muro donde esta el evi raiz de la edcion que nos ocupa
    ///                                             2.2.2.)
    ///                                                 - Si el alta ha finalizado con exito. EL DKS respondera con un concepto respuesta en el que indicara que todo esta OK y
    ///                                                 llevará tambien una referencia al concepto que terminamos de dar de alta. Mostrando el concepto recibido
    ///                                                 indicamos que el alta ha finalizado con exito. El evi de respuesta debe llevar una referencia al concepto recien dado 
    ///                                                 de alta, donde el usuario puede pinchar, para solicitar el concepto y visualizarlo en la interfaz KEE
    /// 
    ///                        1.3.) Eliminamos la solicitud (no hay mas intervinientes)
    ///                                     
    ///                         
    /// </summary>
    public void atiendeMariLlanos(GameObject solicitudAAtender)
    {
        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Entro en ScriptCtrlBaseDeEvi => atiendeMariLlanos"); }

        if (solicitudAAtender != null)
        { 
            // el "tipoElementIntf" debe ser "tipoElemItf_solicitud"
            // Segun el subtipo (tipo de solicitud) habra que atenderla como proceda
            string subTipoElementIntf = solicitudAAtender.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
            string codigoDeSolicitud = solicitudAAtender.GetComponent<ClassSolicitud>().codigoDeSolicitud;

            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  1.) Tipo de solicitud "subTipoSolicitud_RespBtn_Consulta_Si_No". (subTipoElementIntf == ScriptDatosElemenItf.tipoSolicitud_RespBtn_Consulta_Si_No)

            // Si el "subTipoElementIntf" es "subTipoSolicitud_RespBtn_Consulta_Si_No" es una pregunta de si o no y se atiende como sigue
            if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No)  // Si se ha recibido bien
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos en 191. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }

                string respuestaUsuario = solicitudAAtender.GetComponent<ClassSolicitud>().respuesta_txt_solicitud[0];  // Aqui viene la respuesta SI o No del usuario

                // Tenemos varios codigos de solicitud para el tipo "subTipoSolicitud_RespBtn_Consulta_Si_No". Estos son :
                //      - codigoDeSolicitud => "CodigoSol_resp_sino_btn_Editar_Salir"
                //      - codigoDeSolicitud => "CodigoSol_resp_sino_btn_Editar_Grabar"

                // 1.) tipo_sol_resp_sino_btn_Editar_Salir:
                if (codigoDeSolicitud == ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir)  // Si se ha recibido bien
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos en btn_Editar_Salir. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                    // Esta solicitud corresponde al caso en el que un objeto "Btn_Consulta_Si_No" ha consultado al usuario sobre su intencion de salir de la edicion de 
                    // un evi (asociado a este evi base). En este caso el usuario ha contestado con la respuesta pertinente, el objeto "Btn_Consulta_Si_No" nos manda la respuesta
                    // que sera si o no y aqui debemos actuar en consecuencia
                    //    1.1.) - "respUsr_Si". Hay que eliminar la rama de edicion y modificar el "modo" de este elemento de interfaz, desde "modoElemItf_edicion" a "modoElemItf_navegacion"
                    if (respuestaUsuario == ScriptBtn_Consulta_Si_No.respUsr_Si)
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde  ScriptCtrlBaseDeEvi => atiendeMariLlanos entro en SALIR respuesta SI 1281 con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                        edicion_salir();
                    }
                    //    1.2.) - "respUsr_No". No hacemos nada. El usuario no desea abandonar la edicion. 
                    // Lo hacemos en el else para hacerlo como defecto en cualquier caso no conocido
                    else
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde  ScriptCtrlBaseDeEvi => atiendeMariLlanos entro en SALIR respuesta NO 1326 NO HAY QUE HACER NADA con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                    }
                }  // FIn de - if (codigoDeSolicitud == ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir)
                else if (codigoDeSolicitud == ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Grabar)
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos en btn_Editar_Grabar. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                    if (respuestaUsuario == ScriptBtn_Consulta_Si_No.respUsr_Si)
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde  ScriptCtrlBaseDeEvi => atiendeMariLlanos entro en GRABAR respuesta SI 1281 con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                        // "respUsr_Si". Llamamos al metodo grabar nuevo concepto en DKS
                        edicion_grabar();
                    }
                    //    2.3.1.2.) - "respUsr_No". No hacemos nada. El usuario no desea abandonar la edicion. 
                    // Lo hacemos en el else para hacerlo como defecto en cualquier caso no conocido
                    else
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde  ScriptCtrlBaseDeEvi => atiendeMariLlanos entro en GRABAR respuesta NO 1326 NO HAY QUE HACER NADA con idSolicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                        ///                                     2.3.1.1.)
                        ///                                     - "respUsr_No". No hacemos nada. El usuario no desea grabar el estado de la edicion en el DKS. El evi sigue en edicion
                    }
                }
                else
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos con subTipoElementIntf desconocido. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                }

                // Gestionamos los distintos intervinientes
                // Indicamos que este interviniente ya ha terminado (somos el interviniente de 2 de la lista de intervinientes)
                solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[2].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
                solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[2].hora_fin_intervencion = DateTime.Now;
                // Indicamos que  LA SOLICITUD HA TERMINADO y la hora en la que termina
                solicitudAAtender.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_finalizada;
                solicitudAAtender.GetComponent<ClassSolicitud>().hora_fin = DateTime.Now;

                // Ponemos activo el puntero de usuario
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<SphereCollider>().isTrigger = true;

            }  // FIn de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No)

            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  2.) Tipo de solicitud "subTipoSolicitud_consultaKdlADks". (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)

            // Si el "subTipoElementIntf" es "subTipoSolicitud_RespBtn_Consulta_Si_No" es una pregunta de si o no y se atiende como sigue
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)  // Si se ha recibido bien
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos en la entrada, atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }

                // Los codigos de solicitud para el tipo "subTipoSolicitud_consultaKdlADks". Estos son :
                //      - codigoDeSolicitud => "CodigoSol_consultaKdlADks_GetDetails"  (NO ES ATENDIDO por este evi base)
                //      - codigoDeSolicitud => "CodigoSol_consultaKdlADks_AltaConcepto"
                //      - codigoDeSolicitud => "CodigoSol_consultaKdlADks_ModificaConcepto"
                //      - Por ahora no hay mas (2021-10-24)

                if (codigoDeSolicitud == ClassSolicitud.CodigoSol_consultaKdlADks_GetDetails)  // Si se ha recibido bien
                {
                    if (DatosGlobal.niveDebug > 100)
                    { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos NO se atender solicitud de GetDetails : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                }
                else if (codigoDeSolicitud == ClassSolicitud.CodigoSol_consultaKdlADks_AltaConcepto)  // Si se ha recibido bien
                {
                    procesaRespuestaSolicitud_ADks_AltaConcepto(solicitudAAtender);

                }
                else if (codigoDeSolicitud == ClassSolicitud.CodigoSol_consultaKdlADks_GetDetails)  // Si se ha recibido bien
                {
                    if (DatosGlobal.niveDebug > 100)
                    { Debug.Log("Estoy en ScriptCtrlBaseDeEvi => atiendeMariLlanos Todabia NO se atender solicitud de modificacion de concepto : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }
                }
                else
                {
                    if (DatosGlobal.niveDebug > 100)
                    { Debug.Log("Entro en ScriptCtrlBaseDeEvi => atiendeMariLlanos -subTipoSolicitud_consultaKdlADks- y salgo por el else con estado : " + solicitudAAtender.GetComponent<ClassSolicitud>().estado_solicitud); }
                }
            } // FIn de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks) 


            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //   else if (codigoDeSolicitud == ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir) { }  NO ATENDEMOS OROS SUBTIPOS DE SOLICITUD POR AHORA MAFG 2021-08-10
            else
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Entro en ScriptBtn_Consulta_Si_No => atiendeMariLlanos y salgo por el else con estado : " + solicitudAAtender.GetComponent<ClassSolicitud>().estado_solicitud); }
            }
        } // FIn de - if (solicitudAAtender != null)
    }  // FIn de - public void atiendeMariLlanos(GameObject solicitudAAtender)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : apartaEviQuePaso. Aparta el evi "eviQueSeAparta" (normalmente este evi) cuando viene otro empujando
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-07
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     - GameObject eviQueViene; Es el evi que viene y que debe apartar al otro para no superponerse
    ///     - GameObject eviQueSeAparta; Es el evi tenemos que apartar para dar paso al que viene
    /// Variables de salida :
    ///     No devuelve nada, solo aparta al gameobject "eviQueSeAparta" para dejar pasar al "eviQueViene" 
    /// Observaciones:
    ///     - 
    ///     - Pasos a realizar
    ///         - OJO el objeto que choca con este, debe tener un rigidbody (el "PunteroUsuario" lo tiene)
    ///         - Las colisiones que se espera atender son las siguientes:
    ///         1.) Colision con el "PunteroUsuario". 
    ///             1.1.) En el caso de que el puntero de usuario venga sin arrastrar ningun evi, no actuamos de ninguna manera
    ///             1.2.) En el caso de que el puntero de usuario colisione con nosotros, arrastrando un evi, diferenciamos dos casos
    ///                 1.2.1.) Si somos un evi sin techo y estamos en un muro de edicion; lo dejamos pasar. Esto es porque si suelta ese evi sobre nuestro boton de info, debemos
    ///                         cambiar el tipo de dato del sin techo y adoptar como tipo de dato el concepto que han arrastado enciam nuestra
    ///                 1.2.2.) Si no somos un evi sin techo, o no estamos en un muero en edicion, nos apartamos separandonos del evi que viene con el puntero de usuario para no superponernos y
    ///                         que los dos evis se queden superpuestos sin que puedan diferenciarse
    ///         
    /// </summary>
    public void controlaTriger(GameObject elOtro)
    {
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////
        // 1.) Colision con el "PunteroUsuario". 
        if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario)
        {
            /// /////////////////////////////////////////////////////////////////
            /// /////////////////////////////////////////////////////////////////
            //  1.1.) En el caso de que el puntero de usuario venga sin arrastrar ningun evi, no actuamos de ninguna manera
            if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre == null)
            {
                // No hacemos nada
                if (DatosGlobal.niveDebug > 1000)
                {
                    Debug.Log("Estoy en ScriptCtrlBaseDeEvi => controlaTriger en colosion con PunteroUsuario SIN arrastre : \n" +
                      " - Con name : " + this.gameObject.name +
                      " - Con name del collisio : " + elOtro.gameObject.name +
                      " -***-  Con dameSubTipoElementIntf : " + GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() +
                      " -***-  Con name : " + this.name
                      );
                }
            }  // Fin de - if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre == null)
            /// /////////////////////////////////////////////////////////////////
            /// /////////////////////////////////////////////////////////////////
            // 1.2.) En el caso de que el puntero de usuario colisione con nosotros, arrastrando un evi, diferenciamos dos casos
            else
            {
                // Colisionamos con el puntero, pero el que nos interesa tratar es el evi que el puntero arrastra
                GameObject evi_que_elOtro_arrastra = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre.gameObject;

                // 1.2.1.) Si somos un evi sin techo y estamos en un muro de edicion; lo dejamos pasar. Esto es porque si suelta ese evi sobre nuestro boton de info, debemos
                //         cambiar el tipo de dato del sin techo y adoptar como tipo de dato el concepto que han arrastado enciam nuestra
                //          - Ojo, incluimos (evi_que_elOtro_arrastra != this) para evitar actuar cuando es este el evi que el puntero esta arrastrando
                if ((this.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &&
                    (this.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &&
                    (this.gameObject.transform.parent.gameObject == ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo) &&
                    (this.gameObject.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion) &&
                    (evi_que_elOtro_arrastra != this.gameObject))
                {
                    // No hacemos nada

                    //  1.2.1.1.) Si los dos evis se han superpuesto, preguntamos si se desea modificar el tipo de dato del sin techo

                    float unbral_distancia = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi / 10f;
                    Vector3 distancia_entre_evis = new Vector3();
                    distancia_entre_evis = this.transform.localPosition - evi_que_elOtro_arrastra.transform.localPosition;
                    if (Math.Abs(distancia_entre_evis.magnitude) < unbral_distancia)
                    {
                        // Superponemos los dos evis hasta que sepamos si el usuario quiere cambiar el tipo de dato del sin techo o no
                        evi_que_elOtro_arrastra.transform.localPosition = this.transform.localPosition;
                        // Para preguntar si quiere cambiar el tipo de datos del sin techo o no, activamos los botones correspondientes en las opciones
                        // Primero tenemos que identificar los objetos "Btn_BaseDeEvi_N3_1_op_Editar_Grabar" y "Btn_BaseDeEvi_N3_1_op_Editar_Salir" que estan en 
                        //      this.gameObject => Btn_BaseDeEvi_N1_Opciones => Btn_BaseDeEvi_N2_Caja_opciones
                        GameObject Btn_BaseDeEvi_N1_Opciones = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(this.gameObject, "Btn_BaseDeEvi_N1_Opciones");
                        GameObject Btn_BaseDeEvi_N2_Caja_opciones = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(Btn_BaseDeEvi_N1_Opciones.gameObject, "Btn_BaseDeEvi_N2_Caja_opciones");


                        GameObject Btn_BaseDeEvi_N3_1_op_Editar_Grabar = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(Btn_BaseDeEvi_N2_Caja_opciones.gameObject, "Btn_BaseDeEvi_N3_1_op_Editar_Grabar");
                        GameObject Btn_BaseDeEvi_N3_1_op_Editar_Salir = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(Btn_BaseDeEvi_N2_Caja_opciones.gameObject, "Btn_BaseDeEvi_N3_1_op_Editar_Salir");

                        Btn_BaseDeEvi_N2_Caja_opciones.SetActive(true);  // Se activara al entrar en edicion
                        Btn_BaseDeEvi_N3_1_op_Editar_Grabar.SetActive(true);  // Se activara al entrar en edicion
                        Btn_BaseDeEvi_N3_1_op_Editar_Salir.SetActive(true);  // Se activara al entrar en edicion

                        en_espera_resp_edicion = true;
                        evi_pendiente_cambio_tipo_dato_sinTecho = evi_que_elOtro_arrastra;

                        //  1.2.1.1.1.) Si la respuesta es NO, separamos los dos evis sin cambio en ninguno de ellos
                        //  1.2.1.1.2.) Si la respuesta es SI, modificamos el tipo de dato del sin techo, haciendo que su tipo de dato sea el concepto 
                        //              que esta superpuesto y luego separamos los dos evis para que no queden superpuestos
                        // OJOOO estas acciones se ejecutan desde "Script_BaseDeEvi_N2_1", en " case "Btn_BaseDeEvi_N3_1_op_Editar_Grabar":" y "case "Btn_BaseDeEvi_N3_1_op_Editar_Salir":"
                        //      respectivamente.



//                        float distancia_a_mantener = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi * 2f * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;
//                        Vector3 separacion_evis = new Vector3(distancia_a_mantener, distancia_a_mantener, 0f);
//                        Vector3 nueva_posicion_tipo_dato = new Vector3();
//                        nueva_posicion_tipo_dato = evi_que_elOtro_arrastra.transform.localPosition + separacion_evis;
                    //    evi_que_elOtro_arrastra.transform.localPosition = nueva_posicion_tipo_dato;

                    }




                }  // FIn de - if(( this.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) && ...
                // 1.2.2.) Si no somos un evi sin techo, o no estamos en un muero en edicion, nos apartamos separandonos del evi que viene con el puntero de usuario para no superponernos y
                //         que los dos evis se queden superpuestos sin que puedan diferenciarse
                else
                {
                    // De lo que tenemos que apartarnos es del evi base que esta arrastrando el puntero. Por eso, lo que eniamos a "apartaEviQuePaso" es el evi
                    // que arrastra el puntero
                    apartaEviQuePaso(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre.gameObject, this.gameObject);
                    //                    apartaEviQuePaso(elOtro.gameObject, this.gameObject);
                }

            }  // Fin de - else - de - if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre == null)
        } // Fin de - if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario)
        else
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Estoy en ScriptCtrlBaseDeEvi => OnCollisionEnter en colosion con un elemento distinto de PunteroUsuario : \n" +
                  " - Con name : " + this.gameObject.name +
                  " - Con name del collisio : " + elOtro.gameObject.name +
                  " -***-  Con dameSubTipoElementIntf : " + GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() +
                  " -***-  Con name : " + this.name
                  );
            }
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : apartaEviQuePaso. Aparta el evi "eviQueSeAparta" (normalmente este evi) cuando viene otro empujando
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-07
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     - GameObject eviQueViene; Es el evi que viene y que debe apartar al otro para no superponerse
    ///     - GameObject eviQueSeAparta; Es el evi tenemos que apartar para dar paso al que viene
    /// Variables de salida :
    ///     No devuelve nada, solo aparta al gameobject "eviQueSeAparta" para dejar pasar al "eviQueViene" 
    /// Observaciones:
    ///     - 
    ///     - Pasos a realizar
    ///         1.) Calculamos el vextor que une las posiciones de los dos gameobject (su modulo es la distancia d). Osea el vector que va desde "eviQueViene" a "eviQueSeAparta".
    ///             La direccion y el sentido del movimiento del evi que se aparta, debe ser la que este vector indica. EL módulo lo calculamos a continuacion.
    ///         2.) Como estamos usando un colaider esferico ( de rario r= ), el modulo del vector de desplazamiento debe ser
    ///                 M = 2r-d;  Donde : M es Modulo del desplazamiento - r es radio de la esfera del colaider - d es la distancia entre los gameobject
    ///         
    /// </summary>
    public void apartaEviQuePaso(GameObject eviQueViene, GameObject eviQueSeAparta)
    {
        Vector3 posicion_eviQueSeAparta = new Vector3();
        posicion_eviQueSeAparta = eviQueSeAparta.transform.localPosition;
        Vector3 posicion_eviQueViene = new Vector3();
        posicion_eviQueViene = eviQueViene.transform.localPosition;
        Vector3 vector_Aparta_Viene = new Vector3();
        vector_Aparta_Viene = posicion_eviQueSeAparta - posicion_eviQueViene;
        if (vector_Aparta_Viene.magnitude == 0f)
        {   // Si se han superpuesto lo solucionamos
            vector_Aparta_Viene = new Vector3((ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi * 2f), (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi * 2f), 0); 
        }

        float distancia = vector_Aparta_Viene.magnitude;
        float distancia_a_mantener = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi * 2f * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;

        float distancia_a_mover = 0f;
        Vector3 vector_de_movimiento = new Vector3();
        Vector3 nueva_posicion_eviQueSeAparta = new Vector3();
        if (distancia < distancia_a_mantener)
        {
            distancia_a_mover = distancia_a_mantener - distancia;

            vector_de_movimiento = vector_Aparta_Viene.normalized * distancia_a_mover;

            nueva_posicion_eviQueSeAparta = posicion_eviQueSeAparta + vector_de_movimiento;

            transform.localPosition = nueva_posicion_eviQueSeAparta;

            ctrlInterfaz.GetComponent<ScriptGestionaInterfaz>().haz_que_suene(ctrlInterfaz.GetComponent<ScriptGestionaInterfaz>().altavoz_uauario, ctrlInterfaz.GetComponent<ScriptGestionaInterfaz>().evi_aparta_evi);
        }

    }  // FIn de -  public void apartaEviQuePaso(GameObject eviQueViene, GameObject eviQueSeAparta)

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
    /// Metodo (corrutina) : Envia un fichero al servidor
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-03-05
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host)
    /// Variables de salida :
    /// Observaciones:
    ///         - Genero esta funcion segun las instruciones que encuentro en "https://www.youtube.com/watch?v=KN98oLD3n2g"
    /// </summary>
    IEnumerator IEnu_enviaFichero(string nombre_origen, string extension, string nombre_enKee, string path_enKee, string codigo_de_envio_Kee)
    {
        //                        byte[] inage = File.ReadAllBytes(path);  iconoImgPorDefecto

        //         string urlDestino = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host + "/kee/ClientesServWeb/entradaFichero.php";
        string urlDestino = DatosGlobal.Dks_madre_host + ScriptConexionDKS.sufijoAccesoAFicherosDks;

        WWW localFIle = new WWW("file://" + path_enKee + nombre_enKee + extension);

        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Desde enviaFichero IEnumerator - con nombre_origen " + nombre_origen
                + " - extension : " + extension
                + " - nombre_enKee : " + nombre_enKee
                 + " - path_enKee : " + path_enKee
                + " - urlDestino : " + urlDestino); }

        yield return localFIle;

        //        nombre_enKee = nombre_enKee + ".jpg";
        //        nombre_enKee = nombre_enKee + extension;
        // nombre_enKee = nombre_enKee;
        WWWForm postForm = new WWWForm();
        postForm.AddField("codigo_de_envio_Kee", codigo_de_envio_Kee);
        postForm.AddField("nombre_enKee", nombre_enKee);
        postForm.AddField("nombre_origen", nombre_origen);
        postForm.AddField("extension", extension);

        postForm.AddField("Clave_usr_que_envia", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrKey);  // Del usuario que realiza la operacion
        postForm.AddField("Localizacion_usr_que_envia", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrHost);
        postForm.AddField("Clave_agente_que_envia", DatosGlobal.Dks_madre_key);  // DEl DKS al que esta conectado la interfaz KEE
        postForm.AddField("Localizacion_agente_que_envia", DatosGlobal.Dks_madre_host);
        postForm.AddField("Clave_kee_que_envia", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().esta_iterfaz_Key);  // DEl DKS al que esta conectado la interfaz KEE
        postForm.AddField("Localizacion_kee_que_envia", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().esta_iterfaz_Host);
        //            postForm.AddBinaryData(nombre_enKee, localFIle.bytes, "image/png");
        //        postForm.AddBinaryData("fichero_enviado", localFIle.bytes, "image/jpg");
        //        postForm.AddBinaryData("file", localFIle.bytes, "image/jpg");
        postForm.AddBinaryData("file", localFIle.bytes, extension);

        WWW upload = new WWW(urlDestino, postForm);
        yield return upload;

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Desde enviaFichero IEnumerator - ya envie el fichero con nombre_origen"+ nombre_origen+ " - extension : "+ extension + " - nombre_enKee : "+ nombre_enKee + " - path_enKee : " + path_enKee);
        }


        // Borramos el fichero del directorio de Temporales, para no juntar porqueria
        try
        {
            string nombreFichero = path_enKee + nombre_enKee + extension;
            if (DatosGlobal.niveDebug > 50)
            {
                Debug.Log("Desde enviaFichero IEnumerator - Borrando el fichero : "+ nombreFichero);
            }
            File.Delete(nombreFichero);
        }
        catch (DirectoryNotFoundException dirNotFound)
        {
            Debug.Log("Desde enviaFichero IEnumerator - error al borrar el fichero");
            Console.WriteLine(dirNotFound.Message);
        }

        // /////////////////////////////////////////////////////////////////
        // PENDIENTE MAFG 2022-03-22 Habria que desarrollar recursos para controlar la respuesta del servidor DKS
        // al envio del fichero. 
        // En principio el DKS contesta con un concepto respuesta en el que dice si ha ido todo bien o ha existido 
        // algun problema (ver: aprtado 7.3 en "entradaFichero.php" em el directorio "kee/ClientesServWeb" del DKS). 
        // Aqui deberiamos haber anotado (quezas mediante una solicitud) el envio del fichero y dejar a alguien a cargo
        // de recibir la respuesta, quizan mediante una rama en el DAUS de relaciones externas (para nvio de ficheros u otros)
        // - Al enviar el fichero se anotaria en esta rama del DAUS y al llegar se anotaria la recepcion de la respuesta
        //  Cando me ponga a hacerlo vere como resuelvo. Por ahora me pongo con cosas mas urgentes

    }  // Fin de - IIEnumerator enviaFichero(string nombre_origen, string extension, string nombre_enKee, string path_enKee)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita una imagen de ayuda a interfaz a un servidor externo Y la pone como textura en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host)
    /// Variables de salida :
    /// Observaciones:
    ///         OJO  pone como textura de DOS GAMEOBJECT en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    ///         - OJOOO estamos en un evi base. Para gestionar la ayuda a interfaz de los fractum HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "SctCtrlEviTipo_01Fractum"
    /// </summary>
    IEnumerator traeTextura_imagen_AyuIntf(string origenDeLaImagen)
    {
        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Trayendo imagen origenDeLaImagen : "+ origenDeLaImagen); }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(origenDeLaImagen);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Error desde traeTextura_imagen_AyuIntf : " + www.error); }
        }
        else
        {
            textura_imagen_AyuIntf = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // Ponemos la textura recibida y almacenada en "textura_imagen_AyuIntf" en el material correspondiente a la imagen de fondo
            material_imagen_AyuIntf.SetTexture("_MainTex", textura_imagen_AyuIntf);  // Le asinamos la textura al material
                                                                                     // Ponemos el material en el objeto contenedor, Que es quien contiene la imagen de fondo
            this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.GetComponent<MeshRenderer>().material = material_imagen_AyuIntf;

            // Vamos con la imagen que acompaña al icono (Es la misma fuente que la anterior, pero se visualiza mas pequeña cuando marcamos el boton de información)
            textura_ImgAyuIntfBaseDeEvi = textura_imagen_AyuIntf; // Utilizamos la misma imagen para el fondo y para la que acompaña al icono
            material_ImgAyuIntfBaseDeEvi.SetTexture("_MainTex", textura_ImgAyuIntfBaseDeEvi);  // Le asinamos la textura al material
            this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.GetComponent<MeshRenderer>().material = material_ImgAyuIntfBaseDeEvi; // Asignamos el material al objeto

            // Modificamos el estado para actuar en consecuencia
            estadoImgAyudaIntf = "texturaCargada";
        }
    }  // Fin de - IEnumerator traeTextura_imagen_AyuIntf(string origenDeLaImagen)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita una imagen de icono de concepto a un servidor externo
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host)
    /// Variables de salida :
    /// Observaciones:
    ///         - OJOOO estamos en un evi base. Para gestionar la ayuda a interfaz de los fractum HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "SctCtrlEviTipo_01Fractum"
    /// </summary>
    IEnumerator traeTextura_icono_AyuIntf(string origenDeLaImagen)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(origenDeLaImagen);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Error desde  traeTextura_icono_AyuIntf : " + www.error); }
        }
        else
        {
            textura_icono_AyuIntf = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // Ponemos la textura recibida y almacenada en "textura_icono_AyuIntf" en el material correspondiente al icono del concepto
            material_icono_AyuIntf.SetTexture("_MainTex", textura_icono_AyuIntf);  // Le asinamos la textura al material
            this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<MeshRenderer>().material = material_icono_AyuIntf; // Asignamos el material al objeto

            // Modificamos el estado para actuar en consecuencia
            estadoIconoAyudaIntf = "texturaCargada";
        }
    }  // Fin de - IEnumerator  traeTextura_icono_AyuIntf(string origenDeLaImagen)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita audio de ayuda a interfaz de un concepto a un servidor externo
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDelAudio  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host)
    /// Variables de salida :
    /// Observaciones:
    ///         - OJOOO estamos en un evi base. Para gestionar la ayuda a interfaz de los fractum HAY OTROS ITERADORES CON EL MISMO NOMBRE EN "SctCtrlEviTipo_01Fractum"
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
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Error desde  traeAudio__AyuIntf : " + www.error); }
        }
        else
        {
            AudioClip_AyuIntf = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;

            //Aplicamos el audio clip al audiosource
            this.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<Script_BaseDeEvi_N1_Info>().audio_AyuIntf = AudioClip_AyuIntf;

            // Modificamos el estado para actuar en consecuencia
            estadoIconoAyudaIntf = "texturaCargada";
        }
    }  // Fin de - IEnumerator  traeTextura_icono_AyuIntf(string origenDeLaImagen)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Configura el estado inicial del canvas del evi base
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-10-10
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones:
    ///         - Esto hay que hacerlo en esta corrutina porque cuando se genera el evi base, todabia no se han generado el contenedor ni el contenido y por 
    ///         tanto, no se puede configurar convenientemente
    /// </summary>
    IEnumerator configuraInicioCanvasEviBase()
    {
        // Esperamos un par de cuadros para empezar
        yield return null;
        yield return null;

        // Cargamos los datos de ayuda a interfaz que van en el canvas del Evi Base
        //        textoInfoCanvasEviBase = txt_nombre_AyuIntf + " : " + txt_descripcion_AyuIntf;
        textoInfoCanvasEviBase = txt_nombre_AyuIntf;
        caja_texInfoCanvasEviBase.text = textoInfoCanvasEviBase;

        // Configuramos de inicio el canvas del evi base
        caja_texInfoCanvasEviBase.gameObject.SetActive(false);
        este_Panel_Input_Text_SinTecho.gameObject.SetActive(false);

        // Tenemos que mirar la naturaleza del fractum que contiene el contenedor del evi base, para ver si es un sin techo
        string miNaturaleza = this.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Estoy en ScriptCtrlBaseDeEvi => configuraInicioCanvasEviBase : \n" +
              " - Con miNaturaleza : " + miNaturaleza +
              " - Con miNaturaleza : " + miNaturaleza +
              " -***-  Con dameSubTipoElementIntf : " + GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() +
              " -***-  Con name : " + this.name
              );
        }


        if (miNaturaleza == ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho)
        {
            // Cargamos los texto T del sin techo, si es un sin techo
            texto_T_deSinTechoCanvasEviBase = nodo_E_ContEnBaseDeEvi.FirstChild.LastChild.InnerXml;
            caja_Tex_T_deSinTechoCanvasEviBase.text = texto_T_deSinTechoCanvasEviBase;  // Actualizamos el valor de la variable que ira al elemento T del KDL
            este_Input_Text_T_deSinTecho.text = texto_T_deSinTechoCanvasEviBase;  // Actualizamos el valor para que salga por defecto en la edicion del sn techo

            caja_Tex_T_deSinTechoCanvasEviBase.gameObject.SetActive(true);
        }
        else
        {
            caja_Tex_T_deSinTechoCanvasEviBase.gameObject.SetActive(false);
        }

    }  // Fin de - IEnumerator  traeTextura_icono_AyuIntf(string origenDeLaImagen)



    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   LATEUPDATE
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  LateUpdate() 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-07-08
    /// Ultima modificacion :
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    ///         - Este metodo se ejecuta despues de todos los updates, por lo que si hay que hacer algun tipo de actualizacion global del objeto, habria que hacerla aqui
    /// </summary>
    void LateUpdate()
    {
        // //////////////////////////////////////////////////////////////////
        // 1.) Gestionamos el cambio de modo. Para ello atendemos a la naturaleza del padre de este eviBase, en la gerarquia de unity
        // 1.1.) Si existe el padre de este evi base, cambiamos el modo atendiendo a donde se aloja en evi:
        if (this.transform.parent)
        {
            // 1.1.1.)  Si esta alojado en la tramoya (el padre es la ramoya ) => modo = modoElemItf_enTramoya
            if (this.transform.parent.parent.tag == "Tramoya")  //el evi estara en un telon que estara en la tramoya
            {
                this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_enTramoya;
            }
            // 1.1.2.)  Si esta alojado en un muro (el padre es el muro ) 
            else if (this.transform.parent.tag == "MuroDeTrabajo")
            {
                if (this.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_navegacion)  //el evi estara en un telon que estara en la tramoya
                {
                    // 1.1.2.1.)  SI el muro esta en modo navegacion => el evi se pone modo = modoElemItf_navegacion
                    // Pero si es un evi raiz de un arbol de edicion, no lo pasamos a navegacion. En su muro debe seguir en modo modoElemItf_cabezaEdicion
                    if (this.gameObject.GetComponent<ScriptDatosElemenItf>().modo != ScriptDatosElemenItf.modoElemItf_cabezaEdicion)
                    {
//                       this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_navegacion;
                    }
                }
                else if (this.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
                {
                    // 1.1.2.2.) SI el muro esta en modo edicion => el evi se pone modo = modoElemItf_edicion
                    this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_edicion;
                }
            }  // Fin de - else if (this.transform.parent.tag == "MuraDeTrabajo")
        }// Fin de - if (this.transform.parent)
    }  // FIn de - void LateUpdate()

    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   TRIGGERS
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  OnTriggerEnter(Collider elOtro) 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-06
    /// Ultima modificacion :
    /// Parametros de entrada :
    ///         - Collider elOtro : es el ogbjeto (que debe tener reigidbody), que choca con esta base de evi
    /// Parametros de salida :
    /// Observaciones:
    /// </summary>
    void OnTriggerEnter(Collider elOtro)
    {

        controlaTriger(elOtro.gameObject);


    }  // Fin de - void OnTriggerEnter(Collider elOtro)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : OnTriggerStay(Collider elOtro) 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-08
    /// Ultima modificacion :
    /// Parametros de entrada :
    ///         - Collider elOtro : es el ogbjeto (que debe tener reigidbody), que esta chocando con esta base de evi
    /// Parametros de salida :
    /// Observaciones:
    /// </summary>
    void OnTriggerStay(Collider elOtro)
    {

        controlaTriger(elOtro.gameObject);


    }  // Fin de - void OnTriggerEnter(Collider elOtro)

}  // Fin de - public class ScriptCtrlBaseDeEvi : MonoBehaviour {
