using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ClassTransito :  para gestionar los estados de transito del sistema
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-03-11
/// Observaciones :
///     -Ver "ScriptGestorTransitos"
///     
/// </summary>    


public class ClassTransito {

    public GameObject ElementoQuelOGenera;  // Es el elemento que esta ha generado la situacion de transito
    public int idTransito;
    public string estado;  // Indica si el transito esta en proceso o no. Las posibilidades son :
                           // "enTransito" - indica que el transito esta activo 
    public string tipo;  // Indica si el tipo de transito. Las posibilidades son :
                         //  "usuarioEnMovimiento"
    public DateTime dateInicio;  // Momento en el que el usuario comenzo el transito (en formato date)

}
