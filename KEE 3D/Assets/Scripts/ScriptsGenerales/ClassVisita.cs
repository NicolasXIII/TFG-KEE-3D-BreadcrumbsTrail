using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ClassVisita :  para identificar las visitas (accesos) del usuario a los elmentos de la interfaz
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-03-12
/// Observaciones :
///     Esta clase se utiliza para identificar los accesos del usuario por los elmentos de la interfaz
///     -Ver "ScriptGestorVIsitas"
///     
/// </summary>    
public class ClassVisita {

    public int idVisita; // identifica a quien realiza la interaccion.
    public int idElementoVisitado;  // Es el id (general) del elemento de interfaz que ha sido visitado
    public string tipoElementoVisitado;  // Es el tipo del elemento de interfaz que ha sido visitado
    public string tipoVisitante; // Es el tipo del elemento que visita este elemento de interfaz ( ver "tipoElementIntf" y sus tipos en esta misma clase
    public int idDeQuienVsisita; // identifica a quien realiza la interaccion.
                                 // Casos particulares
                                 // 0 - Es el usuario quien ha interacturado con el elemento de interfaz
                                 // En los casos en los que el elemento tiene un "idElementIntf", ese sera el valor
                                 // Por ahora no defino el tipo de acceso
    public DateTime horaDeInicioInteraccion;  // Momento en el que se inicia la interaccion con el elemento de interfaz
    public DateTime horaDeFinInteraccion;  // Momento en el que se finaliza la interaccion con el elemento de interfaz
}
