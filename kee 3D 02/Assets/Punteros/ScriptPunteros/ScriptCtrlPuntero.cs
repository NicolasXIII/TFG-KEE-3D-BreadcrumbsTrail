using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control del puntero que maneja el usuario
// Mueve el cursor para que el usuario pueda seleccionar Evis y elementos en el muro de trabajo, botones del muro de 
// usuario, o elementos de los distintos telones
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-03-21
// Observaciones :
// 		- Debemos tener al menos dos punteros. Uno para la mano derecha y otro para la izquierda
//				Se manejaran bien con dos ratones o bien con la pantlla tactil u otros elementos de control que pueda manejar el usuario
// 		- Cada puntero son en realidad un gupo de punteros, a saber:
//			- Puntero de Usuario : Es el que va pegado al usuario y por tanto se usa para manejar los elementos del muro de trabajo
//			- Puntero de Muro de Usuario :  Va con el puntero de usuario (es hijo suyo), pero en Z esta algo detras, para colocarse sobre el muro de usuario
//					Cuando el usuario se coloca en el muro de trabajo este puntero coloca sobre el muro de usuario. Se usa para manejar los elementos del muro de Usuario
// 		- Este script maneja el comportamiento del puntero de usuario. Los otros elementos de este puntero (puntero de muro de usuario, etc...)
//					se gestionan desde sus scripts correspondientes

public class ScriptCtrlPuntero : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject PuntMuroUsuario; // lo asignaremos al game objet que es el hijo de el puntero de usuario
										// No se geera por codigo porque solo hay un puntero de muro de usuario para cada puntero de usuario

	private Rigidbody rb;
	private Vector3 movement_Puntero;

    public int origenVentanaPixels_x { get; private set; }

    void Start ()
	{
		// Asignamos objetos
		ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");

		// Inicializamos el puntero de usuario
		this.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionPunteroUsuario;
		this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaPunteroUsuario;
		// Inicializamos el puntero del muro usuario.
		PuntMuroUsuario.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().posicionPuntMuroUsuario;
		PuntMuroUsuario.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaPuntMuroUsuario;

        // Lo ponemos por defecto en estado activo
        this.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;

    } // FIn de - void Start ()

	void Update ()
	{
        /* ************************
        // QUITAR ESTE y poner el que vale que es el de abajo
		// Tomamos las indicaciones de teclado
		float mover_en_X = Input.GetAxis ("Mouse X");
		float mover_en_Y = Input.GetAxis ("Mouse Y");
		float mover_en_Z = 0;
		// Calculmos los desplazamientos
        // OJOOO el movimiento del raton es con respecto al tamaño del monitor. Si el tamaño del muro es muy distinto del de la pantalla
        // el ratos y el puntero llevan distintas escalas y pierden en parte la correlacion
		mover_en_X = mover_en_X * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().velocidad_PunteroUsuario_X;
		mover_en_Y = mover_en_Y * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().velocidad_PunteroUsuario_Y;
		mover_en_Z = mover_en_Z * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().velocidad_PunteroUsuario_Z;

		movement_Puntero =  new Vector3 (mover_en_X, mover_en_Y, mover_en_Z);

//        Debug.Log("Desde ScriptCtrlPuntero  - mover_en_X = " + mover_en_X + " - mover_en_Y = " + mover_en_Y);

        transform.Translate (movement_Puntero);  
        ***************** */


        //ESTE ES EL QUE VALE. lo cambio al otro para pruebas

        Vector3 p = Input.mousePosition;

        // La posicion (x,y,z) = (0,0.0) del mouse es la esquina izquierda de la pantalla. Sin embargo la posicion (x,y,z) = (0,0,0) es el centro del muro
        // por lo que para mover el puntero junto con el raton hay que trasladar el eje de coordenada
        float pixelsVentana_x =ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().pixels_x_Pantalla/2f;
        float pixelsVentana_y = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().pixels_y_Pantalla/2f;

        float locPuntero_x = (p.x - (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().origenVentanaPixels_x + pixelsVentana_x)) * (ScriptDatosInterfaz.ancho_x_Pantalla/2) / pixelsVentana_x;
        float locPuntero_y = (p.y - (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().origenVentanaPixels_y + pixelsVentana_y)) * (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla/2) / pixelsVentana_y;

        // Ya hemos centrado el puntero con el rarton, pero el raton se mueve en pixels y el puntero lo hace en unidades de la escala del muro de trabajo. 
        // Como el marco de trabajo se ajusta a la dimension de menor tamaño (ancho o alto del monitor), habra que ajustar los pixels que recorre el 
        // raton hasta llegar al borde correspondiente, con la dimension del muro de trabajo

        Vector3 locPuntero = new Vector3(locPuntero_x, locPuntero_y, 0);
        transform.localPosition = locPuntero;

//        Debug.Log("Desde ScriptCtrlPuntero  -mousePosition_X = " + p.x + " - mousePosition_Y = " + p.y + " - mousePosition_z = " + p.z);
//        Debug.Log("Desde ScriptCtrlPuntero  -LocalPosicion_X = " + transform.localPosition.x + " - LocalPosicion_Y = " + transform.localPosition.y + " - LocalPosicion_z = " + transform.localPosition.z);


        /* ***********************************
        //   COMENTARIO DE DESARROLLO
        // HeaderAttribute intentado controlar los punteros mediante el raton utilizando un raycast. El codigo que sigue funciona bien,
        // con la salvedad de que al darnos el punto de colision del rayo con el muro activo, lo que nos da es la posicion en el 
        // mundo (NO LA LOCAL DEL MURO), con lo que para obtener la posicion local del muro, que es en la que queremos colocar el puntero
        // seria neceario hacer cambios de coordenadas, y no uno solo, ya que el muro tiene sus corrdenadas locales a la rama en la que se 
        // encuentra, que tendra sus coordenadas en la rama en la que nazca y asi sucesivamente hasta llegar a la rama raiz, que es la que 
        // esta en coordenadas del mundo. Geometricamente es calculable, pero creo que el coste computacional va a ser excesivo y por eso he 
        // decidido referenciar los punteros a la posicion del raton por otra via (posiblemente ajustando las posiciones del raton en el monitor
        // a las posiciones del cursor en el muro) MAFG 2021-03-26
        GameObject muroActivo = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.gameObject;
        // Ponemos los valores actuales para inicializar la posicion por si el raycast no da en el muro o algo
        float pos_x = transform.localPosition.x;
        float pos_y = transform.localPosition.y;
        float pos_Z = transform.localPosition.z;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaEntreMuros * (3/2)));
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.gameObject == muroActivo)
            {
                Debug.Log(" - Punto del ojjeto eb que impacta el rayo = " + hit.point.x + " - Y de tag = " + hit.transform.name);
                pos_x = hit.point.x - 10f;
                pos_y = hit.point.y - 10f;
            }
        }
        transform.localPosition = new Vector3(pos_x, pos_y, pos_Z);
        ***************************** */

        // Controlamos ahora que el puntero de usuario no salga del muro de usuario
        //        float tamañoMuroTrabajo_x = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_MuroTrabajo / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().factorEscalaMuroTrabajo; // el muro es un plano de 10x10 baldosas (creo)
        float recorridoRaton_x = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_MuroTrabajo / 2f; // el muro es un plano de 10x10 baldosas (creo)
        float recorridoRaton_y = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_MuroTrabajo / 2f; // el muro es un plano de 10x10 baldosas (creo). OJOOO tomo Z en lugar de Y porque al ser un plano esta girado
            // Obtenemos el valor de posicion
        Vector3 posicion = new Vector3();
        posicion = transform.localPosition;

//        Debug.Log("Desde ScriptCtrlPuntero  - recorridoRaton_x = " + recorridoRaton_x +
//            " - recorridoRaton_y = " + recorridoRaton_y +
//            " - posicion.x = " + posicion.x +
//            " - posicion.y = " + posicion.y);

        // Si se sale de los margenes lo ajustamos
        if (transform.localPosition.x > recorridoRaton_x) {posicion.x = recorridoRaton_x; }
        if (transform.localPosition.x < (-1) * recorridoRaton_x) { posicion.x = (-1) * recorridoRaton_x; }
        if (transform.localPosition.y > recorridoRaton_y) { posicion.y = recorridoRaton_y; }
        if (transform.localPosition.y < (-1) * recorridoRaton_y) { posicion.y = (-1) * recorridoRaton_y; }
            // Incluimos los ajustes en la posicion
        transform.localPosition = posicion;

//        Debug.Log("Desde ScriptCtrlPuntero=>Update, con x muro trabajo = "+ ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_MuroTrabajo +
//            " - Con puntero x : " + posicion.x + " - " + transform.localPosition.x +
//            " - Con puntero z : " + posicion.z +
//            " - Con puntero y : " + posicion.y );


    } // Fin de - void LateUpdate ()

    /// /////////////////////////////////////////////
    /// /////////////////////////////////////////////
    /// Vamos con los trigers

    // Cuando el Game object Usuario LLEGA a un muro de trabajo, este pasa a estar ACTIVO
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MuroDeTrabajo"))
        {
            // Publicamos este muro como el muro activo
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo = other.gameObject;
            // Indicamos mediante la transparencia que el objeto esta activo
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().manejaTransparenciaMaterial(other.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().nivelTransparenciaMuroActivo);
        }
    } // Fin de - void OnTriggerEnter(Collider other) 



    // Cuando el Game object Usuario SALE de un muro de trabajo, este pasa a estar INACTIVO
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MuroDeTrabajo"))
        {
            // Indicamos mediante la transparencia que el objeto esta activo
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().manejaTransparenciaMaterial(other.gameObject, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().nivelTransparenciaMuroInactivo);


            //            other.gameObject.GetComponent<MeshRenderer>().material = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().materialMuro;
            //			other.gameObject.SetActive (false);
        }
    } // Fin de - void OnTriggerEnter(Collider other) 


} // FIn de - public class ScriptCtrlPuntero : MonoBehaviour {
