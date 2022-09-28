using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ClassInterviniente :  gestionar la informacion de los elementos de interfaz que intervienen en las solicitudes
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-08-09
/// Observaciones :
///     Esta clase se utiliza para gestionar los elementos de interfaz que intervienen en las solicitudes.
///     Estaran en la lista de intervinientes de cada solicitud e indican quien ha actuado, quien tiene que actuar y cuando
///     -Ver "ClassSolicitud" y "ScriptGestorSolicitudes"
///     
/// </summary> 

public class ClassInterviniente
{

    public GameObject Interviniente; // Es el elemento de interfaz que realiza la interaccion.
    public int ordinal_Interviniente; // Es un identificador unico para cada uno de los intervinientes de la solcitud (se sugiere hacer coincidir el ordinal con el orden de intervencion).
    public string tipo_Interviniente; // Sirve par indentificar la funcion del interviniente, puede haber uno o varios con el mismo tipo Ej,: un solicitante, varios receptres
                                        // el tipo de los distintos intervinientes se define especificamente para cada tipo de solicitud

    public DateTime hora_ini_intervencion;  // Instante en el que se cursa la solicitud, puede no coincidir en el momento en el que se crea el objeto solicitud (que esta en "ScriptDatosElemenItf. horaDeGenesis")
    public DateTime hora_fin_intervencion;     // Instante en el que se recibe la respuesta

    // Estados de la solicitud
    public string estado_de_la_itervencion;   // identifica el estado en el que se encuentra la solicitud
        public static string estado_itervencion_NoIniciada = "NoIniciada"; //  Todabia no le ha tocado intervenir en la solicitud
        public static string estado_itervencion_requiereAtencion = "requiereAtencion"; // Alguien (normalmente "anterior_Interviniente") indica que este interviniente debe actuar en la solicitud
                                                                                       // OJOOO solo cuando esta en este estado "ScriptGestorSolicitudes" lo envia a la lista de solicitudes 
                                                                                       // pendientes del elemento de interfaz "Interviniente!
        public static string estado_itervencion_enEjecucion = "enEjecucion"; //  Este interviniente esta trabajando en la solicitud
        public static string estado_itervencion_finalizada = "finalizada"; //  Este interviniente ha terminado su trabajo en la solicitud
        public static string estado_itervencion_Error = "Error"; //  Este interviniente ha terminado su trabajo en la solicitud

}

