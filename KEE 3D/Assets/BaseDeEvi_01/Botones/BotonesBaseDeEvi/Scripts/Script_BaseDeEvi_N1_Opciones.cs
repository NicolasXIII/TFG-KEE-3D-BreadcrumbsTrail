using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BaseDeEvi_N1_Opciones : MonoBehaviour {

    public GameObject ctrlInterfaz;

    private Rigidbody rb;

    // Btones de opciones
    public GameObject Btn_Evi_Caja_opciones_local;

    public bool DentroDeBaseDeEvi_N1_Opciones = false;
    public bool DentroDeBtn_Evi_Caja_opciones = false;

    public bool DentroDeBtn_Evi_op_EnEdicionGrabar = false;
    public bool DentroDeBtn_Evi_op_EnEdicionSalir = false;

    private void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");

    }


    // Use this for initialization
    void Start()
    {
        // Botones de opciones
        //        GameObject Btn_Evi_Caja_opciones = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Caja_opciones);
        //        Btn_Evi_Caja_opciones.transform.SetParent(Btn_Evi_Opciones.transform);

        //        GameObject Btn_Evi_Caja_opciones = Instantiate(this.GetComponent<Script_BaseDeEvi_N1_Opciones>().Btn_Evi_Caja_opciones);

        Btn_Evi_Caja_opciones_local = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Caja_opciones;
        // En principio lo desactivamos, para que no aparezca hasta que se le seleccione
        Btn_Evi_Caja_opciones_local.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // Cuando el puntero ha salido del boton y de la caja de opciones, desactivamos la caja
        if (!DentroDeBtn_Evi_Caja_opciones & !DentroDeBaseDeEvi_N1_Opciones & !DentroDeBtn_Evi_op_EnEdicionGrabar & !DentroDeBtn_Evi_op_EnEdicionSalir)
        {
        Btn_Evi_Caja_opciones_local.SetActive(false);
        }
    }  // Fin de - void Update()

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        Btn_Evi_Caja_opciones_local.SetActive(true);
        DentroDeBaseDeEvi_N1_Opciones = true;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    // void OnTriggerStay(Collider other) EL BORTON GLOBAL DE OPCIONES NO RESPONDE AL CLIK DEL RQATON

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        DentroDeBaseDeEvi_N1_Opciones = false;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
