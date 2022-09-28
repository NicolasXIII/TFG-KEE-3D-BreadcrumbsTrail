using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Herramientas_buscador : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    private Rigidbody rb;

    public bool activo;

    // Variables para gestionar el periodo refractario del triger y la activacion de los puntoros que nos disparan
    private bool enTriger; // Estamos dentro del triger (el ontrigerStay me daba problemas. Funciona mejor asi)
    private GameObject quienDispara;  // Para enviarlo al gestor del periodo refractario, que controla tambien si el puntero esta activado

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        enTriger = false;
        quienDispara = null;

        activo = true;
    } // FIn de - void Start ()

    void Update()
    {
        // Si el muro activo es de edicion, este boton no debe aparecer, ya que en edicion no pueden añadirse muros (solo se añaden como ramas de descripcion de instancias)
        if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
        {
            activo = false;
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            activo = true;
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        // Si esta en estado de disparar los trigers, actuamos en consecuencia. Esto se hace asi, porque solo con los triggers daba problemas
        if (enTriger)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Desde ScriptBtnMenu_N2_1_Herramientas_buscador => Update mouse pulsado en frame : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }

                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
                {
                    GameObject nuevoEviBuscador = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().EviTipo_buscador_00) as GameObject;
                }  // FIn de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            }  // FIn de -  if (Input.GetMouseButtonDown(0))
        }  // FIn de - if (entriger)

    } // Fin de - void LateUpdate ()

        /// ///////////////////////////////////
        /// ///////////////////////////////////
        /// Vamos con los trigers
        /// <param name="other">Other.</param>

        // Cuando llega a el el puntero de usuario
        void OnTriggerEnter(Collider other)
    {
        if (activo)  // El boton solo debe trabajar si esta activo
        {

            // ///////////
            // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_buscador_Activado;

            enTriger = true;
            quienDispara = other.gameObject;
        }// Fin de - if (activo)

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_buscador;

        enTriger = false;
        quienDispara = null;
    } // Fin de - void OnTriggerExit(Collider other)

}
