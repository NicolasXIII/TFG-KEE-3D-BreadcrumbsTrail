using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Script_BaseDeEvi_N1_Desplazar : MonoBehaviour {

    public GameObject ctrlInterfaz;

    private Rigidbody rb;

    private float escalaLocalAnchoBoton;
    private float escalaLocalAltoBoton;

    // Use this for initialization
    void Start()
    {
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");

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
    void OnTriggerEnter(Collider elOtro)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde Script_BaseDeEvi_N1_Desplazar => OnTriggerEnter desde other.name : " + elOtro.name); }
        if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario || elOtro.gameObject.tag == DatosGlobal.tag_PunteroTramoya)
//        if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario)
            {
                // ///////////
                // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
                this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Activado;
            Vector3 ajusteDePposition = new Vector3(escalaLocalAnchoBoton * (1f / 2f), 0, escalaLocalAltoBoton * (-1f / 2f)); ;
            this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Desplazar + ajusteDePposition;
        }

} // Fin de - void OnTriggerEnter(Collider other) 

// Cuando llega a el el puntero de usuario
void OnTriggerStay(Collider elOtro)
    {
        if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario || elOtro.gameObject.tag == DatosGlobal.tag_PunteroTramoya)
        {
            // ///////////
            // SI pulsamos el raton cuando estamos sobre el boton, activamos el arrastre del evi
            if (Input.GetMouseButton(0))
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" Desde Script_BaseDeEvi_N1_Desplazar -desde Input.GetMouseButton- => OnTriggerStay.Input.GetMouseButton desde other.name " + elOtro.name); }
                if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre == null)
                {
                    this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().arrastrando = true;
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre = this.transform.parent.gameObject;
                }
            }
            else
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" Desde Script_BaseDeEvi_N1_Desplazar - desde else - => OnTriggerStay.Input.GetMouseButton desde other.name " + elOtro.name); }
                this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().arrastrando = false;
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre = null;
            }
        }  // Fin de - if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario)
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider elOtro)
    {
        if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario || elOtro.gameObject.tag == DatosGlobal.tag_PunteroTramoya)
        {
            // ///////////
            // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
            this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Desactivado;
            this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Desplazar;
            this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().arrastrando = false;
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().objetoEnArrastre = null;
        }  // FIn de - if (elOtro.gameObject.tag == DatosGlobal.tag_PunteroUsuario)
    } // Fin de - void OnTriggerEnter(Collider other) 
}

