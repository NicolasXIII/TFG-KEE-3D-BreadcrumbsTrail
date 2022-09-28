
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control de los punteros que maneja el usuario
// Mueve el cursor para que el usuario pueda seleccionar botones y elementos en el muro de usuario
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-04-02
// Observaciones :
// 		- Debemos tener al menos dos punteros. Uno para la mano derecha y otro para la izquierda
//				Se manejaran bien con dos ratones o bien con la pantlla tactil u otros elementos de control que pueda manejar el usuario
// 		- Cada puntero son en realidad un gupo de punteros, a saber:
//			- Puntero de Usuario : Es el que va pegado al usuario y por tanto se usa para manejar los elementos del muro de trabajo
//			- Puntero de Muro de Usuario :  Va con el puntero de usuario (es hijo suyo), pero en Z esta algo detras, para colocarse sobre el muro de usuario
//					Cuando el usuario se coloca en el muro de trabajo este puntero coloca sobre el muro de usuario. Se usa para manejar los elementos del muro de Usuario
// 		- Este script maneja el comportamiento del muro de usuario

public class ScriptCtrlPuntMuroUsuario : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;
    public GameObject PunteroUsuario;

    private Rigidbody rb;

	private GameObject BtnMenu_N2_CtrlInterfaz;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_cargar;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_grabar;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_audio;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_complejidad;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_idiomas;
    private GameObject BtnMenu_N2_1_CtrlInterfaz_ambito;

    private GameObject BtnMenu_N2_Escena;

    private GameObject BtnMenu_N2_Herramientas;

    private bool primera = true;
	private bool segunda = false;

    private bool DentroDeBtnMenu_N1_1_General_CtrlInterfaz = false;
    private bool DentroDeBtnMenu_N1_1_General_Escena = false;
    private bool DentroDeBtnMenu_N1_1_General_Herramientas = false;

    private bool DentroDeBtnMenu_N2_CtrlInterfaz = false;
    private bool DentroDeBtnMenu_N2_Escena = false;
    private bool DentroDeBtnMenu_N2_Herramientas = false;

    void Start ()
	{
		// Asignamos objetos
		ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
		Usuario = GameObject.FindWithTag("Usuario");
        PunteroUsuario = GameObject.FindWithTag("PunteroUsuario");

        // Lo ponemos por defecto en estado activo
        this.GetComponent<ClassPuntero>().estadoPuntero = ClassPuntero.estPuntero_activo;

    } // FIn de - void Start ()

    void Update ()
	{
		if (primera) 
		{
			BtnMenu_N2_CtrlInterfaz = GameObject.FindWithTag("BtnMenu_N2_CtrlInterfaz");
            BtnMenu_N2_1_CtrlInterfaz_cargar = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_cargar");
            BtnMenu_N2_1_CtrlInterfaz_grabar = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_grabar");
            BtnMenu_N2_1_CtrlInterfaz_audio = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_audio");
            BtnMenu_N2_1_CtrlInterfaz_complejidad = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_complejidad");
            BtnMenu_N2_1_CtrlInterfaz_idiomas = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_idiomas");
            BtnMenu_N2_1_CtrlInterfaz_ambito = GameObject.FindWithTag("BtnMenu_N2_1_CtrlInterfaz_ambito");

            BtnMenu_N2_Escena = GameObject.FindWithTag("BtnMenu_N2_Escena");

            BtnMenu_N2_Herramientas = GameObject.FindWithTag("BtnMenu_N2_Herramientas");

			primera = false;
			segunda = true;
		}
		if (segunda) // En principio, si desactivamos los botones de N2 padres, desactivamos a sus hijos y no hay que desactivarlos aqui
		{
			BtnMenu_N2_CtrlInterfaz.SetActive(false);
			BtnMenu_N2_Escena.SetActive(false);
			BtnMenu_N2_Herramientas.SetActive(false);
			segunda = false;
		}

        // Ajustamos la posicion de este puntero de muro de usuario
        // EL PUNTERO DE MURO DE USUARIO ES HIJO DEL PUNTERO DE USUARIO, POR LO QUE SUS CORRDENADAS SON COORDENADAS LOCALES A ESTE
        // OJOO. esta en otro plano que el puntero de usuario, por lo que al moverse con su padre (puntero de usuario), debido a la 
        // prespectiva de la camara, se desplaza separado de este (el puntero de usuario) y queremos verlos como uno solo para poder
        // realizar su funcion de puntero de forma adecuada.
        // EL plano de usuario esta mas cervca de la camara, y para que este en el mismo campo de vision que el muro de trabajo, este es mas pequeño
        // y el puntero del muro de usuarios si se desplaza lo mismo  que el de usuario (en x e y), visualmente se desplaza ucho mas.
        // para que los dos punteros se vean como uno solo realizamos el siguiente ajuste :
        // VER GRAFICO "calculo posicion puntero muro usr.png" donde se realiza el calclo pertinente

        Vector3 posicion = new Vector3();

        float PU_x = PunteroUsuario.transform.localPosition.x;
        float PU_y = PunteroUsuario.transform.localPosition.y;
        float DC = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario;
        float DPMU = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_PuntMuroUsuario;

        // realizamos el calculo. Lo dividimos por la escala del puntero de usuario, (que es su padre), para ajustar la escala del calculo
        float posLocal_x = (-1) * (PU_x - (((DC- DPMU) * PU_x) / DC)) * (1/ ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_x_PunteroUsuario);
        float posLocal_y = (-1) * (PU_y - (((DC - DPMU) * PU_y) / DC)) * (1 / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_y_PunteroUsuario);

//        Debug.Log("Desde ScriptCtrlPuntMuroUsuario Calculando posision puntero muro usuario con  - PU_x = " + PU_x +
//            " - PU_y = " + PU_y  +
//            " - DC = " + DC +
//            " - DPMU = " + DPMU +
//            " - posLocal_x = " + posLocal_x +
//            " - posLocal_y = " + posLocal_y );

        posicion.x = posLocal_x;
        posicion.y = posLocal_y;
        posicion.z = DPMU;

        // Incluimos los ajustes en la posicion
//        transform.localPosition = posicion;
        transform.localPosition = posicion;

    } // Fin de - void LateUpdate ()

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando el Game object Usuario LLEGA a un muro de trabajo, este pasa a estar ACTIVO
    void OnTriggerEnter(Collider other) 
	{
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" desde ScriptCtrlPuntMuroUsuario => OnTriggerEnter con other.gameObject.Tag = " + other.gameObject.tag); }
        // ///////////
        // SI entramos en un boton de N1_1, activamos el boton de N2 correspondiente
        if (other.gameObject.CompareTag("BtnMenu_N1_1_General_CtrlInterfaz"))
		{
			BtnMenu_N2_CtrlInterfaz.SetActive(true);
            DentroDeBtnMenu_N1_1_General_CtrlInterfaz = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
        if (other.gameObject.CompareTag("BtnMenu_N1_1_General_Escena"))
		{
			BtnMenu_N2_Escena.SetActive(true);
            DentroDeBtnMenu_N1_1_General_Escena = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Escena"))
		if(other.gameObject.CompareTag("BtnMenu_N1_1_General_Herramientas"))
		{
			BtnMenu_N2_Herramientas.SetActive(true);
            DentroDeBtnMenu_N1_1_General_Herramientas = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))

        // ////////////
        // Controlamos si estamos dentro de uno de los botones de N2
        if (other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
        {
            DentroDeBtnMenu_N2_CtrlInterfaz = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
        if (other.gameObject.CompareTag("BtnMenu_N2_Escena"))
        {
            DentroDeBtnMenu_N2_Escena = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Escena"))
        if (other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))
        {
            DentroDeBtnMenu_N2_Herramientas = true;
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))


    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el Game object Usuario SALE de un muro de trabajo, este pasa a estar INACTIVO
    void OnTriggerExit(Collider other) 
	{
		if(other.gameObject.CompareTag("BtnMenu_N1_1_General_CtrlInterfaz"))
		{
            DentroDeBtnMenu_N1_1_General_CtrlInterfaz = false;
            if (!DentroDeBtnMenu_N2_CtrlInterfaz)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_CtrlInterfaz.SetActive(false);
            }
		} // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
		if(other.gameObject.CompareTag("BtnMenu_N1_1_General_Escena"))
		{
            DentroDeBtnMenu_N1_1_General_Escena = false;
            if (!DentroDeBtnMenu_N2_Escena)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_Escena.SetActive(false);
            }
		} // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Escena"))
		if(other.gameObject.CompareTag("BtnMenu_N1_1_General_Herramientas"))
		{
            DentroDeBtnMenu_N1_1_General_Herramientas = false;
            if (!DentroDeBtnMenu_N2_Herramientas)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_Herramientas.SetActive(false);
            }
		} // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))

        if (other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
        {
            DentroDeBtnMenu_N2_CtrlInterfaz = false;
            if (!DentroDeBtnMenu_N1_1_General_CtrlInterfaz)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_CtrlInterfaz.SetActive(false);
            }
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_CtrlInterfaz"))
        if (other.gameObject.CompareTag("BtnMenu_N2_Escena"))
        {
            DentroDeBtnMenu_N2_Escena = false;
            if (!DentroDeBtnMenu_N1_1_General_Escena)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_Escena.SetActive(false);
            }
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Escena"))
        if (other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))
        {
            DentroDeBtnMenu_N2_Herramientas = false;
            if (!DentroDeBtnMenu_N1_1_General_Herramientas)  // Si ebtrado en el boton de N2 no lo desactivamos
            {
                BtnMenu_N2_Herramientas.SetActive(false);
            }
        } // Fin de - if(other.gameObject.CompareTag("BtnMenu_N2_Herramientas"))




    } // Fin de - void OnTriggerEnter(Collider other) 
      // Cuando el Game object Usuario SALE de un muro de trabajo, este pasa a estar INACTIVO

} // Fin de - public class ScriptCtrlPuntMusoUsuario : MonoBehaviour {
