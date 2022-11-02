using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Escena_CreaConcepto : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    private Rigidbody rb;

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
    } // FIn de - void Start ()

    void Update()
    {
    } // Fin de - void LateUpdate ()


    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaConcepto_Activado;
    } // Fin de - void OnTriggerEnter(Collider other) 


    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // ///////////
        // SI pulsamos el raton cuando estamos sobre el boton, generamos un nuevo evi de concepto fractal vacio, teoricamente para editarlo y generar un nuevo concepto
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Boton del mouse activado en algun punto de SctlEviTipo_01Fractum_BtnIndividualizar"); }

                // 2.2.1.) Si es UNA REFERENCIA : 
                // -  Tomo sus datos de identificacion
                // Vamos con el identificador

                string ref_key = ConceptosConocidos.gen_NuevoConcepto_Key;
                string ref_host = ConceptosConocidos.gen_NuevoConcepto_host;                         // host : es el host del concepto (H en KDL - esta en I en KDL)
                string ref_cualificador = ConceptosConocidos.gen_NuevoConcepto_cualifi;                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)
                string ordinalConf = ConceptosConocidos.gen_NuevoConcepto_ordinalConf;
                string ultiModConfEnString = ConceptosConocidos.gen_NuevoConcepto_ultiModConfEnString;
                DateTime ultiModConf = new DateTime(0); // Habria que convertir la ultima modificacion en (DateTime)ultiModConfEnString;
                GameObject elemDestino = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo;

                // Asignamos a las variables de este game object

                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(ref_key, ref_host, ref_cualificador, ordinalConf, ultiModConf, elemDestino);

            }  // Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
        }//  Fin de - if (Input.GetMouseButtonDown(0))
    } // Fin de - void OnTriggerEnter(Collider other) 



    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaConcepto;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
