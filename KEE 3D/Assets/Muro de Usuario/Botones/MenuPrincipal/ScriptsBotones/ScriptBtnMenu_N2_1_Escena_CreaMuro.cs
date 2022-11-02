using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Escena_CreaMuro : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    private Rigidbody rb;

    public bool activo;

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

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
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaMuro_Activado;
        }// Fin de - if (activo)

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        if (activo)  // El boton solo debe trabajar si esta activo
        {
            // ///////////
            // SI pulsamos el raton cuando estamos sobre el boton, generamos un nuevo muro
            if (Input.GetMouseButtonDown(0))
            {
                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
                {
                    // Al generar el nuevo muro, nos vamos hasta el, con lo que hacemos un paso de navegación. Esto hay que reflejarlo en el sistema de migas de pan
                    // Generamos el evi de referencia para la miga de pan MAFG 2022-10-12
                    // Primero tenemos que seleccionar el gameobjet del muro en el que estamos
                    //GameObject muro_actual = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo;
                    //ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(muro_actual.transform.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera);
                    // OJOO esto no hace falta aqui, porque al crear el muro hace el transito y ya genera la miga de pan en el transito

                    // Generamos un nuevo muro
                    // El nuevo muro se genera en la rama que este activa y depende de los muros que tenga y de la posicion del muro desde el que se solicita
                    // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                    // Se genera en modo navegacion, ya que estamos en ese modo (en modo edicion, no se pueden generar muros desde este boton)
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaMuro(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo, ScriptDatosElemenItf.modoElemItf_navegacion);
                }  // Fin de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            }  //  Fin de - if (Input.GetMouseButtonDown(0))
        }  // Fin de - if (activo)
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        if (activo)   // El boton solo debe trabajar si esta activo
        {
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaMuro;
        }// Fin de - if (activo)
    } // Fin de - void OnTriggerEnter(Collider other) 
}
