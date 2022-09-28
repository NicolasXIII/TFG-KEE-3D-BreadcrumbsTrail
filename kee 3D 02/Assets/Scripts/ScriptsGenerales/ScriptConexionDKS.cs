
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de CONEXION con los DKS
/// Se encarga de realizar las conexiones con los DKSs
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-02-04
/// Observaciones :
/// 		Contiene los datos, las constantes de string y las funciones de comunicacion con los DKSs
///
///		DATOS GENERALES :
///			- 
///		METODOS GENERALES :
///			-
/// </summary>

public class ScriptConexionDKS : MonoBehaviour {

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Declaramos los objetos globales para poder acceder a ellos
//    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject Solicitudes; // Se hace referencia a este objeto, para que todas las solicitudes queden como hijos de el. Y de esta forma no queden como
                                    // Gameobjects sueltos en la gerarquia

    // ///////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////
    //      CONSTANTES de cadena

    // ///////////////////////////////////////////////////////////////////////
    // Localizacion de DKSs y conceptos asociados

//    public static string dksGenerico = "http://www.ideando.net/klw/dks_Generic";
//    public static string dksKlw = "http://www.ideando.net/klw/dks_klw";
    public static string dksGenerico = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic";
    public static string dksKlw = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";

    // ///////////////////////////////////////////////////////////////////////
    // Cadenas de acceso a recursos de los DKSs

    public static string sufijoAccesoASwDks = "/kee/ClientesServWeb/gestorCliente.php";  // Es el directorio del DKS donde se atienden las solicitudes
                                                                                         // OJOOO lo atiende a todo el que llega. Hay que poner medidas de seguridad
                                                                                         // PENDIENTE MAFG (2021-02-04)
    public static string sufijoAccesoAFicherosDks = "/kee/ClientesServWeb/entradaFichero.php";  // Es el recurso del DKS donde se reciben los ficheros asociados a sin techo
//   public static string sufijoAccesoAImagenesDks = "/imagenes";  // Lo he modificado al programar el envio de ficheros al DKS (MAFG 2022-03-22)
//   public static string sufijoAccesoAIconosDks = "/imagenes";  //  Lo he modificado al programar el envio de ficheros al DKS (MAFG 2022-03-22)
//   public static string sufijoAccesoAAudiossDks = "/audios";     //  Lo he modificado al programar el envio de ficheros al DKS (MAFG 2022-03-22)
    public static string sufijoAccesoAImagenesDks = "/";  // Es el directorio del DKS donde se almacenan las imagenes e iconos de los conceptos
    public static string sufijoAccesoAIconosDks = "/";  // Es el directorio del DKS donde se almacenan las imagenes e iconos de los conceptos
    public static string sufijoAccesoAAudiossDks = "/";     // Es el directorio del DKS donde se almacenan los audios

    void Awake()
    {
        // Asignamos objetos
//        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // Asignamos objetos
        Solicitudes = GameObject.FindWithTag("Solicitudes");

    }  // Fin de - void Awake()

    // Use this for initialization
    void Start () {

    } // Fin de - void Start () {

    // Update is called once per frame
    void Update () {

    } // Fin de - void Update () {

    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    //       METODOS DE CONSULTAS AL DKS


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameDescendientePorNombre : Busca entre los descendientes de un gameObjetc el primero que tiene el nombre que se le pasa
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-15
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - string key
    ///     - string host
    ///     - GameObject objetoQueSolicita
    ///     - GameObject objetoQueGeneraEviResultado
    /// Variables de salida :
    /// 	- 
    /// Observaciones:
    ///      - Para realizar la solicitud de getdetails al DKS, 
    /// 
    /// </summary>
    public void solicitaGetDetails(string key, string host, GameObject objetoQueSolicita, GameObject objetoQueGeneraEviResultado)
    {
        // //////////////////////////////////////////////////////////
        // Solicitamos el concepto de consulta al DKS
        // Generamos el KDL de solicitud de getDetails que enviaremos como solicitud al DKS
        string kdlConsulta = GetComponent<ScriptLibConceptosXml>().dameConceptoBuskedaKeyHost(key, host);


        // ////////////////////////////////////////////////////
        // Generamos el elemento solicitud para gestionar la solicitud al DKS
        // INICIO SOLICITUD ///////////////////////////////////////////

        GameObject solicitud = Instantiate(GetComponent<ScriptDatosInterfaz>().Solicitud);
        solicitud.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_solicitud);
        solicitud.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks);
        // idSolicitud;  Se ha puesto mediante ponSubTipoElementIntf
        // hora_solicitud - Se pone en el awake de la solicitud
        // hora_respuesta - Todabia sin valor
        // hora_fin - Todabia sin valor
        // tiempoDeVida - lo incrementa "ScriptGestorSolicitudes"
        solicitud.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_consultaKdlADks_GetDetails; // Estamos en el boton de salir de edicion de concepto
        solicitud.GetComponent<ClassSolicitud>().subTipo_solicitud = solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();   // Usamos esta variable para hacer el tipo mas accesible
        solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso;   // La solicitud ya esta en curso. EN ESTE ESTADO ALGUN INTERVINIENTE PUEDE REQUERIR ATENCION
                                                                                                       // XmlDocument datos_Dom_solicitud - No procede en este caso

        // Esto que sigue es provisional. Debe ir todo el la consulta en DOM que es mas elegante PENDIENE MAFG 2021-08-11
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(key); // Es el key del concepto del que pedimos el getDetails. 
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(host); // Es el host del concepto del que pedimos el getDetails.  
        solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add(kdlConsulta); // Es el KDL de solicitud de GetDetails al DKS del concepto del que pedimos el getDetails.. 
                                                                                       // XmlDocument respuesta_Dom_solicitud - No procede en este caso
                                                                                       // List<string> respuesta_txt_solicitud - Se sabrá mas tarde. C0ntendra la respuesta del usuario que sera SI o NO
                                                                                       // Hasta aqui lo PENDIENTE

        //   public List<ClassInterviniente> listaIntervinientes;  // Es una lista que contiene sus hijos de primer nivel

        // //////////////////////////////////////
        // INTERVINIENTES EN LA SOLICITUD :
        // 0.) este "EviTipo_buscador_00"
        //        - Genera la solicitud
        //        - Envia la peticion al DKS. Y cuando esta reciba la respuesta hara que la solicitud requiera al siguiente 
        //          interviniente, que será este mismo "EviTipo_buscador_00"
        //    Siguiente interviniente : este mismo "EviTipo_buscador_00"

        ClassInterviniente interviniente_Solicitante = new ClassInterviniente();
        interviniente_Solicitante.Interviniente = objetoQueSolicita.gameObject; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_Solicitante.ordinal_Interviniente = 0; ; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_Solicitante.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // Es quien realiza la solicitud al DKS.
        interviniente_Solicitante.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;   // esta intervencion realiza la solicitud al DKS y eso ya esta hecho cuando termine este metodo
        interviniente_Solicitante.hora_ini_intervencion = DateTime.Now;
        solicitud.GetComponent<ClassSolicitud>().listaIntervinientes.Add(interviniente_Solicitante); // Es una lista que contiene sus hijos de primer nivel

        // 1.) este "EviTipo_buscador_00"
        //        - Atiende la solicitud. Sera requerido cuando se haya recibido la respuesta del DKS
        //        - Con la respuesta del DKS ...
        //        - Pone el estado de la intervencion en finalizado para que "ScriptGestorSolicitudes" finalice la solicitud

        ClassInterviniente interviniente_QueGeneraEviResultado = new ClassInterviniente();
        interviniente_QueGeneraEviResultado.Interviniente = objetoQueGeneraEviResultado.gameObject; // Es este mismo evi quien debe recibir la respuesta del DKS.
        interviniente_QueGeneraEviResultado.ordinal_Interviniente = 1; ; // Es el elemento de interfaz que realiza la interaccion.
        interviniente_Solicitante.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_receptor; // Es quien debe recibir la respuesta del DKS.
        interviniente_QueGeneraEviResultado.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_enEjecucion;   // en ejecucion a la espera de que llegue la respuesta del DKS
        solicitud.GetComponent<ClassSolicitud>().listaIntervinientes.Add(interviniente_QueGeneraEviResultado);  // Es una lista que contiene sus hijos de primer nivel

        GetComponent<ScriptConexionDKS>().solicitudConKdlAlDks(solicitud);

    }  // Fin de -  public GameObject dameDescendientePorNombre(GameObject objetoDondeBuscar, string nombreDelDescendiente)







    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : envia un KDL de solicitud al DKS. Debe obtener un KDL de respuesta
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-04
    /// Ultima modificacion :
    ///     - 2021-08-13. Adapto la fincion a la nueva programacion de las solicitudes
    /// Variables de entrada :
    ///         - GameObject solicitudAAtender : Es la solicitud asociada
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// Observaciones:
    ///         OOOJOOOOOOOO.
    ///         Esta funcion envia al DKS una solicitud KDL. Pero esta solicitud puede ser de muy diversos tipo. A saber
    ///             1) getDetails : Pide el KDL de un concepto concreto
    ///             
    /// </summary>

    public void  solicitudConKdlAlDks(GameObject solicitudAAtender)
    {

//        string urlDks = host + sufijoAccesoASwDks; // Para solicitud sin post, devuelve lo que le mandamos por el post

        // Realizamos el acceso al DKS mediante la corrutina correspondiente
        StartCoroutine(ejecutaSolicitud(solicitudAAtender));


    }  // Fin de -  public void solicitudConKdlAlDks(string host, string kdlConsulta, string tipoSolicitud, int idEviDestino, string tipoElementIntf)


    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    //         ITERADORES


    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : envia un KDL de solicitud al DKS y cuando llega la respuesta actua en consecuencia
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-04
    /// Ultima modificacion :
    ///     - 2021-08-13. Adapto la fincion a la nueva programacion de las solicitudes
    /// Variables de entrada :
    ///         - GameObject solicitudAAtender : Es la solicitud asociada
    /// Variables de salida :
    ///     No devuelve nada, lanza la solicitud y recibe la respuesta, genera y va actualizando el ganeObject de solicitud donde el gestor de solicitudes podra analizar
    ///     el estado y avisar al EVi que tenga que procesar la respuesta o el error, segun corresponda
    /// Observaciones:
    ///         - Esta corrutina genera una solicitud (gameObject que se incluira en la lista de solicitudes al generarse). Esta solicitud llevará como 
    ///         solicitante, el "idElementIntf" de quien la generó, de forma que el "gestorDeSolicitudes" pueda detectar el estado de la solicitud y avisar al elemento 
    ///         correspondiente cuando esta se haya  obtenido. Los distintos elemento deben contener un metodo "atiendeSolicitud" al que llamara el 
    ///         gestor de comunicaciones cuando detecte en la "" que una solicitud requiere su atencion para que actue en consecuencia.
    ///         - OOOJOOOOOOOO.
    ///         Esta funcion envia al DKS una solicitud KDL. Pero esta solicitud puede ser de muy diversos tipo. A saber
    ///             1) getDetails : Pide el KDL de un concepto concreto
    ///             
    IEnumerator ejecutaSolicitud(GameObject solicitud)
    {
        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        // PASAMOS A EJECUTAR LA SOLICITUD
        // dependiendo del tipo de solicitud tendremos ue hacer y esperar algo distinto

        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        //  solicitudKdlRespKDL : es una solicitud KDL a un DKS
        //  => destinoDeLaSolicitud : la url donde esta alojado el DKS (solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud[1] + sufijoAccesoASwDks;)
        //  => datos_txt_solicitud[0] : un KDL en XML con donde se formula la solicitud al DKS
        //  => debe recibir : un KDL en XML (podriamos hacer un DOM)
        if (solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)
            {
            // ///////////////////////////////
            // Esto es una consulta KDL a un DKS. Por lo que hay que enviar el KDL de solicitud, mediante el POST
            // Preparamos los datos que iran en le POST
            WWWForm formDatosPost = new WWWForm();
            formDatosPost.AddField("KDL", solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud[2]); // el KDL es el tercer elemento de la lista

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log(" Desde ejecutaSolicitud la solicitud realizada :" +
                " KDL de solicitud = " + solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud[2]);
            }

            // OJOOO Falta incluir el "ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().usrKey". PENDIENTE MAFG 2021-10-24

            // Preparamos la conexion por POST con el DKS
            string destinoDeLaSolicitud = solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud[1] + sufijoAccesoASwDks; // el host es el segundo elemento de la lista
            UnityWebRequest ObjWebReqPost = UnityWebRequest.Post(destinoDeLaSolicitud, formDatosPost);

            yield return ObjWebReqPost.SendWebRequest(); // Quedamos a la espera de la resuesta

            solicitud.GetComponent<ClassSolicitud>().hora_respuesta = DateTime.Now;     // Instante en el que se recibe la respuesta

            if (ObjWebReqPost.isNetworkError)
                {
                // Si se produce un error, debe enviarse un KDL de error. PENDIENTE (MAFG 2021-02-04) Por ahora solo anotamos en el log
                if (DatosGlobal.niveDebug > 90)
                { Debug.Log(" Desde ejecutaSolicitud en subTipoSolicitud_consultaKdlADks con error = " + ObjWebReqPost.error); }
                solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_error;  // El receptor encontrara que se ha producido un error
                string este_error_01 = " Error en ejecutaSolicitud en subTipoSolicitud_consultaKdlADks con error = " + ObjWebReqPost.error;
                solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add(este_error_01); // EL recptor podra consultar esto
            }
            else
                {
                // Tomamos la respuesta que sera un fichero KDL, osea un XML en texto
                string respuestaKDL = ObjWebReqPost.downloadHandler.text;
                // Construimos el DOM del XML recibido
                if (DatosGlobal.niveDebug > 50)
                {
                    Debug.Log(" Desde ejecutaSolicitud la respuesta obtenida es = " + respuestaKDL +
                    " - Host = " + destinoDeLaSolicitud +
                    " KDL de solicitud = " + solicitud.GetComponent<ClassSolicitud>().datos_txt_solicitud[2]);
                }
                XmlDocument domDeKdl = new XmlDocument();
                domDeKdl.PreserveWhitespace = true;
                try
                    {
                    domDeKdl.LoadXml(respuestaKDL);  //voy por aqui /// Hay que poner alguna cosa que permita gestionar cuando el DKS manda un error no controlado por el DKS
                    // Aqui habria que validarlo con el esquema de KDL. PENDIENTE (MAFG 2021-01-31)
                    solicitud.GetComponent<ClassSolicitud>().respuesta_Dom_solicitud = domDeKdl;
                    solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[0].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
                    solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[0].hora_fin_intervencion = DateTime.Now;
                    solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_requiereAtencion;
                    solicitud.GetComponent<ClassSolicitud>().listaIntervinientes[1].hora_ini_intervencion = DateTime.Now;

                    solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add("OK"); // EL recptor podra consultar esto
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Ok desde ejecutaSolicitud en subTipoSolicitud_consultaKdlADks. Con respuestaKDL = " + respuestaKDL); }
                    } // FIn de - try
                catch (System.IO.FileNotFoundException)
                    {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Error al generar el DOM desde - con respuestaConPost = " + respuestaKDL); }
                    solicitud.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_error;  // El receptor encontrara que se ha producido un error
                    string este_error_02 = " Error en ejecutaSolicitud en subTipoSolicitud_consultaKdlADks al intentar generar el DOM. Error = " + ObjWebReqPost.error;
                    solicitud.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add(este_error_02); // EL recptor podra consultar esto
                    }  // Fin de -  catch (System.IO.FileNotFoundException)
                }  // Fin de - else de -if (ObjWebReqPost.isNetworkError)
        }  // Fin de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_consultaKdlADks)
        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        //  solicitaTextura : solicita un fichero de imagen a una url  (PENDIENTE. Deberia hacerse mediante una llamada KDL (MAFG 2021-02-05)
        ///  => destinoDeLaSolicitud : la url donde esta alojado el fichero de imagen con el que se generara la textura
        ///  => datos_txt_solicitud[0] : el nombre del fichero de imagen solicitado. 
        ///  => debe recibir un fichero de imagen
        else if (solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoSolicitud_solicitaTextura)
            {
        }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_solicitaTextura)
        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        //  solicitaAudio  : solicita un fichero de audio a una url   (PENDIENTE. Deberia hacerse mediante una llamada KDL (MAFG 2021-02-05) 
        ///  => destinoDeLaSolicitud : la url donde esta alojado el fichero de audio con el que se generara el audioclip
        ///  => datos_txt_solicitud[0] : el nombre del fichero de audio solicitado. 
        ///  => debe recibir un fichero de audio
        else if (solicitud.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoSolicitud_solicitaAudio)
        {
        }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_solicitaAudio)
        // //////////////////////////////////////////////////
        // //////////////////////////////////////////////////
        // Error. El tipò de solicitud no es conocido
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Error desde  ejecutaSolicitud. El tipò de solicitud no es conocido"); }
        }  // Fin de - else

    } // Fin de -  IEnumerator dameUrlPost(string urlSolicitada, string datosPost)

}  // Fin de - public class ScriptConexionDKS : MonoBehaviour {
