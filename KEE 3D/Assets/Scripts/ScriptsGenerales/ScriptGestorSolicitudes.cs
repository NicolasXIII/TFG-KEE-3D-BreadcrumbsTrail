using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script para la gestion de solicitudes
/// Se encarga gestionar las solicitudes y llamar a los elementos de interfaz que tienen que atenderlas
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-02-06
/// Ultima modificacion :
///     - 2021-08-10. Adapto la funcion a la nueva programacion de las solicitudes
/// Observaciones :
/// 		- Se encarga gestionar las solicitudes y llamar a los elementos de interfaz que tienen que atenderlas
/// 		- Esta interfaz requiere multiple llamadas servidores externos, DKSs, y otros, asi como la comunicación entre 
/// 		agentes del sistema de interfaz y otras muchas. TOdas estas comunicaciones son asincronas y el sistema debe seguir 
/// 		funcionando aunque estas tarden es atenderse (por retraso en servidores o procesos). Es por esto que se ha optado por 
/// 		un sistema de solicitudes que funciona de la manera siguiente:
/// 		    - Cada elemento que necesita un recurso web, el resultado de un proceso, o lo que sea, puede realizar una solicitud. Al generar
/// 		    dicha solicitud, debe crearse un gameObject tipo "Solicitud" que contendra en "ClassSolicitud" toda la informacion de la solicitud.
/// 		    
///         - Toda funcion tiene una lista de INTERVINIENTES. Los intervinientes son los distintos elementos que participan para llevar a cabo el 
///             proceso asociado a la solicitud.
///                 ej.: para hacer una llamada a un DKS (Interviniente 0 (SOLICITANTE) - Interviniente 1 (RECEPTOR))
///             Los metodos de essta clase se encargan de ir dando turno a cada uno de los intervinientes segun proceda, como una patata caliente,
///             para que estos vayan participando en el proceso global. La informacion asociada a la solicitud va integrada en el objeto asociado a esta
/// 
// 		    - Conforme se crea el gameObject de solicitud, este se almacena en la lista "ListaSolicitudes" con el estado que corresponda. Si la 
/// 		    solicitud esta "estado_solicitud = estado_enProceso" esta clase mediante el Update, se encarga de ver si la solicitud requiere atencion 
/// 		    de alguno de sus intervinientes. Y si es asi, la envia a su lista de solicitudes pendientes (todos lo eleementos de interfaz tienen 
/// 		    en "ScriptDatosElemenItf" una lista de solicitudes pendientes "misSolicitudesPendientes"
/// 		    
///         - En ejecucion "ScriptGestorSolicitudes" periodicamente, analiza la lista "ScriptDatosInterfaz =>ListaSolicitudes" y mira si alguna requiere
/// 		    atencion y de quien la requiere. En caso afirmativo. "ScriptGestorSolicitudes" anota la solicitud en su lista "misSolicitudesPendientes"
/// 		    para que este llame a su metodo "atiendeMariLlanos" del elemento de interfaz. El elemento al que le corresponde, procesara el resultado de la
/// 		    solicitud, segun su naturaleza
/// 		    
///    - ELEIMINAR UNA SOICITUD : Cuando el objetivo de la solicitud ha sido satisfecho, la solicitud debe eliminarse. Las solicitudes se eliminan como cualquier 
///             otro elemento de interfaz con "Ej. : objetoSolicitudConcreto.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();".
///             - Para que esto se lleve a cabo, cada interviniente, cuando termina su intervencion, la marca como finalizada "estado_de_la_itervencion = estado_itervencion_finalizada"
///             cuando el gestor de solicitudes detecta que todas las intervenciones de una solicitud estan terminadas, da la solicitud por terminada y
///             elimina el elemento de interfaz (solicitud) asociado a esta
///             
///    - CUANDO UNA SOLICITUD NO ENCUENTRA QUIEN LA ATIENDA : Si en la cadena de gestion de una solicitud en cualquiera de sus pasos, el elemento de interfaz 
///             que debe atender la solicitud no existe, se pasa la solicitud al eslabon anterior de dicha cadena de gestión, si este hubiera dejado de existir, se pasa al anterior
///             y asi sucesivamente, hasta que llegaramos al primero (solicitante). Si el solicitante tambien hubiera dejado de existir, este gestor de solicitudes eliminara dicha 
///             solicitud
///             
///    - CADENA DE GESTION DE UNA SOLICITUD : Las solicitudes van siendo resueltas por sus intervinientes de forma secuencial, paralela o como proceda
///                 
/// </summary>

public class ScriptGestorSolicitudes : MonoBehaviour {


    protected List<GameObject> ListaSolicitudesALimpiar;

    void Awake()
    {
        ListaSolicitudesALimpiar = new List<GameObject>();
    }
    // Use this for initialization
    void Start () {
    }  // Fin de - void Start () {

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : Update
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-06
    /// Ultima modificacion :
    ///         - 2021-08-10. Lo adapto a la nueva programacion de las solicitudes
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones:
    /// 		    - En ejecucion "ScriptGestorSolicitudes" periodicamente, analiza la lista "ScriptDatosInterfaz =>ListaSolicitudes" y mira si alguna requiere
    /// 		    atencion y de quien la requiere. Para esto mira la lista de intervinientes de todas las solicitudes que estan "estado_solicitud = estado_enProceso" 
    /// 		    En caso afirmativo. "ScriptGestorSolicitudes" incluye la solicitud en la lista de solicitudes pendientes "misSolicitudesPendientes" del elemento
    /// 		    de interfaz que es requerido por la solicitud. Este elemento de interfaz llama al metodo "atiendeMariLlanos" para procesar la
    /// 		    solicitud, segun su naturaleza
    /// </summary>

    void Update () {

        if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)  // Si tenemos alguna solicitud pendiente
        {
            foreach (GameObject solicitud in GetComponent<ScriptDatosInterfaz>().ListaSolicitudes)
            {
                try  // de solicitud
                {
                    // Si la solicitud esta en proceso, puede ser que tenga intervinientes que la requieran
                    if (solicitud.GetComponent<ClassSolicitud>().estado_solicitud == ClassSolicitud.estado_enProceso)
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). Otra solicitud  : " + solicitud.name); }
                        int ordinal = 0;
                        bool solicitudTerminada = true;
                        // Recorremos ahora la lista de intervinientes para ver si se requiere la atencion de alguno de ellos
                        foreach (ClassInterviniente este_interviniente in solicitud.GetComponent<ClassSolicitud>().listaIntervinientes)
                        {
                            if (DatosGlobal.niveDebug > 1000)
                            { Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). Interviniente, con ordinal : " + ordinal + " - y estado = " + este_interviniente.estado_de_la_itervencion); }
                            try // de Interviniente
                            {
                                // Si la solicitud esta en proceso, puede ser que tenga intervinientes que la requieran
                                if (este_interviniente.estado_de_la_itervencion == ClassInterviniente.estado_itervencion_requiereAtencion)
                                {
                                    // Si requiere atencion de alguien, la ponemos en su lista de solicitudes pendientes
                                    GameObject objAlQueSeRequiere = este_interviniente.Interviniente;
                                    objAlQueSeRequiere.GetComponent<ScriptDatosElemenItf>().misSolicitudesPendientes.Add(solicitud.gameObject);
                                }  // FIn de - if (este_interviniente.estado_de_la_itervencion == ClassInterviniente.estado_itervencion_requiereAtencion)
                                else if (este_interviniente.estado_de_la_itervencion == ClassInterviniente.estado_itervencion_Error)
                                {
                                    // Si se ha producido un error en la intervencion, hay que actuar en consecuencia
                                }
                                else if (este_interviniente.estado_de_la_itervencion != ClassInterviniente.estado_itervencion_finalizada)
                                {
                                    // Si todas las intervenciones han termonado, la solicitud ha terminado
                                    solicitudTerminada = false;
                                }
                                if (DatosGlobal.niveDebug > 1000)
                                { Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). solicitudTerminada  : " + solicitudTerminada); }
                            }  // FIn de - try de Interviniente
                            catch
                            {
                                // Si falla el acceso a algun interviniente es posible que el objeto se haya destruido. 
                                // hay que informar al resto de intervinientes de la solicitud que el interviniente de ordinal "ordinal" no es accesible
                                if (DatosGlobal.niveDebug > 90)
                                { Debug.Log("ERROR desde ScriptGestorSolicitudes =>  Update (). Interviniente no accesible, con ordinal : " + ordinal); }
                            }  // FIn de - catch  de Interviniente
                            ordinal++; // Incrementamos el ordinal

                        }  // FIn de - foreach (ClassInterviniente interviniente in solicitud.GetComponent<ClassSolicitud>().listaIntervinientes)
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). solicitudTerminada antes del if  : " + solicitudTerminada); }
                        if (solicitudTerminada)
                        {
                            if (DatosGlobal.niveDebug > 1000)
                            {
                                Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). solicitudTerminada dentro del if 1 : " + solicitudTerminada);
                                Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). solicitudTerminada dentro del if 2 : " + solicitudTerminada);
                                Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). solicitudTerminada dentro del if 3 : " + solicitudTerminada);


                                int cuantos = 7;
                                Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). Con solicitud añadida tamaño lista 1 : " + cuantos);

                                Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). Con solicitud añadida tamaño lista 1 : " + ListaSolicitudesALimpiar.Count);
                            }
                            // Eliminamos la solicitud. Pero no podemos eliminarla en el foreach que la contiene, asi que ...
                            // la agregamos a LISTA DE SOLICITUDES A ELIMINAR
                            ListaSolicitudesALimpiar.Add(solicitud);
                            if (DatosGlobal.niveDebug > 1000)
                            { Debug.Log("Desde 95 ScriptGestorSolicitudes =>  Update (). Con solicitud añadida tamaño lista 2 : " + ListaSolicitudesALimpiar.Count); }
                        }
                    }  // FIn de - if (solicitud.GetComponent<ClassSolicitud>().estado_solicitud == ClassSolicitud.estado_enProceso)
                } // FIn de - try de solicitud
                catch
                {
                    if (DatosGlobal.niveDebug > 90)
                    { Debug.Log("ERROR desde ScriptGestorSolicitudes =>  Update (). Intentamos acceder a un objeto solicitud en ListaSolicitudes, con "); }
                }  // FIn de - catch  de solicitud
            }  // FIn de - foreach (GameObject solicitud in GetComponent<ScriptDatosInterfaz>().ListaSolicitudes)

            // Eliminamos las solicitudes que han terminado 
            if (ListaSolicitudesALimpiar.Count > 0)  // Si hay solicitudes por limpiar
            {
                limpiaListaSolicitudes();
            }
        }  // FIn de - if (GetComponent<ScriptDatosInterfaz>().ListaSolicitudes != null)
    }  // Fin de - void Update () {


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : public void limpiaListaSolicitudes()
    ///     Borra las solicitudes que han terminado. Estan en la lista "ListaSolicitudesALimpiar"
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-08-10
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo limpia las solicitudes
    /// Observaciones :
    ///     - 
    /// </summary>
    public void limpiaListaSolicitudes()
    {
        // Utilizamos la LISTA DE SOLICITUDES A ELIMINAR
        // solicitudAAtender.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();
        while (ListaSolicitudesALimpiar.Count > 0)
            {
                try  // de solicitud
            {
                GameObject SolicitudQueSaco = ListaSolicitudesALimpiar[0];
//                ListaSolicitudesALimpiar.Remove(SolicitudQueSaco); // La quitamos de la lista de solicitudes a limpiar
//                SolicitudQueSaco.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf(); // Eliminamos la solicitud
                ListaSolicitudesALimpiar.RemoveAt(0); // La quitamos de la lista de solicitudes a limpiar
                SolicitudQueSaco.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf(); // Eliminamos la solicitud
            }
            catch
            {
                if (DatosGlobal.niveDebug > 90)
                { Debug.Log("ERROR desde ScriptGestorSolicitudes =>  limpiaListaSolicitudes()"); }
            }
        }  // Fin de - while (ListaSolicitudesALimpiar != null)
    }  // FIn de -  public void limpiaListaSolicitudes()

}  // Fin de - public class ScriptGestorSolicitudes : MonoBehaviour {
