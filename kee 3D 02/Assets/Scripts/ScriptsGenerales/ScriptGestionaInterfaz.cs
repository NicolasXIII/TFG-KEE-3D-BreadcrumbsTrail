using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptGestionaInterfaz : MonoBehaviour {

    public GameObject Usuario;

    // //////////////////////////////////////
    // Valores de estado del audio
    int volumen_general_audio = 50; // Podria ir de 0a 100

    bool activado_audio; // Activa o desactiva el audio total. - activado = true, desactivado = false

    bool activado_audio_alerta_usr; // Activa o desactiva el audio de alertas al usuario. - activado = true, desactivado = false

    bool activado_audio_intf; // Activa o desactiva el audio de interfaz. Ojo si esta activado, puede desactivarse o activarse por partes. - activado = true, desactivado = false
    bool activado_audio_intf_muro_usr; // Activa o desactiva el audio de interfaz. Ojo si esta activado, puede desactivarse o activarse por partes. - activado = true, desactivado = false
    bool activado_audio_intf_muro; // Activa o desactiva el audio de interfaz. Ojo si esta activado, puede desactivarse o activarse por partes. - activado = true, desactivado = false
    bool activado_audio_intf_tramoya; // Activa o desactiva el audio de interfaz. Ojo si esta activado, puede desactivarse o activarse por partes. - activado = true, desactivado = false
    bool activado_audio_intf_evi; // Activa o desactiva el audio de interfaz. Ojo si esta activado, puede desactivarse o activarse por partes. - activado = true, desactivado = false

    bool activado_audio_ambiente; // Activa o desactiva el audio de ambiente. - activado = true, desactivado = false

    /// QUIERO UN CODIGO DE NOTAS para orientar al usuario (2022-04-12 MAFG)
    // SI podemos activar audios en secuencia para situar al usuario, seria bueno poder silenciar tambien por secuencias
    // por ejemplo, si estamos en edicion, que todos los audios lleven delante un par de notas, para indicar que estamos en edicion

    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // SONIDOS /////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////

    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // Alertas a usuario

    public AudioClip alerta_usr_error;
    public AudioClip alerta_usr_solicito_respuesta_de_usuario;
    public AudioClip alerta_usr_alarma_de_atencion;

    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    //   SONIDOS DE INTERFAZ

    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // Efectos de interfaz 

    /// OJO Seria conveniente hacer algo distinto en edicion para diferenciarlo de la navegacion
    /// QUIERO UN CODIGO DE NOTAS para orientar al usuario (2022-04-12 MAFG)

    // //////////////////////////////////////////////////////////////////
    // Efectos GENERALES

    public AudioClip intf_entrada;
    public AudioClip intf_salida;

    public AudioClip intf_sintonia_ambiente;

    // //////////////////////////////////////////////////////////////////
    // Efectos de MURO DE USUARIO

    public AudioClip muro_usr_confirmar_salir;
    public AudioClip muro_usr_ctrl_intf;
    public AudioClip muro_usr_ctrl_intf_graabar;
    public AudioClip muro_usr_ctrl_intf_salvar;
    public AudioClip muro_usr_ctrl_intf_audio;
    public AudioClip muro_usr_ctrl_intf_complejidad;
    public AudioClip muro_usr_ctrl_muros;
    public AudioClip muro_usr_ctrl_muros_generar_muro;
    public AudioClip muro_usr_ctrl_muros_eliminar_muro;
    public AudioClip muro_usr_ctrl_muros_generar_concepto;
    public AudioClip muro_usr_ctrl_muros_generar_rama;
    public AudioClip muro_usr_herramientas;
    public AudioClip muro_usr_herramientas_buscador;
    public AudioClip muro_usr_herramientas_mochila;
    public AudioClip muro_usr_herramientas_agentes;


    // //////////////////////////////////////////////////////////////////
    // Efectos de MUROS DE TRABAJO

    public AudioClip muro_transito_entre_muros;

    public AudioClip muro_non_plus_ultra;

    public AudioClip muro_en_muro_raiz;
    public AudioClip muro_en_muro_configuracion;

    public AudioClip muro_creo_nuevo;
    public AudioClip muro_elimino;

    // //////////////////////////////////////////////////////////////////
    // Efectos de tramoya

    public AudioClip tramoya_abro;
    public AudioClip tramoya_cierro;

    public AudioClip tramoya_abro_rama;
    public AudioClip tramoya_cierro_rama;

    // //////////////////////////////////////////////////////////////////
    // Efectos de EVIs

    public AudioClip evi_cojo;
    public AudioClip evi_suelto;
    public AudioClip evi_arrastro_en_muro;
    public AudioClip evi_arrastro_en_tramoya;

    public AudioClip evi_abro_info;
    public AudioClip evi_cierro_info;
    public AudioClip evi_abro_contenido;
    public AudioClip evi_cierro_contenido;
    public AudioClip evi_abro_opciones;
    public AudioClip evi_cierro_opciones;

    public AudioClip evi_expando;
    public AudioClip evi_edito;
        public AudioClip evi_edicion_grabar;
        public AudioClip evi_edicion_cancelar;
    public AudioClip evi_clono;
    public AudioClip evi_convierto_en_instancia;
    public AudioClip evi_convierto_en_referencia;
    public AudioClip evi_elimino;

    public AudioClip evi_aparta_evi;

    public AudioClip evi_entra_en_tramoya;
    public AudioClip evi_sale_de_tramoya;
    public AudioClip evi_entra_en_tramoya_duplicando;
    public AudioClip evi_sale_de_tramoya_duplicando;

    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////
    //   FUENTES DE SONIDO
    public AudioSource altavoz_uauario;

    // Inicializamos la escena
    void Start()
    {
        // Asignamos objetos
        Usuario = GameObject.FindWithTag("Usuario");


        altavoz_uauario = Usuario.gameObject.GetComponent<AudioSource>();


        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Arrancamos con la seleccion de idioma principal
        // 
        // Primero el boton general "BtnMenu_N1_General"
        //       GameObject BtnMenu_N1_General = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnMenu_N1_General);
        //       BtnMenu_N1_General.transform.SetParent(this.transform);
        //       BtnMenu_N1_General.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicion_BtnMenu_N1_General;
        //       BtnMenu_N1_General.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N1_General;


        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Solicitamos la identificacion del usuario


        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Solicitamos el DAUS del usuario. Se carga desde   "ScriptGestionaDAUS"


        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Generamos la interfaz de usuario segun el DAUS

        /* *****************************************************
         * ********* Esto se hace en gestiona escena  **********************
         *    
     
        // Por ahora arrancamos con unos muros que jeneramos a mano para pueras (2020-03-xx)
        // Vamos generando y colocando los muros
        //        for (int i = 0; i < 5; i++)
        for (int i = 0; i < 0; i++)
            {
                GameObject otroMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro);
            otroMuro.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Muros.transform);
            Vector3 posicionMuro = new Vector3(0.0f, 0.0f, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros);
            otroMuro.transform.position = posicionMuro;
            // Si es el primer muro lo ponemos como muro activo
            if (i == 0) { ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = otroMuro.gameObject; }
 
            // Generamos unos pocos evis en el muro aleatoriamente
            for (int evi = 0; evi < 5; evi++)
            {
                GameObject otroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().EviTipo_00);
                otroEvi.transform.SetParent(otroMuro.transform);
            } // Fin de - for (int evi = 0; evi < 5; evi++) 
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros++;
        } // Fin de - for (int i = 0; i < 5; i++) 

        ******** Fin de   Esto se hace en gestiona escena    ************
        **************************************************** */

    } // Fin de - void Start () {

    // Update is called once per frame
    void Update () {
		
	}


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cargaDatosBaseDeEvi_Especifico
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string eviTxt : es la informacion de entrada. Debia de ser un DOM (PENDIENTE MAFG 2021-02-07)
    /// Variables de salida :
    ///     No devuelve nada, solo genera el audio
    /// Observaciones:
    /// </summary>
    public void haz_que_suene(AudioSource altavoz, AudioClip sonido)
    {
        altavoz.PlayOneShot(sonido, 1);
    }

}  // FIn de - public class ScriptGestionaInterfaz : MonoBehaviour {
