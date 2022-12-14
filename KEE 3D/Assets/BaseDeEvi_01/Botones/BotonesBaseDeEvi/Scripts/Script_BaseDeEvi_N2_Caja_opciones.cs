using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script_BaseDeEvi_N2_Caja_opciones 
/// Gestiona la configuracion y manejo de los botones de opciones del evi base
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-xx-xx
/// Fecha :	2021-07-04 (incluyo la geation de activacion de lo botones dependiendo de la naturaleza del evi y de si esta en navegacion, edicion, tramoya...)
///		DATOS GENERALES :
///		METODOS GENERALES :
///			-
/// Observaciones :
/// 		TODOS los evis tienen este componente en la caja de opciones de su evi base
/// 		
///         CON RESPECTO AL ESTADO DE ACTIVACION DE LOS BOTONES DE OPCIONES:
///         EL estado de activacion depende de la naturalesa (tipo) y de su situacion (navegacion, edicion, tramoya...)
///              Tipo de elemento de interfaz en el que estamos (este objeto pertenece a un base de evi, que solo esta en los elementos tipo "tipoElemItf_evi"
///                 - Segun el subtipo de evi, tendrá activadas unas u otras opciones. SI no se indica nada es  (todos activo = true)
///                 - Dependiendo del entorno en el que se encuentra el evi base (muro de trabajo, de edicion, ..).
///                 
///                 1.) EN ESTADO DE NAVEGACION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_navegacion)
///                         Los estaods descritos anteriormente son los que corresponden cuando el evi se encuentra en navegacion
///                        1.1.) subTipoElemItf_evi_rama
///                                  - edicion = false; las ramas no se editan. No son conceptos
///                                  - instanciacion = false; las ramas no se instancian. No son conceptos
///                        1.2.) subTipoElemItf_evi_baseRefFractal
///                        1.3.) subTipoElemItf_evi_baseInstFractal
///                               - instanciacion = false; las instancias no se instancian. Ya son instancias, pueden clonarse o 
///                                             OJO : si se editarse (si se editan pasan a ser un concepto culla descripcion es en principio esa unica instancia)
///                        1.4.) subTipoElemItf_evi_baseSinTecho_00
///                               - expandir = false; los sin techo no se pueden expandir ya que no tienen descripcion
///                                              OJO : si se edita permite cambiar el contenido y el tipo de alguna manera
///                                              OJO : si se instancia genera un nuevo sin techo, vacio, pero con el mismo tipo de dato que el original
///                        1.5.) subTipoElemItf_evi_buscador_00
///                                              OJO : expande; permite acceder a los filtros y a los resultados
///                                              OJO : edita; permite el acceso al manejo de filtros, indexaciones de soluciones, etc
///                                              OJO : clona; genera un buscador igual que el actual
///                                              OJO : instancia; abre un nuevo buscador, del mismo tipo pero vacio
///                        1.6.) subTipoElemItf_evi_lista_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        1.7.) subTipoElemItf_evi_camino_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        1.8.) subTipoElemItf_evi_arbol_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        1.9.) subTipoElemItf_evi_EviPrue_001 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        1.10.) subTipoElemItf_evi_EviRefElemen (YA veremos PENDIENTE MAFG 2021-07-04)
///                 
///                 2.) EN ESTADO DE EDICION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_edicion)
///                        2.1.) subTipoElemItf_evi_rama (el evi de rama no existe en modo edicion)
///                        2.2.) subTipoElemItf_evi_baseRefFractal
///                               - expandir = false; ; no se puede expandirse en edicion (es solo una referencia a un concepto dentro de una descripcion)
///                               - edicion = false; no se puede expandirse en edicion (es solo una referencia a un concepto dentro de una descripcion. Para editarlo habria que instanciarlo
///                                              OJO : clona; genera otra referencia al mismo concepto que aparecera tambien en la descripcion
///                                              OJO : instancia; abre una instancia nueva del concepto. Sera una instancia al concepto en la descripcion del KDL que nos ocupa
///                        2.3.) subTipoElemItf_evi_baseInstFractal
///                               - expandir = false; ; no se puede expandirse en edicion (solo puede editarse para manejar la descripcion de la inmstancia en KDL)
///                               - instanciacion = false; las instancias no se instancian. Ya son instancias, pueden clonarse o 
///                                             OJO : al editarse se genera un subarbol que permite definir la descripcion de la instanciacion en la estructura KDL
///                        2.4.) subTipoElemItf_evi_baseSinTecho_00
///                                  - expandir = false; los sin techo no se pueden expandir ya que no tienen descripcion
///                                              OJO : si se edita permite cambiar el contenido y el tipo de alguna manera
///                                              OJO : clona; genera un sin techo igual que el actual
///                                              OJO : si se instancia genera un nuevo sin techo, vacio, pero con el mismo tipo de dato que el original
///                        2.5.) subTipoElemItf_evi_buscador_00 (el evi de busqueda no existe en modo edicion)
///                        2.6.) subTipoElemItf_evi_lista_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        2.7.) subTipoElemItf_evi_camino_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        2.8.) subTipoElemItf_evi_arbol_00 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        2.9.) subTipoElemItf_evi_EviPrue_001 (YA veremos PENDIENTE MAFG 2021-07-04)
///                        2.10.) subTipoElemItf_evi_EviRefElemen (YA veremos PENDIENTE MAFG 2021-07-04)
///                        
///                 3.) EN ESTADO DE EN TRAMOYA : ( en "ScriptDatosElemenItf"  modo = modoElemItf_enTramoya)
///                         - En todos los tipos de evi, cuando estan en la tramoya, el unico boton que queda operativo es el de eliminar
///  
///                 4.) EN ESTADO DE CABEZA DE EDICION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_cabezaEdicion)
///                         - Este estado es solo para los evis que son cabeza de edicion, osea un evi en edicion en un muro de navegacion. De estos 
///                         evis cuelga el arbol de edicion del concepto
///
/// </summary>
public class Script_BaseDeEvi_N2_Caja_opciones : MonoBehaviour {

    public GameObject ctrlInterfaz;
    private GameObject objeto_Evi_Raiz;  // Es el objeto base del evi en el que radican estos botones

    // Declaramos los game objet de los botnes que nos haran falta
    public GameObject Btn_BaseDeEvi_N1_Opciones;
    // Botones hijos de N2_Caja_opciones
    // Btones de opciones
    public GameObject Btn_Evi_op_Expandir;
    public GameObject Btn_Evi_op_Editar;
        public GameObject Btn_Evi_op_EnEdicionGrabar;
        public GameObject Btn_Evi_op_EnEdicionSalir;
    public GameObject Btn_Evi_op_Clonar;
    public GameObject Btn_Evi_op_Instanciar;
    public GameObject Btn_Evi_op_Eliminar;

    // Vamos definiendo escalas y posiciones

        // Para el Evi_Caja_opciones Que contiene al resto
    public Vector3 escalaBtn_Evi_Caja_opciones;
    public Vector3 escalaBtn_Evi_Caja_opcionesActivado;
    public Vector3 positionBtn_Evi_Caja_opciones;

        // Para los botones dentro de la caja de popciones
    public Vector3 escalaBtn_BaseDeEvi_N2_1_Desactivado;  // El tamaño es el mismo para todos los Btn_BaseDeEvi_N2_1
    public Vector3 escalaBtn_BaseDeEvi_N2_1_Activado;
            // Posicion de cada boton
    public Vector3 positionBtn_Evi_op_Expandir;
    public Vector3 positionBtn_Evi_op_Editar;
    public Vector3 positionBtn_Evi_op_Clonar;
    public Vector3 positionBtn_Evi_op_Instanciar;
    public Vector3 positionBtn_Evi_op_Eliminar;

        // Describimos los botones
    public int numeroBotonesOpciones = 5;

    // definimos tamaños de los botones
    public float tamañoAnchoBotonInactivo;
    //        float tamañoAltoBotonInactivo = 1/(numeroBotonesOpciones*2);
    public float tamañoAltoBotonInactivo;
    public float tamañoAnchoBotonActivo;
    public float tamañoAltoBotonActivo;

    public float posicionAlto;
    public float posicionAncho;
    public Vector3 posicionBoton;

    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
    }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////
        /// Metodo : Start () 
        /// Autor : Miguel Angel Fernandez Graciani
        /// Fecha creacion : 2020-xx-xx
        /// Ultima modificacion :
        ///         - 2021-07-04 (incluyo configuracion de botones de opciones dependiendo de naturaleza y modo del evi)
        /// Variables de entrada :
        /// Variables de salida :
        /// Observaciones:
        ///     - Ver observaciones de esta clase
        /// 
        /// </summary>
        void Start () {

        // Estamos en "Btn_BaseDeEvi_N2_Caja_opciones" hijo de "Btn_BaseDeEvi_N1_Opciones"  hijo de "BaseDeEvi_01"
        objeto_Evi_Raiz = transform.parent.parent.gameObject;
        Btn_BaseDeEvi_N1_Opciones = transform.parent.gameObject;  // El boton de opciones hara falta para configurar elestodo de estos, segun el modo OJOO NO FUNCIONA Y NO SE POR QUE **111***

        // Tomamos los botones y los asignamos como objetos para poder direccionarlos desde este script
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if (child.name == "Btn_BaseDeEvi_N2_1_op_Expandir") { Btn_Evi_op_Expandir = child; }
            else if (child.name == "Btn_BaseDeEvi_N2_1_op_Editar") { Btn_Evi_op_Editar = child; }
            else if (child.name == "Btn_BaseDeEvi_N3_1_op_Editar_Grabar") { Btn_Evi_op_EnEdicionGrabar = child; }
            else if (child.name == "Btn_BaseDeEvi_N3_1_op_Editar_Salir") { Btn_Evi_op_EnEdicionSalir = child; }
            else if (child.name == "Btn_BaseDeEvi_N2_1_op_Clonar") { Btn_Evi_op_Clonar = child; }
            else if (child.name == "Btn_BaseDeEvi_N2_1_op_Instanciar") { Btn_Evi_op_Instanciar = child; }
            else if (child.name == "Btn_BaseDeEvi_N2_1_op_Eliminar") { Btn_Evi_op_Eliminar = child; }
            //Do something with child
        }


        // definimos tamaños de los botones
        float tamañoAnchoBotonInactivo = 0.5f;
        //        float tamañoAltoBotonInactivo = 1/(numeroBotonesOpciones*2);
        float tamañoAltoBotonInactivo = 1f / (numeroBotonesOpciones * 2f);
        float tamañoAnchoBotonActivo = 1f;
        float tamañoAltoBotonActivo = 1f / numeroBotonesOpciones;
        escalaBtn_BaseDeEvi_N2_1_Desactivado = new Vector3(tamañoAnchoBotonInactivo, tamañoAltoBotonInactivo, 1.1f);// En la escala (ancho, alto , profundo) Es algo menos profundo para que los botones hijos salgan por encima
        escalaBtn_BaseDeEvi_N2_1_Activado = new Vector3(tamañoAnchoBotonActivo, tamañoAltoBotonActivo, 1.1f);// En la escala (ancho, alto , profundo) Es algo menos profundo para que los botones hijos salgan por encima

        float posicionAlto = -2f;
        Vector3 posicionBoton = new Vector3(0f, posicionAlto, 0f);

        escalaBtn_Evi_Caja_opciones = new Vector3(2f, 2f * numeroBotonesOpciones, 1f);  // En la escala (ancho, alto , profundo)
        positionBtn_Evi_Caja_opciones = new Vector3(-0.5f, numeroBotonesOpciones + (1f/2f)*tamañoAnchoBotonActivo, 1f); // En posición (ancho, alto , profundo)
        this.transform.localScale = escalaBtn_Evi_Caja_opciones;
        this.transform.localPosition = positionBtn_Evi_Caja_opciones;

        // OJOOOO esto que sigue no hace nada, ya que estamos actuando sobre elprefad (que se asigno para cada gameobjet desde la interfaz) y no sobre el gameobject hijo de este evi base

       //        Btn_Evi_op_Expandir = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Expandir);  // Ya esta asociado desde el inspector
//       Btn_Evi_op_Expandir.transform.SetParent(this.transform);
        posicionAlto = (1f / numeroBotonesOpciones) * (-2f);
        posicionBoton = new Vector3(0f, posicionAlto, 0f);
        Btn_Evi_op_Expandir.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
        Btn_Evi_op_Expandir.transform.localPosition = posicionBoton;

//        Btn_Evi_op_Editar = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Editar);  // Ya esta asociado desde el inspector
//        Btn_Evi_op_Editar.transform.SetParent(this.transform);
        posicionAlto = (1f / numeroBotonesOpciones) * (-1f);
        posicionBoton = new Vector3(0f, posicionAlto, 0f);
        Btn_Evi_op_Editar.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
        Btn_Evi_op_Editar.transform.localPosition = posicionBoton;

            //        Btn_Evi_op_EnEdicionGraba = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Editar);  // Ya esta asociado desde el inspector
//            Btn_Evi_op_EnEdicionGrabar.transform.SetParent(this.transform);
            posicionAlto = (1f / numeroBotonesOpciones) * (-1f);
            posicionAncho = 1f;
            posicionBoton = new Vector3(posicionAncho, posicionAlto, 0f);
            Btn_Evi_op_EnEdicionGrabar.gameObject.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
            Btn_Evi_op_EnEdicionGrabar.gameObject.transform.localPosition = posicionBoton;
                    // Este boton " Btn_Evi_op_EnEdicionGrabar" no estara actvo a no ser que estemos en modo edicion
            Btn_Evi_op_EnEdicionGrabar.SetActive(false);

            //       Btn_Evi_op_EnEdicionSalir = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Editar);  // Ya esta asociado desde el inspector
            //            Btn_Evi_op_EnEdicionSalir.transform.SetParent(this.transform);
            posicionAlto = (1f / numeroBotonesOpciones) * (-1f);
            posicionAncho = 2f;
            posicionBoton = new Vector3(posicionAncho, posicionAlto, 0f);
            Btn_Evi_op_EnEdicionSalir.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
            Btn_Evi_op_EnEdicionSalir.transform.localPosition = posicionBoton;
                    // Este boton "Btn_Evi_op_EnEdicionSalir" no estara actvo a no ser que estemos en modo edicion
            Btn_Evi_op_EnEdicionSalir.SetActive(false);


        //        Btn_Evi_op_Clonar = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Clonar);  // Ya esta asociado desde el inspector
        //        Btn_Evi_op_Clonar.transform.SetParent(this.transform);
        posicionAlto = (1f / numeroBotonesOpciones) * (0f);
        posicionBoton = new Vector3(0f, posicionAlto, 0f);
        Btn_Evi_op_Clonar.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
        Btn_Evi_op_Clonar.transform.localPosition = posicionBoton;

        //        Btn_Evi_op_Instanciar = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Instanciar);  // Ya esta asociado desde el inspector
//        Btn_Evi_op_Instanciar.transform.SetParent(this.transform);
        posicionAlto = (1f / numeroBotonesOpciones) * (1f);
        posicionBoton = new Vector3(0f, posicionAlto, 0f);
        Btn_Evi_op_Instanciar.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
        Btn_Evi_op_Instanciar.transform.localPosition = posicionBoton;


        //        Btn_Evi_op_Eliminar = Instantiate(this.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>().Btn_Evi_op_Eliminar);  // Ya esta asociado desde el inspector
        Btn_Evi_op_Eliminar.transform.SetParent(this.transform);
        posicionAlto = (1f / numeroBotonesOpciones) * (2f);
        posicionBoton = new Vector3(0f, posicionAlto, 0f);
        Btn_Evi_op_Eliminar.transform.localScale = escalaBtn_BaseDeEvi_N2_1_Desactivado;
        Btn_Evi_op_Eliminar.transform.localPosition = posicionBoton;

        // //////////////////////////////////////////////////////////////
        // //////////////////////////////////////////////////////////////
        // Inicilizamos la activacion de cada boton, dependiendo de su naturaleza
        configuraBotonesOpcionesEvi(objeto_Evi_Raiz.gameObject.GetComponent<ScriptDatosElemenItf>().subTipoElementIntf, objeto_Evi_Raiz.gameObject.GetComponent<ScriptDatosElemenItf>().modo);

    }  //  Fin de - void Start () {

    // Update is called once per frame
    void Update () {

    }  // FIn de - void Update () {

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  private void eliminaEsteEvi()
    /// Observaciones : Elimina el evi, quitandolo tambien de "ListaEvis" y del DAUS
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-01-30
    /// Ultima modificacion :
    ///     - 2022-11-18. He incluido la funcionalidad de los botones asociada a los evis de referencia a elemento de interfaz mediante las lineas (son varias)
    ///                         else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo obtiene el valor de los datos estandar del concepto
    /// OBSERVACIONES :
    ///     - Este metodo ajusta la configuracion de los botones que hay dentro de la caja de opciones del menu de opciones del evi base.
    ///     - Esta configuracion se hace segun corresponda a la naturaleza (sutipo) del evi y a su modo (navegacion, edicion, tramoya,..)
    ///     - Ver observaciones de esta clase
    /// </summary>
    public void configuraBotonesOpcionesEvi(string subTipoElementIntf, string modo)
    {
        // CON RESPECTO AL ESTADO DE ACTIVACION DE LOS BOTONES DE OPCIONES:
        // EL estado de activacion depende de la naturalesa (tipo) y de su situacion (navegacion, edicion, tramoya...)

        if (modo == ScriptDatosElemenItf.modoElemItf_navegacion) // 1.) EN ESTADO DE NAVEGACION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_navegacion)
        {
            if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            {
                // 1.1.) subTipoElemItf_evi_rama
                Btn_Evi_op_Expandir.SetActive(true); 
                Btn_Evi_op_Editar.SetActive(false);  // edicion = false; las ramas no se editan. No son conceptos
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // edicion = false; las ramas no se editan. No son conceptos
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // edicion = false; las ramas no se editan. No son conceptos
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(false);  // instanciacion = false; las ramas no se instancian. No son conceptos
                Btn_Evi_op_Eliminar.SetActive(true);

            }  // Fin de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            {
                //  1.2.) subTipoElemItf_evi_baseRefFractal
                Btn_BaseDeEvi_N1_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpciones; 
                Btn_Evi_op_Expandir.SetActive(true);
                Btn_Evi_op_Editar.SetActive(true);
                Btn_Evi_op_Editar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnEditar;
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(false);  // Para instanciar un evi de referencia hay que llevarlo a un muro en edicion (en un muro de navegacion, ¿para que quiero editarlo?
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            {
                // 1.3.) subTipoElemItf_evi_baseInstFractal
                Btn_Evi_op_Expandir.SetActive(true);
                Btn_Evi_op_Editar.SetActive(false);  //  Una instancia no se edita en un muro de navegacion .. OJO PENDIENTE MAFG 2021-11-24: si se editarse (si se editan pasan a ser un concepto culla descripcion es en principio esa unica instancia)
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(true);  // se genera un evi de referencia con el concepto que instancia
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            {
                // 1.4.) subTipoElemItf_evi_baseSinTecho_00
                Btn_Evi_op_Expandir.SetActive(true);  // expandir = true; Abre el contenido del sintecho. en el navegador, si es una url, o con la aplicacion que corresponda si es una imagen, documento, etc...
                Btn_Evi_op_Editar.SetActive(false);  // El sin techo, no se edita en navegacion ... OJO : si se edita permite cambiar el contenido y el tipo de alguna manera
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(false);  // PENDIENTE 2021-11-14 . OJO : si se instancia genera un nuevo sin techo, vacio, pero con el mismo tipo de dato que el original 
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            {
                // 1.5.) subTipoElemItf_evi_buscador_00
                Btn_Evi_op_Expandir.SetActive(true);  // OJO : expande; permite acceder a los filtros y a los resultados
                Btn_Evi_op_Editar.SetActive(true);  // OJO : edita; permite el acceso al manejo de filtros, indexaciones de soluciones, etc
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);  // OJO : clona; genera un buscador igual que el actual
                Btn_Evi_op_Instanciar.SetActive(true);  // OJO : instancia; abre un nuevo buscador, del mismo tipo pero vacio
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
            {
                // 1.5.) subTipoElemItf_evi_buscador_00
                Btn_Evi_op_Expandir.SetActive(true);  // OJO : expande; permite acceder a los filtros y a los resultados
                Btn_Evi_op_Editar.SetActive(true);  // OJO : edita; permite el acceso al manejo de filtros, indexaciones de soluciones, etc
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);  // OJO : clona; genera un buscador igual que el actual
                Btn_Evi_op_Instanciar.SetActive(false);  // OJO : instancia; abre un nuevo buscador, del mismo tipo pero vacio
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
            else
            {
                // 1.6.) subTipoElemItf_evi_lista_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                // 1.7.) subTipoElemItf_evi_camino_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                // 1.8.) subTipoElemItf_evi_arbol_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                // 1.9.) subTipoElemItf_evi_EviPrue_001 (YA veremos PENDIENTE MAFG 2021-07-04)
                // 1.10.) subTipoElemItf_evi_EviRefElemen (YA veremos PENDIENTE MAFG 2021-07-04)

                // Como estan sin definir, los ponemos todos por defecto a true
                Btn_Evi_op_Expandir.SetActive(true);
                Btn_Evi_op_Editar.SetActive(true);
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(true);
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else

        }  // FIn de - if (modo == ScriptDatosElemenItf.modoElemItf_navegacion)
        else if (modo == ScriptDatosElemenItf.modoElemItf_edicion) // 2.) EN ESTADO DE EDICION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_edicion)
        {
            // if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            // {
                // 2.1.) subTipoElemItf_evi_rama (el evi de rama no existe en modo edicion)
            // }  // Fin de - if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            {
                // 2.2.) subTipoElemItf_evi_baseRefFractal
                Btn_BaseDeEvi_N1_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;

                Btn_Evi_op_Expandir.SetActive(false);  // expandir = false; ; no se puede expandirse en edicion (es solo una referencia a un concepto dentro de una descripcion)
                Btn_Evi_op_Editar.SetActive(false);  // edicion = false; no se puede editar en edicion (es solo una referencia a un concepto dentro de una descripcion. Para editarlo habria que instanciarlo
                Btn_Evi_op_Editar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnEditarEnEdicion;
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);  // OJO : clona; genera otra referencia al mismo concepto que aparecera tambien en la descripcion
                Btn_Evi_op_Instanciar.SetActive(true);  // OJO : instancia; abre una instancia nueva del concepto. Sera una instancia al concepto en la descripcion del KDL que nos ocupa
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            {
                // 2.3.) subTipoElemItf_evi_baseInstFractal
                Btn_Evi_op_Expandir.SetActive(false);  // expandir = false; ; no se puede expandirse en edicion (solo puede editarse para manejar la descripcion de la inmstancia en KDL)
                Btn_Evi_op_Editar.SetActive(true);  // OJO : al editarse se genera un subarbol que permite definir la descripcion de la instanciacion en la estructura KDL
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);
                Btn_Evi_op_Instanciar.SetActive(true);  // se genera una referencia al concepto que instancia
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            {
                // 2.4.) subTipoElemItf_evi_baseSinTecho_00
                Btn_Evi_op_Expandir.SetActive(true);  // expandir = true; Abre el contenido del sintecho. en el navegador, si es una url, o con la aplicacion que corresponda si es una imagen, documento, etc...
                Btn_Evi_op_Editar.SetActive(true);  // OJO : si se edita permite cambiar el contenido y el tipo de alguna manera
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true); // OJO : clona; genera un sin techo igual que el actual
                Btn_Evi_op_Instanciar.SetActive(true);  // OJO : si se instancia genera un nuevo sin techo, vacio, pero con el mismo tipo de dato que el original
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
            {
                // 1.5.) subTipoElemItf_evi_buscador_00
                Btn_Evi_op_Expandir.SetActive(true);  // OJO : expande; permite acceder a los filtros y a los resultados
                Btn_Evi_op_Editar.SetActive(true);  // OJO : edita; permite el acceso al manejo de filtros, indexaciones de soluciones, etc
                Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
                Btn_Evi_op_Clonar.SetActive(true);  // OJO : clona; genera un buscador igual que el actual
                Btn_Evi_op_Instanciar.SetActive(false);  // OJO : instancia; abre un nuevo buscador, del mismo tipo pero vacio
                Btn_Evi_op_Eliminar.SetActive(true);
            }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_EviRefElemen)
               // else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
               // {
               // 2.5.) subTipoElemItf_evi_buscador_00; No existe en el modo edicion
               // }  // Fin de - else if (subTipoElementIntf == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            else
            {
                ///                        2.5.) subTipoElemItf_evi_buscador_00 (el evi de busqueda no existe en modo edicion)
                ///                        2.6.) subTipoElemItf_evi_lista_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                ///                        2.7.) subTipoElemItf_evi_camino_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                ///                        2.8.) subTipoElemItf_evi_arbol_00 (YA veremos PENDIENTE MAFG 2021-07-04)
                ///                        2.9.) subTipoElemItf_evi_EviPrue_001 (YA veremos PENDIENTE MAFG 2021-07-04)
                ///                        2.10.) subTipoElemItf_evi_EviRefElemen (YA veremos PENDIENTE MAFG 2021-07-04)
            }  // Fin de - else

        }  // FIn de - else if (modo == ScriptDatosElemenItf.modoElemItf_edicion)
        else if (modo == ScriptDatosElemenItf.modoElemItf_enTramoya)
        {
            // 3.) EN ESTADO DE EN TRAMOYA : ( en "ScriptDatosElemenItf"  modo = modoElemItf_enTramoya)
            // - En todos los tipos de evi, cuando estan en la tramoya, el unico boton que queda operativo es el de eliminar
            Btn_Evi_op_Expandir.SetActive(false);
            Btn_Evi_op_Editar.SetActive(false);
            Btn_Evi_op_EnEdicionGrabar.SetActive(false);  // Se activara al entrar en edicion
            Btn_Evi_op_EnEdicionSalir.SetActive(false);  // Se activara al entrar en edicion
            Btn_Evi_op_Clonar.SetActive(false);
            Btn_Evi_op_Instanciar.SetActive(false);
            Btn_Evi_op_Eliminar.SetActive(true);

        }  // FIn de - else if (modo == ScriptDatosElemenItf.modoElemItf_enTramoya)
        else if (modo == ScriptDatosElemenItf.modoElemItf_cabezaEdicion)
        {
            // 4.) EN ESTADO DE CABEZA DE EDICION : ( en "ScriptDatosElemenItf"  modo = modoElemItf_cabezaEdicion)
            // - Este estado es solo para los evis que son cabeza de edicion, osea un evi en edicion en un muro de navegacion. De estos 
            // evis cuelga el arbol de edicion del concepto
            Btn_Evi_op_Expandir.SetActive(false); // Un evie en edicion, no puede expandirse, ya esta expandido en edicion
            Btn_Evi_op_Editar.SetActive(true);
            Btn_Evi_op_EnEdicionGrabar.SetActive(true);  // Se activara al entrar en edicion
            Btn_Evi_op_EnEdicionSalir.SetActive(true);  // Se activara al entrar en edicion
            Btn_Evi_op_Clonar.SetActive(true); 
            Btn_Evi_op_Instanciar.SetActive(true);
            Btn_Evi_op_Eliminar.SetActive(true);

        }  // FIn de - else if (modo == ScriptDatosElemenItf.modoElemItf_cabezaEdicion)
        else
        {

        }  // FIn de - else


    } // Fin de - private void eliminaEsteEvi()


/// ///////////////////////////////////
/// ///////////////////////////////////
/// Vamos con los trigers
/// <param name="other">Other.</param>

// Cuando llega a el el puntero de usuario
void OnTriggerEnter(Collider other)
    {
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño
        this.transform.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_Caja_opciones = true;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        this.transform.parent.GetComponent<Script_BaseDeEvi_N1_Opciones>().DentroDeBtn_Evi_Caja_opciones = false;
    } // Fin de - void OnTriggerEnter(Collider other) 
}
