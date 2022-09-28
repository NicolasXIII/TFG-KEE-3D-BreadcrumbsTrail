using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  ScriptLibGestorEvis  : reune las funciones de generacion y control de los evis
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-03-19
/// Observaciones :
/// 		Se encarga de la gestion de los siguientes tipos de EVIs :
/// 		    - Evi FRACTAL DE REFERENCIA
/// 		    - Evi FRACTAL DE INSTANCIA
/// 		    - Evi de concepto SIN TECHO
///
/// </summary>
public class ScriptLibGestorEvis : MonoBehaviour {

    // definir que espacio del total se ocupa
    public GameObject Tramoya;

    // /////////////////////////////////////////////
    // TIPOS POSIBLES DE CONCERSION DE COORDENADAS
    public static string convCoord_muro_a_canvas = "convCoord_muro_a_canvas";
    public static string convCoord_raton_a_muro = "convCoord_raton_a_muro";
    public static string convCoord_raton_a_canvas = "convCoord_raton_a_canvas";

    // Use this for initialization
    void Awake() {

    }

    // Use this for initialization
    void Start()
    {
        // Asignamos objetos
        Tramoya = GameObject.FindWithTag("Tramoya");
    }

    // Update is called once per frame
    void Update () {
		
	}


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// expandeDescripcion : Obtiene los elementos de descripcion adociados a un concepto o un enlace, y los expande en el elemento destino (normalmente un muro) 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-19
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     Los datos de identificacion del concepto que contiene
    ///         - string key : Key (KDL:I => K ) del concepto que visualiza
    ///         - string host : host (KDL:I => H ) del concepto que visualiza
    ///         - string cualificador : Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
    ///         - string ordinalConf : Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
    ///         - DateTime ultiModConf : Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    ///         
    ///     Recordamos
    ///         => E (enlace)
    ///             => R (referencia)
    ///                 => I (identificador)
    ///                     => F (control de configuracion)
    ///                     => P (Ayuda a interfaz) (opcional)
    /// Variables de salida :
    /// Observaciones:
    ///     Vamos a generar un evi fractal de referencia a partir del key y el host de un concepto. Para hacerlo procedemos como sigue :
    ///         1) Generamos el evi donde ira el concepto
    ///         2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
    ///             y construir el evi fractal al completo
    ///                 
    /// </summary>
    public void generaEviFractalRef(string key, 
                                string host,
                                string cualificador,
                                string ordinalConf,
                                DateTime ultiModConf, 
                                GameObject elemDestino)
    {
        // //////////////////////////////////////////////////////////
        // 1) Generamos el evi donde ira el concepto
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde generaEviFractalRef"); }

        // Generamos el gameObject que albergara el concepto
        GameObject objetoContenedor = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);

        // Cargamos los datos de identificacin del concepto que va a contener, para que pueda proceder a cargarlo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().key = key; // Key (KDL:I => K ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().host = host; // host (KDL:I => H ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador = cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf = ordinalConf; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf = ultiModConf; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

        objetoContenedor.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalReferencia); // Indicamos que contiene una referencia

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_antesDeSolicitarKDL;

        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// expandeDescripcion : Obtiene los elementos de descripcion adociados a un concepto o un enlace, y los expande en el elemento destino (normalmente un muro) 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-19
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     objeto_Evi_Raiz : Es el EviBase que tenemos que transformar de referencia a una instancia del mismo concepto
    ///         
    ///     Recordamos
    ///         => E (enlace)
    ///             => R (referencia)
    ///                 => I (identificador)
    ///                     => F (control de configuracion)
    ///                     => P (Ayuda a interfaz) (opcional)
    /// Variables de salida :
    /// Observaciones:
    ///     - OJO, la instancia que generamos será una instancia con las caracteristicas siguientes:
    ///         - Es una instancia al concepto al que el evi origen referencia
    ///         - Se coloca en el DOM general, en el mismo punto donde se colocaba la referencia (en la misma rama de descripcion)
    ///         - Sustituye a la referencia (la referencia desaparece de la descripcion que se convierte en instancia)
    ///         - La descripción de la instancia, es la misma que la de la referencia que estamos convirtiendo en instancia
    ///     Para transformar la referencia en instancia realizamos los pasos siguientes :
    ///         1.) Cambiamos el subtipo del evi de "subTipoElemItf_evi_baseRefFractal" a "subTipoElemItf_evi_baseInstFractal" 
    ///                 - OJO, para hacerlo hay que pedir un nuevo identificador en como subtipo 
    ///                 - OJO, como sigue siendo un evi, no hay que modificar la lista de tipo, ni "idElementIntf", ni "tipoElementIntf", ni "idElementEnTipo" ya 
    ///                 que sigue siendo un evi
    ///         2.) Ajustamos el comportamiento de los botones de accion al de un evi de instancia en modo de edicion
    ///         2.) Modificamos la textura de los botones del evi base
    ///         3.) Tenemos que adecuar el dom de la referencia para convertirlo en un DOM de instancia y ademas integrarlo en el DOM correspondiente
    ///         en cuya descripcion se ingerta esta instancia
    ///             - OJO, EL camobbio de referencia a instancia, solo puede darse en un muro en edicion, por lo cual debe existir un nodo cabeza de edicion
    ///                 
    /// </summary>
    public void transformaAInstanciaEviFractalRef(GameObject objeto_Evi_Raiz)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde transformaAInstanciaEviFractalRef"); }

        //  1.) Cambiamos el subtipo del evi de "subTipoElemItf_evi_baseRefFractal" a "subTipoElemItf_evi_baseInstFractal" 
            // "objeto_Evi_Raiz" Ya tiene "ScriptDatosElemenItf =>idElementIntf"  puesto que se le dio cuando se genero (como evi de referencia fractal)
            // "objeto_Evi_Raiz" Ya tiene "ScriptDatosElemenItf =>tipoElementIntf"   puesto que sigue siendo un EVI, no hay que cambiarlo (Aunque cambie su subtipo de "subTipoElemItf_evi_baseRefFractal" a "subTipoElemItf_evi_baseInstFractal")
            // "objeto_Evi_Raiz" Ya tiene un "ScriptDatosElemenItf =>idElementEnTipo" puesto que sigue siendo un EVI, no hay que cambiarlo
            // SI HAY QUE MODIFICAR EL SUBTIPO de "subTipoElemItf_evi_baseRefFractal" a "subTipoElemItf_evi_baseInstFractal"
            //      - OJO como al cambiarlo, se genera un elemento que antes no estaba en el subtipo, hay que generarle un nuevo identificador de subtipo y asignarselo
        objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal);

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde transformaAInstanciaEviFractalRef con el objeto_Evi_Raiz. de nombre " + objeto_Evi_Raiz.name); }

        //  2.) Ajustamos el comportamiento de los botones de accion al de un evi de instancia en modo de edicion
            // Obtenemos el objeto del boton para poder ajustar los botones
        GameObject Btn_BaseDeEvi_N2_Caja_opciones = dameDescendientePorNombre(objeto_Evi_Raiz, "Btn_BaseDeEvi_N2_Caja_opciones");
            // Ajustamos la configuracion de los botones
        string este_modo = objeto_Evi_Raiz.GetComponent<ScriptDatosElemenItf>().modo;
        Btn_BaseDeEvi_N2_Caja_opciones.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().configuraBotonesOpcionesEvi(ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal, este_modo);
            // Ponemos las texturas adecuadas para un evi de instancia fractal en modo de edicion
        objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().ponBotonesDeInstanciaFractal();

    }  // Fin de - public void transformaAInstanciaEviFractalRef(GameObject objeto_Evi_Raiz)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : generaEviFractalInst : Genera un evi fractal de instancia y lo coloca en el elemento destino (normalmente un muro) 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-19
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    ///         - XmlDocument domPropio : Es el dom del concepto general que contiene a esta instancia en una de sus ramas
    ///         - XmlNamespaceManager manejadorEspNomb  : Manejador para el espacio de nombres
    ///         - XmlNode nodoEnlace : Es el nodo Enlace E del que cuelga la instancia "A", dentro del arbol DOM "domPropio" que vamos a visualizar mediante el evi que estamos generando
    /// Variables de salida :
    /// Observaciones:
    ///         - Si es UNA ISTANCIA : Ya tengo en el KDL local la descripcion (D) de la instancia, por lo que no tengo que ir a buscarla al DKS
    ///                             => E (enlace)
    ///                                 => A (instancia)
    ///                                     => I (identificador)
    ///                                     => F (control de configuracion)
    ///                                     => P  (Ayuda a interfaz) (opcional)
    ///                                     => D
    ///                                         => E (enlace)
    ///                                         => E (enlace)
    ///                                         .....
    ///                                         => E (enlace)
    ///         * Por ahora solo generamos un evi fractal, pero habria que poder generar listas, caminos, arboles, etc.. PENDIENTE (MAFG 2021-03-19) 
    /// </summary>
    public void generaEviFractalInst(GameObject elemDestino,
                                XmlDocument domPropio,
                                XmlNamespaceManager manejadorEspNomb,
                                XmlNode nodoEnlace)
    {
        // 2.2.1.) Si es UNA REFERENCIA : 
        // -  Tomo sus datos de identificacion
        // Vamos con el identificador

        // generamos el manejador de espacio de nombres
        //        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domPropio.NameTable);
        //        manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);

        // VOY POR AQUI 
 //       XmlNode E_Denodo_A = nodo_A; // Decimos que es A, pero lo que nos mandan es un E

//        if(nodo_A.Name == "A")
//        {
//            E_Denodo_A = nodo_A.ParentNode;
//        }

        XmlNode nodo_A = nodoEnlace.FirstChild; // Decimos que es A, pero lo que nos mandan es un E

        string[] identificadorInstancia_K_H_Q = GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(domPropio, nodo_A);
        // Asignamos a las variables de este game object
        string inst_key = identificadorInstancia_K_H_Q[0];                            // key: es el key del concepto (K en KDL - esta en I en KDL)
        string inst_host = identificadorInstancia_K_H_Q[1];                         // host : es el host del concepto (H en KDL - esta en I en KDL)
        string inst_cualificador = identificadorInstancia_K_H_Q[2];                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)

        string[] ctrlConfInstancia_O_M = GetComponent<ScriptLibConceptosXml>().dameCtrlConfDeNodo(domPropio, nodo_A);
        string inst_ordinal = ctrlConfInstancia_O_M[0];                         // inst_ordinal : es el ordinal de la instancia (O en KDL - esta en F en KDL)
        string inst_fechUltMod = ctrlConfInstancia_O_M[1];                 // fecha de ultima modificacion : es fecha de ultima modificacion de la instancia (M en KDL - esta en F en KDL)

        // VERMILISEGUNDOS ponerlos todos en long
        DateTime inst_fechUltMod_Milisec = new DateTime(0); // Esto esta mal, hay que poner en milisegundos cuando ajustemos
        //        int inst_fechUltMod_miisec = Int32.Parse(inst_fechUltMod);
        //        DateTime inst_fechUltMod_Date = new DateTime();
        //        inst_fechUltMod_Date. = inst_fechUltMod_miisec;
        // VERMILISEGUNDOS
        // long inst_fechUltMod_Milisec = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Desde ScriptLibGestorEvis => generaEviFractalInst. Generamos un evi fractal de instancia." +
            " - Con inst_key = " + inst_key +
            " - inst_host = " + inst_host +
            " - inst_cualificador = " + inst_cualificador +
            " - ordinalConf = " + inst_ordinal +
            " - ultiModConf = " + inst_fechUltMod +
            " - elemDestino = " + elemDestino.name +
            " - nodoEnlace = " + nodoEnlace.Name +
            " - nodoEnlace FirstChild = " + nodoEnlace.FirstChild.Name +
            " - nodoEnlace FirstChild FirstChild = " + nodoEnlace.FirstChild.FirstChild.Name +
            " - nodoEnlace FirstChild FirstChild FirstChild = " + nodoEnlace.FirstChild.FirstChild.FirstChild.Name +
            " - nodoEnlace FirstChild FirstChild FirstChild InnerText = " + nodoEnlace.FirstChild.FirstChild.FirstChild.InnerText);
        }

        GameObject objetoContenedor = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);

        // Cargamos los datos de identificacin del concepto que va a contener, para que pueda proceder a cargarlo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().key = inst_key; // Key (KDL:I => K ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().host = inst_host; // host (KDL:I => H ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador = inst_cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf = inst_ordinal; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf = inst_fechUltMod_Milisec; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().KdlConcepto = domPropio; // Ponemos el DOM del concepto generalen el evi fractal
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().nodo_E_eviFractal = nodoEnlace;  // Este es el nodo del enlace que visualiza este evi fractal (debe ser enlace de instancia, por eso mandamos el padre "E")
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScriptLibGestorEvis - generaEviFractalInst 174 con nodo_A = " + nodo_A.Name + " - nodoEnlace = " + nodoEnlace.Name + " - hijo = " + nodo_A.FirstChild.Name); }

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo
        objetoContenedor.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalInstancia); // Indicamos que contiene una instancia

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_recibidoKDL;


        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

    }  // Fin de - public void generaEviFractalInst(GameObject elemDestino,


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviSinTecho : Genera el evi sin techo asociado a un elemento Z de un DOM de KDL. OJO el sin techo pertenece a la descripcion de un concepto 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-19
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    ///                             => E (enlace)
    ///                                 => Z (sin techo)
    ///                                     => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
    ///                                        => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P (Ayuda a interfaz) (opcional)
    ///                                     => T (dato sin techo, string de texto)
    ///                                     
    ///         - XmlDocument domPropio : Es el dom del concepto general que contiene a este Sin Techo en una de sus ramas
    ///         - XmlNamespaceManager manejadorEspNomb  : Manejador para el espacio de nombres
    ///         - XmlNode nodo_E_A_o_Z) : Es el nodo "Z", dentro del arbol DOM "domPropio" que vamos a visualizar mediante el evi que estamos generando
    /// Variables de salida :
    /// Observaciones:
    ///                 
    ///                 2.3.)  Si es UN SIN TECHO : Ya tengo el el KDL local la referencia (R) al tipo de sintecho, asi como el dato en si (T), por lo que no tengo que ir a buscarla al DKS
    ///                         * Por ahora solo lo tratamos como texto plano, pero habria que poder tratarlo como url, fecha, numero, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                     - sin techo (obtenemos los datos asociados al elemento sin techo)
    ///                 
    /// </summary>
    public void generaEviSinTecho(GameObject elemDestino,
                                XmlDocument domPropio,
                                XmlNamespaceManager manejadorEspNomb,
                                XmlNode nodoEnlace)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde generaEviSinTecho"); }

        XmlNode nodo_Z = nodoEnlace.FirstChild; // Decimos que es A, pero lo que nos mandan es un E
        XmlNode nodo_R_DeTipoDeDato = nodo_Z.FirstChild; // Decimos que es A, pero lo que nos mandan es un E

        string[] identificadorTipoDato_K_H_Q = GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(domPropio, nodo_R_DeTipoDeDato);
        // Asignamos a las variables de este game object
        string tipoDato_key = identificadorTipoDato_K_H_Q[0];                            // key: es el key del concepto tipo de dato (K en KDL - esta en I en KDL)
        string tipoDato_host = identificadorTipoDato_K_H_Q[1];                         // host : es el host del concepto tipo de dato (H en KDL - esta en I en KDL)
        string tipoDato_cualificador = identificadorTipoDato_K_H_Q[2];                 // cualificador : indica la naturaleza del concepto tipo de dato (efimero,...) (Q en KDL - esta en I en KDL)
        string ordinalConf = null;
        DateTime ultiModConf = new DateTime(0);

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Desde ScriptLibGestorEvis => generaEviSinTecho. Generamos un evi fractal de sin techo." +
            " - Con tipoDato_key = " + tipoDato_key +
            " - tipoDato_host = " + tipoDato_host +
            " - tipoDato_cualificador = " + tipoDato_cualificador +
            " - ordinalConf = " + ordinalConf +
            " - ultiModConf = " + ultiModConf +
            " - elemDestino = " + elemDestino.name);
        }


        // Generamos el gameObject que albergara la instancia del concepto
        GameObject objetoContenedor = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);

        // Cargamos los datos de identificacin del concepto que va a contener, para que pueda proceder a cargarlo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().key = tipoDato_key; // Key (KDL:I => K ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().host = tipoDato_host; // host (KDL:I => H ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador = tipoDato_cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf = ordinalConf; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf = ultiModConf; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().KdlConcepto = domPropio; // Ponemos el DOM del concepto generalen el evi fractal
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().nodo_E_eviFractal = nodoEnlace;  // Este es el nodo del enlace que visualiza este evi fractal (debe ser enlace de instancia)

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo

        // Ya deberiamos tener la ayuda a interfaz del tipo de dato en el KDL del concepto que lo contien, por lo que no tenemos que esperar a pedirlo. Luego
        // ya podemos cargar los datos del evi base (PENDIENTE MAFG 2021-05-13, por ahora el DKS no envia los datos de ayuda a interfaz del tipo de dato, pero 
        // tenemos que hacer que los envie )

        // estadoEvi_domRecibido
        objetoContenedor.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho); // Indicamos que contiene un sin techo

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_recibidoKDL;

        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

        // Ponemos las dimensiones del evi Sin techo, que son distintas de los de referencia e instancia
//        objetoContenedor.transform.localScale = GetComponent<ScriptDatosInterfaz>().estaEscalaContenedorEviSinTecho;

    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviSinTecho_sinDOM : Genera el evi sin techo pero sin un DOM asociado. Se usara en la edicion para incluir elementos sin tecjho
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-01-01
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string tipoDato_key :
    ///         - string tipoDato_host :
    ///         - string tipoDato_cualificador :
    ///         - string ordinalConf :
    ///         - DateTime ultiModConf :
    ///         - GameObject elemDestino  : es el game object donde debe ingertarse el sin techo (normalmente un muro)
    /// Variables de salida :
    /// Observaciones:
    ///     - Recordamos la estructura del elemento Z :                 
    ///                             => E (enlace)
    ///                                 => Z (sin techo)
    ///                                     => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
    ///                                        => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P (Ayuda a interfaz) (opcional)
    ///                                     => T (dato sin techo, string de texto)
    ///     - 2.3.)  Si es UN SIN TECHO : Ya tengo el el KDL local la referencia (R) al tipo de sintecho, asi como el dato en si (T), por lo que no tengo que ir a buscarla al DKS
    ///                         * Por ahora solo lo tratamos como texto plano, pero habria que poder tratarlo como url, fecha, numero, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                     - sin techo (obtenemos los datos asociados al elemento sin techo)
    ///                 
    /// </summary>
    public void generaEviSinTecho_sinDOM(string tipoDato_key, string tipoDato_host, string tipoDato_cualificador, string ordinalConf, DateTime ultiModConf, GameObject elemDestino)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde generaEviSinTecho"); }

//        XmlNode nodo_Z = nodoEnlace.FirstChild; // Decimos que es A, pero lo que nos mandan es un E
//        XmlNode nodo_R_DeTipoDeDato = nodo_Z.FirstChild; // Decimos que es A, pero lo que nos mandan es un E

//        string[] identificadorTipoDato_K_H_Q = GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(domPropio, nodo_R_DeTipoDeDato);

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Desde ScriptLibGestorEvis => generaEviSinTecho_sinDOM. Generamos un evi fractal de sin techo." +
            " - Con tipoDato_key = " + tipoDato_key +
            " - tipoDato_host = " + tipoDato_host +
            " - tipoDato_cualificador = " + tipoDato_cualificador +
            " - ordinalConf = " + ordinalConf +
            " - ultiModConf = " + ultiModConf +
            " - elemDestino = " + elemDestino.name);
        }


        // Generamos el gameObject que albergara la instancia del concepto
        GameObject objetoContenedor = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);

        // Cargamos los datos de identificacin del concepto que va a contener, para que pueda proceder a cargarlo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().key = tipoDato_key; // Key (KDL:I => K ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().host = tipoDato_host; // host (KDL:I => H ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador = tipoDato_cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf = ordinalConf; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf = ultiModConf; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

//        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().KdlConcepto = domPropio; // Ponemos el DOM del concepto generalen el evi fractal
//        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().nodo_E_eviFractal = nodoEnlace;  // Este es el nodo del enlace que visualiza este evi fractal (debe ser enlace de instancia)

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo

        // Ya deberiamos tener la ayuda a interfaz del tipo de dato en el KDL del concepto que lo contien, por lo que no tenemos que esperar a pedirlo. Luego
        // ya podemos cargar los datos del evi base (PENDIENTE MAFG 2021-05-13, por ahora el DKS no envia los datos de ayuda a interfaz del tipo de dato, pero 
        // tenemos que hacer que los envie )

        // estadoEvi_domRecibido
        objetoContenedor.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho); // Indicamos que contiene un sin techo

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_listo;

        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

        // Ponemos las dimensiones del evi Sin techo, que son distintas de los de referencia e instancia
        //        objetoContenedor.transform.localScale = GetComponent<ScriptDatosInterfaz>().estaEscalaContenedorEviSinTecho;

    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// instanciaEviSinTecho : instancia un evi sin techo. Esto es genera un nuevo sin techo con el mismo tipo de datos, pero vacio de contenido 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-15
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    ///                             => E (enlace)
    ///                                 => Z (sin techo)
    ///                                     => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
    ///                                        => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P (Ayuda a interfaz) (opcional)
    ///                                     => T (dato sin techo, string de texto)
    ///                                     
    ///         - XmlDocument domPropio : Es el dom del concepto general que contiene a este Sin Techo en una de sus ramas
    ///         - XmlNamespaceManager manejadorEspNomb  : Manejador para el espacio de nombres
    ///         - XmlNode nodo_E_A_o_Z) : Es el nodo "Z", dentro del arbol DOM "domPropio" que vamos a visualizar mediante el evi que estamos generando
    /// Variables de salida :
    /// Observaciones:
    ///                 
    ///                 2.3.)  Si es UN SIN TECHO : Ya tengo el el KDL local la referencia (R) al tipo de sintecho, asi como el dato en si (T), por lo que no tengo que ir a buscarla al DKS
    ///                         * Por ahora solo lo tratamos como texto plano, pero habria que poder tratarlo como url, fecha, numero, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                     - sin techo (obtenemos los datos asociados al elemento sin techo)
    ///                     - Simplemente cojo la funcion "generaEviSinTecho"  y le quito el contenido del sin techo
    ///                 
    /// </summary>
    public void instanciaEviSinTecho(GameObject elemDestino,
                                XmlDocument domPropio,
                                XmlNamespaceManager manejadorEspNomb,
                                XmlNode nodoEnlace_origen)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde instanciaEviSinTecho"); }

        // Clonamos el nodo enlace que tenemos que instanciar, para tener otro nodo en el DOM que corresponda a el sin techo duplicado
        XmlNode nodoEnlace = nodoEnlace_origen.Clone();
        nodoEnlace_origen.ParentNode.AppendChild(nodoEnlace); // Lo ingertamos como hermano del nodo origen en el DOM general

        XmlNode nodo_Z = nodoEnlace.FirstChild; // Z es hijo de E
        XmlNode nodo_R_DeTipoDeDato = nodo_Z.FirstChild; //  R (referencia al tipo de dato) es el primer hijo de E
        XmlNode nodo_T_DeDato = nodo_Z.LastChild; //  T (que contiene el texto del dato) es el ultimo hijo de E
        nodo_T_DeDato.InnerXml = ""; // Vaciamos el contenido para eliminar los datos en el enlace generado

        string[] identificadorTipoDato_K_H_Q = GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(domPropio, nodo_R_DeTipoDeDato);
        // Asignamos a las variables de este game object
        string tipoDato_key = identificadorTipoDato_K_H_Q[0];                            // key: es el key del concepto tipo de dato (K en KDL - esta en I en KDL)
        string tipoDato_host = identificadorTipoDato_K_H_Q[1];                         // host : es el host del concepto tipo de dato (H en KDL - esta en I en KDL)
        string tipoDato_cualificador = identificadorTipoDato_K_H_Q[2];                 // cualificador : indica la naturaleza del concepto tipo de dato (efimero,...) (Q en KDL - esta en I en KDL)
        string ordinalConf = null;
        DateTime ultiModConf = new DateTime(0);

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("Desde ScriptLibGestorEvis => generaEviSinTecho. Generamos un evi fractal de sin techo." +
            " - Con tipoDato_key = " + tipoDato_key +
            " - tipoDato_host = " + tipoDato_host +
            " - tipoDato_cualificador = " + tipoDato_cualificador +
            " - ordinalConf = " + ordinalConf +
            " - ultiModConf = " + ultiModConf +
            " - elemDestino = " + elemDestino.name);
        }


        // Generamos el gameObject que albergara la instancia del concepto
        GameObject objetoContenedor = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RefFractal_01);

        // Cargamos los datos de identificacin del concepto que va a contener, para que pueda proceder a cargarlo
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().key = tipoDato_key; // Key (KDL:I => K ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().host = tipoDato_host; // host (KDL:I => H ) del concepto que visualiza
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().cualificador = tipoDato_cualificador; // Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ordinalConf = ordinalConf; // Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().ultiModConf = ultiModConf; // Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().KdlConcepto = domPropio; // Ponemos el DOM del concepto generalen el evi fractal
        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().nodo_E_eviFractal = nodoEnlace;  // Este es el nodo del enlace que visualiza este evi fractal (debe ser enlace de instancia)

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo

        // Ya deberiamos tener la ayuda a interfaz del tipo de dato en el KDL del concepto que lo contien, por lo que no tenemos que esperar a pedirlo. Luego
        // ya podemos cargar los datos del evi base (PENDIENTE MAFG 2021-05-13, por ahora el DKS no envia los datos de ayuda a interfaz del tipo de dato, pero 
        // tenemos que hacer que los envie )

        // estadoEvi_domRecibido
        objetoContenedor.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_contFractalSinTecho); // Indicamos que contiene un sin techo

        objetoContenedor.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_recibidoKDL;

        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

        // Ponemos las dimensiones del evi Sin techo, que son distintas de los de referencia e instancia
        //        objetoContenedor.transform.localScale = GetComponent<ScriptDatosInterfaz>().estaEscalaContenedorEviSinTecho;

    }  // Fin de - public void instanciaEviSinTechoDescripcion(GameObject elemDestino)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviRefElemen : genera un evi  "EviRefElemen" osea un evi de referencia a un elemento de interfaz, para utilizar en mochila, buscadores, etc..
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-05-30
    /// Ultima modificacion :
    /// Variables de entrada :
    ///     Los datos de identificacion del concepto que contiene
    ///         - string key : Key (KDL:I => K ) del concepto que visualiza
    ///         - string host : host (KDL:I => H ) del concepto que visualiza
    ///         - string cualificador : Cualificador (KDL:I => Q ) del concepto que visualiza, conceptos, efimeros, etc..
    ///         - string ordinalConf : Ordianl (KDL:F=> O) del concepto que visualiza. Ordinal en la secuencia de evolucion (modificaciones) sobre el concepto
    ///         - DateTime ultiModConf : Ultima modificacion (KDL:F=> M) del concepto que visualiza. Es un instante concreto del tiempo global
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    ///         
    ///     Recordamos
    ///         => E (enlace)
    ///             => R (referencia)
    ///                 => I (identificador)
    ///                     => F (control de configuracion)
    ///                     => P (Ayuda a interfaz) (opcional)
    /// Variables de salida :
    /// Observaciones:
    ///     Vamos a generar un evi fractal de referencia a partir del key y el host de un concepto. Para hacerlo procedemos como sigue :
    ///         1) Generamos el evi donde ira el concepto
    ///         2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
    ///             y construir el evi fractal al completo
    ///                 
    /// </summary>
    public void generaEviRefElemen(GameObject ElemenRef, GameObject elemDestino)
    {
        // //////////////////////////////////////////////////////////
        // 1) Generamos el evi EviRefElemen
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde generaEviRefElemen"); }

        // Generamos el gameObject que albergara el concepto
        GameObject EviRefElemen = Instantiate(GetComponent<ScriptDatosInterfaz>().EviRefElemen);

        EviRefElemen.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen); // Indicamos que contiene una referencia


        EviRefElemen.GetComponent<SctCtrlEviRefElemen>().ElemenRef = ElemenRef; // Indicamos que contiene una referencia

        EviRefElemen.GetComponent<SctCtrlEviRefElemen>().idElementIntf_elemenRef = ElemenRef.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf(); // Indicamos que contiene una referencia
        EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef = ElemenRef.GetComponent<ScriptDatosElemenItf>().dameTipoElementIntf(); // Indicamos que contiene una referencia
        EviRefElemen.GetComponent<SctCtrlEviRefElemen>().idElementEnTipo_elemenRef = ElemenRef.GetComponent<ScriptDatosElemenItf>().dameIdElementEnTipo(); // Indicamos que contiene una referencia
        EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef = ElemenRef.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf(); // Indicamos que contiene una referencia


        // Para poner los datos del evi base, hay que esperar a que este exista, por lo que hay que hacerlo mediante una corrutina
        StartCoroutine(esperaYponDatosDeEviBaseDeEviRefElemen(EviRefElemen, ElemenRef, elemDestino));

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log(" Desde generaEviRefElemen con id del ElemenRef = " + ElemenRef.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf() +
            " - y con id de este EviRefElemen " + EviRefElemen.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf() +
            " - y con elemento destino = " + elemDestino.name);
        }

        // /////////////////////////////////////////////////////////
        //  2) Marcamos el evi como pendiente de carga para que el mismo evi fractal que hemos generado se encargue de realizar la carga
        //     y construir el evi fractal al completo
        //        EviRefElemen.GetComponent<SctCtrlEviTipo_Fractal_01>().estOpContenedor = SctCtrlEviTipo_Fractal_01.estOpCont_antesDeSolicitarKDL;

        // //////////////////////////////////////
        // Del resto de la carga ya se encarga el evi fractal

    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// desarrollaFractal() - Metodo : Desarrolla los fractums del en el elemento contenedor que se solicite
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-10
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject EviBase : Es el evi base en el que reside el contenedor en el que vamos a desarrollar el fractal
    ///         - GameObject ObjetoCOntenedor : Es el objeto contenedor donde se va a desarrollar el fractal (normalmente un "EviTipo_RefFractal_01" si es el fractal de primer nivel de un 
    ///                                         "EviTipo_RefFractal_01" o un "Contenedor_FractumRef" si estamos desarrollando el fractal de una instancia de un nivel inferior
    ///         - XmlDocument KdlConcepto : Es el KDL del concepto total que contiene el evi base sobre el que se desarrolla este fractal. OJOOO aunque podemos estar desarrollando un enlace
    ///                                         esta variable lo que contiene es el KDL DE CONCEPTO 
    ///         - XmlNode  nodo_CoE : Es el nodo Enlace (E) o Concepto (C) del que vamos a generar el fractal de su descripcion
    /// Variables de salida :
    /// Observaciones: Los pasos para desarrollar el gractum son:
    ///         1.) Obtener la lista de enlaces de la descripcion que va a contener el fractum. Puede ser cualquiera de los niveles de
    ///         descripcion de un concepto o instancia de concepto
    ///             Voy obteniendo la lista de elementos que componen la descripcion.
    ///                 - La descripcion siempre es una lista de nodos "E", 
    ///                         => D (descripcion)
    ///                             => E (enlace)
    ///                             => E (enlace)
    ///                             .....
    ///                             => E (enlace)
    ///                 Cada uno de los cuales puede ser y se desarrolla como sigue :
    ///                     - referencias
    ///                     - instancias
    ///                     - sin techo
    ///                     
    ///         2.) Atendiendo al numero de enlaces (fractums) que debe contener el contenedor pertinente ("EviTipo_RefFractal_01" o "Contenedor_FractumRef"), calculamos
    ///         el tamaño y la colocacion que estos deben tener, para que quepan todos dentro del contenedor
    ///         3.) Vamos generando cada uno de los fractum, colocandolos en su posicion. Los fractum que sean instancias, iran generando los fractum que contengan de forma recursiva
    ///             3.1.) Primero calculamos el numero de enlaces que tiene la descripcion que tiene que visualizar el contenedor, para organizar el espacio y las posiciones
    ///             3.2.) Analizamos el tipo de enlace
    ///                             => E (enlace)
    ///                                 => A (Instancia) o R (Referencia) o Z(SinTecho)
    ///             3.3.) Segun el tipo de enlace desarrollamos el fractum correspondiente
    ///                 3.3.1.) Si es REFERENCIA 
    ///                         3.3.1.1.) generamos el fractum y ponemos su ayuda a interfaz
    ///                 3.3.2.) Si es INSTANCIA 
    ///                         3.3.2.1.)  generamos el fractum y ponemos su ayuda a interfaz
    ///                         3.3.2.2.)  Desarrollamos la descripcion de la instancia mediante sus fractums correspondientes
    ///                 3.3.3.) Si es SIN TECHO 
    ///                         3.3.3.1.)  generamos el fractum y ponemos el tipo de dato que contiene
    ///                         3.3.3.2.)  Ponemos el dato sin techo (texto) para que sea visible cuando proceda
    /// 
    /// </summary>
    public void desarrollaFractal(GameObject EviBase, GameObject ObjetoCOntenedor, XmlDocument KdlConcepto, XmlNode  nodo_CoE)
    {

        int potenciaFractal;  // El contenedor es un mosaico cuadrado de fractums. Sgun el numero de fractums que vayamos a introducir, necesitaremos un mosaico de nxn tal que (nxn> numero de fractums)
        float marcoFractum;  // Este parametro define el ancho del marco que un fractu deja alrrededor de el para no pegarse al de alado.  Este parametro multiplica al espacio total para 
                                 // definir que espacio del total se ocupa
        potenciaFractal = 2;  // queremos que el mosaico del contenedor tenga al menos 4 baldosas (2x2)
        marcoFractum = 0.9f; // Para dejar un margen entre un fractum y el de alado. Este parametro multiplica al espacio total para definir que espacio del total se ocupa


        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        //  1.) Voy obteniendo la lista de elementos que componen la descripcion.
        //      - La descripcion siempre es una lista de nodos "E", 
        //         => D (descripcion)
        //            => E (enlace)
        //            => E (enlace)
        //           .....
        //            => E (enlace)

        // Definmos la lista que contendra los elementos de descripcion del enlace
        // convertir el return en un array y consegui que pueda devolver uno, ninguno o varios sin problemas 
        //        XmlNodeList ListaNodos_E_en_D = new XmlNodeList();
        //        List<XmlNode> ListaNodos_E_en_D = new List<XmlNode>();

        XmlNodeList ListaNodos_E_en_D;

        // El KDL donde esta la descripcion del conceto esta en "GetComponent<ScriptCtrlBaseDeEvi>().domPropio"
        // Aqui NO analizamos el tipo de concepto, ponemos directamente un EVI subtipo fractal de referencia

        // generamos el manejador de espacio de nombres
        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(EviBase.GetComponent<ScriptCtrlBaseDeEvi>().domPropio.NameTable);
        manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);

        int idDelEvi = EviBase.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf();
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScriptLibGestorEvis - desarrollaFractal 358 con nodo name = " + nodo_CoE.Name + " - nombre hijo = " + nodo_CoE.FirstChild.Name + " - desde el evi = " + idDelEvi); }

        ListaNodos_E_en_D = GetComponent<ScriptLibConceptosXml>().damelistaDescripcionDeEnlace(nodo_CoE, manejadorEspNomb);


        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        //   2.) Atendiendo al numero de enlaces (fractums) que debe contener el contenedor pertinente ("EviTipo_RefFractal_01" o "Contenedor_FractumRef"), calculamos
        //       el tamaño y la colocacion que estos deben tener, para que quepan todos dentro del contenedor

        // El contenedor es un mosaico cuadrado de fractums. Sgun el numero de fractums que vayamos a introducir, necesitaremos un mosaico de nxn tal que (nxn> numero de fractums)
        potenciaFractal = calculaPotenciaDelFractal(ListaNodos_E_en_D.Count);  // n = potenciaFractal

        // El mosaico se genera en el ganeObject "Contenedor_FractumRef" y trabajaremos en la escala de este. Preparamos a continuacion los parametros para ir colocando 
        // cada fractum en una de las numBaldosas (numBaldosas = potenciaFractal x potenciaFractal) baldosas del contenedor.
        // despues iremos colocando cada fractun en una baldosas rellenandolas por filas

        int numBaldosas = potenciaFractal * potenciaFractal; // es el tamaño que debe ocupar una fila y una columna
        float escala_fractum = 1f / potenciaFractal; // es el tamaño que debe ocupar una fila y una columna

        float escala_x_fractum = escala_fractum * marcoFractum;  // Asi tendremos "potenciaFractal" baldosas por fila
        float escala_y_fractum = escala_fractum * marcoFractum;  // Asi tendremos "potenciaFractal" filas de baldosas
        float escala_z_fractum = 1.1f; // Un pelin mas ancho para que sobresalga por encima del padre

        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////
        // 3.) Vamos generando cada uno de los fractum, colocandolos en su posicion. Los fractum que sean instancias, iran generando los fractum que contengan de forma recursiva

        if (ListaNodos_E_en_D != null)
        {
            int numEnlace = 0;

            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibGestorEvis => desarrollaFractal 392 . La lista de descripcion NO es nula. Numero de enlaces = " + ListaNodos_E_en_D.Count); }
            foreach (XmlNode nodoEnlace in ListaNodos_E_en_D)
            {
                // ////////////////////////////////////////////////////
                // ////////////////////////////////////////////////////
                //    OPERACIONES INDEPENDIENTES DEL TIPO DE ENLACE
                // ////////////////////////////////////////////////////
                //    3.1.) Primero calculamos la escala y la posicion que tiene que ocupar el fractun, para organizar el espacio y las posiciones
                // Calculamos la baldosa (fila y columna) que debe ocupar en el mosaico del contenedor 
                int fila = numEnlace / potenciaFractal;
                int columna = numEnlace % potenciaFractal;

                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Desde ScriptLibGestorEvis => desarrollaFractal. Generando fractum numero " + numEnlace + " -  fila = " + fila + " -  columna = " + columna); }

                // Definimos las escalas
                float FractumRef_escala_x = escala_x_fractum;  // Es algo mas pequeño para que asome el borde del contenedor que lo contiene
                float FractumRef_escala_y = escala_y_fractum;  // Es algo mas pequeño para que asome el borde del contenedor que lo contiene
                float FractumRef_escala_z = escala_z_fractum;  // Es algo mas grueso en z para que sobresalga sobre su muro contenedor y podamos verlo
                Vector3 escala_FractumRef = new Vector3(FractumRef_escala_x, FractumRef_escala_y, FractumRef_escala_z);

                // Definimos la localizacion
                float FractumRef_localiz_x = (-1f / 2f) + ((escala_fractum / 2f) + escala_fractum * columna);
                float FractumRef_localiz_y = (1f / 2f) - ((escala_fractum / 2f) + escala_fractum * fila);
                float FractumRef_localiz_z = 0f;
                Vector3 localizacion_FractumRef = new Vector3(FractumRef_localiz_x, FractumRef_localiz_y, FractumRef_localiz_z);


                // ////////////////////////////////////////////////////
                //    3.2.) Analizamos el tipo de enlace
                //         => E (enlace)
                //            => A (Instancia) o R (Referencia) o Z(SinTecho)
                string tipoEnlace = GetComponent<ScriptLibConceptosXml>().dameTipoDeEnlace(KdlConcepto, nodoEnlace);

                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("Desde ScriptLibGestorEvis - desarrollaFractal 425 (todos) con nodo name = " + nodo_CoE.Name + " - nodoEnlace = " + nodoEnlace + " - desde el evi = " + idDelEvi); }


                // ////////////////////////////////////////////////////
                // ////////////////////////////////////////////////////
                //    OPERACIONES QUE SI DEPENDEN DEL TIPO DE ENLACE
                // ////////////////////////////////////////////////////
                //   3.3.) Segun el tipo de enlace desarrollamos el fractum correspondiente

                if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia))
                {
                    //   3.3.2.) Si es INSTANCIA 
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde ScriptLibGestorEvis - desarrollaFractal 437 (instancia) con nodo name = " + nodo_CoE.Name + " - nodoEnlace = " + nodoEnlace + " - desde el evi = " + idDelEvi); }
                    //     3.3.2.1.)  generamos el fractum y ponemos su ayuda a interfaz
                    //     3.3.2.2.)  Desarrollamos la descripcion de la instancia mediante sus fractums correspondientes

                    //  SOLO PARA REBAS LAS DESARROLLAMOS COMO REFERENCIAS
                    //  Generamos el fractum correspondiente al enlace 
                    GameObject EviFractumRef = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RF01_FractumRef);
                    // Colocamos el evi como hijo del contenedor que lo genera
                    EviFractumRef.transform.SetParent(ObjetoCOntenedor.transform);
                    // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                    // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_fractumInstancia);
                    // Ponemos la escala
                    EviFractumRef.transform.localScale = escala_FractumRef;
                    // Ponemos la posicion
                    EviFractumRef.transform.localPosition = localizacion_FractumRef;
                    // Definimos las rotaciones
                    EviFractumRef.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    // Indicamos el yipo de enlace
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace = tipoEnlace;

                    // Le indicamos al fractum que estamos generando, cual es el enlace KDL que debe visualizar y lo ponemos en el estado que le hara procesarlo
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().nodo_E_Fractum = nodoEnlace;
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().KdlConcepto = KdlConcepto; // El fractum tiene un enlace propio a KdlConcepto, ya que es mas facil que controlar la anidacion con prent.parent...
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("PPPPP en desarrollaFractal con KdlConcepto = " + KdlConcepto.ToString()); }



                    StartCoroutine(esperaYponEstado_nodeoEnlaceDisponible(EviFractumRef.gameObject));
                    //                   EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().estOpFractum = SctCtrlEviTipo_RF01FractumRef.estOpFractum_nodeoEnlaceDisponible;

                    //Los enlaces (E) que son instancias (A), contienen su descripcion (D) correspondiente, por lo que debemos generar el fractal de esta descripcion
                    // en el correspondiente contenedor del fractum "Contenedor_FractumRef". Para esto llamamos recursivamente a esta misma funcion "" en la que nos encontramos
                    // GameObject EviBase =>  el mismo
                    // GameObject ObjetoCOntenedor =>  es el contenedor del fractum que es el segundo hijo del fractum en el  que estamos
                    GameObject comtenedorDeEsteFractum = EviFractumRef.transform.GetChild(1).gameObject;
                    // XmlDocument KdlConcepto =>  el mismo
                    // XmlNode nodo_CoE =>  el el nodo enlace (E) que corresponde a la instancia

                    desarrollaFractal(EviBase, comtenedorDeEsteFractum, KdlConcepto, nodoEnlace);  // OOJOOO Llamamos recursivamente a esta misma funcion

                }  // Fin de - if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia))
                else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Referencia))
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde ScriptLibGestorEvis - desarrollaFractal 480 (referencia) con nodo name = " + nodo_CoE.Name + " - nodoEnlace = " + nodoEnlace + " - desde el evi = " + idDelEvi); }
                    //   3.3.1.) Si es REFERENCIA 
                    //     3.3.1.1.) generamos el fractum y ponemos su ayuda a interfaz
                    //  Generamos el fractum correspondiente al enlace 
                    GameObject EviFractumRef = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RF01_FractumRef);
                    // Colocamos el evi como hijo del contenedor que lo genera
                    EviFractumRef.transform.SetParent(ObjetoCOntenedor.transform);
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde SctCtrlEviTipo_Fractal_01 => desarrollaFractal. despues de generar la referencia  en el cuadro = " + GetComponent<ScriptDatosInterfaz>().numDeFrame); }
                    // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                    // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_fractumReferencia);
                    // Ponemos la escala
                    EviFractumRef.transform.localScale = escala_FractumRef;
                    // Ponemos la posicion
                    EviFractumRef.transform.localPosition = localizacion_FractumRef;
                    // Definimos las rotaciones
                    EviFractumRef.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    // Indicamos el yipo de enlace
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace = tipoEnlace;

                    // Le indicamos al fractum que estamos generando, cual es el enlace KDL que debe visualizar y lo ponemos en el estado que le hara procesarlo
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().nodo_E_Fractum = nodoEnlace;
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().KdlConcepto = KdlConcepto; // El fractum tiene un enlace propio a KdlConcepto, ya que es mas facil que controlar la anidacion con prent.parent...
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("RRRRR desde referencia en desarrollaFractal con KdlConcepto = " + KdlConcepto.ToString()); }


                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().estOpFractum = SctCtrlEviTipo_01Fractum.estOpFractum_nodeoEnlaceDisponible;

                }  // Fin de - else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Referencia))
                else if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_SinTecho))
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde ScriptLibGestorEvis - desarrollaFractal 514 (sin techo) con nodo name = " + nodo_CoE.Name + " - nodoEnlace = " + nodoEnlace + " - desde el evi = " + idDelEvi); }
                    //   3.3.3.) Si es SIN TECHO 
                    //     3.3.3.1.)  generamos el fractum y ponemos el tipo de dato que contiene
                    //  Generamos el fractum correspondiente al enlace 
                    GameObject EviFractumRef = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RF01_FractumRef);
                    // Colocamos el evi como hijo del contenedor que lo genera
                    EviFractumRef.transform.SetParent(ObjetoCOntenedor.transform);
                    // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                    // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
                    EviFractumRef.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_fractumSinTecho);
                    // Ponemos la escala
                    EviFractumRef.transform.localScale = escala_FractumRef;
                    // Ponemos la posicion
                    EviFractumRef.transform.localPosition = localizacion_FractumRef;
                    // Definimos las rotaciones
                    EviFractumRef.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    // Indicamos el yipo de enlace
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace = tipoEnlace;

                    // Le indicamos al fractum que estamos generando, cual es el enlace KDL que debe visualizar y lo ponemos en el estado que le hara procesarlo
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().nodo_E_Fractum = nodoEnlace;
                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().KdlConcepto = KdlConcepto; // El fractum tiene un enlace propio a KdlConcepto, ya que es mas facil que controlar la anidacion con prent.parent...

                    EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().estOpFractum = SctCtrlEviTipo_01Fractum.estOpFractum_nodeoEnlaceDisponible;
                    //     3.3.3.2.)  Ponemos el dato sin techo (texto) para que sea visible cuando proceda

                }  // Fin de - if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia))
                else  // SI no es uno de los anteriores tenemos un error
                {
                    if (DatosGlobal.niveDebug > 100)
                    { Debug.Log("Desde SctCtrlEviTipo_Fractal_01 => desarrollaFractal. Con valor de tipo  desconocido = " + tipoEnlace); }

                }  // Fin de - if (tipoEnlace == (ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Instancia))

                numEnlace++;  // Incrementamos el numero de enlace a colocar

            }  // Fin de - foreach (XmlNode nodoEnlace in ListaNodos_E_en_D)
        }  // Fin de - if (ListaNodos_E_en_D != null)
    }  // FIn de - public void desarrollaFractal()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// construyeCOntenidoSinTecho() - Metodo : El contenido de un elemento sin techo, para hacerlo visible al usuario
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-05-16
    /// Ultima modificacion : 2021-07-11. Para adaptarlo a los procesos de edicion (MAFG)
    /// Ultima modificacion : 2021-07-18. Le cambio la configuracion para que muestre solo el tipo de dato y el dato. Ya he cambiado el tamaño y la localizacion del contenedor
    /// Variables de entrada :
    ///         - GameObject EviBase : Es el evi base en el que reside el contenedor en el que vamos a desarrollar el fractal
    ///         - GameObject ObjetoCOntenedor : Es el objeto contenedor donde se va a desarrollar el fractal (normalmente un "EviTipo_RefFractal_01" si es el fractal de primer nivel de un 
    ///                                         "EviTipo_RefFractal_01" o un "Contenedor_FractumRef" si estamos desarrollando el fractal de una instancia de un nivel inferior
    ///                                         OJOOO no es el contenedor que cuelga del evi base, si no el objeto que este contenga
    ///         - XmlDocument KdlConcepto : Es el KDL del concepto total que contiene el evi base sobre el que se desarrolla este fractal. OJOOO aunque podemos estar desarrollando un enlace
    ///                                         esta variable lo que contiene es el KDL DE CONCEPTO 
    ///         - XmlNode  nodo_E_deZ : Es el nodo Enlace (E=>Z) que contiene el sin techo que ponemos a disposicion del usuario
    /// Variables de salida :
    /// Observaciones: Los pasos para desarrollar el gractum son:
    ///         1.) Obtener el dato del sin techo. La estructura del enlace es a que sigue :
    ///                 => E (enlace)
    ///                     => Z (sin techo)
    ///                         => R (Referencia - ojo es la referencia al tipo de dato que contiene)
    ///                             => I (identificador - ojo es el identificador de la referencia al tipo de dato que contiene)
    ///                             => F (Ctrl de configuracion  - ojo es el Ctrl de configuracion de la referencia al tipo de dato que contiene)
    ///                             => P (Ayuda a interfaz - ojo es el l Ayuda a interfaz de la referencia al tipo de dato que contiene. el dks POR AHORA NO LA SUMINISTRA 2021-05-16)
    ///                         => T (Dato - ojo es el dato en si del sin techo)
    ///         2.) Generamos el contenido del SinTecho
    ///             2.1.) Colocamos un fractum con el tipo de dato a la izquierda del contenedor
    ///             2.2.) Hacemos visible el dato en la parte derecha del contenedor, despues del fractum del tipo de dato
    ///         3.) Dependiendo del tipo de dato "R", el evi sin techo tiene un comportamiento distinto al activarlo (boton de expandir)
    ///                 - Si es un texto se muestra,
    ///                 - Si es una url se abre en el navegador
    ///                 - Si es un fichero se actua dependiendo del tipo (de la extension del fichero), imagen, audio, documento pdf, word, etc...
    ///                 OJOO Todo esto se hace desde " SctExpandirEvi => expandeSinTecho()"
    /// 
    /// </summary>
    public void construyeContenidoSinTecho(GameObject EviBase, GameObject ObjetoCOntenedor, XmlDocument KdlConcepto, XmlNode nodo_E_deZ)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScriptLibGestorEvis => construyeContenidoSinTecho. Generando evi sin techo "); }
        ///         1.) Obtener el dato del sin techo. La estructura del enlace es a que sigue :
        ///                 => E (enlace)
        ///                     => Z (sin techo)
        ///                         => R (Referencia - ojo es la referencia al tipo de dato que contiene)
        ///                             => I (identificador - ojo es el identificador de la referencia al tipo de dato que contiene)
        ///                             => F (Ctrl de configuracion  - ojo es el Ctrl de configuracion de la referencia al tipo de dato que contiene)
        ///                             => P (Ayuda a interfaz - ojo es el l Ayuda a interfaz de la referencia al tipo de dato que contiene. el dks POR AHORA NO LA SUMINISTRA 2021-05-16)
        ///                         => T (Dato - ojo es el dato en si del sin techo)

        // 2.) Generamos el contenido del SinTecho
        //    2.1.) Colocamos un fractum con el tipo de dato a la izquierda del contenedor

        // Queremos que el fractum del tipo de dato se coloque a la izquierda del contenedor y sea cuadrado. Calculamos las dimensiones
        float escala_fractum = 1f; // lo hacemos de forma que el fractum ocupe la mitad del lado del contenedor (osea una de cuatro partes)

        // OJOOO "ObjetoCOntenedor" no es el contenedor que cuelga del evi base, si no el objeto que este contenga, por eso tomamos la escala del padre, que si es el contenedor que cuelga del evi base
        float escala_x_fractum = escala_fractum / ObjetoCOntenedor.transform.parent.transform.localScale.x;  // Para que sea cuadrado, del mismo ancho  que el alto del contenedor
        float escala_y_fractum = escala_fractum;  // Lo queremos igual de alto que el contenedor
        float escala_z_fractum = escala_fractum * 1.1f; // Un pelin mas ancho para que sobresalga por encima del padre
        Vector3 escala_FractumRef = new Vector3(escala_x_fractum, escala_y_fractum, escala_z_fractum);

        // Definimos la localizacion
//        float FractumRef_localiz_x = ((-1f / 2f) / escala_x_fractum) + (escala_fractum / 2f);
        float FractumRef_localiz_x = (-1f *escala_fractum / 2f) + (escala_x_fractum * 1f / 2f);
        float FractumRef_localiz_y = 0f;
        float FractumRef_localiz_z = 0f;
        Vector3 localizacion_FractumRef = new Vector3(FractumRef_localiz_x, FractumRef_localiz_y, FractumRef_localiz_z);

        // Definimos el tipo de enlace
        //         => E (enlace)
        //            => Z(SinTecho) - nodo_Z es este nodo, no el del enlace
        string tipoEnlace = nodo_E_deZ.FirstChild.Name;  // Hay que indicar que es un enlace tipo "Z"
// 1234        string tipoEnlace = nodo_E_deZ.FirstChild.FirstChild.Name;  // Hay que indicar que es un enlace tipo "Z"

        //  Generamos el fractum correspondiente al enlace 
        GameObject EviFractumRef = Instantiate(GetComponent<ScriptDatosInterfaz>().EviTipo_RF01_FractumRef);
        // Colocamos el evi como hijo del contenedor que lo genera
        EviFractumRef.transform.SetParent(ObjetoCOntenedor.transform);
        // Hay que poner el tipo al que pertenece el EVI, antes de cargar sus datos
        EviFractumRef.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
        // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
        EviFractumRef.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_fractumReferencia);
        // Ponemos la escala
        EviFractumRef.transform.localScale = escala_FractumRef;
        // Ponemos la posicion
        EviFractumRef.transform.localPosition = localizacion_FractumRef;
        // Definimos las rotaciones
        EviFractumRef.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Indicamos el Tipo de enlace
//        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace = ScriptLibConceptosXml.prefijoDnsKdlConDosPuntos + ScriptLibConceptosXml.letra_Referencia;  // OJOOOO este evi es un sistecho, pero el fractum es el de la referencia al tipo de dato
        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().tipoEnlace = tipoEnlace;  // OJOOOO este evi es un sistecho, pero el fractum es el de la referencia al tipo de dato

        // Le indicamos al fractum que estamos generando, cual es el enlace KDL que debe visualizar y lo ponemos en el estado que le hara procesarlo
//        XmlNode nodo_Z = nodo_E_deZ.FirstChild;
//        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().nodo_E_Fractum = nodo_Z; // tenemos que referenciar el nodo "E", no el nodo "Z"
        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().nodo_E_Fractum = nodo_E_deZ; // tenemos que referenciar el nodo "E", no el nodo "Z"
        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().KdlConcepto = KdlConcepto; // El fractum tiene un enlace propio a KdlConcepto, ya que es mas facil que controlar la anidacion con prent.parent...

        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().estOpFractum = SctCtrlEviTipo_01Fractum.estOpFractum_nodeoEnlaceDisponible;
 
    }  // FIn de - public void construyeCOntenidoSinTecho()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///     modifica_TipoDato_de_SInTecho() - Modifica el tipo de dato de un sin techo
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-10
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - GameObject EviBase_de_sinTecho :  Es el evi sin techo del que hay que cambiar el tipo de dato
    ///         - GameObject Evi_de_tipo_de_dato : Es el evi que contiene el concepto del tipo de dato que hay que asignar al concepto (ojo 
    /// Variables de salida :
    /// Observaciones: 
    ///     - OJO el tipo de dato, puede ser en principio cualquier concepto (aunque se aconseja restringirse a tipos de datos conocidas por KLW)
    ///     Los pasos para desarrollar el gractum son:
    ///         1.) Obtenemos los datos del concepto destinado a ser tipo de dato
    ///         2.) Asignamos los datos del concepto, como nuevos datos del tipo de dato del sin techo
    ///         3.) Hacemos que el evi vuelva a cargar los datos de interfaz para actualizar su 
    /// 
    /// </summary>
    public void modifica_TipoDato_de_SInTecho(GameObject EviBase_de_sinTecho, GameObject Evi_de_tipo_de_dato)
    {
        // Vamos cambiado los datos correspondientes
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().key = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().key;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().host = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().host;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().cualificador = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().cualificador;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().ordinal = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().ordinal;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().fechUltMod = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().fechUltMod;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().idioma_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().idioma_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().icono_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().icono_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().audio_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().audio_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().txt_rotulo_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().txt_rotulo_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().txt_descripcion_AyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().txt_descripcion_AyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().enResources_ImgAyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().enResources_ImgAyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().enResources_IconoAyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().enResources_IconoAyuIntf;
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().enResources_AudioAyuIntf = Evi_de_tipo_de_dato.GetComponent<ScriptCtrlBaseDeEvi>().enResources_AudioAyuIntf;

        // Cargamos los textos de informacion en los elementos del canvas
        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().caja_texInfoCanvasEviBase.text = EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf;

        // Ahora actualizamos las imagenes en el evi de sin techo para que tenga la apariencia apropiada al nuevo tipo de dato

        string host = EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().host;
        string locImgAyuIntf = host + ScriptConexionDKS.sufijoAccesoAImagenesDks + EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf;  // Localizacion del fichero en el DKS
        string locIconoAyuIntf = host + ScriptConexionDKS.sufijoAccesoAIconosDks  + EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().icono_AyuIntf;  // Localizacion del fichero en el DKS
        string locAudioAyuIntf = host + ScriptConexionDKS.sufijoAccesoAAudiossDks + EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().audio_AyuIntf;  // Localizacion del fichero en el DKS

        if (DatosGlobal.niveDebug > 50)
        {
            Debug.Log("Estoy en ScriptLibGestorEvis => modifica_TipoDato_de_SInTecho : " +
              "\n - Con locImgAyuIntf : " + locImgAyuIntf +
              "\n - Con locIconoAyuIntf : " + locIconoAyuIntf +
              "\n - Con locAudioAyuIntf : " + locAudioAyuIntf +
              "\n - Con name : " + this.name
              );
        }

        EviBase_de_sinTecho.GetComponent<ScriptCtrlBaseDeEvi>().reemplaza_aspecto_AyuIntf(locImgAyuIntf, locIconoAyuIntf, locAudioAyuIntf);


}  // FIn de - public void construyeCOntenidoSinTecho()

/// <summary>
/// /////////////////////////////////////////////////////////////////
/// calculaPotenciaDelFractal : Funcion que calcula la potencia fractal, segun el numero de elementos que haya que intergrar
/// Autor : Miguel Angel Fernandez Graciani
/// Fecha creacion : 2021-04-09
/// Ultima modificacion :
/// Variables de entrada : 
///         - GameObject eviBase : Es el evi base del cual tenemos que gestionar la información de canvas
///         - string modo : Indica que tipo de gestion hay que realizar con la información del canvas- Los posibles valores son :
///                 modo = "desactivar"; Hay que apagar la informacion para que no sea visible en el canvas (porque el muro no es el activo por ejemplo)
///                 modo = "activar"; Hay que activar la informacion para que no sea visible en el canvas (porque el muro a pasado a ser activo por ejemplo)
/// Variables de salida :
/// Observaciones:
///         - OJO. La información del canvas, se activa o desactiva atendiendo a si el evibase esta en un muro activo o no
///         
/// </summary>
public void gestionaInfoCanvas(GameObject eviBase) 
    {

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Estoy en ScriptLibGestorEvis => gestionaInfoCanvas. 801 "); }

        // Vamos mirando cada hijo, y si este esta en el canvas lo desactivamos
        foreach (GameObject hijo in eviBase.transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Estoy en ScriptLibGestorEvis => gestionaInfoCanvas en el foreach. 801. En el hojo de nombre = " + hijo.name + " - con padre de nombre = " + hijo.transform.parent.gameObject.name); }
//            if (hijo.transform.parent.gameObject == GetComponent<ScriptDatosInterfaz>().CanvasGeneral)  // Si el elemento es hijo del canvas, hay que controlar su visibilidad
            if (hijo.transform.parent.gameObject.name == "CanvasGeneral")  // Si el elemento es hijo del canvas, hay que controlar su visibilidad
                {
                if (eviBase.transform.parent.gameObject == GetComponent<ScriptDatosInterfaz>().muro_Activo)
                    {
                        hijo.SetActive(true);
                        if (DatosGlobal.niveDebug > 1000)
                        { Debug.Log("Estoy en ScriptLibGestorEvis => gestionaInfoCanvas. 801. Paso por true "); }

                    }  // FIn de - if (modo == "desactivar") 
                else
                    {
                    hijo.SetActive(false);
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Estoy en ScriptLibGestorEvis => gestionaInfoCanvas. 801. Paso por false "); }
                    }
            } // FIn de - if (hijo.transform.parent == GetComponent<ScriptDatosInterfaz>().CanvasGeneral)
        }  // Fin de - foreach (GameObject hijo in eviBase.transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
    }  // Fin de - public float calculaPotenciaDelFractal(int numeroDeElementos)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// calculaPotenciaDelFractal : Funcion que calcula la potencia fractal, segun el numero de elementos que haya que intergrar
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-09
    /// Ultima modificacion :
    /// Variables de entrada : 
    /// Variables de salida :
    /// 	potenciaDelFractal	: Es un array con informacion de la potencia del fractal
    /// 	    [0] => es el numero de cuadros que utilizaremos para integrar los elementos de descripcion
    /// 	    [1] => es el exponente de la potencia del fractal
    /// 	    [2] => es la base de la potencia del fractal
    /// Observaciones:
    ///         - La saco de la version antigua en web "Miguel Angel Fernandez Graciani 2012-04-12"
    ///         - Calculamos la potencia del fractal, es la primera potencia de 2 que supera el numero de elementos de la
    ///         descripcion.La utilizamos para definir el numero de recuadros necesarios para la reperesentacion fractal.Un recuadro
    ///         para cada elemento E de la descripcion(si sobran recuadros nos la bufa). (En la version web utilizamos potencias de 4, pero es porque utiliza
    ///         tantos por ciento para gestionar el espacio. nosotros lo hacemos mediante escalas y por eso usamos potencias de dos)
    ///         - El contenedor es un mosaico cuadrado de fractums. Sgun el numero de fractums que vayamos a introducir, necesitaremos un mosaico de nxn tal que (nxn> numero de fractums)
    ///         
    /// </summary>
    public int calculaPotenciaDelFractal(int numeroDeElementos)
    {
        int PotenciaDelFractal = (int)GetComponent<ScriptDatosInterfaz>().baseDePotenciaFractal; // Queremos que como minimo existan 2x2 fractun en el contenedor
        int valor = 4; // Queremos que como minimo existan 2x2 fractun en el contenedor
        while (valor < numeroDeElementos)
        {
            valor = valor * (int)GetComponent<ScriptDatosInterfaz>().baseDePotenciaFractal;
            PotenciaDelFractal = PotenciaDelFractal + 1;
        }
        return PotenciaDelFractal;
    }  // Fin de - public float calculaPotenciaDelFractal(int numeroDeElementos)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             convierteCoordenadas : Funcion para transformar las coordenadas entre los distintos sistemas de referencia que conviven en la aplicacion
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-09-07
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - Vector3 coordConocidas : Son las coordenadas en el sistema de referencia que se conoce, osea, las que hay que convertir al nuevo sistema
    ///     - string tipoDeConversion
    /// 	    - Los posibles tipos de concersion son (estan declarados como estaticos arriba. ver "TIPOS POSIBLES DE CONCERSION DE COORDENADAS"):
    /// 	        - convCoord_muro_a_canvas : De coordenadas en el muro a coordenadas en el canvas, fundamentalmente para colocar elementos en el 
    /// 	                                    canvas de forma que se relacionen con lo elementos en el muro activo
    /// 	        - convCoord_raton_a_muro :
    /// 	        - convCoord_raton_a_canvas :
    /// Variables de salida :
    /// 	- Vector3 coordCalculadas : Son las coordenadas son las coordenadas calculadas en el nuevo sistema
    /// Observaciones:
    /// 
    ///     CON RESPECTO A LAS VARIABLES PROPIAS DEL KEE
    ///    - Ver en el fichero "ScriptDatosInterfaz" las variables :
    ///         - El bloque "Parametros de PANTALLA del dispositivo" (Ver observaciones en la documentacion en "void calculaInterfaz() {")
    ///             - int pixels_x_Pantalla;
    ///             - int pixels_y_Pantalla;
    ///             - int origenVentanaPixels_x;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor
    ///             - int origenVentanaPixels_y;   // Es la coordenada x en pixels del punto (esquina izquierda inferior) donde se localiza la ventana en la pantalla del monitor
    ///             - static float ancho_x_Pantalla = 100f;
    ///             - float alto_y_Pantalla;
    ///             - float ratio_dimensiones_Pantalla;
    ///             - float dimensionRefBaseEnPixels;
    ///             - float dimensionRefBaseEnEscala;
    ///             - float factorGeneralTamañoObgetos;
    ///             - bool modificaOrientacionMonitor;
    ///         - El bloque "Parametros de los MUROS DE TRABAJO" (Ver observaciones en la documentacion en "void calculaInterfaz() {")
    ///             - float factorEscalaMuroTrabajo;
    ///             - float escalaGeneralMuroTrabajo;
    ///             - float escalaGrosorMuroTrabajo; // se emplea para el grosor de los elementos que cuelgan del muro de trabajo (el muro es un gameobject cubico)
    ///             - float escala_x_MuroTrabajo;
    ///             - float escala_y_MuroTrabajo;
    ///             - float escala_z_MuroTrabajo;
    ///             - Vector3 escalaMuroTrabajo;
    ///             
    ///     CON RESPECTO A LAS VARIABLES PROPIAS DEL CANVAS
    ///         - El canvas general "ScriptDatosInterfaz => CanvasGeneral" se ajusta mediante el parametro "UI Scale Mmode = Constant Pixel Sice" del 
    ///         componente "CanvasScaler" en el entorno de Unity. Con "ScaleFactor = 1" con el fin de ajustar nosotros el tamaño de los objetos que
    ///         aparecen en el Canvas como mas nos convenga. Usamo el parametro "ScriptDatosInterfaz => factorEscalaGeneralCanvas", para ajustar el tamaño de los
    ///         elementos del canvas y poder poner elementos mas pequeños o mas grandes segun estemos en un smartphone, o un televisor, etc...
    ///  
    /// </summary>
    public Vector3 convierteCoordenadas(Vector3 coordConocidas, string tipoDeConversion)
    {
        Vector3 coordCalculadas = new Vector3();

        if (tipoDeConversion == convCoord_muro_a_canvas)
        {
            // En la pantalla vemos el canvas y el muro de trabajo activo, superpuestos. Las coordenadas son distintas en ambos sistemas de referencia.
            // pero el usuario debe verlo como un solo plano.
            // MURO DE TRABAJO. Las coordenadas que nos envian son las referentes al muro de trabajo activo. El muro de trabajo se escala mediante
            //                  el vector "ScriptDatosInterfaz => escalaMuroTrabajo" que se calcula al inicio en "ScriptDatosInterfaz => void calculaInterfaz() {"
            //                  Vector3 escalaMuroTrabajo;
            //                  float ratio_dimensiones_Pantalla = (X/Y)
            //              Sistema de coordenadas esta centrado en la pantalla y las coordenadas se definen en los intervalos siguientes:
            //              - (x_muro,y_muro) = (0,0) : centro de la pantalla
            //              - Coordenada x_muro [-0.5, 0.5]  Osea 1
            //              - Coordenada y_muro [-0.5, 0.5]  Osea 1
            // CANVAS. Las coordenadas del canvas
            //              - ScaleFactor = 1 (toda la pantalla ocupa una unidad de canvas)
            //              - factorEscalaGeneralCanvas (ver "ScriptDatosInterfaz => obtenDimensionMonitor() => factorEscalaGeneralCanvas = 1/2" 
            //              Sistema de coordenadas esta centrado en la pantalla y las coordenadas se definen en los intervalos siguientes:
            //              - (x_canvas,y_canvas) = (0,0) : centro de la pantalla
            //              - Coordenada x_canvas [- pixels_x_Pantalla/2f, pixels_x_Pantalla/2f] osea pixels_x_Pantalla
            //              - Coordenada y_canvas [- pixels_y_Pantalla/2f, pixels_y_Pantalla/2f] osea pixels_y_Pantalla
            //  
            // Nos dan lar coordenadas referidas al muro y nos piden las coordenadas referentes al canvas

            coordCalculadas.x = (coordConocidas.x * GetComponent<ScriptDatosInterfaz>().pixels_x_Pantalla);  // Por regla de tres (coordConocidas es a 1 como coordCalculadas es a pixels_x_Pantalla)
            coordCalculadas.y = (coordConocidas.y * GetComponent<ScriptDatosInterfaz>().pixels_y_Pantalla);
            coordCalculadas.z = 1f;  // La coordenada z no es relevante en el canvas (creo)

        }
        else if (tipoDeConversion == convCoord_raton_a_muro)
        {
            // PENDIENTE MAFG 2021-09-07
        }
        else if (tipoDeConversion == convCoord_raton_a_canvas)
        {
            // PENDIENTE MAFG 2021-09-07
        }
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log("ERROR Desde ScriptLibGestorEvis => convierteCoordenadas. Tipo de conversion no conocida. Con tipoDeConversion =  " + tipoDeConversion); }
            coordCalculadas = coordConocidas;

        }

        return coordCalculadas;
    }  // Fin de -  public Vector3 convierteCoordenadas(Vector3 coordConocidas, string tipoDeConversion)



    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             convierteCoordenadas : Funcion para transformar las coordenadas entre los distintos sistemas de referencia que conviven en la aplicacion
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-09-07
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - Vector3 coordEviBase : Son las coordenadas del evi base en el sistema de referencia del padre en el que se encuentra
    ///     - GameObject Panel_Input_Text_SinTecho : es el panel para el que hay que ajustar las coordenadas
    /// Variables de salida :
    /// 	- Vector3 coordCalculadas : Son las coordenadas son las coordenadas calculadas con el ajuste para que se coloque adecuadamente con respecto al evi sin techo
    /// Observaciones:
    /// 
    /// </summary>
    public Vector3 ajustaLocPanelEdicionSinTecho(Vector3 coordEviBase, GameObject Panel_Input_Text_SinTecho)
    {
        Vector3 coordCalculadas = new Vector3();

        float esc_x_Panel_Input_Text_SinTecho = Panel_Input_Text_SinTecho.transform.localScale.x;
        float esc_y_Panel_Input_Text_SinTecho = Panel_Input_Text_SinTecho.transform.localScale.y;
        float esc_z_Panel_Input_Text_SinTecho = Panel_Input_Text_SinTecho.transform.localScale.z;

        coordCalculadas.x = coordEviBase.x + (esc_x_Panel_Input_Text_SinTecho / 2f);
        coordCalculadas.y = coordEviBase.y + (esc_y_Panel_Input_Text_SinTecho / 2f);
        coordCalculadas.z = coordEviBase.z;

        return coordCalculadas;
    }  // Fin de -  public Vector3 convierteCoordenadas(Vector3 coordConocidas, string tipoDeConversion)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameCabezaDeEdicion : Busca ancestros arriba de un gameObjetc (que debe estar en edicion) hasta encotrar el objeto que es la cabeza de edicion
    ///                                     osea, el objeto donde comienza el arbol de edición
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-29
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject objetoDedeDondeBusco : Es el gameobject a partir del que busco para encontrar la cabeza de edicion, del arbol de edicion donde se encuentra
    /// Variables de salida :
    /// 	- GameObject objetoCabezaDeEdicion : objeto raiz (cabeza de edicion) del arbol en el que se encuentra "objetoDedeDondeBusco" como elemento en edicion
    /// 	                                    - OJOO si no se encuentra la cabeza de edición, se devuelve null
    /// Observaciones:
    ///         - OJO, si no se encuentra la cabeza de edición, se devuelve null. QUin llame a esta funcion debe considerar estre caso
    ///         - Para obtenerlo actuamos como sigue:
    ///             1.) Busamos a nuestro padre de "<ScriptDatosElemenItf>().listaDePadres" (en principio debe haber solo uno).
    ///                     - Si NO existe padre devolvemos null
    ///                     - Si hemos excedido el nivel de profundidad devolvemos null
    ///                 1.1) SI este en un evi en modo cabeza de edicion, lo devolvemos
    ///                 1.2) Si no lo es, seguimos buscando en nuestro padre.
    ///                     - Si existe el padre (volvemos a el paso 1.1. recursivamente
    /// 
    /// </summary>
    public GameObject dameCabezaDeEdicion(GameObject objetoDedeDondeBusco, int profundidad)
    {
        GameObject objetoCabezaDeEdicion = null;

        if (objetoDedeDondeBusco.GetComponent<ScriptDatosElemenItf>().listaDePadres.Count != 0)  // Si tenemos algun padre
        {
            profundidad = profundidad - 1;
            if (profundidad > 0)  // Si no hemos llegado a terminar la profundidad que venimos arrastrando
            {
                objetoCabezaDeEdicion = objetoDedeDondeBusco.GetComponent<ScriptDatosElemenItf>().listaDePadres[0];
                if (objetoCabezaDeEdicion.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_cabezaEdicion)  // si el objeto es la cabeza de edicion la retornamos
                {
                    return objetoCabezaDeEdicion;
                }
                else
                {
                    objetoCabezaDeEdicion = dameCabezaDeEdicion(objetoCabezaDeEdicion, profundidad);
                }
            }  // Fin de -  if (profundidad > 0) 
        }  // Fin de -  if (objetoDedeDondeBusco.GetComponent<ScriptDatosElemenItf>().listaDePadres.Count != 0)


        return objetoCabezaDeEdicion;
    }  // Fin de -  public GameObject dameHijoPorNombre(GameObject objetoDondeBuscar, string nombreDelHijo)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameHijoPorNombre : Busca entre los hijos de un gameObjetc el primero que tiene el nombre que se le pasa
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-15
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject objetoDondeBuscar : Es el gameobject donde vamos a buscar
    ///     - string nombreDelDescendiente : el el nombre del objeto a buscar entre los hijos de "objetoDondeBuscar"
    ///                                         OJO, solo debe haber un objeto con ese nombre
    /// Variables de salida :
    /// 	- GameObject objetoAnalizado : sera el hijo con el nombre indicado, o "null" si no hay ningun hijo con ese nombre
    /// Observaciones:
    ///         - OJO, la busqueda se realiza por orden y solo devuelve el primer objeto que encuentra con el nombre que se le pasa como parametro
    ///         esto quiere decir ESTA FUNCION FUNCIONA BIEN SI SOLO HAY UN HIJO DE "objetoDondeBuscar" CON ESE NOMBRE
    /// 
    /// </summary>
    public GameObject dameHijoPorNombre(GameObject objetoDondeBuscar, string nombreDelHijo)
    {
        GameObject objetoAnalizado = null;

        int numHijos = objetoDondeBuscar.transform.childCount;

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde dameHijoPorNombre con el objetoDondeBuscar de nombre " + objetoDondeBuscar.name +
            " -  con numHijos " + numHijos); }

        if (numHijos != 0)  // Si tenemos algun hijo en la lista
        {
            for (int i=0; i<numHijos; i++)
            {
                objetoAnalizado = objetoDondeBuscar.transform.GetChild(i).gameObject;
                if (objetoAnalizado != null)
                {
                    if (objetoAnalizado.name == nombreDelHijo) { return objetoAnalizado; }
                }  // Fin de - if (objetoAnalizado != null)
            }  // Fin de - for (int i=0; i< numHijos; i++)
        }  // Fin de - if (numHijos != 0)
        else
        {
            objetoAnalizado = null;
        }
        return objetoAnalizado;
    }  // Fin de -  public GameObject dameHijoPorNombre(GameObject objetoDondeBuscar, string nombreDelHijo)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameHijoPorTag : Busca entre los hijos de un gameObjetc el primero que tiene el tag (etiqueta) que se le pasa
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-04-09
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject objetoDondeBuscar : Es el gameobject donde vamos a buscar
    ///     - string tag : el el tag del objeto a buscar entre los hijos de "objetoDondeBuscar"
    ///                                         OJO, solo debe haber un objeto hijo con ese tag
    /// Variables de salida :
    /// 	- GameObject objetoAnalizado : sera el hijo con el nombre indicado, o "null" si no hay ningun hijo con ese tag
    /// Observaciones:
    ///         - OJO, la busqueda se realiza por orden y solo devuelve el primer objeto que encuentra con el tag que se le pasa como parametro
    ///         esto quiere decir ESTA FUNCION FUNCIONA BIEN SI SOLO HAY UN HIJO DE "objetoDondeBuscar" CON ESE TAG
    /// 
    /// </summary>
    public GameObject dameHijoPorTag(GameObject objetoDondeBuscar, string tag)
    {
        GameObject objetoAnalizado = null;

        int numHijos = objetoDondeBuscar.transform.childCount;

        if (numHijos != 0)  // Si tenemos algun hijo en la lista
        {
            for (int i = 0; i < numHijos; i++)
            {
                objetoAnalizado = objetoDondeBuscar.transform.GetChild(i).gameObject;
                if (objetoAnalizado != null)
                {
                    if (objetoAnalizado.tag == tag)
                    {
                        if (DatosGlobal.niveDebug > 1000)
                        {
                            Debug.Log(" Desde dameHijoPorTag con el objetoDondeBuscar de nombre " + objetoDondeBuscar.name +
                              " -  con numHijos " + numHijos + " -  y TAG : " + tag);
                        }
                        return objetoAnalizado;
                    }
                }  // Fin de - if (objetoAnalizado != null)
            }  // Fin de - for (int i=0; i< numHijos; i++)
        }  // Fin de - if (numHijos != 0)
        else
        {
            objetoAnalizado = null;
        }
        return null;
    }  // Fin de -  public GameObject dameHijoPorNombre(GameObject objetoDondeBuscar, string nombreDelHijo)

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameDescendientePorNombre : Busca entre los descendientes de un gameObjetc el primero que tiene el nombre que se le pasa
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-15
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject objetoDondeBuscar : Es el gameobject donde vamos a buscar
    ///     - string nombreDelDescendiente : el el nombre del objeto a buscar entre los descendientes de "objetoDondeBuscar"
    ///                                         OJO, solo debe haber un objeto con ese nombre
    /// Variables de salida :
    /// 	- - GameObject objetoDescendiente : sera el primer descendiente con el nombre indicado, o "null" si no hay ningun hijo con ese nombre
    /// Observaciones:
    ///         - OJO, la busqueda se realiza en profundidad y solo devuelve el primer objeto que encuentra con el nombre que se le pasa como parametro
    ///         esto quiere decir ESTA FUNCION FUNCIONA BIEN SI SOLO HAY UN HIJO DE "objetoDondeBuscar" CON ESE NOMBRE
    /// 
    /// </summary>
    public GameObject dameDescendientePorNombre(GameObject objetoDondeBuscar, string nombreDelDescendiente)
    {
        GameObject objetoDescendiente = null;
        GameObject objetoAnalizado = null;

        int numHijos = objetoDondeBuscar.transform.childCount;

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log(" Desde dameDescendientePorNombre con el objetoDondeBuscar de nombre " + objetoDondeBuscar.name +
              " -  con numHijos " + numHijos);
        }

        if (numHijos != 0)  // Si tenemos algun hijo en la lista
        {
            for (int i = 0; i < numHijos; i++)
            {
                objetoAnalizado = objetoDondeBuscar.transform.GetChild(i).gameObject;
                if (objetoAnalizado != null)
                {
                    if (objetoAnalizado.name == nombreDelDescendiente) { return objetoAnalizado; }
                    else
                    {
                        objetoDescendiente = dameDescendientePorNombre(objetoAnalizado, nombreDelDescendiente);
                        if (objetoDescendiente != null) { return objetoDescendiente; }
                    }
                }  // Fin de - if (objetoAnalizado != null)
            }  // Fin de - for (int i=0; i< numHijos; i++)
        }  // Fin de - if (numHijos != 0)
        else
        {
            objetoDescendiente = null;
        }
        return objetoDescendiente;
    }  // Fin de -  public GameObject dameDescendientePorNombre(GameObject objetoDondeBuscar, string nombreDelDescendiente)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameElem_T_DelPrimerSinTecho_G_elem : Busca (en amplitud) entre los hijos de un gameObjetc el primero que sea un sin techo y devuelve el valor de su elemento T (su dato)
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-29
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject objetoDondeBuscar : Es el gameobject donde vamos a buscar
    /// Variables de salida :
    /// 	- string elemeto_T : es el valor del primer elemento T del primer hijo por amplitud de "objetoDondeBuscar"
    /// Observaciones:
    ///         - OJO, la busqueda se realiza por orden y solo devuelve el primer objeto que encuentra con el nombre que se le pasa como parametro
    ///         esto quiere decir ESTA FUNCION FUNCIONA BIEN SI SOLO HAY UN HIJO DE "objetoDondeBuscar" CON ESE NOMBRE
    ///         - Utilizamos una estructura FIFO para ir explorando el arbol en amplitud
    ///         - OJOO, estamos trabajando con la GERARQUIA DE ELEMENTOS, ya que es la que nos permite llegar al elemento sin que buscamos
    /// 
    /// </summary>
    public string dameElem_T_DelPrimerSinTecho_G_elem(GameObject objetoDondeBuscar, int numMaxElemAAnalizar)
    {
        //  Vamos recorriendo en amplitud el arbol de "GERARQUIA DE ELEMENTOS (G_elem)" hasta en encontremos el primer elemento que sea
        // un evi Sin techo

        // Para recorrer el arbol en amplitud, creamos una FIFO y vamos metiendo los hijos de cada nodo de los que vamos expandiendo 

        GameObject objetoAnalizado = null;
        List<GameObject> listaFIFO = new List<GameObject>();
        string elemeto_T = null;
        int numElemAnalizados = 0;
        int indiceDeFIFO = 0;

        listaFIFO.Add(objetoDondeBuscar); // Ponemos el objetoDondeBuscar en la FIFO

        bool quedaFIFO = true; // Lo poenemos a true para poder entrar en el bucle

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log(" Desde dameElem_T_DelPrimerSinTecho_G_elem con el  objetoDondeBuscar de nombre " + objetoDondeBuscar.name );
        }

        //  "objetoDondeBuscar" es la raiz del nodo, lo analizamos, y si no es el buscado, introducimos sus hijos en la FIFO
        while (quedaFIFO)
        {
            if(listaFIFO[indiceDeFIFO] != null)
            {
                objetoAnalizado = listaFIFO[indiceDeFIFO];
                if (DatosGlobal.niveDebug > 1000)
                {
                    Debug.Log(" Desde dameElem_T_DelPrimerSinTecho_G_elem con el  objetoAnalizado de nombre " + objetoAnalizado.name +
                      " -  indiceDeFIFO : " + indiceDeFIFO +
                      " -  numElemAnalizados : " + numElemAnalizados);
                }
                // Miramos si es un sin techo
                bool esSinTecho = ((objetoAnalizado.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi) &
                                    (objetoAnalizado.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00));
                if (esSinTecho) // Si lo es, devolvemos el valor del elemento T, que es lo que nos han pedido
                {
                    elemeto_T = objetoAnalizado.GetComponent<ScriptCtrlBaseDeEvi>().texto_T_deSinTechoCanvasEviBase;
                    return elemeto_T;
                }  // Fin de - if (objetoAnalizado != null)
                else  // Si no lo es, expandimos el nodo (metemos sus hijos en la FIFO
                {
                    for (int i = 0; i< objetoAnalizado.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count; i++)
                    {
                        GameObject objetoALaFIFO = objetoAnalizado.GetComponent<ScriptDatosElemenItf>().listaDeHijos[i];
                        listaFIFO.Add(objetoALaFIFO);
                    }
                }
            } // Fin de - if(listaFIFO[indiceDeFIFO] != null)
            else
            {
                quedaFIFO = false;

                if (DatosGlobal.niveDebug > 1000)
                {
                    Debug.Log(" Desde dameElem_T_DelPrimerSinTecho_G_elem. Despues de  - else - de - if(listaFIFO[indiceDeFIFO] != null). Con el  objetoAnalizado de nombre " + objetoAnalizado.name);
                }
            } // Fin de - else - de - if(listaFIFO[indiceDeFIFO] != null)

            indiceDeFIFO = indiceDeFIFO + 1;
            numElemAnalizados = numElemAnalizados + 1;
            if (numElemAnalizados > numMaxElemAAnalizar) { quedaFIFO = false; }  // Esto es un limite que ponemos por si entrara en bucle

        }  // Fin de - while (quedaFIFO)

        return elemeto_T;

    }  // Fin de -  public string dameElem_T_DelPrimerSinTecho_G_elem(GameObject objetoDondeBuscar)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             dameEvisEnMuroPorKeyHost : Busca los hijos de un muro que tengan un key y host determinado
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-11-30
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject  muroDondeBuscar : Es el muro donde vamos a buscar
    ///     - string keyBuscado : buscamos los evis hijos de "muroDondeBuscar" que tengan este key
    ///     - string hostBuscado : buscamos los evis hijos de "muroDondeBuscar" que tengan este key
    /// Variables de salida :
    /// 	- ClassListaDeEvis EvisEnMuro : es clase que basicamente contiene una lista de los evis hijos de "" que tienen el key y host que nos mandan
    /// Observaciones:
    ///         - OJO, la busqueda se realiza pasra cualquier tipo de evi, referencia, instancia, sin techo u otros. SOLO SE PIDE QUE COINCIDAD EN KEY Y HOST
    ///         - Los pasos a seguir son los siguientes:
    ///             1.) Buscamos entre lo hijos del muro los que tengan Key y host como el solicitado
    /// 
    /// </summary>
    public ClassListaDeEvis dameEvisEnMuroPorKeyHost(GameObject muroDondeBuscar, string keyBuscado, string hostBuscado)
    {
        ClassListaDeEvis EvisEnMuro = new ClassListaDeEvis();

        // 1.) Buscamos entre lo hijos del muro los que tengan Key y host como el solicitado
        int numHijos = muroDondeBuscar.transform.childCount;

        GameObject objetoAnalizado = null;

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log(" Desde dameEvisEnMuroPorKeyHost con el muroDondeBuscar de nombre " + muroDondeBuscar.name +
              " -  con numHijos " + numHijos);
        }

        if (numHijos != 0)  // Si tenemos algun hijo en la lista
        {
            for (int i = 0; i < numHijos; i++)
            {
                objetoAnalizado = muroDondeBuscar.transform.GetChild(i).gameObject;
                // SI el elemento es un evi
                if(objetoAnalizado.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi)
                {
                    // Miramos si coincide con el key y host buscado
                    string keyDeEsteEvi = objetoAnalizado.GetComponent<ScriptCtrlBaseDeEvi>().key;
                    string hostDeEsteEvi = objetoAnalizado.GetComponent<ScriptCtrlBaseDeEvi>().host;
                    bool esEviBuscado = ((keyDeEsteEvi == keyBuscado) & (hostDeEsteEvi == hostBuscado));
                    if (esEviBuscado)
                    {
                        // Lo metemos en la lista de respuesta
                        EvisEnMuro.listaDeEvis.Add(objetoAnalizado);  // Apunto a mi padre en la lista de padres
                        Debug.Log(" Desde dameEvisEnMuroPorKeyHost con el objetoAnalizado de nombre " + objetoAnalizado.name);

                    }  // Fin de - if (esEviBuscado)
                }  // Fin de - if(objetoAnalizado.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_evi)
            }  // Fin de - for (int i=0; i< numHijos; i++)
        }  // Fin de - if (numHijos != 0)

        return EvisEnMuro;

    }  // Fin de - public ClassListaDeEvis dameEvisEnMuroPorKeyHost(GameObject muroDondeBuscar, string keyBuscado, string hostBuscado)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    ///             genera_identificador_unico : Genera un identificador unico para distintas entidades
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-03-04
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     - GameObject  muroDondeBuscar : Es el muro donde vamos a buscar
    ///     - string keyBuscado : buscamos los evis hijos de "muroDondeBuscar" que tengan este key
    ///     - string hostBuscado : buscamos los evis hijos de "muroDondeBuscar" que tengan este key
    /// Variables de salida :
    /// 	- ClassListaDeEvis EvisEnMuro : es clase que basicamente contiene una lista de los evis hijos de "" que tienen el key y host que nos mandan
    /// Observaciones:
    ///         - Los pasos a seguir son los siguientes:
    ///             1.) Ponemos el identificador del KEE
    ///             2.) ponemos el identificador del tipo de entidad
    ///             3.) Ponemos una marca de tiempo (el actual en milisegundos)
    ///             4.) Ponemos un entero que se va incrementando cada vez que generamos un nuevo identificador.
    ///                 Este entero se pone a cero cada vez que se inicia el KEE, pero la marca de tiempo debe 
    ///                 ser suficiente para distinguir entre varias iniciaciones del KEE
    /// 
    /// </summary>
    public string genera_identificador_unico(string tipo_entidad)
    {
        string identificador_unico = "";
        GetComponent<ScriptDatosInterfaz>().ordinal_de_identificador_unico = GetComponent<ScriptDatosInterfaz>().ordinal_de_identificador_unico + 1;
        long ordinal = GetComponent<ScriptDatosInterfaz>().ordinal_de_identificador_unico;

        identificador_unico = "kee_" + GetComponent<ScriptDatosInterfaz>().host_efimero_de_interfaz; 
        long Tiempo_actual = GetComponent<ScriptDatosInterfaz>().dime_milisegundo_actual();

        identificador_unico = identificador_unico + "_" + tipo_entidad;
        identificador_unico = identificador_unico + "_T_" + Tiempo_actual.ToString();
        identificador_unico = identificador_unico + "_i_" + ordinal.ToString();

        return identificador_unico;

    }  // Fin de - public ClassListaDeEvis dameEvisEnMuroPorKeyHost(GameObject muroDondeBuscar, string keyBuscado, string hostBuscado)





    /* *******************************
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////
        //   CORRUTINAS
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////////////////


        /// <summary>
        /// /////////////////////////////////////////////////////////////////
        /// Metodo (corrutina) : Espera a que el muro destino para desarrolla la expansion del evi este listo y despues llama a expandir 
        /// Autor : Miguel Angel Fernandez Graciani
        /// Fecha creacion : 2021-03-19
        /// Ultima modificacion :
        /// Variables de entrada :
        /// Variables de salida :
        /// Observaciones:
        ///         Pasos de ejecucion :
        ///             1.) Espero a tener el muro para desarrollar la expansion. Y cuando esta listo lo anoto
        ///             2.) Inicio la expansion del evi
        /// </summary>
        IEnumerator esperaPadre(GameObject contenedorAAhijar, GameObject eviPadre)
        {
            // La rama que se ha generado para expandir el evi, no genera el muro donde se van a colocar los evis de la expansion, hasta un frame
            // despues de ser generada, por lo que tenemos que esperar un frame (mediante "yield return null"), para que el muro este creado, antes
            // de empezar a colgar cosas en el muro espero dos frames (dos  "yield return null"), porque no me fio de que el orden de ejecucion de los
            // gameobjet no me ejecute el de intentar colgar cosas en el muro antes del de generarlo
            yield return null;
            yield return null;

            contenedorAAhijar.transform.parent.parent.transform.SetParent(eviPadre.transform);  /// ojoooo HABRIA QUE OCUPARSE DE QUE EL MURO SEA EL QUE ESTABA ACTIVO CUANDO SE HIZO LA SOLICITUR
            contenedorAAhijar.GetComponent<SctCtrlEviTipo_Fractal_01>().heSidoAhijado = true;
        }  // Fin de - IEnumerator traeTextura_imagen_AyuIntf(string origen

    *************************** */
    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// generaEviCompleto : Metodo (corrutina) : Hemos generado el contenido (fractal) que va a tener el contenedor del evi. Pero en este cuadro no 
    ///                         tenemos la base del evi para poder cumplimentarla debidamente, por lo que hacemos esta corrutina, para esperar
    ///                         al cuadro siguiente, donde ya tengamos la base de evi disponible para poder ir rellenandola pertinentemente
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-04-09
    /// Ultima modificacion :
    /// Variables de entrada : 
    /// Variables de salida :
    /// Observaciones:
    /// </summary>

    IEnumerator esperaYponEstado_nodeoEnlaceDisponible(GameObject EviFractumRef)
    {
        yield return null; // Esperamos un cuadro a que el evi base este listo

        yield return null; // Esperamos un cuadro a que el evi base este listo
        yield return null; // Esperamos un cuadro a para no mezcalr procesos que da la lata
                           //        desarrollaFractal();

        EviFractumRef.GetComponent<SctCtrlEviTipo_01Fractum>().estOpFractum = SctCtrlEviTipo_01Fractum.estOpFractum_nodeoEnlaceDisponible;

    }  // Fin de - IEnumerator generaEviCompleto()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// esperaYponDatosDeEviBaseDeEviRefElemen : Metodo (corrutina) : Para cumplimentar los datos de la base de los evis "EviRefElemen"
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-05-31
    /// Ultima modificacion :
    /// Variables de entrada : 
    ///     GameObject EviRefElemen : Es el evi "EviRefElemen" de cuya base vamos a cumplimentar los datos
    ///     GameObject ElemenRef :  el evi base del elemento de interfaz que este "EviRefElemen" referencia (PENDIENTE OJO se pueden 
    ///                             referenciar elementos de interfaz que no sean evis, por lo que no tendrán base. (habra que cumplimentarlo 2021-05-31
    ///     GameObject ObjetoPadre : Es el elemento de interfaz donde debe residir el "EviRefElemen" que estamos generando. OJO, es al evi base, a quien
    ///                                 hay que poner de hijo de este ObjetoPadre
    /// Variables de salida :
    /// Observaciones:
    ///     PENDIENTE OJO se pueden referenciar elementos de interfaz que no sean evis, por lo que no tendrán base. (habra que cumplimentarlo 2021-05-31
    /// </summary>

    IEnumerator esperaYponDatosDeEviBaseDeEviRefElemen(GameObject EviRefElemen, GameObject ElemenRef, GameObject ObjetoPadre)
    {
        yield return null; // Esperamos un cuadro a que el evi base este listo

        yield return null; // Esperamos un cuadro a que el evi base este listo
        yield return null; // Esperamos un cuadro a para no mezcalr procesos que da la lata
                           //        desarrollaFractal();

        // Copiamos los datos de la ayuda a interfaz del elemento que referencia, para que el EviRefElemen tenga la misma ayuda a interfaz que el elemento que referencia "ElemenRef"
        GameObject EviBaseDeEviRefElemen = EviRefElemen.GetComponent<SctCtrlEviRefElemen>().EviBase; // Obtenemos el EVI base (donde temos que cargar la Ayuda Int) del EviRefElemen

        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().idioma_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().idioma_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().icono_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().icono_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().audio_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().audio_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().txt_rotulo_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().txt_rotulo_AyuIntf;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().txt_descripcion_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().txt_descripcion_AyuIntf;

        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().domPropio = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().domPropio;
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi;

        EviBaseDeEviRefElemen.transform.SetParent(ObjetoPadre.transform);

//        EviBaseDeEviRefElemen.transform.SetParent(GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);

        EviBaseDeEviRefElemen.transform.localPosition = Tramoya.gameObject.GetComponent<ScriptCtrlTramoya>().damePosicionIniLocal(ObjetoPadre);

        // //////////////////////////////////////////////////////////////////////
        // //////////////////////////////////////////////////////////////////////
        // Gestion del tipo de elemento de interfaz que refiere este EviRefElemen
        // OJOO Habra que tener en cuenta el tipo de elemento que se referenca ( si es evi podra serlo de referencia, instancia sin techo... y pueden ser otros 
        // elementos de otros tipos 


        if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_evi)
        {
            if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            {

            }  // FIn de -  if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            {
                // Ponemos los botones del evi base, segun proceda
                for (int i = 0; i < EviBaseDeEviRefElemen.transform.childCount; i++)
                {
                    if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Maxi_Mini")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximiza; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Desplazar")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazar; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Opciones")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnOpciones; }
                }
            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            {
                // Ponemos los botones del evi base, segun proceda
                for (int i = 0; i < EviBaseDeEviRefElemen.transform.childCount; i++)
                {
                    if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Maxi_Mini")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximizaInstancia; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Desplazar")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazarInstancia; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Opciones")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesInstancia; }
                }
            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            {
                // Ponemos los botones del evi base, segun proceda
                for (int i = 0; i < EviBaseDeEviRefElemen.transform.childCount; i++)
                {
                    if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Maxi_Mini")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnMaximizaSinTecho; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Desplazar")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnDesplazarSinTecho; }
                    else if (EviBaseDeEviRefElemen.transform.GetChild(i).name == "Btn_BaseDeEvi_N1_Opciones")
                    { EviBaseDeEviRefElemen.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesSinTecho; }
                }
            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            {

            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
            {

            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
            {

            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
            else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
            {

            }  // FIn de -  else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
            else
            {

            }  // FIn de -   el se de if inicial  if if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().subTipoElementIntf_elemenRef == ScriptDatosElemenItf.subTipoElemItf_evi_rama)

        }  // FIn de -  if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_evi)
        else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_rama)
        {

        }  // FIn de - else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_rama)
        else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_muro)
        {

        }  // FIn de - else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_muro)
        else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_agente)
        {

        }  // FIn de - else if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_agente)
        else
        {

        }  // FIn de - el se de if inicial  if (EviRefElemen.GetComponent<SctCtrlEviRefElemen>().tipoElementIntf_elemenRef == ScriptDatosElemenItf.tipoElemItf_evi)


        // Desactivamos el contenedor
        EviRefElemen.transform.parent.gameObject.SetActive(false);
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().maximizado = false;


        // ////////////////////////////////////////////
        // ////////////////////////////////////////////
        // Ahora vamos poniendo en su sitio los distintos elementos de ayuda a interfaz que necesitan asignarse a los gameobject que forman el evi Base
        // OJOOO. Este el evi base de este "EviRefElemen", comparte los elementos que asignamos, texturas, materiales, audios, etc.. con el evi base del 
        // elemento que referencia. Esto es bueno, pero si se modifican aqui, se modifican alli y viceversa (son los mismos)

        // /////////////////////////
        // Comenzamos con la imagen de ayuda a interfaz

        // Ponemos la textura recibida y almacenada en "textura_imagen_AyuIntf" en el material correspondiente a la imagen de fondo
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().material_imagen_AyuIntf.SetTexture("_MainTex", ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().textura_imagen_AyuIntf);  // Le asinamos la textura al material
                                                                                                                                                                                                     // Ponemos el material en el objeto contenedor, Que es quien contiene la imagen de fondo
        EviBaseDeEviRefElemen.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ContenedorDeEvi_01.GetComponent<MeshRenderer>().material = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().material_imagen_AyuIntf;

        // Vamos con la imagen que acompaña al icono (Es la misma fuente que la anterior, pero se visualiza mas pequeña cuando marcamos el boton de información)
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().textura_ImgAyuIntfBaseDeEvi = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().textura_imagen_AyuIntf; // Utilizamos la misma imagen para el fondo y para la que acompaña al icono
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().material_ImgAyuIntfBaseDeEvi.SetTexture("_MainTex", ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().textura_ImgAyuIntfBaseDeEvi);  // Le asinamos la textura al material
        EviBaseDeEviRefElemen.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.GetComponent<MeshRenderer>().material = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().material_ImgAyuIntfBaseDeEvi; // Asignamos el material al objeto

        // Modificamos el estado para actuar en consecuencia
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().estadoImgAyudaIntf = "texturaCargada";

        // /////////////////////////
        // Vamos con el icono de ayuda a interfaz

        // Ponemos la textura recibida y almacenada en "textura_icono_AyuIntf" en el material correspondiente al icono del concepto
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().material_icono_AyuIntf.SetTexture("_MainTex", ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().textura_icono_AyuIntf);  // Le asinamos la textura al material
        EviBaseDeEviRefElemen.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<MeshRenderer>().material = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().material_icono_AyuIntf; // Asignamos el material al objeto

        // Modificamos el estado para actuar en consecuencia
        EviBaseDeEviRefElemen.GetComponent<ScriptCtrlBaseDeEvi>().estadoIconoAyudaIntf = "texturaCargada";

        // /////////////////////////
        // Vamos con el audio de ayuda a interfaz

        //Aplicamos el audio clip al audiosource
        EviBaseDeEviRefElemen.gameObject.GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Info.GetComponent<Script_BaseDeEvi_N1_Info>().audio_AyuIntf = ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().AudioClip_AyuIntf;

        // Modificamos el estado para actuar en consecuencia
        ElemenRef.GetComponent<ScriptCtrlBaseDeEvi>().estadoIconoAyudaIntf = "texturaCargada";

    }  // Fin de - IEnumerator generaEviCompleto()


}  // Fin de - public class ScriptLibGestorEvis : MonoBehaviour {
