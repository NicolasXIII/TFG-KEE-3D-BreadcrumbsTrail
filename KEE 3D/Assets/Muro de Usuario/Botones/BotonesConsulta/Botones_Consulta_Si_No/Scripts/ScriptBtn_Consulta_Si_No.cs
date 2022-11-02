using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script para manejar el Objeto de consulta "SI o NO"
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-06-26
/// Observaciones :
///     - 
///
///		DATOS GENERALES :
///		
///		METODOS GENERALES :
///			-
/// </summary>
public class ScriptBtn_Consulta_Si_No : MonoBehaviour
{
    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public GameObject MuroUsuario;
    public Canvas CanvasGeneral;
    public TextMeshProUGUI textConsulta;

    public GameObject Btn_Resp_No;
    public GameObject Btn_Resp_Si;
    public GameObject este_Btn_Resp_No;
    public GameObject este_Btn_Resp_Si;

    public string respuestaUsuario;
    public static string respUsr_SinIniciar = "respUsr_SinIniciar";
    public static string respUsr_EnEspera = "respUsr_EnEspera";
    public static string respUsr_No = "respUsr_No";
    public static string respUsr_Si = "respUsr_Si";

    public string preguntaAUsuario;

    public float escala_x_y_BotonesInteriores = 1f / 4f;
    public float incrementoEscala_x_y_Activacion = 3f/2f;
    public float escala_z_BotonesInteriores = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        MuroUsuario = GameObject.FindWithTag("MuroUsuario");

        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;
        textConsulta = CanvasGeneral.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); // "Text_ConsultaUsr" es el primer hjo de "CanvasGeneral"
        textConsulta.text = preguntaAUsuario;

        // Asignamos los valores por defecto
        respuestaUsuario = respUsr_SinIniciar;

        // Generamos el boton objeto de Btn_Consulta_Si_No, para cuando haga falta

        // Ahijamos el boton al muro de usuario, que es donde debe estar
        this.gameObject.transform.SetParent(MuroUsuario.transform);


        // ajustamos tamaño y posicion del objeto "Btn_Consulta_Si_No"
        this.gameObject.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_ConsultaUsr_tipo01;
        this.gameObject.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_ConsultaUsr_tipo01;

        // Generamos los botones que contiene
        este_Btn_Resp_No = Instantiate(Btn_Resp_No);
        este_Btn_Resp_Si = Instantiate(Btn_Resp_Si);

        // Ahijamos y ajustamos los botones de opciones
        este_Btn_Resp_No.gameObject.transform.SetParent(this.transform);
        este_Btn_Resp_Si.gameObject.transform.SetParent(this.transform);

        // Asignamos escalas y posiciones de los botones de consulta (dentro del espacio del boton "Btn_Consulta_Si_No")

        Vector3 escala_este_Btn_Resp_xx = new Vector3(escala_x_y_BotonesInteriores, escala_x_y_BotonesInteriores, escala_z_BotonesInteriores);
        este_Btn_Resp_No.transform.localScale = escala_este_Btn_Resp_xx;
        este_Btn_Resp_Si.transform.localScale = escala_este_Btn_Resp_xx;

        Vector3 posicion_este_Btn_Resp_No = new Vector3((1f/4f), 0f, 0f);
        este_Btn_Resp_No.transform.localPosition = posicion_este_Btn_Resp_No;
        Vector3 posicion_este_Btn_Resp_Si = new Vector3((-1f / 4f), 0f, 0f);
        este_Btn_Resp_Si.transform.localPosition = posicion_este_Btn_Resp_Si;

        // ajustamos el estado de los punteros, para que en los evis del muro no se puedan manejar hasta que termine la consulta
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_NoActivaEvis;

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<SphereCollider>().isTrigger = false;

        // Colocamos el texto donde esta el raton, que sera mas o menos donde esta el evi base
        // Convendria colocarlo un poco mas abajo ajustando esta distancia a la escala correspondiente PENDIENTE MAFG 2021-04-22
        Vector3 pos_raton = Input.mousePosition;
        textConsulta.transform.position = pos_raton;


    }  // FIn de -  void Start()

    // Update is called once per frame
    void Update()
    {
        // Miramos si tenemos soicitudes pendientes, y si es asi, se resuelven
        if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)
        {
            // Las solicitudes que pueden llegar son :
            //  1.) Respuesta a la pregunta que se hizo al usuario sobre si quiere abandonar la edicion
            //  2.) Respuesta a la pregunta que se hizo al usuario sobre si quiere grabar en el DKS los cambios realizados durante la edicion
            foreach (GameObject solicitud in GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScriptBtn_Consulta_Si_No => update antes de atiende mari llanos. Con tamaño de lista de solicitudes 107 : " + GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count); }
                atiendeMariLlanos(solicitud);  // Llamamos a quien debe gestionar la solicitud
            }
            GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Clear(); // Debemos habre procesado todos los elementos de la lista y esta estará vacia
                                                                                   // No se pueden borrar en la funciones a las que se llama desde aqui porque entonces casca el foreach
                                                                                   // por modificar la lista mientras se ejecuta
                                                                                   // Si hubiera que eliminar solo algunos habria que generar una lista a parte con los que hubiera que 
                                                                                   // borrar y luego borrarlos uno a uno fuera de este foreach (creo) MAFG 2021-02-14
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Count != 0)


    }  // Fin de -  void Update()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void exponConsulta()
    ///     Expone al susario la consulta a la que debe responder
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-06-23
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo expone la consulta
    /// Observaciones :
    ///     - Por ahora solo expone la consulta en texto. La funcion luego podria hacerse para que la consulta no fuera un texto (PENDIENTE MAFG 2021-06-23)
    /// </summary>
    public void exponConsulta(string textoConsulta)
    {

        // Cargamos el texto en el TextMeshProUGUI correspondiente
        textConsulta.text = textoConsulta;
        // Colocamos el texto donde esta el raton, que sera mas o menos donde esta el evi base
        // Convendria colocarlo un poco mas abajo ajustando esta distancia a la escala correspondiente PENDIENTE MAFG 2021-04-22
        Vector3 pos_raton_salir = Input.mousePosition;
        textConsulta.transform.position = pos_raton_salir;

    }  // Fin de - public void exponConsulta(string textoConsulta)



    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void atiendeMariLlanos(GameObject solicitudAAtender)
    ///     Este metodo atiende las solicictudes asociadas a este objeto.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-06-24
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
    ///         "ListaSolicitudes" y segun la necesidad de atencion de la solicitud "ClassSolicitud => estado_solicitud = "estado_enProceso" llamara al 
    ///         elemento de interfaz interviniente (de la lista "listaIntervinientes" de la solicitud) que requiera su atencion segun "idElementIntf"
    ///         Este metodo atiende las solicictudes en las que se le requiere.
    ///         - Las solicitudes que aqui se esperan son :
    ///             - "subTipoSolicitud_RespBtn_Consulta_Si_No" Solicitudes para botones de consulta Si-No al usuario
    ///             - No conocemos otros subtipos
    ///         
    /// </summary>
    public void atiendeMariLlanos(GameObject solicitudAAtender)
    {
        // ////////////////////////////////////////////////////

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Entro en ScriptBtn_Consulta_Si_No => atiendeMariLlanos 181 "); }

        // el "tipoElementIntf" debe ser "tipoElemItf_solicitud"
        // Segun el subtipo (tipo de solicitud) habra que atenderla como proceda
        string subTipoElementIntf = solicitudAAtender.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
        string codigoDeSolicitud = solicitudAAtender.GetComponent<ClassSolicitud>().codigoDeSolicitud;

        // Si el "subTipoElementIntf" es "subTipoSolicitud_RespBtn_Consulta_Si_No" es una pregunta de si o no y se atiende como sigue
        if (subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No)  // Si se ha recibido bien
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Estoy en ScriptBtn_Consulta_Si_No => atiendeMariLlanos en 191. atendiendo la solicitud : " + solicitudAAtender.GetComponent<ClassSolicitud>().idSolicitud); }

            // Indicamos que estamos en espera, por lo que los triger de los objetos de si o no comienzan a funcionar
            respuestaUsuario = respUsr_EnEspera;

            // Tenemos varios codigos de solicitud para el tipo "subTipoSolicitud_RespBtn_Consulta_Si_No". Estos son :
            //      - codigoDeSolicitud => "CodigoSol_resp_sino_btn_Editar_Salir"
            //      - codigoDeSolicitud => "CodigoSol_resp_sino_btn_Editar_Grabar"
            // Para todos ellos la intervencion de este objeto en la solicitud es la misma

            // Gestionamos los distintos intervinientes
            // Indicamos que este interviniente ya ha terminado (somos el interviniente de 1 de la lista de intervinientes)
            // La solicitud ya no requiere a este interviniente. Cuando el usuario conteste (mediante "StartCoroutine(esperaAQueUsrConteste(solicitudAAtender));"
            // indicaremos que el evi que pregunta (interviniente 2) debe atender la solicitud
            solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[1].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;
            solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[1].hora_fin_intervencion = DateTime.Now;


            // Llamamos a la corrutina que espera la respuesta
            StartCoroutine(esperaAQueUsrConteste(solicitudAAtender));
        }
        // else if subTipoElementIntf == ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No) { }  POR AHORA NO CONOCEMOS ORTOS SUBTIPOS DE SOLICITUD PARA ESTE INTERVINIENTE
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("Entro en ScriptBtn_Consulta_Si_No => atiendeMariLlanos y SIN SUBTIPO CONOCIDO. Con subTipoElementIntf  :  " + subTipoElementIntf); }
        }

    }  // FIn de - public void atiendeMariLlanos(GameObject solicitudAAtender)


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

    IEnumerator esperaAQueUsrConteste(GameObject solicitudAAtender)
    {
        while ( !((respuestaUsuario == respUsr_No) | (respuestaUsuario == respUsr_Si)))
        {
            yield return 0; // Esperamos a que el usuario conteste
        }
        // Ponemos la respuesta del usuario como "respuesta_txt_solicitud" de la solicitud para que esta informacion sea accesible a los interinientes (el 2 en este caso)
        solicitudAAtender.GetComponent<ClassSolicitud>().respuesta_txt_solicitud.Add(respuestaUsuario);

        // Gestionamos los distintos intervinientes
        // Indicamos que el interviniente 2 "obj_objeto_Evi_Raiz (desde "ScriptCtrlBaseDeEvi") del evi que pregunta" es requerido para atender la solicitud
        solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[2].estado_de_la_itervencion = ClassInterviniente.estado_itervencion_requiereAtencion;
        solicitudAAtender.GetComponent<ClassSolicitud>().listaIntervinientes[2].hora_ini_intervencion = DateTime.Now;

        // Reactivamos el puntero de usuario
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario.GetComponent<SphereCollider>().isTrigger = true;


        // Borramos el texto de la pregunta que pusimos en el Canvas general
        exponConsulta("");

        // Cerramos este objeto que ya ha cumplido su mision
        this.gameObject.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();
        // Destroy(gameObject);

    }  // Fin de - IEnumerator generaEviCompleto()

}  // FIn de - public class ScriptBtn_Consulta_Si_No : MonoBehaviour
