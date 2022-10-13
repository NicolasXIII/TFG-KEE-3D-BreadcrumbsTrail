  using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;
using TMPro;
using UnityEngine.UI;


// 			Debug.Log ("Desde cumplimentaDatosEviTipo00 dentro del for Con i = "+i.ToString());
// /////    BtnMenu_N2_1_Escena_LigaMuro entidad

public class ScriptDatosInterfaz : MonoBehaviour {


    // Nicolas
    
    public GameObject BtnBreadcrumbsTrails;
    
    //public GameObject BtnBreadcrumbsTrails;
    public Vector3 posicion_BtnBreadcrumbsTrails;
    public Vector3 escala_BtnBreadcrumbsTrails;
    public Vector3 escala_BtnBreadcrumbsTrails_Activado;
    public Quaternion rotacion_BtnBreadcrumbsTrails;
    /*
    public GameObject BtnBreadcrumbsTrails_Contenedor;
    public Vector3 posicion_BtnBreadcrumbs_N2_Contenedor;
    public Vector3 escala_BtnBreadcrumbs_N2_Contenedor;
    */

    /// ////////////////////////////////////////////////
    /// Datos de Este KEE
    /// Cada implantacion que se haya generado debe tener su concepto asociado.
    /// El responsable de la aplicacion (uien la genero, asoa el suministrador de software o la store o quien la haya generado) debe generar
    /// un concepto donde esta la informacion de la misma, tipo de aplicacion, version, derechos, control de configuracion, etc...
    ///   OJO, este KEE se asocia a un concepto con este key "esta_iterfaz_Key" y host "esta_iterfaz_Host". Si pedimos el getDetails, el 
    ///   DKS correspondiente, podra darnos mas informacion (version, fabricante, propietario, tipo de licencia,...)

    public string esta_iterfaz_Key;
    public string esta_iterfaz_Host;

    /* *************
    Esta interfaz estara definida por su key "esta_iterfaz_Key" y su host "esta_iterfaz_Host". Pero debemos tener en cuenta qu generará conceptos, como consultas, 
    ficheros que deben identificarse como suyos, etc. Esta interfaz no es un DKS en es un DKS en el sentido de que no tiene dominio de conocimiento asociado, pero si tendrá
    que generar conceptos (consultas, etc) que seran conceptos y tendran que tener un key y un host asociado (en general seran conceptos efimeros).
        Estos conceptos deberan tener un key asociado a esta interfaz (eso no es un problema, lo solucionamos con la firma de tiempo y un ordinal) y un host. El host es 
    mas problematico, ya que tiene que ser unico en la red. 
        La solucion que he tomado es que como host, cada interfaz debe tener un nombre de dominio que sea subnombre de dominio de su "esta_iterfaz_Host". Como el "esta_iterfaz_Key"
    es unico en cada "esta_iterfaz_Host", se puede tomar como subdominio, sin peligro a solaparse. No obstante, he decidido poner un subdominio intermedio que los administradores de 
    dominio no deben usar para otra cosa que no sea esto, con lo que evitamos el que se pedan definir subdominios que coincidan co "esta_iterfaz_Key".
        Si el subdominio que definimos es "efim", para esta interfaz podemos definir un string que le sirva como host para los conceptos que genere. Esta seria :

        host_efimero_de_interfaz = esta_iterfaz_Key + ".efim." + esta_iterfaz_Host

    *************** */
    public string host_efimero_de_interfaz;

    // IDENTIFICACION CONCEPTO GENERRADO POR ESTE KEE
    // Esta interfaz puede genrar conceptos (consultas a DKS, DAUS u otros) estos conceptos, aunque muchas veces sean efimeros, necesitan un Key y un Host. Para poder generar
    // dicho key y host, se definen las variables siguientes. "host_efimero_de_interfaz" y "numDeConceptosGenerados". Estas variables se utilizan para generar los identificadores
    // de dichos conceptods como sigue:
    //          - Para qel Key de los conceptos generados por esta interfaz => key = numDeConceptosGenerados
    //          - Para qel host de los conceptos generados por esta interfaz => host = host_efimero_de_interfaz

    /// ////////////////////////////////////////////////
    /// DausInterfaz
    /// Este string es un KDL que contiene el elemento escena con l configuracion que la mantiene el usuario
    ///     - Cada vez que se abre, cierra o modifica un evi o rama, se actualiza el DausInterfaz segun corresponda.
    ///     este daus tiene por tanto toda la infomacion de la escena y su configuracion, ramas, muros, evis, etc...
    public XmlDocument DausInterfaz = new XmlDocument();

    /// ////////////////////////////////////////////////
    /// Datos de USUARIO
    public string usrKey;
    public string usrHost;
    public string usrNombre;
    public string usrPasword;

    /// ////////////////////////////////////////////////
    /// PARAMETROS DE EJECUCION
    /// 
    public int numDeFrame;  // Para saber en que frame desde que arranco la aplicacion nos encontramos
    public int numDeConceptosGenerados;  // Esta interfaz puede generar conceptos (generalmente efimeros, de DAUS u otros). Este es un indice que se va incrementando para identificar
                                         // de forma univoca cada uno de estos conceptos

    private int periodoRefractarioBotonMouse;  // (en milisegundos)  En algunas ocasiones, despues de obtener una pulsacion sobre el boton del mouse, es necesario que durante un tiempo determinado, aunque se pulse 
                                               // el mouse, esta pulsacion no tenga conseguencias (periodo refractario) este parametro sirve para indicar (en milisegundos) cuanto debe ser este tiempo
    private DateTime iniciPerRefractBotonMouse;  // Para almacenar el momento de inicio de cualquier periodo de refraccion para el click del mouse
    private bool enPeriodoDeRefraccion;  // true : el boton del mouse esta en periodo de refraccion - false : el boton del mouse NO esta en periodo de refraccion

    public long ordinal_de_identificador_unico;  // Este ordinal se utiliza para generar identificadores unicos en todo el KEE. Cada vez que se utiliza, debe incrementarse (antes de usarlo)

    // Parametros de test
    public int test_general;
    public int test_entradaEnFuncion;
    public int test_trigers;

    /// ////////////////////////////////////////////////
    /// PARAMETROS DE LA INTERFAZ
    /// Todo esta en unidades UNIY. Todo se ajunsta para que la camara tome desde su posicion inicial un muro completo
    /// - A esa misma distancia se iran activando los muros consecutivamente
    /// - El muro de usuario va a una distancia fija de tal manera que siempre debe encajar en ell marco de la imagen que capta la camara y 
    /// asi de esa forma enmarca la imagen siempre en la misma posicion
    /// 
    /// TODOS LOS ELEMENTOS se colocan atendiendo al game Obgect Usuario
    /// - La camara viaja a una distancia fija del usuario
    /// - EL muro de usuario viaja a una distancia fija del usuario y por lo tanto de la camara
    /// - Los muros de trabajo estan fijos en la escena y el usuario va (hacia delante y atras)
    /// atravesando los muros para pasar de uno a otro
    /// 

    /// ////////////////////////////////////////////////
    /// INVENTARIO DE ELEMENTOS EN LA INTERFAZ
    /// En estas listas estan referenciados todos los elementos multiples de la interfaz de usuario
    /// 

    /// ///////////////////////////////////////////////
    /// Elementos de la interfaz - 
    //   ctrlInterfaz (por ahora no tenemos lista 2021-01-30 solo hay uno y es el propietario de este script)
    //   camara (por ahora no tenemos lista 2021-01-30)
    //   focoLuz (por ahora no tenemos lista 2021-01-30)
    //   puntero (por ahora no tenemos lista 2021-01-30)

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Para el USUARIO (el usuario siempre reside en una rama, sera por tanto siempre hijo de una de ellas, en concreto "rama_Activo")
    public GameObject Usuario;
    public GameObject PunteroUsuario;
    public float factorEscalaEntornoUsuario;  // Este parametro sirve para ajustar la escala de los elementos que van con el usuario
    public float escalaEntornoUsuario;  // Este parametro sirve para ajustar la escala de los elementos que van con el usuario

    public GameObject MainCamera;
    public GameObject Focoluz;

    // Habiamos decidido trabajar con un solo canvas, en el que escribem todos los gameobjet que necesiten escribir(2021-10-01). Pero al final
    // lo que hago es dejar el canvas general para algunas cosas generales y poner un canvas local en cada evi base que sontenga la información 
    // de dicho evi base 
    //  - En el canvas general "CanvasGeneral" iran :
    //          - "Text_ConsultaUsr" : para elboton de consulta a usuario
    //
    //              - Condiciones de uso :
    //                  + Quien lo utiliza recoloca el texto donde le convenga. La opcion mas adecuada es utilizar la posicion del raton como referencia
    //                      ya que el texto esta ancaldo (referenciado mediante un eje de coordenadas con 0,0 abajo a la izquierda), que es la misma referencia
    //                      que utiliza el raton. Si se utiliza otro ptipo de localizacion, hay que tener en cuenta que el (0,0) es la posicion inferior
    //                      izquierda de la pantalla
    //                  + Quien lo utiliza, pone el texto que necesite y despues deja el texto en blanco
    //
    //  - En el canvas de evi base "CanvasEviBase" iran :
    //          -  "textInfoElemento"
    //          -  "Text_T_deSinTecho"
    //          -  "Panel_Input_Text_SinTecho"
    //
    public Canvas CanvasGeneral;
    public float factorEscalaGeneralCanvas;  // Este parametro sirve para ajustar el tamaño de los elementos del canvas
    public float FactorOffsetAvanceCanvas;  // Este parametro sirve para ajustar la distancia del canvas a la camara de forma que quede por delante del muro de trabajo
                                        // para que los elementos del canvasa se vean por delante
                                        // OJOOOO los calculos de escala y posición de los elementos del canvas se hacen sin tener en cuenta este offset,  yq que 
                                        // en principio no es significativo, si lo fuera habria que tenerlo en cuenta
    //    public RectTransform Panel_Input_Text_SinTecho;
    public GameObject Panel_Input_Text_SinTecho;
        // Para el texto completo de canvas general. Es un espacio para texto, unico para toda la interfaz, para mostrar textos de tamaño de los sin techo u otros
        // textos que necesiten espacio amplio para ser visualizados
    public GameObject PanelCanvasCompleto;
    public TextMeshProUGUI TextCanvasGeneralCompleto;
    public Button BotonCerrarPanelCanvasCompleto;

    //      public TextMeshProUGUI textInfoElemento;
    //    public TMP_InputField Input_Text_T_deSinTecho;
    //    public Button Btn_Guardar_Text_T_deSinTecho;
    //    public Button Btn_Cancelar_Text_T_deSinTecho;

    //    public Canvas CanvasEviBase;



    public GameObject objetoEnArrastre;  // Esta variable indica cual de tosdos los elementos de interfaz esta arrastrndo el usuario (teoricamente sera unno solo cada vez
                                         // aunque podremos poner mas con un array list PENDIENTE MAFG 2021-05-30)

    // Los niveles de activacion de los punteros se definen en "ClassPuntero"

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Para el MURO DE USUARIO (el muro de usuario siempre esta contenido en el usuario)
    public GameObject MuroUsuario;
    public GameObject PunteroMuroUsuario;  // Puntero que trabaja en el muro de usuario (esta siempre activo por ahora 2021-06-25) 

    // Para los botones de consulta que apareceran en el muro de usuario cuando haya que realizar consultas al usuario
    // Normalmente estas consultas se hacen (en modal) enfocando al usuario en la respuesta, sin permitir que haga ninguna otra cosa hasta que responda
    public GameObject Btn_Consulta_Si_No;  // Este boton sirve para hacer preguntas culla respuesta en "SI" o "NO". Ejemplo "¿realmente desea salir?" => "Si" o "No"

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para todos los ELEMENTOS DE INTERFAZ (ramas, muros, evis ) son elementos de interfaz
    private int ultimoIdElementIntf;
        // No hay lista general de elementos de interfaz, cada elemento de interfaz esta en su lista correspondiente

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para las RAMAS
    public GameObject Rama;
    public List<GameObject> ListaRamas;
    private int ultimoIdElementTipoRama;

    // Para la definicion del arbol de la interfaz
    // La lista que contiene el arbol de ramas es
//    public List<int[]> arbolIntf;  OJOOO Por ahora voy registrando el arbol con las listas de hijos de los elementos de interfaz
        // El primer entero es el identificador de relacion
        // El segundo elemento identifica el padre de la relacion (coincide con el idRama de la rama padre)
        // El tercer elemento identifica el hijo de la relacion (coincide con el idRama de la rama hijo)

    public bool arbolRecienGenerado;  // T¡Esta variable se pone a true cuando el arbol haya sido generado, para que el usuario pueda colocarse en la rama raiz
                                      // cuando el usuario se coloque en la raiz para iniciar, esta variable vuelve a false
    public int numRamas;
    public int ultimoIdArbolIntf;
    public int idElemItfRama_Activo;
    public GameObject raizActual;  // La raiz del arbol puede cambiar, ya que podemos convertir la raiz en una rama de una gaiz que generemos, padre de esta pudiendo contener otras ramas
    public GameObject rama_Activo;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para los MUROS DE TRABAJO
    public GameObject Muros;
    public GameObject muro;
    public List<GameObject> ListaMuros;
    private int ultimoIdElementTipoMuro;
    public GameObject muro_Activo;
    public int numMuros;  // Sirve entre otras cosas para nombrarlos de forma univoca e identificarlos en desarrollo
        // El modo de los elementos (en este caso muros, se identifica por el color (navegacion blanco, edicion amarillo)
        // La activacion de los elementos se identifica por su nivel de transparencia (Activo mas opaco, inactivo mas pransparente)
    public Color colorMuroNavegacion;
    public Color colorMuroEdicion;
    public float nivelTransparenciaMuroActivo;  
    public float nivelTransparenciaMuroInactivo;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para los EVIs
    public List<GameObject> ListaEvis;
    private int ultimoIdElementTipoEvi;
    public int numEvis;  // Sirve entre otras cosas para nombrarlos de forma univoca e identificarlos en desarrollo

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Tramoya y telones de usuario
    public GameObject Tramoya; // Es el espacio en el que van a aparecer los telones
    public GameObject PunteroTramoya;  // Es el puntero que trabaja en la tramoya, se activa y desactiva (alternandeose con el puntero de usuario, segun el usuario este en la tramoya o no
    public GameObject telon;
    public List<GameObject> ListaTelones;
    private int ultimoIdElementTelones;
    public int numTelones;  // Sirve entre otras cosas para nombrarlos de forma univoca e identificarlos en desarrollo

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para las SOLICITUDES
    public GameObject Solicitud;
    public List<GameObject> ListaSolicitudes; // PENDIENTE. No tengo claro si cada agente sera un gameobjet con su script correspondiente.
    private int ultimoIdElementTipoSolicitud;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// Para los AGENTES de interfaz
    //    public GameObject Agente;  // Tenemos PENDIENTE la generacion de los agentes (mafg 2021-04-05)
    public List<GameObject> ListaAgentes; // PENDIENTE. No tengo claro si cada agente sera un gameobjet con su script correspondiente. 


    /// ////////////////////////////////////////////////
    // TIPOS DE ELEMENTOS DE INTEFAZ  (ver "ScriptDatosElemenItf") los tipos de elementos de interfaz se declaran alli como estaticos 

    // /// Elementos funcionales generales

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // /// Prefijos de string
    // De los que estan en Resources
    public string locIconosKlw;
    public string locIconosExternos;
    public string locImagenesKlw;
    public string locImagenesExternos;
    public string locAudiosKlw;
    public string locAudiosExternos;
    public string locFicherosKlw;
    public string locFicherosExternos;
    public string locFicherosKlw_conResources;
    public string locFicherosExternos_conResources;

    public string locFicherosTemporales; 

    public string nombre_Fich_Resources; 

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Parametros de ESCALA Y LOCALIZACION

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Parametros de PANTALLA del dispositivo donde se muestra la aplicacion
    public int pixels_x_Pantalla;   // Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public int pixels_y_Pantalla;   // Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public int origenVentanaPixels_x;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor
    public int origenVentanaPixels_y;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor

    public static float ancho_x_Pantalla = 100f;  // Ver observaciones en la documentacion en "void calculaInterfaz() {"

    public float alto_y_Pantalla;   // Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public float ratio_dimensiones_Pantalla;  //  Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public float dimensionRefBaseEnPixels;   //  Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public float dimensionRefBaseEnEscala;   //  Ver observaciones en la documentacion en "void calculaInterfaz() {"

    public float factorGeneralTamañoObgetos;   //  Ver observaciones en la documentacion en "void calculaInterfaz() {"
    public bool modificaOrientacionMonitor; //  Ver observaciones en la documentacion en "void calculaInterfaz() {"

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Elementos por defecto 

    public AudioClip AudioClip_PorDefecto;  // Para el audio por defecto

    public Material material_fondoReferencia;  // Material para elementos referencia
    public Material material_fondoInstancia;  // Material para elementos Instancia
    public Material material_fondoSinTecho;  // Material para elementos SinTecho
    public Material material_fondoError;  // Material para casos de error


    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Parametros GENERALES

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Parametros del Usuario
    // Posicion del usuario
    public float posicion_x_Usuario;
    public float posicion_y_Usuario;
    public float posicion_z_Usuario;
    public Vector3 posicionUsuario;
    public Quaternion rotacionLocalnUsuario;

    // Escala de usuario
    public float escala_x_Usuario;
    public float escala_y_Usuario;
    public float escala_z_Usuario;
    //  velocidad de usuario
    public float velocidad_usr_X;
    public float velocidad_usr_Y;
    public float velocidad_usr_Z;
    // cercaDeUsuario es un PARAMETRO que indica el % que se usa para considerar que el usuario esta presente
    // se usa para saber si esta cerca de un murio de trabajo por ejemplo (Ver "usuarioEnMuro")
    public float cercaDeUsuario;
    public float usuarioEnMuro; // es el margen delante y detras del muro en el que se considera que el usuario esta en el muro
    public float modificaVelocidad; // es el margen delante y detras del muro en el que se considera que el usuario esta en el muro

    // ///  Parametros de los punteros que maneja el usuariol Usuario
    // Para el puntero de usuario (es el que va pegado al usario, esta por tanto en los muros de trabajo)
    // Posicion del puntero de usuario
    public float posicion_x_PunteroUsuario;
    public float posicion_y_PunteroUsuario;
    public float posicion_z_PunteroUsuario;
    public Vector3 posicionPunteroUsuario;
    // Escala del puntero de usuario. Estas escalas se definen para CONTROLAR EL MOVIMIENTO DEL PUNTERO, NO PARA SU TAMAÑO
    public float escala_x_PunteroUsuario;
    public float escala_y_PunteroUsuario;
    public float escala_z_PunteroUsuario;
    public Vector3 escalaPunteroUsuario;
    //  velocidad del puntero de de usuario
    public float velocidad_PunteroUsuario_X;
    public float velocidad_PunteroUsuario_Y;
    public float velocidad_PunteroUsuario_Z;
    // Para el puntero del muro de usuario (es el que va pegado al muro de usario, esta por tanto en muro de usuario)
    // Posicion del puntero del muro de usuario
    public float posicion_x_PuntMuroUsuario;
    public float posicion_y_PuntMuroUsuario;
    public float posicion_z_PuntMuroUsuario;
    public float distancia_z_PunteroUsuario;
    public Vector3 posicionPuntMuroUsuario;
    // Escala del puntero del muro de usuario
    public float escala_x_PuntMuroUsuario;
    public float escala_y_PuntMuroUsuario;
    public float escala_z_PuntMuroUsuario;
    public Vector3 escalaPuntMuroUsuario;


    /// ////////////////////////////
    /// ////////////////////////////
    // ///  Parametros de la luz "Foco luz"
    // Distancia de la luz al usuario
    public float distancia_z_LuzUsuario;
    public Vector3 distanciaLuzUsuario;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // /// Parametros de la CAMARA
    public float distancia_z_CamaraUsuario;
    public Vector3 distanciaCamaraUsuario;  // Se fija en "calculaInterfaz" para que encaje segun el tamaño del monitor

    public float campoVisionCamaraInicial; // Indica cuanto abre el objetivo de la camara (de 0 a 179 grados). Este es el dato que cargara al arrancar el sistema 
    public float campoVisionCamaraMuro; // Indica cuanto abre el objetivo de la camara (de 0 a 179 grados). Este es el campo para cubrir un muro completo a la distancia del usuario
    public float campoVisionCamara; // Indica cuanto abre el objetivo de la camara (de 0 a 179 grados). Este es el dato dinamico que indica el valor que tiene en el momento presente
    public float marcoPorCiento; // Defini, en tanto por ciento, el tamaño del marco que deja el campo visual de la camara entorno al muro de trabani, en la pocicion base de la camara
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // TRAMOYA Y TELONES

    // Parametros de la Tramoya
    public float escalaGeneralTramoya;
    public float escala_x_Tramoya;
    public float escala_y_Tramoya;
    public float escala_z_Tramoya;
    public Vector3 escalaTramoya;
    public float distancia_z_Tramoya;
    public Vector3 positionTramoya;
    public Quaternion giroOrientacionTramoya;
//    public Vector3 distanciaTramoya;
    public float ratioOcupacionTramoya;  // Indica cuanto ocupara la tramoya, que se colocara ocupando la parte superior de la pantalla de un lado al otro
    public float ratioEscalaElementosTramoya;  // Permite ajustar el tamaño de los elementos cuando estan en la tramoya, a su tamaño cuando estan fuera de esta

    // Para el puntero de la Tramoya
    public float distancia_z_PuntTramoya;

    // Parametros de los telones
    public float escala_x_Telones;
    public float escala_y_Telones;
    public float escala_z_Telones;
    public float escalaGeneralTelones;
    public Vector3 escalaTelones;
    public Vector3 positionTelones;
    public Quaternion giroOrientacionTelones;
    public float distancia_z_Telones;
    public Vector3 distanciaTelones;
    public float distancia_z_PuntTelones;

    //   ColPuntTelones

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Parametros del MURO DE USUARIO
    // /// Parametros del muro
    public float escalaGeneralMuroUsuario;  // es la dimension sobre la que se construye el muro de usuario. Depende de la diminsion menor (ancho o alto) de la pantalla
    public float escala_x_MuroUsuario;
    public float escala_y_MuroUsuario;
    public float escala_z_MuroUsuario;
    public Vector3 escalaMuroUsuario;
    public float distancia_z_MuroUsuario;
    public Vector3 positionMuroUsuario;
    public Quaternion giroOrientacionMuroUsuario;
    public Vector3 distanciaMuroUsuario;

    // Para el puntero del muro de usuario
    public float distancia_z_PuntMuroUsuario;

    // Para las consultas modales al usuario (SI o No) (selecciones, Tec... que se realizan bloqueando el resto de la aplicacion hast que se contesta
    // OJOO, los objetos de este tipo de consulta deben ser hijos del muro de usuario
    // Afecta en principio a los objetos siguientes :
    //      - Btn_Consulta_Si_No
    public float escala_x_ConsultaUsr_tipo01;
    public float escala_y_ConsultaUsr_tipo01;
    public float escala_z_ConsultaUsr_tipo01;
    public Vector3 escala_ConsultaUsr_tipo01;

    public float posicion_x_ConsultaUsr_tipo01;
    public float posicion_y_ConsultaUsr_tipo01;
    public float posicion_z_ConsultaUsr_tipo01;
    public Vector3 posicion_ConsultaUsr_tipo01;

    // Para los textos sin techo, (cuando se expanden en la pantalla van al texpro general y se colocan en una posicion fija)
    public Vector3 posicion_textoSinTecho;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    /// ///////////////// Nivel 1 de botones
    // /// Parametros de botones del menu (son locales, dentro del muro de usuario)
    // Caja_0 - Caja general de menu de usuario
    public float posicion_x_BtnMenu_N1_General;
    public float posicion_y_BtnMenu_N1_General;
    public float posicion_z_BtnMenu_N1_General;

    public float escala_geneal_BtnMenu_MuroUsuario;  // Define el tamaño general de los botones del muro de usuario. Junto con factorGeneralTamañoObgetos

    public float escala_x_BtnMenu_N1_General;
    public float escala_y_BtnMenu_N1_General;
    public float escala_z_BtnMenu_N1_General;
    public Vector3 posicion_BtnMenu_N1_General;
    public Vector3 escala_BtnMenu_N1_General;
    // //////////////////////////////////////////////////////////////////////////
    // Hijos del boton General
        // Caja_0_1 - Cargar, grabar. sonido, lenguajes,...
            // Para el boton de acceso al menu de control de interfaz dentro de de menu de usuario "BtnMenu_N1_1_Gereral_CtrlInterfaz" dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N1_1_Gereral_CtrlInterfaz; 
    public Vector3 escala_BtnMenu_N1_1_Gereral_CtrlInterfaz;
    public int NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz;
        // Caja_0_2 -  Poner muro, quitarlo, etc...
            // Para el boton de acceso al menu de control de escena dentro de menu de usuario "BtnMenu_N1_1_Gereral_Escena" dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N1_1_Gereral_Escena;
    public Vector3 escala_BtnMenu_N1_1_Gereral_Escena;
    public int NumBtn_BtnMenu_N1_1_Gereral_Escena;
       // Caja_0_3 - Buscadores, mochila, agentes, ramas
          // Para  boton de acceso al menu de control de herramientas dentro de menu de usuario dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N1_1_Gereral_Herramientas; 
    public Vector3 escala_BtnMenu_N1_1_Gereral_Herramientas;
    public int NumBtn_BtnMenu_N1_1_Gereral_Herramientas;
    // Caja_0_4 - Salir de la aplicacion
    // Para  boton de acceso al menu de control de herramientas dentro de menu de usuario dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N1_1_General_Salir;
    public Vector3 escala_BtnMenu_N1_1_General_Salir;
    public int NumBtn_BtnMenu_N1_1_General_Salir;

    /// ///////////////// Nivel 2 de botones (HIJOS DE control interfaz)
    // Caja_0_1_0 - Caja general que contiene : Cargar, grabar. sonido, lenguajes,...
    // Para el menu de  de menu de control de interfaz "BtnMenu_N2_CtrlInterfaz" dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N2_CtrlInterfaz;
    public Vector3 escala_BtnMenu_N2_CtrlInterfaz;
            // Caja_0_1_1 - Cargar
            // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_cargar;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_cargar;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_cargar_Activado;
            // Caja_0_1_2 - Grabar
            // Para eol boton de grabar el estado de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_grabar"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_grabar;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_grabar;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_grabar_Activado;
            // Caja_0_1_3 - Audio
            // Para eol boton de gestion del audio "BtnMenu_N2_1_CtrlInterfaz_audio"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_audio;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_audio;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_audio_Activado;
            // Caja_0_1_4 - Complejidad
            // Para eol boton de gestion del la complejidad de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_complejidad"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_complejidad;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_complejidad;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_complejidad_Activado;
            // Caja_0_1_5 - Idiomas
            // Para eol boton de gestion de la configuracion de ideomas que gestiona el usuario desde la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_idiomas"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_idiomas;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_idiomas;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_idiomas_Activado;
            // Caja_0_1_6 - Ambito
            // Para eol boton de gestion de la configuracion de ideomas que gestiona el usuario desde la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_ambito"
    public Vector3 posicion_BtnMenu_N2_1_CtrlInterfaz_ambito;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_ambito;
    public Vector3 escala_BtnMenu_N2_1_CtrlInterfaz_ambito_Activado;

        // Caja_0_2_0 - Caja general que contiene : Crear muro, borrar muro, ..
        // Para el boton fijo de menu de control de escena "BtnMenu_N2_Escena" dentro de "BtnMenu_N1_General"
    public Vector3 posicion_BtnMenu_N2_Escena;
    public Vector3 escala_BtnMenu_N2_Escena;
    // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
    public Vector3 posicion_BtnMenu_N2_1_Escena_CreaMuro;
    public Vector3 escala_BtnMenu_N2_1_Escena_CreaMuro;
    public Vector3 escala_BtnMenu_N2_1_Escena_CreaMuro_Activado;
    // Para eol boton de grabar el estado de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_grabar"
    public Vector3 posicion_BtnMenu_N2_1_Escena_EliminaMuro;
    public Vector3 escala_BtnMenu_N2_1_Escena_EliminaMuro;
    public Vector3 escala_BtnMenu_N2_1_Escena_EliminaMuro_Activado;
    // Para eol boton de gestion del audio "BtnMenu_N2_1_CtrlInterfaz_audio"
    public Vector3 posicion_BtnMenu_N2_1_Escena_GeneraRama;
    public Vector3 escala_BtnMenu_N2_1_Escena_GeneraRama;
    public Vector3 escala_BtnMenu_N2_1_Escena_GeneraRama_Activado;
    // Para eol boton de gestion del la complejidad de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_complejidad"
    public Vector3 posicion_BtnMenu_N2_1_Escena_CreaConcepto;
    public Vector3 escala_BtnMenu_N2_1_Escena_CreaConcepto;
    public Vector3 escala_BtnMenu_N2_1_Escena_CreaConcepto_Activado;

        // Caja_0_3_0 - Caja general que contiene : Buscador, mochila, agentes, ramas,...
        // Para el boton fijo de menu de herramientas dentro de "BtnMenu_N2_Herramientas"
    public Vector3 posicion_BtnMenu_N2_Herramientas;
    public Vector3 escala_BtnMenu_N2_Herramientas;
    // Para eol boton de acceso al evi de busqueda "BtnMenu_N2_1_Herramientas_buscador"
    public Vector3 posicion_BtnMenu_N2_1_Herramientas_buscador;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_buscador;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_buscador_Activado;
    // Para eol boton de acceso a la mochila "BtnMenu_N2_1_Herramientas_mochila"
    public Vector3 posicion_BtnMenu_N2_1_Herramientas_mochila;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_mochila;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_mochila_Activado;
    // Para eol boton de acceso a los agentes "BtnMenu_N2_1_Herramientas_agentes"
    public Vector3 posicion_BtnMenu_N2_1_Herramientas_agentes;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_agentes;
    public Vector3 escala_BtnMenu_N2_1_Herramientas_agentes_Activado;

    // Nivel 3 de botones (HIJOS DE herramientas)
    // Botones del menu principal
    public GameObject BtnMenu_N1_General;
    public GameObject BtnMenu_N1_1_Gereral_CtrlInterfaz;
    public GameObject BtnMenu_N1_1_Gereral_Escena;
    public GameObject BtnMenu_N1_1_Gereral_Herramientas;
    public GameObject BtnMenu_N1_1_General_Salir;

    public GameObject BtnMenu_N2_CtrlInterfaz;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_cargar;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_grabar;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_audio;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_complejidad;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_idiomas;
    public GameObject BtnMenu_N2_1_CtrlInterfaz_ambito;

    public GameObject BtnMenu_N2_Escena;
    public GameObject BtnMenu_N2_1_Escena_CreaMuro;
    public GameObject BtnMenu_N2_1_Escena_EliminaMuro;
    public GameObject BtnMenu_N2_1_Escena_GeneraRama;
    public GameObject BtnMenu_N2_1_Escena_CreaConcepto;

    public GameObject BtnMenu_N2_Herramientas;
    public GameObject BtnMenu_N2_1_Herramientas_buscador;
    public GameObject BtnMenu_N2_1_Herramientas_mochila;
    public GameObject BtnMenu_N2_1_Herramientas_agentes;
    
    // Autor Nicolas Merino Ramirez
    public GameObject BtnMenu_N2_1_Herramientas_breadcrumbsTrails;
    public GameObject Contenedor_BreadcrumbsTrails_SDI;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Parametros de los MUROS DE TRABAJO
    public float factorEscalaMuroTrabajo;
    public float escalaGeneralMuroTrabajo;

    public float escalaGrosorMuroTrabajo; // se emplea para el grosor de los elementos que cuelgan del muro de trabajo (el muro es un plano y no tiene grosor)

    // Para la escala
    public float escala_x_MuroTrabajo;
    public float escala_y_MuroTrabajo;
    public float escala_z_MuroTrabajo;
    public Vector3 escalaMuroTrabajo;
    // Para la orientacion
    public float giroOrientacion_x_MuroTrabajo;
    public float giroOrientacion_y_MuroTrabajo;
    public float giroOrientacion_z_MuroTrabajo;

    public Quaternion giroOrientacionMuroTrabajo;

    public float distanciaEntreMuros;

    // Hay un material para los muros. El modo se indica por el color, la activacion se indica mediante la trasparencia
//    public Material materialMuroActivo;
    public Material materialMuro;
//    public Material MaterialMuroEdicion;


    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para la base de Evis

    public GameObject BaseDeEvi_01;     // Configura la base sobre la que se construyen todos los demas evis:
                                        // Tiene los botones de control del evi, los datos de ayada a interfaz
                                        // Asi como un contenedor donde hay que introducir cada evi especifico
                                        // con la funcionalidad especifica que le correspondo (busqueda, fractal, lista,...)

    public GameObject ContenedorDeEvi_01;  // Es el game objet en el que hay que construir el evi especifico para cada funcion
                                           // fractal, busqueda, lista,...

    public GameObject TexAyuIntfBaseDeEvi;  // Configura la caja de texto donde aparece el texto de ayuda a interfaz, nombre, descripcion,...

    public GameObject ImgAyuIntfBaseDeEvi;  // Configura la caja de imagen donde aparece la imagen de ayuda a interfaz pequeña que acompaña al icono (NO ES la grande del contenedor), 
                                            // aunque normalmente será la misma foto

    public float factorEscalaBaseDeEvi; // este factor sirve para relacionar el tamaño del evi(en su tamaño reducido, sin el contenedor expandido), con el del muro de trabajo

    public static float SphereCollider_radio_BaseDeEvi;

    // Botones de la base para EVI
    // Botones principales
    public GameObject Btn_Evi_Info;
    public GameObject Btn_Evi_Maxi_Mini;
    public GameObject Btn_Evi_Desplazar;
    public GameObject Btn_Evi_Opciones;
    // Btones de opciones
    public GameObject Btn_Evi_Caja_opciones;

    // MATERIALES de los Botones de la base para EVI
    public Material MaterialBtnCajaOpciones; 
    public Material MaterialBtnClonar; 
    public Material MaterialBtnInstanciar; 
    public Material MaterialBtnDesplazar;
    public Material MaterialBtnDesplazarInstancia; 
    public Material MaterialBtnDesplazarSinTecho; 
    public Material MaterialBtnEditar;
    public Material MaterialBtnEditarEnEdicion;
    public Material MaterialBtnEliminar; 
    public Material MaterialBtnExpandir; 
    public Material MaterialBtnInfo;  
    public Material MaterialBtnMaximiza; 
    public Material MaterialBtnMaximizaInstancia;  
    public Material MaterialBtnMaximizaSinTecho; 
    public Material MaterialBtnMinimiza;  
    public Material MaterialBtnMinimizaInstancia;  
    public Material MaterialBtnMinimizaSinTecho;  
    public Material MaterialBtnOpciones;  
    public Material MaterialBtnOpcionesInstancia;  
    public Material MaterialBtnOpcionesSinTecho;  
    public Material MaterialBtnOpcionesEnEdicion;  


    // evis tipo BaseDeEvi_01
    public float escalaLocal_x_BaseDeEvi_01;
    public float escalaLocal_y_BaseDeEvi_01;
    public float escalaLocal_z_BaseDeEvi_01;
    public Vector3 estaEscalaBaseDeEvi_01;
    // Parece que unity ajusta la escala a su gusto cuando cambiamos de padre. He definido una escala para los evis cuando estan en la tramoya
    public float escalaLocal_x_BaseDeEviEnTramoya;
    public float escalaLocal_y_BaseDeEviEnTramoya;
    public float escalaLocal_z_BaseDeEviEnTramoya;
    public Vector3 estaEscalaBaseDeEviEnTramoya;  

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // Para los EVIs especificos
    public GameObject EviTipo_00;
    public GameObject EviFractal_00;
    public GameObject EviTipo_muestraGetDetails_00;
    

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para los EVIS DE BUSQUEDA

    ///  EVIs de datos
    public GameObject EviTipo_buscador_00;

    // /// Para evis de Buequeda
    // evis de busqueda "EviTipo_buscador_00"
    public float escalaLocal_x_EviTipo_buscador_00;
    public float escalaLocal_y_EviTipo_buscador_00;
    public float escalaLocal_z_EviTipo_buscador_00;
    public Vector3 estaEscalaEviTipo_buscador_00;

    public float escalaLocal_x_EviTipo_buscador_00_Activado;
    public float escalaLocal_y_EviTipo_buscador_00_Activado;
    public float escalaLocal_z_EviTipo_buscador_00_Activado;
    public Vector3 estaEscalaEviTipo_buscador_00_Activado;

    public float factor_mod_escalaLocal_EviTipo_buscador_00_Activado; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para los EVIS DE RAMA  (cada rama esta asociada a un evi, desde el que se crea, se accede y se destruye, Este evi puede ser un evi de rama, u otro evi
    //                             de otro tipo (concepto, buscador, etc...) pero si la rama se crea sin un concepto previo que la genere, se genera un evi de rama
    //                             como el que aqui se define, para que la rama se controle desde este evi)

    ///  EVIs de RAMA
    public GameObject EviTipo_Rama_00;

    // /// Para evis de RAMA
    // evis "EviTipo_Rama_00"
    public float escalaLocal_x_EviTipo_Rama_00;
    public float escalaLocal_y_EviTipo_Rama_00;
    public float escalaLocal_z_EviTipo_Rama_00;
    public Vector3 estaEscalaEviTipo_Rama_00;

    public float escalaLocal_x_EviTipo_Rama_00_Activado;
    public float escalaLocal_y_EviTipo_Rama_00_Activado;
    public float escalaLocal_z_EviTipo_Rama_00_Activado;
    public Vector3 estaEscalaEviTipo_Rama_00_Activado;

    public float factor_mod_escalaLocal_EviTipo_Rama_00_Activado; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para los EVIS FRACTAL DE REFERENCIA  (este es un evi destinado a albergar mediante una representacion tipo fractal, la informacion de un concepto, partiendo 
    //                                            de su referencia (key, host))

        ///  EVIs de FRACTAL DE REFERENCIA
    public GameObject EviTipo_RefFractal_01;

    public GameObject EviTipo_RF01_FractumRef;

    // /// Para evis de FRACTAL DE REFERENCIA
    // evis "EviTipo_RefFractal_01"
    public float escalaLocal_x_EviTipo_RefFractal_01;
    public float escalaLocal_y_EviTipo_RefFractal_01;
    public float escalaLocal_z_EviTipo_RefFractal_01;
    public Vector3 estaEscalaEviTipo_RefFractal_01;

    public float escalaLocal_x_EviTipo_RefFractal_01_Activado;
    public float escalaLocal_y_EviTipo_RefFractal_01_Activado;
    public float escalaLocal_z_EviTipo_RefFractal_01_Activado;
    public Vector3 estaEscalaEviTipo_RefFractal_01_Activado;

    public float factor_mod_escalaLocal_EviTipo_RefFractal_01_Activado; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

    public float baseDePotenciaFractal; // Sirve para definir la potencial del fractal de los evis fractales

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para los EVIS EviRefElemen  (este es un evi destinado a albergar mediante una representacion tipo fractal, la informacion de un concepto, partiendo 
    //                                            de su referencia (key, host))

    ///  EVIs EviRefElemen
    public GameObject EviRefElemen;

    // /// Para evis EviRefElemen
    // evis "EviRefElemen"
    public float escalaLocal_x_EviRefElemen;
    public float escalaLocal_y_EviRefElemen;
    public float escalaLocal_z_EviRefElemen;
    public Vector3 estaEscalaEviRefElemen;

    public float escalaLocal_x_EviRefElemen_Activado;
    public float escalaLocal_y_EviRefElemen_Activado;
    public float escalaLocal_z_EviRefElemen_Activado;
    public Vector3 estaEscalaEviRefElemen_Activado;

    public float factor_mod_escalaLocal_EviRefElemen_Activado; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para  evis de datos : evis tipo 00
    // evis tipo 00
    public float escalaLocal_x_EviTipo_00;
    public float escalaLocal_y_EviTipo_00;
    public float escalaLocal_z_EviTipo_00;
    public Vector3 estaEscalaEviTipo_00;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para  evis de datos :EviFractal_00
    // evis EviFractal_00
    public float escalaLocal_x_EviFractal_00;
    public float escalaLocal_y_EviFractal_00;
    public float escalaLocal_z_EviFractal_00;
    public Vector3 estaEscalaEviFractal_00;

    // evis EviFractal_00
    //   public float escalaLocal_x_EviFractal_00;
    //   public float escalaLocal_y_EviFractal_00;
    //   public float escalaLocal_z_EviFractal_00;
    //   public Vector3 estaEscalaEviFractal_00;

    // ///  Para  evis de datos : ContenedorEviSinTecho
    public float razon_escalaLocal_x_ContenedorEviSinTecho;
    public float razon_escalaLocal_y_ContenedorEviSinTecho;
    public float razon_escalaLocal_z_ContenedorEviSinTecho;

    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Para  evis de datos :  EviTipo_muestraGetDetails_00
    // evis EviTipo_muestraGetDetails_00
    public float escalaLocal_x_EviTipo_muestraGetDetails_00;
    public float escalaLocal_y_EviTipo_muestraGetDetails_00;
    public float escalaLocal_z_EviTipo_muestraGetDetails_00;
    public Vector3 estaEscalaEviTipo_muestraGetDetails_00;

    /// ////////////////////////////
    /// ////////////////////////////
    /// ////////////////////////////
    /// Materiales 
    public Material materialGenerico;   // Este sirve como base para instanciarlo y ponerle imagenes de ayuda a interfaz, iconos, etc 
    // //// Matriales de EVIs de datos
    // Materiales EviTipo_00
    public Material materialBaseDeEvi_01;
    public Material materialEvi_01;
    public Material materialEviActivo;
    public Material materialEviConDatos;
    // //// Matriales de EVIs buscadores
    // Materiales EviTipo_buscador_00
    public Material MaterialEviTipo_buscador_00;

    // //// Matriales de EVIs rama
    // Materiales EviTipo_Rama_00
    public Material MaterialEviTipo_Rama_00;

    // //// Matriales de EVIs fractal de referencia
    // Materiales EviTipo_RefFractal_01
    public Material MaterialEviTipo_RefFractal_01;





    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  Strings de uso general 

    public static string añadir = "añadir";  //   usuario  (de momento solo hay uno)
    public static string eliminar = "eliminar";  //   rama  (existe lista de elementos)


    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// //////////////////////////////////////////////////////////////////////////////////////


    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos todos los parametros del sistema
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2020-03-21
    // Observaciones :
    // 		Se debe llamar al inicio. Se ejecuta antes que cualquier funcion Star, por lo que todos los parametros
    //		deben ser inicializados aqui en lugar de los Start, donde arrancamos con lo propio de cada game objet
    void Awake()
    {
        // OJOOO cuidadin con los `puntos "." y otros caracteres de los nombres de dominio, y los nombres de fichero y otras gaitas que se generan con las variables que siguen
//        esta_iterfaz_Key = "mi_key_como_interfaz";  // Bebe definirla la entidad que genera esta implantacion (store, fabrica de software, o lo que sea) PENDIENTE MAFG 2022-03-06
//        esta_iterfaz_Host = "mi_host_como_interfaz_ideando_net";  // Bebe definirla la entidad que genera esta implantacion (store, fabrica de software, o lo que sea) PENDIENTE MAFG 2022-03-06
        esta_iterfaz_Key = "key_como_interfaz_uclm_00001";  // Bebe definirla la entidad que genera esta implantacion (store, fabrica de software, o lo que sea) PENDIENTE MAFG 2022-03-06
        esta_iterfaz_Host = "kee3d.crab.uclm.es";  // Bebe definirla la entidad que genera esta implantacion (store, fabrica de software, o lo que sea) PENDIENTE MAFG 2022-03-06
        host_efimero_de_interfaz = esta_iterfaz_Key + "_efim_" + esta_iterfaz_Host;// Contiene un identificador propio de quien genera esta interfaz. (que puede ser un distribuidor de software, un DKS u otros
                                                                                   // Debe estar asociada a un nombre de dominio dresponsable
                                                                                   // Debe cumplir por tanto las normas de los nombres de dominio; ejemplo KEE3D.ideando.net
                                                                                   // OJO debe asignarse en "ScriptGestionaDAUS"
                                                                                   // Contiene un identificador (diferente para todos los generados por un generador) propio de esta interfaz. El formato lo define el generador
                                                                                   // OJO. cuando el usuario solicita acceso al DKS, este debe suministrarle este dato, para que cada acceso de cada usuario tenga 
                                                                                   // un "idEstaInterfaz" diferente (ver "")
                                                                                   // OJO debe asignarse en "ScriptGestionaDAUS"


        // Definimos los parametros de usuario PENDIENTE MAFG 2021-10-24, luego habra que asignarlos segun el usuario
        usrKey = "sinKeyUsr";
        usrHost = "sinHostUsr";
        usrNombre = "anonimo";
        usrPasword = "";

 //       host_efimero_de_interfaz = "sinGenerador_gKEEs_ideando_net"; 

        // Averiguamos el tamaño del monitor donde se presenta la aplicacion
        obtenDimensionMonitor();

        // Asignacion de prefijos de string
        locIconosKlw = "Iconos/Klw";
        locIconosExternos = "Iconos/Externos";
        locImagenesKlw = "Imagenes/Klw";
        locImagenesExternos = "Imagenes/Externos";
        locAudiosKlw = "Audios/Klw";
        locAudiosExternos = "Audios/Externos";
        locFicherosKlw = "xml/Klw";
        locFicherosExternos = "xml/Externos";
        locFicherosKlw_conResources = "Assets/Resources/xml/Klw";
        locFicherosExternos_conResources = "Assets/Resources/xml/Externos";

        locFicherosTemporales = "Temporales";

        nombre_Fich_Resources = "Resources";


    //////////////////////////
    // ////////////////////////////
    // Datos generales de interfaz
    //        ancho_x_Pantalla = 100f;

        //        alto_y_Pantalla = 10f;
        //		modificaOrientacionMonitor = true; // Para indicar si deseamos modificar el aspecto de la pantalla
        modificaOrientacionMonitor = false; // Para indicar si deseamos modificar el aspecto de la pantalla



        // Calculamos los parametros de la interfaz, dependiendo del tamaño del monitor
        calculaInterfaz();

        // Generamos las listas de celementos en la interfaz
        ListaRamas = new List<GameObject>();
        ListaMuros = new List<GameObject>();
        ListaEvis = new List<GameObject>();

        // Apagamos el panel global del texto del cambas, que no debe aparecer por defecto
        GetComponent<ScriptDatosInterfaz>().PanelCanvasCompleto.SetActive(false);


        // ////////////////////////////
        ///  Parametros de ejecucion
        numDeFrame = 0;
        numDeConceptosGenerados = 0;

        periodoRefractarioBotonMouse = 250;  // En milisegundos
        // iniciPerRefractBotonMouse; mo hay que inicializarlo, se inicializa cada vez que llamamos a "gestionaEnPerRefracBotonMouse(other.gameObject)"
        enPeriodoDeRefraccion = false;

        ordinal_de_identificador_unico = 0; // Inicializamos el identificador a cero (ver "genera_identificador_unico()")


        ultimoIdElementIntf = 100;  // los valores de 0 a 99 se reservamn para identificar otras entidades de la interfaz de cara a diversas tareas y algoritmos
                                    // El usuario es el 0
        ultimoIdElementTipoRama = 0;
        ultimoIdElementTipoMuro = 0;
        ultimoIdElementTipoEvi = 0;
        ultimoIdElementTipoSolicitud = 0;
        ultimoIdArbolIntf = 0;

        // ////////////////////////////
        ///  Para las ramas y el arbol de ramas
        // Generamos la lista de arrays que contiene el arbol de las ramas (ponerlo despues con las otras generaciones de lista)
//        arbolIntf = new List<int[]>();
//        raizActual = 0;
        arbolRecienGenerado = false;
        numRamas = 0;

        // ////////////////////////////
        ///  Parametros de los muros
        numMuros = 0;
        // Para la orientacion espacial
        Quaternion giroOrientacionMuro = Quaternion.Euler(0f, 0f, 0f);
        muro.transform.rotation = giroOrientacionMuro;
        // Para el tamaño
        Vector3 escalaMuro = new Vector3(escala_x_MuroTrabajo, escala_y_MuroTrabajo, escala_z_MuroTrabajo);
        muro.transform.localScale = escalaMuro;

        nivelTransparenciaMuroActivo = 0.1f; // Bastante transparente es un float de 0 (transparente) a 1 (opaco)
//        nivelTransparenciaMuroInactivo = f;
        nivelTransparenciaMuroInactivo = 0.9f; // Bastante Opaco
        //        colorMuroNavegacion = new Color(124f, 124f,124f, nivelTransparenciaMuroInactivo);
        //        colorMuroEdicion = new Color(255f, 255f, 0f, nivelTransparenciaMuroInactivo);
        colorMuroNavegacion = new Color(1f, 1f, 1f, nivelTransparenciaMuroInactivo); // Color blanco r,g,b y a son un float de 0 a 1
        colorMuroEdicion = new Color(1f, 1f, 0f, nivelTransparenciaMuroInactivo); // Color amarillo r,g,b y a son un float de 0 a 1


        // El "materialMuro" se fija desde la inte4rfaz gráfica


        // ////////////////////////////
        ///  Parametros de los EVIs

        numEvis = 0;
        SphereCollider_radio_BaseDeEvi = 1f;

        // Para el tamaño de los Evis tipo 00
        //		Vector3 escalaEviTipo_00 = new Vector3 (escalaLocal_x_EviTipo_00, escalaLocal_y_EviTipo_00, escalaLocal_z_EviTipo_00);
        //		EviTipo_00.transform.localScale = escalaEviTipo_00;

        // ////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////
        ///  Cargmos la interfaz de usuario
        ///  



    } // FIn de - void Awake()

    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  En esta funcion, inicializamos la interfaz con el estado deseado por el usuario
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2020-03-01
    // Observaciones :
    // 		Debe cargarse el DAUS del usuario y dejar la interfaz en el estado que el usuario dejo la ultima vez que la salvo
    void Start() {


    }  // Fin de - void Start () {

    // Update is called once per frame
    void Update() {

    } // FIn de - 	void Update () {


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Calculamos la geometria y tamaños de la interfaz
    /// Esta funcion obtiene el tamaño del monitor donde se presenta la aplicación, con el fin de adecuar a este la 
    ///	configuracion de todos los elementos de la interfaz
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-13
    /// Observaciones :
    /// 		Se debe llamar al inicio, y cada vez que el monitor o la zona donde se monitoriza la aplicacion
    ///		cambie de tamaño (paso a apaisado, cambio de tamaño de ventana en el navegador, etc...)
    /// </summary>

    void obtenDimensionMonitor() {
        // Vemos el tamaño de la pantalla

//        float anchoPantalla = (float)Screen.width;
//        float altoPantalla = (float)Screen.height;
        pixels_x_Pantalla = Screen.width;   // Es el numero de pixels del monitor a lo largo del eje x
        pixels_y_Pantalla = Screen.height;   // Es el numero de pixels del monitor a lo largo del eje y

        ratio_dimensiones_Pantalla = (float)pixels_x_Pantalla / (float)pixels_y_Pantalla;  // Es la razon entre el el ancho y alto de la pantalla x/y
        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Desde ScriptDatosInterfaz 1 - obtenDimensionMonitor()161, - pixels_x_Pantalla = " + pixels_x_Pantalla +
            " - pixels_y_Pantalla :" + pixels_y_Pantalla +
            " - ratio_dimensiones_Pantalla :" + ratio_dimensiones_Pantalla);
        }
        // Calculamos la escala en el eje y para que la pantalla de juego (muros, etc) encaje en el apaisado del monitor (o ventena de juego)
        alto_y_Pantalla = ancho_x_Pantalla / ratio_dimensiones_Pantalla;

        // Definimos la dimension que sera referencia par el tamaño de los objetos no apaisados (evis y otros)
        if (pixels_y_Pantalla < pixels_x_Pantalla) { dimensionRefBaseEnPixels = pixels_y_Pantalla; }
        else { dimensionRefBaseEnPixels = pixels_x_Pantalla; }
        // Ahora pasamos los pixels a parametros de escena. Mediante una regla de tres (dim escala / dim pixels = ancho escala/ ancho pixels)
        dimensionRefBaseEnEscala = ancho_x_Pantalla * dimensionRefBaseEnPixels / pixels_x_Pantalla;

        // El factor general de objetos debe calcularse atendiendo al tamaño fisico del monitor y su resolucion en pixels. Por ahora lo ponemos fijo
        // PENDIENTE MAFG 2021-03-31
        factorGeneralTamañoObgetos = 1f/2f;
        //        factorGeneralTamañoObgetos = 1f / 3f;
        factorEscalaGeneralCanvas = factorGeneralTamañoObgetos;

        FactorOffsetAvanceCanvas = -0.9f; // Para que el canvas quede un pelin por delante del muro de trabajo

        // Por ahora consideramos que la ventana esta en la esquina inferior del monitor. Hay que ver como se obtiene esta informacion. PENDIENTE MAFG 2021-04-03
        origenVentanaPixels_x = 0;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor
        origenVentanaPixels_y = 0;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor


        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Desde ScriptDatosInterfaz 2 - obtenDimensionMonitor()161, - pixels_x_Pantalla = " + pixels_x_Pantalla +
                " - pixels_y_Pantalla :" + pixels_y_Pantalla +
                " - ratio_dimensiones_Pantalla :" + ratio_dimensiones_Pantalla +
                " - ancho_x_Pantalla :" + ancho_x_Pantalla +
                " - alto_y_Pantalla :" + ancho_x_Pantalla +
                " - dimensionRefBaseEnPixels :" + dimensionRefBaseEnPixels +
                " - dimensionRefBaseEnEscala :" + dimensionRefBaseEnEscala);
        }

} // Fin de - void obtenDimensionMonitor() {


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Calculamos la geometria y tamaños de la interfaz
    /// Esta funcion calcula la posicion de la cammara con respecto al usuario, la distancia al muro de trabajo
    /// la distancia entre muros y otros parametros, dependiendo del tamaño del monitor.
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020-03-13
    /// Observaciones :
    /// 	Los objetivos son los siguientes :
    /// 		1) Que las botoneras del muro de usuario queden en el borde de la pantalla
    /// 		2) QUe a la distancia de observacion, los muros sean completamente visibles en la menor de sus
    ///			dimensiones (ancho o alto) en la imagen de camara, la otra dimension habra que recorrerla
    ///			mediante scroll (esto es mejor que que aparezca una linea negra, si ajustamos la camara 
    /// 			a la dimension mayor)
    /// ///////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////
    /// Parametros generales de la interfaz
    /// 
    /// //////////////////// Con respecto a las escalas, el tamaño del monitor y el espacio de la pantalla que ocupa la interfaz
    /// 
    ///     - El TAMAÑO DEL MONITOR en pixels se pide al sistema y es (pixels_x_Pantalla, pixels_y_Pantalla) :
    ///             - "pixels_x_Pantalla" - Es el numero de pixels del monitor a lo largo del eje x
    ///             - "pixels_y_Pantalla" - Es el numero de pixels del monitor a lo largo del eje y
    ///             - "ratio_dimensiones_Pantalla" - Es la razon entre el el ancho y alto de la pantalla ("ratio_dimensiones_Pantalla"="pixels_x_Pantalla"/"pixels_y_Pantalla")
    ///             
    ///     - El TAMAÑO DEL ESPACIO EN PANTALLA en pantalla, se calcula dependiendo de (pixels_x_Pantalla, pixels_y_Pantalla) :
    ///             - "ancho_x_Pantalla" - Es el ANCHO de la escena en dimensiones UNITY eje x de coordenadas. Debe ser proporcional a un de las dimensiones en pixel del monitor y 
    ///                                 fundamentalmente se ajusta al ancho del muro de trabajo, o mas bien el ancho del muro de trabajo es de este ancho. Normalmente lo 
    ///                                 fijamos a 100, para que el tamaño de los elementos de la escena ronde la unidad.
    ///                                 LO DEFINIMOS COMO ESTATICO PORQUE ES EL UNICO FIJO, TODOS LOS DEMAS SE RECALCULAN SOBRE ESTE
    ///             - "alto_y_Pantalla" - Es el ALTO de la escena en dimensiones UNITY eje y de coordenadas. Debe ser proporcional a un de las dimensiones en pixel del monitor y 
    ///                                 fundamentalmente se ajusta atendiendo a "ancho_x_Pantalla" y "ratio_dimensiones_Pantalla" 
    /// 
    ///     - FACTORES PARA EL AJUSTE de tamaños de los objetos en la escena : 
    ///             - "dimensionRefBaseEnPixels" - Es el menor tamaño de entre el ancho y el alto de la pantalla (en pixels). Se usa para dimensionar todos los elementos que aparecen en 
    ///                                         escena. Los elementos de la escena son normalmente cuadrados, no apaisados, aunque la pantalla sea apaisada. Por esto, el tamaño de 
    ///                                         los objetos que son cuadrados, se ajusta a este parametro , tanto en x como en y.
    ///             - "dimensionRefBaseEnEscala" - Igual que "dimensionRefBaseEnPixels", pero en escala Unity, en lugar de en pixels.
    ///             - "factorGeneralTamañoObgetos" - Sirve para ajustar el tamañe de todos los objetos con respecto a "dimensionRefBaseEnEscala", para que todos lo objetos sean mayores 
    ///                                         o menores segun el tamaño del monitor. En un monitor muy grande, los objetos podran se mas pequeños, o dependiendo de la distancia de
    ///                                          la persona al monitor, murales, u otros.
    ///             - "modificaOrientacionMonitor" - Para indicar si deseamos modificar el aspecto de la pantalla vertical o apaisada. En cualquier caso, para la programacion, el que 
    ///                                         la imagen se exponga de forma apaisada u horizontal es irrelevante, ya que debe recalcularse y siempre trabajamos y recalculamos para 
    ///                                         que x sea horizontal, y sea vertical y z crezca hacia el fondo de la pantalla
    /// pixels_x_Pantalla
    /// pixels_y_Pantalla
    /// ratio_dimensiones_Pantalla
    /// ancho_x_Pantalla 
    /// alto_y_Pantalla
    /// dimensionRefBaseEnPixels
    /// dimensionRefBaseEnEscala
    /// factorGeneralTamañoObgetos
    /// modificaOrientacionMonitor
    /// 
    /// //////////////////// Con respecto a las ESCALAS en los EJES X e Y
    /// 
    ///     - Los PADRES SUPREMOS EN LA GERARQUIA DE UNITY son :
    ///         - 1.) "ctrlInterfaz" : NO TRABAJA CON UN GAME OBJECT fisico asociado. Pero contiene este escript con todos los datos globales del sistema
    ///         - 2.) "Solicitudes" : NO TRABAJA CON UN GAME OBJECT fisico asociado. Se dedica a la gestion de las solicitudes
    ///         - 3.) RAMA : Es la madre suprema del arbol fisico (posteriormente puede pasar a ser hija de otra rama)
    ///                 - Escala :          (1,1,1)
    ///                 - Localizacion:     (0,0,z) las ramas se centran en la rama madre, pero van naciendo a una distancia (que puede ser variable) del origen de su madre
    ///                 - Giro :            (x,y,z) Las ramas salen con un determinado angulo de su rama madre segun corresponda para el desarrollo del arbol
    ///                 
    ///                 - Hijos de la RAMA :
    ///                     - 3.1.) USUARIO :
    ///                         - Escala :          (1,1,1)
    ///                         - Localizacion:     (0,0,z) El usuario va viajando a lo largo de la rama, normelmente de muro en muro
    ///                         - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                         
    ///                         - Hijos del USUARIO :
    ///                         
    ///                             - PUNTERO DE USUARIO:
    ///                                 - Escala :          (1,1,1)
    ///                                 - Localizacion:     (x,y,0) El usuario va viajando a lo largo de la rama, normelmente de muro en muro, en z coincide con el usuario
    ///                                 - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                             
    ///                                 - Hijos de PunteroUsuario :
    ///                                 
    ///                                     - PUNTERO MURO USUARIO
    ///                                         - Escala :          (0.5,0.5,0.1) Es de escala algo mas pequeña que el puntero de usuario porque est mas cerca
    ///                                         - Localizacion:     (x,y,con el muro de usuario) El usuario va viajando a lo largo de la rama, normelmente de muro en muro
    ///                                         - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                                     
    ///                             - MURO DE uSUARIO:
    ///                                 - Escala :          (depende vision camara,depende vision camara,0.1) Se escala segun el campo de vision de la camara y su distancia a esta (en z se estrecha para que sea mas fino)
    ///                                 - Localizacion:     (0,0,Con el usuairo y la camara) Va siempre enganchado al usuario, la distancia depende de si la camara se caerca al muro
    ///                                 - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                             
    ///                                 - Hijos de Muro de Usuario :
    ///                                     
    ///                                     - Botones del muro de usuario
    ///                                         - Escala :          Los botones son cuadrados, No apaisados como su padre el muro de trabajo. Por lo que HAY QUE AJUSTAR EL EFECTO PANORAMICO
    ///                                                             - Se ajustan mediante el parametro "escala_geneal_BtnMenu_MuroUsuario"
    ///                                         - Localizacion:     En la esquina de abajo derecha
    ///                                         - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                                 
    ///                             
    ///                     - 3.2.) MURO DE TRABAJO : - EL usuario tiene escala 1, pero el muro tiene una escala grande para poder albergar los evis.
    ///                                               - La coordenada menor del monitor (ancho o alto), se ancla al valor " escalaGeneralMuroTrabajo", para poder hacer equivalencia entre
    ///                                               pixels en pantalla y coordenadas unity. La otra coordenda, tambien se hace la misma equivalencia, ajustando su tamaño segun corresponda 
    ///                                               en la relacion ancho/alto del monitor
    ///                                               
    ///                         - Escala :          (x,y,0.1)  normalmente (100,100,0.1) 
    ///                                                 - x = escalaGeneralMuroTrabajo, y se adapta atendiendo a la ratio ancho/lalto del monitor. 
    ///                                                 - y = escalaGeneralMuroTrabajo, y se adapta atendiendo a la ratio ancho/lalto del monitor. 
    ///                                                 - z = 0.1 se estrecha para que quede mejor
    ///                         - Localizacion:     (0,0,distanciaEntreMuros) Los muros van colocandose uno tras otro en la rama separados por una distancia que puede variar dependeiendo de las necesidades de crecimiento del arbol
    ///                         - Giro :            (0,0,0) El usuario no gira sobre su rama (se mantiene perpendicular a esta (aunque la direccion de la rama en el espacio puede ser cualquiera)
    ///                         
    ///                         - Hijos del Muro de Trabajo :
    ///                         
    ///                             - EVI BASE:
    ///                                  - Escala :          (x,y,1) normalmente (0.05, 0.05, 1)
    ///                                                         - X depende del factorEscalaBaseDeEvi y del ajuste panoramico
    ///                                                         - y depende del factorEscalaBaseDeEvi y del ajuste panoramico
    ///                                                         - z = 1; tiene el mismo grosos que el muro de trabajo
    ///                                  - Localizacion:     (x,y,0) Se coloca en cualquier punto del muro de trabajo
    ///                                  - Giro :            (0,0,0) Sin giro
    ///                                  
    ///                                  - Hijos del Evi Base:
    ///                                         - BOTONES
    ///                                         - CONTENEDOR
    ///                                               - Escala :          (x,y,1)  Normalmente (5,5,1)
    ///                                                                         - X depende del escalaAnchoContenedorDeEvi_01 (esta en ScriptCtrlBaseDeEvi)
    ///                                                                         - y depende del escalaAltoContenedorDeEvi_01 (esta en ScriptCtrlBaseDeEvi)
    ///                                                                         - z = 1; escalaProfundoContenedorDeEvi_01 (esta en ScriptCtrlBaseDeEvi)
    ///                                               - Localizacion:     (x,y,0) A la izquierda arriba de la base del evi (esta en ScriptCtrlBaseDeEvi)
    ///                                               - Giro :            (0,0,0) Sin giro
    ///                         
    ///                                               - Hijos del Evi Base:
    ///                                                     Los EVIS especificos cada uno con su estructura correspondiente
    ///                                                           - Escala :          (1,1,1)  Normalmente igual que su contenedor (estara en el script especifico del tipo de evi)
    ///                                                           - Localizacion:     (0,0,0) Normalmente en el mismo sitio que el contenedor (estara en el script especifico del tipo de evi)
    ///                                                           - Giro :            (0,0,0) Normalmente Sin giro (estara en el script especifico del tipo de evi)
    ///                 
    /// 
    /// /// //////////////// Con respecto a las PROFUNDIDADES en el EJE Z
    /// 
    /// Sobre el eje Z sobre el que se van colocando todos los elementos
    ///        I             I                I                        I                                       I           I 
    ///        I ---------------------- 7. distancia_z_CamaraUsuario------------------------------------------------------ I 
    ///                      I --------------------- 6. distancia_z_LuzUsuario-------------------------------------------- I 
    ///                                       I -------------- 5.  distancia_z_MuroUsuario ------------------------------- I 
    ///                                                   I ------------ 4.  distancia_z_PuntMuroUsuario  ---------------- I  (este puntero va con el puntero de usuario, pero en otro plano en Z para coincidir con el muro de usuario)
    ///                                                                            I --- 3.  distancia_z_Tramoya --------- I
    ///                                                                                    I - 2. distancia_z_PuntTramoya -I 
    ///                                                                                       1. distancia_z_PunteroUsuarioI 
    ///                                                                                                        I--- 1. --- I  --- 8.  distanciaEntreMuros--- I ---- distanciaEntreMuros-----I
    ///        I             I                I            I                                                   I        Origen =0                            I                              I
    ///        I             I                I            I                       I       I                   I           I                                 I                              I
    ///      canara      FocoLuz         MuroUsuario      PunteroMuroUsuario     Tramoya  PuntTramoya    PunteroUsuario Usuario                    SiguienteMuroTrabajo            SiguienteMuroTrabajo
    ///                                                                                                                MuroTrabajo
    ///                                                                                                                    I El primer muro de trabajo se coloca en z=0, en la posicion inicial del usuario
    /// </summary>


    void calculaInterfaz()
    {
        // Calculamos el tamaño de la interfaz dependiendo de "ancho_x_Pantalla" y "alto_y_Pantalla"

        /// pixels_x_Pantalla
        /// pixels_y_Pantalla
        /// ratio_dimensiones_Pantalla
        /// ancho_x_Pantalla 
        /// alto_y_Pantalla
        /// dimensionRefBaseEnPixels
        /// dimensionRefBaseEnEscala
        /// factorGeneralTamañoObgetos
        /// modificaOrientacionMonitor


        // ////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////
        // Parametros generales que parametrizan las distancias que se definen a continuacion
        // ancho_x_Pantalla;  EsPara tener una referencia comun a todos
        // Para el entorno del usuario (donde estan muro de usuario, Tramoya, camara, luz y sus punteros) Pudrá viajar, crecer y decrecer y moverse por el arbol
        // ojo-2021-12-28       factorEscalaEntornoUsuario = ancho_x_Pantalla * 0.15f;  // Este parametro sirve para ajustar la escala de los elementos que van con el usuario
        factorEscalaEntornoUsuario = ancho_x_Pantalla;  // Este parametro sirve para ajustar la escala de los elementos que van con el usuario
        escalaEntornoUsuario = factorEscalaEntornoUsuario; // El resto de las escalas, se va relacionando con esta
        escala_geneal_BtnMenu_MuroUsuario = (dimensionRefBaseEnEscala / ancho_x_Pantalla) * 1f / 6f; // Define el tamaño de los botones del men del muro de usuario. Junto con "factorGeneralTamañoObgetos
                                                                                                     // Se ajusta al tamaño menor entre el alto y el ancho de la pantalla
                                                                                                     // Para el muro de trabajo (junto con las ramas van configurando una estructura por la que viaja el usuario generando y explorando su contenido
        factorEscalaMuroTrabajo = 1f;  //  10, tamaño 10x10=100  OJOO. Los muros son planos, y los planos se componen de 10x10 baldosas, la escala lo es de una baldosa, luego el tamaño del plano es factorEscalaMuroTrabajo x 10
        escalaGeneralMuroTrabajo = ancho_x_Pantalla * factorEscalaMuroTrabajo;
        factorEscalaBaseDeEvi = factorGeneralTamañoObgetos * (dimensionRefBaseEnEscala / ancho_x_Pantalla) * (1f / 7f);  //  Los muros ya no son planos, si no cubos (2021-07-08).10, tamaño 10x10=100  OJOO. Los muros son planos, y los planos se componen de 10x10 baldosas, la escala lo es de una baldosa, luego el tamaño del plano es factorEscalaMuroTrabajo x 10

        // Para la tramoya
        ratioOcupacionTramoya = 1f / 4f;  // La tramoya ocupara un cuarto de la parte alta de la pantalla

        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        // definicion de diatncias basicas
        // 1. PunteroUsuario
        distancia_z_PunteroUsuario = 0f; // distancia_z_PunteroUsuario. Se situa en el mismo plano que el usuario (normalmente en el muro activo) (LOCAL al gameobject Usuario)
        // 3. Tramoya
        distancia_z_Tramoya = -(escalaEntornoUsuario / 4f); // desde el usuario a la Tramoya (LOCAL al gameobject Usuario)
        // 2. distancia_z_PuntTramoya
        distancia_z_PuntTramoya = distancia_z_Tramoya;  // se coloca en el plano de la Tramoya (LOCAL al gameobject Usuario)
        // 5.  distancia_z_MuroUsuario
        distancia_z_MuroUsuario = -(escalaEntornoUsuario / 3f); // desde el usuario al muro de usuario. Un tercio de escalaEntornoUsuario  (LOCAL al gameobject Usuario)
        // 4.  distancia_z_PuntMuroUsuario
        distancia_z_PuntMuroUsuario = distancia_z_MuroUsuario;  // se coloca en el plano del muro de usuario (LOCAL al gameobject Usuario)
        // 6. distancia_z_LuzUsuario
        distancia_z_LuzUsuario = -(escalaEntornoUsuario / 5f);  // distancia_z_LuzUsuario. La ponemos lo mas cerca del muro de trabajo para que nada haga sombras
        // 7. distancia_z_CamaraUsuario
        distancia_z_CamaraUsuario = -(escalaEntornoUsuario * 2f / 3f); //  -10 distancia_z_CamaraUsuario, va por detras del usuario
                                                                       // 8.  distanciaEntreMuros 
                                                                       //        distanciaEntreMuros = escalaEntornoUsuario * 10f;  // 150 distanciaEntreMuros. Esta se modificara para cada tramo de rama dependiendo del crecimiento del arbol
        distanciaEntreMuros = escalaEntornoUsuario * 5f;  // 150 distanciaEntreMuros. Esta se modificara para cada tramo de rama dependiendo del crecimiento del arbol

        // ///////////////////////////////////////////
        // Otros parametros 
        //        marcoPorCiento = 10f; // Dejamos un marco del 5% alrededor del muro de trabajo, en el campo visual de la camara (en su situacion base)
        marcoPorCiento = 0f; // Dejamos un marco del 5% alrededor del muro de trabajo, en el campo visual de la camara (en su situacion base)
        float factorReduccion01 = 0.5f; // Se usa para manejar el tamaño de algunos botones, segun esten seleccionados o no

        // El campo de vision de la camara, en su configuracion base (luego podra modificarse), forma un cono (piramide) con vertice la camara y basse el muro de trabajo. El 
        // usuario, se localiza en el muro de trabajo (base de la piramida). EL resto de elementos, la Tramoya, muro de usuario, y otros, asi como sus respectivos punteros y botones
        // deben ajustarse a este cono para aparecer adecuadamente en el campo de la camara y por tanto en el monitor.
        // OJOO. la posicion estandar de la camara es centrada sobre el muro de trabajo y cubriendolo completo con su campo de vision, pero en un futuro, la idea es poder
        // acercarse al muro para hacer zumm sobre objetos fractales y poder ir expandendo su contenido, por lo que la camara tendra que poder moverse en (x,y,z) locales al usuario 
        // y modificar el campo de vision para poder realizar zoom sobre objetos fractales u otros

        // ////////////////////////////////////////
        // Primero calculamos la apertura del campo de vision de la camara para susituacion base, esto es centrada en el muro de trabajo y visualizandolo completo
        // Indica cuanto abre el objetivo de la camara (de 0 a 179 grados). Este es el dato que cargara al arrancar el sistema 
        MainCamera.GetComponent<ScriptCtrlCamara>().camaraAPosicionBase();

        // Ajustamos LA TRAMOYA para que se ajuste al cono de vision de la camara
        calculaTramoya();

        // Ajustamos el MURO DE USUARIO para que se ajuste al cono de vision de la camara
        calculaMuroUsuario();



        // ////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////
        // Pasamos a ajustar los distintos elementos de la interfaz

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para el usuario
        // Posicion. El usuario es el origen de la cuestion. La camara y el muro de usuario le siguen
        posicion_x_Usuario = 0f;
        posicion_y_Usuario = 0f;
        posicion_z_Usuario = 0f;
        posicionUsuario = new Vector3(posicion_x_Usuario, posicion_y_Usuario, posicion_z_Usuario);

        rotacionLocalnUsuario = Quaternion.Euler(0, 0, 0);

        //  Movimiento de usuario
        velocidad_usr_X = 0f;
        velocidad_usr_Y = 0f;
        velocidad_usr_Z = 0.8f;

        // ////////////////////
        // Para los punteros de usuario
        // Para el puntero de usuario
        // Posicion. El puntero 1 En principio esta un pelin a la derecha del usuario, es la posicion inicial. luego lo mueve el usuario en el plano correspondiente
        posicion_x_PunteroUsuario = 1.0f;
        posicion_y_PunteroUsuario = 0f;
        posicion_z_PunteroUsuario = distancia_z_PunteroUsuario;
        posicionPunteroUsuario = new Vector3(posicion_x_PunteroUsuario, posicion_y_PunteroUsuario, posicion_z_PunteroUsuario);
        //  Movimiento de usuario
        //        escala_x_PunteroUsuario = 2f;
        //        escala_y_PunteroUsuario = 2f;
        escala_x_PunteroUsuario = 1f;
        escala_y_PunteroUsuario = 1f;
        escala_z_PunteroUsuario = 1f;
        escalaPunteroUsuario = new Vector3(escala_x_PunteroUsuario, escala_y_PunteroUsuario, escala_z_PunteroUsuario);
        //  Movimiento de usuario
        velocidad_PunteroUsuario_X = 2f;
        velocidad_PunteroUsuario_Y = 2f;
        velocidad_PunteroUsuario_Z = 0f;
        // Para el puntero del muro de usuario (OJO, SON PARAMETROS LOCALES A SU PADRE EL PUNTERO DE USUARIO)
        // Posicion. El puntero 1 En principio esta un pelin a la derecha del usuario
        posicion_x_PuntMuroUsuario = 0f;  // Se coloca en la misma posiscion que su padre (puntero de usuario)
        posicion_y_PuntMuroUsuario = 0f;  // Se coloca en la misma posiscion que su padre (puntero de usuario)
        posicion_z_PuntMuroUsuario = distancia_z_MuroUsuario;    // Se coloca por detas de su padre (puntero de usuario) para colocarse en el muro de usuario
        posicionPuntMuroUsuario = new Vector3(posicion_x_PuntMuroUsuario, posicion_y_PuntMuroUsuario, posicion_z_PuntMuroUsuario);
        //  Movimiento de usuario
        escala_x_PuntMuroUsuario = 0.5f;  // De grande igualico que su padre (puntero de usuario)
        escala_y_PuntMuroUsuario = 0.5f;  // De grande igualico que su padre (puntero de usuario)
        escala_z_PuntMuroUsuario = 0.1f;  // De grande igualico que su padre (puntero de usuario)
        escalaPuntMuroUsuario = new Vector3(escala_x_PuntMuroUsuario, escala_y_PuntMuroUsuario, escala_z_PuntMuroUsuario);

        // ///////////////////////
        // Parametros operativos
        cercaDeUsuario = 10f;  // nos sirve para activar o desactivar cosas (muros, etc...) para considerar cuando el usuario esta proximo
        usuarioEnMuro = distanciaEntreMuros / cercaDeUsuario; // es el margen delante y detras del muro en el que se considera que el usuario esta en el muro
        modificaVelocidad = 0.1f; // para aminorar la velocidad cuando el usuario esta en un muro

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para la luz "Foco luz" (iluminacion)
        // La distancia de la luz al usuario
        distanciaLuzUsuario = new Vector3(0.0f, 0.0f, distancia_z_LuzUsuario);

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para la Tramoya
        // Las dimensiones de la tramoya
        escala_x_Tramoya = escalaGeneralTramoya; // La escala define el tamaño de un cuadrante. Si es 5, el muro de usuario es de 10x10
        escala_y_Tramoya = (escalaGeneralTramoya / ratio_dimensiones_Pantalla) * ratioOcupacionTramoya;
        escala_z_Tramoya = 1f / 10f; // No sde muy bien porque lo tengo que hacer mas estrecho, pero si no sale muy grueso
        escalaTramoya = new Vector3(escala_x_Tramoya, escala_y_Tramoya, escala_z_Tramoya);
        // La posicion de la tramoya
        //        positionTramoya = new Vector3(0.0f, ratioOcupacionTramoya, distancia_z_Tramoya); // A la distancia correspondiente del usuario y ocupando la parte superior de la pantalla
        positionTramoya = new Vector3(0.0f, ((escalaGeneralTramoya / ratio_dimensiones_Pantalla) / 2f) * (1f - ratioOcupacionTramoya), distancia_z_Tramoya); // A la distancia correspondiente del usuario y ocupando la parte superior de la pantalla
        ratioEscalaElementosTramoya = 1f / 2f;

        // /////////////////////////////////////////Tramoya
        // /////////////////////////////////////////
        // Para los telones
        escalaGeneralTelones = 1f;
        distanciaTelones = new Vector3(0.0f, 0.0f, distancia_z_Telones);
        // Las dimensiones del muro de usuario
        escala_x_Telones = escalaGeneralTelones; // La escala define el tamaño de un cuadrante. Si es 5, el muro de usuario es de 10x10
        escala_y_Telones = escalaGeneralTelones;
        escala_z_Telones = 1f;
        escalaTelones = new Vector3(escala_x_Telones, escala_y_Telones, escala_z_Telones);

        positionTelones = new Vector3(0.0f, escala_y_Telones, 0.0f); // A la distancia correspondiente del usuario y ocupando la parte superior de la pantalla
        giroOrientacionTelones = Quaternion.Euler(0f, 0f, 0f);


        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para el MURO DE USUARIO
        // La distancia del muro de usuario al usuario
        distanciaMuroUsuario = new Vector3(0.0f, 0.0f, distancia_z_MuroUsuario);
        // Las dimensiones del muro de usuario
        escala_x_MuroUsuario = escalaGeneralMuroUsuario;
        escala_y_MuroUsuario = escalaGeneralMuroUsuario / ratio_dimensiones_Pantalla;
        escala_z_MuroUsuario = 1f / 10f; // No sde muy bien porque lo tengo que hacer mas estrecho, pero si no sale muy grueso
        escalaMuroUsuario = new Vector3(escala_x_MuroUsuario, escala_y_MuroUsuario, escala_z_MuroUsuario);

        // Para la orientacion espacial
        giroOrientacionMuroUsuario = Quaternion.Euler(0f, 0f, 0f);


        // ///////////////
        // Parametros de los botonesw del muro de usuario

        // Parametros de escala y posicion
        float escalaGeneral_N_1_x = factorGeneralTamañoObgetos * escala_geneal_BtnMenu_MuroUsuario;
        float escalaGeneral_N_1_y = escalaGeneral_N_1_x * ratio_dimensiones_Pantalla;  // Para corregir el efecto panoramico y hacer los botones cuadrados


        // Para las consultas modales al usuario (SI o No) (selecciones, Tec... que se realizan bloqueando el resto de la aplicacion hast que se contesta
        // OJOO, los objetos de este tipo de consulta deben ser hijos del muro de usuario
        // Lo escalamos a un tamaño de la mitad del muro de usuario
        escala_x_ConsultaUsr_tipo01 = 1f / 2f;
        escala_y_ConsultaUsr_tipo01 = 1f / 2f; ;
        escala_z_ConsultaUsr_tipo01 = 1f;
        escala_ConsultaUsr_tipo01 = new Vector3(escala_x_ConsultaUsr_tipo01, escala_y_ConsultaUsr_tipo01, escala_z_ConsultaUsr_tipo01); ;
        // Lo colocamos en el centro del muo de usuario
        posicion_x_ConsultaUsr_tipo01 = 0f;
        posicion_y_ConsultaUsr_tipo01 = 0f;
        posicion_z_ConsultaUsr_tipo01 = 0f;
        posicion_ConsultaUsr_tipo01 = new Vector3(posicion_x_ConsultaUsr_tipo01, posicion_y_ConsultaUsr_tipo01, posicion_z_ConsultaUsr_tipo01);

        posicion_textoSinTecho = new Vector3(0f, 0f, 0f); // Lo ponemos en el mismo sitio que las consultas

        // numeros de botones
        NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz = 6;
        NumBtn_BtnMenu_N1_1_Gereral_Escena = 4;
        //NumBtn_BtnMenu_N1_1_Gereral_Herramientas = 3;
        // Nicolas
        NumBtn_BtnMenu_N1_1_Gereral_Herramientas = 4;
        NumBtn_BtnMenu_N1_1_General_Salir = 0;

        /// ///////////////// Nivel 1 de botones Es el contenedor de los botones de CtrlInterfaz, escena y herramientas (Suelo de 4 baldosas, tres ocupadas)
        escala_x_BtnMenu_N1_General = escalaGeneral_N_1_x; // El boton es 1/5 del la escala del muro si la escala es 5, el muro es de 10x10, y el boton es uno
        escala_y_BtnMenu_N1_General = escalaGeneral_N_1_y; // igual que en x
        escala_z_BtnMenu_N1_General = 1f;  // el muro es un cubo casi plano, solo tiene un poco de profundidad en y, para ajustar los colliders
        escala_BtnMenu_N1_General = new Vector3(escala_x_BtnMenu_N1_General, escala_y_BtnMenu_N1_General, escala_z_BtnMenu_N1_General);
        // Posicion de los botones de nmivel 1 
        posicion_x_BtnMenu_N1_General = (1f / 2f) - (escalaGeneral_N_1_x / 2f);  // Para que quede en la esquina pero dentro
        posicion_y_BtnMenu_N1_General = (-1f / 2f) + (escalaGeneral_N_1_y / 2f); // es negativo porque va haci abajo
        posicion_z_BtnMenu_N1_General = 0f;
        posicion_BtnMenu_N1_General = new Vector3(posicion_x_BtnMenu_N1_General, posicion_y_BtnMenu_N1_General, posicion_z_BtnMenu_N1_General);
        // Posicion de los botones de nmivel_1 _1 ( son los botones de CtrlInterfaz, escena y herramientas) 
        // Para el menu de  de menu de usuario "BtnMenu_N1_1_Gereral_CtrlInterfaz" dentro de "BtnMenu_N1_General" (Baldosa arriba derecha)
        posicion_BtnMenu_N1_1_Gereral_CtrlInterfaz = new Vector3(1f / 4f, 1f / 4f, 0f); // el ancal aesta en el centro de la baldosa, los valores son para colocarla arriba a la derecha
        escala_BtnMenu_N1_1_Gereral_CtrlInterfaz = new Vector3(1f / 2f, 1f / 2f, 1f);  // CUatro baldosas cubren el cuadro (la coordenada Z es del grosor de su padre (boton hijo del muro de usuario)
                                                                                       // Para el boton fijo de menu de usuario "BtnMenu_N1_1_Gereral_Escena" dentro de "BtnMenu_N1_General" (Baldosa arriba izquierda)
        posicion_BtnMenu_N1_1_Gereral_Escena = new Vector3(-1f / 4f, 1f / 4f, 0f); // Igual que general
        escala_BtnMenu_N1_1_Gereral_Escena = new Vector3(1f / 2f, 1f / 2f, 1f);
        // Para el boton fijo de menu de usuario dentro de "BtnMenu_N1_General" (baldosa abajo izauierda)
        posicion_BtnMenu_N1_1_Gereral_Herramientas = new Vector3( -1f / 4f, -1f / 4f, 0f);// Igual que general
        escala_BtnMenu_N1_1_Gereral_Herramientas = new Vector3(1f / 2f, 1f / 2f, 1f);
        // Para el boton fijo de menu de usuario dentro de "BtnMenu_N1_General" (baldosa abajo derecha)
        posicion_BtnMenu_N1_1_General_Salir = new Vector3(1f / 4f, -1f / 4f, 0f);// Igual que general
        escala_BtnMenu_N1_1_General_Salir = new Vector3(1f / 2f, 1f / 2f, 1f);

        // ///////////////// Nivel 2 de botones
        // Para el menu de  de menu de control de interfaz "BtnMenu_N2_CtrlInterfaz" dentro de "BtnMenu_N1_General"
        //		posicion_BtnMenu_N2_CtrlInterfaz = new Vector3 ((5f - (escala_x_BtnMenu_N1_General / 2f)), 0f, (-5f + (escala_z_BtnMenu_N1_General * 3/2f)));
        //       posicion_BtnMenu_N2_CtrlInterfaz = new Vector3 ((5f - (escala_x_BtnMenu_N1_General / 2f)), 0f, (-5f + (escala_z_BtnMenu_N1_General)));
        //  posicion_BtnMenu_N2_CtrlInterfaz = new Vector3((escalaGeneralMuroUsuario - (escala_x_BtnMenu_N1_General / 2f)), 0f, (-escalaGeneralMuroUsuario + (escala_z_BtnMenu_N1_General) + (escala_z_BtnMenu_N1_General)*(NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz/2)));
        posicion_BtnMenu_N2_CtrlInterfaz = new Vector3(((1f / 2f) - (escala_x_BtnMenu_N1_General / 2f)), (-(1f / 2f) + (escala_y_BtnMenu_N1_General) + (escala_y_BtnMenu_N1_General) * (NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz / 2)), 0f);
        escala_BtnMenu_N2_CtrlInterfaz = new Vector3(escalaGeneral_N_1_x, escalaGeneral_N_1_y * NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 1f);

        // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
        posicion_BtnMenu_N2_1_CtrlInterfaz_cargar = new Vector3(0f, -2.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_cargar = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_cargar_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        // Para eol boton de grabar el estado de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_grabar"
        posicion_BtnMenu_N2_1_CtrlInterfaz_grabar = new Vector3(0f, -1.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_grabar = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_grabar_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        // Para eol boton de gestion del audio "BtnMenu_N2_1_CtrlInterfaz_audio"
        posicion_BtnMenu_N2_1_CtrlInterfaz_audio = new Vector3(0f, -0.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_audio = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_audio_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        // Para eol boton de gestion del la complejidad de la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_complejidad"
        posicion_BtnMenu_N2_1_CtrlInterfaz_complejidad = new Vector3(0f, 0.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_complejidad = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_complejidad_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        // Para eol boton de gestion de la configuracion de ideomas que gestiona el usuario desde la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_idiomas"
        posicion_BtnMenu_N2_1_CtrlInterfaz_idiomas = new Vector3(0f, 1.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_idiomas = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_idiomas_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        // Para eol boton de gestion de la configuracion de ideomas que gestiona el usuario desde la interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_ambito"
        posicion_BtnMenu_N2_1_CtrlInterfaz_ambito = new Vector3(0f, 2.5f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz, 0f);
        escala_BtnMenu_N2_1_CtrlInterfaz_ambito = new Vector3(factorReduccion01, ((1 - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);
        escala_BtnMenu_N2_1_CtrlInterfaz_ambito_Activado = new Vector3(1f, (1f / NumBtn_BtnMenu_N1_1_Gereral_CtrlInterfaz), 1f);


        // Para el boton fijo de menu de control de escena "BtnMenu_N2_Escena" dentro de "BtnMenu_N1_General"
        //		posicion_BtnMenu_N2_Escena = new Vector3 ((escalaGeneralMuroUsuario - (escala_x_BtnMenu_N1_General * 3f/2f)), 0f, (-escalaGeneralMuroUsuario + (escala_z_BtnMenu_N1_General)));
        //        posicion_BtnMenu_N2_Escena = new Vector3(((1f / 2f) - ((3f/2f) * escala_x_BtnMenu_N1_General)), (-(1f / 2f) + ((3f / 2f) * escala_y_BtnMenu_N1_General)), 0f);
        posicion_BtnMenu_N2_Escena = new Vector3(((1f / 2f) - ((2f) * escala_x_BtnMenu_N1_General)), (-(1f / 2f) + ((2f) * escala_y_BtnMenu_N1_General)), 0f);
        escala_BtnMenu_N2_Escena = new Vector3(2f * escalaGeneral_N_1_x, 2f * escalaGeneral_N_1_y, 1f);  // Va a albergar cuatro botones (un mosaico de 2x2 es igual que el boton de general)

        // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
        posicion_BtnMenu_N2_1_Escena_CreaMuro = new Vector3(0.25f, -0.25f, 0f);
        escala_BtnMenu_N2_1_Escena_CreaMuro = new Vector3(factorReduccion01 / 2, factorReduccion01 / 2, 1f); // Dividimos "factorReduccion01 / 2", ya que la baldosa madre es de 2x2 baldosas hijas
        escala_BtnMenu_N2_1_Escena_CreaMuro_Activado = new Vector3(1f / 2, 1f / 2, 1f);
        // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
        posicion_BtnMenu_N2_1_Escena_EliminaMuro = new Vector3(0.25f, 0.25f, 0f);
        escala_BtnMenu_N2_1_Escena_EliminaMuro = new Vector3(factorReduccion01 / 2, factorReduccion01 / 2, 1f);
        escala_BtnMenu_N2_1_Escena_EliminaMuro_Activado = new Vector3(1f / 2, 1f / 2, 1f);
        // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
        posicion_BtnMenu_N2_1_Escena_GeneraRama = new Vector3(-0.25f, -0.25f, 0f);
        escala_BtnMenu_N2_1_Escena_GeneraRama = new Vector3(factorReduccion01 / 2, factorReduccion01 / 2, 1f);
        escala_BtnMenu_N2_1_Escena_GeneraRama_Activado = new Vector3(1f / 2, 1f / 2, 1f);
        // Para eol boton de cargar el estado de una interfaz de usuario "BtnMenu_N2_1_CtrlInterfaz_cargar"
        posicion_BtnMenu_N2_1_Escena_CreaConcepto = new Vector3(-0.25f, 0.25f, 0f);
        escala_BtnMenu_N2_1_Escena_CreaConcepto = new Vector3(factorReduccion01 / 2, factorReduccion01 / 2, 1f);
        escala_BtnMenu_N2_1_Escena_CreaConcepto_Activado = new Vector3(1f / 2, 1f / 2, 1f);


        // Para el boton fijo de menu de herramientas dentro de "BtnMenu_N2_Herramientas"
        //        posicion_BtnMenu_N2_Herramientas = new Vector3(((1f / 2f) - (escala_x_BtnMenu_N1_General * 3f / 2f) - (escala_x_BtnMenu_N1_General) * (NumBtn_BtnMenu_N1_1_Gereral_Herramientas / 2f)), -(1f / 2f) + escala_y_BtnMenu_N1_General / 2f, 0f);
        posicion_BtnMenu_N2_Herramientas = new Vector3(((1f / 2f) - (escala_x_BtnMenu_N1_General) - (escala_x_BtnMenu_N1_General) * (NumBtn_BtnMenu_N1_1_Gereral_Herramientas / 2f)), -(1f / 2f) + escala_y_BtnMenu_N1_General / 2f, 0f);
        escala_BtnMenu_N2_Herramientas = new Vector3(escalaGeneral_N_1_x * NumBtn_BtnMenu_N1_1_Gereral_Herramientas, escalaGeneral_N_1_y, 1f);

        // Nicolas Merino Ramirez
        // Fecha 13/10/2022
        // Sumo ".12f" en las posiciones de los subbotones del boton de herramientas para cuadrar los subbotones
        // y que no se salgan de su rectangulo contenedor

        // Para eol boton de acceso al evi de busqueda "BtnMenu_N2_1_Herramientas_buscador"
        posicion_BtnMenu_N2_1_Herramientas_buscador = new Vector3((1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas) +.12f, 0f, 0f);
        escala_BtnMenu_N2_1_Herramientas_buscador = new Vector3(((1f - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), factorReduccion01, 1f);
        escala_BtnMenu_N2_1_Herramientas_buscador_Activado = new Vector3((1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), 1f, 1f);
        // Para eol boton de acceso a la mochila "BtnMenu_N2_1_Herramientas_mochila"
        posicion_BtnMenu_N2_1_Herramientas_mochila = new Vector3((0f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas) + .12f, 0f, 0f);
        escala_BtnMenu_N2_1_Herramientas_mochila = new Vector3(((1f - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), factorReduccion01, 1f);
        escala_BtnMenu_N2_1_Herramientas_mochila_Activado = new Vector3((1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), 1f, 1f);
        // Para eol boton de acceso a herramientas "BtnMenu_N2_1_CtrlInterfaz_audio"
        posicion_BtnMenu_N2_1_Herramientas_agentes = new Vector3((-1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas) + .12f, 0f, 0f);
        escala_BtnMenu_N2_1_Herramientas_agentes = new Vector3(((1f - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), factorReduccion01, 1f);
        escala_BtnMenu_N2_1_Herramientas_agentes_Activado = new Vector3((1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas), 1f, 1f);
        
        
        // Autor Nicolas Merino Ramirez
        // Fecha 27/06/2022
        // Descripcion
        //      Calculo de los datos de inicializacion para el boton del breadcrubms trail o migas de pan
        //       - posicion
        //       - escala
        //       - escala de activacion (hover)

        this.posicion_BtnBreadcrumbsTrails = new Vector3
            (
                (-2f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas) + .12f,
                0f,
                0f
            );

        // Roto el icono de las migas de pan 180 grados (para que no este al reves)
        this.rotacion_BtnBreadcrumbsTrails = new Quaternion(0, 180, 0, 0);

        // Calculo la escala del boton
        this.escala_BtnBreadcrumbsTrails = new Vector3
            (
                ((1f - factorReduccion01) / NumBtn_BtnMenu_N1_1_Gereral_Herramientas),
                factorReduccion01,
                1f
            );

        this.escala_BtnBreadcrumbsTrails_Activado = new Vector3
            (
                (1f / NumBtn_BtnMenu_N1_1_Gereral_Herramientas),
                1f,
                1f
            );

        

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para los muros de Trabajo
        // Calculamos el tamaño de los muros de trabajo
        escala_x_MuroTrabajo = escalaGeneralMuroTrabajo;
        escala_y_MuroTrabajo = escalaGeneralMuroTrabajo / ratio_dimensiones_Pantalla;
        //        escala_y_MuroTrabajo = escalaGeneralMuroTrabajo;
        escala_z_MuroTrabajo = 1f / 10f;
        escalaMuroTrabajo = new Vector3(escala_x_MuroTrabajo, escala_y_MuroTrabajo, escala_z_MuroTrabajo);

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log(" Desde calculaInterfaz con - escala_x_MuroTrabajo = " + escala_x_MuroTrabajo +
            " - escala_y_MuroTrabajo = " + escala_y_MuroTrabajo +
            " - escala_z_MuroTrabajo = " + escala_z_MuroTrabajo +
            " - ratio_dimensiones_Pantalla = " + ratio_dimensiones_Pantalla
            );
        }

        escalaGrosorMuroTrabajo = escalaGeneralMuroTrabajo / 100; // Se utiliza para el grosor de los elementos (EVIs) que cuelgan del muro (el muro es un plano y no tiene grosor

        // La orientacion (al definir el gme objet, aparece tumbado, por lo que lo guiramos para poder tomar los eje X e Y como el plano vista
        giroOrientacion_x_MuroTrabajo = 0f;
        giroOrientacion_y_MuroTrabajo = 0f;
        giroOrientacion_z_MuroTrabajo = 0f;

        giroOrientacionMuroTrabajo = Quaternion.Euler(giroOrientacion_x_MuroTrabajo, giroOrientacion_y_MuroTrabajo, giroOrientacion_z_MuroTrabajo);


        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para el CanvasGeneral. Este es un canvas unico que se utiliza para poner informacion en toda la pantalla
        // Calculamos el tamaño y los tamaños de los elementos que contiene

        //        CanvasGeneral.transform.Width  = pixels_x_Pantalla;
        //        CanvasGeneral.transform.Height = pixels_y_Pantalla;
        //    PanelCanvasCompleto; definido desde el framework
        //    TextCanvasGeneralCompleto; definido desde el framework
        Vector3 escala_BotonCerrarPanelCanvasCompleto = new Vector3(1f, 1f, 1f);
        BotonCerrarPanelCanvasCompleto.transform.localScale = escala_BotonCerrarPanelCanvasCompleto;
        Vector3 posicion_BotonCerrarPanelCanvasCompleto = new Vector3(-pixels_x_Pantalla / 2f, -pixels_y_Pantalla / 2f, 0f);
        BotonCerrarPanelCanvasCompleto.transform.localPosition = posicion_BotonCerrarPanelCanvasCompleto;






        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Para los evis
        // Solo se define aqui el valor de la escala, las posiciones etc, se definen en los scripts asociados a cada tipo de evi

        // Parametros de la base general de los EVIs
        escalaLocal_x_BaseDeEvi_01 = factorEscalaBaseDeEvi;
        escalaLocal_y_BaseDeEvi_01 = escalaLocal_x_BaseDeEvi_01 * ratio_dimensiones_Pantalla;
        escalaLocal_z_BaseDeEvi_01 = 1f;
        estaEscalaBaseDeEvi_01 = new Vector3(escalaLocal_x_BaseDeEvi_01, escalaLocal_y_BaseDeEvi_01, escalaLocal_z_BaseDeEvi_01);

        // Parametros de la base general de los EVIs Para cuando son ahijados en la tramoya
        escalaLocal_x_BaseDeEviEnTramoya = factorEscalaBaseDeEvi * ratioEscalaElementosTramoya;
        escalaLocal_y_BaseDeEviEnTramoya = escalaLocal_x_BaseDeEviEnTramoya * (escalaTramoya.x / escalaTramoya.y);  // Ajustamos la escala y para que sea proporcional a la de la tramoya
        escalaLocal_z_BaseDeEviEnTramoya = 1f;
        estaEscalaBaseDeEviEnTramoya = new Vector3(escalaLocal_x_BaseDeEviEnTramoya, escalaLocal_y_BaseDeEviEnTramoya, escalaLocal_z_BaseDeEviEnTramoya);

        // Parametros de evis tipo EviTipo_00
        escalaLocal_x_EviTipo_00 = 1f / 10f;
        escalaLocal_y_EviTipo_00 = 1f / 10F;
        escalaLocal_z_EviTipo_00 = 1f;
        estaEscalaEviTipo_00 = new Vector3(escalaLocal_x_EviTipo_00, escalaLocal_y_EviTipo_00, escalaLocal_z_EviTipo_00);

        // Parametros de evis tipo EviFractal_00
        escalaLocal_x_EviFractal_00 = 1f / 10f;
        escalaLocal_y_EviFractal_00 = 1f / 10f;
        escalaLocal_z_EviFractal_00 = 1f;
        estaEscalaEviFractal_00 = new Vector3(escalaLocal_x_EviFractal_00, escalaLocal_y_EviFractal_00, escalaLocal_z_EviFractal_00);

        // Parametros del CONTENEDOR de evis tipo EviFractal_00 que almacenan informacion de Sin Techo (queremos hacerlo mas alargado y ponerlo a la derecha
        razon_escalaLocal_x_ContenedorEviSinTecho = 3f;
        razon_escalaLocal_y_ContenedorEviSinTecho = 1f / 5f;
        razon_escalaLocal_z_ContenedorEviSinTecho = 1f;


        // Parametros de evis tipo EviTipo_muestraGetDetails_00
        escalaLocal_x_EviTipo_muestraGetDetails_00 = 1f / 10f;
        escalaLocal_y_EviTipo_muestraGetDetails_00 = 1f / 10f;
        escalaLocal_z_EviTipo_muestraGetDetails_00 = 1f;
        estaEscalaEviTipo_muestraGetDetails_00 = new Vector3(escalaLocal_x_EviTipo_muestraGetDetails_00, escalaLocal_y_EviTipo_muestraGetDetails_00, escalaLocal_z_EviTipo_muestraGetDetails_00);
        // Parametros de evis tipo EviFractal_00
        //       escalaLocal_x_EviFractal_00 = escalaGeneralMuroTrabajo / 10;
        //       escalaLocal_y_EviFractal_00 = escalaGrosorMuroTrabajo;
        //       escalaLocal_z_EviFractal_00 = escalaGeneralMuroTrabajo / 10;
        //       estaEscalaEviFractal_00 = new Vector3(escalaLocal_x_EviFractal_00, escalaLocal_y_EviFractal_00, escalaLocal_z_EviFractal_00);

        // Parametros de evis de busqueda tipo EviTipo_buscador_00
        escalaLocal_x_EviTipo_buscador_00 = 1f;
        escalaLocal_y_EviTipo_buscador_00 = 1f;
        escalaLocal_z_EviTipo_buscador_00 = 1f;
        estaEscalaEviTipo_buscador_00 = new Vector3(escalaLocal_x_EviTipo_buscador_00, escalaLocal_y_EviTipo_buscador_00, escalaLocal_z_EviTipo_buscador_00);

        factor_mod_escalaLocal_EviTipo_buscador_00_Activado = 1.1f; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

        escalaLocal_x_EviTipo_buscador_00_Activado = escalaLocal_x_EviTipo_buscador_00 * factor_mod_escalaLocal_EviTipo_buscador_00_Activado;
        escalaLocal_y_EviTipo_buscador_00_Activado = escalaLocal_y_EviTipo_buscador_00 * factor_mod_escalaLocal_EviTipo_buscador_00_Activado; // solo lo modificamos en las dimensiones del muro de trabajo
        escalaLocal_z_EviTipo_buscador_00_Activado = escalaLocal_z_EviTipo_buscador_00 * factor_mod_escalaLocal_EviTipo_buscador_00_Activado;
        estaEscalaEviTipo_buscador_00_Activado = new Vector3(escalaLocal_x_EviTipo_buscador_00_Activado, escalaLocal_y_EviTipo_buscador_00_Activado, escalaLocal_z_EviTipo_buscador_00_Activado);

        // Parametros de evis de Rama tipo EviTipo_Rama_00
        escalaLocal_x_EviTipo_Rama_00 = 1f;
        escalaLocal_y_EviTipo_Rama_00 = 1f;
        escalaLocal_z_EviTipo_Rama_00 = 1f;
        estaEscalaEviTipo_Rama_00 = new Vector3(escalaLocal_x_EviTipo_Rama_00, escalaLocal_y_EviTipo_Rama_00, escalaLocal_z_EviTipo_Rama_00);

        factor_mod_escalaLocal_EviTipo_Rama_00_Activado = 1.1f; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

        escalaLocal_x_EviTipo_Rama_00_Activado = escalaLocal_x_EviTipo_Rama_00 * factor_mod_escalaLocal_EviTipo_Rama_00_Activado;
        escalaLocal_y_EviTipo_Rama_00_Activado = escalaLocal_y_EviTipo_Rama_00 * factor_mod_escalaLocal_EviTipo_Rama_00_Activado; // solo lo modificamos en las dimensiones del muro de trabajo
        escalaLocal_z_EviTipo_Rama_00_Activado = escalaLocal_z_EviTipo_Rama_00 * factor_mod_escalaLocal_EviTipo_Rama_00_Activado;
        estaEscalaEviTipo_Rama_00_Activado = new Vector3(escalaLocal_x_EviTipo_Rama_00_Activado, escalaLocal_y_EviTipo_Rama_00_Activado, escalaLocal_z_EviTipo_Rama_00_Activado);

        // Parametros de evis de Rama tipo EviTipo_RefFractal_01
        escalaLocal_x_EviTipo_RefFractal_01 = 1f;
        escalaLocal_y_EviTipo_RefFractal_01 = 1f;
        escalaLocal_z_EviTipo_RefFractal_01 = 1f;
        estaEscalaEviTipo_RefFractal_01 = new Vector3(escalaLocal_x_EviTipo_RefFractal_01, escalaLocal_y_EviTipo_RefFractal_01, escalaLocal_z_EviTipo_RefFractal_01);

        factor_mod_escalaLocal_EviTipo_RefFractal_01_Activado = 1.1f; // mediante este factor modificamos el tamaño del evi buscador al seleccionarlo

        escalaLocal_x_EviTipo_RefFractal_01_Activado = escalaLocal_x_EviTipo_RefFractal_01 * factor_mod_escalaLocal_EviTipo_RefFractal_01_Activado;
        escalaLocal_y_EviTipo_RefFractal_01_Activado = escalaLocal_y_EviTipo_RefFractal_01 * factor_mod_escalaLocal_EviTipo_RefFractal_01_Activado; // solo lo modificamos en las dimensiones del muro de trabajo
        escalaLocal_z_EviTipo_RefFractal_01_Activado = escalaLocal_z_EviTipo_RefFractal_01 * factor_mod_escalaLocal_EviTipo_RefFractal_01_Activado;
        estaEscalaEviTipo_RefFractal_01_Activado = new Vector3(escalaLocal_x_EviTipo_RefFractal_01_Activado, escalaLocal_y_EviTipo_RefFractal_01_Activado, escalaLocal_z_EviTipo_RefFractal_01_Activado);

        baseDePotenciaFractal = 2f; // La potencia de los evis fractales es 2 porque estos son cuadrados

    } // Fin de - void calculaInterfaz () {


    /// ////////////////////////////////////////////////////////////////////////////////////// 
    /// ///////////  Genera y entrega un identificador unico para todos los elementos de interfaz (ramas, muros, ecis, agentes, solicitudes, etc...)
    // Esta funcion envia un identificador unico (que va incrementando) cada vez que se le solicita
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-01-29
    // Observaciones :
    // 	    Cada elemento de la interfaz debe tener su identificador unico distinto de todos
    //      OJOO el identificador que aqui se genera es "idElementIntf" qe es unico entre todos lo elementos de interfaz (ramas, muros, agentes, evis, solicitudes,...)
    //      hay otro identificador "idElementEnTipo" que es unico solo en el tipo segun "tipoElementIntf
    public int generaIdElementIntf()
        {
		int identificador = ultimoIdElementIntf;
        ultimoIdElementIntf++;
        return identificador;
    }  // Fin de - public int generaIdElementIntf()

    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Genera y entrega un identificador unico para cada elemento dentro de un tipo
    // Esta funcion envia un identificador unico (que va incrementando) cada vez que se le solicita
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-01-29
    // Observaciones :
    // 	    Cada elemento de la interfaz debe tener su identificador unico distinto de todos
    //      OJOO el identificador que aqui se genera es  "idElementEnTipo" que es unico solo en el tipo segun "tipoElementIntf"
    //      hay otro identificador "idElementIntf" qe es unico entre todos lo elementos de interfaz (ramas, muros, agentes, evis, solicitudes,...)
    public int generaIdElementEnTipo(string tipoElement)
    {
        int identificador = 0;

        if (tipoElement == "rama")
            {
            identificador = ultimoIdElementTipoRama;
            ultimoIdElementTipoRama++;
            }
        else if (tipoElement == "muro")
            {
            identificador = ultimoIdElementTipoMuro;
            ultimoIdElementTipoMuro++;
            }
        else if (tipoElement == "evi")
            {
            identificador = ultimoIdElementTipoEvi;
            ultimoIdElementTipoEvi++;
            }
        else if (tipoElement == "solicitud")
            {
            identificador = ultimoIdElementTipoSolicitud;
            ultimoIdElementTipoSolicitud++;
            }
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Error al intentar generar el identificador de elemento en un tipo. Con identificador : " + identificador + "- con solicitud de tipo  : " + tipoElement); }
        }
        
        return identificador;
    }  // Fin de - public int generaIdElementEnTipo()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  calculaMuroUsuario()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-27
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///      El tamaño del muro de usuario depende de la distancia a la que esta de la camara y del campo de vision de esta, para que este entre justo dentro de este campo 
    ///      de vision. 
    ///      - Iba a poner esta funcion en "ScriptCtrlMuroUsuario", pero se le llama aqui antes de que este generado el muro de usuario y da problemas, Asique la pongo aqui
    /// </summary>
    public void calculaMuroUsuario()
    {
        // - El cateto opuesto (1/2 del tamaño del muro) es igual al cateto contiguo (distancia de la camara al muro de usuario) multiplicado por la tangente 
        // del angulo (la mitad del angulo del campo de vision de la camara)

        float anguloEnRadianes = (float)(Math.PI * campoVisionCamara) / (2f * 180f); // El campo de vision de la camara se define para el eje y
        float mitadDeTamaño_y_MuroUsuario = (float)Math.Tan(anguloEnRadianes) * (Math.Abs(distancia_z_CamaraUsuario) - Math.Abs(distancia_z_MuroUsuario));

        float mitadDeTamaño_x_MuroUsuario = mitadDeTamaño_y_MuroUsuario * ratio_dimensiones_Pantalla;
        // El campo de vison de la camara se ha ajustado al tamaño del menor de los ejes x e y.
        // La escala general del muro de usuario, y el resto de parametros generales estan definidos para que se ajusten atendiendo al eje x

        float estaEscalaGeneralMuroUsuario = mitadDeTamaño_x_MuroUsuario - ((mitadDeTamaño_x_MuroUsuario * marcoPorCiento) / 100f); // Le quito el % del marco
        // float estaEscalaGeneralMuroUsuario = mitadDeTamaño_x_MuroUsuario; // Le o pongo para probar pero luego lo vuelavo a quitar ojo-2021-12-28        
        escalaGeneralMuroUsuario = estaEscalaGeneralMuroUsuario * 2f; // multipolico por 2 para todo el muro
    }  // Fin de - public void calculaMuroUsuario()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  calculaTramoya()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-27
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///      Al igual que el muro de usuario, el tamaño de la Tramoya depende de la distancia a la que esta de la camara y del campo de vision de esta, para que este entre justo dentro de este campo 
    ///      de vision. 
    /// </summary>
    public void calculaTramoya()
    {
        // - El cateto opuesto (1/2 del tamaño del muro) es igual al cateto contiguo (distancia de la camara al muro de usuario) multiplicado por la tangente 
        // del angulo (la mitad del angulo del campo de vision de la camara)

        float anguloEnRadianes = (float)(Math.PI * campoVisionCamara) / (2f * 180f); // El campo de vision de la camara se define para el eje y
        float mitadDeTamaño_y_Tramoya = (float)Math.Tan(anguloEnRadianes) * (Math.Abs(distancia_z_CamaraUsuario) - Math.Abs(distancia_z_Tramoya));

        float mitadDeTamaño_x_Tramoya = mitadDeTamaño_y_Tramoya * ratio_dimensiones_Pantalla;
        // El campo de vison de la camara se ha ajustado al tamaño del menor de los ejes x e y.
        // La escala general del muro de usuario, y el resto de parametros generales estan definidos para que se ajusten atendiendo al eje x

        float estaEscalaGeneralTramoya = mitadDeTamaño_x_Tramoya - ((mitadDeTamaño_x_Tramoya * marcoPorCiento) / 100f); // Le quito el % del marco

        escalaGeneralTramoya = estaEscalaGeneralTramoya * 2f; // multipolico por 2 para todo el muro
    }  // Fin de - public void calculaTramoya()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  manejaTransparenciaMaterial()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-07-02
    /// Variables de entrada :
    ///         - GameObject objetoAManejar = Es el objeto al que hay que ajustarle la transparencia
    ///         - float nivelTransparencia = es el mnivel de transparencia que hay que poner en el material del objeto que nos pasan
    /// Variables de salida :
    /// Observaciones :
    ///      Los valores de transparencia estan distribuidos en sus definiciones a lo largo del proyecto. Enunciamos aqui los que maneja este metodo:
    ///             - nivelTransparenciaMuroActivo  = Nivel de transparencia para el muro activo
    ///             - nivelTransparenciaMuroInactivo = nivel de transparencia para los muros inactivos
    ///      - Los modos de los elementos se identifican con el color (navegacion blanco, edicion amarillo...; pero la activacion de los elementos, se 
    ///      identifica por el nivel de transparencia (activo mas opaco, inactivo mas transparente)

    /// </summary>
    public void manejaTransparenciaMaterial(GameObject objetoAManejar, float nivelTransparencia)
    {

        Color colorModificado = new Color();
        //        float trnaspColorAnterior = objetoAManejar.GetComponent<Material>().color.a;
        float trnaspColorAnterior = objetoAManejar.GetComponent<MeshRenderer>().material.color.a;
        Color soloTranspAnterior = new Color(0f, 0f, 0f, trnaspColorAnterior);
        Color soloTranspFinal = new Color(0f, 0f, 0f, nivelTransparencia);
//        Color colorAnterior = objetoAManejar.GetComponent<MeshRenderer>().material.color;
        colorModificado = objetoAManejar.GetComponent<MeshRenderer>().material.color;
        colorModificado = colorModificado - soloTranspAnterior;
        colorModificado = colorModificado + soloTranspFinal;

//        objetoAManejar.GetComponent<MeshRenderer>().material.SetColor("color", colorModificado);
//        objetoAManejar.GetComponent<Material>().SetColor("color", colorModificado);
        objetoAManejar.GetComponent<MeshRenderer>().material.SetColor("_Color", colorModificado);

        //       objeto.GetComponent<MeshRenderer>().material = materialMuro;   // materialMuroActivo
        //            other.gameObject.GetComponent<MeshRenderer>().material.
    }  // Fin de - public void manejaTransparenciaMaterial(GameObject objetoAManejar, float nivelTransparencia)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  gestionaEnPerRefracBotonMouse : controla el periodo refractario del boton del mouse
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-04-23
    /// Variables de entrada :
    /// Variables de salida :
    ///   - enPeriodoDeRefraccion 
    ///       - false = el periodo de refraccion NO ESTA ACTIVADO. La operacion asociada al click del mouse puede realizarse (a partir de este momento se activa el periodo de refraccion)
    ///       - true = el periodo de refraccion SI ESTA ACTIVADO. La operacion asociada al click del mouse NO debe realizarse (hay que esperar a que el periodo de activacion termine)
    /// Observaciones :
    ///     OJOOO cuando se llama a esta funcion ocurre lo siguiente
    ///         1) Si NO esta en periodo de refraccion (enPeriodoDeRefraccion = false) devuelve false e inicia un periodo de refraccion
    ///         2) SI esta en periodo de refraccion (enPeriodoDeRefraccion = true)
    ///         
    /// </summary>
    public bool gestionaEnPerRefracBotonMouse(GameObject quienDispara)
    {
        // SI QUIEN DISPARA NO ESTA ACTIVO,  no le dejamos disparar el evento
        if (quienDispara.GetComponent<ClassPuntero>()) // Si quien dispara es un puntero
        {
            if (quienDispara.GetComponent<ClassPuntero>().estadoPuntero != ClassPuntero.estPuntero_activo)
            {
                return true;  // Decimos que estamos en periodo de refraccion para que el puntero no pueda disparar
            }
        }

        // SI EL PERIODO DE REFRACCION NO ESTA ACTIVO,  lo ponemos activo, anotamos el momento del inicio del periodo de refraccion y devoolvemos false
        // para que quien lo llampueda ejecutar la orden asociada al clik del raton (no se debera ejecutar otra orden hasta que el periodo de refraccion
        // haya terminado
        if (!enPeriodoDeRefraccion)
        {
            enPeriodoDeRefraccion = true;
            iniciPerRefractBotonMouse = DateTime.Now;
            return false;
        }

        // SI EL PERIODO DE REFRACCION ESTA ACTIVO, miramos a ver si ha pasado el tiempo de refraccion. 

        DateTime tiempoActual = DateTime.Now;
        TimeSpan tiempoEnRefraccion = tiempoActual - iniciPerRefractBotonMouse;
        int tiempoEnRefraccion_ms = tiempoEnRefraccion.Milliseconds;

        if (tiempoEnRefraccion_ms < periodoRefractarioBotonMouse) // Si el tiempo de refraccion NO ha pasado devolvemos true para indicar que el periodo de refraccion esta activo
        {
            enPeriodoDeRefraccion = true;
        }
        else  // Si el tiempo de refraccion SI ha pasado devolvemos ponemos el estado a false para indicar que el periodo de refraccion ya no esta activo
        {
            enPeriodoDeRefraccion = true;
            iniciPerRefractBotonMouse = DateTime.Now;
            return false;
        }

        return enPeriodoDeRefraccion;

    }  // Fin de - public void ahijaRama(int idMadre, int idHija)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  dameNuevo_numDeConceptosGenerados - Esta interfaz puede generar conceptos (solicitud a DKS, DAUS u otros). esta funcion los genra de forma unnivoca
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-27
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///     Esta interfaz puede generar conceptos (solicitud a DKS, DAUS u otros). Para poder identificarlos de forma univoca se utiliza el indice "numDeConceptosGenerados" que se 
    ///     incrementa cada vez que esta interfaz genera un concepto nuevo.
    /// 
    /// 	    
    /// </summary>
    public int dameNuevo_numDeConceptosGenerados()
    {
        int nuevoNumeroConcepto;
        nuevoNumeroConcepto = numDeConceptosGenerados++;   // identificador de la relacion

        return nuevoNumeroConcepto;

    }  // Fin de - public int dameNuevo_numDeConceptosGenerados()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  dime_milisegundo_actual - me devuelve el milisegundo actual en un long.
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2022-03-04
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///     Quiero los tiempos en milisegundos globales. No he encontrado una manera de hacerlo mas sencilla. Asique lo hago asi
    /// 	    
    /// </summary>
    public long dime_milisegundo_actual()
    {
        DateTime tiempoActual = DateTime.Now;
        DateTime tiempoCero = new DateTime(1970, 1, 1);
        TimeSpan tiempoTranscurrido = tiempoActual - tiempoCero;
        double tiempoActual_ms_double = tiempoTranscurrido.TotalMilliseconds;
        long tiempoActual_ms = Convert.ToInt64(tiempoActual_ms_double);

        return tiempoActual_ms;

    }  // Fin de - public int dameNuevo_numDeConceptosGenerados()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Inserta en la lista "arbolIntf" la relacion madre-hija de una rama del arbol de la interfaz
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-18
    /// Variables de entrada :
    ///     int idMadre : Es el identificador de elemento de interfaz "idElementIntf" de la rama madre
    ///     int idHija : Es el identificador de elemento de interfaz "idElementIntf" de la rama hija
    /// Variables de salida :
    /// Observaciones :
    ///     Por ahora no se utiliza porque el arbol se registra en las listas de hijos de los elementos de interfaz
    /// 
    /// 	    
    /// </summary>
    /* *********************
        public void ahijaRama(int idMadre, int idHija)
        {
            int[] relacionArbol = new int[3];
            relacionArbol[0] = ultimoIdArbolIntf;   // identificador de la relacion
            relacionArbol[1] = idMadre;   // identificador de la rama madre
            relacionArbol[2] = idHija;   // Identificador de la rama hija
                                         // Añadimos el array a la lista que contiene el arbol
            arbolIntf.Add(relacionArbol);
            // Incrementamos el identificador de relacion
            ultimoIdArbolIntf++;   // identificador de la relacion

        }  // Fin de - public void ahijaRama(int idMadre, int idHija)
    ************************** */

} // Fin de - public class ScriptDatosInterfaz : MonoBehaviour {
