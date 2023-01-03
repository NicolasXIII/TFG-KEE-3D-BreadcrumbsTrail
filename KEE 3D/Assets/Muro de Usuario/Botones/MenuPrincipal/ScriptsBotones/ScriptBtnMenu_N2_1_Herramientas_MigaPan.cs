using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  class ScriptBtnMenu_N2_1_Herramientas_MigaPan
/// Basicamente muueve al usuario mor el eje Z y se encarga de gestionar las colisiones con los
/// colaider correspondientes
/// Autor : 	Nicolas Merino (Miguel Angel Fernandez Graciani)
/// Fecha :	2022-xx-xx
/// Modificaciones :
///     - MAFG 2022-11-11 - He modificado la clasee, para hacerla equivalente a la de la tramoya, porque la de Nicolas me daba algun problemilla
///     - ** Nicolas 2022-11-11 *** La he modificado 
/// Observaciones :
/// 		Lo acompañan la camara, el foco de luz, el muro de usuario y los telones
/// </summary>


public class ScriptBtnMenu_N2_1_Herramientas_MigaPan : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject MuroUsuario;
    public GameObject panera;

    public GameObject Tramoya;

    private Rigidbody rb;

    // Variables para gestionar el periodo refractario del triger y la activacion de los puntoros que nos disparan
    // Nos sirve para gestionar los tics del raton. Solo con los trigers no funciona bien, creo que porque pierde tics al procesar por frames. Por eso hemos hecho esta martingala
    private bool enTriger; // Estamos dentro del triger (el ontrigerStay me daba problemas. Funciona mejor asi)
    private GameObject quienDispara;  // Para enviarlo al gestor del periodo refractario, que controla tambien si el puntero esta activado

    // Inicializacion
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        MuroUsuario = GameObject.FindWithTag("MuroUsuario");
//        panera = GameObject.FindWithTag("Panera");
        panera = MuroUsuario.GetComponent<ScriptCtrlMuroUsuario>().panera.gameObject;

        enTriger = false; // Inicializamos la variable
//        quienDispara = null;
        quienDispara = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Pulsacion del boton derecho del raton
        //      GetMouseButtonDown solo se puede usar desde el update
        //      GetMouseButtonDown(0) es el click izquierdo
        //     if (Input.GetMouseButtonDown(0))
        //     {
        // Bandera que permite mantener la escala_Activada del boton, cuando se clicka en el
        //         click = !click;

        //panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(click);
        //         ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(click);
        //     }


        if (enTriger)
        {
            // ///////////
            // SI pulsamos el raton cuando estamos sobre el boton, activamos la panera
            if (Input.GetMouseButtonDown(0))
            {
                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
                {
                    if (panera.activeSelf == false)
                    {
                        panera.gameObject.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(true);
                    }
                    else
                    {
                        panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(false);
                        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan_Activado;
                    }

                }  // Fin de if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            }  // Fin de - if (Input.GetMouseButtonDown(0))
        }  // FIn de - if (enTriger)
        else
        {
            if (panera.activeSelf == false)
            {
                this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
            }
        }
    }

    //private void OnMouseDown()
    //{
    //    // SI pulsamos el raton cuando estamos sobre el boton, activamos la panera
    //    if (enTriger)
    //    {
            
    //        if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
    //        {
    //            if (panera.activeSelf == false)
    //            {
    //                panera.gameObject.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(true);
    //            }
    //            else
    //            {
    //                panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(false);
    //                this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan_Activado;
    //            }
    //        } // Fin de if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
    //    } // FIn de - if (enTriger)
    //    else
    //    {
    //        if (panera.activeSelf == false)
    //        {
    //            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
    //        }
    //    }
    //}

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Cuando llega a el el puntero de usuario al boton, lo indicamos al usuario modificando su tamaño
    void OnTriggerEnter(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan_Activado;
        enTriger = true;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    void OnTriggerExit(Collider other)
    {
        if (enTriger)
        {
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
            enTriger = false;
        }
    } // Fin de - void OnTriggerEnter(Collider other)

}
