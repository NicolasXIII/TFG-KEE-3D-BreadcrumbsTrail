using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// /////////////////////////////////////////////////////////////////
/// public class ScriptCtrlMuroTrabajo
/// 
/// Autor : Miguel Angel Fernandez Graciani
/// Fecha creacion : 202x
/// Ultima modificacion :
///     - 2021-03-24. Añado la funcion "" que indica la posicion donde debe colocarse cada evi al ser generado
/// Observaciones:
///             
/// </summary>
public class ScriptCtrlMuroTrabajo : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

    public string estado;   // identifica el estado en el que se encuentra el elemento
    public string estadoAnterior;   // identifica el estado en el que se encontraba el elemento en el cuadro anterior (sirve para detectar cambios de estao)
        public static string estado_muro_Activo = " estado_muro_Activo";  // Es el muro que actualmente esta activo en la interfaz (solo debe habedr uno)
        public static string estado_No_muro_Activo = "estado_No_muro_Activo";   // NO es el muro que actualmente esta activo en la interfaz (solo debe habedr uno)


    public float pos_x;
    public float pos_y;
    public float pos_z;

    public Material materialDeEsteMuro;

    public float longTramoDeRama; // Es la distancia que hay, en el eje x por el que crece la rama, desde el muro anterior o desde el origen de la rama si soy el primer muro

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// //////  void Awake()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 202x
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones:
    /// </summary>
    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        materialDeEsteMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro);

        this.GetComponent<MeshRenderer>().material = materialDeEsteMuro;

        estadoAnterior = estado_No_muro_Activo;



    }  // Fin de - void Awake()


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// //////  void Start ()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 202x
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones:
    /// </summary>
    void Start ()
	{	

		// ////////////////////////////////////////
		// Para el Muro de Trabajo
		transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaMuroTrabajo;
		// Para la orientacion espacial
		transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().giroOrientacionMuroTrabajo;
        //       transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Por ahora la longitud del tramo es la distancia entre muros que definimos inicialmete
        // OJOOO PEDIENTE MAFG 2021-02-23 ( este parametro luego sera variable dependiendo del crecimiento del arbol y habra que cambiar esto)
        longTramoDeRama = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros;

        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().colorMuroNavegacion);

        if (this.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
        {
            //            this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialMuro;  // MaterialMuroEdicion

            this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().colorMuroEdicion);

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Desde ScriptCtrlMuroTrabajo => Start en 91 con modo " + this.GetComponent<ScriptDatosElemenItf>().modo +
                            " - Con colorMuroEdicion" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().colorMuroEdicion +
                            " - Con color r  :" + this.gameObject.GetComponent<MeshRenderer>().material.color.r.ToString() +
                            " - Con color g  :" + this.gameObject.GetComponent<MeshRenderer>().material.color.g.ToString() +
                            " - Con color b  :" + this.gameObject.GetComponent<MeshRenderer>().material.color.b.ToString() +
                            " - Con color a  :" + this.gameObject.GetComponent<MeshRenderer>().material.color.a.ToString());
            }
        }


    } // FIn de - void Start ()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// //////  void Update ()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 202x
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones:
    ///     1.) COntrolamos el estado de activacion del muro
    ///     2.) Si se ha producido un cambio de estado
    ///         2.1.) Si el muro se ha activado
    ///         2.2.) Si el muro se ha desactivado
    ///             Recorremos todos lo hijos del muro indicandles que estan en un muro que se ha desactivado.
    ///             Esto les debe obligar a ajustar su configuración como sigue:
    ///                 - Deben desactivar todos los paneles que esten en el canvas general, ya que solo los elemntos del muro activo
    ///                     ponen su información en el canvas generas
    ///         2.3.) Actualizamos el estado de"estadoAnterior"
    /// </summary>
    void Update ()
	{
        // 1.) COntrolamos el estado de activacion del muro
        // Controlamos si somo el muro activo o no y actualizamos el estado en consecuencia
        if (this.gameObject == ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo) { estado = estado_muro_Activo;}
        else { estado = estado_No_muro_Activo;}

        // Controlamos ahora si ha habido un cambio en el estado de activacion del muro y actuamos en consecuencia
        if (estado != estadoAnterior)
        {

//            public void gestionaInfoCanvas(GameObject eviBase)
  
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Estoy en ScriptCtrlMuroTrabajo => Update 141 . Ha acaecido un cambio de estado con estadoAnterior = " + estadoAnterior + " - Y con estado = " + estado);}

                // Vamos mirando cada hijo, y si este esta en el canvas lo desactivamos
                foreach (GameObject hijo in this.transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
                {
                    if (hijo.gameObject.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi)  // Si el hijo es una rama 
                    {
                        if (DatosGlobal.niveDebug > 1000){ Debug.Log("Estoy en ScriptCtrlMuroTrabajo => Update 141 . desde dentro del foreach en el hijo de nombre  = " + hijo.name); }
                        ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().gestionaInfoCanvas(hijo.gameObject);
                    } // FIn de - if (hijo.transform.parent == GetComponent<ScriptDatosInterfaz>().CanvasGeneral)
                }  // Fin de - foreach (GameObject hijo in eviBase.transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)


            // 2.3.) Actualizamos el estado de"estadoAnterior"
            estadoAnterior = estado;

        }  // FIn de - if (estado != estadoAnterior)

    } // Fin de - void Update ()

/// <summary>
/// /////////////////////////////////////////////////////////////////
/// //////  void Awake()
/// Autor : Miguel Angel Fernandez Graciani
/// Fecha creacion : 202x
/// Ultima modificacion :
/// Variables de entrada :
/// Variables de salida :
/// Observaciones:
/// </summary>
public Vector3 damePosicionIniLocal()
    {

        // Para la posicion en el muro
        float margen = 20f; // es el tanto por ciento que dejo de margen para que no se me vallan los evis al borde, que no puedo manejarlos
        float hastaDonde = 0.5f - (0.5f * margen / 100f);

        float pos_x = Random.Range((-1) * hastaDonde, hastaDonde);
        float pos_y = Random.Range((-1) * hastaDonde, hastaDonde);
        float pos_z = 0.0f;
        Vector3 posicionLocalEvi = new Vector3(pos_x, pos_y, pos_z);

        return posicionLocalEvi;
    } // Fin de - void Update ()


}  // FIn de - public class ScriptCtrlMuroTrabajo : MonoBehaviour {
