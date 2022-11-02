using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ScriptGestorVIsitas :  para gestionar los recorridos del usuario por los elmentos de la interfaz
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-03-12
/// Observaciones :
///     Esta clase se utiliza para identificar los recorridos del usuario por los elmentos de la interfaz
///     La idea es llevar un control de los accesos, para poder volver atras, definir los elementos preferidos o mas 
///     visitados, hacer propuestas o presentar elementos al usuario segun su comportamiento o si hay elementos de interfaz 
///     sin actividad ninguna desde hace mucho tiempo, poder eliminarlos del arbol (u otros mecanismos funcionales o de 
///     aprendizaje del sistema)
///     Este tipo de proceso deberia ejecutarlo un agente de intrfaz
///     -Ver "ScriptGestorTransitos"
///     
/// </summary>
public class ScriptGestorVIsitas : MonoBehaviour {

public List<ClassVisita> ListaVisitas; // Es una lista de los accesos que el sistema realiza sobre el elemento de interfaz. La idea es llevar un control de los
                                         // accesos, para si hay elementos de interfaz sin actividad ninguna desde hace mucho tiempo, poder eliminarlos del arbol
                                         // Este tipo de proceso deberia ejecutarlo un agente de intrfaz. Ver "public class Visitas"
    protected int ultimaVisita;  // es un contador que se incrementa por cada visita generada, para identificar cada una de forma univoca


    // This function is executed al principio
    void Awake()
    {

        // OJOOOOOOO, lo que sigue lo he quitado del awake y lo he puesto en el strrat (para la prueba de esferas), porque no se dan de alta ecvis, si no ramas y muros
        // Apuntamos el EVI en la lista dorrespondiente
        // ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaEvis.Add(gameObject);
        ListaVisitas = new List<ClassVisita>();
        ultimaVisita = 0;
    }

    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : anotaInicioVisita : genera nera un objeto visita e inicializa sus datos
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-13
    /// Ultima modificacion :
    ///
    /// Variables de entrada :
    ///     - int idElementoVisitado : Identificador (como elemento de interfaz general) del elemento visitado
    ///     - string tipoElementoVisitado : Tipo del elemento visitado
    ///     - string tipoVisitante : Tipo del visitante
    ///     - int idDeQuienVsisita  : Identificador (como elemento de interfaz general) del elemento visitante
    /// Variables de salida :
    ///     - nuevoIdVisita : es el id de la visita, para que quien la inicio lo conozca y pueda notificar el final de la misma (entre otras cosas)
    /// Observaciones:
    /// </summary>
    public int anotaInicioVisita(int idElementoVisitado, string tipoElementoVisitado, string tipoVisitante, int idDeQuienVsisita)
    {
        ClassVisita nuevaVisita = new ClassVisita();
        DateTime horaDeInicioInteraccion = new DateTime();
        int nuevoIdVisita = ultimaVisita;
        ultimaVisita++;

        nuevaVisita.idVisita = nuevoIdVisita;
        nuevaVisita.idElementoVisitado = idElementoVisitado;
        nuevaVisita.tipoElementoVisitado = tipoElementoVisitado;
        nuevaVisita.idVisita = ultimaVisita;
        nuevaVisita.tipoVisitante = tipoVisitante;
        nuevaVisita.idDeQuienVsisita = idDeQuienVsisita;
        nuevaVisita.horaDeInicioInteraccion = DateTime.Now;

        ListaVisitas.Add(nuevaVisita);

        return nuevoIdVisita;

    }  // Fin de - public void anotaVisita(string tipoVisitante, int idDeQuienVsisita)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : anotaFinVisita : Anota la finalizacion de una visita
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-13
    /// Ultima modificacion :
    ///
    /// Variables de entrada :
    ///     - int idVisita : Identificador de la visita que finaliza
    /// Variables de salida :
    ///     - NO TIENE
    /// Observaciones:
    /// </summary>
    public void anotaFinVisita(int idVisita)
    {
        if (ListaVisitas.Count > 0)  // Si tenemos algun hijo en la lista
        {
            foreach (ClassVisita visita in ListaVisitas)
            {
                if (visita.idElementoVisitado == idVisita)
                {
                    visita.horaDeFinInteraccion = DateTime.Now;
                }
            } // Fin de -  foreach (ClassVisita visita in ListaVisitas)
        }  // Fin de - if (ListaVisitas.Count > 0)
    }  // Fin de - public void anotaVisita(string tipoVisitante, int idDeQuienVsisita)

}  // Fin de - public void anotaFinVisita(int idVisita)
