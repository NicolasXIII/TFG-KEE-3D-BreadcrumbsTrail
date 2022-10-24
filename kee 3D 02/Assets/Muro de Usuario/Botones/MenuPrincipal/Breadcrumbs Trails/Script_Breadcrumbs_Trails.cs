using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Breadcrumbs_Trails : MonoBehaviour
{
    public GameObject ctrlInterfaz;
    public GameObject usuario;
    public GameObject muroUsuario;
    public GameObject Contenedor_BreadcrumbsTrails;

    // Estado de las migas de pan
    public bool bt_Estado = false;
    public bool bt_btn_Estado = false;
    public bool click = false;

    // Cambiar GameObject por Evis o Evi de muro
    public List<GameObject> List_BreadcrumbsTrails;

    
    
    




    /// <summary> 
    /// CODIGO ESPARCIDO
    /// 
    ///     La instanciacion del boton de las migas de pan se hace en:
    ///         ScripCtrlMuroUsuaruio : line 163
    /// </summary>




    // Start is called before the first frame update
    void Start()
    {
        // Asignamos objetos en tiempo de ejecucion
        // SOLUCION PROBLEMA :
        //      A unity no le gusta que haga esta asignacion en la declaracion del atributo (arriba), lo quiere en el metodo start()
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        usuario = GameObject.FindWithTag("Usuario");
        muroUsuario = GameObject.FindWithTag("MuroUsuario");
        GameObject muroUsu = GameObject.Find("MuroUsuario");

        this.Contenedor_BreadcrumbsTrails = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Contenedor_BreadcrumbsTrails);
        this.Contenedor_BreadcrumbsTrails.transform.SetParent(muroUsuario.transform);
        this.Contenedor_BreadcrumbsTrails.transform.localPosition = new Vector3(-0.361000001f, -0.39199999f, -15f);
        this.Contenedor_BreadcrumbsTrails.transform.localScale = new Vector3(0.206234068f, 0.0904989168f, 0.100000001f);
        this.Contenedor_BreadcrumbsTrails.SetActive(false);

        //this.Bt_Contenedor = GetComponent<ScriptCtrlMuroUsuario>().Contenedor_BreadcrumbsTrails;
        //Debug.Log("Resultado de FindGameObjectsWithTag " + GetComponent<ScriptCtrlMuroUsuario>().Contenedor_BreadcrumbsTrails + FindGameObjectsWithTag("BT_Contenedor").Length);
    } // Fin de - void Start()


    void Update()
    {
        // Pulsacion del boton derecho del raton
        //      GetMouseButtonDown solo se puede usar desde el update
        //      GetMouseButtonDown(0) es el click izquierdo
        if (Input.GetMouseButtonDown(0) && bt_btn_Estado)
        {
            this.bt_btn_click();
            click = true;
        }
    }

    // ***************************************************************************************
    // ******************************            HOVER          ******************************
    // ***************************************************************************************

    // Autor Nicolas Merino Ramirez
    // Fecha 28/06/2022
    // Descripcion
    //      SI entramos en el boton, lo indicamos al usuario modificando su tamaño
    void OnTriggerEnter(Collider other)
    {
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnBreadcrumbsTrails_Activado;
        this.bt_btn_Estado = true;

    } // Fin de - void OnTriggerEnter(Collider other) 

    // Autor Nicolas Merino Ramirez
    // Fecha 28/06/2022
    // Descripcion
    //      Cuando el puntero lo abandona, este pasa a estar INACTIVO y recuperamos su tamaño normal
    void OnTriggerExit(Collider other)
    {
        // Solo mientras las migas de pan estan desactivadas el icono se hace peke
        // Si, las migas estan activadas, no quiero que el icono se minimice
        if (!this.bt_Estado)
        {
            this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnBreadcrumbsTrails;
            this.bt_btn_Estado = false;
        }

    } // Fin de - void OnTriggerEnter(Collider other) 

    // ***************************************************************************************

    // Autor Nicolas Merino Ramirez
    // Fecha 29/06/2022
    // Descripcion
    //      Al hacer click sobre el boton pasa lo siguiente:
    void bt_btn_click()
    {
        this.breadcrumbs_Trail();

        Debug.Log("Trigger = " + this.bt_btn_Estado + "\n" +
            "Migas de pan = " + this.bt_Estado);

        // Busco el "Contenedor_BreadcrumbsTrails" del script "ScriptCtrlMuroUsuario" y lo asigno a Bt_Contenedor
        //Bt_Contenedor = GetComponentIn<ScriptCtrlMuroUsuario>(). FindWithTag("BT_Contenedor");

        this.click = false;

        // ACTIVADO, es decir, boton con la escala aumentada
        // COMPLETAR - this.breadcrumbs activar
        this.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnBreadcrumbsTrails_Activado;
        this.bt_btn_Estado = true;
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 07/07/2022
    // Descripcion
    //      Este metodo activa y desactiva el contenedor donde se meten los EVIs, formando asi las migas de pan
    void breadcrumbs_Trail()
    {
        this.bt_Estado = !this.bt_Estado;

        this.Contenedor_BreadcrumbsTrails.SetActive(this.bt_Estado);
        //muroUsuario.GetComponent<ScriptCtrlMuroUsuario>().Contenedor_BreadcrumbsTrails.SetActive(this.bt_Estado);
    }

    

    // Autor Nicolas Merino Ramirez
    // Fecha 23/05/2022
    // Descripcion
    //      Annadir elementos al vector de migas de pan, y en caso de que el elemento ya este
    //      dentro del vector, se eliminan todos los que estan detras del elemento introducido
    public void annadir_Evi_BreadcrumbsTrail(GameObject evi)
    {
        // Si las migas de pan NO contienen el nuevo elemento, se annade
        if (!this.List_BreadcrumbsTrails.Contains(evi))
        {
            this.List_BreadcrumbsTrails.Add(evi);
        } 
        else // en caso contrario elimino todos los elementos detras del nuevo evi
        {
            int posicion_evi = this.List_BreadcrumbsTrails.IndexOf(evi);
            this.List_BreadcrumbsTrails.RemoveRange( posicion_evi + 1, this.List_BreadcrumbsTrails.Count - (posicion_evi + 1));
        }

        // Ahijo la miga de pan a la panera
        evi.transform.SetParent(this.Contenedor_BreadcrumbsTrails.transform);

        this.redimensionar(evi);

    } // Fin de - public void annadir_Evi_BreadcrumbsTrail(GameObject evi)

    void redimensionar(GameObject evi)
    {
        // pongo a los elementos de la panera, la escala de los botones de herramientas,...
        evi.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escala_BtnBreadcrumbsTrails;

        // pongo el evi en la panera, pero en el eje x lo muevo ".12f" para crear una separacion
        evi.transform.localPosition = new Vector3
        (
            this.List_BreadcrumbsTrails.Count + .12f,
            this.Contenedor_BreadcrumbsTrails.transform.position.y,
            this.Contenedor_BreadcrumbsTrails.transform.position.z
        );
    }
}
