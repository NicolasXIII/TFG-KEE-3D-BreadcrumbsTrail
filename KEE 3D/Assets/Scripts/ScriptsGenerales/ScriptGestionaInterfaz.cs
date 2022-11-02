using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


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
    /// Metodo : actualiza_Lista_idiomas_usuario
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-10-14 (solo prototipo)
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         
    /// Variables de salida :
    ///     No devuelve nada, solo genera el audio
    /// Observaciones:
    ///     - Este metodo consulta el muro en el que esta definida la lista de idiomas y actualiza el array correspondiente para hacer las solicitudes a DKSs y 
    ///     configurar la interfaz como proceda
    ///     - Al actualizar la lista de idiomas, deben volverse a cargar los conceptos de interfaz cuya informacion de ayuda a interfaz se carga del concepto
    ///     correspondiente en el DKS. Estos son :
    ///             - Concepto de "gen_listaDeIdiomasDeInterfaz" en DKs "http://www.ideando.net/klw/dks_kl"
    ///             - Concepto de muro, que se utiliza en los evis de referencia a elemento de interfaz entrre otros
    ///         OJOO. para hacerlo bien seria necesario localizar estos conceptos y recargar su ayuda a interfaz, cada vez que se modifique la lista de idiomas 
    ///         seleccionada por el usuario en la interfaz
    ///         
    /// </summary>
    public void actualiza_Lista_idiomas_usuario()
    {

        // public List<GameObject> Lista_idiomas_usuario;  Contiene una lista de idiomas priorizada que el usuario selecciona como sus idomas preferidos para el acceso a la información
        // Esta lista se definirá en un muro especifico dentro de la configuracion de la interfaz (PENDIENTE MAFG 2022-10-14)
        // El concepto asociado es "gen_listaDeIdiomasDeInterfaz" en el DKS "http://www.ideando.net/klw/dks_klw"
        // Cada uno de lo elementos del array debe ser un idioma concreto que instancie "gen_idioma" del DKS "http://www.ideando.net/klw/dks_Languajes"


        actualiza_conceptos_que_usa_la_interfaz(); //- Hay que llamarla, para que actualice el idioma a interfaz de los conceptos que utiliza la interfaz
                                                 //      para que el usuario los vea en el idioma correcto

    } // Fin de - public void actualiza_Lista_idiomas_usuario()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : actualiza_conceptos_que_usa_la_interfaz()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-10-14 (solo se actualizan los conceptos que se usan hasta la fecha)
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         
    /// Variables de salida :
    ///     No devuelve nada, solo genera el audio
    /// Observaciones:
    ///     - La interfaz de utiliza conceptos que le son necesaios, por ejemplo 
    ///             - para decir que un evi de referencia refiere a un muro 
    ///             - en otras muchas ocasiones
    ///         La inteerfaz comoce estos conceptos y sabe el DKS que los contiene, Podrian tenerse aqui grabados, pero hay dos problemas:
    ///             - Se perderian las actualizaciones
    ///             - El concepto puede ir acompañado de la ayuda a interfaz een diversos idiomas, y nos hara falta una u otra atendiendo a la configuracion de 
    ///                 idioma del usuario
    ///         Es por esto por lo que el lo que hacemos es recargar la descripcion de estos conceptos, al iniciar la interfaz y a cada cambio de la lista
    ///             de preferencias de idioma del usuario
    ///     - Los conceptos se cargan en gameobjetcs que estan referenciados en "ScriptDatosInterfaz" > "CONCEPTOS EXTERNOS QUE USA LA INTERFAZ" para que se pueda acceder a su informacion cuando sea necesario
    ///     - OJOO  hay que actualizarlos cada vez que se modifique lalista de preferencia de idiomas que define el usuario, para que estos muestren
    ///         su ayuda a interfaz en el idioma idoneo para el usuario
    ///     - La lista de conceptos que se cargan es la siguiente :
    ///         - "gen_muro" del DKS "http://www.ideando.net/klw/dks_klw" - se almacena en el gameobject "gen_muro" de "ScriptDatosInterfaz"
    ///         - ...
    ///         
    /// </summary>
    public void actualiza_conceptos_que_usa_la_interfaz()
    {
        //  para gen_muro;
        string key_DeEvi = ConceptosConocidos.gen_muro_Key;
        string host_DeEvi = ConceptosConocidos.gen_muro_host;
        string cualificador_DeEvi = ConceptosConocidos.gen_muro_cualifi;
        string ordinalConf_DeEvi = ConceptosConocidos.gen_muro_ordinal;
        DateTime ultiModConf_DeEvi = ConceptosConocidos.gen_muro_ultiMod;

        GameObject elemDestino_DeEvi = GetComponent<ScriptDatosInterfaz>().almacen_MuroUsuario;
        GameObject EviTipo_Fractal_01_DeEvi = GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);
        string nombre_concepto_a_alamcenar = "gen_muro";

        StartCoroutine(cuelga_concepto_en_almacen_MuroUsuario(EviTipo_Fractal_01_DeEvi, GetComponent<ScriptDatosInterfaz>().almacen_MuroUsuario, nombre_concepto_a_alamcenar));

        // /////////////////////////////////////////////////////////////////
        // Para otros conceptos que utilice la interfaz habra que hacer lo mismo

    } // Fin de - public void actualiza_conceptos_que_usa_la_interfaz()


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


    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   CORRUTINAS
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cuelga_concepto_en_almacen_MuroUsuario(), ahija un evi a un destino concreto, y lo asigna como objeto (normalmente en 
    ///                 algun objeto definido en "ScriptDatosInterfaz", para que sea accesible desde donde correponda
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-10-20
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     - GameObject EviDentroDelContenedor; es el evi contenido en el contenedor del evi recien generado (para las referencias habra que ir al evi base)
    ///     - GameObject destino; Es el gameobject, donde hay que ahijar el evi base de "EviDentroDelContenedor"
    ///     - string nombre_concepto_a_alamcenar ; este string indica a que gameobjet de e "ScriptDatosInterfaz" debe señalar a el evi base de "EviDentroDelContenedor"
    ///     - GameObject obj_en_ScriptDatosInterfaz; Es la variable se "ScriptDatosInterfaz" que debe señalar a el evi base de "EviDentroDelContenedor"
    /// 
    /// Variables de entrada :    
    /// Observaciones:
    ///     - Este iterador es necesario, porque cuando se crea el evi, su evi base no ha sido generado todabia y no es posible acceder a el 
    ///         en el mismo cuadro. Por eso es necesario esperar a los cuadros siguientes, para que el evi base este generado y pueda
    ///         ahijarse donde corresponda y pueda asignarse a la varible de "ScriptDatosInterfaz" que corresponda
    ///         
    /// </summary>
    IEnumerator cuelga_concepto_en_almacen_MuroUsuario(GameObject EviDentroDelContenedor, GameObject destino, string nombre_concepto_a_alamcenar)
    {
        yield return null; // Esperamos a que el evi base haya sido creado
        yield return null; // Esperamos a que el evi base haya sido creado

        // Localizamos el evi base que ya debe de existir correctamente
        GameObject evi_base_buscado = EviDentroDelContenedor.transform.parent.transform.parent.gameObject; // Accedemos al evi base
            // Lo asignamos al objeto correspondiente definido en "ScriptDatosInterfaz", para que sea accesible desde donde correponda
            // Esto que sigue he intentado hacerlo pasando en ves del parametro "string nombre_concepto_a_alamcenar"  un parametro " GameObject obj_en_ScriptDatosInterfaz"
            // en el que en la llamada a esta funcion hacia "obj_en_ScriptDatosInterfaz = GetComponent<ScriptDatosInterfaz>().gen_muro" y haciendo aqui
            // "obj_en_ScriptDatosInterfaz = evi_base_buscado;", pero no me funciona, creo que es porque se le pasa por valor y no por referencia
            // tengo que comprobarlo MAFG 2022-10-21
        if (nombre_concepto_a_alamcenar.Equals("gen_muro"))
        { GetComponent<ScriptDatosInterfaz>().gen_muro = evi_base_buscado; }
        else
        { }
        // Lo colocamos como hijo del destino donde debe ahijarse (noemalmente almacen_MuroUsuario)
        evi_base_buscado.transform.SetParent(destino.transform);



        //  GetComponent<ScriptDatosInterfaz>().almacen_MuroUsuario, GetComponent<ScriptDatosInterfaz>().gen_muro));
    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()



}  // FIn de - public class ScriptGestionaInterfaz : MonoBehaviour {
