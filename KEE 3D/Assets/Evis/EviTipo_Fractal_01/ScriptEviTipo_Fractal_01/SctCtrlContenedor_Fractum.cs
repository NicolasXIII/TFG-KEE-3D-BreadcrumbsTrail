using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control el contenedor que esta dentro de un fractum
/// 
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-04-10
/// Observaciones :
/// 		- Por ahora tan solo hace visible, sobre el texto del canvas general, el texto asociado al dato sin techo, si el fractum lo es de tipo sin techo
///			
/// </summary>
public class SctCtrlContenedor_Fractum : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public Canvas CanvasGeneral;

    // Start is called before the first frame update
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;

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
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde CrtCtrlContenedor_FractumRef => OnTriggerEnter entramos en el triger"); }
        // ///////////
        // SI entramos en el contenedor de un fractum de tipo Sin techo, cuando entra el raton, visualizamos el dato sin techo en el texto del canvas

        if ( (this.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace) == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_SinTecho)) 
        {
            // Activamos el texto de ayuda a interfaz
            // Cargamos el texto en el TextMeshProUGUI correspondiente
            string textoSinTecho_T = this.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().datoSinTecho;

        }  // FIn de - if ( (this.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace) == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_SinTecho)) )

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        //        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Desactivado;
        //        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info;

        // Al salir desactivamos el texto de ayuda a interfaz
        //        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().TexAyuIntfBaseDeEvi.SetActive(false);
        //        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.SetActive(false);

    } // Fin de - void OnTriggerEnter(Collider other) 


}
