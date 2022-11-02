using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control de la camara
// Basicamente muueve la camara por el eje Z para que siga al usuario
// colaider correspondientes
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-03-15
// Observaciones :
// 		Lo acompañan el foco de luz, el muro de usuario y los telones
//		El objetivo es que la camara lo encuadre siempre de forma que los botones de control
//		de la interfaz esten siempre accesibles cuando se activen

public class ScriptCtrlCamara : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;


    void Awake()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

    }

        void Start ()
	{

        // He intentado poner aqui los valores de "campoVisionCamara" y "distanciaCamaraUsuario", PERO
        // no consigo que lo haga bien y lo deja a cer. Debe ser por el orden de ejecucion

        // Hacemos a la camara hija del usuario, para que lo siga a todas partes siempre igual
        // Si es la raiz del arbol, ponemos el usuario como hijo y lo colocamos adecuadamente
        transform.SetParent(Usuario.transform);

        Camera.main.fieldOfView = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamaraInicial;


        //		Camera.main.fieldOfView = 160;
        //		Debug.Log ("Desde lateUpdate, valor de campoVisionCamara es :"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamara);


        ////        float distanciaUsuarioCamara = 10;
        ////        Vector3 posicionUsuario = new Vector3(0.0f, -distanciaUsuarioCamara, 0.0f);
        transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaCamaraUsuario;
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        transform.localRotation = Quaternion.Euler(0, 0, 0);

    }  // Fin de - void Start ()

    void Update ()
	{
        //		this.GetComponent<Camera>().aspect = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ancho_x_Pantalla / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla; // Coloca la relacion decimal aspecto decimal LayerMask ScriptCtrlCamara supongo
        //		Debug.Log ("Desde lateUpdate 27, el ancho es :"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ancho_x_Pantalla+" - el alto es : "+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla);

        // UpdateAspect ();

        // Tomamos el campo de vision de la camara
        ////		Camera.main.fieldOfView = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamara;
        //		Camera.main.fieldOfView = 160;
        //		Debug.Log ("Desde lateUpdate, valor de campoVisionCamara es :"+ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamara);
        Camera.main.fieldOfView = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamara;  // Actualizamos el valor para que sea accesible al resto de los scripts del sistema

        // Colocamos el canvasa a la distancia correspondiente de la camara para que encaje con el muro de trabajo
//        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral.GetComponent<Canvas>().planeDistance = (-1f) * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario;
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral.GetComponent<Canvas>().planeDistance = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().FactorOffsetAvanceCanvas * ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario;


        // Colocams la camara a la distancia adecuada del usuario
        ////		transform.position = Usuario.transform.position + ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaCamaraUsuario;

    } // FIn de - void LateUpdate ()



    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Actualiza el aspecto de imagen segun la orientacion del monitor
    // Actualiza el aspecto de imagen segun la orientacion del monitor
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2020-03-13
    // Observaciones :
    //     No se muy bien lo que hago y si funciona o no 
    // 	 lo cojo del sitio web "https://arjierdagames.com/blog/unity/manejar-aspecto-de-pantalla-para-diferentes-resoluciones/"
    //  2020-03-11
    public void UpdateAspect()
	{
		if (ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().modificaOrientacionMonitor)
		{
			this.GetComponent<Camera>().aspect = ScriptDatosInterfaz.ancho_x_Pantalla / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla; // Coloca la relacion decimal aspecto decimal LayerMask ScriptCtrlCamara supongo
		}
		else
		{
			Resize();
		}
	} // Fin de - public void UpdateAspect()


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  camaraAPosicionBase()
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-27
    /// Variables de entrada :
    /// Variables de salida :
    /// Observaciones :
    ///     - Coloca la camara en su posicion base, esto es centrada con el usuario, por detras de el a la distancia correcta y con el campo de vision
    ///     adecuado para visualizar el muro de trabajo
    ///     - EL campo de la camara cuando esta delante del muro lo calculamos como sigue. 
    ///           -La mitad de la escala del muto de trabajo setra el cateto opuesto
    ///           - la distancia de la camara al usuario (osea al muro) sera el cateto contiguo
    ///           - El campo de vivion de la camara ante el muro sera el doble del arco cuya tangente sea el cateto opuesto partido por el contiguo
    ///      Indica cuanto abre el objetivo de la camara (de 0 a 179 grados). Este es el campo para cubrir un muro completo a la distancia del usuario
    /// </summary>
    public void camaraAPosicionBase()
    {
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario = -(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaEntornoUsuario * 2f / 3f); //  -10 distancia_z_CamaraUsuario, va por detras del usuario
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaCamaraUsuario = new Vector3(0.0f, 0.0f, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario);  // La colocamos por detras del usuario, centrada y a la distancia correspondiente

        // La camara no se gira porque en principio siempre va mirando al usuario que va delante de ella

        //        float dimensionPantalla = escalaGeneralMuroTrabajo * 10f; //      Hay que recordar que un muro es un plano, y que un plano son 10x10 baldosas, y que la escala del plano es la escala de una baldosa
//        float dimensionPantalla = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().dimensionRefBaseEnEscala; // Ajustamos la escala a la menor dimension entre ancho y alto esto es "dimensionRefBaseEnEscala"
        float dimensionPantalla = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla; // Ajustamos la escala a la menor dimension entre ancho y alto esto es "dimensionRefBaseEnEscala"
        float distanciaCamara = Math.Abs(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distancia_z_CamaraUsuario); // La distancia de la camara es negativa
        bool radianes = false; // Lo queremos en grados
        ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().campoVisionCamara = caculaCampoVisionCamara(dimensionPantalla, distanciaCamara, ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().marcoPorCiento, radianes);
    }  // Fin de - public void camaraAPosicionBase()

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  caculaCampoVisionCamara
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-27
    /// Variables de entrada :
    ///     - float dimensionPantalla : Son las dimensiones de la pantalla ante la que debe encontrarse centrada la camara 
    ///     - float distanciaCamara : es la distancia de la camara a la pantalla (con signo positivo)
    ///     - float marcoPorCiento : Es el tanto por ciento del tamaño de la pantalla, que el campo visual de la camara excedera a esta (para no ir justo al borde de la pantalla)
    ///     - bool radianes : Si "radianes = true" devuelve el campo visual en radianes. Si "radianes = false" devuelve el campo visual en grados
    /// Variables de salida :
    ///     - float campoVisionCamara : Es el campo de vison de la camara esta en GRADOS NO EN RADIANES (su destino debe ser el parametro FIeld of view de la camara)
    /// Observaciones :
    ///     
    /// 	    
    /// </summary>
    public float caculaCampoVisionCamara(float dimensionPantalla, float distanciaCamara, float marcoPorCiento, bool radianes)
    {
        if (marcoPorCiento >= 100)
        {
            marcoPorCiento = 0;
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log(" Dede ScriptCtrlCamara=>caculaCampoVisionCamara. marcoPorCiento excede de 100"); }
        }
        double dimensionConMarco = dimensionPantalla + (dimensionPantalla * marcoPorCiento / 100);
        double tangente = (dimensionConMarco * 1f / 2f) / (double)distanciaCamara; // La distancia de la camara es negativa
        double angTangenteEnRadianes = Math.Atan(tangente);

        float esteCampoVisionCamara = 2f * (float)angTangenteEnRadianes;  // se multiplica por 2 porque lo hemos calculado con cateto opuesto la mitad de la pantalla y debe cubir toda la pantalla

        if (!radianes) { esteCampoVisionCamara = esteCampoVisionCamara * (180f / (float)Math.PI); }  // Si lo han pedido en grados, se pasa a grados

        // Definimos ahora la relacion ancho-alto del rectangulo de vision de la camara, para que se ajuste al tamaño panoramico de la ventana que ocupa la aplicacion en el monitor

        if (DatosGlobal.niveDebug > 1000)
        {
            Debug.Log(" Dede ScriptCtrlCamara=>caculaCampoVisionCamara. dimensionPantalla = " + dimensionPantalla +
            " - dimensionConMarco = " + dimensionConMarco +
            " - distanciaCamara = " + distanciaCamara +
            " - tangente = " + tangente +
            " - angTangenteEnRadianes = " + angTangenteEnRadianes +
            " - esteCampoVisionCamara = " + esteCampoVisionCamara +
            //            " - campoVisionCamara = " + GetComponent<ScriptDatosInterfaz>().campoVisionCamara +
            " - scaledPixelHeight  = " + Camera.main.scaledPixelHeight +
            " - scaledPixelWidth  = " + Camera.main.scaledPixelWidth
            );
        }

        Camera.main.aspect = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().ratio_dimensiones_Pantalla;

        return esteCampoVisionCamara;
    }  // Fin de - public float caculaCampoVisionCamara(float dimensionPantalla, float distanciaCamara, float marcoPorCiento, bool radianes)




    /// //////////////////////////////////////////////////////////////////////////////////////
    /// ///////////  Actualiza el aspecto de imagen segun la orientacion del monitor
    // Actualiza el aspecto de imagen segun la orientacion del monitor
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2020-03-13
    // Observaciones :
    // 		Trabaja para "public void UpdateAspect()"
    //     No se muy bien lo que hago y si funciona o no 
    // 	 lo cojo del sitio web "https://arjierdagames.com/blog/unity/manejar-aspecto-de-pantalla-para-diferentes-resoluciones/"
    //  2020-03-11
    private void Resize()
	{
		//Aspect ratio
		float aspectoDeseado = ScriptDatosInterfaz.ancho_x_Pantalla / ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().alto_y_Pantalla;

		//check actual aspectratio
		float ratioMonitor = (float)Screen.width / (float)Screen.height;

		//check actual vs wish aspect
		float scaleheight = ratioMonitor / aspectoDeseado;

		if (scaleheight < 1.0f) //portrait
		{
			Rect rect = GetComponent<Camera>().rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			GetComponent<Camera>().rect = rect;
		}
		else //lanscape
		{
			float scalewidth = 1.0f / scaleheight;

			Rect rect = GetComponent<Camera>().rect;

			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;

			GetComponent<Camera>().rect = rect;
		}

		//Create background in black
		CreateBackGround();

	} // FIn de - private void Resize()

	/// //////////////////////////////////////////////////////////////////////////////////////
	/// ///////////  Rellena de negro la zona que queda en el monitor que no esta ocupada por la imagen dependiendo del tamaño de imagen
	// Rellena de negro la zona que queda en el monitor que no esta ocupada por la imagen dependiendo del tamaño de imagen
	// Autor : 	Miguel Angel Fernandez Graciani
	// Fecha :	2020-03-13
	// Observaciones :
	// 		Trabaja para "public void UpdateAspect()"
	//     No se muy bien lo que hago y si funciona o no 
	// 	 lo cojo del sitio web "https://arjierdagames.com/blog/unity/manejar-aspecto-de-pantalla-para-diferentes-resoluciones/"
	//  2020-03-11
	private void CreateBackGround()
	{
		Camera cam = new GameObject().AddComponent<Camera>();
		cam.gameObject.isStatic = true;
		cam.depth = -10;
		cam.cullingMask = 0;
		cam.farClipPlane = 1f;
		cam.orthographic = true;
		cam.backgroundColor = Color.black;
		cam.gameObject.name = "BackGround_Camera";

	} // FIn de - private void CreateBackGround()


} // Fin de - public class ScriptCtrlCamara : MonoBehaviour {

