using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script de control del foco de luz
// El foco de luz basicamente sigue al usuario para que la iluminacion sea la adecuada
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2020-03-15
// Observaciones :
// 		Va tambien junto a la camara y al muro de usuario y los telones

public class ScriptCtrlFocoLuz : MonoBehaviour {

	public GameObject ctrlInterfaz;
	public GameObject Usuario;

	void Start ()
	{
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        // He intentado poner aqui los valores de "campoVisionCamara" y "distanciaCamaraUsuario", PERO
        // no consigo que lo haga bien y lo deja a cer. Debe ser por el orden de ejecucion

        // Hacemos a la camara hija del usuario, para que lo siga a todas partes siempre igual
        // Si es la raiz del arbol, ponemos el usuario como hijo y lo colocamos adecuadamente
        transform.SetParent(Usuario.transform);

        ////        float distanciaUsuarioCamara = 10;
        ////        Vector3 posicionUsuario = new Vector3(0.0f, -distanciaUsuarioCamara, 0.0f);
        transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().distanciaLuzUsuario;
        // No se porque, pero tengo que especificar explicitamente que mantenga la direccion de su padre
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Update ()
	{
	}
} // Fin de - public class ScriptCtrlFocoLuz : MonoBehaviour {
