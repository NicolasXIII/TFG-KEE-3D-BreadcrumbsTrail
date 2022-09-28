using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using System;

/// //////////////////////////////////////////////////////////////////////////////////////  nombre_enKee
/// ///////////  ScriptLibConceptosXml = Script que contiene los recursos para procesar los XML de conceptos en general
// Autor : 	Miguel Angel Fernandez Graciani
// Fecha :	2021-01-21
// Observaciones :
// 		Tomo varias funciones de los ficheros de :  
//          - "KEE3D/ChatarraElementos/lib_Kee"   Que procede de el DKS de desarrollo web inicial
// 		    -  "KEE3D/ChatarraElementos/SC_DAUS_2.cs"   Que procede de el TFG de Pau
//          - 


public class ScriptLibConceptosXml : MonoBehaviour {


    // ///////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////
    //      CONSTANTES de cadena

    public static string prefijoDnsKdl = "kdl";
    public static string prefijoDnsKdlConDosPuntos = "kdl:";

    public static string DnsKdl = "http://kdlnamespace.ideando.net";
    public static string BaseConceptoKDL = "<?xml version='1.0' encoding='UTF-8'?><kdl:C xmlns:kdl='http://kdlnamespace.ideando.net'></kdl:C>";

    // ///////////////////////////////////////////////////////////////////////
    // constantes de KLW para construir y analizar los KDL
    // Datos del DKS
    string gen_nombre = "gen_nombre";  //  en host dksGenerico
    string gen_descripcion = "gen_descripcion";  //  en host dksGenerico
    string gen_rotulo = "gen_rotulo";  //  en host dksGenerico
    string gen_iconoImagenConcepto = "gen_iconoImagenConcepto";  //  en host dksKlw
    string gen_iconoAudioConcepto = "gen_iconoAudioConcepto";  //  en host dksKlw
    string gen_imagenConcepto = "gen_imagenConcepto";  //  en host dksKlw


    // ///////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////
    //      CADENAS de xmlPath para busqueda

    // ///////////////////////////////////////////////////////////////////////
    //      CONSTANTES de cadena ; etiquetas KDL

    public static string letra_Concepto = "C";  // Para nodos de raiz de CONCEPTO
    public static string xPathString_para_C = "/kdl:C";  // Para nodos de raiz de CONCEPTO
    public static string xPathString_name_C = "kdl:C";  // Para nodos de raiz de CONCEPTO

    public static string letra_Identificador = "I";  // Para Identificador
    public static string xPathString_para_I = "/kdl:I";  // Para nodos IDENTIFICADOR
    public static string xPathString_name_I = "kdl:I";  // Para nodos IDENTIFICADOR
        public static string letra_Key = "K";  // Para Key
        public static string xPathString_para_K = "/kdl:K";  // Para nodos KEY
        public static string xPathString_name_K = "kdl:K";  // Para nodos KEY
        public static string letra_Host = "H";  // Para host
        public static string xPathString_para_H = "/kdl:H";  // Para nodos HOST
        public static string xPathString_name_H = "kdl:H";  // Para nodos HOST
        public static string letra_Cualificador = "Q";  // Para cualificado
        public static string xPathString_para_Q = "/kdl:Q";  // Para nodos CUALIFICADOR
        public static string xPathString_name_Q = "kdl:Q";  // Para nodos CUALIFICADOR

    public static string letra_CtrlConf = "F";  // Para control de configuracion
    public static string xPathString_para_F = "/kdl:F";  // Para nodos CONTROL DE CONFIGURACION
    public static string xPathString_name_F = "kdl:F";  // Para nodos CONTROL DE CONFIGURACION
        public static string letra_Ordinal = "O";  // Para Ordinal
        public static string xPathString_para_O = "/kdl:O";  // Para nodos ORDINAL de version
        public static string xPathString_name_O = "kdl:O";  // Para nodos ORDINAL de version
        public static string letra_UltModf = "M";  // Para Ultima modificacion
        public static string xPathString_para_M = "/kdl:M";  // Para nodos ULTIMA MODIFICACION
        public static string xPathString_name_M = "kdl:M";  // Para nodos ULTIMA MODIFICACION

    public static string letra_AyuInterfaz = "P";  // Para AyuInterfaz
    public static string xPathString_para_P = "/kdl:P";  // Para nodos de AYUDA A INTERFAZ
    public static string xPathString_name_P = "kdl:P";  // Para nodos de AYUDA A INTERFAZ
        public static string letra_Lenguaje = "L";  // Para Lenguaje
        public static string xPathString_para_L = "/kdl:L";  // Para nodos de LENGUAJE
        public static string xPathString_name_L = "kdl:L";  // Para nodos de LENGUAJE

    public static string letra_Descripcion = "D";  // Para Descripcion
    public static string xPathString_para_D = "/kdl:D";  // Para nodos DESCRIPCION
    public static string xPathString_name_D = "kdl:D";  // Para nodos DESCRIPCION
        public static string letra_Enlace = "E";  // Para Enlace
        public static string xPathString_para_E = "/kdl:E";  // Para nodos ENLACE
        public static string xPathString_name_E = "kdl:E";  // Para nodos ENLACE
            public static string letra_Instancia = "A";  // Para Instancia
            public static string xPathString_para_A = "/kdl:A";  // Para nodos IMSTANCIA
            public static string xPathString_name_A = "kdl:A";  // Para nodos IMSTANCIA
            public static string letra_Referencia = "R";  // Para Referencia
            public static string xPathString_para_R = "/kdl:R";  // Para nodos REFERENCIA
            public static string xPathString_name_R = "kdl:R";  // Para nodos REFERENCIA
            public static string letra_SinTecho = "Z";  // Para SinTecho
            public static string xPathString_para_Z = "/kdl:Z";  // Para nodos SIN TECHO
            public static string xPathString_name_Z = "kdl:Z";  // Para nodos SIN TECHO
                public static string letra_Texto = "T";  // Para Texto
                public static string xPathString_para_T = "/kdl:T";  // Para nodos TEXTO SIN TECHO
                public static string xPathString_name_T = "kdl:T";  // Para nodos TEXTO SIN TECHO

    // ///////////////////////////////////////////////////////////////////////
    //      CONSTANTES de cadena ; Cadenas XPath especificas

    // Para el enlace que contiene el nombre, buscamos un hijo E (enlace) de P (ayuda a interfaz) que instancie el concepto (Key=gen_nombre y host=http://www.ideando.net/klw/dks_Generic) 
    
//    string xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I/kdl:H[text()='http://www.ideando.net/klw/dks_Generic']";  // Lo defino es estar porque se define refiriendo otros string que tienen que estar inicializados  
    string xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I/kdl:H[text()='http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic']";  // Lo defino es estar porque se define refiriendo otros string que tienen que estar inicializados  

    string xPathString_para_id_Instancia = "/kdl:E/kdl:A/kdl:I";

    string xPathString_para_texto_de_sinTecho = ".//kdl:T";  // Este epara pedir el T que hay en el elemento en que estamos


    
    // Use this for initialization
    void Start () {

        // /////////////////////////////////////////////////////////////////////////////
        // Definimos los string estaticos necesarios para construir los ficheros de concepto XML

        //        xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I/kdl:K[text()='gen_nombre']";

        //        xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I[/kdl:K[text()='gen_nombre'] and /kdl:H[text()='http://www.ideando.net/klw/dks_Generic']]";
        //        xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I/kdl:H[text()='http://www.ideando.net/klw/dks_Generic']";
        //        xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I[/kdl:K[text()='gen_nombre'] and /kdl:H[text()='http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic']]";
        //        xPathString_para_nombre = "./kdl:E/kdl:A/kdl:I/kdl:H[text()='http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic']";

        //button[text()="Submit"]
    }  // Fin de - void Start () {

    // Update is called once per frame
    void Update () {

    }  // Fin de -   void Update()

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Aqui la libreria de métodos que sirven para procesar los conceptos XML
    // ///////////////////////////////////////////////////////////////////////////////////////////////

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos basicos para procesar los conceptos XML

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos basicos para OBTENER INFORMACION DEL CONCEPTO

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///  dameTipoDeEnlace :  Metodos que devuelve el tipo de enlace que nos envian. Puede ser 
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-04-17
    /// Parametros de entrada :
    ///      - domKDL = Dom del concepto general
    ///      - nodoElemento = Nodo del que debemos indicar el tipo (E, R o Z)
    /// Retorna : string [key, host, cualificador]
    /// Observaciones :
    ///      Nos deben haber enviado un nodo enlace "E", debemos contestar si es instancia "A", referencia "R" o sin techo "Z". Si no es asi, devolvemos un error
    /// <summary>
    public string dameTipoDeEnlace(XmlDocument domKDL, XmlNode nodoElemento)
    {
        // Declaramos variables
        string tipoDeEnlace;

        // Controlamos que el nodo que nos han pasado es en enlace "E"          , instancia "A" o referencia "R". Si no es asi, devolvemos un error
        bool esAdecuado = nodoElemento.Name == prefijoDnsKdlConDosPuntos + letra_Enlace;
        if (!esAdecuado)  // Si lo que nos mandan no es un nodo de enlace "E", devolvemos un error
        {
            tipoDeEnlace = "Error desde ScriptLibConceptosXml => dameTipoDeEnlace. No es un nodo enlace. El nombre del nodo recibido es " + nodoElemento.Name;
            return tipoDeEnlace;
        }

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
        manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

        // El primer hijo del nodo enlace debe ser siempre el nodo tipo del enlace 
        tipoDeEnlace = nodoElemento.FirstChild.Name;

        bool esTipoDeEnlace = tipoDeEnlace == prefijoDnsKdlConDosPuntos + letra_Instancia | tipoDeEnlace == prefijoDnsKdlConDosPuntos + letra_Referencia | tipoDeEnlace == prefijoDnsKdlConDosPuntos + letra_SinTecho;

        if (!esTipoDeEnlace) { tipoDeEnlace = "Error desde ScriptLibConceptosXml => dameTipoDeEnlace. No es un tipo de enlace. El nombre del tipo de enlace recibido es " + tipoDeEnlace; }

        return tipoDeEnlace;
    } // Fin de -  public string dameTipoDeEnlace(XmlDocument domKDL, XmlNode nodoElemento)



    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que obtiene el identificador de un conceto (key K, host H y cualificador Q (efimero,...)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-01-31
    /// Parametros de entrada :
    ///      - domKDL = Dom del concepto general
    ///      - nodoElemento = Nodo del que extraemos la informacion
    /// Retorna : string [key, host, cualificador]
    /// Observaciones :
    ///      Nos pueden pasar un nodo de uno de estos tipos
    ///          - Nodo concepto "C" (concepto)
    ///          - Nodo instancia "A" (Instancia)
    ///          - Nodo referencia "R" (referencia)
    ///          - Nodo referencia "Z" (sin techo : en este casso devolvemos el identificador del tipo de dato que esta en la referencia R que contiene)
    /// </summary>

    public string[] dameIdentificadorDeNodo(XmlDocument domKDL, XmlNode nodoElemento)
   {
        // Declaramos variables
        string[] identificadorConcepto_K_H_Q = new string[3];

        XmlNode KDL_Nodo_I;
        XmlNode KDL_Nodo_K;
        XmlNode KDL_Nodo_H;
        XmlNode KDL_Nodo_Q;

        try
        {

            // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A", referencia "R" o sin techo "Z". Si no es asi, devolvemos un error

            bool esAdecuado = nodoElemento.Name == prefijoDnsKdlConDosPuntos + "C" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "A" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "R" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "Z";
            if (!esAdecuado)
            {
                identificadorConcepto_K_H_Q[0] = "0";  // Key = 0 indica error en el key
                identificadorConcepto_K_H_Q[1] = "Error al obtener host, no es un nodo adecuado, Con nodoElemento.Name = " + nodoElemento.Name;
                identificadorConcepto_K_H_Q[2] = "Error al obtener cualificador, no es un nodo adecuado";

                return identificadorConcepto_K_H_Q;
            }


            // Si es un nodo Z, lo que devolvemos es el identificador del tipo de dato, esto es el nodo referencia "R" que hay dentro 
            if (nodoElemento.Name == prefijoDnsKdlConDosPuntos + "Z")
            {
                nodoElemento = nodoElemento.FirstChild;  // Indicamos que queremos el identificador del tipo de dato que esta en "R" dentro de "Z"
            }

            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("dameIdentificadorDeNodo con domKDL = " + domKDL.ToString()); }

            //Declaracion del Espacio de Nombres, necesario para xPath
            XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
            manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

            // Obtenemos el elemento identificador del nodo
            string aqui_xPathString_para_I = "." + xPathString_para_I;
            KDL_Nodo_I = nodoElemento.SelectSingleNode(aqui_xPathString_para_I, manejadorEspNomb);
            // Obtenemos los datos de identificacion del concepto y los vamos asignado al array
            string aqui_xPathString_para_K = "." + xPathString_para_K;
            KDL_Nodo_K = KDL_Nodo_I.SelectSingleNode(aqui_xPathString_para_K, manejadorEspNomb);
            identificadorConcepto_K_H_Q[0] = KDL_Nodo_K.InnerText;

            string aqui_xPathString_para_H = "." + xPathString_para_H;
            KDL_Nodo_H = KDL_Nodo_I.SelectSingleNode(aqui_xPathString_para_H, manejadorEspNomb);
            identificadorConcepto_K_H_Q[1] = KDL_Nodo_H.InnerText;

            string aqui_xPathString_para_Q = "." + xPathString_para_Q;
            KDL_Nodo_Q = KDL_Nodo_I.SelectSingleNode(aqui_xPathString_para_Q, manejadorEspNomb);
            identificadorConcepto_K_H_Q[2] = KDL_Nodo_Q.InnerText;

            return identificadorConcepto_K_H_Q;
        }  // FIn de -  try
        catch
        {
            identificadorConcepto_K_H_Q[0] = "0";
            identificadorConcepto_K_H_Q[1] = "Error al obtener host";
            identificadorConcepto_K_H_Q[2] = "Error al obtener cualificador";

            return identificadorConcepto_K_H_Q;
        }
    } // Fin de -  public string[] dameIdentificadorDeNodo(XmlDocument domKDL, XmlNode nodoElemento)

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos que obtiene los datos de control de configuracion de un conceto (ordinal O, fecha de ultima modificacion M)
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-02-01
    // Parametros de entrada :
    //      - domKDL = Nodo del que extraemos la informacion
    //      - nodoElemento = Nodo del que extraemos la informacion
    // Retorna : string [ordinal, fechaUltmaModificacion]
    // Observaciones :
    //      Nos pueden pasar un nodo de uno de estos tipos
    //          - Nodo concepto "C"
    //          - Nodo instancia "A"
    //          - Nodo referencia "R"
    public string[] dameCtrlConfDeNodo(XmlDocument domKDL, XmlNode nodoElemento)
    {
        // Declaramos variables
        string[] ctrlCof_O_M = new string[2];

        XmlNode KDL_Nodo_F;
        XmlNode KDL_Nodo_O;
        XmlNode KDL_Nodo_M;

        // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R". Si no es asi, devolvemos un error
        try
        {
            bool esAdecuado = nodoElemento.Name == prefijoDnsKdlConDosPuntos + "C" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "A" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "R";
            if (!esAdecuado)
            {
                ctrlCof_O_M[0] = "Error al obtener Ordinal, no es un nodo adecuado";
                ctrlCof_O_M[1] = "Error al obtener ultima modificacion, no es un nodo adecuado";

                return ctrlCof_O_M;
            }

            //Declaracion del Espacio de Nombres, necesario para xPath
            XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
            manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

            // Obtenemos el elemento identificador del nodo
            string aqui_xPathString_para_F = "." + xPathString_para_F;
            KDL_Nodo_F = nodoElemento.SelectSingleNode(aqui_xPathString_para_F, manejadorEspNomb);
            // Obtenemos los datos de identificacion del concepto y los vamos asignado al array
            string aqui_xPathString_para_O = "." + xPathString_para_O;
            KDL_Nodo_O = KDL_Nodo_F.SelectSingleNode(aqui_xPathString_para_O, manejadorEspNomb);
            ctrlCof_O_M[0] = KDL_Nodo_O.InnerText;

            string aqui_xPathString_para_M = "." + xPathString_para_M;
            KDL_Nodo_M = KDL_Nodo_F.SelectSingleNode(aqui_xPathString_para_M, manejadorEspNomb);
            ctrlCof_O_M[1] = KDL_Nodo_M.InnerText;

            return ctrlCof_O_M;
        }
        catch
        {
            ctrlCof_O_M[0] = "Error al obtener Ordinal";
            ctrlCof_O_M[1] = "Error al obtener ultima modificacion";

            return ctrlCof_O_M;
        }
    } // Fin de -  public string[] dameCtrlConfDeNodo(XmlDocument domKDL, XmlNode nodoElemento)

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos que obtiene los datos de cayuda a interfaz de un conceto (definidos abajo)
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-02-01
    // Parametros de entrada :
    //      - domKDL = Nodo del que extraemos la informacion
    //      - nodoElemento = Nodo del que extraemos la informacion
    // Retorna : string []
    //                  idioma_AyuIntf;               // idioma_AyuIntf : es el idioma de ayuda a interfaz propia del concepto  (esta en L en KDL)
    //                  txt_nombre_AyuIntf;          // txt_nombre_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
    //                  txt_descripcion_AyuIntf;     // txt_descripcion_concepto : es el nombre de ayuda a interfaz del concepto  (esta en P en KDL)
    //                  txt_rotulo_AyuIntf;          // txt_rotulo_concepto : es el rotulo de ayuda a interfaz del concepto  (esta en P en KDL)
    //                  icono_AyuIntf;               // icono_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    //                  audio_AyuIntf;               // audio_concepto : es el icono de ayuda a interfaz del concepto  (esta en P en KDL)
    //                  imagen_AyuIntf;               // imagen_AyuIntf : es la imagen de ayuda a interfaz propia del concepto  (esta en P en KDL)
    // Observaciones :
    //      Nos pueden pasar un nodo de uno de estos tipos
    //          - Nodo concepto "C"
    //          - Nodo instancia "A"
    //          - Nodo referencia "R"
    public string[] dameAyudaIntDeNodo(XmlDocument domKDL, XmlNode nodoElemento)
    {
        // Declaramos variables
        string[] ayudaInt = new string[7];

        XmlNode KDL_Nodo_P;
        XmlNode KDL_Nodo_L;

        XmlNode KDL_Nodo_E_nombre;
        XmlNode KDL_Nodo_E_descripcion;
        XmlNode KDL_Nodo_E_rotulo;
        XmlNode KDL_Nodo_E_iconoImagenConcepto;
        XmlNode KDL_Nodo_E_iconoAudioConcepto;
        XmlNode KDL_Nodo_E_imagenConcepto;

        // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R". Si no es asi, devolvemos un error
        bool esAdecuado = nodoElemento.Name == prefijoDnsKdlConDosPuntos + "C" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "A" | nodoElemento.Name == prefijoDnsKdlConDosPuntos + "R";
        if (!esAdecuado)
        {
            ayudaInt[0] = "Idioma por defecto";
            ayudaInt[1] = "Nombre por defecto";
            ayudaInt[2] = "Descripcion : ";
            ayudaInt[3] = "Rotulo por defecto";
            ayudaInt[4] = "Icono-imagen por defecto";
            ayudaInt[5] = "Icono-audio por defecto";
            ayudaInt[6] = "Imagen por defecto";

            return ayudaInt;
        }

        //Declaracion del Espacio de Nombres, necesario para xPath
        XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
        manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

        // Obtenemos el elemento P del nodo
        string aqui_xPathString_para_P = "." + xPathString_para_P;
        KDL_Nodo_P = nodoElemento.SelectSingleNode(aqui_xPathString_para_P, manejadorEspNomb);

        // PENDIENTE MAFG 2021-05-16. Si el elemento no tiene ayuda a interfaz (no tiene "P") lo que habria que hacer es pedirselo al DKS 
        // correspondiente y ponerlo aqui. Por ahora, el tipo de datos que viene en las referencias "R" de los elementos sin techo "Z", no
        // traen ayuda a interfaz "P", por lo que seria bueno implementar la funcionalidad que esta pendiente. Anque tambien seria adecuado 
        // hacer que los DKS enviaran la "P" de las referencias "R" de los tipos de dato del os elementos sin techo "Z"

        // Obtenemos los datos de ayuda a interfaz del concepto y los vamos asignado al array
        try
        {
            string aqui_xPathString_para_L = "." + xPathString_para_L;
            KDL_Nodo_L = KDL_Nodo_P.SelectSingleNode(aqui_xPathString_para_L, manejadorEspNomb);
            ayudaInt[0] = KDL_Nodo_L.InnerText;
        }
        catch
        {
            ayudaInt[0] = "Error en idioma";
            ayudaInt[2] = ayudaInt[2]+"Idioma: Error al buscar el idioma en ayuda a interfaz. El enlace es erroneo";
        }

        // Los siguientes datos no tienen elemento especifico, hay que buscarlos como instancias de cada elemento
        // por lo que el acceso es algo distinto. Ver la definicion del xPathString_para_*** de cada uno
        List<XmlNode> ListaNodosQueInstancian = new List<XmlNode>();
            // Vamos con "gen_nombre"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_nombre, ScriptConexionDKS.dksGenerico, manejadorEspNomb);  // Seleccionamos las instancias de nombre

        if (ListaNodosQueInstancian.Count == 1 )
        {
            try
            {
                KDL_Nodo_E_nombre = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene el nombre
                string KDL_String_nombre = KDL_Nodo_E_nombre.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[1] = KDL_String_nombre;
            }
            catch
            {
                ayudaInt[1] = "Error en nombre";
                ayudaInt[2] = ayudaInt[2] + "Nombre: Error al buscar el nombre en ayuda a interfaz. El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[1] = "Error en nombre";
            ayudaInt[2] = ayudaInt[2] + "Nombre: Error al buscar el nombre en ayuda a interfaz. Sin enlace";
        }
        // Vamos con "gen_descripcion"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_descripcion, ScriptConexionDKS.dksGenerico, manejadorEspNomb);  // Seleccionamos las instancias de nombre

        if (ListaNodosQueInstancian.Count == 1)
        {
            try
            {
                KDL_Nodo_E_descripcion = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene la descripcion
                string KDL_String_descripcion = KDL_Nodo_E_descripcion.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[2] = KDL_String_descripcion;
            }
            catch
            {
                ayudaInt[2] = "Error en descripcion";
                ayudaInt[2] = ayudaInt[2] + "Descripcion: Error al buscar la descripcion en ayuda a interfaz. El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[2] = "Error en descripcion";
            ayudaInt[2] = ayudaInt[2] + "Descripcion: Error al buscar la descripcion en ayuda a interfaz. Sin enlace";
        }
        // Vamos con "gen_rotulo"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_rotulo, ScriptConexionDKS.dksGenerico, manejadorEspNomb);  // Seleccionamos las instancias de nombre
        if (ListaNodosQueInstancian.Count == 1)
        {
            try
            {
                KDL_Nodo_E_rotulo = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene el rotulo
                string KDL_String_rotulo = KDL_Nodo_E_rotulo.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[3] = KDL_String_rotulo;
            }
            catch
            {
                ayudaInt[3] = "Sin rotulo";
                ayudaInt[2] = ayudaInt[2] + "Rotulo: Error al buscar el rotulo en ayuda a interfaz. El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[3] = "Sin rotulo";
            ayudaInt[2] = ayudaInt[2] + "Rotulo: Error al buscar el rotulo en ayuda a interfaz. Sin rotulo";
        }
        // Vamos con "gen_iconoImagenConcepto"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_iconoImagenConcepto, ScriptConexionDKS.dksKlw, manejadorEspNomb);  // Seleccionamos las instancias de nombre
        if (ListaNodosQueInstancian.Count == 1)
        {
            try
            {
                KDL_Nodo_E_iconoImagenConcepto = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene el nombre del icono de imagen de ayuda a interfaz del concepto
                string KDL_String_iconoImagenConcepto = KDL_Nodo_E_iconoImagenConcepto.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[4] = KDL_String_iconoImagenConcepto;
            }
            catch
            {
                ayudaInt[4] = "iconoImgPorDefecto.png";
                ayudaInt[2] = ayudaInt[2] + "Icono: Error al buscar el icono en ayuda a interfaz.  El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[4] = "iconoImgPorDefecto.png";
            ayudaInt[2] = ayudaInt[2] + "Icono: Error al buscar el icono en ayuda a interfaz. Sin icono";
        }
        // Vamos con "gen_iconoAudioConcepto"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_iconoAudioConcepto, ScriptConexionDKS.dksKlw, manejadorEspNomb);  // Seleccionamos las instancias de nombre
        if (ListaNodosQueInstancian.Count == 1)
        {
            try
            {
                KDL_Nodo_E_iconoAudioConcepto = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene el nombre del icono de audio de ayuda a interfaz del concepto
                string KDL_String_iconoAudioConcepto = KDL_Nodo_E_iconoAudioConcepto.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[5] = KDL_String_iconoAudioConcepto;
            }
            catch
            {
                ayudaInt[5] = "IconoAudioPorDefecto.wav";
                ayudaInt[2] = ayudaInt[2] + "Icono-audio: Error al buscar el icono-audio en ayuda a interfaz.  El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[5] = "IconoAudioPorDefecto.wav";
            ayudaInt[2] = ayudaInt[2] + "Icono-audio: Error al buscar el icono-audio en ayuda a interfaz.  Sin icono de audio";
        }
        // Vamos con "gen_imagenConcepto"
        ListaNodosQueInstancian = dameHijos_N1_QueInstancianA(KDL_Nodo_P, gen_imagenConcepto, ScriptConexionDKS.dksKlw, manejadorEspNomb);  // Seleccionamos las instancias de nombre
        if (ListaNodosQueInstancian.Count == 1)
        {
            try
            {
                KDL_Nodo_E_imagenConcepto = ListaNodosQueInstancian[0];  // Seleccionamos el enlace que contiene el nombre de la imagen de ayuda a interfaz del concepto
                string KDL_String_imagenConcepto = KDL_Nodo_E_imagenConcepto.SelectSingleNode(xPathString_para_texto_de_sinTecho, manejadorEspNomb).InnerText;
                ayudaInt[6] = KDL_String_imagenConcepto;
            }
            catch
            {
                ayudaInt[6] = "sinFoto.jpg";
                ayudaInt[2] = ayudaInt[2] + "Imagen: Error al buscar la imagen en ayuda a interfaz.  El enlace es erroneo";
            }
        }
        else
        {
            ayudaInt[6] = "sinFoto.jpg";
            ayudaInt[2] = ayudaInt[2] + "Imagen: Error al buscar la imagen en ayuda a interfaz. Sin imagen";
        }

        return ayudaInt;
    } // Fin de -  public string[] dameAyudaIntDeNodo(XmlDocument domKDL, XmlNode nodoElemento)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que obtiene el dato en texto (T) que se almacena en un elemento sin techo (Z)
    ///   Autor : 	Miguel Angel Fernandez Graciani
    ///   Fecha :	2021-04-27
    ///   Parametros de entrada :
    ///      - domKDL = Nodo del que extraemos la informacion
    ///      - nodoElemento = Nodo del que extraemos la informacion
    ///   Retorna : 
    ///      - string DatoSinTecho : Es string que configura el contenido del dato sin techo
    ///   Observaciones :
    ///      Solo nos deben pasar un nodo de dato de Sin Techo - Nodo Sin Techo  "Z". Aqui accedemos al nodo de dato (T) que contiene y
    ///      le pasamos el string que es el dato del sin techo
    ///      
    /// </summary>

    public string dameDatoSinTecho(XmlDocument domKDL, XmlNode nodo_Z_DelFractum)
    {
        // Declaramos variables
        string DatoSinTecho = "Sin techo por defecto.";

        XmlNode KDL_Nodo_T;

        // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R". Si no es asi, devolvemos un error
        try
        {
            bool esAdecuado = nodo_Z_DelFractum.Name == prefijoDnsKdlConDosPuntos + "Z";
            if (!esAdecuado)
            {
                DatoSinTecho = "Error en dameDatoSinTecho - No es un elemento adecuado T. Nomnre del elemento recibido : " + nodo_Z_DelFractum.Name;

                return DatoSinTecho;
            }

            //Declaracion del Espacio de Nombres, necesario para xPath
            XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
            manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

            // Obtenemos el dato SIn Techo
            string aqui_xPathString_para_T = "." + xPathString_para_T;
            KDL_Nodo_T = nodo_Z_DelFractum.SelectSingleNode(aqui_xPathString_para_T, manejadorEspNomb);
            DatoSinTecho = KDL_Nodo_T.InnerText;

            return DatoSinTecho;

        }  // Fin de - try
        catch
        {
            DatoSinTecho = "Error en dameDatoSinTecho al acceder al elemento T. Nomnre del elemento recibido : " + nodo_Z_DelFractum.Name;

            return DatoSinTecho;
        }
    } // Fin de -  public string dameDatoSinTecho(XmlDocument domKDL, XmlNode nodo_Z_DelFractum)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que obtiene el tipo de dato que aparece en la referencia (R) que se almacena en un elemento sin techo (Z)
    ///   Autor : 	Miguel Angel Fernandez Graciani
    ///   Fecha :	2021-04-27
    ///   Parametros de entrada :
    ///      - domKDL = Nodo del que extraemos la informacion
    ///      - nodoElemento = Nodo del que extraemos la informacion
    ///   Retorna : 
    ///      - string[] tipoDeDatoSinTecho : Es string array de estring con los datos que siguen:
    ///         tipoDeDatoSinTecho [0] => elemento k (de I=>K)
    ///         tipoDeDatoSinTecho [1] => elemento H (de I=>H)
    ///         tipoDeDatoSinTecho [2] => elemento Q (de I=>Q)
    ///         tipoDeDatoSinTecho [3] => elemento O (de F=>M)
    ///         tipoDeDatoSinTecho [4] => elemento M (de F=>M)
    ///   Observaciones :
    ///      Solo nos deben pasar un nodo de dato de Sin Techo - Nodo Sin Techo  "Z". Aqui accedemos al nodo de tipo de dato (R) que contiene y
    ///      le pasamos el array de string con la informacion del tipo de dato del sin techo
    ///      - La estructura de los sin techo es :
    ///                                 => Z (sin techo)
    ///                                     => R (referencia al tipo de dato sin techo : texto, numero, url, fecha, etc...)
    ///                                        => I (identificador)
    ///                                        => F (control de configuracion)
    ///                                        => P (Ayuda a interfaz) (opcional)
    ///                                     => T (dato sin techo, string de texto)
    ///      
    /// </summary>

    public string[] dameTipoDeDatoSinTecho(XmlDocument domKDL, XmlNode nodo_Z_DelFractum)
    {
        // Declaramos variables
        string[] tipoDeDatoSinTecho = new string[5];

        XmlNode KDL_Nodo_R;
        XmlNode KDL_Nodo_I;
        XmlNode KDL_Nodo_F;

        // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R". Si no es asi, devolvemos un error
        try
        {
            bool esAdecuado = nodo_Z_DelFractum.Name == prefijoDnsKdlConDosPuntos + "Z";
            if (!esAdecuado)  // SI no nos envian un elemento de tipo Z, devolvemos un error en tipo de dato
            {
                tipoDeDatoSinTecho [0] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_Key; //=> elemento k (de I=>K)
                tipoDeDatoSinTecho [1] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_host; //=> elemento H (de I=>H)
                tipoDeDatoSinTecho [2] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_cualifi; //=> elemento Q (de I=>Q)
                tipoDeDatoSinTecho [3] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_ordinal; //=> elemento O (de F=>M)
                tipoDeDatoSinTecho [4] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_ultiMod; //=> elemento M (de F=>M)

                return tipoDeDatoSinTecho;
            }

            //Declaracion del Espacio de Nombres, necesario para xPath
            XmlNamespaceManager manejadorEspNomb = new XmlNamespaceManager(domKDL.NameTable);
            manejadorEspNomb.AddNamespace(prefijoDnsKdl, DnsKdl);

            // Obtenemos el dato SIn Techo
            string aqui_xPathString_para_R = "." + xPathString_para_R;
            KDL_Nodo_R = nodo_Z_DelFractum.SelectSingleNode(aqui_xPathString_para_R, manejadorEspNomb);
            KDL_Nodo_I = KDL_Nodo_R.ChildNodes[0];
            tipoDeDatoSinTecho[0] = KDL_Nodo_I.ChildNodes[0].InnerText;   //=> elemento k (de I=>K)
            tipoDeDatoSinTecho[1] = KDL_Nodo_I.ChildNodes[1].InnerText;   //=> elemento H (de I=>H)
            tipoDeDatoSinTecho[2] = KDL_Nodo_I.ChildNodes[2].InnerText;   //=> elemento Q (de I=>Q)

            KDL_Nodo_F = KDL_Nodo_R.ChildNodes[1];
            tipoDeDatoSinTecho[3] = KDL_Nodo_F.ChildNodes[0].InnerText; ; //=> elemento O (de F=>M)
            tipoDeDatoSinTecho[4] = KDL_Nodo_F.ChildNodes[1].InnerText; //=> elemento M (de F=>M)

            return tipoDeDatoSinTecho;

        }  // Fin de - try
        catch
        {
            tipoDeDatoSinTecho[0] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_Key; //=> elemento k (de I=>K)
            tipoDeDatoSinTecho[1] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_host; //=> elemento H (de I=>H)
            tipoDeDatoSinTecho[2] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_cualifi; //=> elemento Q (de I=>Q)
            tipoDeDatoSinTecho[3] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_ordinal; //=> elemento O (de F=>M)
            tipoDeDatoSinTecho[4] = ConceptosConocidos.gen_errorKLW_en_tipo_de_dato_sin_techo_ultiMod; //=> elemento M (de F=>M)

            return tipoDeDatoSinTecho;
        }
    } // Fin de - public string[] dameTipoDeDatoSinTecho(XmlDocument domKDL, XmlNode nodo_Z_DelFractum)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   damelistaDescripcionDeEnlace : Metodos que obtiene los hijos de un nodo que instancian a otro determinado
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-03-21
    /// Parametros de entrada :
    ///      - XmlNode nodoOrigen_C_o_E = Es el nodo del que debemos devolver la lista de enlaces (E) que contiene su descripcion (D)
    ///      - XmlNamespaceManager manejadorEspNomb = Manejador del espacio de nombres
    /// Retorna : List<XmlNode>
    ///      - ListaNodos_E_en_D = la lista de enlaces (E) que contiene la descripcion (D) de nodoOrigen_C_o_E
    /// Observaciones :
    ///     - Dado el enlaca o concpto (E o C), Buscamos su D y obtenemos la lista de hijos E. Mediante los pasos siguientes :
    ///         - 1.) Miramos si el nodo que nos han enviado es enlace (E) o raiz del concepto (C) y buscamos su nodo descripcion
    ///             - 1.1.) Si es raiz de concepto, su descripcion es hija directa de este
    ///                   => C (raiz de concepto)
    ///                      => D (descripcion)  ****
    ///                         => E (enlace)
    ///                         => E (enlace)
    ///                         .....
    ///                         => E (enlace)
    ///             - 1.2.) Si es si es un enlace, su descripcion NO es hija directa de este. 
    ///                   => E (enlace)
    ///                       => A (instancia) ---  OJOO  R y Z (referencia y sin techo) NO TIENEN DESCRIPCION (D)
    ///                          => D (descripcion)  ****
    ///                             => E (enlace)
    ///                             => E (enlace)
    ///                             .....
    ///                             => E (enlace)
    ///         - 2.) Ya sabemos el elemento descripcion (D) del que tenemos que obteber los enlaces (E). Generamos la lista.
    ///                 En la lista ponemos todos los enlaces (E) sin atender a que sean A, R o Z, de eso debe encargarse el que solicito la lista.
    ///      - Si el enelace que nos han enviado es instancia, referencia o sin techo, para nosotros es irrelevante. Es quien se nos llamo, quien 
    ///         debe saber con lo que esta tratando
    ///      - OJOOO, los enlaces REFERENCIA (R) no tienen descripcion
    ///                   => E (enlace)
    ///                       => R (referencia)
    ///                          => R (referencia al tipo de dato)
    ///                             => I (identificador)
    ///                             => F (caontrol de configuracion)
    ///                             => P (Ayuda a interfaz)
    ///      - OJOOO, los enlaces sin techo (Z) no tienen descripcion
    ///                   => E (enlace)
    ///                       => Z (sin techo)
    ///                          => R (referencia al tipo de dato)
    ///                          => T (dato del sin techo (texto))
    /// </summary>
    public XmlNodeList damelistaDescripcionDeEnlace(XmlNode nodoOrigen_C_o_E, XmlNamespaceManager manejadorEspNomb)
//    public List<XmlNode> damelistaDescripcionDeEnlace(XmlNode nodoOrigen_C_o_E, XmlNamespaceManager manejadorEspNomb)
    {
        // Generamos la lista que devlveremos
        //        List<XmlNode> ListaNodos_E_en_D = new List<XmlNode>();
//        XmlNodeList ListaNodos_E_en_D = new XmlNodeList();

        // ////////////////////////////////////////
        // - 1.) Miramos si el nodo que nos han enviado es enlace (E) o raiz del concepto (C) y buscamos su nodo descripcion

        string xpath_D = "";

        if (nodoOrigen_C_o_E.Name == xPathString_name_C) {xpath_D = xPathString_para_D; } // D estadra en xPathString_para_D = "/kdl:D"
        else if (nodoOrigen_C_o_E.Name == xPathString_name_E)
        {
            if (nodoOrigen_C_o_E.FirstChild.Name == xPathString_name_A) { xpath_D = xPathString_para_A + xPathString_para_D; }
            else
            {
                if (DatosGlobal.niveDebug > 90)
                { Debug.Log("Desde ScriptLibConceptosXml => damelistaDescripcionDeEnlace. Se nos pide una descripcion de un elemento que n o es Concepto (C) ni instancia (A). Es de tipo : " + nodoOrigen_C_o_E.Name); }
                return null; // Si no es una instancia (A), ni un concepto (C), no tiene descripcion,.por lo que devolvemos la lista vacia
            }
        }  // Fin de - else if (nodoOrigen_C_o_E.Name == xPathString_name_E)

        // ////////////////////////////////////////
        // - 2.) Ya sabemos el elemento descripcion (D) del que tenemos que obteber los enlaces (E). Generamos la lista.
        //       En la lista ponemos todos los enlaces (E) sin atender a que sean A, R o Z, de eso debe encargarse el que solicito la lista.

        string xpath_D_E = "." + xpath_D + xPathString_para_E;  // El punto hace falta para iniciar la busqueda en el punto presente
        if (DatosGlobal.niveDebug > 1000)
        { Debug.Log("Desde ScriptLibConceptosXml => damelistaDescripcionDeEnlace. Voy a por la lista con  xpath_D_E : " + xpath_D_E); }
        XmlNodeList ListaNodos_E_en_D = nodoOrigen_C_o_E.SelectNodes(xpath_D_E, manejadorEspNomb);  // Seleccionamos el enlace que cntiene el nombre

        return ListaNodos_E_en_D;
    } // Fin de -  public XmlNodeList damelistaDescripcionDeEnlace(XmlNode nodoOrigen_C_o_E, XmlNamespaceManager manejadorEspNomb)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que obtiene los hijos de un nodo que instancian a otro determinado
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-02-02
    /// Parametros de entrada :
    ///      - nodoAnalizar = Nodo del que extraemos los hijos que instancian al otro
    ///      - key = key del nodo instanciado
    ///      - host = host del nodo instanciado
    ///      - manejadorEspNomb = manejadorde espacio de nombre del DOM con el que estamos trabajando
    /// Retorna : string []
    ///                  Es un strins de XmlNode que contiene los nodos localizados o sin ningun elemento si no hay ninguno
    /// Observaciones :
    ///     Obtiene los hijos DE PRIMER NIVEL que intancan al concepto dado (por key y host)
    ///      Los NODOS que devuelve son los E (kdl:E) NO LA INSTANCIA (kdl:A)
    /// </summary>

    public List<XmlNode> dameHijos_N1_QueInstancianA(XmlNode nodoAnalizar, string key, string host, XmlNamespaceManager manejadorEspNomb)
    {
        // convertir el return en un array y consegui que pueda devolver uno, ninguno o varios sin problemas 
        List<XmlNode> ListaNodosQueInstancian = new List<XmlNode>();

        //       XmlNode nodo_K_QueInstancia; No hace falta porque se declara en el foreach
        XmlNode nodo_H_QueInstancia;
        XmlNode nodo_E_QueInstancia;

        try
        {
            // Buscamos las instancias con el key correpondiente
            string xpathDeKey = "." + xPathString_para_id_Instancia + xPathString_para_K + "[text() = '" + key + "']";
            XmlNodeList nodos_K_QueInstancia = nodoAnalizar.SelectNodes(xpathDeKey, manejadorEspNomb);  // Seleccionamos el enlace que contiene el nombre

            foreach (XmlNode nodoKAnalizado in nodos_K_QueInstancia)
            {
                // Miramos a ver si su hermano H conincide con el host que nos han enviado
                string xpathHermanoH = "../kdl:H[text() = '" + host + "']";
                nodo_H_QueInstancia = nodoKAnalizado.SelectSingleNode(xpathHermanoH, manejadorEspNomb);
                string hostDeEsteKey = nodo_H_QueInstancia.InnerText;
                // Si el host que nos envian es igual del que hemos encontrado, SI es una instancia del concepto que nos dicen
                if (hostDeEsteKey == host)
                {
                    string xpathDeNodoE = "../../.."; // Busco el padre kdl:E del padre kdl:A del padre kdl:I de este nodo kdl:K 
                                                      //                Debug.Log("Desde dameHijos_N1_QueInstanciaA en 327 con xpathDeNodoE = " + xpathDeNodoE);
                    nodo_E_QueInstancia = nodoKAnalizado.SelectSingleNode(xpathDeNodoE, manejadorEspNomb);  // Seleccionamos el enlace que cntiene el nombre
                    ListaNodosQueInstancian.Add(nodo_E_QueInstancia);
                    //                string nombreDeNodoE = nodo_E_QueInstancia.Name;
                }  // Fin de - if (hostDeEsteKey == host)
                   // Si el host que nos envian es distinto del que hemos encontrado, no es una instancia del concepto que nos dicen por lo que no lo incluimos en la lista de retorno
            }  // FIn de - foreach (XmlNode nodoKAnalizado in nodos_K_QueInstancia)
        }
        catch
        {
            Debug.Log("Desde ScriptLibConceptosXml => dameHijos_N1_QueInstancianA. No hayhijos que instancien a key : " + key + " - host : "+ host);
        }

        //        string nombreDeNodoK = nodo_K_QueInstancia.Name;
        //        Debug.Log("Desde dameHijos_N1_QueInstanciaA en 315 con nombreDeNodo K = " + nombreDeNodoK+ " - con key "+ key + "- Texto del nodo K = "+ nodo_K_QueInstancia.InnerText);

        return ListaNodosQueInstancian;
    } // Fin de -  public List<XmlNode> dameHijos_N1_QueInstancianA(XmlNode nodoAnalizar, string key, string host, XmlNamespaceManager manejadorEspNomb)



    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos basicos para GENERAR ELEMENTOS DE CONCEPTO


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que genera la base para construir cualquier concepto KDL
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-23
    /// Parametros de entrada :
    /// Retorna : 
    ///     - string BaseConceptoKDL : es una cadena de estring con la cabecera XML y la raiz nodo raiz C, de un cualquier concepto KDL
    /// Observaciones :
    ///      Lo que devuelve es una base de concepto KDL EN STRING
    /// </summary>

    public string dameBaseConceptoKDL()
    {
        string BaseConceptoKDL = "<?xml version='1.0' encoding='UTF-8'?><kdl:C xmlns:kdl='http://kdlnamespace.ideando.net'></kdl:C>";
        return BaseConceptoKDL;
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta un elemento Sin Techo en un nodo descripcion (D)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-25
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo del que extraemos la informacion    
    ///      - string textoSinTecho : Texto contenido en el SinTecho
    ///      - string key_tipo : Key del tipo de dato Sin Techo que se insertara en el identificador  (del elemento I)
    ///      - string host_tipo : host del tipo de dato Sin Techo que se insertara en el identificador  (del elemento I)
    ///      - string Cualificador_tipo :  del tipo de dato Sin Techo que se insertara en el identificador   (del elemento I)
    ///      - string ordinal_tipo : Es el ordinal de la configuracion del concepto del tipo de dato Sin Techo , segun su evolucion cada modificacion lleva un ordinal consecutivo para identificarlas  (del elemento F)
    ///      - string ultimaModificacion_tipo : es la fecha de la ultima modificacion del conceptodel tipo de dato Sin Techo  (en milisegundos de tiempo gregoriano de ese) (del elemento F)
    /// Retorna : 
    ///     - string resultadoDelIngerto : Si todo ha ido bien retorna "OK". Si ha fallado algo, retorna un mensaje de error
    /// Observaciones :
    ///      - Debemos recibir un nodo D (elemento_recibido debe ser un elemento descripcion D)
    ///         <kdl:D>  (Este es el elemento elemento_recibido. el resto lo generamos)
    ///              <kdl:E>
    ///                 <kdl:Z>
    ///                   <kdl:R>
    ///                       <kdl:I>... K = gen_tipoDeSinTechoTextoPlano  y host = http://www.ideando.net/klw/dks_klw (por ejemplo)</kdl:I>
    ///                        <kdl:F> ... </kdl:F>
    ///                   </kdl:R>
    ///                   <kdl:T>
    ///                         la variable de entrada "string texto" osea el texto del sin techo
    ///                   </kdl:T>
    ///                 </kdl:Z>
    ///             </kdl:E>
    ///         </kdl:D>
    ///         
    ///      - Quien lo solicita, debe enviar en la variable "string textoSinTecho" el texto del sin techo, asi como el tipo de dato
    ///      
    ///     - Pasos para realizarlo :
    ///         1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos Sin Techo (Z), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
    ///         2.) Generamos el nodo de elemento (E)
    ///         3.) Generamos el elemento sin techo (Z), y lo ingertamos en el nodo elemento (E)
    ///         4.) Ingertamos el elemento que referencia al concepto de tipo de datos del sin techo  (R)
    ///         5.) Ingertamos el identificador y el control de configuracion de la referencia al tipo de dato (R)
    ///         6.) Ingertamos el elemento que de texto del sin techo  (T) y cumplimentamos su contenido "string texto"
    ///         7.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
    ///         8.) Si todo ha ido bien, devolvemos el OK
    /// </summary>

    public string ingerta_sinTecho(XmlDocument domKDL, XmlElement elemento_recibido, string textoSinTecho, string key_tipo, string host_tipo, string Cualificador_tipo, string ordinal_tipo, string ultimaModificacion_tipo)
    {
        // Declaramos variables
        string resultadoDelIngerto = "resultado de ingerta_solicitud_altaConcepto por defecto";

        XmlElement KDL_E;
        XmlElement KDL_Z;
        XmlElement KDL_R;
        XmlElement KDL_T;

        try
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingerta_sinTecho, al entrar al try con domKDL = " + domKDL.ToString()); }

            // 1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error

            bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "D";
            if (!esAdecuado)
            {
                resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingerta_sinTecho el ingerto no ha podido realizarse, el nodo madre, no es D. EL nodo recibido es elemento_recibido.Name = "
                    + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;
                return resultadoDelIngerto;
            }

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Desde ScriptLibConceptosXml => ingerta_sinTecho con elemento_recibido.Name = " + elemento_recibido.Name +
                  " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
                  " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
                  " - elemento_recibido.Prefix = " + elemento_recibido.Prefix);
            }

            //            elemento_recibido = domKDL.DocumentElement.FirstChild;


            // 2.) Generamos el nodo de elemento (E)

            KDL_E = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

            // 3.) Generamos el nodo sin techo Z, y lo ingertamos en el nodo elemento (E)
            KDL_Z = domKDL.CreateElement(prefijoDnsKdl, letra_SinTecho, DnsKdl);
            KDL_E.AppendChild(KDL_Z);

            // 3.) Generamos el nodo de tipo del dato sin techo R, y lo ingertamos en el nodo elemento (Z)
            KDL_R = domKDL.CreateElement(prefijoDnsKdl, letra_Referencia, DnsKdl);
            KDL_Z.AppendChild(KDL_R);

            // 4.) Ingertamos el identificador y el control de configuracion del nodo instancia (A)
            string resultIngerIdentificador = ingertaIdentificadorDeNodo(domKDL, KDL_R, key_tipo, host_tipo, Cualificador_tipo);
            string resultIngerCtrlConfig = ingertaCtrlConfDeNodo(domKDL, KDL_R, ordinal_tipo, ultimaModificacion_tipo);

            // 5.) Generamos el nodo (D) de descripcion de la instancia (A), La descripcion debe cumplimentarla quien llame a esta funcion, si procede
            KDL_T = domKDL.CreateElement(prefijoDnsKdl, letra_Texto, DnsKdl);
            KDL_T.InnerText = textoSinTecho;
            KDL_Z.AppendChild(KDL_T);

            // 6.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
            elemento_recibido.AppendChild(KDL_E);


            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);

                Debug.Log(" Desde ScriptLibConceptosXml => ingerta_sinTecho _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 7.) Si todo ha ido bien, devolvemos el OK
            resultadoDelIngerto = "OK";
            return resultadoDelIngerto;

        }  // FIn de -  try
        catch
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaElementoInstancia con en nodo = " + elemento_recibido.Name); }

            resultadoDelIngerto = "ERROR, desde ScriptLibConceptosXml => ScriptLibConceptosXml => ingertaElementoInstancia al generar el nodo elemento";
            return resultadoDelIngerto;
        }
    } // Fin de -  public string ingerta_sinTecho(XmlDocument domKDL, XmlElement elemento_recibido, string texto, string key_tipo, string host_tipo, string Cualificador_tipo, string ordinal_tipo, string ultimaModificacion_tipo)


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta un nodo de elemento de una referencia en un nodo descripcion (D)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-30
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo en el que hay que realizar el ingerto    
    ///      - string key : Key del concepto a referenciar (del elemento I)
    ///      - string host : host del concepto a referenciar  (del elemento I)
    ///      - string Cualificador :  del concepto a referenciar que se insertara en el identificador   (del elemento I)
    ///      - string ordinal : Es el ordinal de la configuracion del concepto, segun su evolucion cada modificacion lleva un ordinal consecutivo para identificarlas  (del elemento F)
    ///      - string ultimaModificacion : es la fecha de la ultima modificacion del concepto (en milisegundos de tiempo gregoriano de ese) (del elemento F)
    /// Retorna : 
    ///     - string resultadoDelIngerto : Si todo ha ido bien retorna "OK". Si ha fallado algo, retorna un mensaje de error
    /// Observaciones :
    ///    - OJOOO Quien la llama DEBE ATENDER LA EXCEPCION : throw new ArgumentException("ERROR. No es posoible ingertar un elemento instancia (E=>A) fuera de un elemento descripcion (D)");
    ///    
    ///      Debemos recibir un nodo D (elemento_recibido debe ser un elemento descripcion D)
    ///         <kdl:D>  (Este es el elemento elemento_recibido. el resto lo generamos)
    ///              <kdl:E>
    ///                  <kdl:R>
    ///                     <kdl:I> ... </kdl:I>
    ///                     <kdl:H> ... </kdl:F>
    ///                  </kdl:R>
    ///             </kdl:E>
    ///         </kdl:D>
    ///      - Los elementos E, solo pueden tener un padre descripcion (elemento D)
    ///     - Pasos para realizarlo :
    ///         1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de referencia (R), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
    ///         2.) Generamos el nodo de elemento (E)
    ///         3.) Generamos el nodo referencia R, y lo ingertamos en el nodo elemento (E)
    ///         4.) Ingertamos el identificador y el control de configuracion del nodo referencia (R)
    ///         5.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
    ///         6.) Si todo ha ido bien, devolvemos el OK
    ///         
    /// </summary>

    public string ingertaElementoReferencia(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador, string ordinal, string ultimaModificacion)
    {
        // Declaramos variables
        string resultadoDelIngerto = "resultado de ingerto de elemento instancia por defecto";

        XmlElement KDL_E;
        XmlElement KDL_R;

        // 1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de referencia (R), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
        bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "D";
        if (!esAdecuado)
        {
            resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingertaElementoReferencia el ingerto no ha podido realizarse, el nodo madre, no es D. EL nodo recibido es elemento_recibido.Name = "
                 + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;

            throw new ArgumentException(resultadoDelIngerto);
        }

        string datosDeProceso = "ScriptLibConceptosXml => ingertaElementoReferencia. DatosDeProceso - Por defecto";
        try
        {
            datosDeProceso = "Desde ScriptLibConceptosXml => ingertaElementoReferencia con elemento_recibido.Name = " + elemento_recibido.Name +
              " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
              " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
              " - elemento_recibido.Prefix = " + elemento_recibido.Prefix;

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log(datosDeProceso);
            }

            // 2.) Generamos el nodo de elemento (E)

            KDL_E = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

            // 3.) Generamos el nodo referencia R, y lo ingertamos en el nodo elemento (E)
            KDL_R = domKDL.CreateElement(prefijoDnsKdl, letra_Referencia, DnsKdl);
            KDL_E.AppendChild(KDL_R);

            // 4.) Ingertamos el identificador y el control de configuracion del nodo referencia (R)
            string resultIngerIdentificador = ingertaIdentificadorDeNodo(domKDL, KDL_R, key, host, Cualificador);
            string resultIngerCtrlConfig = ingertaCtrlConfDeNodo(domKDL, KDL_R, ordinal, ultimaModificacion);

            // 5.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
            elemento_recibido.AppendChild(KDL_E);

            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);
                Debug.Log(" Desde ScriptLibConceptosXml => ingertaElementoReferencia _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 7.) Si todo ha ido bien, devolvemos el OK
            resultadoDelIngerto = "OK";
            return resultadoDelIngerto;

        }  // FIn de -  try
        catch
        {
            throw new ArgumentException("ERROR. Desde ScriptLibConceptosXml => ingertaElementoReferencia. Al intentar ingertar el elemento instancia (E=>R) fuera de un elemento descripcion (D). Con datos = " + datosDeProceso);
        }
    } // Fin de -  public XmlElement ingertaElementoReferencia(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador, string ordinal, string ultimaModificacion)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta un nodo de elemento de una instancia en un nodo descripcion (D)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-25
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo en el que hay que realizar el ingerto    
    ///      - string key : Key que se insertara en el identificador  (del elemento I)
    ///      - string host : host que se insertara en el identificador  (del elemento I)
    ///      - string Cualificador :  que se insertara en el identificador   (del elemento I)
    ///      - string ordinal : Es el ordinal de la configuracion del concepto, segun su evolucion cada modificacion lleva un ordinal consecutivo para identificarlas  (del elemento F)
    ///      - string ultimaModificacion : es la fecha de la ultima modificacion del concepto (en milisegundos de tiempo gregoriano de ese) (del elemento F)
    /// Retorna : 
    ///     - XmlElement KDL_D_hijo : Es el nodo descripcion de la instancia. Normalmente, quien llama a esta funcion, debe cumplimentar la descripcion de la instancia
    ///                                 ingertando los elementos que proceda en este elemento descripcion
    /// Observaciones :
    ///    - OJOOO Quien la llama DEBE ATENDER LA EXCEPCION : throw new ArgumentException("ERROR. No es posoible ingertar un elemento instancia (E=>A) fuera de un elemento descripcion (D)");
    ///    
    ///      Debemos recibir un nodo D (elemento_recibido debe ser un elemento descripcion D)
    ///         <kdl:d>  (Este es el elemento elemento_recibido. el resto lo generamos)
    ///              <kdl:E>
    ///                  <kdl:A>
    ///                     <kdl:I> ... </kdl:I>
    ///                     <kdl:H> ... </kdl:F>
    ///                     <kdl:D></kdl:D>  (Este aqui lo hemos llamado KDL_D_hijo)
    ///                  </kdl:A>
    ///             </kdl:E>
    ///         </kdl:d>
    ///      - Los elementos E, solo pueden tener un padre descripcion (elemento D)
    ///     - Pasos para realizarlo :
    ///         1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
    ///         2.) Generamos el nodo de elemento (E)
    ///         3.) Generamos el nodo instancia A, y lo ingertamos en el nodo elemento (E)
    ///         4.) Ingertamos el identificador y el control de configuracion del nodo instancia (A)
    ///         5.) Generamos el nodo (D) de descripcion de la instancia (A), La descripcion debe cumplimentarla quien llame a esta funcion, si procede
    ///         6.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
    ///         7.) Si todo ha ido bien, devolvemos el OK
    ///         
    /// </summary>

    public XmlElement ingertaElementoInstancia(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador, string ordinal, string ultimaModificacion)
    {
        // Declaramos variables
        string resultadoDelIngerto = "resultado de ingerto de elemento instancia por defecto";

        XmlElement KDL_E;
        XmlElement KDL_A;
        XmlElement KDL_D_hijo;

        // 1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
        bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "D";
        if (!esAdecuado)
        {
            resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingertaElementoInstancia el ingerto no ha podido realizarse, el nodo madre, no es D. EL nodo recibido es elemento_recibido.Name = "
                 + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;

            throw new ArgumentException(resultadoDelIngerto);
        }

        string datosDeProceso = "ScriptLibConceptosXml => ingertaElementoInstancia. DatosDeProceso - Por defecto";
        try
        {
            datosDeProceso = "Desde ScriptLibConceptosXml => ingertaElementoInstancia con elemento_recibido.Name = " + elemento_recibido.Name +
              " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
              " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
              " - elemento_recibido.Prefix = " + elemento_recibido.Prefix;

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log(datosDeProceso);
            }

            // 2.) Generamos el nodo de elemento (E)

            KDL_E = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

            // 3.) Generamos el nodo instancia A, y lo ingertamos en el nodo elemento (E)
            KDL_A = domKDL.CreateElement(prefijoDnsKdl, letra_Instancia, DnsKdl);
            KDL_E.AppendChild(KDL_A);

            // 4.) Ingertamos el identificador y el control de configuracion del nodo instancia (A)
            string resultIngerIdentificador = ingertaIdentificadorDeNodo(domKDL, KDL_A, key, host, Cualificador);
            string resultIngerCtrlConfig = ingertaCtrlConfDeNodo(domKDL, KDL_A, ordinal, ultimaModificacion);

            // 5.) Generamos el nodo (D) de descripcion de la instancia (A), La descripcion debe cumplimentarla quien llame a esta funcion, si procede
            KDL_D_hijo = domKDL.CreateElement(prefijoDnsKdl, letra_Descripcion, DnsKdl);
            KDL_A.AppendChild(KDL_D_hijo);

            // 6.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
            elemento_recibido.AppendChild(KDL_E);

            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);
                Debug.Log(" Desde ScriptLibConceptosXml => ingertaElementoInstancia _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 7.) Si todo ha ido bien, devolvemos el OK
            return KDL_D_hijo;

        }  // FIn de -  try
        catch
        {
            throw new ArgumentException("ERROR. Desde ScriptLibConceptosXml => ingertaElementoInstancia. Al intentar ingertar el elemento instancia (E=>A) fuera de un elemento descripcion (D). Con datos = " + datosDeProceso);
        }
    } // Fin de -  public XmlElement ingertaElementoInstancia(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador, string ordinal, string ultimaModificacion)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta el identificador de una rama de conceto (key K, host H y cualificador Q (efimero,...)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-21
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo del que extraemos la informacion    
    ///      - string key : Key que se insertara en el identificador (del elemento I)
    ///      - string host : host que se insertara en el identificador (del elemento I)
    ///      - string Cualificador :  que se insertara en el identificador (del elemento I)
    /// Retorna : 
    ///     - string resultadoDelIngerto : Si todo ha ido bien retorna "OK". Si ha fallado algo, retorna un mensaje de error
    /// Observaciones :
    ///      El identificador es un nodo I
    ///         <kdl:I>
    ///             <kdl:K>gen_efimero_solicitudADks_0</kdl:K>
    ///             <kdl:H>agenteLocal</kdl:H>
    ///             <kdl:Q>1</kdl:Q>
    ///         </kdl:I>
    ///      -Los nodos de KDL que tienen como hijo un nodo I pueden ser:
    ///          - Nodo concepto "C" (concepto)
    ///          - Nodo instancia "A" (Instancia)
    ///          - Nodo referencia "R" (referencia)
    ///     - Pasos para realizarlo :
    ///         1.) Generamos el nodo I
    ///         2.) Generamos el nodo K, le ponemos el contenido testo (Key) y lo hacemos hijoo de I
    ///         3.) Generamos el nodo H, le ponemos el contenido testo (Host) y lo hacemos hijoo de I
    ///         4.) Generamos el nodo Q, le ponemos el contenido testo (Cualificador) y lo hacemos hijoo de I
    ///         5.) Hacemos el nodo I hijo dl elemento (C, A o R) que nos han enviado
    ///         6.) Si todo ha ido bien, devolvemos el OK
    /// </summary>

    public string ingertaIdentificadorDeNodo(XmlDocument domKDL, XmlElement elemento_recibido, string key, string host, string Cualificador)
    {
        // Declaramos variables
        string resultadoDelIngerto = "resultado de ingerto de identificador por defecto";

        XmlElement KDL_I;
        XmlElement KDL_K;
        XmlElement KDL_H;
        XmlElement KDL_Q;

        try
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaIdentificadorDeNodo, al entrar al try con domKDL = " + domKDL.ToString()); }

            // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R" ( los sin techo "Z" no tienen identificador). Si no es asi, devolvemos un error

            bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "C" | elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "A" | elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "R";
            if (!esAdecuado)
            {
                resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingertaIdentificadorDeNodo el ingerto no ha podido realizarse, el nodo madre, no es C, ni A, ni R"; 
                return resultadoDelIngerto;
            }

            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaIdentificadorDeNodo con elemento_recibido.Name = " + elemento_recibido.Name +
                " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
                " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
                " - elemento_recibido.Prefix = " + elemento_recibido.Prefix); }

            //            elemento_recibido = domKDL.DocumentElement.FirstChild;

            // 1.) Generamos el nodo I

            KDL_I = domKDL.CreateElement(prefijoDnsKdl, letra_Identificador, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

            // 2.) Generamos el nodo K, le ponemos el contenido testo (Key) y lo hacemos hijoo de I
            KDL_K = domKDL.CreateElement(prefijoDnsKdl, letra_Key, DnsKdl);
            KDL_K.InnerText = key;
            KDL_I.AppendChild(KDL_K);

            // 3.) Generamos el nodo H, le ponemos el contenido testo (Host) y lo hacemos hijoo de I
            KDL_H = domKDL.CreateElement(prefijoDnsKdl, letra_Host, DnsKdl);
            KDL_H.InnerText = host;
            KDL_I.AppendChild(KDL_H);

            ///         4.) Generamos el nodo Q, le ponemos el contenido testo (Cualificador) y lo hacemos hijoo de I
            KDL_Q = domKDL.CreateElement(prefijoDnsKdl, letra_Cualificador, DnsKdl);
            KDL_Q.InnerText = Cualificador;
            KDL_I.AppendChild(KDL_Q);

            // 5.) Hacemos el nodo I hijo dl elemento (C, A o R) que nos han enviado
            elemento_recibido.AppendChild(KDL_I);


            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);

                Debug.Log(" Desde ScriptLibConceptosXml => ingertaIdentificadorDeNodo _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 6.) Si todo ha ido bien, devolvemos el OK
            resultadoDelIngerto = "OK";
            return resultadoDelIngerto;

        }  // FIn de -  try
        catch
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaIdentificadorDeNodo con en nodo = " + elemento_recibido.Name); }

            resultadoDelIngerto = "ERROR, desde ScriptLibConceptosXml => ScriptLibConceptosXml => ingertaIdentificadorDeNodo al generar el nodo identificador";
            return resultadoDelIngerto;
        }
    } // Fin de -  public string[] dameIdentificadorDeNodo(XmlDocument domKDL, XmlNode nodoElemento)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta el identificador de una rama de conceto (key K, host H y cualificador Q (efimero,...)
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-21
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo del que extraemos la informacion    
    ///      - string ordinal : Es el ordinal de la configuracion del concepto, segun su evolucion cada modificacion lleva un ordinal consecutivo para identificarlas (del elemento F)
    ///      - string ultimaModificacion : es la fecha de la ultima modificacion del concepto (en milisegundos de tiempo gregoriano de ese) (del elemento F)
    /// Retorna : 
    ///     - string resultadoDelIngerto : Si todo ha ido bien retorna "OK". Si ha fallado algo, retorna un mensaje de error
    /// Observaciones :
    ///      El control de configuracion es un nodo F
    ///         <kdl:F>
    ///             <kdl:O>ordinal segun las distintas modificaciones</kdl:O>
    ///             <kdl:M>fecha de ultima modificacion (en milisegundos de tiempo gregoriano de ese)</kdl:M>
    ///         </kdl:F>
    ///      -Los nodos de KDL que tienen como hijo un nodo F pueden ser:
    ///          - Nodo concepto "C" (concepto)
    ///          - Nodo instancia "A" (Instancia)
    ///          - Nodo referencia "R" (referencia)
    ///     - Pasos para realizarlo :
    ///         1.) Generamos el nodo F
    ///         2.) Generamos el nodo O, le ponemos el contenido testo (ordinal) y lo hacemos hijoo de F
    ///         3.) Generamos el nodo M, le ponemos el contenido testo (ultimaModificacion) y lo hacemos hijoo de F
    ///         4.) Hacemos el nodo F hijo dl elemento (C, A o R) que nos han enviado
    ///         5.) Si todo ha ido bien, devolvemos el OK
    /// </summary>

    public string ingertaCtrlConfDeNodo(XmlDocument domKDL, XmlElement elemento_recibido, string ordinal, string ultimaModificacion)
    {
        // Declaramos variables
        string resultadoDelIngerto = "resultado de ingerto de identificador por defecto";

        XmlElement KDL_F;
        XmlElement KDL_O;
        XmlElement KDL_M;

        try
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaCtrlConfDeNodo, al entrar al try con domKDL = " + domKDL.ToString()); }

            // Controlamos que el nodo que nos han pasado es Concpto "C", instancia "A" o referencia "R" ( los sin techo "Z" no tienen identificador). Si no es asi, devolvemos un error

            bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "C" | elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "A" | elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "R";
            if (!esAdecuado)
            {
                resultadoDelIngerto = "ERROR, Desde ScriptLibConceptosXml => ingertaCtrlConfDeNodo el ingerto no ha podido realizarse, el nodo madre, no es C, ni A, ni R";
                return resultadoDelIngerto;
            }

            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaCtrlConfDeNodo con en nodo = " + elemento_recibido.Name +
                " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
                " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
                " - elemento_recibido.Prefix = " + elemento_recibido.Prefix); }


            // 1.) Generamos el nodo I
            KDL_F = domKDL.CreateElement(prefijoDnsKdl, letra_CtrlConf, DnsKdl);
 
            // 2.) Generamos el nodo K, le ponemos el contenido testo (Key) y lo hacemos hijoo de I
            KDL_O = domKDL.CreateElement(prefijoDnsKdl, letra_Ordinal, DnsKdl);
            KDL_O.InnerText = ordinal;
            KDL_F.AppendChild(KDL_O);

            // 3.) Generamos el nodo H, le ponemos el contenido testo (Host) y lo hacemos hijoo de I
            KDL_M = domKDL.CreateElement(prefijoDnsKdl, letra_UltModf, DnsKdl);
            KDL_M.InnerText = ultimaModificacion;
            KDL_F.AppendChild(KDL_M);

            // 4.) Hacemos el nodo I hijo dl elemento (C, A o R) que nos han enviado
            elemento_recibido.AppendChild(KDL_F);

            // 6.) Si todo ha ido bien, devolvemos el OK
            resultadoDelIngerto = "OK";
            return resultadoDelIngerto;

        }  // FIn de -  try
        catch
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaCtrlConfDeNodo con en nodo = " + elemento_recibido.Name); }

            resultadoDelIngerto = "ERROR, Desde ScriptLibConceptosXml => ingertaCtrlConfDeNodo al generar el nodo identificador";
            return resultadoDelIngerto;
        }
    } // Fin de -  public string[] dameIdentificadorDeNodo(XmlDocument domKDL, XmlNode nodoElemento)


    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos para generar conceptos XML  especificos


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que ingerta un nodo de elemento de una instancia de solicitud a DKS en una descripcion (elemento D) que viene como entrada
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-29
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo D en que que tenemos que realizar el ingerto   
    /// Retorna : 
    ///     - XmlElement KDL_D_de_gen_solicitudADks : Es el elemento D donde, aquel que llame a esta funcion, debe ingertar la descripcion de esta instancia
    /// Observaciones :
    ///     - La estructura puede verse en el fichero "D:\Datos\Ideando\Desarrollo\KLW\XML Conceptos\Conceptos KLW\Solicitudes a DKS\sol_altaConcepto.xml"
    ///     - El concepto de interes es  : key="gen_solicitud_altaConcepto", host="http://www.ideando.net/klw/dks_klw"
    ///     - La semantica de ancestros es fundamentalmente : (solicitud de alta a DKS es una solicitud a DKS) 
    ///             - (key="gen_solicitudADks", host="http://www.ideando.net/klw/dks_klw")
    ///                 
    ///     - ingerta la rama necesaria para una solicitud de alta a un concepto. La rama DOM generada es como sigue :
    ///     
    ///      Debemos recibir un nodo D (elemento_recibido debe ser un elemento descripcion D)
    ///         <kdl:D>  (Este es el elemento elemento_recibido. el resto lo generamos)
    ///             <kdl:E>
    ///                <kdl:A>
    ///                     <kdl:I>
    ///                         <kdl:K>gen_solicitudADks</kdl:K>
    ///                         <kdl:H>http://localhost/klw/dks_klw</kdl:H>
    ///                         <kdl:Q>0</kdl:Q>
    ///                     </kdl:I>
    ///                     <kdl:F>
    ///                         <kdl:O>0</kdl:O>
    ///                         <kdl:M>http://localhost/klw/dks_klw</kdl:M>
    ///                     </kdl:F>
    ///                     <kdl:D>  (este es el elemento "KDL_D_de_gen_solicitudADks" que devuelve esta funcion)
    ///                         ... aqui debe ingertar la descripcion de esta instancia, aquel que llame a esta funcion
    ///                     </kdl:D>
    ///                 </kdl:A>
    ///             </kdl:E>
    ///         </kdl:D>
    ///         
    ///     - Pasos para realizarlo :
    ///         1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A:gen_solicitudADks), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error
    ///         2.) Generamos el nodo de elemento (E)
    ///         3.) Generamos el nodo instancia A:gen_solicitudADks, y lo ingertamos en el nodo elemento (E)
    ///         4.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_solicitudADks)
    ///         5.) Generamos el nodo (D) de descripcion de la instancia (A:gen_solicitudADks), La descripcion debe cumplimentarla quien llame a esta funcion, si procede
    ///         6.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
    ///         7.) Si todo ha ido bien, devolvemos el elemento donde debe ponerse la descripcion (D) como instancia del concepto gen_solicitudADks 
    /// </summary>

    public XmlElement ingerta_solicitud_aDKS(XmlDocument domKDL, XmlElement elemento_recibido)
    {
        XmlElement KDL_E;
        XmlElement KDL_A;
        XmlElement KDL_D_de_gen_solicitudADks;// este es el elemento D, donde debe ponerse la descripcion, COMO INSTANCIA, 
                                              // de gen_solicitudADks (normalmente un tipo de solicitud : gen_solicitud_altaConcepto, o cualquier otro)


        try
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingerta_solicitud_aDKS, al entrar al try con domKDL = " + domKDL.ToString()); }

            // 1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A:gen_solicitudADks), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error

            bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "D";
            if (!esAdecuado)
            {
                string resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingerta_solicitud_aDKS. No es posoible ingertar un elemento instancia (E=>A:gen_solicitud_altaConcepto) fuera de un elemento descripcion (D)." +
                    " EL nodo recibido es elemento_recibido.Name = " + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;
                throw new ArgumentException(resultadoDelIngerto);
            }

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Desde ScriptLibConceptosXml => ingertaElementoInstancia con elemento_recibido.Name = " + elemento_recibido.Name +
                  " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
                  " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
                  " - elemento_recibido.Prefix = " + elemento_recibido.Prefix);
            }

            // 2.) Generamos el nodo de elemento (E)

            KDL_E = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

            // 3.) Generamos el nodo instancia A, y lo ingertamos en el nodo elemento (E)
            KDL_A = domKDL.CreateElement(prefijoDnsKdl, letra_Instancia, DnsKdl);
            KDL_E.AppendChild(KDL_A);

            // 4.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_solicitudADks)
            string cualificador_gen_solicitudADks = "0"; // esta dentro de un efimero, pero la instancia lo es a un concepto que no lo es
            string resultIngerIdentificador = ingertaIdentificadorDeNodo(domKDL, KDL_A, ConceptosConocidos.gen_solicitudADks_key, ConceptosConocidos.gen_solicitudADks_host, cualificador_gen_solicitudADks);
            string resultIngerCtrlConfig = ingertaCtrlConfDeNodo(domKDL, KDL_A, ConceptosConocidos.gen_solicitudADks_ordinal, ConceptosConocidos.gen_solicitudADks_ultimaModif);

            // 5.) Generamos el nodo (D) de descripcion de la instancia (A:gen_solicitudADks), La descripcion debe cumplimentarla quien llame a esta funcion, si procede
            KDL_D_de_gen_solicitudADks = domKDL.CreateElement(prefijoDnsKdl, letra_Descripcion, DnsKdl);
            KDL_A.AppendChild(KDL_D_de_gen_solicitudADks);

            // 6.) Hacemos el nodo E hijo del elemento descripcion (D) que nos han enviado
            elemento_recibido.AppendChild(KDL_E);


            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);

                Debug.Log(" Desde ScriptLibConceptosXml => ingertaElementoInstancia _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 7.) Si todo ha ido bien, devolvemos el elemento donde debe ponerse la descripcion (D) como instancia del concepto gen_solicitudADks 
            return KDL_D_de_gen_solicitudADks;

        }  // FIn de -  try
        catch
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingertaElementoInstancia con en nodo = " + elemento_recibido.Name); }

            throw new ArgumentException("ERROR. Desde ScriptLibConceptosXml => ingerta_solicitud_aDKS. Fallo al ingertar el elemento instancia (E=>A:gen_solicitudADks)");
        }
    } // Fin de -  public string[] dameIdentificadorDeNodo(XmlDocument domKDL, XmlNode nodoElemento)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///  Metodos que ingerta un nodo de elemento de una instancia de solicitud de alta de concepto a DKS en una descripcion (elemento D) que viene com entrada
    /// Autor : 	Miguel Angel Fernandez Graciani
    /// Fecha :	2021-08-25
    /// Fecha de ultima modificacion :	
    ///     - 2022-01-14 : Con el fin de poder incluir el dia de mañana mas caracteristicas asociadas al alta del concepto (usuario u otros futuros), he decidido
    ///                     en lugar de poner la descripcion del concepto a dar de alta directamente en el elemento D de la instancia de "gen_solicitud_altaConcepto", 
    ///                     incluir en esta descripcion una instancia al concepto "gen_D" cuya descripcion, ahora si sera la descripcion del concepto a dar de alta.
    ///                     De esta manera en el elemento D de "gen_solicitud_altaConcepto", se pueden incluir otras intancias a conceptos que complementen en un futuro
    ///                     la informacion asociada a la solicitud de alta de concepto
    /// Parametros de entrada :
    ///      - XmlDocument domKDL : Dom del concepto general
    ///      - XmlElement elemento_recibido : Nodo en el que debemos ingertar el elemento E que contiene la rama completa de la solicitud    
    /// Retorna : 
    ///     - XmlElement KDL_D_de_gen_solicitud_altaConcepto : Es el elemento D donde, aquel que llame a esta funcion, debe ingertar la descripcion de esta instancia
    /// Observaciones :
    ///     - La estructura puede verse en el fichero "D:\Datos\Ideando\Desarrollo\KLW\XML Conceptos\Conceptos KLW\Solicitudes a DKS\sol_altaConcepto.xml"
    ///     - El concepto de interes es  : key="gen_solicitud_altaConcepto", host="http://www.ideando.net/klw/dks_klw"
    ///     - La semantica de ancestros es fundamentalmente : (solicitud de alta a DKS es una solicitud a DKS) 
    ///             - (key="gen_solicitudADks", host="http://www.ideando.net/klw/dks_klw")
    ///                 - (key="gen_solicitud_altaConcepto", host="http://www.ideando.net/klw/dks_klw")
    ///                 
    ///     - ingerta la rama necesaria para una solicitud de alta a un concepto. La rama DOM generada es como sigue :
    ///     
    ///      Debemos recibir un nodo D (elemento_recibido debe ser un elemento descripcion D)
    ///         <kdl:D>  (Este es el elemento elemento_recibido. el resto lo generamos)
    ///            <kdl:E>  (Este es el elemento asociado a la instancia de "gen_solicitudADks")- Ingertamos primero una instancia de "gen_solicitudADks" mediante la funcion "ingerta_solicitud_aDKS()"
    ///                 .....
    ///                 .....
    ///                <kdl:D>  (Este es el elemento D de la instancia de "gen_solicitudADks")
    ///                     <kdl:E>
    ///                         <kdl:A>
    ///                             <kdl:I>
    ///                                 <kdl:K>gen_solicitud_altaConcepto</kdl:K>
    ///                                 <kdl:H>http://localhost/klw/dks_klw</kdl:H>
    ///                                 <kdl:Q>0</kdl:Q>
    ///                             </kdl:I>
    ///                             <kdl:F>
    ///                                 <kdl:O>0</kdl:O>
    ///                                 <kdl:M>http://localhost/klw/dks_klw</kdl:M>
    ///                            </kdl:F>
    ///                            <kdl:D>  (este es el elemento D de "KDL_D_de_gen_solicitudADks" que contendra la descripcion de la solicitud de alta de concepro)
    ///                                 ... contirnr una instancia de "gen_D" que es donde va la descripcion del concepto a dar de alta, pero puede contener otra
    ///                                 informacion referente al usuario que lo da de alta, o a otras condiciones de alta futuras (PENDIENTE MAFG 2022-01-14)
    ///                                 <kdl:E>
    ///                                     <kdl:A>
    ///                                         <kdl:I>
    ///                                             <kdl:K>gen_D</kdl:K>
    ///                                             <kdl:H>http://localhost/klw/dks_klw</kdl:H>
    ///                                             <kdl:Q>0</kdl:Q>
    ///                                         </kdl:I>
    ///                                         <kdl:F>
    ///                                             <kdl:O>0</kdl:O>
    ///                                             <kdl:M>http://localhost/klw/dks_klw</kdl:M>
    ///                                         </kdl:F>
    ///                                         <kdl:D>  ( ***1*** este es el elemento D de la inastancia de "gen_D" que devuelve esta funcion)
    ///                                               ... aqui debe ingertar la descripcion de esta instancia, aquel que llame a esta funcion
    ///                                         </kdl:D>
    ///                                     </kdl:A>
    ///                                 </kdl:E>
    ///                                 <kdl:E>
    ///                                 ... Aqui iran futuras carcteristicas de la solicitud de alta de concepto
    ///                                 </kdl:E>
    ///                            </kdl:D>
    ///                         </kdl:A>
    ///                     </kdl:E>
    ///                <kdl:D>  (FN de el elemento D de la instancia de "gen_solicitudADks")
    ///            </kdl:E>  (Fin de el elemento asociado a la instancia de "gen_solicitudADks")- Ingertamos primero una instancia de "gen_solicitudADks" mediante la funcion "ingerta_solicitud_aDKS()"
    ///         </kdl:D> ( Fin de el elemento elemento_recibido. el resto lo generamos)
    ///      
    ///     - Los Pasos para realizar por este metodo son:
    ///         1.) Controlamos que el nodo que nos han pasado es Concepto "D", ya que los elementos de instancia (A:gen_solicitudADks), solo pueden ser hijos de un elemento Descripcion (D). Si no es asi, devolvemos un error
    ///         
    ///         2.) Ingertamos primeramente un enlace de instancia al concepto "gen_solicitudADks", ya que el concepto que queremos instanciar "gen_solicitud_altaConcepto", instancia a este
    ///         
    ///         3.) Generamos el nodo para la instancia de "gen_solicitud_altaConcepto"         
    ///             3.1.) Generamos el nodo de elemento (E) para la instancia de "gen_solicitud_altaConcepto". Que va en la descripcion de la instancia de "gen_solicitudADks" que ya hemos generado
    ///             3.2.) Generamos el nodo instancia A:gen_solicitud_altaConcepto, y lo ingertamos en el nodo elemento (E)
    ///             3.3.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_solicitud_altaConcepto)
    ///             3.4.) Generamos el nodo (D) de descripcion de la instancia (A:gen_solicitud_altaConcepto), 
    ///             3.5.) Hacemos el nodo E hijo del elemento descripcion (D) de la instancia "gen_solicitudADks" que generamos anteriormente mediante "gen_solicitud_altaConcepto" en el paso 2.)
    ///         
    ///         4.) Generamos el nodo para la instancia de "gen_D"         
    ///             4.1.) Generamos el nodo de elemento (E) para la instancia de "gen_D". Que va en la descripcion de la instancia de "gen_solicitud_altaConcepto" que ya hemos generado
    ///             4.2.) Generamos el nodo instancia A:gen_D, y lo ingertamos en el nodo elemento (E)
    ///             4.3.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_D)
    ///             4.4.) Generamos el nodo (D) ***1*** de descripcion de la instancia (A:gen_D), La descripcion debe cumplimentarla quien llame a esta funcion, con la descripcion del concepto a dar de alta
    ///             4.5.) Hacemos el nodo E hijo del elemento descripcion (D) de la instancia "gen_solicitud_altaConcepto" que generamos anteriormente mediante "gen_solicitud_altaConcepto" en el paso 3.)
    ///         
    ///         5.) Si todo ha ido bien, devolvemos el elemento donde debe ponerse la descripcion (D) como instancia del concepto "gen_D". Esta descripcion
    ///               debe contener el elemento descripcion D, del concepto que se va a dar de alta. Esta descripcion, normalmente se obtiene mediante la funcion "edicion_dameDescripcion()"
    ///               recorriendo el arbol de edicion del concepto en edicion en la interfaz KEE
    ///         
    ///      - Quien lo solicita, debe cumplimentar la descripcion del concepto a dar de alta en el elemento D marcado con ***1***, antes de enviar la solicitud al DKS
    ///
    /// </summary>

    public XmlElement ingerta_solicitud_altaConcepto(XmlDocument domKDL, XmlElement elemento_recibido)
    {
        // Declaramos variables

        XmlElement KDL_D_de_gen_solicitudADks;// este es el elemento D, donde debe ponerse la descripcion, COMO INSTANCIA, 
                                              // de gen_solicitudADks (normalmente un tipo de solicitud : gen_solicitud_altaConcepto, o cualquier otro)
        XmlElement KDL_E_de_gen_solicitud_altaConcepto;
        XmlElement KDL_A_de_gen_solicitud_altaConcepto;
        XmlElement KDL_D_de_gen_solicitud_altaConcepto; // este es el elemento D de gen_solicitud_altaConcepto, donde ira la instancia de "gen_D" y otros en un futuro

        XmlElement KDL_E_de_gen_D;
        XmlElement KDL_A_de_gen_D;
        XmlElement KDL_D_de_gen_D; // este es el elemento D, donde debe ponerse la descripcion, COMO INSTANCIA, 
                                   // de gen_D (osea la descripcion del concepto a dar de alta en el DKS)

        try
        {
            if (DatosGlobal.niveDebug > 1000)
            { Debug.Log("Desde ScriptLibConceptosXml => ingerta_solicitud_altaConcepto, al entrar al try con elemento_recibido.Name = " + elemento_recibido.Name); }

            // 1.) Controlamos que el nodo que nos han pasado es Concpto "D", ya que los elementos de instancia (A), solo pueden ser hijos d eun elemento Descripcion (D). Si no es asi, devolvemos un error

            bool esAdecuado = elemento_recibido.Name == prefijoDnsKdlConDosPuntos + "D";
            if (!esAdecuado)
            {
                string resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingerta_solicitud_altaConcepto. No es posoible ingertar un elemento instancia (E=>A:gen_solicitud_altaConcepto) fuera de un elemento descripcion (D)." +
                    " EL nodo recibido es elemento_recibido.Name = " + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;
                throw new ArgumentException(resultadoDelIngerto);
            }

            if (DatosGlobal.niveDebug > 1000)
            {
                Debug.Log("Desde ScriptLibConceptosXml => ingertaElementoInstancia con elemento_recibido.Name = " + elemento_recibido.Name +
                  " - elemento_recibido.NamespaceURI = " + elemento_recibido.NamespaceURI +
                  " - elemento_recibido.NodeType = " + elemento_recibido.NodeType +
                  " - elemento_recibido.Prefix = " + elemento_recibido.Prefix);
            }

            // 2.) Ingertamos primeramente un enlace de instancia al concepto "gen_solicitudADks", ya que el concepto que queremos instanciar "gen_solicitud_altaConcepto", instancia a este
            KDL_D_de_gen_solicitudADks = ingerta_solicitud_aDKS(domKDL, elemento_recibido);

            // ////////////////////////////////////////////////////
            // 3.) Generamos el nodo para la instancia de "gen_solicitud_altaConcepto"
            
                // 3.1.) Generamos el nodo de elemento (E) para la instancia de "gen_solicitud_altaConcepto". Que va en la descripcion de la instancia de "gen_solicitudADks" que ya hemos generado

            KDL_E_de_gen_solicitud_altaConcepto = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

                // 3.2.) Generamos el nodo instancia A:gen_solicitud_altaConcepto, y lo ingertamos en el nodo elemento (E)
            KDL_A_de_gen_solicitud_altaConcepto = domKDL.CreateElement(prefijoDnsKdl, letra_Instancia, DnsKdl);
            KDL_E_de_gen_solicitud_altaConcepto.AppendChild(KDL_A_de_gen_solicitud_altaConcepto);

                // 3.3.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_solicitud_altaConcepto)
            string cualificador_gen_solicitud_altaConcepto = "0"; // esta dentro de un efimero, pero la instancia lo es a un concepto que no lo es
            string resultIngerIdentificador = ingertaIdentificadorDeNodo(domKDL, KDL_A_de_gen_solicitud_altaConcepto, ConceptosConocidos.gen_solicitud_altaConcepto_key, ConceptosConocidos.gen_solicitud_altaConcepto_host, cualificador_gen_solicitud_altaConcepto);
            string resultIngerCtrlConfig = ingertaCtrlConfDeNodo(domKDL, KDL_A_de_gen_solicitud_altaConcepto, ConceptosConocidos.gen_solicitud_altaConcepto_ordinal, ConceptosConocidos.gen_solicitud_altaConcepto_ultimaModif);

                // 3.4.) Generamos el nodo (D) de descripcion de la instancia (A:gen_solicitud_altaConcepto), 
            KDL_D_de_gen_solicitud_altaConcepto = domKDL.CreateElement(prefijoDnsKdl, letra_Descripcion, DnsKdl);
            KDL_A_de_gen_solicitud_altaConcepto.AppendChild(KDL_D_de_gen_solicitud_altaConcepto);

                // 3.5.) Hacemos el nodo E hijo del elemento descripcion (D) de la instancia "gen_solicitudADks" que generamos anteriormente mediante "gen_solicitud_altaConcepto" en el paso 1.)
            KDL_D_de_gen_solicitudADks.AppendChild(KDL_E_de_gen_solicitud_altaConcepto);


            // ////////////////////////////////////////////////////
            // 4.) Generamos el nodo para la instancia de "gen_D"         

                // 4.1.) Generamos el nodo de elemento (E) para la instancia de "gen_D". Que va en la descripcion de la instancia de "gen_solicitud_altaConcepto" que ya hemos generado

            KDL_E_de_gen_D = domKDL.CreateElement(prefijoDnsKdl, letra_Enlace, DnsKdl); //CreateElement(prefijo del dns, nombre etiqueta, nombre del espacio de nombre URL vamos..)

                // 4.2.) Generamos el nodo instancia A:gen_D, y lo ingertamos en el nodo elemento (E)
            KDL_A_de_gen_D = domKDL.CreateElement(prefijoDnsKdl, letra_Instancia, DnsKdl);
            KDL_E_de_gen_D.AppendChild(KDL_A_de_gen_D);

                // 4.3.) Ingertamos el identificador y el control de configuracion del nodo instancia (A:gen_D)
            string cualificador_gen_D = "0"; // esta dentro de un efimero, pero la instancia lo es a un concepto que no lo es
            string resultIngerIdentificador_gen_D = ingertaIdentificadorDeNodo(domKDL, KDL_A_de_gen_D, ConceptosConocidos.gen_D_key, ConceptosConocidos.gen_D_host, cualificador_gen_D);
            string resultIngerCtrlConfig_gen_D = ingertaCtrlConfDeNodo(domKDL, KDL_A_de_gen_D, ConceptosConocidos.gen_D_ordinal, ConceptosConocidos.gen_D_ultimaModif);

                // 4.4.) Generamos el nodo (D) ***1*** de descripcion de la instancia (A:gen_D), La descripcion debe cumplimentarla quien llame a esta funcion, con la descripcion del concepto a dar de alta
            KDL_D_de_gen_D = domKDL.CreateElement(prefijoDnsKdl, letra_Descripcion, DnsKdl);
            KDL_A_de_gen_D.AppendChild(KDL_D_de_gen_D);

                // 4.5.) Hacemos el nodo E hijo del elemento descripcion (D) de la instancia "gen_solicitud_altaConcepto" que generamos anteriormente mediante "gen_solicitud_altaConcepto" en el paso 3.)
            KDL_D_de_gen_solicitud_altaConcepto.AppendChild(KDL_E_de_gen_D);

            if (DatosGlobal.niveDebug > 1000)
            {
                string Xml_en_string = dameStringDeXmlDocument(domKDL);

                Debug.Log(" Desde ScriptLibConceptosXml => ingertaElementoInstancia _ desde el if con . Xml_en_string : " + Xml_en_string);
            }

            // 5.) Si todo ha ido bien, devolvemos el elemento donde debe ponerse la descripcion (D) como instancia del concepto "gen_D". Esta descripcion
            //      debe contener el elemento descripcion D, del concepto que se va a dar de alta. Esta descripcion, normalmente se obtiene mediante la funcion "edicion_dameDescripcion()"
            //      recorriendo el arbol de edicion del concepto en edicion en la interfaz KEE
            return KDL_D_de_gen_D;

        }  // FIn de -  try
        catch
        {
            string resultadoDelIngerto = "ERROR, desde  ScriptLibConceptosXml => ingerta_solicitud_altaConcepto. al generar el nodo elemento (E=>A:gen_solicitud_altaConcepto)." +
                " EL nodo recibido es elemento_recibido.Name = " + elemento_recibido.Name + " - de tipo = " + elemento_recibido.NodeType;
            throw new ArgumentException(resultadoDelIngerto);
        }
    } // Fin de -  public XmlElement ingerta_solicitud_altaConcepto(XmlDocument domKDL, XmlElement elemento_recibido)


    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos especificos para procesar los conceptos XML

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos que genera un concepto de solicitud de busqueda a un DKS. Se le pasa el Key y el host del conceto
    //  y el metodo genera un XML de concepto que es la solicitud, lista para enviarla al DKS corrrespondiente
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-01-21
    // Parametros de entrada :
    //      - key
    //      - host
    // Retorna :
    // Observaciones :
    public string dameConceptoBuskedaKeyHost(string key, string host)
    {
        // Por ahora lo hacemos concatenando los pedazos de string del XML. PENDIENTE luego habra que hacerlo construyendo el 
        // concepto mediante su DOMXML correspondiente (2021-01-29 Miguel Angel Fernandez Graciani)

        string aDevolver = "<?xml version='1.0' encoding='UTF-8'?><kdl:C xmlns:kdl='http://kdlnamespace.ideando.net'><kdl:I><kdl:K>gen_efimero_solicitudADks_0</kdl:K><kdl:H>agenteLocal</kdl:H><kdl:Q>1</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_solicitudADks</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_getDetails</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_K</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:Z><kdl:R><kdl:I><kdl:K>gen_K</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F></kdl:R><kdl:T>";
        aDevolver = aDevolver + key;
        aDevolver = aDevolver + "</kdl:T></kdl:Z></kdl:E></kdl:D></kdl:A></kdl:E><kdl:E><kdl:A><kdl:I><kdl:K>gen_H</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:Z><kdl:R><kdl:I><kdl:K>gen_H</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F></kdl:R><kdl:T>";
        aDevolver = aDevolver + host;
        aDevolver = aDevolver + "</kdl:T></kdl:Z></kdl:E></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:Z><kdl:R><kdl:I><kdl:K>gen_numero</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F></kdl:R><kdl:T>0</kdl:T></kdl:Z></kdl:E></kdl:D></kdl:A></kdl:E><kdl:E><kdl:A><kdl:I><kdl:K>gen_idioma_eapa�ol</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Languajes</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:A><kdl:I><kdl:K>gen_configuracionDeAcceso</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_klw</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D><kdl:E><kdl:Z><kdl:R><kdl:I><kdl:K>gen_numero</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Generic</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F></kdl:R><kdl:T>1</kdl:T></kdl:Z></kdl:E></kdl:D></kdl:A></kdl:E><kdl:E><kdl:A><kdl:I><kdl:K>gen_idioma_ingles</kdl:K><kdl:H>http://" + ConceptosConocidos.localizacionBase_001 + "/klw/dks_Languajes</kdl:H><kdl:Q>0</kdl:Q></kdl:I><kdl:F><kdl:O>0</kdl:O><kdl:M>0</kdl:M></kdl:F><kdl:D></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E></kdl:D></kdl:A></kdl:E></kdl:D></kdl:C>";

        // Antes de enviarlo habria que validarlo con el esquema de KDL. PENDIENTE (MAFG 2021-01-31)

        return aDevolver;
    } // Fin de -  public string dameConceptoBuskedaKeyHost(string key, string host)

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Este metodo devuelve EN STRING, el XML de un DOM
    ///   Autor : 	Miguel Angel Fernandez Graciani
    ///   Fecha :	2021-08-24
    ///   Parametros de entrada :
    ///       - XmlDocument este_XmlDocument : Es el DOM del que queremos obtener el strin XML
    ///   Retorna :
    ///       - String : es el XML del concepto en string
    ///   Observaciones :
    ///         - OJOOOO el string que devuelve del XML, PUEDE LLEVAR COMILLAS DOBLES, por lo que hay que tratarlo como corresponda (normalmente acotarlo con comillas simples)
    /// </summary>
    public string dameStringDeXmlDocument(XmlDocument este_XmlDocument)
    {
        StringWriter stringWriter = new StringWriter();
        XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

        este_XmlDocument.WriteTo(xmlTextWriter);

        return stringWriter.ToString();
    } // Fin de -  public string dameConceptoBuskedaKeyHost(string key, string host)




    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    ///   Metodos que genera un xml de error
    ///   Autor : 	Miguel Angel Fernandez Graciani
    ///   Fecha :	2021-01-31
    ///   Parametros de entrada :
    ///      - funcion
    ///      - codigo
    ///      - mensajeDeError
    ///   Retorna : 
    ///      -  Un XML que incluye informacion de error
    ///   Observaciones : Anota el error en la consola
    /// </summary>
    public string generaXmlDeError(string funcion, int codigo, string mensajeDeError)
    {
        // Por ahora lo hacemos concatenando los pedazos de string del XML. PENDIENTE luego habra que hacerlo construyendo el 
        // concepto mediante su DOMXML correspondiente (2021-01-29 Miguel Angel Fernandez Graciani)

        string aDevolver = "<?xml version='1.0' encoding='UTF-8'?><error>";
        aDevolver = aDevolver + "<funcion> - error generado en la funcion : " + funcion + "</funcion>";
        aDevolver = aDevolver + "<codigo> - codigo de error : " + codigo + "</codigo>";
        aDevolver = aDevolver + "<mensajeDeError> - mensaje de error : " + mensajeDeError + "</mensajeDeError>";
        aDevolver = aDevolver + "</error>";

        return aDevolver;
    } // Fin de -  public string generaXmlDeError(string funcion, int codigo, string mensajeDeError)

    // ///////////////////////////////////////////////////////////////////////////////////////////////
    //   Metodos que genera un DOM por defecto
    // Autor : 	Miguel Angel Fernandez Graciani
    // Fecha :	2021-01-31
    // Parametros de entrada :
    //      - funcion
    //      - codigo
    //      - mensajeDeDOMPorDefecto
    // Retorna : Un objeto DOM que se usara como DOM por defecto
    // Observaciones : Anota el error en la consola
    public XmlDocument generaDomPorDefecto(string funcion, int codigo, string mensajeDeDOMPorDefecto)
    {
        // Por ahora lo hacemos concatenando los pedazos de string del XML. PENDIENTE luego habra que hacerlo construyendo el 
        // concepto mediante su DOMXML correspondiente (2021-01-29 Miguel Angel Fernandez Graciani)

        string xmlPrevio = "<?xml version='1.0' encoding='UTF-8'?><error>";
        xmlPrevio = xmlPrevio + "<funcion> - DOM por fdefecto generado en la funcion : " + funcion + "</funcion>";
        xmlPrevio = xmlPrevio + "<codigo> - codigo de DOM por defecto : " + codigo + "</codigo>";
        xmlPrevio = xmlPrevio + "<mensajeDeDOMPorDefecto> - mensaje de error : " + mensajeDeDOMPorDefecto + "</mensajeDeDOMPorDefecto>";
        xmlPrevio = xmlPrevio + "</error>";

        XmlDocument domADevolver = new XmlDocument();
        domADevolver.PreserveWhitespace = true;
        try
        {
           domADevolver.LoadXml(xmlPrevio);
        }
        catch (System.IO.FileNotFoundException)
        {
            if (DatosGlobal.niveDebug > 90)
            { Debug.Log("Error al generar el DOM por defecto desde generaDomPorDefecto con funcion : " + funcion + " - codigo = " + codigo + " - y mensajeDeDOMPorDefecto : " + mensajeDeDOMPorDefecto); }
        }

        return domADevolver;
    } // Fin de -  public XmlDocument generaDomPorDefecto(string funcion, int codigo, string mensajeDeDOMPorDefecto)



}  // Fin de´- public class ScriptLibConceptosXml : MonoBehaviour {
