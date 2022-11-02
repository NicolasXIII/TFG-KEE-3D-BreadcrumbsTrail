using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.UI;


/// <summary>
/// ////////////////////////////////////////////////////////////////////////////////////// Error al obtener host, no es un nodo adecuado - Error en dameDatoSinTecho - No es un elemento adecuado T. Nomnre del elemento recibido 
/// ///////////  Script para editar un evi editando su descripcion en un nuevo muro DE EDICION
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-06-17
/// Observaciones :
///     - Este componente se usa tanto para editar un evi ya existente, como para editar un nuevo evi recien creado o uno clonado
///     o uno que se ha genarado ara instanciar un concepto de un evi concreto
///
///		DATOS GENERALES :
///		
///		METODOS GENERALES :
///			-
/// </summary>

public class SctEditaEvi : MonoBehaviour
{
    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Algunos elementos que tienen informacion a la que deben acceder
    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public GameObject Btn_BaseDeEvi_N1_Opciones;
    public GameObject Btn_BaseDeEvi_N2_Caja_opciones;
    public GameObject Btn_BaseDeEvi_N2_1_op_Editar;
    public GameObject Btn_BaseDeEvi_N3_1_op_Editar_Grabar;
    public GameObject Btn_BaseDeEvi_N3_1_op_Editar_Salir;

    public GameObject ramaAsociada;

//    public RectTransform Panel_Input_Text_SinTecho;
    public GameObject Panel_Input_Text_SinTecho;
    public TMP_InputField Input_Text_T_deSinTecho;
    public Button Btn_Guardar_Text_T_deSinTecho;
    public Button Btn_Cancelar_Text_T_deSinTecho;

    // Use this for initialization
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // Tomamos los botones y los asignamos como objetos para poder direccionarlos desde este script
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if (child.name == "Btn_BaseDeEvi_N1_Opciones")
            {
                Btn_BaseDeEvi_N1_Opciones = child;
                // "Btn_BaseDeEvi_N1_Opciones" solo iene un hijo que es "Btn_BaseDeEvi_N2_Caja_opciones"
                Btn_BaseDeEvi_N2_Caja_opciones = Btn_BaseDeEvi_N1_Opciones.gameObject.transform.GetChild(0).gameObject;
            }
        }

        if (Btn_BaseDeEvi_N2_Caja_opciones != null)
        {
            for (int i = 0; i < Btn_BaseDeEvi_N2_Caja_opciones.transform.childCount; i++)
            {
                GameObject child = Btn_BaseDeEvi_N2_Caja_opciones.transform.GetChild(i).gameObject;
                if (child.name == "Btn_BaseDeEvi_N2_1_op_Editar") { Btn_BaseDeEvi_N2_1_op_Editar = child; }
                else if (child.name == "Btn_BaseDeEvi_N3_1_op_Editar_Grabar") { Btn_BaseDeEvi_N3_1_op_Editar_Grabar = child; }
                else if (child.name == "Btn_BaseDeEvi_N3_1_op_Editar_Salir") { Btn_BaseDeEvi_N3_1_op_Editar_Salir = child; }
            }
        }


        //  Btn_Caja_opciones = this.transform.parent.tamañoAltoBotonActivo;
        // this.transform.localScale = this.transform.parent.GetComponent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;
        //this.transform.localScale = this.GetComponentInParent<Script_BaseDeEvi_N2_Caja_opciones>.tamañoAltoBotonActivo;

        ramaAsociada = null;

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo :  public void botonEditaEvi(). Se ha pulsado el boton de editar del evi base.
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2021-06-xx
    /// Ultima modificacion :
    ///         - 2021-12-aprox. Incluyo la posibilidad de editar el texto de los evis sin techo
    ///         - 2022-02-27. Me pongo a programar la funcionalidad para dar la posibilidad de asociar ficheros (imagen, video, audio, otros...) al dato
    ///                     sin techo, para poder hacer modificaciones y altas de conceptos
    /// Variables de entrada :
    /// Variables de salida :
    ///     No devuelve nada, solo edita el evi 
    /// Observaciones : Se ha pulsado el boton de editar del evi base. 
    /// 
    ///     - OJO, cuando grabamos un concepto en edicion, la descripcion de este
    ///         ATIENDE EXCLUSIVAMENTE A LA CONFIGURACION DEL ARBOL DE MUROS Y EVIS QUE EXISTE EN ESE MOMENTO EN SU ARBOL DE EDICION
    ///         NO SE TIENE EN CUENTA LA RELACION DE LOS "" DE CADA EVI.
    ///         LA DESCRIPCION DEL CONCEPTO, SERA LA CONFIGURACION DEL ARBOL DE EDICION 
    ///             - El concepto que se graba corresponde al "Evi cabeza de edicion"
    ///             - Su descripcion seran las referencias, los sin techo y las instancias que hay en su muro de edicion
    ///             - La descripcion de cada una de las instancias sera (recursivamente)
    ///                 - SI se ha editado : las referencias, los sin techo y las instancias que hay en su muro de edicion
    ///                 - Si No se ha editado : La descripcion inicial que figura en su nodo de descripcion del KDL
    ///                 
    ///     - Segun proceda existen varias posibilidades
    ///       1.) Miramos si el evi ya ha sido editado (o la rama generada, para el caso de evi de rama)
    ///       
    ///       2.) Si el evi ya ha sido editado (o la rama creada), nos colocamos e la rama asociada (la que edita el evi) en el primer muro,
    ///        para navegar por ella (TODOS LOS MUROS DE UNA RAMA DE EDICION SON AMARILLOS)
    ///        
    ///       3.) Si el evi no esta editado, se edita el evi, generando la rama correspondiente
    ///           - SOlo se editan los elementos de interfaz que son EVIs " tipoElementIntf = tipoElemItf_evi ". Pero su edicion esta condicionada al subtipo de este. Osea a la naturaleza
    ///           de este. Definimos a continuacion que evis son editables dependiendo de su subtipo:
    ///           
    ///         TABLA DE POSIBILIDADES DE EDICION :
    ///             Posibilidades de edicion asociadas a cada TIPO DE EVI (SOLO SE EDITAN LOS ELEMENTOS DE INTERFAZ QUE SON DE TIPO EVI, Y SOLO ALGUNAS SUBCLASES DE ESTOS)
    ///             SI se editan los subtipos :
    ///                     - subTipoElemItf_evi_baseRefFractal; para editar conceptos que iran a grabarse al DKS asociado
    ///                     - subTipoElemItf_evi_baseInstFractal; Etos se editan SI ESTAN EN UN MURO DE EDICION, para modificar la instancia concreta durante el proceso 
    ///                                                             de edicion de una descripcion a la que pertenecen
    ///                     - subTipoElemItf_evi_baseSinTecho_00; Etos se editan SI ESTAN EN UN MURO DE EDICION, para modificar el valor sin techo concreto durante el proceso 
    ///                                                             de edicion de una descripcion a la que pertenecen
    ///                                       
    ///                                                             
    ///             SI se editaran mas tarde cuando tengamos tiempo de programarlo (PENDIENTE MAFG 2021-06-30):
    ///                     - subTipoElemItf_evi_buscador_00; Su proceso de edicion, lo será para configurar búsquedas complejas
    ///                     - subTipoElemItf_evi_lista_00; Su proceso de edicion, lo será para configurar filtros e indexaciones
    ///                     - subTipoElemItf_evi_camino_00; Su proceso de edicion, lo será para configurar filtros e indexaciones
    ///                     - subTipoElemItf_evi_arbol_00; Su proceso de edicion, lo será para configurar arboles de busqueda o edicion en forma de ramas completas
    ///                     - 
    ///             
    ///             NO se editan los subtipos :
    ///                     - subTipoElemItf_evi_rama; No es editable porque este tipo de evi lo es para gestionar ramas de búsqueda (en edicion, cualquier nueva rama es una descripcion que 
    ///                                                 tiene que estar asociada obligatoriamente a la descripcion de una instancia conreta9
    ///                     - subTipoElemItf_evi_EviPrue_001; solo es para probar
    ///                     - subTipoElemItf_evi_EviRefElemen; son evis para referenciar elementos como muros, ramas u otros, por lo que creo que no sera necesario editarlos
    ///                     - subTipoElemItf_evi_Btn_Consulta_Si_No; Es un evi de consulta asociado a la funcionalidad del KEE (por ahora no veo necesidad de editarlo)
    ///                     - Los evis tsubtipo CONTENEDOR o FRACTUM, no se editan ya que a su informacion se accede mediante la edición del evi base que los contiene
    ///                                Un fractum o el contenido de un contenedor, se convierten en evis fractales o de referencia en el muro de edicion que se genera al
    ///                                editar el evi base que los contiene, y es desde ah.i, desde donde pueden editarse
    ///         FIN DE - TABLA DE POSIBILIDADES DE EDICION :
    ///                                
    ///           
    ///           - Para editar cualquier evi generamos una nueva rama de edicion asociada a este y un muro inicial de edicion (AMARILLO) dosde se desarrollan cada uno de los 
    ///                 subelementos del evi como un evi independiente, cada nueva descripcion de un elemento, sera una nueva rama del arbol de edicion.
    ///                 
    ///            MURO DE EDICION :
    ///                 El muro de edicion es un muro similar a los muros de busqueda, pero DESTINADO A LA EDICION ede evis, NO A LA BUSQUEDA O NAVEGACION. Pero con unas caraccteristicas distintas
    ///                 que son las siguientes :
    ///                     - El color del muro es AMARILLO, para diferenciarlo del resto de muros de trabajo
    ///                     - Desde un MURO DE EDICION NO ES POSIBLE. 
    ///                         - Generar ramas de busqueda
    ///                         - Generar nuevos muros de busqueda
    ///                     - Desde los EVI BASE que se albergan en un muro de edicion
    ///                         - Desde un evi REFERENCIA, NO ES POSIBLE
    ///                             - No es posible Expandir el evi, ya que en la descripcion solo aparecera como referencia, si se quiere instanciar habra que generar una instancia
    ///                             - No es posible Editar  el evi, ya que en la descripcion solo aparecera como referencia, si se quiere instanciar habra que generar una instancia
    ///                         - Desde un evi INSTANCIA, NO ES POSIBLE
    ///                             - No es posible Expandir el evi, ya que una instancia en edicion, solo necesita expandirse cuando se edita para ser modificada
    ///                             - No es posible instanciar el evi, ya que ya es una instancia, si se quiere generar otra instancia similar se debe clonar esta
    ///                         - Desde un evi SIN TECHO, NO ES POSIBLE
    ///                             - No es posible Expandir el evi, Los evis sin techo no se expanden
    ///                 
    ///            ARBOL DE EDICION : 
    ///                 Cuando editamos un evi se genera una rama de edicion asociada a el. De esta rama cuelgan todos los muros y ramas que configuran el arbol 
    ///                 de edicion del evi en cuestion. Dependiendo de la naturaleza del evi raiz de la edicion, la estructura del arbol tiene uno u otro sentido. Vemos cada caso 
    ///                 en concreto :
    ///                     - ARBOL DE EDICION de un subTipoElemItf_evi_baseRefFractal (sea asociado a la edicion de un concepto, una clonacion, una instanciacion o la edicion de un concepto nuevo)
    ///                             - El evi raiz es el evi asociado al concepto que se edita (sea la edicion de un concepto, una clonacion, una instanciacion o la edicion de un concepto nuevo)
    ///                             - En el primer muro aparece la descripcion de dicho concepto de forma expandida. Dentro de este muro el comportamiento de los evis puede verse en el parrafo
    ///                             "MURO DEEDICION" de este mismo comentario.
    ///                             - El primer muro contiene la descripcion del concepto (referencias, instancias y sin techos) como en toda descripcion de concepto KDL :
    ///                                 - Las referencias no pueden estenderse, son solo referencias a otro concepto
    ///                                 - Las instancias si admiten que su descripcion pueda editarse, por lo que desde ellas se puede acceder a una rama hija, con un muro de descripcion que 
    ///                                 tendra un conportamiento igual a este (con sus referencias, instancias y sin techos), y asi continua realizandose la descripcion del arbol KDL del concepto que 
    ///                                 editamos de forma recursiva
    ///                                 - Los "Sin Techo" pueden editarse, para modificar su tipo o para modificar su contenido
    ///                             - Las ramas, hijos de este muro de edicion, son las descripciones de las instancias que contiene el muro que a su vez contiene una descripcion (Y asi recursivamente 
    ///                             hasta completar la estructura de albol del concepto KDL que estamos editando, que sera la que se envie al DKS que es quien la almacena)
    ///                             - El usuario va definiendo la descripcion del concepto (y todas sus instancias) insertando en el muro que corresponda :
    ///                                     - Referencias (simplemente arrastrando un concepto (desde la tramoya) hasta el muro de edicion que corresponda a la rama donde se desee hacer la referencia.
    ///                                     Las referencias de un arbol de edicion, no tienen descendencia (ninguna rama parte de ellas, son dreferencias KDL)
    ///                                     - Sin techo. Generando, duplicando, arrastrando (desde tramoya) elementos y modificando su tipo o contenido segun considere. DEl mismo modo, los sistecho
    ///                                     que se insertan en la rama del KDL que corresponde a la rama de edicion en la que se coloca el evi sin techo. Los sin techo de un arbol de edicion, no tienen 
    ///                                     descendencia (ninguna rama parte de ellas, son sin techos KDL)
    ///                                     - Instancias. EL usuario define la descripcion de la instancia usando el subarbol que quiera generar descendiente de la instancia que edita. Las instancias
    ///                                     SI tienen una rama hija de la que cuelga el subarbol que las describe (Son instancias KDL)
    ///                             - Desde los muros de edicion, no pueden generarse muros (ya que estos siempre son la descripcion de una instancia y estaran asociados a ella), ni ramas (por la misma 
    ///                             razon), ni se pueden hacer busquedas.
    ///                             - Las operaciones que el usuario puede realizar:
    ///                                     - Para insertar una referencia: 
    ///                                         - Navega por el arbol de busqueda, 
    ///                                         - localiza el concepto 
    ///                                         - Si desea dejar una copia donde esta, debe clonarlo previamente
    ///                                         - Almacena la copia (o el concepto original) en la tramoya 
    ///                                         - Vuelve al punto del arbol de edicion donde quiere referenciar el concepto
    ///                                         - Arrastra el evi del concepto desde la tramoya al muro asociado al elemento de descripcion KDL donde quiere referenciarlo
    ///                                     - Para insertar una instancia: 
    ///                                         - Navega por el arbol de busqueda, 
    ///                                         - localiza el concepto
    ///                                         - Generar una instancia del concepto (puede clonar o no el concepto y luego convertirlo en instancia)
    ///                                         - Generar un a referencia a un concepto partiendo de una instancia este (ojo, la referencia al concepto tiene la descripcion del concepto en su origen, no la de la instancia)
    ///                                         - Almacena la instancia del concepto en la tramoya
    ///                                         - Vuelve al punto del arbol de edicion donde quiere instanciar el concepto
    ///                                         - Arrastra el evi de instancia del concepto desde la tramoya al muro asociado al elemento de descripcion KDL donde quiere instanciarlo
    ///                                                 - Si a partir de aqui quiere editar la descripcion de la instancia (cosa que sera lo normal), entonces debe darle al boton de 
    ///                                                 editar y realizar las modificaciones que crea necesarias el el subarbol de edicion de la instancia al caso
    ///                                         
    ///                     
    ///                 - Los pasos para realizar el proceso son los siguientes
    ///                     - 1. Generamos una rama y su muro de edicion (amarillo) correspondiente
    ///                     - 2. En el nuevo muro (que ahora será el activoy estara en edicion (amarillo)), generamos cada uno de los evis en los que se expande el evi origen, segun corresponda :
    ///                     Dependiendo pues del subtipo de evi en el que estamos
    ///             
    ///           3.1.) Evi subtipo de gestion de rama " subTipoElementIntf = subTipoElemItf_evi_rama" => Por ahora no se editan los evis de rtama (MAFG 2021-07-08)
    ///           3.2.) Evi subtipo referencia fractal " subTipoElementIntf = subTipoElemItf_evi_RefFractal" => genera una rama de edicion, segun se comenta en las observaciones de esta funcion
    ///           3.3.) Evi subtipo instancia fractal " subTipoElementIntf = subTipoElemItf_evi_InstFractal" => genera una rama de edicion, segun se comenta en las observaciones de esta funcion
    ///           3.4.) Evi subtipo sin techo " subTipoElementIntf = subTipoElemItf_evi_sinTecho_00" => Edita el evi sin techo. Permite cambiar el tipo de dato y el dato que contiene 
    ///                      - Cuando un evi Sin techo esta en un muro de edicion, es sensible al cambio de tipo de dato. Si un concepto (evi de referencia) se arrastra sobre su caja de tipo de dato
    ///                      este se activa (se agranda y suena). Si el evi de referencia se suelta soble la caja, el tipo de dato pasa a ser el concepto que se ha arrastrado sobre la caja. OJOOO
    ///                      puede arrastrarse cualquier concepto, pero la interfaz KEE solo conoce algunos tipos de dato (texto, entero, url, ficheros jpm. wav. etc...) si la interfaz no conoce
    ///                      el tipo de dato, siempre lo tratara como texto plano
    ///                        3.4.1.) Un evi sin techo, solo puede editarse si se encuentra en un muro de edicion, comprobamos que es asi
    ///                        3.4.2.) Dependiendo del tipo de dato, al editar el evi actuamos en consecuencia
    ///                               3.4.2.1.) Para datos  "gen_tipoDeSinTechoTextoPlano" => Habilitamos la edicion del texto
    ///                               3.4.2.2.) Para datos  "gen_tipoDeSinTecho_NumeroEntero" => Habilitamos la edicion del texto para introducir un numero entero
    ///                               3.4.2.3.) Para datos  "gen_tipoDeSinTecho_Url" => Habilitamos la edicion del texto para introducir la url pertinente
    ///                               3.4.2.4.) Para cualquiera de los tipos de fichero conocidos, permitimos que el usuario busque (con los
    ///                                         recursos del sistema) el fichero asociado al dato sin techo y hacemos lo siguiente :
    ///                                             3.4.2.4.1) Guardamos el fichero a enviar en el directorio "Assets\Resources\Temporales", con "un nuevo nombre especifico"
    ///                                             para que cuando se proceda al alta, el KEE envie el fichero al DKS para que alli quede asociado a este Sin Techo
    ///                                             3.4.2.4.2)  Guardamos en la instancia de la clase "ClassFichero" asociada a este sin techo la informacion referente al fichero que sera
    ///                                             necesaria mas tarde para enviar el fichero asociado al sin dato desde el KEE hasta el DKS
    ///                                             para que cuando se proceda al alta, el KEE envie el fichero al DKS para que alli quede asociado a este Sin Techo
    ///                                             3.4.2.4.3) En el dato T del sin techo Z, almacenamos el "nuevo nombre especifico" para que el sin techo y el fichero 
    ///                                             puedan asociarse posteriormente, tanto en el KEE como a su recepcion en el DKS
    ///                                                                                     
    ///                               3.4.2.5.) Para OTROS datos => (PENDIENTE MAFG 2021-09-02)
    ///           3.5.) Evi subtipo buscador 00 " subTipoElementIntf = subTipoElemItf_evi_buscador_00" => genera una rama de edicion de buscador (PENDIENTE MAFG 2021-07-08)
    ///           3.6.) Evi subtipo lista 00 " subTipoElementIntf = subTipoElemItf_evi_lista_00" => genera un evi que contendra una lista de elementos de interfaz
    ///                     Si el evi origen ES UN EVI DE LISTA (PENDIENTE MAFG 2021-03-14)
    ///           3.7.) Evi subtipo camino 00 " subTipoElementIntf = subTipoElemItf_evi_camino_00" => genera un evi que contendra la informacion de caminos de relacion entre conceptos
    ///                     Si el evi origen ES UN EVI DE CAMINO (PENDIENTE MAFG 2021-03-14)
    ///           3.8.) Evi subtipo arbol 00 " subTipoElementIntf = subTipoElemItf_evi_arbol_00" => genera un evi que contendra la informacion de un arbol
    ///                     Si el evi origen ES UN EVI DE ARBOL (PENDIENTE MAFG 2021-03-14)
    /// 
    /// </summary>
    public void botonEditaEvi()
    {

        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        // 1.) Miramos si el evi ya ha sido expandido y su rama de edicion generada

        // OOOJOOOO estamos suponiendo que un evi expandido siempre tiene un hijo rama asociado. Esto puede no ser asi, por ejemplo en la expansion de un nodo buscador
        // que abre el resultado en el mismo muro donde reside Hay que revisar pues el if que sigue atendiendo a esta caracteristica. PENDIENTE MAFG 2021-03-17)

        // Buscamos una rama hija de este evi base. Si lo hemos expandido o es una rama hija, debe haber un hijo rama de este evi
        //  OJOO la lista de hijos esta en el EVI base, donde estamos
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
        // 2.) Si el evi ya ha sido editado, tendra creada como hija la rama de edicion. Nos colocamos e la rama de edicion asociada, en el primer muro, para navegar por ella

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

            // Generamos el evi de referencia para la miga de pan MAFG 2022-10-12
            ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(this.transform.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera);
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generacion_EviRefElemen_ocupada = true; // ver definicion de "generacion_EviRefElemen_ocupada"


            // COlocamos al usuario en el prime muro de la ramma en la que acabamos de entrar
            int direccion = 1;  // La direccion es 1, porque vamos hacia adelante
            float distancia = ramaAsociada.GetComponent<ScriptCtrlRama>().calculaLongitudTramoRama();
            Usuario.GetComponent<ScriptCtrlUsuario>().iniciaTransicion(direccion, distancia);

        } // Fin de - if (ramaAsociada != null)
        // ///////////////////////////////////////////
        // ///////////////////////////////////////////
        // 3.) Si el evi no ha sido editado, no se ha generado la rama de edicion. Se genera la rama de edicion correspondiente y se expande el evi en modo edicion
        else
        {
            // Ponemos en amarillo el boton de opciones para indicar que el evi esta en edicion
            GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;


            // ///////////////////////////////////////////
            //   3.1.) Evi subtipo de gestion de rama "subTipoElementIntf = subTipoElemItf_evi_rama" => Por ahora no se editan los evis de rtama (MAFG 2021-07-08)
            if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), los evi de rama no se editan (MAFG 2021-07-08) "); }
            }  // Fin de - if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_rama)
               // ///////////////////////////////////////////
               //  3.2.) Evi subtipo referencia fractal " subTipoElementIntf = subTipoElemItf_evi_RefFractal" => genera una rama de edicion, segun se comenta en las observaciones de esta funcion
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseRefFractal)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.2, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }

                // ///////////
                // Generamos una nueva rama
                // La rama se genera como hija de la rama activa y en este caso asociada a este evi () que la ha generado
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                // Como estamos editando, el modo de la rama sera en edicion
                ramaAsociada = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaRama(gameObject, ScriptDatosElemenItf.modoElemItf_edicion);

                // OJOOO hemos generado la rama, pero la rama no genera el muro asociado si no hasta el siguiente frame (Ver "ScriptCtrlRama => generaRama")
                // Por lo que no tenemos muro donde colocar los evis que vayamos generando.
                // Esperamos a que la rama tenga el muro inicial, y despues, vamos generando en este los evis que expanden el elemento que estamos expandiendo
                // utilizamos para ello una corrutina 

                StartCoroutine(esperaMuro());

                // Indicamoss que el evi esta en edicion
//                this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_edicion; 
                this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_cabezaEdicion; 

                // Ponemos en amarillo el boton de opciones para indicar que el evi esta en edicion
                //                GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;
                Btn_BaseDeEvi_N1_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;
                Btn_BaseDeEvi_N2_1_op_Editar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnEditarEnEdicion;
                Btn_BaseDeEvi_N3_1_op_Editar_Grabar.SetActive(true);
                Btn_BaseDeEvi_N3_1_op_Editar_Salir.SetActive(true);

                // Generamos el evi de referencia para la miga de pan MAFG 2022-10-12
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(this.transform.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera);
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generacion_EviRefElemen_ocupada = true; // ver definicion de "generacion_EviRefElemen_ocupada"

            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_RefFractal)
               // ///////////////////////////////////////////
               //  3.3.) Evi subtipo instancia fractal " subTipoElementIntf = subTipoElemItf_evi_InstFractal" => genera una rama de edicion, segun se comenta en las observaciones de esta funcion
               //   (PENDIENTE MAFG 2021-03-14)
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseInstFractal)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.3, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                // ///////////
                // Generamos una nueva rama
                // La rama se genera como hija de la rama activa y en este caso asociada a este evi () que la ha generado
                // El nuevo nuevo muro se genera desde el muro en el que nos encontramos, que debe ser el muro activo
                // Como estamos editando, el modo de la rama sera en edicion
                ramaAsociada = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.GetComponent<ScriptCtrlRama>().generaRama(gameObject, ScriptDatosElemenItf.modoElemItf_edicion);

                // OJOOO hemos generado la rama, pero la rama no genera el muro asociado si no hasta el siguiente frame (Ver "ScriptCtrlRama => generaRama")
                // Por lo que no tenemos muro donde colocar los evis que vayamos generando.
                // Esperamos a que la rama tenga el muro inicial, y despues, vamos generando en este los evis que expanden el elemento que estamos expandiendo
                // utilizamos para ello una corrutina 

                StartCoroutine(esperaMuro());

                // Indicamoss que el evi esta en edicion
                //                this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_edicion; 
                this.gameObject.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_edicion; // Las instancias no son cabeza de edicion

                // Ponemos en amarillo el boton de opciones para indicar que el evi esta en edicion
                //                GetComponent<ScriptCtrlBaseDeEvi>().Btn_Evi_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;
                Btn_BaseDeEvi_N1_Opciones.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnOpcionesEnEdicion;
                Btn_BaseDeEvi_N2_1_op_Editar.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MaterialBtnEditarEnEdicion;
                Btn_BaseDeEvi_N3_1_op_Editar_Grabar.SetActive(false); // La edicion de una instancia no se graba. Solo se puede grabar el concepto del que la instancia es parte del arbol de descripcion
                Btn_BaseDeEvi_N3_1_op_Editar_Salir.SetActive(true);

                // Generamos el evi de referencia para la miga de pan MAFG 2022-10-12
                ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(this.transform.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera);
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generacion_EviRefElemen_ocupada = true; // ver definicion de "generacion_EviRefElemen_ocupada"

            }  // Fin de - 
               // ///////////////////////////////////////////
               //  3.4.) Evi subtipo sin techo " subTipoElementIntf = subTipoElemItf_evi_sinTecho_00" => Edita el evi sin techo. Permite cambiar el tipo de dato y el dato que contiene 
               // (PENDIENTE MAFG 2021-03-14)
               //         - 2021-12-aprox. Incluyo la posibilidad de editar el texto de los evis sin techo
               //         - 2022-02-27. Me pongo a programar la funcionalidad para dar la posibilidad de asociar ficheros (imagen, video, audio, otros...) al dato
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_baseSinTecho_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.4, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                // - Cuando un evi Sin techo esta en un muro de edicion, es sensible al cambio de tipo de dato. Si un concepto (evi de referencia) se arrastra sobre su caja de tipo de dato
                // este se activa (se agranda y suena). Si el evi de referencia se suelta soble la caja, el tipo de dato pasa a ser el concepto que se ha arrastrado sobre la caja. OJOOO
                // puede arrastrarse cualquier concepto, pero la interfaz KEE solo conoce algunos tipos de dato (texto, entero, url, ficheros jpm. wav. etc...) si la interfaz no conoce
                // el tipo de dato, siempre lo tratara como texto plano
                //   3.4.1.) Un evi sin techo, solo puede editarse si se encuentra en un muro de edicion, comprobamos que es asi
                if (transform.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
                {
                    // 3.4.2.) Dependiendo del tipo de dato, al editar el evi actuamos en consecuencia
                    // Obtenemos el tipo de dato del sin techo
                    XmlNode  nodo_Z_DelFractum = transform.GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi.FirstChild;
                    string[] esteTipoDeDatoSinTecho = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().dameTipoDeDatoSinTecho(transform.GetComponent<ScriptCtrlBaseDeEvi>().domPropio, nodo_Z_DelFractum);
                    // nos vale con el key y el host, las versiones nos dan igual
                    string tipoDeDatoSinTecho_key = esteTipoDeDatoSinTecho[0];
                    string tipoDeDatoSinTecho_host = esteTipoDeDatoSinTecho[1];

                    //   3.4.2.1.) Para datos  "gen_tipoDeSinTechoTextoPlano" => Haboilitamos la edicion del texto
                    //   3.4.2.2.) Para datos  "gen_tipoDeSinTecho_NumeroEntero" => Haboilitamos la edicion del texto para introducir un numero entero
                    //   3.4.2.3.) Para datos  "gen_tipoDeSinTecho_Url" => Haboilitamos la edicion del texto para introducir la url pertinente
                    //    if (
                    //        ((tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_Key) ||
                    //        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_NumeroEntero_Key) ||
                    //        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_Url_Key))
                    //        &
                    //        (tipoDeDatoSinTecho_host == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_host)
                    //        )
                    // Por ahora tratamos igual a "gen_tipoDeSinTechoTextoPlano", "gen_tipoDeSinTecho_NumeroEntero" y "gen_tipoDeSinTecho_Url"
                    // Controlamos tambien que todabia no hemos generado el panel de edicion mediante "Panel_Input_Text_SinTecho == null"
                    // Si ya existe el panel, en caso de volver a pulsar el boton de edicion, no hacemos nada"
                    if (
                        ((tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_Key) ||
                        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_NumeroEntero_Key) ||
                        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_Url_Key))
                        &
                        (tipoDeDatoSinTecho_host == ConceptosConocidos.gen_tipoDeSinTechoTextoPlano_host)  // Todos estan el el DKS_klw
                        &
                        (Panel_Input_Text_SinTecho == null)
                        )
                    {
                        // Habilitamos la edicion del texto almacenado en el elemento T
                        // preparamos el elemento para la entrada de texto
                        //TMP_InputField Input_Text_T_deSinTecho = new TMP_InputField();

                        //                        GameObject Panel_Input_Text_SinTecho = new GameObject();

                        // Activamos la imagen de ayuda ainterfaz  de ayuda a interfaz
                        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().este_Panel_Input_Text_SinTecho.SetActive(true);

                    }
                    //   3.4.2.4.) Para cualquiera de los tipos de fichero conocidos, permitimos que el usuario busque (con los
                    //                                                                recursos del sistema) el fichero asociado al dato sin techo
                    // Por ahora tratamos igual a todo tipo de fichero
                    // Controlamos tambien que todabia no hemos generado el panel de edicion mediante "Panel_Input_Text_SinTecho == null"
                    // Si ya existe el panel, en caso de volver a pulsar el boton de edicion, no hacemos nada"
                    else if (
                        ((tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroGenerico_Key) ||
                        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroAudio_Key) ||
                        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroImagen_Key) ||
                        (tipoDeDatoSinTecho_key == ConceptosConocidos.gen_tipoDeSinTecho_FicheroVideo_Key))
                        &
                        (tipoDeDatoSinTecho_host == ConceptosConocidos.gen_tipoDeSinTecho_FicheroGenerico_host) // Todos estan el el DKS_klw
                        &
                        (Panel_Input_Text_SinTecho == null) // Creo que esto se puede quitar
                        )
                    {
                        // Aqui solo posibilitamos al usuario la seleccion de un nuevo fichero asociado a este sin techo. Para ello hacemos lo siguiente:
                        //  - Abrimos el buscador de ficheros del sistema
                        //      - Cuando el usuario ha seleccionado el fichero
                        //          - Obtenemos el nombre del fichero en el sistema "nombre_origen" y lo anotamos
                        //          - Seleccionamos un nuevo nombre para el fichero en unity "nombre_Unity" y lo anotamos
                        //          - Copiamos el fichero al directorio de Unity, para enviarlo posteriormente al DKS cuando el alta se confirme
                        // - OJO, si alguno de los valores de "nombre_origen" o "nombre_Unity" es un string vacio, esto quiere decir que no se ha modificado el fichero asociado al sin techo

                        //  3.4.2.4.1) Guardamos el fichero a enviar en el directorio "Assets\Resources\Temporales", con "un nuevo nombre especifico"
                        ///       para que cuando se proceda al alta, el KEE envie el fichero al DKS para que alli quede asociado a este Sin Techo

                        // TIpo de dato fichero. Hay que posibilitar al usuario buscar un fichero en su ordenador para importarlo a este sin techp (PENDIENTE MAFG 2021-09-02)
                        // Abrimos el navegador de ficheros del sistema operativo, para que el usuario seleccone el fichero que desee como objeto de este sin techo

                        // segun el tipo de fichero, ajustamos las extensiones posibles
                        //                        string[] extensions = { "image files", "png,jpg,jpeg" };
                        string[] extensions = { "All files", "*" };  // Esto habria que afinarlo segun el tipo de dato del sin techo (PENDIENTE MAFG 2022-03-05)
                                                                     // Abrimos la ventana de busqueda de ficheros del sistema


                        // Me da errores al compilar, lo quito provisionalmente para ver si funciona MAFG 2022-05-25
//                        var path_fichero_Seleccionado = "";
//                        #if UNITY_EDITOR
                        var path_fichero_Seleccionado = EditorUtility.OpenFilePanelWithFilters("KEE", "", extensions);
//                        #endif


                        string nombre_origen = Path.GetFileName(path_fichero_Seleccionado);
                        string extension = Path.GetExtension(path_fichero_Seleccionado);

                        // Generamos el nuevo nombre con el que vamos a almacenar el fichero en KEE
                        string nombre_enKee = ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().genera_identificador_unico(DatosGlobal.tipo_entidad_fichero);
//                        string nombre_enKee = "2009-12-24 nuevo fichero 06.JPG";
                        

                        string path_enKee = Application.dataPath 
                            + "/" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().nombre_Fich_Resources
                            + "/" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().locFicherosTemporales
                            + "/";
                        string fichero_destino = path_enKee + nombre_enKee + extension;

                        
                        File.Copy(path_fichero_Seleccionado, fichero_destino, true);
                        long Tiempo_copia_enKee = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().dime_milisegundo_actual();
                        long Tiempo_de_vida = 0;  // Cuando es igual a cero, quiere decir que no caduca (tiempo de vida infinito o hasta que alguien  decida borrarlo)

                        if (File.Exists(path_fichero_Seleccionado))
                        {
                            //  3.4.2.4.2.) Guardamos en la instancia de la clase "ClassFichero" asociada a este sin techo la informacion referente al fichero que sera
                            //        necesaria mas tarde para enviar el fichero asociado al sin dato desde el KEE hasta el DKS
                            //        para que cuando se proceda al alta, el KEE envie el fichero al DKS para que alli quede asociado a este Sin Techo
                            // Anotamos el nombre del nuevo fichero modificado en la variable del Sin techo que voy a generar "ultimo_fichero_asociado["nombre_origen"], ultimo_fichero_asociado["nombre_Unity"]"
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.nombre_origen = nombre_origen;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.extension = extension;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.nombre_enKee = nombre_enKee;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.path_enKee = path_enKee;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.modificado = true;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.Tiempo_copia_enKee = Tiempo_copia_enKee;
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.Tiempo_de_vida = Tiempo_de_vida;

                            //  3.4.2.4.3) En el dato T del sin techo Z, almacenamos el "nuevo nombre especifico" para que el sin techo y el fichero 
                            //        puedan asociarse posteriormente, tanto en el KEE como a su recepcion en el DKS
                            transform.GetComponent<ScriptCtrlBaseDeEvi>().texto_T_deSinTechoCanvasEviBase = nombre_enKee;

                            // Ojo, si cambio el tipo, de dato, hay que indicar que el dato cambia.
                            //      - Si es un dato de texto, se deja como esta
                            //      - Si pasa de fichero a texto, se pone en blanco
                            //      - Si cambia de tipo de fichero, se quita el fichero asociado y se le dice al usuario que el fichero asociado hay que definirlo de nuevo

                            if (DatosGlobal.niveDebug > 50)
                            { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.4.2.4, Desde el evi : " 
                                + transform.name 
                                + " - con path_fichero_Seleccionado : " + path_fichero_Seleccionado
                                + " - con nombre_origen : " + nombre_origen
                                + " - con extension : " + extension
                                + " - con nombre_enKee : " + nombre_enKee
                                + " - con path_enKee : " + path_enKee
                                + " - con Tiempo_copia_enKee : " + Tiempo_copia_enKee.ToString()
                                + " - con Tiempo_de_vida : " + Tiempo_de_vida.ToString()
                                + " - con modificado : " + transform.GetComponent<ScriptCtrlBaseDeEvi>().miFicheroAsociado.modificado
                                + " - con path_destino : " + path_enKee);
                            }
                        }

                        // Queda PENDIENTE MAFG 2021-10-22, el habilitar que al arrastrar un concepto al boton de info de un Sin Techo en edicion, este pase a ser del tipo del 
                        // concepto arrastrado (por ejemplo, si arrastramos el concepto url, pasa a ser un dato sin techo de tipo url. Hay que poner una consulta al usuario para confirmar que 
                        // desea realizar el cambio. Y si el tipo de dato no es conocido por el sistema, se debe hacer la pregunta con mas enfasis
                    }
                    //   3.4.2.5.) Para OTROS datos => (PENDIENTE MAFG 2021-09-02)
                    else
                    {
                        // Avisamos de que es un tipo de dato no conocido y lo tratamos como testo plano (PENDIENTE MAFG 2021-09-02)
                    } // FIn de else - de - if  de la opcion de tipo de dato fichero

                }  // Fin de -  if (transform.GetComponent<ScriptDatosElemenItf>().modo == ScriptDatosElemenItf.modoElemItf_edicion)
                else
                {
                    // Avisamos de que el EVI sin techo, solo es editable si el muro en el que se encuentra es un muro en edicion (PENDIENTE MAFG 2021-09-02)
                }


            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_sinTecho_00)
               // ///////////////////////////////////////////
               //  3.5.) Evi subtipo buscador 00 " subTipoElementIntf = subTipoElemItf_evi_buscador_00" => genera una rama de edicion de buscador (PENDIENTE MAFG 2021-07-08)
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.5, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI BUSCADOR (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_buscador_00)
               // ///////////////////////////////////////////
               //  3.6.) Evi subtipo lista 00 " subTipoElementIntf = subTipoElemItf_evi_lista_00" => genera un evi que contendra una lista de elementos de interfaz
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.6, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE LISTA (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_lista_00)
               // ///////////////////////////////////////////
               //  3.7.) Evi subtipo camino 00 " subTipoElementIntf = subTipoElemItf_evi_camino_00" => genera un evi que contendra la informacion de caminos de relacion entre conceptos
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.7, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE CAMINO (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_camino_00)
               // ///////////////////////////////////////////
               //  3.8.) Evi subtipo arbol 00 " subTipoElementIntf = subTipoElemItf_evi_arbol_00" => genera un evi que contendra la informacion de un arbol
            else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonEditaEvi(), paso 3.8, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }
                //   Si el evi origen ES UN EVI DE ARBOL (PENDIENTE MAFG 2021-03-14)
            }  // Fin de - else if (transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf() == ScriptDatosElemenItf.subTipoElemItf_evi_arbol_00)
               // ///////////////////////////////////////////
               //  Si llegamos aqui esque el evi es de un subtipo que no conocemos
            else
            {
                if (DatosGlobal.niveDebug > 1000)
                { Debug.Log(" En SctExpandirEvi => botonExpandeEvi(), per el else sin condicion, Desde el evi : " + transform.name + " - con SubTipoElementIntf : " + transform.GetComponent<ScriptDatosElemenItf>().dameSubTipoElementIntf()); }

            }  // Fin de - else (sin condiciones)

        }  // Fin de - else - de - if (ramaAsociada != null)

    } // Fin de - public void botonExpandeEvi()

    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// expandeDescripcion : Obtiene los elementos de descripcion adociados a un concepto o un enlace, y los expande en el elemento destino (normalmente un muro) 
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
//        XmlNode KDL_Nodo_C = KDL_Raiz; // COmo lo vemos como un get details, la raiz del KDL que hemos obtenido es la raiz del concepto
        XmlNode nodo_E_ContEnBaseDeEvi = GetComponent<ScriptCtrlBaseDeEvi>().nodo_E_ContEnBaseDeEvi;
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde expandeDescripcion con nodo name" + nodo_E_ContEnBaseDeEvi.Name); }

        ListaNodos_E_en_D = ctrlInterfaz.GetComponent<ScriptLibConceptosXml>().damelistaDescripcionDeEnlace(nodo_E_ContEnBaseDeEvi, manejadorEspNomb);


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
                    if (DatosGlobal.niveDebug > 1000)
                    { Debug.Log("ERROR. Desde SctExpandirEvi => expandeDescripcion. Pasamos por tipo DESCONOCIDO. Es de tipo : " + tipoEnlace); }
                }
            }  // Fin de - foreach (XmlNode nodoEnlace in ListaNodos_E_en_D)
        }  // Fin de - if (ListaNodos_E_en_D != null)
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde SctExpandirEvi => expandeDescripcion. Ya he pasado la lista de descripcion"); }
    }  // Fin de - public void expandeDescripcion(GameObject elemDestino)


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
