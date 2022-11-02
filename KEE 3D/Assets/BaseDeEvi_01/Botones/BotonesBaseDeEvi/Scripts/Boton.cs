using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ////////////////////////////////////////////////////////////////////////////////////// 
/// ///////////  Clase para gestionar la operativa de los botones de guardar y cancelar la edicion de los evis sin techo
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-10-xx
/// Observaciones :
///     - Al parecer, para que funcione, esta clase tiene que llamarse Boton, y tiene que estar asociada como componente a la clase que se asigna en el onclick del boton correspondiente
///			-
/// </summary>
/// 
public class Boton : MonoBehaviour
{

    public GameObject ctrlInterfaz;
    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
    }

        public void cancelaEdicion_Text_T_deSinTecho()
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en Scpt_CanvasEviBase_Btn_Cancelar_Text_T_deSinTecho =>Start. Desde cancelaEdicion_Text_T_deSinTecho : " + this.gameObject.name); }

        //Output this to console when Button1 or Button3 is clicked
        this.transform.parent.gameObject.GetComponent< ScriptCtrlBaseDeEvi>().este_Panel_Input_Text_SinTecho.gameObject.SetActive(false);
    }

    public void guardaEdicion_Text_T_deSinTecho()
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en Scpt_CanvasEviBase_Btn_Cancelar_Text_T_deSinTecho =>Start. Desde guardaEdicion_Text_T_deSinTecho  VOY A GUARDAR : " + this.gameObject.name); }
        string nuevoTexto = this.transform.parent.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().este_Input_Text_T_deSinTecho.text;

        this.transform.parent.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().texto_T_deSinTechoCanvasEviBase = nuevoTexto;  // Actualizamos la variable que contiene el valor asociado al elemento T del KDL
        this.transform.parent.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().caja_Tex_T_deSinTechoCanvasEviBase.text = nuevoTexto;  // Actualizamos el texto en

        // Actualizamos tambien la variable "datoSinTecho" del componente "SctCtrlEviTipo_01Fractum" del gameObject "EviTipo_RF01_Fractum" que es de donde la recojeran para ponerla en los KDL correspondientes
        // Tenemos que "CanvasEviBase" es hijo de "BaseDeEvi_01" que es padre de "ContenedorDeEvi_01" que es padre de "EviTipo_Fractal_01" que es padre de "EviTipo_RF01_Fractum" que es quien  guarda el dato sin techo
        this.transform.parent.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<SctCtrlEviTipo_01Fractum>().datoSinTecho = nuevoTexto;

        //Desactivamos el panel de edicion
        this.transform.parent.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().este_Panel_Input_Text_SinTecho.gameObject.SetActive(false);
    }

    public void cierra_PanelCanvasCompleto()
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en ublic class Boton : MonoBehaviour =>cierra_PanelCanvasCompleto(). Cerraando PanelCanvasCompleto "); }

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PanelCanvasCompleto.SetActive(false);
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextCanvasGeneralCompleto.text = "";
    }

}
