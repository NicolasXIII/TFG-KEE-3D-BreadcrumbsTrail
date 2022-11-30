using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control del usuario
/// Basicamente muueve al usuario mor el eje Z y se encarga de gestionar las colisiones con los
/// colaider correspondientes
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2020-03-15
/// Observaciones :
/// 		Lo acompañan la camara, el foco de luz, el muro de usuario y los telones
/// </summary>

public class ScriptCtrlUsuario : MonoBehaviour {

	public GameObject ctrlInterfaz; // For profit objects

    public GameObject PunteroUsuario;

    private Rigidbody rb;
	private Vector3 movement_usr;

    public int miRamaActual;
    public GameObject rama_EnLaQueEstaElUsuario;

    public GameObject Tramoya;


    // ////////////////////////////////////////////
    // Para la gestion de transito entre muros
    protected int idTransicionGeneral;  // Es el id que se le asigna a la transicion en la que pueda estar el usuario, con el fin de poder eliminarla cuando termine

    protected int enTransitoLocal; // Indica si el usuario esta estable delante de un muro, o esta viajando de uno a otro o en otro transito
                                   // enTransitoLocal = 0 - El usuario esta estable en una posición operativa
                                   // enTransitoLocal = 100 - El usuario esta en transito entre dos muros de la misma rama

    public int Ttrans_ms;  // Tiempo total (en milisegundos) que debe tardar en relaizarse el viage desde un muro hasta el siguiente o el anterior
    public float Ttrans_s;  // Tiempo total (en segundos) que debe tardar en relaizarse el viage desde un muro hasta el siguiente o el anterior
    public float Ts; // Tiempo de salida. Tiempo que el usuario esta acelerando al comenzar la transicion
    public float pts; // Porcion de Ttrans que el usuario esta acelerando al comenzar la transicion
    public float acs; // Aceleracion de salida cuando el usuario parte del punto de origen
    public float Tc; // Tiempo de crucero. Tiempo que el usuario esta viajando a velocidad constante desde que termina de acelerar hasta que comienza a decelerara
    public float ptc; // Porcion de Ttrans que el usuario esta viajando a velocidad constante desde que termina de acelerar hasta que comienza a decelerara
    public float velc; // Velocidad de crucero. Veloocidad que lleva el usuario cuando esta viajando a velocidad constante desde que termina de acelerar hasta que comienza a decelerara
    public float Tf; // Tiempo de frenada. Tiempo que el usuario esta decelerando hasta parar en el muro destino
    public float ptf; //Porcion de Ttrans que el usuario esta decelerando hasta parar en el muro destino
    public float acf; // Aceleracion de frenado cuando el usuario esta llegando al punto (muro) de destino

    public float dist; // Distancia a recorres en la transicion (normalmente entre este muro y el siguiente o el anterior)
    public float localizacionInicial; // Localizacion del usuario con respecto a su padre (rama en la que esta) al iniciar el transito

    public DateTime dateActual; // Esta variable la iremos utilizando para obtener el momento presente en distintas partes del codigo
    public DateTime dateInicioTrans;  // Momento en el que el usuario comenzo el transito (en formato date)

    public GameObject objOrigen; // Objeto (generalmente muro) donde se encuentra el usuario antes de comenzar la transicion
    public GameObject objDestino; // Objeto (generalmente muro) donde se encuentra el usuario al terminar la transicion

    // This function is only executed when the game object is activated
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  void Start ()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    /// 	    
    /// </summary>
    void Start ()
	{
		// Asignamos objetos
		ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");

		rb = GetComponent<Rigidbody> ();
		this.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionUsuario;

        // Generamos los game objet de punteros de usuario
        GameObject PunteroUsuario = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario);
        // Queremos que sea esta instancia la que referencie "ScriptDatosInterfaz". No que referencie al asset como esta en principio
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroUsuario = PunteroUsuario;
            
        PunteroUsuario.transform.SetParent(this.transform);

		// Generamos los game objet del muro de usuario
		GameObject MuroUsuario = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().MuroUsuario);
		MuroUsuario.transform.SetParent(this.transform);

        // Generamos el game objet para contener la tramoya Donde se organizaran los telones
        Tramoya = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Tramoya);
//        Tramoya = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Tramoya;
        Tramoya.transform.SetParent(this.transform);

        // Localizamos los punteros "PunteroTramoya" y "PunteroMuroUsuario" que son hijos del puntero de usuario y los asignamos a 
        // sus variables correspondientes  en "ScriptDatosInterfaz" para poder referenciarlos globalmente
        for (int i = 0; i < PunteroUsuario.transform.childCount; i++)
        {
            if (PunteroUsuario.transform.GetChild(i).name == "PunteroMuroUsuario")
            { ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroMuroUsuario = PunteroUsuario.transform.GetChild(i).gameObject; }
            else if (PunteroUsuario.transform.GetChild(i).name == "PunteroTramoya")
            { ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PunteroTramoya = PunteroUsuario.transform.GetChild(i).gameObject; }
        }

        // ///////////////////////////////////////////////////////
        // Definimos los parametros de transito que son constantes a lo largo de toda la ejecucion
        idTransicionGeneral = 0; // Iniciamos la variable

        dateActual = new DateTime(); // Generamos la instancia para utilizarla luego
        dateInicioTrans = new DateTime(); // Generamos la instancia para utilizarla luego

        Ttrans_ms = 1000;  // Tiempo total (en milisegundos segundos) que debe tardar en relaizarse el viage desde un muro hasta el siguiente o el anterior
        Ttrans_s = Ttrans_ms/1000;  // Tiempo total (en segundos) que debe tardar en relaizarse el viage desde un muro hasta el siguiente o el anterior
                                    // OJOOO (ts + tc + tf = 1) ya que son las prorciones de Ttrans que los completan (Ts + Tc * Tf = Ttrans_s)
        pts = 1f/6f; // Tiempo de salida. Porcion de Ttrans que el usuario esta acelerando al comenzar la transicion
        Ts = pts * Ttrans_s; // Tiempo de salida. Porcion de Ttrans que el usuario esta acelerando al comenzar la transicion
        ptc = 1f/2f; // Tiempo de crucero. Porcion de Ttrans que el usuario esta viajando a velocidad constante desde que termina de acelerar hasta que comienza a decelerara
        Tc = ptc * Ttrans_s; // Tiempo de crucero. Porcion de Ttrans que el usuario esta viajando a velocidad constante desde que termina de acelerar hasta que comienza a decelerara
        ptf = 1f/3f; // Tiempo de frenada. Porcion de Ttrans que el usuario esta decelerando hasta parar en el muro destino
        Tf = ptf * Ttrans_s; // Tiempo de frenada. Porcion de Ttrans que el usuario esta decelerando hasta parar en el muro destino

        velc = 0f; // OJOO Este valor se calculara para cada transicion y dependera de la distancia a recorrer

             // OJOO como hemos hecho que (tF = TS / 2), TENEMOS  QUE HACER QUE (acf = (-1/2) * (acs)) ya que sale con velocidad 0 y queremos que llegue con velocidad 0
        acs = 0f; // Este valor se calculara para cada transicion y dependera de la distancia a recorrer
        acf = (-1f/2f) * (acs); // Aceleracion de frenado cuando el usuario esta llegando al punto (muro) de destino

    } // Finde -  void Start ()


    // This function is executed every time you have to perform physical calculations
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  void Update ()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2020
    /// Ultima modificacion :
    ///     - 2022-10-21 : Introduzco la generacion del evi de referencia a alemento de interfaz, para que pueda realizarse la gestion de la miga de pan
    ///                     - Tambien comento el metodo, que estaba muy mal comentado
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///     - idTransicionGeneral;  Es el id que se le asigna a la transicion en la que pueda estar el usuario, con el fin de poder eliminarla cuando termine
    ///     - enTransitoLocal; Indica si el usuario esta estable delante de un muro, o esta viajando de uno a otro o en otro transito
    ///                     - enTransitoLocal = 0 - El usuario esta estable en una posición operativa
    ///                     - enTransitoLocal = 100 - El usuario esta en transito entre dos muros de la misma rama
    ///     Pasos a ejecutar :
    ///         1.) Si el usuario esta estable en una posición operativa (enTransitoLocal == 0)
    ///             1.1.) Si la transicion ha finalizado junsto el cuadro de antes entonces sera "idTransicionGeneral != 0"
    ///                 - 
    ///                 - Indicamos ahora al sistema que ha finalizado la transicion para que otros elementos lo sepan y actuen en consecuencia
    ///                 - Ponemos a cero "idTransicionGeneral == 0", para indicar que ya estamos en un estado estable
    /// </summary>
    void Update ()
	{
        if (enTransitoLocal == 0)
        {

            if (idTransicionGeneral != 0)
            {
                if(!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generacion_EviRefElemen_ocupada)  // ver definicion de "generacion_EviRefElemen_ocupada"
                {
                    // Generamos el evi de referencia para la miga de pan MAFG 2022-10-12
                    // OJOOO, cualquier transito generara un evi de referencia a elemento de interfaz, esto quiere decir que se generaran incluso en los casos de final de 
                    // carrera, en los que se vuelve al mismo muro donde estabamos, por lo que el gestor de miga de pan, tendra que eliminar aquellos evis de referencia a 
                    // elemento que refieran el mismo elemento que la ultima miga de pan encadenada
                    ctrlInterfaz.GetComponent<ScriptLibGestorEvis>().generaEviRefElemen(
                        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo, 
                        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera
                    );
                }
                // Indicamos ahora al sistema que ha finalizado la transicion para que otros elementos lo sepan y actuen en consecuencia
                ctrlInterfaz.GetComponent<ScriptGestorTransitos>().finalizoTransito(gameObject, idTransicionGeneral);
                if (DatosGlobal.niveDebug > 50)
                { Debug.Log(" Desde ScriptCtrlUsuario => Update. Con enTransitoLocal = " + enTransitoLocal +
                                                                "Con idTransicionGeneral = " + idTransicionGeneral); }
                idTransicionGeneral = 0; // La ponemos a cero para resetearla
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().generacion_EviRefElemen_ocupada = false; // ver definicion de "generacion_EviRefElemen_ocupada"
            }

            // Si el usuario esta donde no debe, lo colocamos donde debe estar
            if (rama_EnLaQueEstaElUsuario != ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo)
            {
                // Si es la raiz del arbol, ponemos el usuario como hijo y lo colocamos adecuadamente
                transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo.transform);

                float distanciaUsuario = 0f;
                Vector3 posicionUsuario = new Vector3(0.0f, distanciaUsuario, 0.0f);
                transform.localPosition = posicionUsuario;
                // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                // Indicamos que el usuario esta en esta rama 
                miRamaActual = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().idElemItfRama_Activo;
                rama_EnLaQueEstaElUsuario = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo;
            }  // Fin de - if (rama_EnLaQueEstaElUsuario != ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo)

            // /////////////////////////////////////////////////////////////////
            // /////////////////////////////////////////////////////////////////
            // Controlamos los finales de recorrido del usuario a lo largo de la rama en la que esta

            // /////////////////////////////////////////////////////////////////
            // primero miramos si el usuario se ha colocado por detras del inicio de la rama
            if (transform.localPosition.z < 0f)
            {
                int direccion = 1;  // Volvemos hacia adelante para volver a colocarnos en el origen
                float distancia = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros; // OJOOO PENDIENTE MAFG 2921.02-26. Estamos pasando la distancia entre muros a pelo
                                                                                                        // Si hacemos la longitud de las ramas variables para mejorar el arbol, hay que cambiarlo, posiblemente pasando el 
                                                                                                        // objeto al que tiene que ir, para que pueda calcular la distancia
                                                                                                        // Para mover el usuario entre dos posiciones del arbol, utilizamos una corrutina que ejecuta el viaje
                                                                                                        // durante el viaje la variable "enTransitoLocal !=0" 
                iniciaTransicion(direccion, distancia);

                // Por si no lo hemos hecho bien, lo colocamos en el final de carrea (la ultima frontera "ultimaFrontera")
            //    Vector3 posicionUsuario = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionUsuario;
            //    transform.localPosition = posicionUsuario;

                // OJOOOOO al implementar la posibilidad de viajar de una rama a su madre, aqui no debemos ir a la posicion cero de esta rama
                // si lo a la rama madre, a la posicion de ingerto en la que esta rama se ingerta en su madre
            }
            // Si en algun viaje nos hemos pasado del ultimo elemento de la rama, volvemos a la ultima posición ocupada
            // es para limitar que el usuario no se vaya mas alla del final de la rama
            else if (rama_EnLaQueEstaElUsuario.GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)  // Si tenemos algun hijo en la lista
            {
                float ultimaFrontera = 0f;  // Para ver cual es la posicion del elemento mas lejano

                foreach (GameObject hijo in rama_EnLaQueEstaElUsuario.GetComponent<ScriptDatosElemenItf>().listaDeHijos)
                {
                    // En cualquier caso, si el hijo esta mas lejos en Z que el que se (añade o modifica), modificamos su coordenada z
                    if (hijo.transform.localPosition.z >= ultimaFrontera)
                    {
                        ultimaFrontera = hijo.transform.localPosition.z;
                    }
                } // Fin de - foreach (GameObject hijo in GetComponent<ScriptDatosElemenItf>().listaDeHijos)
                  // Si el usuario ha llegado mas lejos, lo ponemos en el ultimo elemento que esta en "ultimaFrontera = 0f"
                if (transform.localPosition.z > ultimaFrontera)
                {
                    int direccion = -1;  // Volvemos hacia a tras
                    float distancia = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros; // OJOOO PENDIENTE MAFG 2921.02-26. Estamos pasando la distancia entre muros a pelo
                                                                                                            // Si hacemos la longitud de las ramas variables para mejorar el arbol, hay que cambiarlo, posiblemente pasando el 
                                                                                                            // objeto al que tiene que ir, para que pueda calcular la distancia
                                                                                                            // Para mover el usuario entre dos posiciones del arbol, utilizamos una corrutina que ejecuta el viaje
                                                                                                            // durante el viaje la variable "enTransitoLocal !=0" 
                    iniciaTransicion(direccion, distancia);

                    // Por si no lo hemos hecho bien, lo colocamos en el final de carrea (la ultima frontera "ultimaFrontera")
                    Vector3 posicionUsuario = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionUsuario;
                    posicionUsuario.z = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionUsuario.z + ultimaFrontera;
                    transform.localPosition = posicionUsuario;
                }  // FIn de - if (transform.localPosition.z > ultimaFrontera)
            }  // Fin de - if (GetComponent<ScriptDatosElemenItf>().listaDeHijos != null)


            // Tomamos las indicaciones de teclado
            //		float mover_en_Z = Input.GetAxis ("Vertical");
            float mover_en_Z = Input.GetAxis("Mouse ScrollWheel");

            if ((mover_en_Z != 0f) & (enTransitoLocal == 0)) // Si movemos la rueda del raton y no estabamos en transito
            {
                int direccion = 0; // Indica si el despalazamiento del movimiento ser ahacia las z positivas (direccion = 0) o hacia las negativas (direccion = 1)
                if (mover_en_Z < 0f) { direccion = -1; }

                float distancia = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros; // OJOOO PENDIENTE MAFG 2921.02-26. Estamos pasando la distancia entre muros a pelo
                                                                                                        // Si hacemos la longitud de las ramas variables para mejorar el arbol, hay que cambiarlo, posiblemente pasando el 
                                                                                                        // objeto al que tiene que ir, para que pueda calcular la distancia
                                                                                                        // Para mover el usuario entre dos posiciones del arbol, utilizamos una corrutina que ejecuta el viaje
                                                                                                        // durante el viaje la variable "enTransitoLocal !=0" 
                iniciaTransicion(direccion, distancia);

            }  // Fin de -  if ((mover_en_Z != 0) & (enTransitoLocal == 0)) // Si movemos la rueda del raton y no estabamos en transito

        }  // FIn de -  if (enTransitoLocal == 0)

    } // Finde - void Update ()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Inicia el movimiento del usuario a traves de muros y ramas
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-26
    /// Inputs;
    ///     -  int direccion : Indica si vamos hacia adelante "direccion = " o hacia atras "direccion = " en el eje Z (de la rama por la que nos movemos)
    ///     -  float distancia : Indica el espacio que vamos a recorrer sobre el eje Z (de la rama por la que nos movemos)
    /// Observaciones :
    ///     - Ver el fichero "Documentacion/calculoTransito_01.pdf" donde se explica el calculo.
    ///     - En definitiva deseamos una aceleracion raprida, un transito estable y un frenada algo mas lenta, y que la transicion se realice en un tiempo 
    ///     determinado, independientemente de la distancia que se tenga que recorrer
    /// <summary>
    public void iniciaTransicion(int direccion, float distancia)
    {
        // Anotamos la localizacion inicial del usuario con respecto a su padre (rama en la que reside)
        localizacionInicial = transform.localPosition.z;
        // Anotamos el momento en el quse inicia la transicion
        dateInicioTrans = DateTime.Now;

        // /////////////////////////////////
        // /////////////////////////////////
        // Hacemos el calculo de los parametros de espacio, velocidad y aceleracion para esta transicion 

        // /////////////////////////////////
        // DISTANCIA A RECORRER en la transicio (normalmente entre este muro y el siguiente o el anterior)
        // Calculamos la distancia que debe recorrer el usuario
        // normalmente habra que ver la posicion local del muro donde esta, asi como la posicion local del muro donde va y 
        // calcular la distancia entre ambos. Como por ahora no la variamos, tomamos sienpre la distancia entre muros que 
        // definimos en "ScriptDatosInterfaz" (PENDIENTE MAFG 2021.02.24
//        dist = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros; // Por ahora es esta, pero puede cambiar por necesadades del crecimiento del arbol
        dist = distancia; // Por ahora es esta, pero puede cambiar por necesadades del crecimiento del arbol


        // objOrigen = ?; // Objeto (generalmente muro) donde se encuentra el usuario antes de comenzar la transicion
        // objDestino = ?; // Objeto (generalmente muro) donde se encuentra el usuario al terminar la transicion


        // /////////////////////////////////
        // Calculamos el valor de la aceleracion de salida
        acs = dist / ((1f/2f) * Ts * Ts + Ts * Tc + Ts * Tf - (1f/4f) * Tf * Tf);   // Ver el fichero "Documentacion/calculoTransito_01.pdf" donde se explica el calculo
        if (direccion == -1) { acs = (-1f) * acs; } // Si el desplazamiento es hacia atras, le cambiamos el signo

        // /////////////////////////////////
        // Calculamos el valor de la velocidad de crucero 
        velc = acs * Ts;

        // /////////////////////////////////
        // Calculamos la aceleracion de frenado (ahora con el valor de acs bueno)
        acf = (-1f/2f) * (acs);

        //        Debug.Log("desde ScriptCtrlUsuario =>  Update . Con :" +
        //            " - Ttrans_ms : " + Ttrans_ms +
        //            " - Ttrans_s :  " + Ttrans_s +
        //            " - Ts : " + Ts +
        //            " - pts : " + pts +
        //            " - acs : " + acs +
        //            " - Tc : " + Tc +
        //            " - ptc : " + ptc +
        //            " - velc : " + velc +
        //            " - Tf : " + Tf +
        //            " - ptf : " + ptf +
        //            " - acf : " + acf +
        //            " - dist : " + dist +
        //            " - dateInicioTrans : " + dateInicioTrans.ToLongDateString());

        enTransitoLocal = 100;  // Indicamos que el usuario esta en transito, para que fixedupdate mueva el muro hacia su destino new Date
        // Indicamos ahora al sistema que hay una transicion activa para que otros elementos lo sepan y actuen en consecuencia
        string tipoTransito = ScriptGestorTransitos.transTipo_usuarioEnMovimiento;
        idTransicionGeneral = ctrlInterfaz.GetComponent<ScriptGestorTransitos>().inicioTransito(gameObject, tipoTransito);

    }  // Fin de - public void iniciaTransicion(int direccion, float distancia)



    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  relocalizaUsuarioEnMuro : COloca al usuario directamente en una rama, frente al muro que corresponda
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-11-26
    /// Inputs;
    ///     -  GameObject muroDestino : Es el muro delante del cual debe colocarse el usuario, para que visualice los elementos del muro
    /// Observaciones :
    ///     - Lo normal es que el muro en el que se coloca la camara sea el muro activo
    ///     - La camara se coloca frente al muro, pero en realidad, lo que hacemos es proahijarla a la rama madre de "muroDestino" y colocarla en 
    ///         la distancia de la rama adecuada para que monitorice el muro destino
    ///     - Para llevarlo a cabo realizamos las operaciones siguientes
    ///         1.) Obtenemos la rama a la que pertenece el muro
    ///         2.) Obtenemos la rama distancia en Z en la que el muro se encuentra (referente a la rama, que es su padre)
    ///         3. Colocamos al usuario como hijo de la rama correspondiente a la misma distancia a la que esta el muro (el muro activo y el usuario deben coincidir) con la rotacion correspondiente
    /// <summary>
    public void relocalizaUsuarioEnMuro(GameObject muroDestino)
    {
        GameObject ramaDestino = null;
        float componente_Z = 0f;

        // 1.) Obtenemos la rama a la que pertenece el muro
        ramaDestino = muroDestino.gameObject.transform.parent.gameObject;
        // 2.) Obtenemos la rama distancia en Z en la que el muro se encuentra (referente a la rama, que es su padre)
        componente_Z = muroDestino.transform.localPosition.z;
        Vector3 posicionUsuario = this.transform.localPosition;
        posicionUsuario.z = componente_Z;
        // 3. Colocamos al usuario como hijo de la rama correspondiente a la misma distancia a la que esta el muro (el muro activo y el usuario deben coincidir) con la rotacion correspondiente
        this.transform.SetParent(ramaDestino.transform);
        this.transform.localPosition = posicionUsuario;
        this.transform.localRotation = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rotacionLocalnUsuario;

    }  // Fin de - public void relocalizaUsuarioEnMuro(GameObject muroDestino)

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Gestiona entre otras cosas el movimiento del usuario a traves de muros y ramas
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-24
    /// Observaciones :
    /// <summary>
    void FixedUpdate()
    {
        if (enTransitoLocal == 100)  // Si estamos en transito, vamos poniendo al usuario donde corresponde
        {
            dateActual = DateTime.Now;
            TimeSpan tiempoEnTransitoLocal = dateActual - dateInicioTrans;
            
            double tiempoEnTransitoLocal_ms = tiempoEnTransitoLocal.TotalMilliseconds;

            //  public DateTime dateActual; // Esta variable la iremos utilizando para obtener el momento presente en distintas partes del codigo
            //  public DateTime dateInicioTrans;  // Momento en el que el usuario comenzo el transito (en formato date)


            double distRecorrida = 0;  // Representa la distancia recorrida desde el inicio de la transicion hasta este momento
            float posicionLocalActual = 0;  // localizacion actual del usuario con respecto a su padre (rama en la que reside)
            double tiempoEnTransitoLocal_s = tiempoEnTransitoLocal_ms / 1000f;  // Tiempo en transito en segundos

            //            Debug.Log("desde ScriptCtrlUsuario =>  FixedUpdate . Con :" +
            //                " - Ttrans_s : " + Ttrans_s +
            //                " - Ts : " + Ts +
            //                " - Tcs : " + Tc +
            //                " - Tf : " + Tf +
            //                " - tiempoEnTransitoLocal_ms : " + tiempoEnTransitoLocal_ms +
            //                " - tiempoEnTransitoLocal_s : " + tiempoEnTransitoLocal_s);


            if (tiempoEnTransitoLocal_s <= Ts)  // Si estamos en el tramo de aceleracion
            {
                distRecorrida = (1f/2f) * acs * tiempoEnTransitoLocal_s * tiempoEnTransitoLocal_s; // Mov. uniformemente acelerado
            }
            else if ((Ts < tiempoEnTransitoLocal_s) & (tiempoEnTransitoLocal_s <= (Ts + Tc)))  // Si estamos en el tramo de velocidad uniforme
            {
                double tiempoEnTramo = tiempoEnTransitoLocal_s - Ts;
                distRecorrida = ((1f/2f) * acs * Ts * Ts) + (velc * tiempoEnTramo); //  Mov. uniformemente acelerado + mov uniforme
            }
            else if (((Ts + Tc) < tiempoEnTransitoLocal_s) & (tiempoEnTransitoLocal_s <= (Ts + Tc + Tf)))  // sim estamos en el tramo de deceleración
            {
                double tiempoEnTramo = tiempoEnTransitoLocal_s - (Ts + Tc);
                distRecorrida = ((1f/2f) * acs * Ts * Ts) + 
                                (velc * Tc) + 
                                ((velc * tiempoEnTramo) + (1f/2f) * acf * tiempoEnTramo * tiempoEnTramo); // Mov. uniformemente acelerado + mov uniforme + mov unif. decelerado
            }
            else if (Ttrans_s < tiempoEnTransitoLocal_s)  // Si ha pasado el tiempo en el que deberiamos haber finalizado el transito. Colocamos directamente al usuario en el destino
            {
                if (acs >= 0){ distRecorrida = dist; } else { distRecorrida = - dist; }
                enTransitoLocal = 0;  // Indicamos que el transito ha finalizado
            }

            // Si no hemos terminado ponemos la posicion actual. Si hemos terminado ponemos la posicion del objeto destino

            if (enTransitoLocal == 100)
            {
//                Debug.Log("distRecorrida : " + distRecorrida);
                double posicionLocalActual_double = localizacionInicial + distRecorrida;
                posicionLocalActual = Convert.ToSingle(posicionLocalActual_double);
                Vector3 vectorLocalActual = new Vector3(0.0f, 0.0f, posicionLocalActual);  // Direction vector of applied force
                transform.localPosition = vectorLocalActual;
            }
            else
            {
                // PENDIENTE. lo que hay que hacer aqui es poner el usuario en el objeto (muro) destino
                // objDestino = ?; // Objeto (generalmente muro) donde se encuentra el usuario al terminar la transicion
                double posicionLocalActual_double = localizacionInicial + distRecorrida;
                posicionLocalActual = Convert.ToSingle(posicionLocalActual_double);
                Vector3 vectorLocalActual = new Vector3(0.0f, 0.0f, posicionLocalActual);  // Direction vector of applied force
                transform.localPosition = vectorLocalActual;
            }

            // //////////////////////////////////////////////
            // Controlamos ahora si el usuario ha llegado a la raiz de una rama. Si es asi, y esta rama sale de otra rama (es hija suya), al llegar
            // a la raiz de la rama hija, el usuario dee pasar de la rama hija a la madre, para poder viajar hacia atras en el arbol
            if (posicionLocalActual <= 0)
            {
                // Tomamos la coordenada z de la rama donde esta el usuario ya que sera en esa coordenada de la rama madre, donde debe 
                // colocarse el usuario en la rama de (madre de la actual) en la que se va a recolocar
                float coor_Z_deLaRamaUsuario = transform.parent.transform.localPosition.z;

                // miramos si esta rama tiene una rama madre
                if (transform.parent.parent != null)
                {
                    // Si tiene rama madre la obtenemos y ahijamos al usuario en esta rama madre
                    transform.SetParent(transform.parent.parent.transform);
                    // Y colocamos al usuario en la coordenada z donde nacia la rama en la que estaba
                    float distanciaUsuario = 0;
                    Vector3 posicionUsuario = new Vector3(0.0f, distanciaUsuario, coor_Z_deLaRamaUsuario);
                    transform.localPosition = posicionUsuario;
                    // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
                    transform.localRotation = Quaternion.Euler(0, 0, 0);

                    // Indicamos que el usuario esta en esta rama 
                    int idRamaAlAQueSeSubeElUsuario = transform.parent.GetComponent<ScriptDatosElemenItf>().dameIdElementIntf();
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().idElemItfRama_Activo = idRamaAlAQueSeSubeElUsuario;
                    ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo = transform.parent.gameObject;
                    miRamaActual = idRamaAlAQueSeSubeElUsuario;
                    rama_EnLaQueEstaElUsuario = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().rama_Activo;

                }  // Fin de - if (transform.parent.parent != null)

                // Si no la tiene no hacemos nada. El usuario se queda en la raiz del arbol

            }  // Fin de - if (posicionLocalActual < 0)

        }  // Fin de - if (enTransitoLocal == 100) 

    } // FIn de - void FixedUpdate()

} // Finde - public class ScriptCtrlUsuario : MonoBehaviour {
