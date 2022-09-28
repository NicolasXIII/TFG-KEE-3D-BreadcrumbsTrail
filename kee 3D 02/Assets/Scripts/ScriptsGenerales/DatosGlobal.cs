using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatosGlobal 
{
    // Esta variable se usa para definir que logs se generasn, segun sigue
    //  - niveDebug > 1000 : se disparan todos los logs
    //  - niveDebug > 100 : se disparan los logs de los else por opciones incorrectas, etc
    //  - niveDebug > 90 : se disparan los logs asociados a errores
    // Los logs que se quieran ver en cada momento se deben poner 101 < niveDebug < 1000, para asi ver los errores significativos
    // Suelo poner niveDebug > 50  para poder localizarlos y quitarlos cuando termino con esa linea de depuracion de codigo
    public static int niveDebug = 110;

    // //////////////////////////////////////////////////////////////////////////
    // Para lostag que usan los distintos elementos
    public static string tag_PunteroUsuario = "PunteroUsuario";  // Debe coincidir con el tag del assest "PunteroUsuario"
    public static string tag_PunteroTramoya = "PunteroTramoya";  // Debe coincidir con el tag del assest "PunteroUsuario"

    // //////////////////////////////////////////////////////////////////////////

    // Para los distintos tipos de entidades (para generar el identificador unico por ejemplo) Ver "genera_identificador_unico()"
    public static string tipo_entidad_fichero = "ent_fich";



}
