using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////  nombre_enKee
/// ///////////  Script de datos generales para tpdps los elementos de interfaz
/// Construye y gestiona la parte general de todos los EVIs
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-02-07
/// Observaciones :
/// 		TODOS los elementos de interfaz tienen unos datos básicos que tienen en todos la misma denominación.
/// 		Este script es el contenedor de estos datos
/// 		Todos los elementos de interfaz deben incorporar este componente
/// </summary>
public class ScriptDatosElemenItf : MonoBehaviour {


    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;
    public GameObject Solicitudes;


    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // DATOS PROPIOS DE TODOS LOS ELEMENTOS DE INTERFAZ
    //      Desde aqui****, debe repetirse en (ScriptCtrlBaseDeEvi, ScriptCtrlMuroTrabajo (y en el de las ramas cuando lo genere))
    //      SI se modifica aqui hay que modificar los otros
    // Los tipos de elementos de interfaz estan en "ScriptDatosInterfaz => TIPOS DE ELEMENTOS DE INTEFAZ"
    public int idElementIntf;  // Es el identificador unico de todos los elementos de la interzfaz. Se consigue desde "generaIdElementIntf()"
    public string tipoElementIntf;      // identifica el tipo de elemento de interfaz al que corresponde cada elemento de interfaz
    public int idElementEnTipo;  // Es el identificador unico de todos los elementos de un mismo tipo. Se consigue desde "generaIdElementEnTipo()"
    public string subTipoElementIntf;   // identifica el tipo de elemento de interfaz al que corresponde cada elemento de interfaz dentro de un tipo

        // Para poder manejarnos a la hora de controlar los elementos de interfaz (borrarlos, recorrerlo, etc...) hemos generado una gerarquia que 
        // denominamos GERARQUIA DE ELEMENTOS. Esta gerarquia se define mediante las listas que se ven a continuacion
        // OJOOO hay otra gerarquia en el sistema que es la que define unity entre sus game objets (la denominaremos GERARQUIA DE OBJETOS), PERO ESTA ES OTRA GERARQUIA DISTINTA

        // - GERARQUIA DE ELEMENTOS (G_elem) : La que definimos mediante la lista de hijos "listaDeHijos" que se declara a continuacion
        // - GERARQUIA DE OBJETOS (G_obj) : la que se define en la gerarquia de objetos de Unity
    public List<GameObject> listaDeHijos;  // Es una lista que contiene sus hijos de primer nivel
                                           // Si es rama, seran muros (OJOOO el usuario puede ser hijo de una rama)
                                           // Si es muro, los hijos seran evis
    public List<GameObject> listaDePadres;  // Es una lista que contiene los padres que me tienen como hijo en "listaDeHijos"
                                            // En principio solo debe haber uno, pero no quiero perder la posibilidad de hacerlos multiples
    public int ordinalHijoYTipo; // Es el ordinal del elemento en la lista de hijos (OJOOO hay una secuencia de ordinal para cada tipo de hijo, evis, muros, ramas, etc...)

    protected DateTime horaDeGenesis;  // Es el momento en el que se genero el elemento de interfaz

    public List<GameObject> misSolicitudesPendientes; // Son las solicitudes que el elemento de inerfaz debe atender. 




    // ///////////////////////////////////////////////////////////////////
    //      VALORES DE TIPO de elemento de interfaz que es : "tipoElementIntf" ( ver "ScriptDatosInterfaz")
    //          - Tdodos los elementos de interfaz que tienen una minima entidad (usuario, muros, camara, evis, celementos contenidos por los evis, agentes, etc...) tienen
    //          un tipo y un subtipo.
    // 
    //      los subtipos se definen el el componente de script propio de cada tipo
    // Los posibles tipos de elemento de interfaz van en cada elemento de interfaz (rama, muro o evi) y se anotan el las variables :
    // int idElementIntf;  // Es el identificador unico de todos los elementos de la interzfaz. Se consigue desde "generaIdElementIntf()"
    // string tipoElementIntf;      // identifica el tipo de elemento de interfaz al que corresponde cada elemento de interfaz
    // int idElementEnTipo;  // Es el identificador unico de todos los elementos de un mismo tipo. Se consigue desde "generaIdElementEnTipo()"
    // string subTipoElementIntf;   // identifica el tipo de elemento de interfaz al que corresponde cada elemento de interfaz dentro de un tipo
    // Los posibles tipos de elemento son :

    //   TIPOS DE ELEMENTOS DE INTERFAZ
    //   ctrlInterfaz (Sin identificador porque hay solo uno)
    //   camara
    //   focoLuz
    //   usuario (Sin identificador porque hay solo uno)
    //   puntero
            public static string tipoElemItf_usuario = "usuario";  //   usuario  (de momento solo hay uno)
            public static string tipoElemItf_rama = "rama";  //   rama  (existe lista de elementos)
            public static string tipoElemItf_muro = "muro";  //   muro  (existe lista de elementos)
            public static string tipoElemItf_evi = "evi"; //   evi  (existe lista de elementos) (todos comparten la Base de evi)
            public static string tipoElemItf_agente = "agente"; // agente (existe una lista de elementos)
            public static string tipoElemItf_solicitud = "solicitud";    //   solicitud (existe una lista de elementos)

    //   SUBTIPOS DE ELEMENTOS DE INTERFAZ : Indica la naturaleza del elemento dentro de su tipo.
    //   OJOO los subtipos de evi no estan gerarquizados, aunque si lo esten los elementos de interfaz (ej: evi base - evi contenedor  - evi fractum
    //   son padre e hijo, pero cada uno tiene su subtipo ndependiente
        // Para el TIPO EVI
                public static string subTipoElemItf_evi_sinDefinir = "sinDefinir"; //   evi qe contiene la informacion de un arbol
            // Los evis BASE, pueden tener varias naturalezas
                public static string subTipoElemItf_evi_rama = "evi_rama"; //   evi destinado al control de una rama que se genera (sin un concepto asociado, se genera por el usuario para crear otra rama de navegacion)
                public static string subTipoElemItf_evi_baseRefFractal = "evi_baseRefFractal"; //   evi destinado a visualizar un concepto mediante la estructura fractal (lo que viene siendo un getdetails)
                public static string subTipoElemItf_evi_baseInstFractal = "evi_baseInstFractal"; //   evi destinado a visualizar un instancia a un concepto mediante la estructura fractal (se obtiene de una instancia en una descripcion)
                public static string subTipoElemItf_evi_baseSinTecho_00 = "evi_baseSinTecho_00"; //   evi destinado a visualizar un sin techo (lo que viene siendo tipo de dato y dato de formulario)
                public static string subTipoElemItf_evi_buscador_00 = "evi_buscador_00"; //   evi destinado a gestionar un buscador
                public static string subTipoElemItf_evi_lista_00 = "evi_lista_00"; //   evi destinado a visualizar una lista de elementos
                public static string subTipoElemItf_evi_camino_00 = "evi_camino_00"; //   evi que contiene cainos de relacion entre conceptos
                public static string subTipoElemItf_evi_arbol_00 = "evi_arbol_00"; //   evi qe contiene la informacion de un arbol
                public static string subTipoElemItf_evi_EviPrue_001 = "EviPrue_001"; //   evi qe contiene la informacion de un arbol

                public static string subTipoElemItf_evi_EviRefElemen = "EviRefElemen"; //   evi que se utiliza en mochila, buscadores, etc para hacer referencia a distintos elementos de la interfaz

                    // Para consultas de usuario en el muro de usuario
                public static string subTipoElemItf_evi_Btn_Consulta_Si_No = "Btn_Consulta_Si_No"; //   evi que se utiliza para consultas al usuario de si o no (se colocan en el muro de usuario
                                                                                                   // ojo hay un tipo de solicitud para Btn_Consulta_Si_No, pero es para gestionar la solicitud, no 
                                                                                                   // corresponde al gameobject que aparece en el muro de usuario para que el usuario responda si o no


            // Los EVIs contenedor pueden tener varias naturalezas
                public static string subTipoElemItf_evi_contFractalSinDefinir = "evi_contFractalSinDefinir"; //   evi contenedor de la descripcion fractal dentro de un evi base (sin indicar todabia el tipo de contenido referencia, instancia o sin techo)
                public static string subTipoElemItf_evi_contFractalReferencia = "evi_contFractalReferencia"; //   evi contenedor de la descripcion fractal de referencia dentro de un evi base 
                public static string subTipoElemItf_evi_contFractalInstancia = "evi_contFractalInstancia"; //   evi contenedor de la descripcion fractal de instancia dentro de un evi base
                public static string subTipoElemItf_evi_contFractalSinTecho = "evi_contFractalSinTecho"; //   evi contenedor de la descripcion fractal sin techo dentro de un evi base

            // Los EVIs fractum pueden tener varias naturalezas
                public static string subTipoElemItf_evi_fractumSinDefinir = "evi_fractumSinDefinir"; //   evi contenedor de la descripcion fractal dentro de un evi base de subtipo  "subTipoElemItf_evi_RefFractal" (lo que viene siendo un getdetails)
                public static string subTipoElemItf_evi_fractumReferencia = "evi_fractumReferencia"; //   evi contenedor de la descripcion fractal dentro de un evi base de subtipo  "subTipoElemItf_evi_RefFractal" (lo que viene siendo un getdetails)
                public static string subTipoElemItf_evi_fractumInstancia = "evi_fractumInstancia"; //   evi contenedor de la descripcion fractal dentro de un evi base de subtipo  "subTipoElemItf_evi_RefFractal" (lo que viene siendo un getdetails)
                public static string subTipoElemItf_evi_fractumSinTecho = "evi_fractumSinTecho"; //   evi contenedor de la descripcion fractal dentro de un evi base de subtipo  "subTipoElemItf_evi_RefFractal" (lo que viene siendo un getdetails)

        // Para el TIPO SOLICITUD
                public static string subTipoSolicitud_consultaKdlADks = "consultaKdlADks"; //  solicitudKdlRespKDL : es una solicitud KDL a un DKS
                public static string subTipoSolicitud_solicitaTextura = "solicitaTextura"; //  solicitaTextura : solicita un fichero de imagen a una url  (PENDIENTE. Deberia hacerse mediante una llamada KDL (MAFG 2021-02-05)
                public static string subTipoSolicitud_solicitaAudio = "solicitaAudio"; //  solicitaAudio  : solicita un fichero de audio a una url   (PENDIENTE. Deberia hacerse mediante una llamada KDL (MAFG 2021-02-05) 
                public static string subTipoSolicitud_RespBtn_Consulta_Si_No = "solicitaRespBtn_Consulta_Si_No"; //  Se solicita a un Btn_Consulta_Si_No, que emita una precunta al usuario e informe de su respuesta
                                                                                                              // ojo hay un subtipo de vi para Btn_Consulta_Si_No, pero es para al gameobject que aparece en el muro de 
                                                                                                              //usuario para que el usuario responda si o nogestionar la solicitud, este tipo de solicitud es para que quien 
                                                                                                              // la envia, la ejecuta y la recibe sepa lo que tien eque hacer

    // ///////////////////////////////////////////////////////////////////
    //      VALORES DE MODO indica el modo en el que esta trabajando el elemento de interfaz
    public string modo;  // Indica el modo en el que esta operando el elemento de interfaz. (normalmente navegacion o edicion)
    public string modoAnterior;  // Indica el modo en el que estaba operando el elemento de interfaz en el frame anterior. (Se usa para identificar los cambios de modo y actuar en consecuencia)

        public static string modoElemItf_navegacion = "navegacion";  //   el elemento de interfaz esta operando con funcionalidad de navegacion
        public static string modoElemItf_edicion = "edicion";  //   el elemento de interfaz esta operando con funcionalidad de edicion
                                                               // Un evi esta en modo edicion cuando se esta editando. Su boton de opciones aparecera de color amarillo
                                                               // - Ojo cuando un evi esta en edicion, tiene asociada una rama en la que aparece su arbol de descripcion
                                                               // - Ojo, un concepto puede editarse en el KEE, pero solo puede grabarse en el DKS si se tiene permiso para ello en el mismo DKS
        public static string modoElemItf_cabezaEdicion = "cabezaEdicion";  // estan en este estado los evis que son la raiz de la edicion de un concpto. Estos evis son evis que pertenecen en parte a un
                                                                           // arbol de edicion (ya que son la raiz de este) y pero residen en un muro de navegacion (donde se ha solicitado la edicion
                                                                           // del concepto y donde arranca el arbol de edicion) por esto necesitamos generar un estado distinro que no es navegacion,
                                                                           // ya que pertenecen a un arbol de edicion, ni es edicion, ya que residen en un arbol de navegacion
        public static string modoElemItf_enTramoya = "enTramoya";  //   el elemento de interfaz esta alojado en la tramoya

    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");
        Solicitudes = GameObject.FindWithTag("Solicitudes");

        // Asignamos los valores propios de todos los elementos de interfaz
        // OJOOOOO el tipo "" y el subtipo "" deben ser establecidos al generar el elemento de interfaz por quien corresponda. Aqui solo se les da un valor por defecto
        idElementIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generaIdElementIntf();  // Es el identificador unico de todos los elementos de la interzfaz. Se consigue desde "dameIdElementIntf()"
        tipoElementIntf = "Sin tipo";      // identifica el tipo de elemento de interfaz al que corresponde cada elemento de interfaz
        idElementEnTipo = 0;// No podemos poner el identificador en la lista de Tipo hasta saber a que tipo oertenece. Se pone al definir el tipo mediante "generaElementEnTipo"
        subTipoElementIntf = "Sin subTipo";  // subTipoElementIntf;   El subtipo se asigna mas especificamente donde corresponde

        // Generamos la lista de Evis que va a contener
        listaDeHijos = new List<GameObject>();
        // Generamos la lista de Evis que nos contienen en su "listaDeHijos"
        listaDePadres = new List<GameObject>();

        misSolicitudesPendientes = new List<GameObject>(); // Son las solicitudes que el elemento de inerfaz debe atender. 

        horaDeGenesis = DateTime.Now;

}  // Fin de - void Awake()


// Use this for initialization
void Start () {

        if (tipoElementIntf == tipoElemItf_evi)
        {
            // Apuntamos el evi en la lista general de evis
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaEvis.Add(gameObject);
            // Apuntamos el evi en la lista de evis de su padre (normalmente en la del muro que lo contiene

        }
        else if (tipoElementIntf == tipoElemItf_muro)
        {
            // Apuntamos la rama en la lista dorrespondiente
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaMuros.Add(gameObject);
        }
        else if (tipoElementIntf == tipoElemItf_rama)
        {
            // Apuntamos la rama en la lista dorrespondiente
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaRamas.Add(gameObject);
        }
        else if (tipoElementIntf == tipoElemItf_solicitud)
        {
            // Apuntamos la rama en la lista dorrespondiente
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log(" Hemos entrado en ScriptDatosElemenItf => void Start con tipoElementIntf = " + tipoElemItf_solicitud + "- 185  "); }
//            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaSolicitudes.Add(gameObject);  Ya se hace en el awake de la solicitud
            this.transform.SetParent(Solicitudes.transform);  // Lo ponemos como hijo de Solicitudes para que no quede suelto en la jerarquia. Pero lo localizaremos
                                                                   // pos la lista correspondiente "ListaSolicitudes" y su "idElementIntf", que tendra en esa lista
        }
        else if (tipoElementIntf == tipoElemItf_agente)
        {
            // Apuntamos la rama en la lista dorrespondiente
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log(" Hemos entrado en ScriptDatosElemenItf => void Start con tipoElementIntf = " + tipoElemItf_agente + "- Por ahora sin lista, creo"); }
        }
        else
        {
            // Apuntamos la rama en la lista dorrespondiente
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log(" Hemos entrado en ScriptDatosElemenItf => void Start, llegando al ultimo else,  con tipoElementIntf = " + tipoElementIntf); }
        }
        // si el elemento de interfaz tiene padre
        if (transform.parent != null)
        {
            GameObject miPadre = transform.parent.gameObject;
            // Algunos elementos de interfaz, tienen padre pero este no tiene lista de hijos (no tienen  ScriptDatosElemenItf)
            // por lo que miramos a ver si lo tienen antes de intentar apuntarnos en la lista de hijos
            ScriptDatosElemenItf miPadreScriptDatosElemenItf = miPadre.GetComponent<ScriptDatosElemenItf>();
            // Si tiene lista de hijos, nos apuntamos en la lista
            if (miPadreScriptDatosElemenItf != null)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("lo del padre con idElementIntf = " + idElementIntf); }

                // ahijamos el elemento en su padre (en la gerarquia que no es unity)
                gestionaArbolGenealogico(miPadre, "ahija");

                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log("lo del padre con tipo = " + miPadre.GetComponent<ScriptDatosElemenItf>().tipoElementIntf); }
            }
        }  // Fin de - if (transform.parent != null)
    }  // Fin de - void Start () {

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  Update() 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-07-08
    /// Ultima modificacion :
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    /// </summary>
    void Update () {

}

// ///////////////////////////////////////////////////////////////
// ///////////////////////////////////////////////////////////////
// ///////////////////////////////////////////////////////////////
//       METODOS DE CONSULTA y acceso a propiedades


/// <summary>
/// /////////////////////////////////////////////////////////////////
/// Metodo : genera la informacion asociada al tipo 
/// 
/// Autor : Miguel Angel Fernandez Graciani
/// Fecha creacion : 2021-02-07
/// Ultima modificacion :
///
/// Variables de entrada :
///     tipoElementIntf : Es el tipo al que pertenece el elemento de interfaz (Ver "VALORES DE TIPO" en este script)
/// Observaciones:
///         - El tipo del emlemento de interfaz que se genera lo define quien genera el elemento y es por tanto quien debe definirlo
///         - EL identificador en la lista de tipo, no se puede generar hasta que se conoce el tipo, por lo que se genera aqui
///         - OJOOOO quien genera un elemento de interfaz DEBE LLAMAR A ESTA FUNCION, para definir el tipo y el identificador en este tipo
/// </summary>
public void generaElementEnTipo(string tipoDelElemento)
    {
        tipoElementIntf = tipoDelElemento;
        idElementEnTipo = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generaIdElementEnTipo(tipoElementIntf);  // Es el identificador unico de todos los elementos de un mismo tipo. Se consigue desde "dameIdElementEnTipo()"
    }  // Fin de - public void generaElementEnTipo(string tipoDelElemento)


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : devuelve idElementIntf_privado
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-05
    /// Ultima modificacion :
    ///
    /// Variables de salida :
    ///     cada metodo devuelve la propiedad correpondiente
    /// Observaciones:
    ///         Se acedera a ellos cada vez que se recorra la lista de solicitudes en busca de las que correspondan
    /// </summary>
    public int dameIdElementIntf() { return idElementIntf; }
    public string dameTipoElementIntf() { return tipoElementIntf; }
    public int dameIdElementEnTipo() { return idElementEnTipo; }
    public string dameSubTipoElementIntf() { return subTipoElementIntf; }

//    public void ponTipoElementIntf(string tipoElemento) { tipoElementIntf = tipoElemento; }
    public void ponSubTipoElementIntf(string subTipoElemento) { subTipoElementIntf = subTipoElemento; }


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : gestionaArbolGenealogico(). Mira si esta, inserta o elimina un elemento, de "listaDePadres" 0 "listaDeHijos" de este elemento
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-01-11
    /// Ultima modificacion :
    ///
    /// Variables de entrada :
    ///     GameObject elementoPadre; es el elemento al que se ahija o del que se desahija este elemento (this) : 
    ///     string operacion; indica la la operacion a realizar: 
    ///             - "ahija": hace este elemento (this) hijo del elemento "elementoPadre" lo pone en su "listaDePadres" y se pone en su "listaDeHijos"
    ///                 devuelve "Resultado" :
    ///                     - "correcto": este elemento (this) se ha puesto como hijo del elemento "elementoPadre"
    ///                     - "yaEraHijo": el "Elemento" No estaa en la "listaDePadres" 0 "listaDeHijos" (segun "padreOhijo")
    ///                     - "error": se ha producido un error 
    ///             - "desahija":  elimina este elemento (this) como hijo del elemento "elementoPadre" lo quita de su "listaDePadres" y se se quita en su "listaDeHijos"
    ///                 devuelve "Resultado" :
    ///                     - "correcto": la operacion se ha realizado correctamente
    ///                     - "noEraHijo": "Elemento" ya estaba en la "listaDePadres" 0 "listaDeHijos" (segun "padreOhijo")
    ///                     - "error": se ha producido un error 
    /// 
    /// Variables de salida :
    ///     string Resultado;  - Si el string esta vacio ( Resultado = "") es que todo ha ido bien
    ///                        - Si algo no ha ido bien devuelve un string con el informe de error
    ///         
    /// Observaciones:
    ///     - OJOO el arbol genealogico al que aqui se maneja NO ES LA GERARQUIA DE OBJETOS DE UNITY, si no la genealogia que hemos generado 
    ///     mediante las listas "listaDePadres" y "listaDeHijos", que tiene naturaleza, estructura y funciones distintas
    ///     - OJOO si este elemento (this) esta como hijo varias veces o tiene a "elementoPadre" como padre varias veces, se eliminaran todas las 
    ///     ocurrencias de la lista correspondiente
    ///     
    ///     Pasos a ejecutar
    ///         1.) Miramos si este elemento (this) esta como hijo del elemento "elementoPadre" en su "listaDeHijos" 
    ///             1.1.) Si SI esta : Segun la operacion a realizar:
    ///                 1.1.1.) "desahija" : Si tenemos que insertar el elemento en la lista
    ///                     - Si este elemento (this), Si estaba en la "listaDeHijos" del elemento "elementoPadre", lo quitamos
    ///                 1.1.2.) "ahija" : 
    ///                     - Si este elemento (this), ya estaba en la "listaDeHijos" del elemento "elementoPadre", avisamos
    ///             1.2.) Si NO esta : Segun la operacion a realizar:
    ///                 1.2.1.) "ahija" : 
    ///                     - Si este elemento (this), no estaba en la "listaDeHijos" del elemento "elementoPadre", lo incluimos
    ///                 1.2.2.) "desahija" : Si tenemos que insertar el elemento en la lista
    ///                     - Si este elemento (this), NO estaba en la "listaDeHijos" del elemento "elementoPadre", avisamos
    ///         2.) Miramos si el elemento "elementoPadre" esta como padre en la lista "listaDePadres" de este elemento (this)
    ///             2.1.) Si SI esta : Segun la operacion a realizar:
    ///                 2.1.1.) "desahija" : Si tenemos que eliminar el elemento en la lista
    ///                     - - Si "elementoPadre" Si estaba en la lista "listaDePadres" de este elemento (this), lo quitamos
    ///                 2.1.2.) "ahija" : 
    ///                     - Si "elementoPadre" ya estaba en la lista "listaDePadres" de este elemento (this), avisamos
    ///             2.2.) Si NO esta : Segun la operacion a realizar:
    ///                 2.2.1.) "ahija" :  Si tenemos que insertar el elemento en la lista
    ///                     - Si "elementoPadre" no estaba en la lista "listaDePadres" de este elemento (this), lo incluimos
    ///                 2.2.2.) "desahija" :
    ///                     - Si "elementoPadre" NO estaba en la lista "listaDePadres" de este elemento (this), avisamos
    ///     
    /// </summary>
    public string gestionaArbolGenealogico(GameObject elementoPadre, string operacion)
    {
        string Resultado = "";

        // ///////////////////////
        // 1.) Miramos si este elemento (this) esta como hijo del elemento "elementoPadre" en su "listaDeHijos" 

        if (elementoPadre.gameObject.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Contains(this.gameObject))
        {
            // 1.1.) Si SI esta : Segun la operacion a realizar:
            //     1.1.1.) "desahija" : Si tenemos que insertar el elemento en la lista
            //         - Si este elemento (this), Si estaba en la "listaDeHijos" del elemento "elementoPadre", lo quitamos
            if (operacion == "desahija")
            {
                elementoPadre.gameObject.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Remove(this.gameObject);
            }
            //     1.1.2.) "ahija" : 
            //         - Si este elemento (this), ya estaba en la "listaDeHijos" del elemento "elementoPadre", avisamos
            else
            {
                Resultado = Resultado + " - ya estaba en la lista de hijos";
            }
        }  // Fin de - if (elementoPadre.gameObject.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Contains(this.gameObject))
        // 1.2.) Si NO esta : Segun la operacion a realizar:
        else
        {
            //     1.2.1.) "ahija" : 
            //         - Si este elemento (this), no estaba en la "listaDeHijos" del elemento "elementoPadre", lo incluimos
            if (operacion == "ahija")
            {
                elementoPadre.gameObject.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Add(this.gameObject);
            }
            //     1.2.2.) "desahija" : Si tenemos que insertar el elemento en la lista
            //        - Si este elemento (this), NO estaba en la "listaDeHijos" del elemento "elementoPadre", avisamos
            else
            {
                Resultado = Resultado + " - NO estaba en la lista de hijos";
            }
        }  // Fin de else de - if (elementoPadre.gameObject.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Contains(this.gameObject))

        // /////////////////////////////
        // 2.) Miramos si el elemento "elementoPadre" esta como padre en la lista "listaDePadres" de este elemento (this)
        //      2.2.) Si NO esta : Segun la operacion a realizar:
        //         2.2.1.) "ahija" :  Si tenemos que insertar el elemento en la lista
        //             - Si "elementoPadre" no estaba en la lista "listaDePadres" de este elemento (this), lo incluimos
        //         2.2.2.) "desahija" :
        //             - Si "elementoPadre" NO estaba en la lista "listaDePadres" de este elemento (this), avisamos
        if (listaDePadres.Contains(elementoPadre))
        {
            //     2.1.) Si SI esta : Segun la operacion a realizar:
            //         2.1.1.) "desahija" : Si tenemos que eliminar el elemento en la lista
            //             - Si "elementoPadre" Si estaba en la lista "listaDePadres" de este elemento (this), lo quitamos
            if (operacion == "desahija")
            {
                listaDePadres.Remove(elementoPadre.gameObject);
            }
            //         2.1.2.) "ahija" : 
            //            - Si "elementoPadre" ya estaba en la lista "listaDePadres" de este elemento (this), avisamos
            else
            {
                Resultado = Resultado + " - ya estaba en la ista de padres";
            }
        }  // Fin de - if (listaDePadres.Contains(elementoPadre))
        // 1.2.) Si NO esta : Segun la operacion a realizar:
        else
        {
            //     1.2.1.) "ahija" : 
            //         - Si este elemento (this), no estaba en la "listaDeHijos" del elemento "elementoPadre", lo incluimos
            if (operacion == "ahija")
            {
                listaDePadres.Add(elementoPadre.gameObject);
            }
            //     1.2.2.) "desahija" : Si tenemos que insertar el elemento en la lista
            //        - Si este elemento (this), NO estaba en la "listaDeHijos" del elemento "elementoPadre", avisamos
            else
            {
                Resultado = Resultado + " - NO estaba eb la lista de padres";
            }
        }  // Fin de else de - if (listaDePadres.Contains(elementoPadre))

        return Resultado;
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  eliminaElemIntf - Elimina elelemento de interfaz borrando todos sus hijos y dependientes
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-12
    /// Última modificacion :	
    ///     - 2021-08-07. Incluyo el borrado de las solicitudes
    /// Inputs;
    ///     -  
    /// Observaciones :
    /// 
    ///     - Hay que eliminar todos los hijos y dependientes. Para llevar a cabo la eliminación de todos los hjos se actua de forma recursiva. Cada padre va pidiendo
    ///     eliminar a sus hijos, y cuando estos ya no tienen mas hijos, se van borrando todas la listas y de todas las listas, hasta que al final se elimina el 
    ///     gameObject padre y con el todos sus descendientes en la gerarquia de unity
    ///     Para ello es necesario hacer las siguientes operaciones
    ///         - Fase_1) Ejecutamos el borrado de todos los hijos de nuestra lista de hijos
    ///         - Fase_2) Nos eliminamos de todas las listas en las que estamos suscritos
    ///         - Fase_3) eliminamos nuestro game Object
    ///         
    ///     Para la Fase_2.
    ///     - OJOOO. BORRAR UN ELEMENTO DE INTERFAZ NO ES ELEMENTAL. Al eliminar el gameObject, se eliminan tambien todos los gameobjects
    ///     que descienden de el en la gerarquia de Unity. Pero tanto este elemento de interfaz como sus descendientes tienen relaciones con otros 
    ///     elementos que no son descendientes suyos (en la gerarquia de Unity) (por ejemplo, una rama es hija del evi de rama que la genero) y ademas 
    ///     estan referenciados en multiples listas en las que deberan "eliminar su subcripcion". 
    ///     Procuramos aqui enumerar dicahas lista y relaciones
    ///         - Fase_2_1. LISTAS QUE REFERENCIAN UN ELEMENTO EN LAS QUE DEBEMOS ELIMINAR NUESTRA SUBCRIPCION (independientemente de la gerarquia de objetos de unity):
    ///                 - Fase_2_1_1 Listas de tipo de elemento de interfaz. En "ScriptDatosInterfaz" :
    ///                  Dependiendo del tipo de elemnto de interfaz, el elemento estará en una de esta listas :
    ///                         - Fase_2_1.1.1 ListaRamas (Si es una rama aparecera listado aqui)
    ///                         - Fase_2_1.1.2 ListaMuros (Si es un muro aparecera listado aqui)
    ///                         - Fase_2_1.1.3 ListaEvis (Si es un evi aparecera listado aqui)
    ///                 - Fase_2_1_2. LISTAS EN LAS QUE APPARECERA REFERENCIADO EN ALGUNA PROPIEDAD DE OTRO OBJETO.Las siguientes listas pueden tener uno o varios elementos 
    ///                  que lo referencian y que por lo tanto habra que eliminar
    ///                         - Fase_2_1.2.1 ListaVisitas ( habra que borrar las visitas que hagan referencia a este elemento de interfaz). En "ScriptGestorVIsitas"
    ///                         - Fase_2_1.2.2 ListaTransitos  (). En "ScriptGestorTransitos"
    ///                         - Fase_2_1.3.3 misSolicitudesPendientes  (SOn las solicitudes que he generado y todabia no se han resuelto HAY QE ENCARGARSE DE ELLAS). En este mismo script "ScriptDatosElemenItf"
    ///                         - Fase_2_1.2.4 ListaSolicitudes ( Cualquier elemento de interfaz puede pararecer en una lista de solicitudes como solicitante o como destinatario). En "ScriptDatosInterfaz"
    ///                     
    ///          - Fase_2_2. TAMBIEN HAY UNA SERIE DE ELEMENTOS DE INTERFAZ EN LOS QUE FIGURAMOS EN SU LISTA DE HIJOS, DE LAS QUE IGUALMENTE DEBEMOS ELIMINARNOS
    ///                     - Fase_2_2.1 Cuando se genero este elemento de interfaz, SI ESTE ELEMENTO TENIA UN PARDRE EN LA GERARQUIA DE UNITY, se anoto en su lista 
    ///                     de hijos "listaDeHijos", por lo que ahora debe borrarse
    ///                             - Un evi es hijo del muro en el que reside (tambien lo es en la gerarquia de unity)
    ///                             - Un muro es hijo de la rama en la que reside  (tambien lo es en la gerarquia de unity)
    ///                             - Una rama es hija de la rama de la que surge  (tambien lo es en la gerarquia de unity)
    ///                     
    ///                     - Fase_2_2.2 Existen algunos "ahijados", que no son hijos por jerarquia en unity, pero que si figuran en la list de hjos de un elemento de interfaz
    ///                     y por tanto, deberan borrarse de la lista de hijos de sus "padrinos".
    ///                         - Estos son una serie de casos concretos que enumeramos ahora de for ma extensiva :
    ///                             - Fase_2_2.2.1 UNA RAMA es tambien ahija del evi que la genera. Este podra ser UN EVI DE RAMA que gestiona la rama
    ///                             - Fase_2_2.2.2 UNA RAMA es tambien ahija del evi que la genera. Este podra ser UN EVI DE CONCEPTO que se ha expandido mediante una rama
    ///                             - Fase_2_2.2.3 UNA RAMA es tambien ahija del evi que la genera. Este podra ser UN EVI DE BUSQUEDA cuyo resutado se expone mediante una rama
    ///                             - Fase_2_2.2.4 UNA RAMA es tambien ahija del evi que la genera. Este podra ser UN EVI DE EDICION el cual se edita mediante una rama
    ///                             - Fase_2_2.2.5 UNA RAMA es tambien ahija del evi que la genera. Este podra ser Otros (QUE HABRA QUE AÑADIR CUANDO SE VAYAN PROGRAMANDO)  CAMBIO EVOLUTIVO (MAFG 2021-03-12)
    ///                             - Fase_2_2.2.6 UN EVI que es parte de otro evi sera hijo de este (aunque no sea hijo en la gerarquia de unity). Las relaciones a este 
    ///                                  nivel pueden ser diversas dependiendo de la naturaleza y funcionalidad de los evis (estos pueden ser incluso diseños ajenos)
    /// <summary>
    public void eliminaElemIntf()
    {
        // //////////////////////////////////////////////
        // Vamos realizando el proceso conforme se indica en las observaciones

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Vamos a eliminar el objeto de nombre = " + this.name + " - y con idElementIntf = " + idElementIntf); }

        // //////////////////////////////////////////////
        // //////////////////////////////////////////////
        // Fase_1 : Primeramente  solicitamos a nuestros hijos que se borren del mapa.
        while (listaDeHijos.Count > 0)  // Mientras la lista de hijos no este vacia vamos borrando hijos. Siempre el primero
        {
//            Debug.Log("desde eliminaElemIntf en this name = " + this.name + " - mando borrar a listaDeHijos[0] de nombre = " + listaDeHijos[0].name);
            listaDeHijos[0].GetComponent<ScriptDatosElemenItf>().eliminaElemIntf(); // Ordenamos al hijo que se autodestruya
            if (listaDeHijos.Count > 0)  // Algun elemento de interfaz puede, al borrarse, puede eliminarse de esta lista, con lo que al intentar eliminarlo cascaria
            {
                listaDeHijos.Remove(listaDeHijos[0]);  // Borramos al hijo de la lista de hijos
            }
        }  // Fin de - while (listaDeHijos.Count > 0)

        // No puedo borrar los hijos de la lista con un foreach, porque se borran mientras se esta recorriendo la lista y asi da error
        // if (listaDeHijos != null)  // Si tenemos algun hijo en la lista
        // {foreach (GameObject hijo in listaDeHijos)
        //     {if (hijo != null){hijo.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();}} // Fin de - foreach (GameObject hijo in listaDeHijos)
        // }  // Fin de - if (listaDeHijos != null)  // Si tenemos algun hijo en la lista

        // //////////////////////////////////////////////
        // //////////////////////////////////////////////
        // Fase_2 : Nos borramos de las listas correspondientes

        // //////////////////////////////////
        //   - Fase_2_1. LISTAS QUE REFERENCIAN UN ELEMENTO EN LAS QUE DEBEMOS ELIMINAR NUESTRA SUBCRIPCION (independientemente de la gerarquia de objetos de unity):
        //      - Fase_2_1_1 Listas de tipo de elemento de interfaz. En "ScriptDatosInterfaz" :
        if (tipoElementIntf == tipoElemItf_rama)  // Fase_2_1.1.1 ListaRamas (Si es una rama aparecera listado aqui)
        {
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaRamas.Remove(gameObject); // Borramos el elemento de la listas de su padre Unity
        }
        else if (tipoElementIntf == tipoElemItf_muro)  // Fase_2_1.1.2 ListaMuros (Si es un muro aparecera listado aqui)
        {
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaMuros.Remove(gameObject); // Borramos el elemento de la listas de su padre Unity
        }
        else if (tipoElementIntf == tipoElemItf_evi)  // Fase_2_1.1.3 ListaEvis (Si es un evi aparecera listado aqui)
        {
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaEvis.Remove(gameObject); // Borramos el elemento de la listas de su padre Unity
        }
        else if (tipoElementIntf == tipoElemItf_agente)
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log(" - ERROR en ScriptDatosElemenItf => eliminaElemIntf(). Los mecanismos para eleminar agentes no estan desarrollados"); }
        }
        else if (tipoElementIntf == tipoElemItf_solicitud)
        {
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaSolicitudes.Remove(gameObject); // Borramos el elemento de la listas de su padre Unity
        }
        else 
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log(" - ERROR en ScriptDatosElemenItf => eliminaElemIntf(). Elemento a eliminar de tipo desconocido"); }
        }  // Fin de - else - del - if (tipoElementIntf == tipoElemItf_rama) 

        //    - Fase_2_1_2. LISTAS EN LAS QUE APPARECERA REFERENCIADO EN ALGUNA PROPIEDAD DE OTRO OBJETO.Las siguientes listas pueden tener uno o varios elementos 
        //    que lo referencian y que por lo tanto habra que eliminar

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Vamos a eliminar  en 364 el objeto de nombre = " + this.name + " - y con idElementIntf = " + idElementIntf); }

        //     - Fase_2_1.2.1 ListaVisitas ( habra que borrar las visitas que hagan referencia a este elemento de interfaz). En "ScriptGestorVIsitas"
        //          Eliminamos las referencias a este elemento de la lista de visitas. La información se perdera, pero al no existir el elemento, consideramos
        //          que no lo incluiremos en los procesos que se realicen para interpretar las visitas
        int punteroALista = ctrlInterfaz.GetComponent<ScriptGestorVIsitas>().ListaVisitas.Count;
        while (punteroALista > 0)  // Mientras la lista de hijos no este vacia vamos borrando hijos. Siempre el primero
        {//   Miramos si la visita lo es a este elemento de interfaz
            int idEnLista = ctrlInterfaz.GetComponent<ScriptGestorVIsitas>().ListaVisitas[punteroALista].idElementoVisitado;
            if (idEnLista == idElementIntf)
            {
                ctrlInterfaz.GetComponent<ScriptGestorVIsitas>().ListaVisitas.Remove(ctrlInterfaz.GetComponent<ScriptGestorVIsitas>().ListaVisitas[punteroALista]);  // Borramos al hijo de la lista de visitas
            }
            punteroALista--; // Decrementamos el puntero para recorres la lista hacia abajo
        }  // Fin de - while (punteroALista > 0)


        // PENDIENTE (MAFG 202103-12)
        //     - Fase_2_1.2.2 ListaTransitos  (). En "ScriptGestorTransitos"


        // PENDIENTE (MAFG 202103-12)
        //     - Fase_2_1.3.3 misSolicitudesPendientes  (SOn las solicitudes que he generado y teodabia no se han resuelto HAY QE ENCARGARSE DE ELLAS). En este mismo script "ScriptDatosElemenItf"


        // PENDIENTE (MAFG 202103-12)
        //     - Fase_2_1.2.4 ListaSolicitudes ( Cualquier lemento de interfaz puede pararecer en una lista de solicitudes como solicitante o como destinatario). En "ScriptDatosInterfaz"

        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Vamos a eliminar  en 391 el objeto de nombre = " + this.name + " - y con idElementIntf = " + idElementIntf); }


        // //////////////////////////////////
        //   - Fase_2_2. TAMBIEN HAY UNA SERIE DE ELEMENTOS DE INTERFAZ EN LOS QUE FIGURAMOS EN SU LISTA DE HIJOS, DE LAS QUE IGUALMENTE DEBEMOS ELIMINARNOS
        //      - Fase_2_2.1 Cuando se genero este elemento de interfaz, SI ESTE ELEMENTO TENIA UN PARDRE EN LA GERARQUIA DE UNITY, se anoto en su lista 
        //     de hijos "listaDeHijos", por lo que ahora debe borrarse
        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<ScriptDatosElemenItf>())  // Si el padre es un elemento de la interfaz (solicitudes no lo es)
            {
                transform.parent.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Remove(gameObject);
            }
        }

        //   - Fase_2_2.2 Existen algunos "ahijados", que no son hijos por jerarquia en unity, pero que si figuran en la list de hjos de un elemento de interfaz
        //   y por tanto, deberan borrarse de la lista de hijos de sus "padrinos".

        if (tipoElementIntf == tipoElemItf_rama)
        {
            //     - Fase_2_2.2.1 UNA RAMA es tambien ahijada del evi que la genera. Este podra ser UN EVI DE RAMA que gestiona la rama
            //     - Fase_2_2.2.2 UNA RAMA es tambien ahijada del evi que la genera. Este podra ser UN EVI DE CONCEPTO que se ha expandido mediante una rama
            //     - Fase_2_2.2.3 UNA RAMA es tambien ahijada del evi que la genera. Este podra ser UN EVI DE BUSQUEDA cuyo resutado se expone mediante una rama
            //     - Fase_2_2.2.4 UNA RAMA es tambien ahijada del evi que la genera. Este podra ser UN EVI DE EDICION el cual se edita mediante una rama
            //     - Fase_2_2.2.5 UNA RAMA es tambien ahijada del evi que la genera. Este podra ser Otros (QUE HABRA QUE AÑADIR CUANDO SE VAYAN PROGRAMANDO)  CAMBIO EVOLUTIVO (MAFG 2021-03-12)
            GetComponent<ScriptCtrlRama>().elemIntfQueLaOrigina.GetComponent<ScriptDatosElemenItf>().listaDeHijos.Remove(gameObject);
        }  // Fin de - if (tipoElementIntf == tipoElemItf_rama)

        // PENDIENTE (MAFG 202103-12)
        //     - Fase_2_2.2.6 UN EVI que es parte de otro evi sera hijo de este (aunque no sea hijo en la gerarquia de unity). Las relaciones a este 
        //     nivel pueden ser diversas dependiendo de la naturaleza y funcionalidad de los evis (estos pueden ser incluso diseños ajenos)

        // Borramos el EVI del DAUS PENDIENTE (MAFG 2021-01-30)

        if (DatosGlobal.niveDebug > 50)
        { Debug.Log("Vamos a eliminar  en 421 el objeto de nombre = " + this.name + " - y con idElementIntf = " + idElementIntf); }

        // //////////////////////////////////////////////
        // //////////////////////////////////////////////
        // Fase_3 : Eliminamos el gameObject
        Destroy(gameObject);

    }  // Fin de -  public void eliminaElemIntf()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  LateUpdate() 
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-07-08
    /// Ultima modificacion :
    /// Parametros de entrada :
    /// Parametros de entrada :
    /// Observaciones:
    ///         - Este metodo se ejecuta despues de todos los updates, por lo que si hay que hacer algun tipo de actualizacion global del objeto, habria que hacerla aqui
    /// </summary>
    void LateUpdate()
    {
        // Si hemos cambiado de modo, cada update habra actuado en consecuencia. Aqui actualizamos la variable "modoAnterior" para que los distintos métodos
        // detecten en cuadros posteriores, que se ha cambiado de modo
        if (modoAnterior != modo)
        {
            // Actualizamos el modo anterior
            // Lo hacemos mediante una corrutina, para poder esperar un cuadro, si no, los objetos no detectan el cambio y no funciona
            // debe ser porque se cambia el "modoAnterior" antes de que los objetos hayan detectado el cambio
            StartCoroutine(esperaYActualizaModoAnterior());
        }
    }  // Fin de - void LateUpdate()

    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   CORRUTINAS
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo (corrutina) : Solicita una imagen de ayuda a interfaz a un servidor externo Y la pone como textura en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-02-12
    /// Ultima modificacion :
    /// Variables de entrada :
    ///         - string origenDeLaImagen  : segun el tipo, en ocadiones la url del host del DKS al que se envia la consulta (Ej. : ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Dks_en_conexion_host)
    /// Variables de salida :
    /// Observaciones:
    ///         OJO  pone como textura de DOS GAMEOBJECT en el fondo del contenedor del evi y en la imagen que acompaña añ icono
    /// </summary>
    IEnumerator esperaYActualizaModoAnterior()
    {

        yield return null;
        modoAnterior = modo;
    }  // Fin de - IEnumerator esperaYActualizaModoAnterior()



}  // Fin de - public class ScriptDatosElemenItf : MonoBehaviour {
