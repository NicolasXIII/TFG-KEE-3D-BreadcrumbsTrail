using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ClassFichero :  gestionar la informacion de los elementos de interfaz que intervienen en las solicitudes
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2022-03-04
/// Observaciones :
///     Esta clase se utiliza para gestionar los elementos de interfaz que intervienen en las solicitudes.
///     Estaran en la lista de intervinientes de cada solicitud e indican quien ha actuado, quien tiene que actuar y cuando
///     -Ver "ClassSolicitud" y "ScriptGestorSolicitudes"
///     
/// </summary> 

public class ClassFichero
{
    public string nombre_origen; // Es el nombre del fichero en el sistema
    public string extension; // es la extension del fichero (jpg, wav, etc...)
    public string nombre_enKee; // Cuando el fichero es copiado desde el sistema al directorio de unity, se le da un nombre especifico y unico en este KEE
    public string path_enKee; // ruta al directorio donde se encuentra el directorio en KEE (no incluye el nombre del fichero)
    public bool modificado; // true si ha sido modificado; false si NO ha sido modificado (si no ha sido modificado, no tiene que reenviarse al DKS, 
                            // ya que alli debe haber una copia anterior
    public long Tiempo_copia_enKee;     // es el milisegundo en el que se copia al fichero del sistema a UNITY (osea al KEE )
    public long Tiempo_de_vida;  // Definimos este intervalo de tiempo, para que los ficheros puedan borrarse si quedaran obsoletos u olvidados en el KEE
}
