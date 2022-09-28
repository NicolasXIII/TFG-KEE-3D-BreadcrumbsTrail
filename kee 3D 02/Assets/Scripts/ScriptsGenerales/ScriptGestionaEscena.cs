using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/* **************************************************************************************************
*****************************************************************************************************
                        Script de Gestion de la interfaz 
*****************************************************************************************************
*****************************************************************************************************
Gestiona la interfaz, las escenas, las ramas, los muros , sus evis hijosa y sus ramas hijas
  La informacion del estado de la escena esta en el fichero DAUS de usuario, que es un KDL del concepto de DAUS
Autor : 	Miguel Angel Fernandez Graciani
Fecha :	2020-03-30
Modificaciones :
      - 2020-07-20, incluyo los accesos reales a los DAUS remotos y comienzo a programar la gestion del DAUS y la interfaz
Observaciones :
 		- El fichero DAUS es un KDL instancia de DAUS que contiene la configuracion de la interfaz, SU estructura basicamente es:
               INTERFAZ DE USUARIO
                  - Escena(s)
                  - Raiz del arbol (rama principal)
                      - * Muro(s)
                          - EVI(s)
                              - Rama(s)
                                  - Muro(s) Recursivamente a *
                                      Las ramas tienen muros, en los que hay evis de los que pueden salir ramas con otros muros que tiene otros evis y asi sucesivamente
                  - Historial (ultimas localizaciones del usuario
                  - Estado de usuario 
                      - muro de usuario

                      - Opciones (sonido, idiomas, etc...)
                      - Mochila
                      - Agentes

      - Todos los conceptos del DAUS (escenas, ramas, muros y evis) tienen su Key y su Host.
              - El Key lo va proporcionando el gestor de interfaz, con la intencion de que no se repita ninguno en toda la interfaz
              - EL host de todos los conceptos de la interfaz es el mismo. Lo llamamos
                          host = "raizDelConcepto"
                  Esto es asi porque son entidades que tienen sentido dentro del concepto total (interfaz de usuario en este caso), pero 
                  NO tienen sentido furera de estos (si llegaran a tenerlo algun host los apadrinara)

	DATOS GENERALES :
        - De todas las entiodades
              - Key (el KEY es unico en la interfaz y lo define este script para que no este duplicado)
              - Host = "raizDelConcepto"  // Para todos salvo el concepto General que instancia el DAUS ( que tendra de host el que lo hospede y evidentemente
                                          todos los conceptos que se referencian o nstancian
             Todas las entidades instancian a escena, rama, muro o evi
        - Escena :
              - Nombre de escena
              - Descripcion
              - Ordinal
              - Fecha de creacion
              - Fecha de ultima modificacion
        - Rama :
              - Rama madre
              - EVI Padre
              - Punto de origen
              - Direccion global
              - Distancia entre muros
        - Muro
            - Rama en la que esta
            - Posicion global
            - Factor de escala
        - Evi
            - Muro en el que esta
            - Posicion global
            - Factor de escala
            - Expandifo SI/NO
            - Minimizado SI/NO


*****************************************************************************************************
	METODOS GENERALES
        (ver el documento "D:\Datos\Ideando\Desarrollo\KLW\KEE3D\Documentos de especificacion/Especificacion funcional.docx". 
        Mas concretamente el apartado de o	Botones de control de interfaz (generales a toda la interfaz))

Metodos Generados



Metodos a generar
    




 * *****************************************************************************************************
*****************************************************************************************************
*****************************************************************************************************
***************************************************************************************************** */

public class ScriptGestionaEscena : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

//    public GameObject MiBaseDeEvi_01;

    
    // Para pruebas cargamos el DAUS de Paulino
    public string rutaACargar = "http://crab.uclm.es/klw/pruebasPau/uploads/gen_miDAUS_limpio.xml";
    //       ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().DausInterfaz

    int numeroDeUpdate = 0;

    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Cargar el Daus del usuario si existe

        // Cargamos los datos del EVI, llamando a su DKS correspondiente
        // Arrancamos una corrutina para que traiga el KDL de forma asincrona (sin detener la ejecucion)
        StartCoroutine("cargaDausInterfaz");


        // /////////////////////////////////////////
        // Configurar la interfaz de usuario segun el daus


        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // ////////////////////////////////////////
        // Si no hay DAUS de usuario, cargamos tan solo una rama raiz y un muro en dicha rama, donde el usuario podrá comenzar a generar el arbol

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Generamos la primera rama raiz del arbol
        GameObject ramaRaiz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Rama);
        string nombreRamaInicial = "Rama_ini_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas;
        ramaRaiz.name = nombreRamaInicial;

        // Antes de seguir hay que definir el tipo de elemento de interfaz que es "tipoElemItf_rama"
        ramaRaiz.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_rama);
        // En principio las ramas no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"


        // En este caso y por ahora, esta es la rama raiz del arbol (luego podrá cambiar
        // Obtenemos el identificador general de elemento de interfaz de la rama creada
        int idRamaMadre = ramaRaiz.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf();
        // La ponemos como rama raiz y como rama activa
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().raizActual = ramaRaiz;
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo = ramaRaiz;

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas++;

        // Indicamos que el arbol esta recien generado, para que el usuario se coloque en la raiz
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().arbolRecienGenerado = true;

        // ////////////////////////////////
        // Ponemos ahora al usuario como hijo de esta rama, como posicion inicial. Luego tendra que ser segun el DAUS
        Usuario.transform.SetParent(ramaRaiz.transform);

        float distanciaUsuario = 0;
        Vector3 posicionUsuario = new Vector3(0.0f, 0.0f, distanciaUsuario);
        Usuario.transform.localPosition = posicionUsuario;
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        Usuario.transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rotacionLocalnUsuario;

        // Indicamos que el usuario esta en esta rama 
        Usuario.GetComponent<ScriptCtrlUsuario>().rama_EnLaQueEstaElUsuario = ramaRaiz;

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde Generador => Start con idRamaMadre = " + idRamaMadre); }

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Generamos el muro inicial
        GameObject otroMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro);
        otroMuro.transform.SetParent(ramaRaiz.transform);
        Vector3 posicionMuro = new Vector3(0.0f, 0.0f, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros);
        otroMuro.transform.localPosition = posicionMuro;
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        otroMuro.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Ponemos el nombre del gamaobject al muro
        string nombreMuro = "Muro_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros;
        otroMuro.name = nombreMuro;

        // Antes de seguir hay que definir el tip de elemento de interfaz que es "tipoElemItf_muro"
        otroMuro.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_muro);
        // En principio los muros no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"

        // Lo ponemos como muro activo. Por ahora es el unico que hay
        // No hace falta, debe ponerse activo al colisionar con el triger del 
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = otroMuro.gameObject;
        otroMuro.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro;   // materialMuroActivo
        // El muro inicial comienza en modo navegacion
        otroMuro.GetComponent<ScriptDatosElemenItf>().modo = ScriptDatosElemenItf.modoElemItf_navegacion;

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros++;

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("he entrado en`poner el primer muro con numMuros = " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros); }

    }  // Fin de - void Awake()




    // Inicializamos la escena
    void Start () {

        /* *************************************************

        // Vamos generando las ramas a partir de la rama raiz. Esto es por ahora para pruebas, 
        // HABRIA QUE CARGAR LAS DEL DAUS (PENDIENTE MAFG 2021-02-20)
        GameObject ramaRaiz = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().raizActual;
        int idRamaMadre = ramaRaiz.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf();

        for (int r = 0; r < 6; ++r)
        {
            // //////////////////////////////////
            // Generamos una nueva rama
                // Definimos los parametros siguientes para ña generacion en pruebas
            float distanciaEntreRamas = 5f;
            float anguloEntreRamas = 45;

            GameObject otraRama = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Rama);
            otraRama.transform.SetParent(ramaRaiz.transform);
            // Vamos con la posicion de la rama  en la referencia de la madre
            float distanciaRama = r * distanciaEntreRamas;
            Vector3 posicionRama = new Vector3(0.0f, distanciaRama, 0.0f);
            otraRama.transform.localPosition = posicionRama;
            // Vamos con la direccion de la rama  en la referencia de la madre
            float anguloRama = r * anguloEntreRamas;
            otraRama.transform.localRotation = Quaternion.Euler(anguloRama, 0, 0);

            string nombreRama = "Rama_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas;
            otraRama.name = nombreRama;

            // Antes de seguir hay que definir el tip de elemento de interfaz que es "tipoElemItf_rama"
            otraRama.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_rama);
            // En principio las ramas no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"

            // ///////////////////////////////////////////////
            // Añadimos la rama al array que define el arbol completo
            // Quien genera la rama es quien sabe cual es la rama madre y por tanto quien incluye la rama recien creada en el array "arbolIntf"
            // Obtenemos el identificador de la rama creada
            int idEstaRama = otraRama.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf();
            // Ahora el de la rama madre
            int ramaMadre = idRamaMadre;

            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ahijaRama(ramaMadre, idEstaRama);

            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas++;

            // Vamos generando y colocando los muros
            for (int m = 0; m < 4; ++m) 
		    {
			    GameObject otroMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro);
			    otroMuro.transform.SetParent(otraRama.transform);
			    Vector3 posicionMuro = new Vector3 (0.0f, 0.0f, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros);
			    otroMuro.transform.localPosition = posicionMuro;
                    // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
                otroMuro.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    // Ponemos el nombre del gamaobject al muro
                string nombreMuro = "Muro_"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros;
                otroMuro.name = nombreMuro;

                // Antes de seguir hay que definir el tip de elemento de interfaz que es "tipoElemItf_muro"
                otroMuro.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_muro);
                // En principio los muros no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"

                // Si es el primer muro lo ponemos como muro activo. 
                // No hace falta, debe ponerse activo al colisionar con el triger del 
                if ((m == 0) & (r == 0)) {
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = otroMuro.gameObject;
                    otroMuro.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro;   // materialMuroActivo

                    Debug.Log("he entrado en`poner el primer muro como el muro activo con m = " + m.ToString());
                }

                // Generamos unos pocos evis en el muro aleatoriamente
                for (int e = 0; e < 3; ++e) 
			    {
                    // Pongo bases de EVI o prototipos de prueba fractal alternativamente, para probar que van todos (2020-12-23)
                    if (e == 1)
                    {
                        //                    GameObject otroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01) as GameObject;
                        GameObject otroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01) as GameObject;
                        otroEvi.transform.SetParent(otroMuro.transform);

                        // No se muy bien donde se posiciona el evi
 //                       float distanciaEvi = e * (distanciaEntreEvi + 1);
 //                       Vector3 posicionEvi = new Vector3(0.0f, 0.0f, distanciaEvi);
 //                       otroEvi.transform.localPosition = posicionEvi;
                        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
                        otroEvi.transform.localRotation = Quaternion.Euler(0, 0, 0);

                        // Antes de cargar los datos hay que definir el tip del EVI
                        otroEvi.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);

                        // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
                        otroEvi.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptDatosElemenItf.subTipoElemItf_evi_EviPrue_001);
                        otroEvi.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi_Especifico();
                        int idObjeto = otroEvi.GetInstanceID();
    //                    string nombreDeEvi = "Evi_"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numEvis;
    //                    otroEvi.name = nombreDeEvi;
                        Debug.Log("he entrado en`poner evi  EviPrue_001 con m = " + m + " - y con evi = " + e+" -y con ID : "+ idObjeto+" - y nombre : "+ otroEvi.gameObject.name);
                    }

                    else
                    {
                        //                    GameObject unOtroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01) as GameObject;
    //                    GameObject unOtroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01) as GameObject;
                        GameObject unOtroEvi = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BaseDeEvi_01) as GameObject;
                        unOtroEvi.transform.SetParent(otroMuro.transform);

                        // No se muy bien donde se posiciona el evi
                        //                       float distanciaEvi = e * (distanciaEntreEvi + 1);
                        //                       Vector3 posicionEvi = new Vector3(0.0f, 0.0f, distanciaEvi);
                        //                       unOtroEvi.transform.localPosition = posicionEvi;
                        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
                        unOtroEvi.transform.localRotation = Quaternion.Euler(0, 0, 0);



                        // Antes de cargar los datos hay que definir el tip del EVI
                        unOtroEvi.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_evi);
                            // Estamos cargando un evi especifico a pelo, por lo que cargamos tambien el subtipo
                        unOtroEvi.GetComponent<ScriptDatosElemenItf>().ponSubTipoElementIntf(ScriptCtrlBaseDeEvi.ESTO YA NO SE USA subTipoEvi_Base);
                        unOtroEvi.GetComponent<ScriptCtrlBaseDeEvi>().cargaDatosBaseDeEvi_Especifico();
                        int idObjeto = unOtroEvi.GetInstanceID();
    //                    string nombreDeEvi = "Evi_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numEvis;
    //                    unOtroEvi.name = nombreDeEvi;
                        Debug.Log("he entrado en`poner evi Base con m = " + m + " - y con evi = " + e + " -y con ID : " + idObjeto + " - y nombre : " + unOtroEvi.gameObject.name);
                    }

                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numEvis++;

                } // Fin de - for (int evi = 0; evi < 5; evi++) 
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros++;
		    } // Fin de - for (int i = 0; i < 5; i++) 




        } // Fin de - for (int r = 0; r < 3; ++3)

        // Indicamos que el arbol esta recien generado, para que el usuario se coloque en la raiz
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().arbolRecienGenerado = true;

        ************************************************* */


    } // Fin de - void Start () {

    // Update is called once per frame
    void Update () {

        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame++;

        // Hay cosas que si se hacen antes del primer update, se mezclan de forma rara por la secuencia que lleva Unity al ir arrncando los objetos
        // por eso, en el primer frame cuando ya se han generado todos los objetos iniciales, ordenamos los parametros y asignamos algunos roles de forma adecuada
        // Macemos lo que toca en el primer cuadro
        if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame == 1)
        {
            // al arrancar se producen extraños trigers, supongo por la situación inicial de los gameobjet. En este caso
            // el muro activo se me activava con otro muro que no era con el que queria arrancar porque se activava como 
            // muro activo algun muro por la colisión con el triger del puntero de usuario (supongo). Por lo que cuando ya hemos 
            // arrancado pongo como muro activo el cero, que es en el que arranco
            //      Nota: cuando cargue el daus habra que poner como muro activo, el que indiaque el DAUS (2021-01-18)


            GameObject MuroAActivar = GameObject.FindWithTag("MuroDeTrabajo");
            MuroAActivar.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro;   // materialMuroActivo
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = MuroAActivar.gameObject;


        }

        // SI estamos en pruebas y hace ya un ratito que hemos empezado, preparamos la interfaz para que sea mas comoda durante el desarrollo
        //      1.) Generamos algunos evis y los cargamos en el muro de inicio para que no haya que ir a por ellos cada vez que arrancamos.
        //              (ire modificando estos segun elmodulo que este desarrollanado
        if ((ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame == 5) & (DatosGlobal.niveDebug > 50))
        {
            // Generamos los parametros globales para generar los evis en el muro activo
            string cualificador_DeEvi = "0";
            string ordinalConf_DeEvi = "0";
            DateTime ultiModConf_DeEvi = new DateTime(0);
            // Para obtener el objeto de telon donde van a ir los evis en pruebas
            // OJOOOO esto puede hacerse mientras no tengamos mas que un telon. POr ahora lo hacemos asi para simplificar (PENDIENTE MAFG 2022-01-01)
            GameObject elemDestino_DeEvi = GetComponent<ScriptDatosInterfaz>().muro_Activo; ;  // Los evis iran a la tramoya 

            // Vamos generando cada evi de los que nos hacen falta

            // Generamos un evi de "gen_recAyuIntf" de "dks_klw"
            string key_DeEvi = "gen_BuscadorKee_por_key_host";
            string host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_ParaConceptoNuevo" de "dks_klw"
            key_DeEvi = "gen_ParaConceptoNuevo";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_recAyuIntf" de "dks_klw"
            key_DeEvi = "gen_recAyuIntf";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_idioma_eapañol" de "dks_Languajes"
            key_DeEvi = "gen_idioma_español";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Languajes";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_casa" de "dks_desarrollo"
            key_DeEvi = "gen_casa";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_desarrollo";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_datosLudicos" de "dks_desarrollo"
            key_DeEvi = "gen_datosLudicos";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_desarrollo";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_datosLudicos" de "dks_desarrollo"
            key_DeEvi = "gen_prueba_001";
            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_desarrollo";
            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);

            // Generamos un evi de "gen_datosLudicos" de "dks_desarrollo"
//            key_DeEvi = "gen_I_43_C_";
//            host_DeEvi = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_desarrollo";
//            GetComponent<ScriptLibGestorEvis>().generaEviFractalRef(key_DeEvi, host_DeEvi, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);


            // Asignamos a las variables de este game object
            string tipoDato_key = "gen_tipoDeSinTechoTextoPlano";
            string tipoDato_host = "http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw";

//            GetComponent<ScriptLibGestorEvis>().generaEviSinTecho_sinDOM(tipoDato_key, tipoDato_host, cualificador_DeEvi, ordinalConf_DeEvi, ultiModConf_DeEvi, elemDestino_DeEvi);



        }


    }  // Fin de - void Update () {



    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cerrarAplicacion(), Cierra la aplicacion
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 2022-01-13
    /// Ultima modificacion :
    ///
    /// Variables de entrada :
    /// Variables de entrada :    
    /// Observaciones:
    ///         - Esta funcion cierra la aplicacion. Debe preguntar antes al usuario :
    ///             - ¿Desea salir realmente?
    ///                 - Responde si => continua
    ///                 - Responde no => vuelve a la aplicacion en el estado en el que estaba
    ///             - ¿Desea guardar el estado actual?
    ///                 - Responde si => envia el DAUS al DKS madre
    ///                 - Responde NO => sale de la aplicacion sin salvar OJOO por ahora solo hacemos esto PENDIENTE MAFG 2022-01-13
    ///         
    /// </summary>
    public void cerrarAplicacion()
    {
        // FALTA TODO ESTO, PENDIENTE MAFG 2022-01-13
        ///             - ¿Desea salir realmente?
        ///                 - Responde si => continua
        ///                 - Responde no => vuelve a la aplicacion en el estado en el que estaba
        ///             - ¿Desea guardar el estado actual?
        ///                 - Responde si => envia el DAUS al DKS madre
        ///                 - Responde NO => sale de la aplicacion sin salvar OJOO por ahora solo hacemos esto PENDIENTE MAFG 2022-01-13

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScriptGestionaEscena => cerrarAplicacion. Intento derrar la aplicacion en frame : " + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numDeFrame); }


        Application.Quit();  // OJOOO NO cierra si lo corremos desde el entorno de unity. Para que cierre hay que hacer un ejecutable (asi si cierra)

    }  // Fin de - public void generaElementEnTipo(string tipoDelElemento)


    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    //   CORRUTINAS
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////
    /// /////////////////////////////////////////////////////////////////


    /// <summary>
    /// /////////////////////////////////////////////////////////////////
    /// Metodo : cargaDausInterfaz(), Carga el DAUS del usuario y configura la aplicacion atendiendo al DAUS obtenido
    /// 
    /// Autor : Miguel Angel Fernandez Graciani
    /// Fecha creacion : 202x-xx-xx
    /// Ultima modificacion :
    ///
    /// Variables de entrada :
    /// Variables de entrada :    
    /// Observaciones:
    ///         
    /// </summary>
    IEnumerator cargaDausInterfaz()
    {

        UnityWebRequest www = UnityWebRequest.Get(rutaACargar);
        yield return www.Send();

        if (www.isNetworkError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(www.error); }
        }
        else
        {
            // Show results as text
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(www.downloadHandler.text); }
        }

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Aqui fin de la carga del daus de escena"); }

    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()





}
