using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  public class ScriptCtrlRama :  para gestionar ramas
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-02-11?
/// Observaciones :
///     -Ver "ScriptGestorTransitos"
///     
/// </summary>    
public class ScriptCtrlRama : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public bool debeGenerrMuroInicial;

    public GameObject elemIntfQueLaOrigina;

    //    public int ordinalMuroRaiz;

    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // Ponemos a false la necesidad  de generar un muro. esto debe ponerlo a uno queien genera la rama
        debeGenerrMuroInicial = false;
        elemIntfQueLaOrigina = null;

    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {

        if (debeGenerrMuroInicial)
        {
            // //////////////////////////////////////////////////////////
            // Generamos un muro para que la rama no este vacia

            // Generamos el muro. La transicion hasta este se realiza automaticamente al generarlo
            // Segun el modo en el que estemos (navegacion o edicion), generamos asi el muro en ese modod
            generaMuro(gameObject, this.GetComponent<ScriptDatosElemenItf>().modo);

            debeGenerrMuroInicial = false;
        }
    }


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Genera una Rama nueva, hijo de la rama que lo ejecuta y localizada en el muro, evi o rama desde el que se crea
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-28
    /// Fecha :	2021-07-02
    /// Variables de entrada :
    ///     - GameObject objDesdeDondeSolicita
    ///     - string modo = modo en el que se debe crear la rama (ver  ScriptDatosElemenItf => public string modo)
    /// Observaciones :
    ///         - Un a rama se puede generar desde un evi. En cuyo caso es hija de la rama a la que pertenece el muro al que pertene el evi que la generó 
    ///         - Un a rama se puede generar desde un muro. En cuyo caso es hija de la rama a la que pertenece el muro
    ///         - Un a rama se puede generar desde una rama. En cuyo caso es hija de dicha rama
    ///         
    /// <summary>
    public GameObject generaRama(GameObject objDesdeDondeSolicita, string modo)
    {

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde ScriptCtrlRama => generaRama desde el objeto : " + objDesdeDondeSolicita.name); }

        float distancia = 0f;  // Para ver donde tenemos que poner el muro


        /* **********************************  Esto lo quito porque ya no se usa creo 2021-03-14
        int maxOrdinalHijoMuro = 0; // Ponemos el ordinal a cero, para obtener el ultimo en el foreach

        // /////////////////////////////////////////////////////////////
        // Controlamos si el muro desde donde se crea es el ultimo de esta rama

        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                // Si el hijo es un muro 
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro) // Miramos si la solicitud requiere atencion
                {
                    // Vamos seleccionando el ordinal maximo
                    if (hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo > maxOrdinalHijoMuro) { maxOrdinalHijoMuro = hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo; }
                } // FIn de - if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)
        *****************************  */

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Generamos la rama
        GameObject otraRama = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Rama);
        otraRama.transform.SetParent(transform);
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        otraRama.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        // Ponemos el nombre del gamaobject al muro
        string nombreRama = "Rama_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas;
        otraRama.name = nombreRama;
        otraRama.GetComponent<ScriptDatosElemenItf>().modo = modo;

        // Antes de seguir hay que definir el tip de elemento de interfaz que es "tipoElemItf_muro"
        otraRama.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_rama);
        // En principio las ramas no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"

        // Definimos quien genero esta rama (no tiene por que ser su padre, las ramas son hijas de ramas, pero las pueden generar muros, evis, agentes, etc...
        // nos hara falta ya que cuando se elimina a quien la genero, eliminaremos tambien esta rama
        otraRama.GetComponent<ScriptCtrlRama>().elemIntfQueLaOrigina = objDesdeDondeSolicita;

        // Lo ponemos como muro activo. Por ahora es el unico que hay
        // No hace falta, debe ponerse activo al colisionar con el triger del 
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo = otraRama.gameObject;

        // Reorganizamos el resto de muros antes de localizar el nuevo
        //        reordenaRama(objDesdeDondeSolicita, otraRama, ScriptDatosInterfaz.añadir);

        // Colocamos la rama recien creada, 
        // Primeramente calculamos el angulo que forma la rama (girando en X y en Y) con respecto al eje z
        distancia = calculaLongitudTramoRama();
        Quaternion rotacionRama;
        if (objDesdeDondeSolicita == null)  // Esto es para el caso en el que se genera el primer elemento (no hay gameObjet que solicite la generacion de elemento)
        {
            rotacionRama = Quaternion.Euler(0f, 45f, 0f);
        }
        else  // Para el caso normal en el que la solicitud de generacion se hace desde un elemento que existe
        {
            rotacionRama = calculaAnguloRama();
        }
        otraRama.transform.localRotation = rotacionRama;

        // Colocamos ahora la rama en el eje z. La corrdenada z es la misma que el muro donde se genera, es decir, el muro activo 
        Vector3 posicionMuroActivo = new Vector3(0.0f, 0.0f, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform.localPosition.z);
        otraRama.transform.localPosition = posicionMuroActivo;

        // ///////////////////////////////
        // Vamos con las listas

        // La rama es hija tipo rama, de la rama que la crea
        // OJOOO en el script "ScriptDatosElemenItf => Start (), al generar el elemento de interfaz, se ahija a este con su padre (transform.parent) 
        // en la gerarquia del juego, por lo que no hay que ahijarlo aqui. Por eso la linea siguiente esta comentada
        //       GetComponent<ScriptDatosElemenItf>().listaDeHijos.Add(otraRama);
        // Añadimos la informacion en el array de ramas del arbol
        //  public List<int[]> arbolIntf;

        // La rama es hija del elemento de interfaz que la genera, salvo qe lo haya solicitado la misma rama de la que es hija
        if (objDesdeDondeSolicita != this.gameObject)
        {
            // ahijamos la rama en el elemento que la solicita (en la gerarquia que no es unity)
            otraRama.GetComponent<ScriptDatosElemenItf>().gestionaArbolGenealogico(objDesdeDondeSolicita.gameObject, "ahija");

        }

        // Actualizamos el numero total de ramas
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numRamas++;

        // //////////////////////////////////////////////////////
        // definimos la rama como la rama activa 
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo = otraRama;

        // ///////////////////////////////////////////
        // OJOOOO. ahora deberiamos generar el muro inicialasociado a la rama, pero si lo hacemos aqui no funciona. Aunque la rama ya se ha generado
        // no esta lista para tener el muro como hijo (no recuerdo muy bien porque, lo estoy comentando a posteriori). Por lo que utilizamos la variable
        // "debeGenerrMuroInicial", para que la rama genere su muro inicial en el primer update
        otraRama.GetComponent<ScriptCtrlRama>().debeGenerrMuroInicial = true;

        return otraRama;

    }  // Fin de -  public void generaRama(GameObject objDesdeDondeSolicita)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Genera un muro nuevo, hijo de la rama que lo ejecuta
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-26
    /// Variables de entrada :
    ///     - GameObject objDesdeDondeSolicita
    ///     - string modo = modo en el que se debe crear el muro (ver  ScriptDatosElemenItf => public string modo)
    /// Observaciones :
    /// <summary>
    public void generaMuro(GameObject objDesdeDondeSolicita, string modo)
    {
        // La generacion de un nuevo muro requiere que el usuario no este en transito, por lo que primero miramos a ver si el sistema esta
        // en transito antes de comenzar a generarlo. Si el sistema esta entransito, le hacemos saber al usuario, mediante el objeto "" que aparecera y se 
        // auto destruira cuando sea posible iniciar la generacion del muro
        // E OJOOO el script "ScriptGestorTransitos" reside en el objeto "ctrlInterfaz" 
        string tipoTransito = ScriptGestorTransitos.transTipo_usuarioEnMovimiento;
        string hayTransito = ctrlInterfaz.GetComponent<ScriptGestorTransitos>().enTransito(gameObject, tipoTransito);
        if (hayTransito != ScriptGestorTransitos.transTipo_ninguno)  // Tenemos algun transito pendiente y no debemos generar el muro
        {
            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log(" Desde generaMuro. No pudo generar el muro porque hay transitos pendientes. Con hayTransito = " + hayTransito +
                    " - Y con ListaTransitos.Count = " + ctrlInterfaz.GetComponent<ScriptGestorTransitos>().ListaTransitos.Count);
            }
            // Aqui hay que generar algun recurso que haga saber al usuario que debe esperar para volver a realizar la operacion
            return;

        }

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde generaMuro fuera del else. Con  ListaTransitos " + ctrlInterfaz.GetComponent<ScriptGestorTransitos>().ListaTransitos.Count + " - y con hayTransito = " + hayTransito); }
        // La generacion de un muro siempre se solicita desde la rama activa
        // Asuminos que el metodo que se esta ejecutando es el de la rama que tiene que generar el muro (sera la rama activa)

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde ScriptCtrlRama => generaMuro con desde el objeto : " + objDesdeDondeSolicita.name); }

        float distancia = 0f;  // Para ver donde tenemos que poner el muro

        /* ***********************  Esto lo quitamos porque no se usa CREO 2021-03-14
        int maxOrdinalHijoMuro = 0; // Ponemos el ordinal a cero, para obtener el ultimo en el foreach

        // /////////////////////////////////////////////////////////////
        // Controlamos si el muro desde donde se crea es el ultimo de esta rama

        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                // Si el hijo es un muro 
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro) // Miramos si la solicitud requiere atencion
                {
                    // Vamos seleccionando el ordinal maximo
                    if (hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo > maxOrdinalHijoMuro) { maxOrdinalHijoMuro = hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo; }
                } // FIn de - if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)
        *************************** */

        // /////////////////////////////////////////
        // /////////////////////////////////////////
        // Generamos el muro
        GameObject otroMuro = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro);
        otroMuro.transform.SetParent(transform);  // El muro creado es hijo de la rama activa (objeto que esta ejecutando el script)
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        otroMuro.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Ponemos el nombre del gamaobject al muro
        string nombreMuro = "Muro_" + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros;
        otroMuro.name = nombreMuro;
        otroMuro.GetComponent<ScriptDatosElemenItf>().modo = modo;

        // Antes de seguir hay que definir el tipo de elemento de interfaz que es "tipoElemItf_muro"
        otroMuro.GetComponent<ScriptDatosElemenItf>().generaElementEnTipo(ScriptDatosElemenItf.tipoElemItf_muro);
        // En principio los muros no tienen subtipo, por lo que no necesitamos "ponSubTipoElementIntf"

        // Lo ponemos como muro activo. Es en el que nos vamos a quedar
        // No hace falta, debe ponerse activo al colisionar con el triger del 
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = otroMuro.gameObject;
        otroMuro.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro;   // materialMuroActivo

        // Reorganizamos el resto de muros antes de localizar el nuevo
        reordenaRama(objDesdeDondeSolicita, otroMuro, ScriptDatosInterfaz.añadir);

        // Colocamos el muro recien creado, a continuacion del que lo solicita (si no es el ultimo, habra que recolocar los otros)
        distancia = calculaLongitudTramoRama();
        Vector3 posicionMuro = new Vector3(0.0f, 0.0f, 0);
        if (objDesdeDondeSolicita == null)  // Esto es para el caso en el que se genera el primer elemento (no hay gameObjet que solicite la generacion de elemento)
        {
            posicionMuro = new Vector3(0.0f, 0.0f, 0);
        }
        else  // Para el caso normal en el que la solicitud de generacion se hace desde un elemento que existe
        {
            // Si el muro se genera desde otro muro, estara a la distancia correspondente del muro que lo genera
            if (objDesdeDondeSolicita.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            {
                posicionMuro = new Vector3(0.0f, 0.0f, objDesdeDondeSolicita.transform.localPosition.z + distancia);
            }
            // Si el muro se genera desde una rama, estara a la distancia "" del origen de la rama
            else if (objDesdeDondeSolicita.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama)
            {
                posicionMuro = new Vector3(0.0f, 0.0f, distancia);
            }
            else if (objDesdeDondeSolicita.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama)
            {
                if (DatosGlobal.niveDebug > 100)
                { Debug.Log(" Desde ScriptCtrlRama => generaMuro el muro se genera desde el objeto : " + objDesdeDondeSolicita.name + " - Que no es muro ni rama"); }
                posicionMuro = new Vector3(0.0f, 0.0f, 0);
            }
        }

        otroMuro.transform.localPosition = posicionMuro;
        // Actualizamos el numero total de muros
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros++;


        // //////////////////////////////////////////////////////////
        // COlocamos al usuario en el muro recien creado
        int direccion = 0;  // La direccion es 0, porque vamos hacia adelante
        Usuario.GetComponent<ScriptCtrlUsuario>().iniciaTransicion(direccion, distancia);

    }  // Fin de -  public void generaMuro(GameObject objDesdeDondeSolicita)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Elimina el muro desde el que se le llama, hijo de la rama que lo ejecuta
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-26
    /// Observaciones :
    /// <summary>
    public void eliminaMuro(GameObject objDesdeDondeSolicita)
    {
        float distancia = calculaLongitudTramoRama();

        // La eliminacion de un muro siempre se solicita desde la rama activa
        // Asuminos que el metodo que se esta ejecutando es el de la rama que tiene que generar el muro (sera la rama activa)

        // Reorganizamos el resto de muros antes de localizar el nuevo
        // como se elimina un muro, el nuevo muro estara a null
        reordenaRama(objDesdeDondeSolicita, null, ScriptDatosInterfaz.eliminar);

        objDesdeDondeSolicita.GetComponent<ScriptDatosElemenItf>().eliminaElemIntf();

        // Primero reordenamos y luego eliminamos porque si no, no podemos pasar el objeto ya eliminado
//        eliminaEsteMuro(objDesdeDondeSolicita);

        // El numero de muros no se reduce, ya que asi los nuevos tendran nombres distintos
        // ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().numMuros++;

        // //////////////////////////////////////////////////////////
        // Colocamos al usuario en el muro anterior
        // El usuario debe retroceder al muro anterior para estar frente a un muro
        int direccion = 1;  // La direccion es 0, porque vamos hacia adelante
        Usuario.GetComponent<ScriptCtrlUsuario>().iniciaTransicion(direccion, distancia);

    }  // Fin de -  public void generaMuro(GameObject objDesdeDondeSolicita)

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
//    private void eliminaEsteMuro(GameObject muroAEliminar)  ESTA FUNCION ESTA OBSOLETA MAFG 2021-03-12
//    {
        // Borramos el muro de todas las listas en las que debe estar
//        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ListaMuros.Remove(muroAEliminar);
//        GetComponent<ScriptDatosElemenItf>().listaDeHijos.Remove(muroAEliminar);

        // Borramos el EVI del DAUS PENDIENTE (MAFG 2021-01-30)

        // Eliminamos el gameObject
//        Destroy(muroAEliminar);

//    } // Fin de - private void eliminaEsteEvi()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Calcula la longitud de un tramo de la rama
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-26
    /// Observaciones :
    ///     En principio, tomamos una longitud de tramo constante e igual a "distanciaEntreMuros". pero para mas tarde, si deseamos 
    ///     que el arbol pueda crecer indefinidamente sin colisiones entre muros, tendremos que poder hacer los tr4amos de rama
    ///     mas largos, entonces entrara en juego este metodo recalculando la longitud de cada tramo PENDIENTE MAFG 2021-02-27
    /// <summary>
    public float calculaLongitudTramoRama()
    {
        return ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros;
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// /////////// calculaAnguloRama
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-13
    /// Observaciones :
    ///     - Las ramas deben generarse con diferentes angulos para que no se superpongan si nacen en el mismo punto
    ///     - POR AHORA (2021-03-13 MAFG) lo que hacemos es :
    ///             - Vemos si hay mas ramas en el punto donde la vamos a generar.
    ///                 - Si no hay ninguna rama generada, se genera en el angulo de inicio de ramificacion.
    ///                 - Si hay mas ramas, se van ocupando angulos segun se indica seguidamente
    ///                     1) se van generando ramas aumentando el angulo en XYZ en el incremento de de rotacion en el cono
    ///                     2) Si un cono se ha llenado, se aumenta el angulo del cono y se van incluyendo ramas hasta hasta volver a completar
    ///                     la rotacion en el cono
    ///     
    ///     - Para hacerlo decentemente habria que generar un algoritmo que calculara la longitud y angulo de las ramas para que no se 
    ///     superpongan aun cuando el arbol crezca indefinidamente - PENDIENTE MAFG 2021-03-13
    /// <summary>
    public Quaternion calculaAnguloRama()
    {
        Quaternion rotacionRama = Quaternion.Euler(0f, 45f, 0f); ;
        int numRamasEnElMismoZ = 0;  // Para ver cual es la posicion del elemento mas lejano

        // Hemos generado la rama, pero todabia no se ha ejecutado su metodo "start", por lo que la rama recien generada todabia no esta en la lista
        // de hijos de la rama madre, que es en la que estamos.
        // Miramos a ver si hay otra rama en el punto donde vamos a ingertar esta. Para esto, sabemos que la rama se va a ingertar en la coordenada Z de la rama 
        // madre, que es donde esta el muro activo. Miramos por tanto si entre los hijos de la rama madre hay alguna rama con esta coordenada z
        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count != 0)  // Si tenemos algun hijo en la lista
        {
            float posicionMuroActivo_Z = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform.localPosition.z;

            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama) // Si es una rama
                {
                    if (hijo.transform.localPosition.z == posicionMuroActivo_Z)
                    {
                        numRamasEnElMismoZ++;
                    }  // Fin de - if (hijo.transform.localPosition.z == posicionMuroActivo_Z)
                }  // FIn de - if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_rama)
            } // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos.Count != 0)

        if (numRamasEnElMismoZ > 0)  // Esto es para el caso en el que se genera el primer elemento (no hay gameObjet que solicite la generacion de elemento)
        {
            // La direccion en la que crece la rama es Z. Un giro en Z, lo hara en un plano perpendicular a el
            // SI giramos en Y y despues en Z, generamos un cono cuyo eje es el eje Z
            float giro_X = 45f * (float)numRamasEnElMismoZ;
            float giro_Y = 45f;
            float giro_Z = 0f;
            rotacionRama = Quaternion.Euler(giro_X, giro_Y, giro_Z);
        }  // FIn de - if (numRamasEnElMismoZ > 0)

        return rotacionRama;

    }  // Fin de - public float calculaAnguloRama()

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// ///////////  Reordena los ordinales de los hijos de 
        /// Autor : 	Miguel Angel Fernandez Graciani
        /// Fecha :	2021-02-27
        /// Variables de entrada :
        ///         - elementoQueSolicita : es el objeto que realiza la solicitud (el muro desde donde se crea, por ejemplo)
        ///         - elementoGenerado : es el objeto que se ha generado (muro, rama....) 
        ///                     - si la operacion es de eliminacion, sera null
        ///         - operacion : Noemalmente sera 
        ///             - añadir  ver "ScriptDatosInterfaz.añadir"
        ///             - eliminar  ver "ScriptDatosInterfaz.eliminar"
        /// Variables de salida :
        /// Observaciones :
        ///         Reordena los ordinales de los hijos de la rama, sean muros o ramas, y reajusta la componente Z de 
        ///         estos, ya qe si se ha incluido un nuevo elemento de ordinal ordinalElem, todos los que tengan una 
        ///         coordenada z mayor que estos tendran que alejarse, y en caso de haberse suprimido un elemento de 
        ///         ordinal ordinalElem, estos deberan acercarse
        /// <summary>
        public void reordenaRama(GameObject elementoQueSolicita, GameObject elementoGenerado, string operacion)
    {
        // Ajustamos las modificaciones de ordinal y coordenada z que vamos a realizar dependiendo de si añadimos o eliminamos
        int incOrdinal = 0;  // Sera lo que se sume al ordinal de los que hay que modificar
        float incDistancia = 0;   // Sera lo que se sume a la componente z de los que hay que modificar

        // Si se ha generado un elemento, el tipo de la lista a ordenar sera la del elemento generado
        // si se ha eliminado un elemento, el tipo de lista a ordenar sera la del elemento que lo solicita, que es el que se elimina
        string este_tipoElementIntf; // necesita un valor asignado
        if (elementoGenerado == null) {este_tipoElementIntf = elementoQueSolicita.GetComponent<ScriptDatosElemenItf>().tipoElementIntf;}
        else {este_tipoElementIntf = elementoGenerado.GetComponent<ScriptDatosElemenItf>().tipoElementIntf;}

        // Segun añadimos o eliminaos
        if (operacion == ScriptDatosInterfaz.añadir)
        {
            incOrdinal = 1;
            incDistancia = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros;
        }
        else if (operacion == ScriptDatosInterfaz.eliminar)
        {
            incOrdinal = -1;
            incDistancia = (-1f) * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros;
        }
        else
        {
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log(" Error en ScriptCtrlRama => reordenaRama. Operacion : " + operacion + " - NO CONOCIDA "); }
            return;
        }

        // Ajustamos ahora los ordinales y coordenadas z de todos los elementos que lo necesitan
        // Los elementos a modificar seran aquellos que esten mas distantes de la raiz de la rama que el 
        // elemento que lo solicitan (la ordinacion numerica coincide con la ordinacion espacial)
        // OJOOO las ramas y los muros tienen distinta ordinacion y por tanto ordinales independientes, que 
        // deben modificarse segun su tipo de elemnto de interfaz "este_tipoElementIntf"
        // no obstante, tanto las ramos como los muros que esten mas lejos que el muro que lo solicita deben 
        // reordenarse en la coordenada Z. 

        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                // En cualquier caso, si el hijo esta mas lejos en Z que el que se (añade o modifica), modificamos su coordenada z
                if (hijo.transform.localPosition.z > elementoQueSolicita.transform.localPosition.z)
                {
                    Vector3 posicionLocal = hijo.transform.localPosition;
                    posicionLocal.z = posicionLocal.z + incDistancia;
                    hijo.transform.localPosition = posicionLocal;
                }
                // Ahora reorganizamos los ordinales, para los elementos de interfaz del tipo que corresponde
                if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == este_tipoElementIntf) &
                     (hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo > elementoQueSolicita.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo))
                {
                    // Vamos seleccionando el ordinal maximo
                    hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo = hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo + incOrdinal;
                } // FIn de - if ((hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == este_tipoElementIntf) &
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

            // Ahora, si hemos incluido un elemento de interfaz, debemos ponerle el ordinal correspondiente, que será el siguiente
            // al ordinal del elemento que lo generó, y que debe haber quedado libre al reordenar
            if (elementoGenerado != null)
            {
                if (elementoQueSolicita == null)  // Esto es para el caso en el que se genera el primer elemento (no hay gameObjet que solicite la generacion de elemento)
                {
                    elementoGenerado.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo = 0;
                }
                else  // Para el caso normal en el que la solicitud de generacion se hace desde un elemento que existe
                {
                    elementoGenerado.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo = elementoQueSolicita.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo + 1;
                }
            }
        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)

    }  // Fin de - public void reordenaRama(GameObject elementoQueSolicita, string este_tipoElementIntf, string operacion)


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  public GameObject dimeUltimoMuroDeRama() : Devuelve el muro mas lejano que pertenece a la rama
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-14
    /// Parametros de entrada :
    /// Parametros de salid:
    ///         -  GameObject muroFrontera : Es el gameobject del muro que esta mas alejado en la rama
    /// Observaciones :
    ///     
    /// <summary>
    public GameObject dimeUltimoMuroDeRama()
    {
        int maxOrdinalHijoMuro = 0;
        float maxDistanciaAMuro = 0; // Ponemos el ordinal a cero, para obtener el ultimo en el foreach
        GameObject muroMayoDeMayorOrdinal = null;
        GameObject muroAMasDistancia = null;
        GameObject muroFrontera = null;

        // /////////////////////////////////////////////////////////////
        // Buscamos el muro mas lejano de la rama

        if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
        {
            foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
            {
                // Si el hijo es un muro 
                if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro) // Miramos si la solicitud requiere atencion
                {
                    // Vamos seleccionando el de ordinal maximo
                    if (hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo > maxOrdinalHijoMuro)
                    {
                        maxOrdinalHijoMuro = hijo.GetComponent<ScriptDatosElemenItf>().ordinalHijoYTipo;
                        muroMayoDeMayorOrdinal = hijo;
                    }
                    if (hijo.transform.localPosition.z > maxDistanciaAMuro)
                    {
                        maxDistanciaAMuro = hijo.transform.localPosition.z;
                        muroAMasDistancia = hijo; ;
                    }

                } // FIn de - if (hijo.GetComponent<ScriptDatosElemenItf>().tipoElementIntf == ScriptDatosElemenItf.tipoElemItf_muro)
            }  // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)

            // SI todo esta bien, el muro que esta a mayor distancia, debe ser el mismo que el de ordinal mayor
            if (muroAMasDistancia == muroMayoDeMayorOrdinal) { muroFrontera = muroAMasDistancia; }

        }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)

        return muroFrontera;
    }  // Fin de -  public GameObject dimeUltimoMuroDeRama()




}  // Fin de - public class ScriptCtrlRama : MonoBehaviour {
