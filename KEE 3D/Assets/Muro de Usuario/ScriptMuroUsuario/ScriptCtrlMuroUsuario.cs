﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control del Muro de usuario
// Basicamente muueve el muro de usuario para que acompañe al usuario
// colaider correspondientes
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-03-15
// Observaciones :
// 		Lo acompañan la camara, el foco de luz y los telones
//		El objetivo es que la camara lo encuadre siempre de forma que los botones de control
//		de la interfaz esten siempre accesibles cuando se activen

public class ScriptCtrlMuroUsuario : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

    public GameObject panera;


    void Start ()
	{	
		// Asignamos objetos
		ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
		Usuario = GameObject.FindWithTag("Usuario");

		// ////////////////////////////////////////
		// Asignamos los parametros del Muro de usuario
		this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaMuroUsuario;
		// Para la orientacion espacial
		this.transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().giroOrientacionMuroUsuario;

		// ////////////////////////////////////////
		// Vamos generando los elementos del muro de interfaz
			// Primero el boton general "BtnMenu_N1_General"
		GameObject BtnMenu_N1_General = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_General);
		BtnMenu_N1_General.transform.SetParent(this.transform);
		BtnMenu_N1_General.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_General;
		BtnMenu_N1_General.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_General;
			// Para los hijos de "BtnMenu_N1_General"
				// El boton general "BtnMenu_N1_1_General_CtrlInterfaz"
		GameObject BtnMenu_N1_1_General_CtrlInterfaz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_1_Gereral_CtrlInterfaz);
		BtnMenu_N1_1_General_CtrlInterfaz.transform.SetParent(BtnMenu_N1_General.transform);
		BtnMenu_N1_1_General_CtrlInterfaz.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_1_Gereral_CtrlInterfaz;
		BtnMenu_N1_1_General_CtrlInterfaz.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_1_Gereral_CtrlInterfaz;
				// El boton general "BtnMenu_N1_1_Gereral_Escena"
		GameObject BtnMenu_N1_1_Gereral_Escena = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_1_Gereral_Escena);
		BtnMenu_N1_1_Gereral_Escena.transform.SetParent(BtnMenu_N1_General.transform);
		BtnMenu_N1_1_Gereral_Escena.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_1_Gereral_Escena;
		BtnMenu_N1_1_Gereral_Escena.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_1_Gereral_Escena;
				// El boton general "BtnMenu_N1_1_Gereral_Herramientas"
		GameObject BtnMenu_N1_1_Gereral_Herramientas = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_1_Gereral_Herramientas);
		BtnMenu_N1_1_Gereral_Herramientas.transform.SetParent(BtnMenu_N1_General.transform);
		BtnMenu_N1_1_Gereral_Herramientas.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_1_Gereral_Herramientas;
		BtnMenu_N1_1_Gereral_Herramientas.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_1_Gereral_Herramientas;

        // El boton general "BtnMenu_N1_1_Gereral_Herramientas"
        GameObject BtnMenu_N1_1_General_Salir = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_1_General_Salir);
        BtnMenu_N1_1_General_Salir.transform.SetParent(BtnMenu_N1_General.transform);
        BtnMenu_N1_1_General_Salir.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_1_General_Salir;
        BtnMenu_N1_1_General_Salir.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_1_General_Salir;

        // Los botones de nivel 2 
        // Los botones de nivel 2 "BtnMenu_N2_CtrlInterfaz"
        GameObject BtnMenu_N2_CtrlInterfaz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_CtrlInterfaz);
		BtnMenu_N2_CtrlInterfaz.transform.SetParent(this.transform);
		BtnMenu_N2_CtrlInterfaz.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_CtrlInterfaz;
		BtnMenu_N2_CtrlInterfaz.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_CtrlInterfaz;
					// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_cargar"
		GameObject BtnMenu_N2_1_CtrlInterfaz_cargar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_cargar);
		BtnMenu_N2_1_CtrlInterfaz_cargar.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_cargar.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_cargar;
		BtnMenu_N2_1_CtrlInterfaz_cargar.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_cargar;
					// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_grabar"
		GameObject BtnMenu_N2_1_CtrlInterfaz_grabar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_grabar);
		BtnMenu_N2_1_CtrlInterfaz_grabar.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_grabar.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_grabar;
		BtnMenu_N2_1_CtrlInterfaz_grabar.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_grabar;
					// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_audio"
		GameObject BtnMenu_N2_1_CtrlInterfaz_audio = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_audio);
		BtnMenu_N2_1_CtrlInterfaz_audio.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_audio.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_audio;
		BtnMenu_N2_1_CtrlInterfaz_audio.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_audio;
					// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_complejidad"
		GameObject BtnMenu_N2_1_CtrlInterfaz_complejidad = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_complejidad);
		BtnMenu_N2_1_CtrlInterfaz_complejidad.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_complejidad.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_complejidad;
		BtnMenu_N2_1_CtrlInterfaz_complejidad.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_complejidad;
					// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_idiomas"
		GameObject BtnMenu_N2_1_CtrlInterfaz_idiomas = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_idiomas);
		BtnMenu_N2_1_CtrlInterfaz_idiomas.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_idiomas.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_idiomas;
		BtnMenu_N2_1_CtrlInterfaz_idiomas.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_idiomas;
		// Boton de cargar interfaz "BtnMenu_N2_1_CtrlInterfaz_ambito"
		GameObject BtnMenu_N2_1_CtrlInterfaz_ambito = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_CtrlInterfaz_ambito);
		BtnMenu_N2_1_CtrlInterfaz_ambito.transform.SetParent(BtnMenu_N2_CtrlInterfaz.transform);
		BtnMenu_N2_1_CtrlInterfaz_ambito.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_CtrlInterfaz_ambito;
		BtnMenu_N2_1_CtrlInterfaz_ambito.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_CtrlInterfaz_ambito;

		// Los botones de nivel 2 "BtnMenu_N2_Escena"
		GameObject BtnMenu_N2_Escena = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_Escena);
		BtnMenu_N2_Escena.transform.SetParent(this.transform);
		BtnMenu_N2_Escena.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_Escena;
		BtnMenu_N2_Escena.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_Escena;
                // Boton de cargar interfaz "BtnMenu_N2_1_Escena_CreaMuro"
        GameObject BtnMenu_N2_1_Escena_CreaMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Escena_CreaMuro);
        BtnMenu_N2_1_Escena_CreaMuro.transform.SetParent(BtnMenu_N2_Escena.transform);
        BtnMenu_N2_1_Escena_CreaMuro.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Escena_CreaMuro;
        BtnMenu_N2_1_Escena_CreaMuro.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaMuro;
                // Boton de cargar interfaz "BtnMenu_N2_1_Escena_EliminaMuro"
        GameObject BtnMenu_N2_1_Escena_EliminaMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Escena_EliminaMuro);
        BtnMenu_N2_1_Escena_EliminaMuro.transform.SetParent(BtnMenu_N2_Escena.transform);
        BtnMenu_N2_1_Escena_EliminaMuro.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Escena_EliminaMuro;
        BtnMenu_N2_1_Escena_EliminaMuro.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_EliminaMuro;
                // Boton de cargar interfaz "BtnMenu_N2_1_Escena_GeneraRama"
        GameObject BtnMenu_N2_1_Escena_GeneraRama = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Escena_GeneraRama);
        BtnMenu_N2_1_Escena_GeneraRama.transform.SetParent(BtnMenu_N2_Escena.transform);
        BtnMenu_N2_1_Escena_GeneraRama.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Escena_GeneraRama;
        BtnMenu_N2_1_Escena_GeneraRama.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_GeneraRama;
                // Boton de cargar interfaz "BtnMenu_N2_1_Escena_CreaConcepto"
        GameObject BtnMenu_N2_1_Escena_CreaConcepto = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Escena_CreaConcepto);
        BtnMenu_N2_1_Escena_CreaConcepto.transform.SetParent(BtnMenu_N2_Escena.transform);
        BtnMenu_N2_1_Escena_CreaConcepto.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Escena_CreaConcepto;
        BtnMenu_N2_1_Escena_CreaConcepto.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Escena_CreaConcepto;


        // Los botones de nivel 2 "BtnMenu_N2_Herramientas"
        GameObject BtnMenu_N2_Herramientas = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_Herramientas);
		BtnMenu_N2_Herramientas.transform.SetParent(this.transform);
		BtnMenu_N2_Herramientas.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_Herramientas;
		BtnMenu_N2_Herramientas.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_Herramientas;
			// Boton de acceso al evi de busqueda "BtnMenu_N2_1_Herramientas_buscador"
		GameObject BtnMenu_N2_1_Herramientas_buscador = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Herramientas_buscador);
		BtnMenu_N2_1_Herramientas_buscador.transform.SetParent(BtnMenu_N2_Herramientas.transform);
		BtnMenu_N2_1_Herramientas_buscador.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Herramientas_buscador;
		BtnMenu_N2_1_Herramientas_buscador.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_buscador;
			// Boton de acceso a la mochila "BtnMenu_N2_1_Herramientas_mochila"
		GameObject BtnMenu_N2_1_Herramientas_mochila = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Herramientas_mochila);
		BtnMenu_N2_1_Herramientas_mochila.transform.SetParent(BtnMenu_N2_Herramientas.transform);
		BtnMenu_N2_1_Herramientas_mochila.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Herramientas_mochila;
		BtnMenu_N2_1_Herramientas_mochila.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_mochila;
			// Boton de acceso a los agentes "BtnMenu_N2_1_Herramientas_agentes"
		GameObject BtnMenu_N2_1_Herramientas_agentes = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Herramientas_agentes);
		BtnMenu_N2_1_Herramientas_agentes.transform.SetParent(BtnMenu_N2_Herramientas.transform);
		BtnMenu_N2_1_Herramientas_agentes.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Herramientas_agentes;
		BtnMenu_N2_1_Herramientas_agentes.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_agentes;

        // Autor    Nicolas Merino Ramirez(2022-10-27)
        GameObject BtnMenu_N2_1_Herramientas_MigaPan = Instantiate(this.ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N2_1_Herramientas_MigaPan);
        BtnMenu_N2_1_Herramientas_MigaPan.transform.SetParent(BtnMenu_N2_Herramientas.transform);
        BtnMenu_N2_1_Herramientas_MigaPan.transform.localPosition = this.ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N2_1_Herramientas_MigaPan;
        BtnMenu_N2_1_Herramientas_MigaPan.transform.localScale = this.ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
        BtnMenu_N2_1_Herramientas_MigaPan.transform.localRotation = this.ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rotacion_BtnMenu_N2_1_Herramientas_MigaPan;


        // /////////////////    panera  Nicolas Merino Ramirez(2022-10-27)/////////
        panera = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera);
        panera.transform.SetParent(this.transform);
        panera.transform.gameObject.SetActive(false);
        panera.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_panera;
        panera.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_panera;
        // Actualizo la panera de "ScriptDatosInterfaz" con los datos que he puesto a la panera actualmente
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera = panera;

        // /////////////////    almacen_MuroUsuario  /////////
        GameObject almacen_MuroUsuario = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().almacen_MuroUsuario);
        almacen_MuroUsuario.transform.SetParent(this.transform);
        almacen_MuroUsuario.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_almacen_MuroUsuario;
        almacen_MuroUsuario.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_almacen_MuroUsuario;
        // Asignamos ahora el almacen_MuroUsuario generado, como el "almacen_MuroUsuario" que se almacena en "ScriptDatosInterfaz". Antes lo que habia era un aset
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().almacen_MuroUsuario = almacen_MuroUsuario;

    } // FIn de - void Start ()

    void LateUpdate ()
	{
        // Colocamos el muro de usuario a la distancia adecuada, para que aparezca de forma adecuada en el monitor
        // los botones del muro deben aparecer en los bordes de la pantalla
        //		Debug.Log ("Desde lateUpdate, valor de este_distanciaMuroUsuario es :"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaMuroUsuario);

//        transform.localPosition = Usuario.transform.position + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaMuroUsuario;
        transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaMuroUsuario;
	} // Fin de - void LateUpdate ()


} // Fin de - public class ScriptCtrlMuroUsuario : MonoBehaviour {
