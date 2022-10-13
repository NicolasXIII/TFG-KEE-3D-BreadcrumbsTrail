using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using TMPro;

using UnityEngine.UI;
using UnityEditor;


/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script para expandir un evi colocando su descripcion en un nuevo muro 
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-0x-
/// Observaciones :
///     - La opcion de expandir, no siempre es generar un nuevo muro con la descripcion del concepto. Existen otras posibilidades
///         - Para evi de referencia => expande el concepto en un nuevo muro en una rama hija del actual
///         - Para evis de instancia => expande la descripcion de la instancia en un nuevo muro en una rama hija del actual
///         - Para evis Sin Techo => actua dependiendo del tipo de dato
///                 - Ver "public void expandeSinTecho()" 
///
///		DATOS GENERALES :
///		
///		METODOS GENERALES :
///			-
/// </summary>
public class SctExpandirEvi : MonoBehaviour {

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public Canvas CanvasGeneral;

    public GameObject ramaAsociada;

    //Nicolas Merino Ramirez
    public GameObject BT_Contenedor;

    // Use this for initialization
    void Start ()
    {

        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;
        //  Btn_Caja_opciones = this.transform.parent.tamañoAltoBotonActivo;
        // this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;
        //this.transform.localScale = this.GetComponentInParent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;
        ramaAsociada = null;
        //x = ctrlInterfaz.GetComponent<ScriptCtrlMuroUsuario>();
        
        //Nicolas Merino Ramirez
        //this.BT_Contenedor = GameObject.Find("BT_Contenedor");



    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  public void botonExpandeEvi(). Se ha pulsado el boton de expandir del evi base.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-14
    /// Ultima modificacion :
    ///         - 2021-09-01 : Hago que se puedan expandir las instancias
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo expande el evi en un nuevo muro
    /// Observaciones : Se ha pulsado el boton de expandir del evi base. Segun proceda existen varias posibilidades
    ///       0.) Si el evi es un evi sin techo, la expansion consiste en abrir el contenido del evi segun el tipo de dato (texto, url, fichero...)
    ///             Abrimos el contenido y salimos
    ///       1.) Miramos si el evi ya ha sido expandido (o la rama generada, para el caso de evi de rama)
    ///       
    ///       2.) Si el evi ya ha sido expandido (o la rama creada), nos colocamos e la rama asociada (o la que expande el evi) en el primer muro,
    ///        para navegar por ella
    ///        
    ///       3.) Si el evi no esta expandido o no se ha generado la rama (si es un evi de rama), se expande el evi, o se genera la rama correspondiente
    ///           - Todos los elementos de interfaz seran EVIs " tipoElementIntf = tipoElemItf_evi ". Pero para expandirlo, lo haremos atendiendo a la naturaleza
    ///           de este, es decir a su subtipo. 
    ///           - En muchos casos se expande el evi generando un nuevo muro y desarrollando cada uno de los subelementos del evi como un evi independiente en el nuevo muro
    ///                 - Los pasos para realizar el proceso son los siguientes
    ///                     - 1. Generamos una rama y su muro correspondiente para expandir la rama
    ///                     - 2. En el nuevo muro (que ahora será el activo), generamos cada uno de los evis en los que se expande el evi origen, segun corresponda :
    ///                     Dependiendo pues del subtipo de evi en el que estamos
    ///             
    ///           3.1.) Evi subtipo de gestion de rama " subTipoElementIntf = subTipoElemItf_evi_rama" => genera una nueva rama asociada a este evi
    ///                     SI el evi ES UN EVI DE RAMA, generamos una rama con su nuro inicial, para que el usuario tenga la nueva rama de navegacion que ha solicitado
    ///           3.2.) Evi subtipo referencia fractal " subTipoElementIntf = subTipoElemItf_evi_RefFractal" => genera un evi de referencia fractal (getdetails)
    ///                     Si el evi origen ES UN EVI DE REFERENCIA FRACTAL, generamos un evi para cada uno de los conceptos que existen en su  descripcion, instancias y 
    ///                     referencias. Indicando lo que son, mediante el color del evi base o algo  asi (tambien podemos indicar si son efimeros, sin techo, etc...)
    ///           3.3.) Evi subtipo instancia fractal " subTipoElementIntf = subTipoElemItf_evi_InstFractal" => genera un evi de instancia fractal (descripcion de una instanciacion)
    ///                     Si el evi origen ES UN EVI DE INSTANCIA FRACTAL (PENDIENTE MAFG 2021-03-14)
    ///           3.4.) Evi subtipo sin techo " subTipoElementIntf = subTipoElemItf_evi_sinTecho_00" => genera un evi sin techo (lo que viene siendo un formulari, dato y tipo de dato)
    ///                     Este caso es el PASO 0 DE ESTA SECUENCIA. Si es un sin techo se actua en consecuenci (navegador si es url, mostrar si es texto,...)
    ///           3.5.) Evi subtipo buscador 00 " subTipoElementIntf = subTipoElemItf_evi_buscador_00" => genera un evi que contendra el resultado de la busqueda
    ///                     Si el evi origen ES UN EVI BUSCADOR (PENDIENTE MAFG 2021-03-14)
    ///           3.6.) Evi subtipo lista 00 " subTipoElementIntf = subTipoElemItf_evi_lista_00" => genera un evi que contendra una lista de elementos de interfaz
    ///                     Si el evi origen ES UN EVI DE LISTA (PENDIENTE MAFG 2021-03-14)
    ///           3.7.) Evi subtipo camino 00 " subTipoElementIntf = subTipoElemItf_evi_camino_00" => genera un evi que contendra la informacion de caminos de relacion entre conceptos
    ///                     Si el evi origen ES UN EVI DE CAMINO (PENDIENTE MAFG 2021-03-14)
    ///           3.8.) Evi subtipo arbol 00 " subTipoElementIntf = subTipoElemItf_evi_arbol_00" => genera un evi que contendra la informacion de un arbol
    ///                     Si el evi origen ES UN EVI DE ARBOL (PENDIENTE MAFG 2021-03-14)
    ///                 
    /// </summary>
    public void botonExpandeEvi()
    {
        
        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        //   0.) Si el evi es un evi sin techo, la expansion consiste en abrir el contenido del evi segun el tipo de dato (texto, url, fichero...)
        //       Abrimos el contenido y salimos

        if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 0, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
            expandeSinTecho();
            return;
            //    Si el evi origen ES UN EVI SIN TECHO (PENDIENTE MAFG 2021-03-14)
        }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_sinTecho_00)


        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        // 1.) Miramos si el evi ya ha sido expandido (o la rama generada, para el caso de evi de rama)

        // OOOJOOOO estamos suponiendo que un evi expanddo siempre tiene un hijo rama asociado. Esto puede no ser asi, por ejemplo en la expansion de un nodo buscador
        // que abre el resultado en el mismo muro donde reside Hay que revisar pues el if que sigue atendiendo a esta caracteristica. PENDIENTE MAFG 2021-03-17)

        // Buscamos una rama hija de este evi base. Si lo hemos expandido o es una rama hija, debe haber un hijo rama de este evi
        //  OJOO la lista de hijos esta en el EVI base, y estamos en el boton
        if (transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in transform.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama)  // Si el hijo es una rama 
                {
                    // si es rama, es la rama asociada a la expansion del evi (OJOOO ESTO IMPLICA QUE CADA EVI, SOLO PUEDE TENER UNA RAMA ASOCIADA)                               
                    ramaAsociada = hijo;
                } // FIn de - if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)



        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        // 2.) Si el evi ya ha sido expandido (o la rama creada), nos colocamos e la rama asociada (o la que expande el evi) en el primer muro, para navegar por ella

        if (ramaAsociada != null)
        {
            // //////////////////////////////////////////////////////
            // definimos la rama como la rama activa 
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo = ramaAsociada;
            // Indicamos que el usuario esta en esta rama
            Usuario.GetComponent<ScriptCtrlUsuario>().rama_EnLaQueEstaElUsuario = ramaAsociada;

            // //////////////////////////////////////////////////////
            // Colocamos al usuario
            // Hacemos al usuario hijo de la rama a la que vamos
            Usuario.transform.SetParent(ramaAsociada.transform);

            // Y lo colocamos en el origen de la rama y con su orientacion correspondiente
            Vector3 posicionUsr = new Vector3(0.0f, 0.0f, 0.0f);
            Usuario.transform.localPosition = posicionUsr;

            Quaternion rotacionUsr = Quaternion.Euler(0f, 0f, 0f);
            Usuario.transform.localRotation = rotacionUsr;

            // COlocamos al usuario en el prime muro de la ramma en la que acabamos de entrar
            int direccion = 1;  // La direccion es 1, porque vamos hacia adelante
            float distancia = ramaAsociada.GetComponent<ScriptCtrlRama>().calculaLongitudTramoRama();
            Usuario.GetComponent<ScriptCtrlUsuario>().iniciaTransicion(direccion, distancia);

        } // Fin de - if (ramaAsociada != null)
        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        // 3.) Si el evi no esta expandido o no se ha generado la rama(si es un evi de rama), se expande el evi, o se genera la rama correspondiente
        else
        {
                // ///////////////////////////////////////////
                //  3.1.) Evi subtipo de gestion de rama " subTipoElementIntf = subTipoElemItf_evi_rama" => genera una nueva rama asociada a este evi
                //        SI el evi ES UN EVI DE RAMA, generamos una rama con su nuro inicial, para que el usuario tenga la nueva rama de navegacion que ha solicitado
            if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.1, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                // Por ahora solo solicitamos la generacion de una nueva rama
                // ///////////
                // Generamos una nueva rama
                // La rama se genera como hija de la rama activa y en este caso asociada a este evi () que la ha generado
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                ramaAsociada = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaRama(gameObject, this.GetComponent<ScriptDatosElemenItf>().modo);

            }  // Fin de - if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
               // ///////////////////////////////////////////
               //  3.2.) Evi subtipo referencia fractal " subTipoElementIntf = subTipoElemItf_evi_RefFractal" => genera un evi de referencia fractal (getdetails)
               //        Si el evi origen ES UN EVI DE REFERENCIA FRACTAL, generamos un evi para cada uno de los conceptos que existen en su  descripcion, instancias y 
               //        referencias. Indicando lo que son, mediante el color del evi base o algo  asi (tambien podemos indicar si son efimeros, sin techo, etc...)
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.2, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }

                // ///////////
                // Generamos una nueva rama
                // La rama se genera como hija de la rama activa y en este caso asociada a este evi () que la ha generado
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                ramaAsociada = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaRama(gameObject, this.GetComponent<ScriptDatosElemenItf>().modo);

                // OJOOO hemos generado la rama, pero la rama no genera el muro asociado si no hasta el siguiente frame (Ver "ScriptCtrlRama => generaRama")
                // Por lo que no tenemos muro donde colocar los evis que vayamos generando.
                // Esperamos a que la rama tenga el muro inicial, y despues, vamos generando en este los evis que expanden el elemento que estamos expandiendo
                // utilizamos para ello una corrutina 

                StartCoroutine(esperaMuro());


            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_RefFractal)
               // ///////////////////////////////////////////
               //  3.3.) Evi subtipo instancia fractal " subTipoElementIntf = subTipoElemItf_evi_InstFractal" => genera un evi de instancia fractal (descripcion de una instanciacion)
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.3, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }

                // ///////////
                // Generamos una nueva rama
                // La rama se genera como hija de la rama activa y en este caso asociada a este evi () que la ha generado
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                ramaAsociada = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaRama(gameObject, this.GetComponent<ScriptDatosElemenItf>().modo);

                // OJOOO hemos generado la rama, pero la rama no genera el muro asociado si no hasta el siguiente frame (Ver "ScriptCtrlRama => generaRama")
                // Por lo que no tenemos muro donde colocar los evis que vayamos generando.
                // Esperamos a que la rama tenga el muro inicial, y despues, vamos generando en este los evis que expanden el elemento que estamos expandiendo
                // utilizamos para ello una corrutina 

                StartCoroutine(esperaMuro());

            }  // Fin de - 
               // ///////////////////////////////////////////
               //  3.4.) Evi subtipo sin techo " subTipoElementIntf = subTipoElemItf_evi_sinTecho_00" => genera un evi sin techo (lo que viene siendo un formulari, dato y tipo de dato)
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            {
                // Esto no debe suceder debido al if que hay en el punto 0. de este metodo
                // Este caso es el PASO 0 DE ESTA SECUENCIA.Si es un sin techo se actua en consecuenci(navegador si es url, mostrar si es texto,...)
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.4, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //    Si el evi origen ES UN EVI SIN TECHO (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_sinTecho_00)
               // ///////////////////////////////////////////
               //  3.5.) Evi subtipo buscador 00 " subTipoElementIntf = subTipoElemItf_evi_buscador_00" => genera un evi que contendra el resultado de la busqueda
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.5, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI BUSCADOR (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
               // ///////////////////////////////////////////
               //  3.6.) Evi subtipo lista 00 " subTipoElementIntf = subTipoElemItf_evi_lista_00" => genera un evi que contendra una lista de elementos de interfaz
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.6, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE LISTA (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
               // ///////////////////////////////////////////
               //  3.7.) Evi subtipo camino 00 " subTipoElementIntf = subTipoElemItf_evi_camino_00" => genera un evi que contendra la informacion de caminos de relacion entre conceptos
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.7, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE CAMINO (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
               // ///////////////////////////////////////////
               //  3.8.) Evi subtipo arbol 00 " subTipoElementIntf = subTipoElemItf_evi_arbol_00" => genera un evi que contendra la informacion de un arbol
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), paso 3.8, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE ARBOL (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
               // ///////////////////////////////////////////
               //  Si llegamos aqui esque el evi es de un subtipo que no conocemos
            else   
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), per el else sin condicion, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }

            }  // Fin de - else (sin condiciones)


            // Nicolas Merino Ramirez
            // Meter en un metodo
            // Descripcion
            //      Genera un EVI de referencia del EVI X en el contenedor de las migas de pan 
            Debug.Log("Crear referencia EVI en Contenedor_BreadcrumbsTrails \n");
            
            Debug.Log("1 EXPANDIR EVI INIT");

            GameObject panera = Usuario.gameObject.GetComponentInChildren<ScriptCtrlMuroUsuario>().Contenedor_BreadcrumbsTrails;

            this.ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen
            (
                this.transform.gameObject,
                //this.BT_Contenedor
                panera
            );

            Debug.Log(" 9999 EXPANDIR EVI FIN");

        }  // Fin de - else - de - if (ramaAsociada != null)

    } // Fin de - public void botonExpandeEvi()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// expandeDescripcion : Obtiene los elementos de descripcion asociados a un concepto o un enlace, y los expande en el elemento destino (normalmente un muro) 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-03-19
    /// Ultima modificacion : 
    ///         2021-05-11 para expandir instancias y sin techo como tales (antes abria solo el concepto como referencia)
    /// Variables de entrada :
    ///         - GameObject elemDestino  : es el game object donde debe expandirse la descripcion (normalmente un muro)
    /// Variables de salida :
    /// Observaciones:
    ///         - En el evi que estamos expandiendo, en su componente "ScriptCtrlBaseDeEvi" tenemos la propiedad "domPropio", donde se almacena el KDL
    ///           que describe el concepto que el evi visualiza.
    ///           
    ///         - Pasos de ejecucion :
    ///             1.) Voy obteniendo la lista de elementos que componen la descripcion.
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
    ///             2.) Voy generando un evi nuevo sobre el muro para cada uno de los elementos de la descripcion del evi a expandir, segun sea referencia, instancia o sin techo
    ///                 Para cada enlace de descripcion :
    ///                 2.1.) Primero vemos que tipo de enlace es (Referencia, instancia o sin techo
    ///                                 => E (enlace)
    ///                                     => R (referencia)
    ///                                     => A (instancia)
    ///                                     => Z (sin techo)
    ///                 2.2.) Dependiendo del tipo de enlace:
    ///                  2.2.1.) Si es UNA REFERENCIA : 
    ///                             * Por ahora solo generamos un evi fractal, pero habria que poder generar listas, caminos, arboles, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                         - referencias (obtenemos los datos asociados a la referencia)
    ///                                 => E (enlace)
    ///                                     => R (referencia)
    ///                                         => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P (Ayuda a interfaz) (opcional)
    ///                      -  Tomo sus datos de identificacion
    ///                      - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
    ///                 
    ///                     2.2.2.)  Si es UNA ISTANCIA : Ya tengo en el KDL local la descripcion (D) de la instancia, por lo que no tengo que ir a buscarla al DKS
    ///                             * Por ahora solo generamos un evi fractal, pero habria que poder generar listas, caminos, arboles, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                         - instancias (obtenemos los datos asociados a la instancia)
    ///                                 => E (enlace)
    ///                                     => A (instancia)
    ///                                         => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P  (Ayuda a interfaz) (opcional)
    ///                                         => D
    ///                                             => E (enlace)
    ///                                             => E (enlace)
    ///                                            .....
    ///                                             => E (enlace)
    ///                      - Llamamos al generador de evi fractal de instancia  "generaEviFractalInst()"
    ///                 
    ///                     2.2.3.)  Si es UN SIN TECHO : Ya tengo el el KDL local la referencia (R) al tipo de sintecho, asi como el dato en si (T), por lo que no tengo que ir a buscarla al DKS
    ///                             * Por ahora solo lo tratamos como texto plano, pero habria que poder tratarlo como url, fecha, numero, etc.. PENDIENTE (MAFG 2021-03-19) 
    ///                         - sin techo (obtenemos los datos asociados al elemento sin techo)
    ///                              => E (enlace)
    ///                                     => Z (sin techo)
    ///                                         => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
    ///                                            => I (identificador)
    ///                                             => F (control de configuracion)
    ///                                             => P (Ayuda a interfaz) (opcional)
    ///                                         => T (dato sin techo, string de texto)
    ///                       - Llamamos al generador de evi sin techo  "generaEviSinTecho()"
    ///                 
    /// </summary>
    public void expandeDescripcion(GameObject elemDestino)
    {
        // ////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////
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
        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(GetComponent<ScriptCtrlBaseDeEvi>().domPropio.NameTable);
        manejadorEspNomb.AddNamespace(ScriptLibConceptosXml.prefijoDnsKdl, ScriptLibConceptosXml.DnsKdl);

        XmlNode KDL_Raiz = GetComponent<ScriptCtrlBaseDeEvi>().domPropio.DocumentElement;
        XmlNode KDL_Nodo_C = KDL_Raiz; // COmo lo vemos como un get details, la raiz del KDL que hemos obtenido es la raiz del concepto

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde expandeDescripcion con nodo name" + KDL_Nodo_C.Name); }

//        ListaNodos_E_en_D = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().damelistaDescripcionDeEnlace(KDL_Nodo_C, manejadorEspNomb);
        ListaNodos_E_en_D = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().damelistaDescripcionDeEnlace(GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi, manejadorEspNomb);

        
        // ////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////
        // 2.) Voy generando un evi nuevo sobre el muro para cada uno de los elementos de la descripcion del evi a expandir, segun sea referencia, instancia o sin techo

        if (ListaNodos_E_en_D != null)
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde SctExpandirEvi => expandeDescripcion. La lista de descripcion NO es nula. Numero de enlaces = " + ListaNodos_E_en_D.Count); }
            foreach (XmlNode nodoEnlace in ListaNodos_E_en_D)
            {
                // Para cada enlace
                XmlNode nodo_E_A_o_Z = nodoEnlace.FirstChild;

                // 2.1.) Primero vemos que tipo de enlace es (Referencia, instancia o sin techo
                string tipoEnlace = nodo_E_A_o_Z.Name;


                if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_R)
                {
                    // 2.2.1.) Si es UNA REFERENCIA : 
                    // -  Tomo sus datos de identificacion
                    // Vamos con el identificador
                    string[] identificadorConcepto_K_H_Q = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameIdentificadorDeNodo(GetComponent<ScriptCtrlBaseDeEvi>().domPropio, nodo_E_A_o_Z);
                    // Asignamos a las variables de este game object
                    string ref_key = identificadorConcepto_K_H_Q[0];                            // key: es el key del concepto (K en KDL - esta en I en KDL)
                    string ref_host = identificadorConcepto_K_H_Q[1];                         // host : es el host del concepto (H en KDL - esta en I en KDL)
                    string ref_cualificador = identificadorConcepto_K_H_Q[2];                 // cualificador : indica la naturaleza del concpto (efimero,...) (Q en KDL - esta en I en KDL)
                    string ordinalConf = null;
                    DateTime ultiModConf = new DateTime(0);

                    // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(ref_key, ref_host, ref_cualificador, ordinalConf, ultiModConf, elemDestino);

                    if (DatosGlobal.niveDebug > 1000)
                    {
                        Debug.Log("Desde SctExpandirEvi => expandeDescripcion. Pasamos por tipo referencia. Es de tipo : " + tipoEnlace +
                        " - Con ref_key = " + ref_key +
                        " - ref_host = " + ref_host +
                        " - ref_cualificador = " + ref_cualificador +
                        " - ordinalConf = " + ordinalConf +
                        " - ultiModConf = " + ultiModConf +
                        " - elemDestino = " + elemDestino.name);
                    }
                } // Fin de - if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_R)
                else if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_A)
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde SctExpandirEvi => expandeDescripcion. Pasamos por tipo instancia. Es de tipo : " + tipoEnlace); }
                    if (DatosGlobal.niveDebug > 1000)
                    {
                        Debug.Log("Desde SctExpandirEvi => expandeDescripcion. " +
                        " - nodoEnlace = " + nodoEnlace.Name +
                        " - nodo_E_A_o_Z = " + nodo_E_A_o_Z.Name +
                        " - nodo_E_A_o_Z FirstChild = " + nodo_E_A_o_Z.FirstChild.Name +
                        " - nodo_E_A_o_Z FirstChild FirstChild = " + nodo_E_A_o_Z.FirstChild.FirstChild.Name +
                        " - nodo_E_A_o_Z FirstChild FirstChild InnerText = " + nodo_E_A_o_Z.FirstChild.FirstChild.InnerText);
                    }

                    ///                     2.2.2.)  Si es UNA ISTANCIA : Ya tengo en el KDL local la descripcion (D) de la instancia, por lo que no tengo que ir a buscarla al DKS
                    ///                             * Por ahora solo generamos un evi fractal, pero habria que poder generar listas, caminos, arboles, etc.. PENDIENTE (MAFG 2021-03-19) 
                    ///                         - instancias (obtenemos los datos asociados a la instancia)
                    ///                                 => E (enlace)
                    ///                                     => A (instancia)
                    ///                                         => I (identificador)
                    ///                                         => F (control de configuracion)
                    ///                                         => P  (Ayuda a interfaz) (opcional)
                    ///                                         => D
                    ///                                             => E (enlace)
                    ///                                             => E (enlace)
                    ///                                            .....
                    ///                                             => E (enlace)
                    ///                      - Llamamos al generador de evi fractal de instancia  "generaEviFractalInst()"
                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalInst(elemDestino,
                                                                                    GetComponent<ScriptCtrlBaseDeEvi>().domPropio,
                                                                                    manejadorEspNomb,
                                                                                    nodoEnlace);
                }  // Fin de - else if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_A)
                else if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_Z)
                {
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("Desde SctExpandirEvi => expandeDescripcion. Pasamos por tipo sin techo. Es de tipo : " + tipoEnlace); }
                    // 2.2.3.)  Si es UN SIN TECHO : Ya tengo el el KDL local la referencia (R) al tipo de sintecho, asi como el dato en si (T), por lo que no tengo que ir a buscarla al DKS
                    //             * Por ahora solo lo tratamos como texto plano, pero habria que poder tratarlo como url, fecha, numero, etc.. PENDIENTE (MAFG 2021-03-19) 
                    //         - sin techo (obtenemos los datos asociados al elemento sin techo)
                    //              => E (enlace)
                    //                     => Z (sin techo)
                    //                         => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
                    //                            => I (identificador)
                    //                            => F (control de configuracion)
                    //                            => P (Ayuda a interfaz) (opcional)
                    //                         => T (dato sin techo, string de texto)
                    // - Llamamos al generador de evi sin techo  "generaEviSinTecho()"
                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviSinTecho(elemDestino,
                                                                                    GetComponent<ScriptCtrlBaseDeEvi>().domPropio,
                                                                                    manejadorEspNomb,
                                                                                    nodoEnlace);
                }  // Fin de - else if (tipoEnlace == ScriptLibConceptosXml.xPathString_name_Z)
                else
                {
                    if (DatosGlobal.niveDebug > 90)
                    { Debug.Log("ERROR. Desde SctExpandirEvi => expandeDescripcion. Pasamos por tipo DESCONOCIDO. Es de tipo : " + tipoEnlace); }
                }
            }  // Fin de - foreach (XmlNode nodoEnlace in ListaNodos_E_en_D)
        }  // Fin de - if (ListaNodos_E_en_D != null)
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde SctExpandirEvi => expandeDescripcion. Ya he pasado la lista de descripcion"); }
    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  public void expandeSinTecho()
    /// Observaciones : Expandir un Sin techo consiste en activar su contenido (url, imagen, texto, etc...)
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-07-15
    /// Ultima modificacion :
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///     Expandir un Sin techo consiste en activar su contenido (url, imagen, texto, etc...). La operacion de expansion depende en este caso del tipo de dato
    ///     Los tipos de datos conocidos pueden verse en "ConceptosConocidos => Conceptos referentes a TIPOS DE DATOS SIN TECHO"
    ///     
    ///     - Segun el tipo de dato actuamos en consecuencia
    /// 
    ///         1.)   "gen_tipoDeSinTechoTextoPlano"
    ///                     TEXTO PLANO como tipo de dato sin techo => Se abre el texto (contenido en el elemento T hijo de Z de KDL) en el cambas general
    ///         2.)   "gen_tipoDeSinTecho_NumeroEntero"
    ///                     NUMERO ENTERO (numero entero) como tipo de dato sin techo = => Se abre el texto con el numero entero que contiene (contenido en el elemento T hijo de Z de KDL) en el cambas general
    ///         3.)   "gen_tipoDeSinTecho_Url"
    ///                     URL (localizacion en internet) como tipo de dato sin techo = => Se abre la url (contenida como texto en el elemento T hijo de Z de KDL) en el navegador por defecto del sistema
    ///         4.)   "gen_tipoDeSinTecho_FicheroGenerico"  (no esta programado PENDIENTE MAFG 2021-11-26)
    ///             4.1.) Obtenemos la extension del fichero
    ///             4.2.) Segun el tipo de extensión actuamos en consecuencia
    ///         5.)   "gen_tipoDeSinTecho_FicheroAudio"  (no esta programado PENDIENTE MAFG 2021-11-26)
    ///             5.1.) Obtenemos la extension del fichero
    ///             5.2.) Segun el tipo de extensión actuamos en consecuencia
    ///         6.)   "gen_tipoDeSinTecho_FicheroImagen"  (no esta programado PENDIENTE MAFG 2021-11-26)
    ///             6.1.) Obtenemos la extension del fichero
    ///             6.2.) Segun el tipo de extensión actuamos en consecuencia
    ///         7.)   "gen_tipoDeSinTecho_FicheroVideo"  (no esta programado PENDIENTE MAFG 2021-11-26)
    ///             7.1.) Obtenemos la extension del fichero
    ///             7.2.) Segun el tipo de extensión actuamos en consecuencia
    ///         8.)   "gen_tipoDeSinTecho_LlamadaFuncionKEE" : 
    ///                 - Este tipo es para llamar afunciones preexistentes en la interfaz KEE. 
    ///                 - Cuando el usuario teclea sobre la opcion de expandir de un evi sin techo que contiene un dato de tipo "gen_tipoDeSinTecho_FicEjecIntfKLW", la interfaz del KEE
    ///                     ejecuta la funcion que aparece en el campo T del sin techo.
    ///                 - OJOOO la funcion se ejecuta sobre el evi localizado (en la gerarquia), segun se indica en la documentacion de cada funcion
    ///                 - Estas funciones estan localizadas en el script "" asociado a ""
    ///                 - El nombre de la función a llamar debe estar en dato el elemento sin techo como sigue
    ///                 
    ///                             => E (enlace)
    ///                                 => Z (sin techo)
    ///                                     => R (referencia al tipo de dato sin techo : "gen_tipoDeSinTecho_FicEjecIntfKLW")
    ///                                        => I (identificador)
    ///                                         => F (control de configuracion)
    ///                                         => P (Ayuda a interfaz) (opcional)
    ///                                     => T (dato sin techo, string de texto aqui va el nombre de la funcion a ejecutar. Ej.: "funcionKEE_ActivadoBoton_BuscadorKee_por_key_host")
    /// 
    ///                 - Para que esto suceda asi, en la BBDD de "dks_klw" debe existir un sin techo que contenga la cadena con el nombre de la funcion a ejecutar.
    ///                     ej. : 
    ///                         insert  into `conceptos_sin_techo`(`Idcst`,`Clave`,`Localizacion`,`Ordinal`,`TiempoActualizacion`,`Contenido`,`ClaveTipo`,`LocalizacionTipo`,`OrdinalTipo`,`TiempoActualizacionTipo`,`ClaveUsuario`,`LocalizacionUsuario`,`Acceso`) 
    ///                     	    values
    ///                     			(37,"gen_st_vacio_para_FicEjecIntfKLW","conceptos_sin_techo",0,0,"funcionKEE_ActivadoBoton_BuscadorKee_por_key_host","gen_tipoDeSinTecho_FicEjecIntfKLW",@localizacion_DKS_KLW,@ordinalDeAlta_KLW,@tiemoDeAlta_KLW,@claveUsuario,@localizacionUsuario,@accesoPorDefecto),
    ///                         Y este debe ser definido como revferencia 
    ///                         
    ///                             - O bien en una isntancia de un tipo de boto. Por ejemplo
    ///                                  (396,0,"gen_botonKee",@locDKSLocal,@ordinalDeAlta,@tiemoDeAlta,1,"",77,0),
    ///		                                (397,396,"gen_recAyuIntf",@locDKSLocal,@ordinalDeAlta,@tiemoDeAlta,1,"",2,0),
    ///		                                /* definimos ahora el resto de la descripcion del concepto *** */
    ///	                                   /* Tiene basicamente un texto para indicar la identidad del boton y un ejecutable aue indica la funcion de codigo que se ejecuta al pulsarlo */
    ///		                                (413,396,"gen_st_vacio_para_textoPlano","conceptos_sin_techo",@ordinalDeAlta,@tiemoDeAlta,1,"",30,2),
    ///		                                (414,396,"gen_st_vacio_para_FicEjecIntfKLW","conceptos_sin_techo",@ordinalDeAlta,@tiemoDeAlta,1,"",37,2);
    ///                                 En este caso cuando aparece la instancia del boton hay que abrirla hasta llegar a poder pulsar el boton
    ///                             - bien directamente. Por ejemplo: 
    ///                                  (392,358,"gen_BuscadorKee",@locDKSLocal,@ordinalDeAlta,@tiemoDeAlta,1,"",2,0),
    ///                                     /* como buscador, tiene una referencia a una referencia a gen_K y otra a gen_H  */
    ///                                 	(393,392,"gen_K",@locDKSLocal,@ordinalDeAlta,@tiemoDeAlta,1,"",56,1),
    ///                                 	(394,392,"gen_H",@locDKSLocal,@ordinalDeAlta,@tiemoDeAlta,1,"",59,1),
    ///                                 	(415,392,"gen_st_para_FicEjecIntfKLW_funcionKEE_ActivadoBoton_BuscadorKee_por_key_host","conceptos_sin_techo",@ordinalDeAlta,@tiemoDeAlta,1,"",45,2),
    ///                                 En este caso el pulsado del boton es accesible directamente
    ///                                 
    ///                 - SOlo conocemos un conjunto especifico de funciones que se pueden ejecurar. Que eson :
    ///                 8.1.) "funcionKEE_ActivadoBoton_BuscadorKee_por_key_host"
    ///                     - Este componente de script "SctExpandirEvi" es un componente del evi sintecho que esta haciendo de boton. 
    ///                             El key y el host del concepto que queremos solicitar, estan en sendos descendientes sin techo de los evis (uno instancia de Key y otro instancia de host)
    ///                         que comparten muro con este evi que esta haciendo de boton
    ///                             Para conseguir el key y el host del conceto a buscar, tengo tomarlo de dichos evis sin techo
    ///                     8.1.1.) Obtenemos los valore de key y host que tenemos que buscar
    ///                     8.1.2.) solicitamos en el host el getdetails correspondiente al cocepto que buscamos. Para ello vale con generar el evi correspondiente. La 
    ///                               creacciondel evi ya se encarga de ir a por el getDetails
    ///                 8.2.) iremos completando con otras funciones KEE
    ///                 ...
    ///
    /// 
    /// </summary>
    public void expandeSinTecho()
    {
        // Tomamos el string asociado al dato (contenido del ndodo "T"
        string T_DeSinTecho = this.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild.LastChild.InnerXml;

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log("- Desde 4_482  SctExpandirEvi => expandeSinTecho(); con tipo de dato TEXTO PLANO  con key = "
        + this.GetComponent<ScriptCtrlBaseDeEvi>().key
        + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
        + " - Dato sin techo T = " + T_DeSinTecho);
        }

        // Segun el tipo de dato actuamos en consecuencia

        //  1.)   "gen_tipoDeSinTechoTextoPlano"
        //  TEXTO PLANO como tipo de dato sin techo => Se abre el texto (contenido en el elemento T hijo de Z de KDL) en el cambas general
        if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_host))
        {
            // Ponemos el texto en el panel general de texto del canvas
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PanelCanvasCompleto.SetActive(true);
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextCanvasGeneralCompleto.text = T_DeSinTecho;

            EditorGUIUtility.systemCopyBuffer = T_DeSinTecho;  // Con esta sentencia, hacemos un "copy" el contenido de texto del sintecho, para poder hacer un "paste" donde queramos

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("- Desde 4_493  SctExpandirEvi => expandeSinTecho(); con tipo de dato TEXTO PLANO  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho);
            }

        }
        //  2.)   "gen_tipoDeSinTecho_NumeroEntero"
        //  NUMERO ENTERO (numero entero) como tipo de dato sin techo = => Se abre el texto con el numero entero que contiene (contenido en el elemento T hijo de Z de KDL) en el cambas general
        else if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTecho_NumeroEntero_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTecho_NumeroEntero_host))
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("- Desde  4_504  SctExpandirEvi => expandeSinTecho(); con tipo de dato URL  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho);
            }

        }
        //  3.)   "gen_tipoDeSinTecho_Url"
        //  URL (localizacion en internet) como tipo de dato sin techo = => Se abre la url (contenida como texto en el elemento T hijo de Z de KDL) en el navegador por defecto del sistema
        else if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTecho_Url_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTecho_Url_host))
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("- Desde  4_504  SctExpandirEvi => expandeSinTecho(); con tipo de dato URL  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho);
            }

            string urlAAbrir = T_DeSinTecho;
            Application.OpenURL(urlAAbrir);

        }
        // 4.)   "gen_tipoDeSinTecho_FicheroGenerico"  (no esta programado PENDIENTE MAFG 2021-11-26)
        //     4.1.) Obtenemos la extension del fichero
        //     4.2.) Segun el tipo de extensión actuamos en consecuencia
        else if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroGenerico_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTecho_FicheroGenerico_host))
        {
            string pathAAbrir = Usuario.GetComponent<ScriptGestionaDAUS>().Dks_madre_host + "/" + T_DeSinTecho;
            if (DatosGlobal.niveDebug > 50)
            {
                Debug.Log("- Desde  SctExpandirEvi => expandeSinTecho() => gen_tipoDeSinTecho_FicheroGenerico ; con tipo de dato URL  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - pathAAbrir = " + pathAAbrir);
            }
            Application.OpenURL(pathAAbrir);

        }
        // 5.)   "gen_tipoDeSinTecho_FicheroAudio"  (no esta programado PENDIENTE MAFG 2021-11-26)
        //     5.1.) Obtenemos la extension del fichero
        //     5.2.) Segun el tipo de extensión actuamos en consecuencia
        // 6.)   "gen_tipoDeSinTecho_FicheroImagen"  (no esta programado PENDIENTE MAFG 2021-11-26)
        //     6.1.) Obtenemos la extension del fichero
        //     6.2.) Segun el tipo de extensión actuamos en consecuencia
        else if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroImagen_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTecho_FicheroImagen_host))
        {
            string pathImgAAbrir = Usuario.GetComponent<ScriptGestionaDAUS>().Dks_madre_host + "/" + T_DeSinTecho;
            if (DatosGlobal.niveDebug > 50)
            {
                Debug.Log("- Desde  SctExpandirEvi => expandeSinTecho() => gen_tipoDeSinTecho_FicheroImagen; con tipo de dato URL  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - pathImgAAbrir = " + pathImgAAbrir);
            }
            Application.OpenURL(pathImgAAbrir);

        }
        // 7.)   "gen_tipoDeSinTecho_FicheroVideo"  (no esta programado PENDIENTE MAFG 2021-11-26)
        //     7.1.) Obtenemos la extension del fichero
        //     7.2.) Segun el tipo de extensión actuamos en consecuencia

        // 8.)   "gen_tipoDeSinTecho_LlamadaFuncionKEE" : 
        else if ((this.GetComponent<ScriptCtrlBaseDeEvi>().key == ConceptosConocidos.gen_tipoDeSinTecho_LlamadaFuncionKEE_Key) &&
            (this.GetComponent<ScriptCtrlBaseDeEvi>().host == ConceptosConocidos.gen_tipoDeSinTecho_LlamadaFuncionKEE_host))
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("- Desde SctExpandirEvi => expandeSinTecho(); en inicio de llamada a funcion KEE  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho
                + " - ConceptosConocidos.funcionKEE_ActivadoBoton_BuscadorKee_por_key_host = " + ConceptosConocidos.funcionKEE_ActivadoBoton_BuscadorKee_por_key_host);
            }
            // Vemos cual es la funcion que hay que ejecutar y la llamamos
            // 8.1.) "funcionKEE_ActivadoBoton_BuscadorKee_por_key_host"
            if (T_DeSinTecho == ConceptosConocidos.funcionKEE_ActivadoBoton_BuscadorKee_por_key_host)
            {
                Debug.Log("- Desde SctExpandirEvi => expandeSinTecho(); en llamada a funcion KEE (funcionKEE_ActivadoBoton_BuscadorKee_por_key_host) con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho 
                + " - ConceptosConocidos.funcionKEE_ActivadoBoton_BuscadorKee_por_key_host = " + ConceptosConocidos.funcionKEE_ActivadoBoton_BuscadorKee_por_key_host);

                // 8.1.1.) Obtenemos los valore de key y host que tenemos que buscar

                // string keyDeBusqueda = "gen_ventana";
                // string keyDeBusqueda = "gen_mikiDatosLudicos";
                // string keyDeBusqueda = "gen_miCasa";
                // string keyDeBusqueda = "gen_miki";
                string keyDeBusqueda = "gen_prueba_001";
                string hostDeBusqueda = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_desarrollo";  // Para que se adapte al host en el que se trabaja
                string cualificadorDeBusqueda = "0";
                string ordinalConfDeBusqueda = "0";
                DateTime ultiModConfDeBusqueda = new DateTime(0);
                GameObject muroDeBotonYDatosDeBusqueda = this.gameObject.transform.parent.gameObject;
                GameObject muroDestino = null;

                // El mismo muro donde esta el boton, es el muro donde estan la instancia de key y de host que contienen los sin techo en donde estan los datos del key y 
                // el host del concepto del que tenemos que pedir el getDetails

                int numMaxElemAAnalizar = 20;  // definimos un numero maximo de elementos a analizar por si caemos en bucle o algo
                    // Buscamos entre los hijos del muro los evis que instancien o referencies al concepto "key". Este contendra un Sin techo, con el key del concepto a buscar
                ClassListaDeEvis listaDeEvisKey = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameEvisEnMuroPorKeyHost(muroDeBotonYDatosDeBusqueda, ConceptosConocidos.gen_K_key, ConceptosConocidos.gen_K_host);
                        // Solo debe haber un elemento en la lista
                GameObject eviDeKey = listaDeEvisKey.listaDeEvis[0];
                keyDeBusqueda = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameElem_T_DelPrimerSinTecho_G_elem(eviDeKey, numMaxElemAAnalizar);

                // Buscamos entre los hijos del muro los evis que instancien o referencies al concepto "host". Este contendra un Sin techo, con el host del concepto a buscar
                ClassListaDeEvis listaDeEvisHost = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameEvisEnMuroPorKeyHost(muroDeBotonYDatosDeBusqueda, ConceptosConocidos.gen_H_key, ConceptosConocidos.gen_H_host);
                        // Solo debe haber un elemento en la lista
                GameObject eviDeHost = listaDeEvisHost.listaDeEvis[0];
               hostDeBusqueda = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameElem_T_DelPrimerSinTecho_G_elem(eviDeHost, numMaxElemAAnalizar);

                // 8.1.2.) solicitamos en el host el getdetails correspondiente al cocepto que buscamos. Para ello vale con generar el evi correspondiente. La 
                //          creacciondel evi ya se encarga de ir a por el getDetails

                // Primeramente convertimos el muro del evi que es la raiz de la búsqueda en muro activo, para que de esta manera, el nuevo evi que se genere con
                // el concepto que estamos solicitando al DKS, se genere en eel mismo muro donde esta el evi raiz de la busqueda

                //               EviBase.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);
                int profundidadDeBusqueda = 10;
                GameObject cabezaDeEdicion = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().dameCabezaDeEdicion(this.gameObject, profundidadDeBusqueda);
                if (cabezaDeEdicion != null)
                {
                    // Si hay un gameobjet que es cabeza de edicion, el muro en el que esta sera su padre y este es el muro que debemos poner como muro activo
                    muroDestino = cabezaDeEdicion.gameObject.transform.parent.gameObject;
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = muroDestino;
                    Usuario.GetComponent<ScriptCtrlUsuario>().relocalizaUsuarioEnMuro(muroDestino);
                }

                // - Llamamos al generador de evi fractal de referencia  "generaEviFractalRef()"
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(keyDeBusqueda, hostDeBusqueda, cualificadorDeBusqueda, ordinalConfDeBusqueda, ultiModConfDeBusqueda, muroDestino);

            }
            // 8.2.) iremos completando con otras funciones KEE
            // ...
            // else if ((T_DeSinTecho == ConceptosConocidos.otras funciones KEE futuras...))
            // {
            // }
            else
            {
                if (DatosGlobal.niveDebug > 100)
                {
                    Debug.Log("- Desde   SctExpandirEvi => expandeSinTecho(); con llamada a funcion KEE desconocida  con key = "
                    + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                    + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                    + " - Dato sin techo T (nombre de la funcion a llamar) = " + T_DeSinTecho);
                }
            }

        }
        else  // Si es de algun tipo de datos no conocodo lo tyratamos como si fuera un dato sin techo y mostramos en "TextCanvasGeneralCompleto", lo que hay en el elemento T
        {
            // Ponemos el texto en el panel general de texto del canvas
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PanelCanvasCompleto.SetActive(true);
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextCanvasGeneralCompleto.text = T_DeSinTecho;

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("- Desde   SctExpandirEvi => expandeSinTecho(); con tipo de dato desconocido  con key = "
                + this.GetComponent<ScriptCtrlBaseDeEvi>().key
                + " . y host = " + this.GetComponent<ScriptCtrlBaseDeEvi>().host
                + " - Dato sin techo T = " + T_DeSinTecho);
            }
        }  // Fin de -  else  // Si es de algun tipo de datos no conocodo lo tyratamos como si fuera un dato sin techo y mostramos en "TextCanvasGeneralCompleto", lo que hay en el elemento T


    } // Fin de - private void eliminaEsteEvi()


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
    IEnumerator esperaMuro()
    {
        // La rama que se ha generado para expandir el evi, no genera el muro donde se van a colocar los evis de la expansion, hasta un frame
        // despues de ser generada, por lo que tenemos que esperar un frame (mediante "yield return null"), para que el muro este creado, antes
        // de empezar a colgar cosas en el muro espero dos frames (dos  "yield return null"), porque no me fio de que el orden de ejecucion de los
        // gameobjet no me ejecute el de intentar colgar cosas en el muro antes del de generarlo
        yield return null;
        yield return null;

        bool yaTengoMuro = false;
        GameObject muroDestino = null;

        if (ramaAsociada.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count != 0)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in ramaAsociada.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro) // Si es una rama
                {
                    muroDestino = hijo;
                    yaTengoMuro = true;
                }  // FIn de - f (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            } // Fin de - foreach (GameObject hijo in ramaAsociada.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
        }  // Fin de - if (ramaAsociada.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count != 0)

        if (yaTengoMuro) // si tenemos muro vamos generando evis y colgandolos en el
        {
            expandeDescripcion(muroDestino);
        }
        else
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("ERROR desde SctExpandirEvi => IEnumerator esperaMuro. El muro no se ha generado."); }
        }

    }  // Fin de - IEnumerator traeTextura_imagen_AyuIntf(string origenDeLaImagen)

}  // Fin de - public class SctExpandirEvi : MonoBehaviour {
