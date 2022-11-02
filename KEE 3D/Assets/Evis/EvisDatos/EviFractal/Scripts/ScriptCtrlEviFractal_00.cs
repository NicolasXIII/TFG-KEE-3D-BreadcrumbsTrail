using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using UnityEngine.Networking;
 using UnityEngine.EventSystems;
 using UnityEngine.Events;

public class ScriptCtrlEviFractal_00 : MonoBehaviour, IDragHandler {

    public GameObject ctrlInterfaz;
    public GameObject Usuario;

    public Vector3 escalaBtnEliminar;
	public Vector3 positionBtnEliminar;
    public Vector3 escalaBtnMinimizar;
    public Vector3 positionBtnMinimizar;
    public Vector3 escalaBtnIcono;
    public Vector3 positionBtnIcono;
    public Vector3 escalaBtnDesplazar;
    public Vector3 positionBtnDesplazar;

    public bool contenidoCumplimentado;

    public TextMesh textoArchivoXml; 

    public TextMesh descripcionAyudaInterfaz;

    public float z = 0.0f;

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

        //UnityEngine.Debug.Log("Vamos a cargar el fichero:");
        //UnityEngine.Debug.Log(rutaACargar);

        // Cargamos los datos del EVI, llamando a su DKS correspondiente
            // Arrancamos una corrutina para que traiga el KDL de forma asincrona (sin detener la ejecucion)

         //StartCoroutine("cumplimentaDatosEviTipoFractal_00");



    // Usando el XML local en vez de del servidor

        //string ruta = "C:/Users/José/ProyectosUnity/KEE3D_002_base/Assets/XML_conceptos/gen_miCasaBuenoIdeando_01.xml";

            // Leer el texto directamente del archivo
        //StreamReader reader = new StreamReader(ruta); 
        //Debug.Log(reader.ReadToEnd());
        //string textoXml = reader.ReadToEnd();
        //Debug.Log(textoXml);
        //GenerarEviFractal(textoXml);

        TextMesh textoArchivo = Instantiate(this.textoArchivoXml);
        GenerarEviFractal(textoArchivo.text);


    } // FIn de - void Start ()

    void Update()
    {

    } // Fin de - void LateUpdate ()

    public void OnDrag(PointerEventData eventData)
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = z;
		
		transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
	}

    IEnumerator cumplimentaDatosEviTipoFractal_00()
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
            GenerarEviFractal(www.downloadHandler.text);
        }

    } // FIn de - IEnumerator cumplimentaDatosEviTipo00()

    public void GenerarEviFractal(string textoXML)
    {
        string[] arrayIdentificador = new string[3]; // array para la información de I
        string[] arrayControlConfiguracion = new string[2];  // array para la información de F
        string[] arrayAyudaInterfaz = new string[6]; // array para la información de P
        int numeroHijos = 0;
        int numeroHijosPrimerNivel = 0;
        List<string[]> descripcionHijosListaPrimerNivel = new List<string[]>(); // Lista de arrays con la información de los hijos
        List<string[]> descripcionHijosLista = new List<string[]>(); // Lista de arrays con la información de los hijos

        //Variable DOM donde cargaremos el KDL buscado desde el servidor
        XmlDocument documentoXMLEvi = new XmlDocument();
        documentoXMLEvi.LoadXml(textoXML);

        // Información de la I, F y P
        arrayIdentificador = obtener_I_IdentificadorConcepto(documentoXMLEvi);
        arrayControlConfiguracion = obtener_F_ControlConfiguracionConcepto(documentoXMLEvi);
        arrayAyudaInterfaz = obtener_P_AyudaInterfazConcepto(documentoXMLEvi);

        //Información de D
        // Se obtiene el número de hijos de Casa y después se recorre con un for para crear una lista de array con la
        // información de todos ellos
        numeroHijos = obtenerHijosDescripcion(documentoXMLEvi);
        for (int i = 1; i <= numeroHijos; i++)
        {
            descripcionHijosLista.Add(obtiene_D_DescripcionHijos(documentoXMLEvi, i));
        }
        // Para mostrar por consola los string dentro de los arrays de la lista de hijos
        descripcionHijosLista.ForEach(delegate(String[] datosHijo)
        {
            //UnityEngine.Debug.Log(datosHijo[0]);
            //UnityEngine.Debug.Log(datosHijo[1]);
            //UnityEngine.Debug.Log(datosHijo[2]);
            //UnityEngine.Debug.Log(datosHijo[4]);
            //UnityEngine.Debug.Log(datosHijo[5]);
        });

        numeroHijosPrimerNivel = obtenerHijosDescripcionPrimerNivel(documentoXMLEvi);
        for (int i = 1; i <= numeroHijosPrimerNivel; i++)
        {
            descripcionHijosListaPrimerNivel.Add(obtiene_Info_HijosPrimerNivel(documentoXMLEvi, i));
        }
        // Para mostrar por consola los string dentro de los arrays de la lista de hijos
        descripcionHijosListaPrimerNivel.ForEach(delegate(String[] datosHijo)
        {
            //UnityEngine.Debug.Log(datosHijo[0]);
            //UnityEngine.Debug.Log(datosHijo[1]);
            //UnityEngine.Debug.Log(datosHijo[2]);
            //UnityEngine.Debug.Log(datosHijo[4]);
            //UnityEngine.Debug.Log(datosHijo[5]);
        });

/* ***********************************************************************
/* ****************  Quitamos botones  ***********************************

        // botón eliminar
        GameObject btn_Eliminar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Eliminar);
        btn_Eliminar.transform.SetParent(this.transform);
        btn_Eliminar.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnEliminar;
        btn_Eliminar.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnEliminar;

        // botón minimizar
        GameObject btn_Minimizar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Minimizar);
        btn_Minimizar.transform.SetParent(this.transform);
        btn_Minimizar.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnMinimizar;
        btn_Minimizar.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnMinimizar;

        GameObject btn_Icono = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Icono);
        btn_Icono.transform.SetParent(this.transform);
        btn_Icono.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnIcono;
        btn_Icono.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnIcono;
        btn_Icono.transform.localRotation = Quaternion.Euler (90f, 180f, 0);

        GameObject btn_Desplazar = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Btn_Evi_Desplazar);
        btn_Desplazar.transform.SetParent(this.transform);
        btn_Desplazar.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnDesplazar;
        btn_Desplazar.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnDesplazar;

        TextMesh texto_Nombre = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoNombre);
        texto_Nombre.transform.SetParent(this.transform);
        texto_Nombre.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoNombre;
        texto_Nombre.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoNombre;
        texto_Nombre.text = arrayAyudaInterfaz[0];

        TextMesh texto_descripcion = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoDescripcion);
        texto_descripcion.transform.SetParent(this.transform);
        texto_descripcion.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoDescripcionOculto;
        texto_descripcion.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoDescripcionOculto;
        texto_descripcion.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);
        texto_descripcion.text = arrayAyudaInterfaz[1];

* ****************  FIN Quitamos botones  ***********************************
* ***********************************************************************  */




/* ***********************************************************************
/* ****************  Quitamos Hijos  ***********************************


        // SE EMPIEZA A AÑADIR LOS HIJOS
        //UnityEngine.Debug.Log(obtenerNumeroHijosPrimerNivel(documentoXMLEvi));
        int numHijosPrimerNivel = obtenerNumeroHijosPrimerNivel(documentoXMLEvi);

        for (int i = 1; i <= numHijosPrimerNivel; i++)
        {
            
            // Vemos si el hijo tiene otros hijos
            if (!tieneHijos(documentoXMLEvi, i)) {

                GameObject EviAyudaInterfaz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Evi_AyudaInterfaz);
                EviAyudaInterfaz.transform.SetParent(this.transform);
                EviAyudaInterfaz.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaEviAyudaInterfaz;
                EviAyudaInterfaz.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionEviAyudaInterfaz;
                EviAyudaInterfaz.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                GameObject BtnIconoAyudaInterfaz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnIconoAyudaInterfaz);
                BtnIconoAyudaInterfaz.transform.SetParent(EviAyudaInterfaz.transform);
                BtnIconoAyudaInterfaz.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnIconoAyudaInterfaz;
                BtnIconoAyudaInterfaz.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnIconoAyudaInterfaz;
                BtnIconoAyudaInterfaz.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                TextMesh TextAyudaInterfazNombre = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoAyudaInterfazNombre);
                TextAyudaInterfazNombre.transform.SetParent(EviAyudaInterfaz.transform);
                TextAyudaInterfazNombre.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoAyudaInterfazNombre;
                TextAyudaInterfazNombre.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoAyudaInterfazNombre;
                TextAyudaInterfazNombre.text = descripcionHijosListaPrimerNivel[0][0];

                TextMesh descripcionAyudaInterfaz = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoAyudaInterfazDescripcion);
                descripcionAyudaInterfaz.transform.SetParent(EviAyudaInterfaz.transform);
                descripcionAyudaInterfaz.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoAyudaInterfazDescripcionOculto;
                descripcionAyudaInterfaz.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoAyudaInterfazDescripcionOculto;
                descripcionAyudaInterfaz.text = descripcionHijosListaPrimerNivel[0][1];

            } else {

                GameObject EviHijo1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Evi_Hijo1);
                EviHijo1.transform.SetParent(this.transform);
                EviHijo1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaEviHijo1;
                EviHijo1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionEviHijo1;
                EviHijo1.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                GameObject BtnIconoHijo1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnIconoHijo1);
                BtnIconoHijo1.transform.SetParent(EviHijo1.transform);
                BtnIconoHijo1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnIconoHijo1;
                BtnIconoHijo1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnIconoHijo1;
                BtnIconoHijo1.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                TextMesh TextHijo1Nombre = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1Nombre);
                TextHijo1Nombre.transform.SetParent(EviHijo1.transform);
                TextHijo1Nombre.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1Nombre;
                TextHijo1Nombre.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1Nombre;
                TextHijo1Nombre.text = descripcionHijosListaPrimerNivel[1][0];

                TextMesh descripcionHijo1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1Descripcion);
                descripcionHijo1.transform.SetParent(EviHijo1.transform);
                descripcionHijo1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1DescripcionOculto;
                descripcionHijo1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1DescripcionOculto;
                descripcionHijo1.text = descripcionHijosListaPrimerNivel[1][1];

                int numHijos = obtenerHijos(documentoXMLEvi, i);
                for (int j = 1; j <= numHijos; j++)
                {
                    if (j == 1) {                    
                        GameObject EviHijo1_1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Evi_Hijo1_1);
                        EviHijo1_1.transform.SetParent(EviHijo1.transform);
                        EviHijo1_1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaEviHijo1_1;
                        EviHijo1_1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionEviHijo1_1;
                        EviHijo1_1.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                        GameObject BtnIconoHijo1_1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnIconoHijo1_1);
                        BtnIconoHijo1_1.transform.SetParent(EviHijo1_1.transform);
                        BtnIconoHijo1_1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnIconoHijo1_1;
                        BtnIconoHijo1_1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnIconoHijo1_1;
                        BtnIconoHijo1_1.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                        TextMesh TextHijo1_1Nombre = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1_1Nombre);
                        TextHijo1_1Nombre.transform.SetParent(EviHijo1_1.transform);
                        TextHijo1_1Nombre.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1_1Nombre;
                        TextHijo1_1Nombre.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1_1Nombre;
                        TextHijo1_1Nombre.text = descripcionHijosLista[0][0];

                        TextMesh descripcionHijo1_1 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1_1Descripcion);
                        descripcionHijo1_1.transform.SetParent(EviHijo1_1.transform);
                        descripcionHijo1_1.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1_1DescripcionOculto;
                        descripcionHijo1_1.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1_1DescripcionOculto;
                        descripcionHijo1_1.text = descripcionHijosLista[0][1];

                        
                    } else if(j == 2) {
                        GameObject EviHijo1_2 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().Evi_Hijo1_2);
                        EviHijo1_2.transform.SetParent(EviHijo1.transform);
                        EviHijo1_2.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaEviHijo1_2;
                        EviHijo1_2.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionEviHijo1_2;
                        EviHijo1_2.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                        GameObject BtnIconoHijo1_2 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().BtnIconoHijo1_2);
                        BtnIconoHijo1_2.transform.SetParent(EviHijo1_2.transform);
                        BtnIconoHijo1_2.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaBtnIconoHijo1_2;
                        BtnIconoHijo1_2.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionBtnIconoHijo1_2;
                        BtnIconoHijo1_2.transform.rotation = Quaternion.Euler (0f, 180f, 0);

                        TextMesh TextHijo1_2Nombre = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1_2Nombre);
                        TextHijo1_2Nombre.transform.SetParent(EviHijo1_2.transform);
                        TextHijo1_2Nombre.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1_2Nombre;
                        TextHijo1_2Nombre.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1_2Nombre;
                        TextHijo1_2Nombre.text = descripcionHijosLista[1][0]; 

                        TextMesh descripcionHijo1_2 = Instantiate(ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().TextoHijo1_2Descripcion);
                        descripcionHijo1_2.transform.SetParent(EviHijo1_2.transform);
                        descripcionHijo1_2.transform.localPosition = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().escalaTextoHijo1_2DescripcionOculto;
                        descripcionHijo1_2.transform.localScale = ctrlInterfaz.GetComponent<ScriptDatosInterfaz>().positionTextoHijo1_2DescripcionOculto;
                        descripcionHijo1_2.text = descripcionHijosLista[1][1];

                    }
                }
            }
        }

* ****************  FIN Quitamos Hijos  ***********************************
* *********************************************************************** */


    }


        // Se tienen el número de hijos que hay en el primer nivel del KDL
    public int obtenerNumeroHijosPrimerNivel(XmlDocument documentoXMLEvi)
    {
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:D";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        return nodeList[0].ChildNodes.Count;
    }

    // Se obtiene si tiene hijos o no
    public bool tieneHijos(XmlDocument documentoXMLEvi, int i)
    {
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:D/kdl:E[" + i + "]/kdl:A/kdl:D";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        if (nodeList[0].ChildNodes.Count == 1) {

            return false;

        } else {

            return true;
        }
        
    }

    // Se tienen el número de hijos que hay en el KDL
    public int obtenerHijos(XmlDocument documentoXMLEvi, int i)
    {
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:D/kdl:E["+i+"]/kdl:A/kdl:D";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        return nodeList[0].ChildNodes.Count;
    }

    // Se obtiene la información en un array de uno de los hijos
    public string[] obtiene_Info_HijosPrimerNivel(XmlDocument documentoXMLEvi, int numHijoEnlace)
    {
        string[] descripcionHijos = new string [6];
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPathNombre = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[1]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathDescripcion = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[2]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathRotulo = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[3]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathIconoImagen = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[4]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathIconoAudio = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[5]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathImagenPrincipal = "/kdl:C/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[6]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeListNombre = root.SelectNodes(consultaxPathNombre, nsmgr);
        string Nombre = nodeListNombre[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListDescripcion = root.SelectNodes(consultaxPathDescripcion, nsmgr);
        string Descripcion = nodeListDescripcion[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListRotulo = root.SelectNodes(consultaxPathRotulo, nsmgr);
        string Rotulo = nodeListRotulo[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListIconoImagen = root.SelectNodes(consultaxPathIconoImagen, nsmgr);
        string IconoImagen = nodeListIconoImagen[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListIconoAudio = root.SelectNodes(consultaxPathIconoAudio, nsmgr);
        string IconoAudio = nodeListIconoAudio[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListImagenPrincipal = root.SelectNodes(consultaxPathImagenPrincipal, nsmgr);
        string ImagenPrincipal = nodeListImagenPrincipal[0].ChildNodes[0].InnerText;

        descripcionHijos[0] = Nombre;
        descripcionHijos[1] = Descripcion;
        descripcionHijos[2] = Rotulo;
        descripcionHijos[3] = IconoImagen;
        descripcionHijos[4] = IconoAudio;
        descripcionHijos[5] = ImagenPrincipal;

        return descripcionHijos;
    }

    
    public int obtenerHijosDescripcionPrimerNivel(XmlDocument documentoXMLEvi)
    {
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:D";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        return nodeList[0].ChildNodes.Count;
    }


    //LISTA DE FUNCIONES PARA LEER CONCEPTO DE UN DOCUMENTO XML Y PODER REPRESENTARLO EN UN EVI

    // Devuelve un array con los tres datos del identificador del concepto
    public string[] obtener_I_IdentificadorConcepto(XmlDocument documentoXMLEvi)
    {
        string[] identificadores = new string[3];
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:I";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        //foreach (XmlNode node in nodeList)
        //{
        //    identificadores[0] = node.ChildNodes[0].InnerText;
        //    identificadores[1] = node.ChildNodes[1].InnerText;
        //    identificadores[2] = node.ChildNodes[2].InnerText;
        //}

        identificadores[0] = nodeList[0].ChildNodes[0].InnerText;
        identificadores[1] = nodeList[0].ChildNodes[1].InnerText;
        identificadores[2] = nodeList[0].ChildNodes[2].InnerText;

        return identificadores;
    }

    // Devuelve un array con los dos datos del control de configuración
    public string[] obtener_F_ControlConfiguracionConcepto(XmlDocument documentoXMLEvi)
    {
        string[] identificadores = new string[3];
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:F";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            identificadores[0] = node.ChildNodes[0].InnerText;
            identificadores[1] = node.ChildNodes[1].InnerText;
        }

        return identificadores;
    }

    // Junta todos los datos de la ayuda interfaz (P) en un array que devuelve como parámetro
    public string[] obtener_P_AyudaInterfazConcepto(XmlDocument documentoXMLEvi)
    {
        string[] ayudaInterfaz = new string[6];

        ayudaInterfaz[0] = obtenerNombreAI(documentoXMLEvi);
        ayudaInterfaz[1] = obtenerDescripcionAI(documentoXMLEvi);
        ayudaInterfaz[2] = obtenerRotuloAI(documentoXMLEvi);
        ayudaInterfaz[3] = obtenerIconoImagenAI(documentoXMLEvi);
        ayudaInterfaz[4] = obtenerIconoAudioAI(documentoXMLEvi);
        ayudaInterfaz[5] = obtenerImagenConceptoAI(documentoXMLEvi);

        return ayudaInterfaz;
    }

    public string obtenerNombreAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[1]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        resultado = nodeList[0].ChildNodes[0].InnerText;
        
        return resultado;
    }

    public string obtenerDescripcionAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[2]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            resultado = node.ChildNodes[0].InnerText;
        }

        return resultado;
    }

    public string obtenerRotuloAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[3]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            resultado = node.ChildNodes[0].InnerText;
        }

        return resultado;
    }

    public string obtenerIconoImagenAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[4]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            resultado = node.ChildNodes[0].InnerText;
        }

        return resultado;
    }

    public string obtenerIconoAudioAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[5]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            resultado = node.ChildNodes[0].InnerText;
        }

        return resultado;
    }

    public string obtenerImagenConceptoAI(XmlDocument documentoXMLEvi)
    {
        string resultado = "";
        // parte necesaria si queremos utilizar xPath
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:P/kdl:E[6]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        foreach (XmlNode node in nodeList)
        {
            resultado = node.ChildNodes[0].InnerText;
        }

        return resultado;
    }

    // Se tienen el número de hijos que hay en el KDL
    public int obtenerHijosDescripcion(XmlDocument documentoXMLEvi)
    {
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPath = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D";

        XmlNodeList nodeList = root.SelectNodes(consultaxPath, nsmgr);

        return nodeList[0].ChildNodes.Count;
    }

    // Se obtiene la información en un array de uno de los hijos
    public string[] obtiene_D_DescripcionHijos(XmlDocument documentoXMLEvi, int numHijoEnlace)
    {
        string[] descripcionHijos = new string [6];
        XmlElement root = documentoXMLEvi.DocumentElement;

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentoXMLEvi.NameTable);
        nsmgr.AddNamespace("kdl", "http://kdlnamespace.ideando.net");

        string consultaxPathNombre = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[1]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathDescripcion = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[2]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathRotulo = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[3]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathIconoImagen = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[4]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathIconoAudio = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[5]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";
        string consultaxPathImagenPrincipal = "/kdl:C/kdl:D/kdl:E[2]/kdl:A/kdl:D/kdl:E[" + numHijoEnlace + "]/kdl:A/kdl:P/kdl:E[6]/kdl:A/kdl:D/kdl:E/kdl:Z/kdl:T";

        XmlNodeList nodeListNombre = root.SelectNodes(consultaxPathNombre, nsmgr);
        string Nombre = nodeListNombre[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListDescripcion = root.SelectNodes(consultaxPathDescripcion, nsmgr);
        string Descripcion = nodeListDescripcion[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListRotulo = root.SelectNodes(consultaxPathRotulo, nsmgr);
        string Rotulo = nodeListRotulo[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListIconoImagen = root.SelectNodes(consultaxPathIconoImagen, nsmgr);
        string IconoImagen = nodeListIconoImagen[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListIconoAudio = root.SelectNodes(consultaxPathIconoAudio, nsmgr);
        string IconoAudio = nodeListIconoAudio[0].ChildNodes[0].InnerText;

        XmlNodeList nodeListImagenPrincipal = root.SelectNodes(consultaxPathImagenPrincipal, nsmgr);
        string ImagenPrincipal = nodeListImagenPrincipal[0].ChildNodes[0].InnerText;

        descripcionHijos[0] = Nombre;
        descripcionHijos[1] = Descripcion;
        descripcionHijos[2] = Rotulo;
        descripcionHijos[3] = IconoImagen;
        descripcionHijos[4] = IconoAudio;
        descripcionHijos[5] = ImagenPrincipal;

        return descripcionHijos;
    }

}