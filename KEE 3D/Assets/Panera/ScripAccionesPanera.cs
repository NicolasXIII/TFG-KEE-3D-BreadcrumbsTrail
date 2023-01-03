using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScripAccionesPanera : MonoBehaviour
{
    GameObject scrollbar;
    float desplazamiento;
    GameObject migas_container;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        migas_container = GameObject.Find("migas_container");

        // Instanciacion de la scrollbar de la panera
        scrollbar = new GameObject();
        scrollbar.name = "scrollbar";
        scrollbar.transform.SetParent(this.transform);
        scrollbar.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Cuando muevo los elementos detro de la panera, por el gameobject donde se visualizaran,
        // se iran activando conforme entren en dicho espacio y desactivando conforme salgan de este
        visualizacion_Evis();
    }

    // Autor Nicolas Merino Ramirez
    // Fecha 2022/12/20
    // Mod 2022/12/27
    // Descripcion
    //      Muestra / Activa los evis en espacio del gameobject panera
    //      y los desactiva cuando salen de dicho espacio
    private void visualizacion_Evis()
    {
        // Cuando muevo los elementos detro de la panera, por el gameobject donde se visualizaran,
        // se iran activando conforme entren en dicho espacio y desactivando conforme salgan de este
        if (this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails.Count > 0)
        {
            foreach (GameObject elem in this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails)
            {
                float panera_inicio_X = this.transform.position.x - (1f / 2f) * this.transform.localScale.x * this.transform.parent.localScale.x;
                float panera_final_X  = this.transform.position.x + (1f / 2f) * this.transform.localScale.x * this.transform.parent.localScale.x;
                // NOTA: Puesto que la escala de la panera en x es 0.3, es decir 1/3 de su padre, es necesario multiplicar tb por la escala del padre
                
                float evi_inicio_X = elem.transform.position.x - elem.transform.localScale.x * 1f / 2f;
                float evi_final_X  = elem.transform.position.x + elem.transform.localScale.x * 1f / 2f;

                if (evi_inicio_X >= panera_inicio_X && evi_final_X <= panera_final_X)
                {
                    elem.SetActive(true);
                }
                else
                {
                    elem.SetActive(false);
                }
            }
        }
    }

    private void OnMouseOver()
    {
        movimiento_Rueda_Raton_Sobre_Panera();
    }


    private void movimiento_Rueda_Raton_Sobre_Panera()
    {
        float velocidad = 80f;

        if (Input.mouseScrollDelta.y > 0 && !estado_Ultimo())
        {
            desplazamiento = this.transform.localPosition.x - velocidad * Time.deltaTime;
            mover();
        }
        else if (Input.mouseScrollDelta.y < 0 && !estado_Primero())
        {
            desplazamiento = this.transform.localPosition.x + velocidad * Time.deltaTime;
            mover();
        }
    }

    private bool estado_Ultimo()
    {
        int ultimo = this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails.Count - 1;

        return this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[ultimo].activeSelf;
    }

    private bool estado_Primero()
    {
        return this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[0].activeSelf;
    }

    private void mover()
    {
        this.transform.localPosition = new Vector3(desplazamiento, this.transform.localPosition.y, this.transform.localPosition.z);

        scrollbar.transform.localScale = new Vector3(desplazamiento / this.transform.localScale.x, scrollbar.transform.localPosition.y, scrollbar.transform.localPosition.z);
        //scrollbar.transform.localPosition = new Vector3(desplazamiento / this.transform.localScale.x, scrollbar.transform.localPosition.y, scrollbar.transform.localPosition.z);
    }

    public void activar_ultimos()
    {
        int lenght = this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails.Count;
        float espacio_panera = this.transform.localScale.x * this.transform.parent.localScale.x;
        float num_evis_en_panera = espacio_panera / this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[0].transform.localScale.x * 1.5f;

        int num_evis_en_panera_INT = (int) num_evis_en_panera;

        Debug.Log("espacio_panera" + espacio_panera + "\n" +
            "num_evis_en_panera" + num_evis_en_panera + "\n" +
            "num_evis_en_panera_INT" + num_evis_en_panera_INT);

        num_evis_en_panera_INT = 3;

        if (lenght > num_evis_en_panera_INT)
        {
            migas_container.transform.localPosition = new Vector3
            (
                                                                         // con el "+1" funciona pero no entiendo bn porque
                -this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[lenght - num_evis_en_panera_INT + 1].transform.localPosition.x,
                migas_container.transform.localPosition.y,
                migas_container.transform.localPosition.z
            );
        }
        
    }

    public void actualizar_scrollbar_tam()
    {
        // Espacio que ocupa un evi + separacion
        float espacio_Evi_X = this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[0].transform.localScale.x + 1f / 2f * this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails[0].transform.localScale.x;
        float espacio_panera = this.transform.localScale.x;
        float espacio_Evis_Totales = this.GetComponent<ScriptCtrlPanera>().List_BreadcrumbsTrails.Count * espacio_Evi_X;

        scrollbar.transform.localScale = new Vector3
        (
            1 / (espacio_Evis_Totales / espacio_panera),
            1f,
            1f
        );

        if (espacio_Evis_Totales <= espacio_panera)
        {
            // Desactivar Scrollbar
            scrollbar.SetActive(false);
        }
        else
        {
            // Instanciacion
            scrollbar.SetActive(true);

            // Nueva poscicion

            // Nueva escala
            scrollbar.transform.localScale = new Vector3
            (
                1 / (espacio_Evis_Totales / espacio_panera),
                1f,
                1f
            );
        }
    }
}
