using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;


/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control de los botones de opciones de la caja de control del evi base
///         Controla los botones de OPCIONES del evi base
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-03-15  (desarrollo inicial y boton de eliminar)
/// Modificacion : 2021-03-14 (añadimos la funcionalidad al boton de expandir)
/// Observaciones :
///     - OJOOO este script es el mismo para todos los botones de opcionesN"_! (CLlonar, editar, eliminar, expandir, instanciar, editar_Grabar y editar_Sair)
///     pero evidentemente cada uno de ellos tiene sus variables para el. 
///     - El manejo de las activaciones de estos botones se controla desde "Script_BaseDeEvi_N2_Caja_opciones" que centraliza la configuracion de todos ellos
/// 		
/// </summary>

public class Script_BaseDeEvi_N2_1 : MonoBehaviour {

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject MuroUsuario;
    public Canvas CanvasGeneral;
    public TextMeshProUGUI Text_ConsultaUsr;

    public GameObject Btn_Consulta_Si_No;


    private Rigidbody rb;
    private GameObject objeto_Evi_Raiz;

    public GameObject Solicitudes; // Se hace referencia a este objeto, para que todas las solicitudes queden como hijos de el. 


    // Variables para gestionar el periodo refractario del triger y la activacion de los puntoros que nos disparan
    private bool enTriger; // Estamos dentro del triger (el ontrigerStay me daba problemas. Funciona mejor asi)
    private GameObject quienDispara;  // Para enviarlo al gestor del periodo refractario, que controla tambien si el puntero esta activado

    public bool activo;

    // Use this for initialization
    void Start () {

        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        MuroUsuario = GameObject.FindWithTag("MuroUsuario");
        Solicitudes = GameObject.FindWithTag("Solicitudes");

        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;
        Text_ConsultaUsr = CanvasGeneral.transform.GetChild(0).GetComponent<TextMeshProUGUI>();


        //  Btn_Caja_opciones = this.transform.parent.tamañoAltoBotonActivo;
        // this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;
        //this.transform.localScale = this.GetComponentInParent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;
        objeto_Evi_Raiz = transform.parent.parent.parent.gameObject;

        enTriger = false;
        quienDispara = null;

        activo = true;  // Se añade por si queremos desactivar especificamente alguna opcion. Pero no se usa. Esto se controla en "Script_BaseDeEvi_N2_Caja_opciones => configuraBotonesOpcionesEvi" 2021-07-17 MAFG

    }  // Fin de -  void Start () {

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Update is called once per frame
    /// Metodo : Update()
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-08-10
    /// Ultima modificacion :
    ///     - 2022-04-09 MAFG, para incluir la gestion de los botones de grabar y salir en los casos de evis sin techo en edicion, para modificacion
    ///                         de tipo de datos del sin techo y para solicitud de confirmacion a la seleccion de ficheros desde los sin techo en los
    ///                         que hay que seleccionar un fichero del sistema como contenido del sin techo 
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    /// 
    ///                                 // Podemos atender la orden de grabar desde distintos estados:
    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    //  1.) Si estamos EN UN SIN TECHO podemos tener varios casos:
    //      1.1.) Para el caso en el que nos estan pidiendo grabar en una situacion de CAMBIO DE TIPO DE DATO asignado al sin techo. Esto sucede cuando en edicion
    //          arrastramos un concepto hasta un sin techo; suponemos que el usuario con esta accion quiere definir el concepto que arrastra
    //          sbre el sin techo, como tipo de dato de este sin techo
    //      1.2.) Para el caso de la edicion del CONTENIDO "T" DEL SIN TECHO : Atendiendo al tipo de dato del sin techo. El sistema conoce y actua en 
    //              consecuencia con un conjunto de tipos de dato:
    //              - OJOOO, si el sistema no conoce el tipo de dato, lo tratara como un sin techo
    //              - Los tipos de dato y las actuaciones segun el tipo de dato son:
    //          1.2.1.) Para los tipos de dato de FICHERO:
    //              - Tipo de dato FICHERO generico "gen_tipoDeSinTecho_FicheroGenerico_Key"
    //              - Tipo de dato FICHERO de audio "gen_tipoDeSinTecho_FicheroAudio_Key"
    //              - Tipo de dato FICHERO de imagen "gen_tipoDeSinTecho_FicheroImagen_Key"
    //              - Tipo de dato FICHERO de video "gen_tipoDeSinTecho_FicheroVideo_Key"
    //          1.2.ultimo.) Para los tipos de dato Y PARA TODOS LOS NO CONOCIDOS:
    //              - Tipo de dato TEXTO PLANO "gen_tipoDeSinTechoTextoPlano_Key"
    //              - Tipo de dato TEXTO numero entero "gen_tipoDeSinTecho_NumeroEntero_Key"
    //              - Tipo de dato TEXTO url "gen_tipoDeSinTecho_Url_Key"
    //              - Tipo de dato ...
    //              Para todos estos tipos dedatos, la edicion la accion de "salr" o "grabar", se gestionan desde "Panel_Input_Text_SinTecho", que 
    //              es un subobjeto del evi destinado a este fin, por lo que no se realiza ningun tipo de gestion desde este boton


    // ///////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////
    //  2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 
    //      2.1.) primero preguntamos si realmente quiere enviar el concepto al DKS
    //              - Los pasos para esto son los siguientes
    //          2.1.1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
    //          2.1.2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
    //                 al evi que esta en edicion, para que este actue en consecuencia.
    //             Con esto queda por terminada la faena asociada al click sobre el boton de grabar. El resto lo resuelven el "Btn_Consulta_Si_No" 
    //             y el evi base

    // ////////////////////////////////////////////////////
    // 2.1.1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
        // Leg pasqamos el texto de la consulta
                                // ////////////////////////////////////////////////////
                                //  2.1.2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
                                // Con generar la solicitud, este boton "Btn_Consulta_Si_No", ha terminado, el mecanismo de la solicitud se encargara de realizar la operacion de "grabar"

                                // ////////////////////////////////////////////////////
                                // Generamos el elemento solicitud que gestionara la solicitud, su estado y evolucion
                                // INICIO SOLICITUD ///////////////////////////////////////////

                                // //////////////////////////////////////
                                // INTERVINIENTES EN LA SOLICITUD :
                                // 1.) este "Btn_BaseDeEvi_N3_1_op_Editar_Grabar"
                                //        - Genera el objeto "Btn_Consulta_Si_No"
                                //        - Genera la solicitud
                                //    Siguiente interviniente : "Btn_Consulta_Si_No"

                                // 2.) "Btn_Consulta_Si_No"
                                //        - Atiende la solicitud
                                //            pone la respuesta que sera "si" o "No" segun la respuesta de usuario
                                //            Elimina el boton de consulta
                                //    Siguiente interviniente. El evi que debe finalizar o no su edicion. Osea este evi "objeto_Evi_Raiz."

                                // 3.) Este evi "objeto_Evi_Raiz."
                                //        - Atiende la solicitud
                                //            Actua en consecuencia a la respuesta del usuario
                                //                - NO, continua con el evi en edicion sin hacer nada mas
                                //                - SI, 
                                //                    - cierra la consulta
                                //                    - Elimina la rama de edicion
                                //                    - Pone el evi en modo navegacion

                                // FIN SOLICITUD ///////////////////////////////////////////
                                // Si ha dicho que si, construimos el KDL de alta o modificacion para realizar la solicitud al DKS

                                // Construimos el KDL de solicitud de alta o edicion

                                // Enviamos la solicitud al DKS correspondiente

                                // Dejamos lista la solicitud para acturar en consecuencia cuando llegue la respuesta
    /// 
    /// </summary>
    void Update()
    {
        // Si el evi esta en un muro que no es el activo, debemos considerar que ya no estamos en el triger, puesto que los clikcs del raton que de el usuario seran
        // para lo que este viendo (el muro activo) por lo que hay que indicar que ya no estamos en el trigrer
        if (objeto_Evi_Raiz.transform.parent.gameObject != ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo)
        {
            this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().escalaBtn_BaseDeEvi_N2_1_Desactivado;
            enTriger = false;
        }

        // Si el boton esta activo debe aparecer, si no lo esta no
        if (activo)  // Si el boton no esta activo, lo quitamos
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else // Si el boton esta activo, lo ponemos
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        if (enTriger & activo)
        {
            // ///////////
            // SI pulsamos el raton cuando estamos sobre el boton, actuamos en consecuencia
            if (Input.GetMouseButtonDown(0))
            {
                // Controlamos el periodo de refreaccion para la pulsacion del raton
                if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
                {
                    //           GameObject nuevoEviBuscador = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().EviTipo_buscador_00);
                    //           nuevoEviBuscador.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);
                    string TipooBtnBaseDeEvi_N2_1 = this.tag;
                    if (DatosGlobal.niveDebug > 1000) { Debug.Log("Lo que tengo en TipooBtnBaseDeEvi_N2_1 es :" + TipooBtnBaseDeEvi_N2_1); }

                    // Obtenemos los datos por si hacen falta para la operacion a realizar
                    string clon_key = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().key;
                    string clon_host = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().host;                         // host : es el host del concepto (H en KDL - esta en I en KDL)
                    string clon_cualificador = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().cualificador;                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)
                    string clon_ordinalConf = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().ordinal;
                    string clon_ultiModConfEnString = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().fechUltMod;
                    DateTime clon_ultiModConf = new DateTime(0); // Habria que convertir la ultima modificacion en (DateTime)ultiModConfEnString;
                    GameObject clon_elemDestino = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo;

                    XmlDocument este_domPropio = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().domPropio;
                    XmlNode este_nodoEnlace = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi;
                    XmlNamespaceManager este_manejadorEspNomb = new XmlNamespaceManager(objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().domPropio.NameTable);
                    este_manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);


                    switch (TipooBtnBaseDeEvi_N2_1)
                    {
                        case "Btn_EliminareEvi_N2_1_op_Eliminar":
                            eliminaEsteEvi();
                            // Habria tambien que eliminar el EVI del DAUS PENDIENTE
                            break;
                        case "Btn_BaseDeEvi_N2_1_op_Clonar":
                            // Si pulsamos el raton cuando estamos sobre el boton de clonar, Segun el modo y el subtipo del evi ;
                            // 1.CLonar) Tanto en modo edicion como en modo navegacion:
                            //      1.1.) Si es un evi de referencia fractal : Generamos un nuevo evi de referencia fractal con el mismo concepto
                            //      1.2.) Si es un evi de instancia fractal : Generamos una evi de instancia igual a este (instancia al mismo concepto, con la misma descripcion)
                            //      1.3.) Si es un evi Sin Techo fractal : Generamos una evi sin techo igual a este (mismo tipo de datos y mismo contenido)

                            if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                            {
                                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(clon_key, clon_host, clon_cualificador, clon_ordinalConf, clon_ultiModConf, clon_elemDestino);
                            }
                            else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
                            {
                                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalInst(clon_elemDestino, este_domPropio, este_manejadorEspNomb, este_nodoEnlace);
                            }
                            else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
                            {
                                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviSinTecho(clon_elemDestino, este_domPropio, este_manejadorEspNomb, este_nodoEnlace);
                            }
                            else
                            {
                                if (DatosGlobal.niveDebug > 100)
                                {
                                    Debug.Log("Error en Script_BaseDeEvi_N2_1 => Update. En clonar, el subtipo del elemento de interfaz es desconocido");
                                }
                            }  // FIn de -  if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)

                            break;
                        case "Btn_BaseDeEvi_N2_1_op_Instanciar":
                            // Si pulsamos el raton cuando estamos sobre el boton de instanciar, Segun el modo y el subtipo del evi ;
                            // 1.Instanciar) Si esta en modo edicion :
                            //      1.1.) Si es un evi de referencia fractal : Este evi de referencia a este concepto, se convierte en una istancia que instancia a este concepto
                            //                                                 cuya descripcion es el la descripcion del concepto original ***PENDIENTE
                            //      1.2.) Si es un evi de instancia fractal : Generamos un nuevo evi de referencia fractal con el concepto al que instancia (si se quiere hacer 
                            //                                                  una instancia podra convertirse este en instancia)
                            //      1.3.) Si es un evi Sin Techo fractal : Generamos una evi sin techo con mismo tipo de datos pero vacio de contenido  ***PENDIENTE
                            // 2.Instanciar) Si esta en modo navegacion
                            //      2.1.) Si es un evi de referencia fractal :No se puede instanciar (solo se instancia un concepto para cuando estamos editando)
                            //      2.2.) Si es un evi de instancia fractal : Generamos un nuevo evi de referencia fractal con el concepto al que instancia
                            //      2.3.) Si es un evi Sin Techo fractal :  No se puede instanciar (solo se instancia un concepto para cuando estamos editando)

                            // 1.Instanciar) Si esta en modo edicion :
                            if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
                            {
                                if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                                {
                                    // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().transformaAInstanciaEviFractalRef(objeto_Evi_Raiz);
                                }
                                else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
                                {
                                    // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(clon_key, clon_host, clon_cualificador, clon_ordinalConf, clon_ultiModConf, clon_elemDestino);
                                }
                                else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
                                {
                                    // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().instanciaEviSinTecho(clon_elemDestino, este_domPropio, este_manejadorEspNomb, este_nodoEnlace);
                                }
                                else
                                {
                                    if (DatosGlobal.niveDebug > 100)
                                    {
                                        Debug.Log("Error en Script_BaseDeEvi_N2_1 => Update. En clonar, el subtipo del elemento de interfaz es desconocido");
                                    }
                                }  // FIn de -  if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                            }
                            // 2.Instanciar) Si esta en modo navegacion (mas bien si no estamos en edicion)
                            else
                            {
                                if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                                {
                                    if (DatosGlobal.niveDebug > 100)
                                    {
                                        Debug.Log("Error en Script_BaseDeEvi_N2_1 => Update. En instanciar, un evi de referencia fractal, no se puede instanciar en navegacion");
                                    }
                                }
                                else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
                                {
                                    // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(clon_key, clon_host, clon_cualificador, clon_ordinalConf, clon_ultiModConf, clon_elemDestino);
                                }
                                else if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
                                {
                                    if (DatosGlobal.niveDebug > 100)
                                    {
                                        Debug.Log("Error en Script_BaseDeEvi_N2_1 => Update. En instanciar, un evi Sin Techo, no se puede instanciar en navegacion");
                                    }
                                }
                                else
                                {
                                    if (DatosGlobal.niveDebug > 100)
                                    {
                                        Debug.Log("Error en Script_BaseDeEvi_N2_1 => Update. En clonar, el subtipo del elemento de interfaz es desconocido");
                                    }
                                }  // FIn de -  if (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
                            }  // FIn de - else - de - 

                            break;
                        case "Btn_BaseDeEvi_N2_1_op_Editar":
                            // Si pulsamos el raton cuando estamos sobre el boton de editar, ponemos el evi en modo edicion
                            // Por ahora solo generamos uno igual a falta de programar la edicion (PENDIENTE MAFG 2021-05-26) 

                            transform.parent.parent.parent.GetComponent<SctEditaEvi>().botonEditaEvi();

                            break;
                        case "Btn_BaseDeEvi_N3_1_op_Editar_Grabar":
                            if (DatosGlobal.niveDebug > 1000) { Debug.Log("Entramos en la solicitud de grabar un evi en edicion desde 158"); }

                            // Podemos atender la orden de grabar desde distintos estados:
                            // ///////////////////////////////////////////////////////////////
                            // ///////////////////////////////////////////////////////////////
                            //  1.) Si estamos EN UN SIN TECHO podemos tener varios casos:
                            if ((objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &&
                                    (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &&
                                    (objeto_Evi_Raiz.gameObject.transform.parent.gameObject == ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo) &&
                                    (objeto_Evi_Raiz.gameObject.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion))
                            {

                                // voy por aqui *****

                                //      1.1.) Para el caso en el que nos estan pidiendo grabar en una situacion de CAMBIO DE TIPO DE DATO asignado al sin techo. Esto sucede cuando en edicion
                                //          arrastramos un concepto hasta un sin techo; suponemos que el usuario con esta accion quiere definir el concepto que arrastra
                                //          sbre el sin techo, como tipo de dato de este sin techo
                                //          - OJOOO esta accion esta relacionada con "public void controlaTriger(GameObject elOtro)", ya que es alli donde se enlazan el evi sin techo
                                //              y el que esta pendiente para el cambio de tipo de dato. Aqui tan solo se atiende a la respuesta del usuario mediante estos botones
                                //              actuando en consecuencia
                                //              - EN concreto esta es la respuesta al supuesto "1.2.1.1.1.)" de "public void controlaTriger(GameObject elOtro)"
                                //                      - 1.2.1.1.2.) Si la respuesta es SI, modificamos el tipo de dato del sin techo, haciendo que su tipo de dato sea el concepto 
                                //                                      que esta superpuesto y luego separamos los dos evis para que no queden superpuestos
                                //          La respuesta del usuario ha sigo GRABAR (osea asignar el nuevo tipo de dato al sin techo)
                                //          - 1.1.1.)   Asignamos al este evi sin techo como su tipo de dato el concepto que estaba asociado en espera de cambio de tipo. 
                                //          - 1.1.2.)   Apartamos el evi del concepto del evi sin techo, para que cada cual siga su curso. 
                                //          - 1.1.3.)   Asignamos los valores correspondientes a las variables de control asociadas


                                // 1.1.1.)   Asignamos al este evi sin techo como su tipo de dato el concepto que estaba asociado en espera de cambio de tipo. 

                                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().modifica_TipoDato_de_SInTecho(objeto_Evi_Raiz, objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho);

                                // 1.1.2.)   Apartamos el evi del concepto del evi sin techo, para que cada cual siga su curso. 
                                float distancia_a_mantener = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi * 2f * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;
                                Vector3 separacion_evis = new Vector3(distancia_a_mantener, distancia_a_mantener, 0f);
                                Vector3 nueva_posicion_tipo_dato = new Vector3();
                                nueva_posicion_tipo_dato = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho.transform.localPosition + separacion_evis;
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho.transform.localPosition = nueva_posicion_tipo_dato;

                                // 1.1.3.)   Asignamos los valores correspondientes a las variables de control asociadas
                                    // Primero ajustamos los valores de las variables
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().en_espera_resp_edicion = false;
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho = null;
                                // Ahora desactivamos los votones correspondientes a la consulta de grabar o cancelar

                                GameObject Btn_BaseDeEvi_N2_Caja_opciones = this.transform.parent.transform.gameObject;
                                GameObject Btn_BaseDeEvi_N3_1_op_Editar_Grabar = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(Btn_BaseDeEvi_N2_Caja_opciones, "Btn_BaseDeEvi_N3_1_op_Editar_Grabar");
                                GameObject Btn_BaseDeEvi_N3_1_op_Editar_Salir = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(Btn_BaseDeEvi_N2_Caja_opciones, "Btn_BaseDeEvi_N3_1_op_Editar_Salir");


                                Btn_BaseDeEvi_N3_1_op_Editar_Grabar.SetActive(false);
                                Btn_BaseDeEvi_N3_1_op_Editar_Salir.SetActive(false);
                                Btn_BaseDeEvi_N2_Caja_opciones.SetActive(false);



                                //      PENDIENTE MAFG (2022-04-10)
                                //      1.2.) Para el caso de la edicion del CONTENIDO "T" DEL SIN TECHO : Atendiendo al tipo de dato del sin techo. El sistema conoce y actua en 
                                //              consecuencia con un conjunto de tipos de dato:
                                //              - OJOOO, si el sistema no conoce el tipo de dato, lo tratara como un sin techo
                                //              - Los tipos de dato y las actuaciones segun el tipo de dato son:
                                //          1.2.1.) Para los tipos de dato de FICHERO:
                                //              - Tipo de dato FICHERO generico "gen_tipoDeSinTecho_FicheroGenerico_Key"
                                //              - Tipo de dato FICHERO de audio "gen_tipoDeSinTecho_FicheroAudio_Key"
                                //              - Tipo de dato FICHERO de imagen "gen_tipoDeSinTecho_FicheroImagen_Key"
                                //              - Tipo de dato FICHERO de video "gen_tipoDeSinTecho_FicheroVideo_Key"
                                //          1.2.ultimo.) Para los tipos de dato Y PARA TODOS LOS NO CONOCIDOS:
                                //              - Tipo de dato TEXTO PLANO "gen_tipoDeSinTechoTextoPlano_Key"
                                //              - Tipo de dato TEXTO numero entero "gen_tipoDeSinTecho_NumeroEntero_Key"
                                //              - Tipo de dato TEXTO url "gen_tipoDeSinTecho_Url_Key"
                                //              - Tipo de dato ...
                                //              Para todos estos tipos dedatos, la edicion la accion de "salr" o "grabar", se gestionan desde "Panel_Input_Text_SinTecho", que 
                                //              es un subobjeto del evi destinado a este fin, por lo que no se realiza ningun tipo de gestion desde este boton

                                // Indicamos que hemos salido del boton
                                this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionGrabar = false;

                            }  // Fin de - if ((objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) && ...
                            else  // 2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 
                            {
                                // ///////////////////////////////////////////////////////////////
                                // ///////////////////////////////////////////////////////////////
                                //  2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 
                                //      2.1.) primero preguntamos si realmente quiere enviar el concepto al DKS
                                //              - Los pasos para esto son los siguientes
                                //          2.1.1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
                                //          2.1.2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
                                //                 al evi que esta en edicion, para que este actue en consecuencia.
                                //             Con esto queda por terminada la faena asociada al click sobre el boton de grabar. El resto lo resuelven el "Btn_Consulta_Si_No" 
                                //             y el evi base

                                // ////////////////////////////////////////////////////
                                // 2.1.1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
                                GameObject Btn_Consulta_Si_No_grabar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Consulta_Si_No);
                                Btn_Consulta_Si_No_grabar.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                                Btn_Consulta_Si_No_grabar.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_Btn_Consulta_Si_No);
                                // Leg pasqamos el texto de la consulta
                                string textoConsulta_grabar = "Realmente quieres grabar el concepto en el DKS";
                                Btn_Consulta_Si_No_grabar.GetComponent<ScriptBtn_Consulta_Si_No>().preguntaAUsuario = textoConsulta_grabar;

                                // ////////////////////////////////////////////////////
                                //  2.1.2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
                                // Con generar la solicitud, este boton "Btn_Consulta_Si_No", ha terminado, el mecanismo de la solicitud se encargara de realizar la operacion de "grabar"

                                // ////////////////////////////////////////////////////
                                // Generamos el elemento solicitud que gestionara la solicitud, su estado y evolucion
                                // INICIO SOLICITUD ///////////////////////////////////////////

                                GameObject solicitud_grabar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Solicitud);
                                solicitud_grabar.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_solicitud);
                                solicitud_grabar.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No);
                                // idSolicitud;  Se ha puesto mediante ponSubTipoElementIntf
                                // hora_solicitud - Se pone en el awake de la solicitud
                                // hora_respuesta - Todabia sin valor
                                // hora_fin - Todabia sin valor
                                // tiempoDeVida - lo incrementa "ScriptGestorSolicitudes"
                                solicitud_grabar.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Grabar; // Estamos en el boton de grabar la edicion de un concepto que estamos editando
                                solicitud_grabar.GetComponent<ClassSolicitud>().subTipo_solicitud = solicitud_grabar.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();   // Usamos esta variable para hacer el tipo mas accesible
                                solicitud_grabar.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso;   // La solicitud ya esta en curso. EN ESTE ESTADO ALGUN INTERVINIENTE PUEDE REQUERIR ATENCION
                                                                                                                                      // XmlDocument datos_Dom_solicitud - No procede en este caso
                                                                                                                                      // solicitud_grabar.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add("¿?"); // La pregunta ya se pone en el boton.
                                                                                                                                      // XmlDocument respuesta_Dom_solicitud - No procede en este caso
                                                                                                                                      // List<string> respuesta_txt_solicitud - Se sabrá mas tarde. C0ntendra la respuesta del usuario que sera SI o NO

                                //   public List<ClassInterviniente> listaIntervinientes;  // Es una lista que contiene sus hijos de primer nivel

                                // //////////////////////////////////////
                                // INTERVINIENTES EN LA SOLICITUD :
                                // 1.) este "Btn_BaseDeEvi_N3_1_op_Editar_Grabar"
                                //        - Genera el objeto "Btn_Consulta_Si_No"
                                //        - Genera la solicitud
                                //    Siguiente interviniente : "Btn_Consulta_Si_No"

                                ClassInterviniente obj_Boton_de_grabar = new ClassInterviniente();
                                obj_Boton_de_grabar.Interviniente = this.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_Boton_de_grabar.ordinal_Interviniente = 0; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                 // obj_Boton_de_grabar_edicion.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_Boton_de_grabar.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;   // identifica el estado en el que se encuentra la solicitud
                                obj_Boton_de_grabar.hora_ini_intervencion = DateTime.Now;
                                solicitud_grabar.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_Boton_de_grabar); // Es una lista que contiene sus hijos de primer nivel

                                // 2.) "Btn_Consulta_Si_No"
                                //        - Atiende la solicitud
                                //            pone la respuesta que sera "si" o "No" segun la respuesta de usuario
                                //            Elimina el boton de consulta
                                //    Siguiente interviniente. El evi que debe finalizar o no su edicion. Osea este evi "objeto_Evi_Raiz."

                                ClassInterviniente obj_Btn_Consulta_Si_No_grabar = new ClassInterviniente();
                                obj_Btn_Consulta_Si_No_grabar.Interviniente = Btn_Consulta_Si_No_grabar.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_Btn_Consulta_Si_No_grabar.ordinal_Interviniente = 1; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                           // obj_Btn_Consulta_Si_No.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_Btn_Consulta_Si_No_grabar.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_requiereAtencion;   // identifica el estado en el que se encuentra la solicitud
                                solicitud_grabar.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_Btn_Consulta_Si_No_grabar);  // Es una lista que contiene sus hijos de primer nivel

                                // 3.) Este evi "objeto_Evi_Raiz."
                                //        - Atiende la solicitud
                                //            Actua en consecuencia a la respuesta del usuario
                                //                - NO, continua con el evi en edicion sin hacer nada mas
                                //                - SI, 
                                //                    - cierra la consulta
                                //                    - Elimina la rama de edicion
                                //                    - Pone el evi en modo navegacion

                                ClassInterviniente obj_objeto_Evi_Raiz_grabar = new ClassInterviniente();
                                obj_objeto_Evi_Raiz_grabar.Interviniente = objeto_Evi_Raiz.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_objeto_Evi_Raiz_grabar.ordinal_Interviniente = 2; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                        // obj_objeto_Evi_Raiz.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_objeto_Evi_Raiz_grabar.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_NoIniciada;   // identifica el estado en el que se encuentra la solicitud
                                solicitud_grabar.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_objeto_Evi_Raiz_grabar);  // Es una lista que contiene sus hijos de primer nivel

                                // FIN SOLICITUD ///////////////////////////////////////////
                                // Si ha dicho que si, construimos el KDL de alta o modificacion para realizar la solicitud al DKS

                                // Construimos el KDL de solicitud de alta o edicion

                                // Enviamos la solicitud al DKS correspondiente

                                // Dejamos lista la solicitud para acturar en consecuencia cuando llegue la respuesta

                            }  // Fin de - else  // 2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 

                            break;
                        case "Btn_BaseDeEvi_N3_1_op_Editar_Salir":
                            if (DatosGlobal.niveDebug > 1000)
                            {
                                Debug.Log("Entramos en la solicitud de salir de un evi en edicion desde 135");
                            }

                            // Podemos atender la orden de salir desde distintos estados:
                            // ///////////////////////////////////////////////////////////////
                            // ///////////////////////////////////////////////////////////////
                            //  1.) Si estamos EN UN SIN TECHO podemos tener varios casos:
                            if ((objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &&
                                    (objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00) &&
                                    (objeto_Evi_Raiz.gameObject.transform.parent.gameObject == ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo) &&
                                    (objeto_Evi_Raiz.gameObject.transform.parent.gameObject.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion))
                            {

                                //      1.1.) Para el caso en el que nos estan pidiendo cancelar una situacion de CAMBIO DE TIPO DE DATO NO asignado el concepto asociado como tipo de 
                                //          dato del sin techo. Esto sucede cuando en edicion arrastramos un concepto hasta un sin techo; suponemos que el usuario con esta accion quiere 
                                //          definir el concepto que arrastra sobre el sin techo, como tipo de dato de este sin techo. Pero ha decidido cancelar esta accion.
                                //          - OJOOO esta accion esta relacionada con "public void controlaTriger(GameObject elOtro)", ya que es alli donde se enlazan el evi sin techo
                                //              y el que esta pendiente para el cambio de tipo de dato. Aqui tan solo se atiende a la respuesta del usuario mediante estos botones
                                //              actuando en consecuencia
                                //              - EN concreto esta es la respuesta al supuesto "1.2.1.1.1.)" de "public void controlaTriger(GameObject elOtro)"
                                //                      - 1.2.1.1.1.) Si la respuesta es NO, separamos los dos evis sin cambio en ninguno de ellos
                                //          - 1.1.1.) Simplemente apartamos el evi del concepto del evi sin techo, sin modificar nada (sin cambiar el tipo de dato del sin techo) para que cada 
                                //              cual siga su curso. 
                                //          - 1.1.2.) Asignamos los valores correspondientes a las variables de control asociadas

                                //  1.1.1.) Simplemente apartamos el evi del concepto del evi sin techo, sin modificar nada (sin cambiar el tipo de dato del sin techo) para que cada 
                                float distancia_a_mantener = ScriptDatosInterfaz.SphereCollider_radio_BaseDeEvi * 2f * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaBaseDeEvi;
                                Vector3 separacion_evis = new Vector3(distancia_a_mantener, distancia_a_mantener, 0f);
                                Vector3 nueva_posicion_tipo_dato = new Vector3();
                                nueva_posicion_tipo_dato = objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho.transform.localPosition + separacion_evis;
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho.transform.localPosition = nueva_posicion_tipo_dato;


                                //  1.1.2.) Asignamos los valores correspondientes a las variables de control asociadas
                                    // Primero ajustamos los valores de las variables
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().en_espera_resp_edicion = false;
                                objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().evi_pendiente_cambio_tipo_dato_sinTecho = null;
                                    // Ahora desactivamos los votones correspondientes a la consulta de grabar o cancelar

                                GameObject Btn_BaseDeEvi_N3_1_op_Editar_Grabar = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(this.transform.parent.transform.gameObject, "Btn_BaseDeEvi_N3_1_op_Editar_Grabar");
                                GameObject Btn_BaseDeEvi_N3_1_op_Editar_Salir = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameHijoPorTag(this.transform.parent.transform.gameObject, "Btn_BaseDeEvi_N3_1_op_Editar_Salir");

                                Btn_BaseDeEvi_N3_1_op_Editar_Grabar.SetActive(false);  
                                Btn_BaseDeEvi_N3_1_op_Editar_Salir.SetActive(false); 
                                this.transform.parent.transform.gameObject.SetActive(false);

                                //      PENDIENTE MAFG (2022-04-10)
                                //      1.2.) Para el caso de la edicion del CONTENIDO "T" DEL SIN TECHO : Atendiendo al tipo de dato del sin techo. El sistema conoce y actua en 
                                //              consecuencia con un conjunto de tipos de dato:
                                //              - OJOOO, si el sistema no conoce el tipo de dato, lo tratara como un sin techo
                                //              - Los tipos de dato y las actuaciones segun el tipo de dato son:
                                //          1.2.1.) Para los tipos de dato de FICHERO:
                                //              - Tipo de dato FICHERO generico "gen_tipoDeSinTecho_FicheroGenerico_Key"
                                //              - Tipo de dato FICHERO de audio "gen_tipoDeSinTecho_FicheroAudio_Key"
                                //              - Tipo de dato FICHERO de imagen "gen_tipoDeSinTecho_FicheroImagen_Key"
                                //              - Tipo de dato FICHERO de video "gen_tipoDeSinTecho_FicheroVideo_Key"
                                //          1.2.ultimo.) Para los tipos de dato Y PARA TODOS LOS NO CONOCIDOS:
                                //              - Tipo de dato TEXTO PLANO "gen_tipoDeSinTechoTextoPlano_Key"
                                //              - Tipo de dato TEXTO numero entero "gen_tipoDeSinTecho_NumeroEntero_Key"
                                //              - Tipo de dato TEXTO url "gen_tipoDeSinTecho_Url_Key"
                                //              - Tipo de dato ...
                                //              Para todos estos tipos dedatos, la edicion la accion de "salr" o "grabar", se gestionan desde "Panel_Input_Text_SinTecho", que 
                                //              es un subobjeto del evi destinado a este fin, por lo que no se realiza ningun tipo de gestion desde este boton

                                // Indicamos que hemos salido del boton
                                this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionSalir = false;

                            }  // Fin de - if ((objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) && ...
                            else  // 2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 
                            {
                                // Terminamos con la edicion
                                // Primero preguntamos si realmente quiere abandonar la edicion (controlamos si ha salvado)
                                // Los pasos para esto son los siguientes
                                //      1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
                                //      2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
                                //              al evi que esta en edicion, para que este actue en consecuencia.
                                //      Con esto queda por terminada la faena asociada al click sobre el boton de eliminar. El resto lo resuelven el "Btn_Consulta_Si_No" 
                                //      y el evi base

                                // ////////////////////////////////////////////////////
                                // 1.) Generamos el objeto de consulta "Btn_Consulta_Si_No"
                                GameObject Btn_Consulta_Si_No_finEdicion = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Consulta_Si_No);
                                Btn_Consulta_Si_No_finEdicion.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                                Btn_Consulta_Si_No_finEdicion.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_Btn_Consulta_Si_No);
                                // Leg pasqamos el texto de la consulta
                                string textoConsulta_finEdicion = "Realmente quieres abandonar la edicion";
                                Btn_Consulta_Si_No_finEdicion.GetComponent<ScriptBtn_Consulta_Si_No>().preguntaAUsuario = textoConsulta_finEdicion;

                                // ////////////////////////////////////////////////////
                                //      2.) Enviamos a elelemento recien creado "Btn_Consulta_Si_No" una solicitud, para que envie la respuesta que emita el usuario
                                // Con generar la solicitud, este boton "Btn_Consulta_Si_No", ha terminado, el mecanismo de la solicitud se encargara de realizar la operacion de "salir"

                                // ////////////////////////////////////////////////////
                                // Generamos el elemento solicitud que gestionara la solicitud, su estado y evolucion
                                // INICIO SOLICITUD ///////////////////////////////////////////

                                GameObject solicitud_finEdicion = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Solicitud);
                                solicitud_finEdicion.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_solicitud);
                                solicitud_finEdicion.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoSolicitud_RespBtn_Consulta_Si_No);
                                // idSolicitud;  Se ha puesto mediante ponSubTipoElementIntf
                                // hora_solicitud - Se pone en el awake de la solicitud
                                // hora_respuesta - Todabia sin valor
                                // hora_fin - Todabia sin valor
                                // tiempoDeVida - lo incrementa "ScriptGestorSolicitudes"
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().codigoDeSolicitud = ClassSolicitud.CodigoSol_resp_sino_btn_Editar_Salir; // Estamos en el boton de salir de edicion de concepto
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().subTipo_solicitud = solicitud_finEdicion.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf();   // Usamos esta variable para hacer el tipo mas accesible
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().estado_solicitud = ClassSolicitud.estado_enProceso;   // La solicitud ya esta en curso. EN ESTE ESTADO ALGUN INTERVINIENTE PUEDE REQUERIR ATENCION
                                                                                                                                          // XmlDocument datos_Dom_solicitud - No procede en este caso
                                                                                                                                          // solicitud_grabar.GetComponent<ClassSolicitud>().datos_txt_solicitud.Add("¿?"); // La pregunta ya se pone en el boton.
                                                                                                                                          // XmlDocument respuesta_Dom_solicitud - No procede en este caso
                                                                                                                                          // List<string> respuesta_txt_solicitud - Se sabrá mas tarde. C0ntendra la respuesta del usuario que sera SI o NO

                                //   public List<ClassInterviniente> listaIntervinientes;  // Es una lista que contiene sus hijos de primer nivel

                                // //////////////////////////////////////
                                // INTERVINIENTES EN LA SOLICITUD :
                                // 1.) este "Btn_BaseDeEvi_N3_1_op_Editar_Salir"
                                //        - Genera el objeto "Btn_Consulta_Si_No"
                                //        - Genera la solicitud
                                //    Siguiente interviniente : "Btn_Consulta_Si_No"

                                ClassInterviniente obj_Boton_de_salir_edicion = new ClassInterviniente();
                                obj_Boton_de_salir_edicion.Interviniente = this.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_Boton_de_salir_edicion.ordinal_Interviniente = 0; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                        // obj_Boton_de_salir_edicion.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_Boton_de_salir_edicion.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_finalizada;   // identifica el estado en el que se encuentra la solicitud
                                obj_Boton_de_salir_edicion.hora_ini_intervencion = DateTime.Now;
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_Boton_de_salir_edicion); // Es una lista que contiene sus hijos de primer nivel

                                // 2.) "Btn_Consulta_Si_No"
                                //        - Atiende la solicitud
                                //            pone la respuesta que sera "si" o "No" segun la respuesta de usuario
                                //            Elimina el boton de consulta
                                //    Siguiente interviniente. El evi que debe finalizar o no su edicion. Osea este evi "objeto_Evi_Raiz."

                                ClassInterviniente obj_Btn_Consulta_Si_No_finEdicion = new ClassInterviniente();
                                obj_Btn_Consulta_Si_No_finEdicion.Interviniente = Btn_Consulta_Si_No_finEdicion.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_Btn_Consulta_Si_No_finEdicion.ordinal_Interviniente = 1; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                               // obj_Btn_Consulta_Si_No.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_Btn_Consulta_Si_No_finEdicion.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_requiereAtencion;   // identifica el estado en el que se encuentra la solicitud
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_Btn_Consulta_Si_No_finEdicion);  // Es una lista que contiene sus hijos de primer nivel

                                // 3.) Este evi "objeto_Evi_Raiz."
                                //        - Atiende la solicitud
                                //            Actua en consecuencia a la respuesta del usuario
                                //                - NO, continua con el evi en edicion sin hacer nada mas
                                //                - SI, 
                                //                    - cierra la consulta
                                //                    - Elimina la rama de edicion
                                //                    - Pone el evi en modo navegacion

                                ClassInterviniente obj_objeto_Evi_Raiz_finEdicion = new ClassInterviniente();
                                obj_objeto_Evi_Raiz_finEdicion.Interviniente = objeto_Evi_Raiz.gameObject; // Es el elemento de interfaz que realiza la interaccion.
                                obj_objeto_Evi_Raiz_finEdicion.ordinal_Interviniente = 2; ; // Es el elemento de interfaz que realiza la interaccion.
                                                                                            // obj_objeto_Evi_Raiz.tipo_Interviniente = ClassSolicitud.tipo_Interviniente_solicitante; // No se usa en este caso
                                obj_objeto_Evi_Raiz_finEdicion.estado_de_la_itervencion = ClassInterviniente.estado_itervencion_NoIniciada;   // identifica el estado en el que se encuentra la solicitud
                                solicitud_finEdicion.GetComponent<ClassSolicitud>().listaIntervinientes.Add(obj_objeto_Evi_Raiz_finEdicion);  // Es una lista que contiene sus hijos de primer nivel

                                // FIN SOLICITUD ///////////////////////////////////////////
                            }  // Fin de - else  // 2.) Para cuando estamos en EVIS QUE NO SON SIN TECHO EN EDICION, 

                            break;
                        case "Btn_BaseDeEvi_N2_1_op_Expandir":
                            transform.parent.parent.parent.GetComponent<SctExpandirEvi>().botonExpandeEvi();
                            break;
                        default:
                            if (DatosGlobal.niveDebug > 1000)
                            { Debug.Log("default en boton BaseDeEvi_N2_1"); }
                            break;
                    } // Fin de - switch (TipooBtnBaseDeEvi_N2_1)
                }  // Fin de if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(quienDispara))
            }  // Fin de - if (Input.GetMouseButtonDown(0))
        }  // FIn de - if (enTriger)
    }  // FIn de - void Update () {

        /// ///////////////////////////////////
        /// ///////////////////////////////////
        /// Vamos con los trigers
        /// <param name="other">Other.</param>

        // Cuando llega a el el puntero de usuario
        void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().escalaBtn_BaseDeEvi_N2_1_Activado;

        enTriger = true;
        quienDispara = other.gameObject;


        // Si estamos en edicion, los botones "Btn_BaseDeEvi_N3_1_op_Editar_Grabar" y "Btn_BaseDeEvi_N3_1_op_Editar_Salir" estaran activos, y aunque son hijos suyos, 
        // estos no estan dentro del evi "Btn_BaseDeEvi_N2_Caja_opciones". Por lo que para que la caja no se desactive mientras estamos en estos botones hemos generado
        // unas variables de estado que nos permiten hacer que el boton "Btn_BaseDeEvi_N2_Caja_opciones" no se desactive hasta que hayamos salido de estos botones
        if (this.transform.gameObject.name == "Btn_BaseDeEvi_N3_1_op_Editar_Grabar") { this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionGrabar = true;}
        else if (this.transform.gameObject.name == "Btn_BaseDeEvi_N3_1_op_Editar_Salir"){ this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionSalir = true;}

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().escalaBtn_BaseDeEvi_N2_1_Desactivado;

        enTriger = false;
        quienDispara = null;


        // Si estamos en edicion, los botones "Btn_BaseDeEvi_N3_1_op_Editar_Grabar" y "Btn_BaseDeEvi_N3_1_op_Editar_Salir" estaran activos, y aunque son hijos suyos, 
        // estos no estan dentro del evi "Btn_BaseDeEvi_N2_Caja_opciones". Por lo que para que la caja no se desactive mientras estamos en estos botones hemos generado
        // unas variables de estado que nos permiten hacer que el boton "Btn_BaseDeEvi_N2_Caja_opciones" se desactive cuando hayamos salido de estos botones
        if (this.transform.gameObject.name == "Btn_BaseDeEvi_N3_1_op_Editar_Grabar") { this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionGrabar = false; }
        else if (this.transform.gameObject.name == "Btn_BaseDeEvi_N3_1_op_Editar_Salir") { this.transform.parent.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_op_EnEdicionSalir = false; }
    } // Fin de - void OnTriggerEnter(Collider other) 

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  private void eliminaEsteEvi()
    /// Observaciones : Elimina el evi, quitandolo tambien de "ListaEvis" y del DAUS
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-01-30
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// </summary>
    private void eliminaEsteEvi()
    {
        // Borramos el EVI que vamos a destruir de "ListaEvis"
 //       ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaEvis.Remove(objeto_Evi_Raiz);

        objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();

        // Borramos el EVI del DAUS PENDIENTE (MAFG 2021-01-30)

        // Eliminamos el gameObject
//        Destroy(objeto_Evi_Raiz);

    } // Fin de - private void eliminaEsteEvi()

}  // Fin de - public class Script_BaseDeEvi_N2_1 : MonoBehaviour {
