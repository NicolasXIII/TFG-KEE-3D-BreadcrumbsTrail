using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_CtrlInterfaz_idiomas : MonoBehaviour {

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
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_idiomas_Activado;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_idiomas;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
