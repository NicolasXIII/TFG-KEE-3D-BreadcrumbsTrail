using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBtnMenu_N2_1_Herramientas_MigaPan : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    //public bool btn_Estado;
    public bool onTrigger;
    //public bool panera_Activada;
    public bool bt_Estado = false;
    public bool bt_btn_Estado = false;
    public bool click = false;

    public GameObject panera;


    // Inicializacion
    void Start()
    {
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        //panera = GameObject.FindWithTag("Panera");
        //panera = GameObject.Find("Panera");

        //Debug.Log("encontrar panera por tag " + panera.transform.localScale);
        //Debug.Log("encontrar ctrlinterfaz por tag " + ctrlInterfaz.transform.localScale);

        //this.panera_Activada = this.ctrlInterfaz.GetComponent<ScriptCtrlPanera>().activada;
        bt_Estado = false;
        bt_btn_Estado = false;
}

    // Update is called once per frame
    void Update()
    {
        // Pulsacion del boton derecho del raton
        //      GetMouseButtonDown solo se puede usar desde el update
        //      GetMouseButtonDown(0) es el click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            // Bandera que permite mantener la escala_Activada del boton, cuando se clicka en el
            click = !click;

            //panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(click);
            ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().panera.GetComponent<ScriptCtrlPanera>().activar_o_desactivar_Panera(click);
        }
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    // Descripcion
    //      Cuando llega a el el puntero de usuario al boton, lo indicamos al usuario modificando su tamaño
    void OnTriggerEnter(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan_Activado;
        this.onTrigger = true;
    } // Fin de - void OnTriggerEnter(Collider other) 

    // Autor Nicolas Merino Ramirez
    // Fecha 19/10/2022
    void OnTriggerExit(Collider other)
    {
        if (!click)
        {
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnMenu_N2_1_Herramientas_MigaPan;
            onTrigger = false;
        }

    } // Fin de - void OnTriggerEnter(Collider other)

}
