using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script para la gestion de transitos
/// Se encarga gestionar las estados de transito del sistema
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-03-11
/// Observaciones :
///     - Para que el sistema sepa si hay alguno activo.Algunos tronsitos activos
///     deben impedir que se realicen algunas acciones. Por ejemplo, cuando el usuario esta en transito entre un muro 
///     y otro, no debe generarse otro muro u otra rama, ya que si se genera, esta aparece con parametros de localizacion
///     referentes al usuaio en ese momento de transito, que no son los adecuados
///     - Cuando algun elemento inicia una transicion que debe impedir a otros realizar ciertas operaciones, debe generar una 
///     instancia de esta clase mediante la funcion "public int inicioTransito" que esta en "ScriptDatosInterfaz"
///     
///     * Algunos ejemplos de estado de transito en los cuales no pueden realizarse otras operaciones
///         - 1) Mientras el usuario esta viajando de un muro a otro:
///                 No se deben generar ni borrar muros ni ramas, ya que estos se generarian en localizaciones con algunas 
///                 referencias a la localizacion del usuario, que mientras esta en transito, ocupa posiciones que harian generar
///                 muros o ramas fuera de las localizaciones deseadas y en estados tambien no deseados
///     
/// </summary>

public class ScriptGestorTransitos : MonoBehaviour {

    public int numTransito;  // Esa variable se utiliza para identificar cada transito de los que se realizan durante el funcionamiento de la interfaz
    public int numTransitosActivos;  // Esa variable la añado para poder ver en el inspector el valor

    public List<ClassTransito> ListaTransitos;  // Es una lista de transitos activos ( Ver "ClassTransito"
                                                // Tipos de transito
    public static string transTipo_usuarioEnMovimiento = "usuarioEnMovimiento";

    public static string transTipo_cualquiera = "cualquiera";  // Identifica cualquier tipo de transito
    public static string transTipo_ninguno = "ninguno";  // Identifica que no existe ningun tipo de transito
    public static string transTipo_alguno = "alguno";  // Identifica que existe algun tipo de transito

    void Awake()
    {
        ListaTransitos = new List<ClassTransito>();
        numTransito = 0;
        numTransitosActivos = 0;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        numTransitosActivos = ListaTransitos.Count;  // Para poder verlo en el inspector
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  public void inicioTransito : genera un transito "ClassTransito" y lo anota en la lista 
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-11
    /// Variables de entrada :
    ///     - GameObject quienInicia : Es el elemento que inicia el transito
    ///     - string tipoTransito : indica el tipo de transito 
    /// Variables de salida :
    ///     - idTransito : es el id que identifica el transito para eleminarlo cuando proceda
    /// Observaciones :
    ///     Ver las observaciones de la calse "ClassTransito"
    /// 	    
    /// </summary>
    public int inicioTransito(GameObject quienInicia, string tipoTransito)
    {
        // Generamos el identificador de transito
        numTransito++;
        int idTransito = numTransito;

        // Generamos el objeto de la clase transito y le ponemos sus datos
        ClassTransito nuevoTransito = new ClassTransito();
        nuevoTransito.ElementoQuelOGenera = quienInicia;
        nuevoTransito.idTransito = idTransito;
        nuevoTransito.tipo = tipoTransito;
        DateTime dateActual = new DateTime();
        dateActual = DateTime.Now;
        nuevoTransito.dateInicio = dateActual;

        ListaTransitos.Add(nuevoTransito); // Incluimos el transito en la lista de transitos

        return idTransito; // Devolvemos el identificador 

    }  // Fin de - public int inicioTransito(GameObject quienInicia, string tipoTransito)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  public void inicioTransito : elimina un transito "ClassTransito" de la lista  "ListaTransitos"
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-11
    /// Variables de entrada :
    ///     - GameObject quienInicio : Es el elemento que inicio el transito
    ///     - string idTransito : indica el idTransito del transito a eliminar
    /// Variables de salida :
    /// Observaciones :
    ///      Ver las observaciones de la calse "ClassTransito"
    /// 	    
    /// </summary>
    public void finalizoTransito(GameObject quienInicio, int idTransito)
    {
        bool transEnLista = false;
        ClassTransito transitoABorrar = null;

        if (ListaTransitos != null)  // Si tenemos algun transito pendiente
        {
            foreach (ClassTransito transito in ListaTransitos)
            {
                if (transito.idTransito == idTransito)
                {
                    transEnLista = true;
                    transitoABorrar = transito;
                }
            }
            if (!transEnLista)
            {
                if (DatosGlobal.niveDebug > 90)
                {
                    Debug.Log(" ERROR desde finalizoTransito. Se solicita eleiminar un transito. idTransito = " + idTransito +
                    " - y Nombre de quienInicio = " + quienInicio.name + ". - Pero EL ID NO ESTA EN LA LISTA");
                }
            }
        }
        else // Si no hay nada en la lista no podemos borrar el objeto de transito
        {
            if (DatosGlobal.niveDebug > 100)
            {
                Debug.Log(" ERROR desde finalizoTransito. Se solicita eleiminar un transito. idTransito = " + idTransito +
                " - y Nombre de quienInicio = " + quienInicio.name + ". - Pero no hay transitos en lista");
            }
        } // FIn de - else - de - if (GetComponent<ScriptDatosInterfaz>().ListaTransitos != null)
        if (transEnLista)
        {
            // Borramos el objeto de la lista
            ListaTransitos.Remove(transitoABorrar);

            // Destruimos el objeto, para que no ocupe sitio
            // Se ocupara el recogedor de basura. Posiblemente habria que optimizarlo (PENDIENTE MAFG 2021-03-11)
        }// Fin de - if (transEnLista)

    }  // Fin de - public void finalizoTransito(GameObject quienInicio, int idTransito)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  public void enTransito : mira a ver si hay algun transito en curso  "ClassTransito" en la lista  "ListaTransitos"
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-11
    /// Variables de entrada :
    ///     - GameObject quienPregunta : Es el elemento que realiza la consulta
    ///     - string tipoTransito : indica el tipo del transito por el que se pregunta. Si "tipoTransito = "transTipo_cualquiera"", se entiende que se pregunta por cualquier transito
    /// Variables de salida :
    /// Observaciones :
    ///      Ver las observaciones de la calse "ClassTransito"
    ///      - Por ahora solo respondemos para cualquier tipo de transito. Luego, si es necesario especificaremos funcionalidades distintaas con mas 
    ///      tipos de transito (PENDIENTE  MAFG 2021-03-11)
    /// 	    
    /// </summary>
    public string enTransito(GameObject quienInicio, string tipoTransito)
    {
        //    bool transEnLista = false;
        //    ClassTransito transitoABorrar = null;
        string respuesta = transTipo_alguno;

        if (ListaTransitos.Count == 0)  // Si tenemos algun transito pendiente
        {
            respuesta = transTipo_ninguno;
        }

        return respuesta;

    }  // Fin de - public void finalizoTransito(GameObject quienInicio, int idTransito)


}  // Fin de - public class ScriptGestorTransitos : MonoBehaviour {
