using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control de los EVIs tipo 00
// Construye y gestiona los EVIs de tipo 00
// usuario, o elementos de los distintos telones
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-03-21
// Observaciones :
// 		Cada tipo de EVI se inicializa y se comporta 
//			1) Igual que todos los demas en su parte generica. PARA ESTO SE UTILIZA EL 
//				COMPONENTE "CtrlEviGeneral" ue es el que gestiona los datos y funciones generales de todos los EVIs
//			2) De modo especifico en lo que corresponde a su parte especifica

public class ScriptCtrlEviTipo_00 : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

	void Start ()
	{	
		// Asignamos objetos
		ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
		Usuario = GameObject.FindWithTag("Usuario");

        // Cargamos los datos del EVI, llamando a su DKS correspondiente
        if (!this.GetComponent<ScriptCtrlBaseDeEvi>().contenidoCumplimentado)
        {
            StartCoroutine("cumplimentaDatosEviTipo00");
        }

    } // FIn de - void Start ()

	void Update ()
	{

	} // Fin de - void LateUpdate ()

    IEnumerator cumplimentaDatosEviTipo00()
    {
        for (int i = 0; i < 5; i++)
        {
            float esperaSimulada = Random.Range(0f, 1.0f);
            yield return new WaitForSeconds(esperaSimulada);
        }

        gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialEviConDatos;
        this.GetComponent<ScriptCtrlBaseDeEvi>().contenidoCumplimentado = true; // En principio el evi esta sin contenido

    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()
}  // Fin de - public class ScriptCtrlEviTipo_00 : MonoBehaviour {
