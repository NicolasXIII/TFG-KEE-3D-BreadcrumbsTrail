using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// /////////// SctlEviTipo_01Fractum_BtnInfo : Script para manejar el boton de indormacion dentro de opciones de un evi fractum de referencia
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-04-22
/// Observaciones :
/// 		Este boton tiene el icono del concepto o instancia que contiene.
/// 		Al pasar por encima del boton con el raton, se visualiza el texro de ayuda a interfaz (en el cambas general) y se
/// 		reproduce tambien el audio de ayuda a interfaz
/// </summary>
public class SctlEviTipo_01Fractum_BtnInfo : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public Canvas CanvasGeneral;


    private Rigidbody rb;

    private float escalaLocalAnchoBoton;
    private float escalaLocalAltoBoton;

    public AudioClip audio_AyuIntf;
    AudioSource audio1;

    // Use this for initialization
    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        CanvasGeneral = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().CanvasGeneral;

        //        escalaLocalAnchoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoBoton;
        //        escalaLocalAltoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoBoton;
        // De primeras ponemos el audio por defect, ya que el audio del fractum podria no estar cargado todabia
        audio_AyuIntf = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().AudioClip_PorDefecto;

        audio1 = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    // Cuando llega a el el puntero de usuario
    void OnTriggerEnter(Collider other)
    {
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log(" Desde SctlEviTipo_01Fractum_BtnInfo => OnTriggerEnter entramos en el triger"); }
        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño y lo recolocamos
        //       this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Activado;
        //        Vector3 ajusteDePposition = new Vector3(escalaLocalAnchoBoton * (-1f / 2f), 0, escalaLocalAltoBoton * (-1 / 2f)); ;
        //        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info + ajusteDePposition;
        // Hacemos sonar el icono de audio de la ayuda a interfaz
        //        audio1.PlayOneShot(audio_AyuIntf, 1); /////////////  no se por que no me lo admite como instancia u objeto. LO QUITO PROVISIONALMENTE, pero debe estar 2021-01-17
        /////////////  asi no me lo admitia y he tenido que ponerlo como sigue (referenciando el componente
        audio_AyuIntf = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().AudioClip_AyuIntf;
        gameObject.GetComponent<AudioSource>().PlayOneShot(audio_AyuIntf, 1);

        // Activamos el texto de ayuda a interfaz
        // Cargamos el texto en el TextMeshProUGUI correspondiente
        string textoAI = this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().txt_nombre_AyuIntf + " : " + 
                            this.transform.parent.transform.parent.GetComponent<SctCtrlEviTipo_01Fractum>().txt_descripcion_AyuIntf;

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // ///////////
        // SI pulsamos el raton cuando estamos sobre el boton, generamos un evi ce busqueda
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            { }
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Boton del mouse activado en algun boton BaseDeEvi_N1 el de info"); }
            //           GameObject nuevoEviBuscador = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().EviTipo_buscador_00);
            //           nuevoEviBuscador.transform.SetParent(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().muro_Activo.transform);

        }
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
//        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Desactivado;
//        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info;

        // Al salir desactivamos el texto de ayuda a interfaz
        //        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().TexAyuIntfBaseDeEvi.SetActive(false);
        //        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.SetActive(false);

    } // Fin de - void OnTriggerEnter(Collider other) 
}
