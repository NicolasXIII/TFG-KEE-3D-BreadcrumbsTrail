using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Script_BaseDeEvi_N1_Info : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public Canvas CanvasGeneral;

    private GameObject objeto_Evi_Raiz;

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

        escalaLocalAnchoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAnchoBoton;
        escalaLocalAltoBoton = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaAltoBoton;

        audio1 = GetComponent<AudioSource>();

        objeto_Evi_Raiz = transform.parent.gameObject;

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// ///////////////////////////////////
    /// ///////////////////////////////////
    /// Vamos con los trigers
    /// <param name="other">Other.</param>

    void OnTriggerEnter(Collider other)
    {
        // Cuando llega a el el puntero de usuario

        // ///////////
        // SI entramos en un boton de N2_1, lo indicamos al usuario modificando su tamaño y lo recolocamos
        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Activado;
        Vector3 ajusteDePposition = new Vector3(escalaLocalAnchoBoton *(-1f/2f), 0, escalaLocalAltoBoton * (-1/2f)); ;
        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info + ajusteDePposition;
        // Hacemos sonar el icono de audio de la ayuda a interfaz
            //        audio1.PlayOneShot(audio_AyuIntf, 1); /////////////  no se por que no me lo admite como instancia u objeto. LO QUITO PROVISIONALMENTE, pero debe estar 2021-01-17
            /////////////  asi no me lo admitia y he tenido que ponerlo como sigue (referenciando el componente
        gameObject.GetComponent<AudioSource>().PlayOneShot(audio_AyuIntf, 1);

        // Activamos la imagen de ayuda ainterfaz  de ayuda a interfaz
        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.SetActive(true);

        objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().caja_texInfoCanvasEviBase.gameObject.SetActive(true);

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando llega a el el puntero de usuario
    void OnTriggerStay(Collider other)
    {
        // ///////////
        // SI pulsamos el raton cuando estamos sobre el boton, hacemos que la informcion del concepto aparezca en el "PanelCanvasCompleto" para visualizarla concenientemente
        if (Input.GetMouseButtonDown(0))
        {
            if (!ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().gestionaEnPerRefracBotonMouse(other.gameObject))
            {
                // Obtenemos la informacion del concepto para mostrarla en "PanelCanvasCompleto"
                string infoConcepto = " - IDENTIFICADOR : ";
                infoConcepto = infoConcepto + "\n\t" + " - key : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().key;
                infoConcepto = infoConcepto + "\n\t" + " - host : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().host;
                infoConcepto = infoConcepto + "\n\t" + " - cualificador : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().cualificador;
                infoConcepto = infoConcepto + "\n\t" + " - ordinal : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().ordinal;
                infoConcepto = infoConcepto + "\n\t" + " - fechUltMod : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().fechUltMod;
                infoConcepto = infoConcepto + "\n" + " - AYUDA A INTERFAZ : ";
                infoConcepto = infoConcepto + "\n\t" + " - Nombre : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().txt_nombre_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - Descripcion : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().txt_descripcion_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - Rotulo : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().txt_rotulo_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - imagen_AyuIntf : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().imagen_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - icono_AyuIntf : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().icono_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - audio_AyuIntf : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().audio_AyuIntf;
                infoConcepto = infoConcepto + "\n\t" + " - Idioma : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().idioma_AyuIntf;
                infoConcepto = infoConcepto + "\n" + " - DATOS DE SIN TECHO : ";
                infoConcepto = infoConcepto + "\n\t" + " - Contenido : " + objeto_Evi_Raiz.GetComponentInParent<ScriptCtrlBaseDeEvi>().texto_T_deSinTechoCanvasEviBase;
                infoConcepto = infoConcepto + "\n" + " - DATOS EN KEE : ";
                infoConcepto = infoConcepto + "\n\t" + objeto_Evi_Raiz.name;

                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().PanelCanvasCompleto.SetActive(true);
                ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextCanvasGeneralCompleto.text = infoConcepto;
            }
        }
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // ///////////
        // Al salir de un boton de N2_1, lo indicamos al usuario modificando su tamaño al normal
        this.transform.localScale = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().escalaBtn_BaseDeEvi_N1_Desactivado;
        this.transform.localPosition = this.transform.parent.GetComponent<ScriptCtrlBaseDeEvi>().positionBtn_Evi_Info;

        // Al salir desactivamos el texto de ayuda a interfaz
        this.GetComponentInParent<ScriptCtrlBaseDeEvi>().ImgAyuIntfBaseDeEvi.SetActive(false);

        // Desactivamos el "textInfoElemento" del canvas de evi base para ocultar la info de ayuda a interfaz del evi
        objeto_Evi_Raiz.GetComponent<ScriptCtrlBaseDeEvi>().caja_texInfoCanvasEviBase.gameObject.SetActive(false);

    } // Fin de - void OnTriggerEnter(Collider other) 
}
