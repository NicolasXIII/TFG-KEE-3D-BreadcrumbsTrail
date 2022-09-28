using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control GENERAL de las solicitudes realizadas
/// Construye y gestiona las solicitudes realizadas (solicitudes a DKSs, a objetos del muro de usuario para consulta al usuario "Btn_Consulta_Si_No" por ejemplo, a agentes, etc...)
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-02-05
/// Ultima modificacion :
///     - 2021-08-06 - He cambiado la filosofia de las solicitudes. He quitado los roles de Solicitante, ejecutor y receptor, que eran complejos y demasiado especificos
///                     y lo he sustituido por una cadena de intervinientes. Asi queda mas simple y mas versatil. A cambio, cada tipo de solicitud debe definir su protocolo
///                     y las funciones de cada uno de los intervinientes
/// Observaciones :
/// 		- TODOS las solicitudes (consultas a DKSs, obtencion de ficheros de imagen, de audio, consultas de agentes, etc..
/// 		tienen asociado un game object con este script. Este, al generarse se anota en la lista "ListaSolicitudes". 
/// 		- El "ScriptGestorSolicitudes" a cada cuadro, analiza esta lista y actua segun se indica en la documentacion de "ScriptGestorSolicitudes". Pero
/// 		basicamente mira si alguna solicitud requiere de la atencion de alguno de sus intervinientes y si es asi, la anota en la lista "misSolicitudesPendientes" de
/// 		dicho elemento de interfaz, la cual atendera desde su update. 
/// 		- El "ScriptGestorSolicitudes" tambien se ocupa de eliminar las solicitudes que han terminado de realizarse, asi como de la gestion de 
/// 		errores en las solicitudes
/// 		- Las solicitudes tienen informacion de para quien se solicitaron, asi como el momento en que se generaron, errores, momento de la respuesta, etc
/// 		- Cabe la posibilidad de hacer un agente que lleve un control de como se van atendiendo y gestione los problemas que pudieran aparecer (PENDIENTE MAFG 2021-02-05)
/// 		- Los tipos de solicitud se describen mas adelante
/// 		- En un futuro los distintos desarrolladores pueden definir otros tipos de solicitudes, utilizando "ScriptGestorSolicitudes" como base, pero definiendo
/// 		procesos diferentes
/// 		
///         - ESTADOS DE UNA SOLICITUD
///             Ver en la definicion de variables de esta clase debajo de "public string estado_solicitud;"
///         
///         - INTERVINIENTES
/// 		    - Las solicitudes se resuelven pues mediante las actuaciones consecutivas o paralelas de diversos intervinientes que aparecen en "listaIntervinientes"
/// 		    - El numero y orden de intervencion de los intervinientes se define para cada "subTipo_solicitud" y para cada  "codigoDeSolicitud". Ya que mediante estas 
/// 		    dos variablles se define la operacion que se realiza mediante la solicitud
/// 		        - ESTADOS DE INTERVINIENTE :OJO los estados del interviniente no son los de la solicitud. La solicitud tiene su estado y cada interviniente tiene el suyo
/// 		        
///         - ESTADOS DE INTERVINIENTE
///             - Hay una serie de estados que "ScriptGestorSolicitudes" conoce. Estos son los que aparecen en "ClassInterviniente => estado_de_la_itervencion"
///             - Los programadores podran definir otros estados en el interviniente, pero su gestion en este caso es responsabilidad de dicho programador y
///             ademas "ScriptGestorSolicitudes"  no reconocera dichos estados
/// 	
///         - LISTAS DE SOLICITUDES
///             - Existe una LISTA GENERAL DE SOLICITUDES en "ScriptDatosInterfaz => ListaSolicitudes" donde estan anotadas todas las solicitudes
///             - Cada elemento de interfaz tiene una lista de SUS solicitudes (solo las que requieren su atencion) en "ScriptDatosElemenItf => misSolicitudesPendientes"
///             . El proceso "ScriptGestorSolicitudes" en cada frame, repasa la "ScriptDatosInterfaz => ListaSolicitudes" y anota las solicitudes que requiren atencion
///                 ("estado_solicitud" = "enProceso") y como interviniente ("ClassInterviniente => estado_de_la_itervencion = estado_itervencion_requiereAtencion)
///                 de algun elemento de interfaz en su lista "ScriptDatosElemenItf => misSolicitudesPendientes". Cada elemento de interfaz en cada cuadro debe revisar su 
///                 lista "ScriptDatosElemenItf => misSolicitudesPendientes" y atender las solicitudes segun proceda. Al terminar debe borrar la solicitud atendida de SU 
///                 lista "ScriptDatosElemenItf => misSolicitudesPendientes"
///             - Cuando todas las intervenciones de una solicitud estan teminadas ("ClassInterviniente => estado_de_la_itervencion = estado_finalizada") el 
///                 "ScriptGestorSolicitudes" elimina la solicitud y la borra de la lista "ScriptDatosInterfaz => ListaSolicitudes"
///                 
/// 
///         - COMO INSERTAR SOLICITUDES EN EL SISTEMA
///             - Cuando un elemento de interfaz requiere un proceso en el que intervengan varios elementos de interfaz de forma asincrona, puede utilizar el
///                 recurso de solicitudes que aqui se define. Para ello debera operar como sigue:
///                 
///                 1.) El elemento que genra la solicitud debe hacer lo siguiente
///                     1.1.) Generamos el objeto solicitud                 
///                              GameObject solicitud = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Solicitud);  // Generamos el objeto
///                              solicitud.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_solicitud);  // Generamos su identificacion como elemento de interfaz
///                              solicitud.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.TIPO DE SOLICITUD); // (ver TIPOS DE SOLICITUD abajo) cada tipo de solicitud tiene su rpotocolo e intervinientes 
///                                     (NO HAY QUE PONER idSolicitud;  Se ha puesto mediante ponSubTipoElementIntf)
///                                     (NO HAY QUE PONER hora_solicitud - Se pone en el awake de la solicitud)
///                                     (NO HAY QUE PONER hora_respuesta - Todabia sin valor)
///                                     (NO HAY QUE PONER hora_fin - Todabia sin valor)
///                             solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CODIGO_DE_LA_SOLICITUD; // (ver TIPOS DE SOLICITUD abajo)
///                             solicitud.GetComponent<ClassSolicitud>().subTipo_solicitud = solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();   // Usamos esta variable para hacer el tipo mas accesible
///                             solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso;   // Ver ESTADOS DE UNA SOLICITUD mas arriba
///                             XmlDocument datos_Dom_solicitud = datosendom (DEBE SER UN  XmlDocument) - si la solicitud requiere datos en formato DOM, si no, no se define
///                             solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add("DATOS DE LA SOLCICTUD EN TEXTO");  - si la solicitud requiere datos en formato DOM, si no, no se define// Es lo que se envia al ejecutor de la solicitud. ; // Es lo que se envia al ejecutor de la solicitud. 
///                                     (NO HAY QUE PONER XmlDocument respuesta_Dom_solicitud - Todabia sin valor)
///                                     (NO HAY QUE PONER List<string> respuesta_txt_solicitud - Todabia sin valor)
///                 2.) Cumplimentamos la lista de intervinientes. (Esta lista es  "public List<ClassInterviniente> listaIntervinientes").
///                         - OJO en ocasiones los intervinientes ya existen y en otras ocasiones habra que generarlos previamente EJ.: ·obj_Boton_de_salir_edicion"
///                         - Cada interviniente debe ser un objeto de la clase "ClassInterviniente" que contiene un objeto elemento de interfaz y algo mas de informacion
///                         2.1.) Generamos el objeto interviniente
///                                 ClassInterviniente obj_Boton_de_salir_edicion = new ClassInterviniente();  // Generamos el objeto de clase "ClassInterviniente"
///                                 obj_Boton_de_salir_edicion.Interviniente = ElementoDeInterfazQueDebeAtenderLaIntervencion.gameObject; // Es el elemento de interfaz que realiza la interaccion.
///                                 obj_Boton_de_salir_edicion.ordinal_Interviniente = int; ; // le asignamos un entero consecutivamente Normalmente (0, 1, 2, 3,...)
///                                 obj_Boton_de_salir_edicion.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // Para identificar el tipo (ver ClassInterviniente)
///                                 obj_Boton_de_salir_edicion.estado_de_la_itervencion = ClassInterviniente.ESTADO;  // Para ESTADO ver ESTADOS DE INTERVINIENTE mas arriba
///                                         (NO HAY QUE PONER obj_Boton_de_salir_edicion.hora_ini_intervencion = DateTime.Now; - solo si el interviniente inicia su intervencion en este momento)
///                                 solicitud.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_Boton_de_salir_edicion); // Añadimos el interviniente a la lista de intervinientes de esta solicitud
///                         2.2.) PARA CADA INTERVINIENTE EN LA SOLICITUD VOLVEMOS A REALIZAR EL PASO 2.1.. Teniendo en cuenta lo siguiente :
///                                 - Atendiendo al tipo de solicitud "subTipo_solicitud" y  a su codigo "codigoDeSolicitud" (ver "DOCUMENTACION TIPOS DE SOLICITUD" mas abajo), cada solicitud
///                                 tiene sus objetivos, sus datos, sus intervinientes y su secuencia de proceso
///                                 - Cada interviniente debe manejar los datos de la solicitud para realizar el proceso que le corresponde.
///                                 - Cada interviniente, cuando interviene debe manejar los estados de las intervenciones que considere necesario, para indicar si la intervencion ha terminado
///                                 si debe esperar, si ha tenido errores o poner su estado para que sea la proxima en ejecutarse ("ClassInterviniente => estado_de_la_itervencion = estado_itervencion_requiereAtencion)
///                                 - Solo aquellas intervenciones cuyo estado es ("ClassInterviniente => estado_de_la_itervencion = estado_itervencion_requiereAtencion), seran gestionadas 
///                                 por "ScriptGestorSolicitudes" para que sean atendidas por el elemento de interfaz correspondiente, por lo que normalmente cada interviniente, cuando termina su
///                                 intervencion, la marca como ("ClassInterviniente => estado_de_la_itervencion = estado_itervencion_finalizada) y pone la intervencion que debe ser la 
///                                 siguiente en actuar como ("ClassInterviniente => estado_de_la_itervencion = estado_itervencion_requiereAtencion)
///                                 
///                                 - En general, la documentacion sobre como se ejecuta cada solicitud esta en  DOCUMENTACION TIPOS DE SOLICITUD mas abajo en este fichero
///                                 
/// </summary>


public class ClassSolicitud : MonoBehaviour {

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Declaramos los objetos globales para poder acceder a ellos
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Declaramos las propiedades de la solicitud

    // Datos generales
    public int idSolicitud;  // Identificador de la solicitud

    public DateTime hora_solicitud;  // Instante en el que se cursa la solicitud, puede no coincidir en el momento en el que se crea el objeto solicitud (que esta en "ScriptDatosElemenItf. horaDeGenesis")
    public DateTime hora_respuesta;     // Instante en el que se recibe la respuesta
    public DateTime hora_fin;     // Instante en el que el receptor recibe la respuesta de la solicitud y termina de procesarla
    public int tiempoDeVida;     // El gestor de solicitudes debe incrementarlo cada vez que la analiza. Tambien calcula si excede mas de lo permitdo, en cuyo caso la elimina o actua en consecuencia

    // TIPOS DE SOLICITUD. OJO Aqui declaramos la variable que indica el tipo de esta solicitud. Los tipos de solicitud estan en "ScriptDatosElemenItf"
    public string subTipo_solicitud;   // identifica el tipo de solicitud como subtipo de elemnto de interfaz
    public string codigoDeSolicitud; // Es un codigo que quien la genera incluye en la solicitud, para que el resto de intervinientes la identifique y sepa procesar el contenido de la respuesta (ver declaracion de la variable)
                                     // El codigoDeSolicitud, aunque va aqui en la solicitud, debe ser conocido por todos los intervinientes

    // LOS CODIGOS de solicitud segun el TIPO de solicitud son los siguientes
    // Para TIPO : subTipoSolicitud_consultaKdlADks = "consultaKdlADks"; (ver "ScriptDatosElemenItf")
        //  tipoSolicitud_consultaKdlADks => codigoDeSolicitud
        // Para geswtionar una peticion a un DKS mediante el KDL correspondiente
        public static string CodigoSol_consultaKdlADks_GetDetails = "CodigoSol_consultaKdlADks_GetDetails";
        public static string CodigoSol_consultaKdlADks_AltaConcepto = "CodigoSol_consultaKdlADks_AltaConcepto";
        public static string CodigoSol_consultaKdlADks_ModificaConcepto = "CodigoSol_consultaKdlADks_ModificaConcepto";
    // Para TIPO : subTipoSolicitud_solicitaTextura = "solicitaTextura"; (ver "ScriptDatosElemenItf")
    // Para TIPO : subTipoSolicitud_solicitaAudio = "solicitaAudio"; (ver "ScriptDatosElemenItf")
    // Para TIPO : subTipoSolicitud_RespBtn_Consulta_Si_No = (ver "ScriptDatosElemenItf")
        // subTipoSolicitud_RespBtn_Consulta_Si_No => codigoDeSolicitud
        // Para geswtionar la respuesta del usuario a la pregunta de si desea realmente salir de la edicion de un concepto
        public static string CodigoSol_resp_sino_btn_Editar_Salir = "CodigoSol_resp_sino_btn_Editar_Salir";
                // Para geswtionar la respuesta del usuario a la pregunta de si desea realmente salir de la edicion de un concepto    
        public static string CodigoSol_resp_sino_btn_Editar_Grabar = "CodigoSol_resp_sino_btn_Editar_Grabar";

    // //////////////////////////////////////////////////////////////////////
    // Intervenciones
    public List<ClassInterviniente> listaIntervinientes;  // Es una lista que contiene sus hijos de primer nivel

    // TIPOS DE INTERVINIENTES. Se definen aqui distintos tipos de intervinientes para usarlos libremente en distintos tipos de solicitudes
    public static string tipo_Interviniente_solicitante = "tipo_Interviniente_solicitante";
        public static string tipo_Interviniente_receptor = "tipo_Interviniente_receptor";

    // Estados de la solicitud
        public string estado_solicitud;   // identifica el estado en el que se encuentra la solicitud
        public static string estado_NoIniciada = "NoIniciada"; //  El solicitante ha iniciado la solicitud (NO REQUIERE ATENCION)
        public static string estado_enEspera = "enEspera"; //  El solicitante ha iniciado la solicitud (NO REQUIERE ATENCION)
        public static string estado_enProceso = "enProceso"; //  La solicitud ha iniciado enviada al Receptor (SI PUEDE REQUERIR ATENCION)
        public static string estado_finalizada = "finalizada"; //  Todo el proceso asociado a la solicitud ha sido finalizado. Puede eliminarse o archivarse (NO REQUIERE ATENCION)
        public static string estado_error = "error"; //  Todo el proceso asociado a la solicitud ha sido finalizado. Puede eliminarse o archivarse (NO REQUIERE ATENCION)
                                                     // OJOOO los estados especificos del  los procesos para los que se utilizan las solicitudes, deben gestionarse en los datos de la solicitud
                                                     // aqui solo se gestionan los estados de la solicitud en si

    // /////////////////////////////////////////////////////////////////// 
    //     Otros datos de la solicitud

    // Datos de la solicitud. Puede ir en txt o en DOM
    public XmlDocument datos_Dom_solicitud; // este debe ser un DOM, pero para ir desarrollando pongo el string xml PENDIENTE (2021-01-30 Miguel AFG)
    public List<string> datos_txt_solicitud; // Es lo que se envia al ejecutor de la solicitud. 
                                     //      - Si es que se solicita pregunta de Si o No; el texto de la pregunta
                                     //      - Si es a un DKS, el KDL de solicitud. 
                                     //      - Si se solicita un fichero, el nombre del fichero

    // Datos del resultado de la ejecucion de la solicitud
    public XmlDocument respuesta_Dom_solicitud; // Es el dom de un XML concreto. Normalmente un KDL para cuando es un XML lo que se espera como respuesta
    public List<string> respuesta_txt_solicitud; // Para cuando se espera recibir texto. SSI  SE PRODUCE uUN ERROR, el mensaje de error ira en esta variable
                                                 //      - Si es se solicita respuesta Si o No; true=Si, false=No
                                                 //      - Si es a un concepto al DKS, el KDL del concepto. 
                                                 //      - Si se solicita un fichero, el fichero de imagen, o un enlace a este segun corresponda  

    void Awake()
    {
        // OJOOO Hay que tener en cuenta que las solicitudes son elementos de interfaz, por lo que parte de su informacion basica
        // estara en su componente "ScriptDatosElemenItf" correspondiente

        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // //////////////////////////////////
        // Asignamos los valores PROPIOS DE LA SOLICITUD
        hora_solicitud = DateTime.Now; // En principio lo ponemos en la hora que se ejecuta el awake. Quien procesa la informacion puede indicar otro valor si lo cree conceniente
        tiempoDeVida = 0;  // Se ira incrementando por los distintos agentes que operan con la solicitud, para cuantificar cuantas vueltas ha dado
        estado_solicitud = estado_NoIniciada;

        idSolicitud = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generaIdElementEnTipo("solicitud");

        listaIntervinientes = new List<ClassInterviniente>();  // Es una lista que contiene sus hijos de primer nivel


    // Apuntamos la solicitud en la lista dorrespondiente
    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaSolicitudes.Add(gameObject); //Ya se genera al generar el elemento de interfaz en ScriptDatosElemenItf=>Start

    } // Fin de - void Awake()

    // Use this for initialization
    void Start () {

        // Ponemos un nombre para identificarlo en el entorno de trabajo
        gameObject.name = "Sol_" + GetComponent<ScriptDatosElemenItf>().dameIdElementIntf(); 

    }

    /* ************  Para ponerlo en una libreria DLL y acceder desde todos sitios, pero tengo que ver como PENDIENTE MAFG 2021-02-14
    public class codigosDeEmisor
    {
        public string Dato;
        public string valorString;
        public int valorInt;
    }
    ******************* */



}  // FIn de - public class ScriptSolicitud : MonoBehaviour {


/// <summary>
///   DOCUMENTACION TIPOS DE SOLICITUD  //////////////////////////////////////////////
///   
///  //////////////////////////////////////////////////////////////////////////////
///  //////////////////////////////////////////////////////////////////////////////
/// - SOLICITUD PARA PEDIR UNA TEXTURA VIA HTTP
/// "subTipo_solicitud = "subTipoSolicitud_solicitaTextura" - se declara en "ScriptDatosElemenItf"
///          (POR AHORA NO SE UTILIZA MAFG 2021-08-11, SE HACE CON CORRUTINAS)
/// 
///  //////////////////////////////////////////////////////////////////////////////
///  //////////////////////////////////////////////////////////////////////////////
/// - SOLICITUD PARA PEDIR UN FICHERO DE AUDIO VIA HTTP
/// "subTipo_solicitud = "subTipoSolicitud_solicitaAudio" - se declara en "ScriptDatosElemenItf"
///          (POR AHORA NO SE UTILIZA MAFG 2021-08-11, SE HACE CON CORRUTINAS)
/// 
///  //////////////////////////////////////////////////////////////////////////////
///  //////////////////////////////////////////////////////////////////////////////
/// - SOLICITUD PARA HACER UNA CONSULTA AL USUARIO (SI O NO)
/// "subTipo_solicitud = "subTipoSolicitud_RespBtn_Consulta_Si_No" - se declara en "ScriptDatosElemenItf"
///         - Observaciones :
///             - Sirve para hacer de forma asincrona una pregunta al usuario que este contestara 
/// 
///         - TIPOS DE CODIGO DE SOLICITUD
///             - Pregunta salir de edicion (SI o NO) => "codigoDeSolicitud = CodigoSol_resp_sino_btn_Editar_Salir" (se declara en esta misma clase)
///             - Pregunta grabar edicion en el DKS (SI o NO) => "codigoDeSolicitud = CodigoSol_resp_sino_btn_Editar_Grabar" (se declara en esta misma clase)
///             
///                 - OJOOO la unica diferencia de "CodigoSol_resp_sino_btn_Editar_Salir" con "CodigoSol_resp_sino_btn_Editar_Grabar" en que 
///                   en el - Paso  1.1. de  COMO INSERTAR SOLICITUDES EN EL SISTEMA : se asigna el codigo de solicitud correspondiente a cada una
///                             solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir;
///                             solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Grabar;
///                   de esta forma el interviniente 2 (es el evi "BaseDeEvi_01" que se esta editando) sabe como debe de actuar en cada caso
///             
///     - PASOS A SEGUIR :
///         1.) GENERAMOS EL OBJETO BOTON para la consulta de Si o No (General para todos los codigos de solicitud)
///                   GameObject Btn_Consulta_Si_No = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Consulta_Si_No);
///                   Btn_Consulta_Si_No.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
///                   Btn_Consulta_Si_No.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_Btn_Consulta_Si_No);
///                   // Leg pasqamos el texto de la consulta
///                   string textoConsulta = "Realmente quieres abandonar la edicion"; // O LO QUE PORCEDA. Es la pregunta que aparece en pantalla para que el usuario conteste
///                   Btn_Consulta_Si_No.GetComponent<ScriptBtn_Consulta_Si_No>().preguntaAUsuario = textoConsulta;
///                   
///         2.) GENERAMOS LA SOLICITUD. Para hacerlo, tenemos que seguir las indicaciones que aparecen en este fichero, en el apartado COMO INSERTAR SOLICITUDES EN EL SISTEMA
///             definiendo especificamente los valores siguientes
///             
///                    Paso  1.1. de  COMO INSERTAR SOLICITUDES EN EL SISTEMA :
///                             solicitud.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No); 
///                             solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir;
///                             solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso; 
///                             solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add("¿desea realmente abandonar la edicion?");
///                             
///                    Paso  2. de  COMO INSERTAR SOLICITUDES EN EL SISTEMA :
///                    
///                             - DESCRIPCION GENERAL DE LA SOLCITUD (3 intervinientes):
///                             
///                                 - Interviniente 0 : es el boton "Btn_BaseDeEvi_N3_1_op_Editar_Salir"
///                                     - genera el objeto boton "Btn_Consulta_Si_No"
///                                     - Genera la solicitud y la envia al interviniente 1 (Btn_Consulta_Si_No)
///                 
///                                 - Interviniente 1 : es el boton "Btn_Consulta_Si_No"
///                                     - Basicamente espera a que el usuario conteste
///                                     - Cuando el usuario contesta, cumplimenta el parametro "respuesta_txt_solicitud" con la respuesta (SI o NO) del usuario
///                                     - Envia ña solicitud al interviniente 2 ()
///                 
///                                 - Interviniente 2 : es el evi "BaseDeEvi_01" que se esta editando. Atiende desde "ScriptCtrlBaseDeEvi" de sicho evi base
///                                     - Atendiendo a la respuesta del usuario actua en consecuencia
///                                         - Respuesta NO : No hace nada el evi continua en edicion como estaba
///                                         - Respuesta SI : 
///                                            - Segun el "codigoDeSolicitud" :
///                                                 - Para "codigoDeSolicitud = CodigoSol_resp_sino_btn_Editar_Salir"
///                                                     - Hace que el evi salga de edicion
///                                                     - Marca la intervencion como terminada. Las intervenciones 0 y 1 deben estar terminadas, por lo que la solicitud debe estarlo
///                                                 - Para "codigoDeSolicitud = CodigoSol_resp_sino_btn_Editar_Grabar"
///                                                     - Graba el KDL del concepto atendiendo a la configuracion de la edicion en el DKS pertinente
///                                                     - Marca la intervencion como terminada. Las intervenciones 0 y 1 deben estar terminadas, por lo que la solicitud debe estarlo
///                                     
///                             - Despues de generar la solicitud, SE GENERAN LOS INTERVINIENTES
///                                     - Puede verse un ejemplo en "Script_BaseDeEvi_N2_1" en el comentario "INTERVINIENTES EN LA SOLICITUD"
/// 
///  //////////////////////////////////////////////////////////////////////////////
///  //////////////////////////////////////////////////////////////////////////////
/// - SOLICITUD PARA CONSULTA KDL A DKS
/// "subTipo_solicitud = "subTipoSolicitud_consultaKdlADks" - se declara en "ScriptDatosElemenItf"
/// 
///         - Observaciones :
///             - Sirve para hacer de forma asincrona una consulta al DKS 
/// 
///         - TIPOS DE CODIGO DE SOLICITUD
///             - "codigoDeSolicitud = CodigoSol_consultaKdlADks_GetDetails" 
///             - "codigoDeSolicitud = CodigoSol_consultaKdlADks_AltaConcepto" 
///             - "codigoDeSolicitud = CodigoSol_consultaKdlADks_ModificaConcepto" 
///             - Por ahora no hay otros codigos de solicitud para este tipo de solicitud (2021-10-24)
///             
///         - DESCRIPCION GENERAL DE LA SOLCITUD (3 intervinientes):
///                             
///             - Interviniente 0 SOLICITANTE : puede ser cualquier elemento de interfaz
///                  - Genera la solicitud y la deja en estado "estado_enProceso"
///                  - Llama a la funcion "ctrlInterfaz.GetComponent<ScriptConexionDKS>().solicitudConKdlAlDks(solicitud);" enviandole la solicitud
///                     será este quien realice la solicitud al DKS mediante una corrutina "IEnumerator ejecutaSolicitud (GameObject solicitud)"
///                         - Cuando la corrutina haya terminado y tenga la respuesta del DKS, pone la respuesta en la solicitud y 
///                             la envia al interviniente 1 (RECEPTOR)
///                
///             - Interviniente 1 RECEPTOR : Recoge el KDL de respuesta en formato DOM, que se encuentra en "respuesta_Dom_solicitud" de la solicitud
///                 y actua en consecuencia:
///                     - Para "CodigoSol_consultaKdlADks_GetDetails" : normalmente genera un evi fractal con este KDL (que es el del getDetails), y lo pone en el muro que corresponda
///                     - Para "CodigoSol_consultaKdlADks_AltaConcepto" : normalmente genera un evi fractal con este KDL (que es el del concepto recien generado), y lo pone en el muro que corresponda
///                     - Para "CodigoSol_consultaKdlADks_ModificaConcepto" : normalmente genera un evi fractal con este KDL (que es el del concepto recien modificado), y lo pone en el muro que corresponda
///                 
///     - PASOS A SEGUIR :
///     
///         0.) Intreviniente 0. EL SOLICITANTE : GENERA LA SOLICITUD. Para hacerlo, tenemos que seguir las indicaciones que aparecen en este fichero, en el apartado COMO INSERTAR SOLICITUDES EN EL SISTEMA
///             definiendo especificamente los valores siguientes
///             
///                   0.1.) Paso  1.1. de  COMO INSERTAR SOLICITUDES EN EL SISTEMA :
///                             solicitud.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks); 
///                                     - Segun proceda 
///                                         solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_consultaKdlADks_GetDetails;
///                                         solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_consultaKdlADks_AltaConcepto;
///                                         solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_consultaKdlADks_ModificaConcepto;
///                             solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso; 
///                             solicitud.GetComponent<ClassSolicitud>().datos_Dom_solicitud.Add(kdlConsulta); // Es el KDL de solicitud  al DKS
///                                                                                                            // de GetDetails del concepto del que pedimos el getDetails, de alta, de modificacion, consulta, etc..
///                                     - No necesitamos datos en txt al generarla
///                                         solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(""); (NO SE PONE)
/// 
///                                     - OBSOLETO_01 : Esto que sigue esta  desde aqui
///                                         - OJO Los datos que siguen son provisionales. Lo suyo seria enviar el KDL en DOM, en vez de en texto
///                                         solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(key); // Es el key del concepto del que pedimos el getDetails. 
///                                         solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(host); // Es el host del concepto del que pedimos el getDetails. 
///                                         - Hasta aqui OBSOLETO_01
///                             
///                             solicitud.GetComponent<ClassSolicitud>().respuesta_Dom_solicitud.Add(kdlRespuesta); // Es el KDL de respuesta del DKS. NO SE PONE en principio, lo rellenara el interviniente que lo reciba
///                                     - No necesitamos datos en txt al procesarla
///                                         solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add(""); (inicialmente NO SE PONE, lo rellenara el interviniente que corresponda)
/// 
///                                                                                                             
///                 - 0.2.) Cuando ya se ha generado la solicitud, el interviniente 0 (SOLICITANTE) realiza tambien las siguientes operaciones
///                 
///                          - Llama a la funcion "ctrlInterfaz.GetComponent<ScriptConexionDKS>().solicitudConKdlAlDks(solicitud);" enviandole la solicitud
///                             será este quien realice la solicitud al DKS mediante una corrutina "IEnumerator ejecutaSolicitud(GameObject solicitud)"
///                                 - Cuando la corrutina haya terminado y tenga la respuesta del DKS
///                                     - Guarda la respuesta en "respuesta_Dom_solicitud" en formato DOM
///                                             solicitud.GetComponent<ClassSolicitud>().respuesta_Dom_solicitud = domDeKdl;
///                                     - Pone al interviniente 0 (SOLICITANTE) en estado "estado_itervencion_finalizada" para que cuando terminen las demas intervenciones "ScriptGestorSolicitudes" 
///                                       sepa que debe cerrar la solicitud
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[0].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
///                                     - Anota la hora en la que finaliza el interviniente 0 (SOLICITANTE)
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[0].hora_fin_intervencion = DateTime.Now;
///                                     - Pone al interviniente 1 (RECEPTOR) en estado "estado_itervencion_requiereAtencion" para que realice su intervencion sobre la solicitud
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_requiereAtencion;
///                                     - Anota la hora en la que comienza el interviniente 1 (RECEPTOR)
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].hora_ini_intervencion = DateTime.Now;
///                                    - Indica que la respuesta se ha recibido correctamete mediante la lista de string de respuesta
///                                             solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add("OK");
///                                    - Marca esta intervencin como terminada, para que cuando terminen las demas intervenciones "ScriptGestorSolicitudes" sepa que debe cerrar la solicitud
///                                                                                                             
///         1.) Intreviniente 1. EL RECEPTOR : Recoge el KDL de respuesta en formato DOM, que se encuentra en "respuesta_Dom_solicitud" de la solicitud
///                 y actua en consecuencia (normalmente genera un evi fractal con este KDL, y lo pone en el muro que corresponda, o lo que corresponda segun sea getdetails, alta de concepto, modificacion, busqueda, etc...)
///                    
///                                     - Pone al interviniente 1 (RECEPTOR) en estado "estado_itervencion_finalizada" para que "ScriptGestorSolicitudes" sepa que debe cerrar la solicitud
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
///                                     - Anota la hora en la que comienza el interviniente 1 (RECEPTOR)
///                                             solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].hora_fin_intervencion = DateTime.Now;
///                                             
///         - OJOOO Podemos encontrar UN EJEMPLO de uso en "ScreiptCtrlEviTipo_buscador_00 => Update() => if (enTriger)"
/// 
/// 
///                     
/// </summary>


