using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using UnityEngine.Networking;

public class ScriptCtrlEviTipo_muestraGetDetails_00 : MonoBehaviour {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public bool contenidoCumplimentado;

    public XmlDocument midocumentoKDL = new XmlDocument();

    //Cargamos un KDL concerto
    //        string rutaACargar = "http://crab.uclm.es/klw/pruebasPau/uploads/gen_miDAUS_limpio.xml";
    public string rutaACargar = ConceptosConocidos.gen_dks_desarrollo_host + "/Pruebas/docsXml/gen_miCasaBueno.xml";
    // public string rutaACargar = ConceptosConocidos.gen_dks_desarrollo_host + "/Pruebas/docsXml/gen_miCasaBueno.xml";

    void Start()
    {
        // Asignamos objetos
        ctrlInterfaz = GameObject.FindWithTag("ctrlInterfaz");
        Usuario = GameObject.FindWithTag("Usuario");

        UnityEngine.Debug.Log("Vamos a cargar el fichero:");
        UnityEngine.Debug.Log(rutaACargar);

        // Cargamos los datos del EVI, llamando a su DKS correspondiente
            // Arrancamos una corrutina para que traiga el KDL de forma asincrona (sin detener la ejecucion)
            StartCoroutine("cumplimentaDatosEviTipo_muestraGetDetails_00");

    } // FIn de - void Start ()

    void Update()
    {

    } // Fin de - void LateUpdate ()

    IEnumerator cumplimentaDatosEviTipo_muestraGetDetails_00()
    {

        UnityWebRequest www = UnityWebRequest.Get(rutaACargar);
        yield return www.Send();

        if (www.isNetworkError)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log(www.error); }
        }
        else
        {
            // Show results as text
            if (DatosGlobal.niveDebug > 100)
            { Debug.Log(www.downloadHandler.text); }
        }

        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Aqui fin del KDL recibido"); }

    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()
}
