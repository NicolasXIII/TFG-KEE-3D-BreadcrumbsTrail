using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Escena_EliminaMuro : MonoBehaviour {

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
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_EliminaMuro_Activado;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // ///////////
        // SI pulsamos el raton cuando estamos sobre el boton, generamos un nuevo muro
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            {
                // Generamos un nuevo muro
                // El nuevo muro se genera en la rama que este activa y depende de los muros que tenga y de la posicion del muro desde el que 
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().eliminaMuro(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo);
            }  // Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
        }  //  Fin de - if (Input.GetMouseButtonDown(0))
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_EliminaMuro;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
