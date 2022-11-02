using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ClassPuntero, para gestionar los datos propios de todo puntero
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-06-28
/// Observaciones :
/// 		Generamos esta clase para poder gestionar el estado de los punteros desde cualquier boton. Necesitabamos una estructura similar en todos los punteros
/// 		ya que queremos acceder a estos datos del mismo modo sea clual fuere el puntero
/// </summary>
public class ClassPuntero : MonoBehaviour
{
    public string estadoPuntero;  // Indica si el puntero esta activo y a que nivel. Los posibles valores estan en  "ScriptDatosInterfaz"

    // Para definir el nivel de activacion de los punteros
    public static string estPuntero_activo = "estPuntero_activo";  // Indica que el puntero esta activo para todo
    public static string estPuntero_NoActivaEvis = "estPuntero_NoActivaEvis";  // Indica que el puntero esta activo para todo, pero no actua al clicar sobre los evis
    public static string estPuntero_NoActivaBotones = "estPuntero_NoActivaBotones"; // Indica que el puntero esta activo para todo, pero no actua al clicar sobre los botones
    public static string estPuntero_desactivado = "estPuntero_desactivado"; // Indica que el puntero no esta activo
                                                                            //        - PunteroUsuario : 
                                                                            //              - Estara activo cuando el foco esta en el muro de trabajo 
                                                                            //              - Estara inactivo cuando el usuario tenga el foco en los telones
                                                                            //        - PunteroTramoya 
                                                                            //              - Estara activo cuando el usuario tenga el foco en los telones
                                                                            //              - Estara inactivo cuando el foco esta en el muro de trabajo 

}  // FIn de - public class ClassPuntero 
