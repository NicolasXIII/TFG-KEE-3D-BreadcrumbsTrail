using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// /////////// SctlEviTipo_01Fractum_BtnIndividualizar : Script para manejar el boton de individualizar dentro de opciones de un evi fractum de referencia
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-04-22
/// Observaciones :
/// 		Este boton tiene el icono del concepto o instancia que contiene.
/// 		Al pasar por encima del boton con el raton, se visualiza el texro de ayuda a interfaz (en el cambas general) y se
/// 		reproduce tambien el audio de ayuda a interfaz
/// </summary>
public class SctlEviTipo_01Fractum_BtnIndividualizar : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public Canvas CanvasGeneral;


    private Rigidbody rb;

    private float escalaLocalAnchoBoton;
    private float escalaLocalAltoBoton;

    // Use this for initialization
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;

        //        escalaLocalAnchoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoBoton;
        //        escalaLocalAltoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoBoton;
        // De primeras ponemos el audio por defect, ya que el audio del fractum podria no estar cargado todabia
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
        { Debug.Log(" Desde SctlEviTipo_01Fractum_BtnIndividualizar => OnTriggerEnter entramos en el triger"); }
        // ///////////
        // Si entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño y lo recolocamos
        //       this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Activado;
        //        Vector3 ajusteDePposition = new Vector3(escalaLocalAnchoBoton * (-1f / 2f), 0, escalaLocalAltoBoton * (-1 / 2f)); ;
        //        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info + ajusteDePposition;
        /////////////  asi no me lo admitia y he tenido que ponerlo como sigue (referenciando el componente

        // Si pulsamos el raton, generamos un evi nuevo e independiente con la descripcion del concepto que este fractum visualiza


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
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Boton del mouse activado en algun punto de SctlEviTipo_RF01FractumRef_BtnIndividualizar"); }

                // 2.2.1.) Si es UNA REFERENCIA : 
                // -  Tomo sus datos de identificacion
                // Vamos con el identificador

                string ref_key = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().key;
                string ref_host = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().host;                         // host : es el host del concepto (H en KDL - esta en I en KDL)
                string ref_cualificador = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().cualificador;                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)
                string ordinalConf = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().ordinal;
                string ultiModConfEnString = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().fechUltMod;
                DateTime ultiModConf = new DateTime(0); // Habria que convertir la ultima modificacion en (DateTime)ultiModConfEnString;
                GameObject elemDestino = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo;

                // Asignamos a las variables de este game object

                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(ref_key, ref_host, ref_cualificador, ordinalConf, ultiModConf, elemDestino);

                if (DatosGlobal.niveDebug > 1000)
                {
                    Debug.Log(" 94 Desde SctlEviTipo_01Fractum_BtnIndividualizar => OnTriggerStay. Generamos evi independiente para concepto. " +
                    " - Con ref_key = " + ref_key +
                    " - ref_host = " + ref_host +
                    " - ref_cualificador = " + ref_cualificador +
                    " - ordinalConf = " + ordinalConf +
                    " - ultiModConf = " + ultiModConf +
                    " - elemDestino = " + elemDestino.name);
                }
            }  // Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))

        }  // FIn de - if (Input.GetMouseButtonDown(0)) 

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
