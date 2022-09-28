using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Herramientas_mochila : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject Tramoya;

    private Rigidbody rb;

    // Variables para gestionar el periodo refractario del triger y la activacion de los puntoros que nos disparan
    // Nos sirve para gestionar los tics del raton. Solo con los trigers no funciona bien, creo que porque pierde tics al procesar por frames. Por eso hemos hecho esta martingala
    private bool enTriger; // Estamos dentro del triger (el ontrigerStay me daba problemas. Funciona mejor asi)
    private GameObject quienDispara;  // Para enviarlo al gestor del periodo refractario, que controla tambien si el puntero esta activado


    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        Tramoya = GameObject.FindWithTag("Tramoya");

        enTriger = false; // Inicializamos la variable
        quienDispara = null;

    } // FIn de - void Start ()

    void Update()
    {

        if (enTriger)
        {
            // ///////////
            // SI pulsamos el raton cuando estamos sobre el boton, generamos un evi ce busqueda
            if (Input.GetMouseButtonDown(0))
            {
                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
                {
                    if (Tramoya.gameObject.GetComponent<ScriptCtrlTramoya>().Telon_mochila_01_activado == false)
                    {
                        Tramoya.gameObject.GetComponent<ScriptCtrlTramoya>().Telon_mochila_01_activado = true;
                    }
                    else
                    {
                        Tramoya.GetComponent<ScriptCtrlTramoya>().Telon_mochila_01_activado = false;
                        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_mochila;
                    }

                }  // Fin de if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            }  // Fin de - if (Input.GetMouseButtonDown(0))
        }  // FIn de - if (enTriger)
        else
        {
            if (Tramoya.gameObject.GetComponent<ScriptCtrlTramoya>().Telon_mochila_01_activado == false)
            {
                this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_mochila;
            }
        }

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
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_mochila_Activado;

        enTriger = true;
        quienDispara = other.gameObject;

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        if (Tramoya.GetComponent<ScriptCtrlTramoya>().Telon_mochila_01_activado == false)
        {
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_mochila;
        }
        

        enTriger = false;
        quienDispara = null;

    } // Fin de - void OnTriggerEnter(Collider other) 
}
