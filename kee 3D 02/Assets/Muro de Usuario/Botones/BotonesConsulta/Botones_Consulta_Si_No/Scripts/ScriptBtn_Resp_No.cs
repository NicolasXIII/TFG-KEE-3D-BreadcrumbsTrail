using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control para el manejo de la opcion "NO" en el gameobject "Btn_Consulta_Si_No" para preguntas de SI o NO
/// Basicamente muueve al usuario mor el eje Z y se encarga de gestionar las colisiones con los
/// colaider correspondientes
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-06-26
/// Observaciones :
/// 		
/// </summary>

public class ScriptBtn_Resp_No : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    private Rigidbody rb;

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
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

//        Vector3 escala_este_Btn_Resp_xx = new Vector3(this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_x_y_BotonesInteriores,
//                                                        this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_x_y_BotonesInteriores,
//                                                        this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_z_BotonesInteriores);

        float excala_x_activo = this.transform.localScale.x * this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().incrementoEscala_x_y_Activacion;
        float excala_y_activo = this.transform.localScale.y * this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().incrementoEscala_x_y_Activacion;
        float excala_z_activo = this.transform.localScale.z;

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("desde ScriptBtn_Resp_No 42. Con excala_x_activo = " + excala_x_activo + " -con  this.transform.localScale.x = " + this.transform.localScale.x + " - con incrementoEscala_x_y_Activacion = " + this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().incrementoEscala_x_y_Activacion); }

        Vector3 escala_activo = new Vector3(excala_x_activo, excala_y_activo, excala_z_activo);

        this.transform.localScale = escala_activo;

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
                this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().respuestaUsuario = ScriptBtn_Consulta_Si_No.respUsr_No;
            } // FIn de - if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
        }
    } // Fin de - void OnTriggerEnter(Collider other) 


    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        Vector3 escala_este_Btn_Resp_xx = new Vector3(this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_x_y_BotonesInteriores, 
                                                        this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_x_y_BotonesInteriores, 
                                                        this.transform.parent.GetComponent<ScriptBtn_Consulta_Si_No>().escala_z_BotonesInteriores);
        this.transform.localScale = escala_este_Btn_Resp_xx;
    } // Fin de - void OnTriggerEnter(Collider other) 

}  // FIn de - public class Btn_Resp_No : MonoBehaviour
