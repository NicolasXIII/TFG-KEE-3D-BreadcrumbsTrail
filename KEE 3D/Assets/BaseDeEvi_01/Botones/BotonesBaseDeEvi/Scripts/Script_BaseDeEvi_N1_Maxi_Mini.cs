using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BaseDeEvi_N1_Maxi_Mini : MonoBehaviour {

    public GameObject ctrlInterfaz;

    private Rigidbody rb;

    private float escalaLocalAnchoBoton;
    private float escalaLocalAltoBoton;

//    public Material MaterialBtnMaximiza;
//    public Material MaterialBtnMinimiza;

    private void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");

    }


    // Use this for initialization
    void Start()
    {
    escalaLocalAnchoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoBoton;
    escalaLocalAltoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoBoton;

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Activado;
        Vector3 ajusteDePposition = new Vector3(escalaLocalAnchoBoton * (-1f / 2f), 0, escalaLocalAltoBoton *(1 / 2f)); 
        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Maxi_Mini + ajusteDePposition;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // ///////////
        // SI pulsamos el raton cuando estamos sobre el boton, generamos un evi ce busqueda
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            {
                string subTipoEviBase = this.transform.parent.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();
                if (this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().maximizado)
                {
                    // Desactivamos el contenedor
                    this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.SetActive(false);
                    this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().maximizado = false;
                    // Actualizamos el boton de maxi-mini, segun proceda
                    if (subTipoEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
                        { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximizaInstancia; }
                    else if (subTipoEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
                        { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximizaSinTecho; }
                    else // Si no es instancia ni sin techo (osea, referencia o cualquier otra cosa), va con el material normal (blanco)
                        { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximiza; }
                }  // Fin de - if (this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().maximizado)
                else
                {
                    // Activamos el contenedor
                    this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.SetActive(true);
                    this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().maximizado = true;
                    // Actualizamos el boton de maxi-mini, segun proceda
                    if (subTipoEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
                    { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimizaInstancia; }
                    else if (subTipoEviBase == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
                    { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimizaSinTecho; }
                    else // Si no es instancia ni sin techo (osea, referencia o cualquier otra cosa), va con el material normal (blanco)
                    { this.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnMinimiza; }
                }  // Fin de - else 
            } // FIn de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
        }
    } // Fin de - void OnTriggerStay(Collider other)

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Desactivado;
        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Maxi_Mini;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
