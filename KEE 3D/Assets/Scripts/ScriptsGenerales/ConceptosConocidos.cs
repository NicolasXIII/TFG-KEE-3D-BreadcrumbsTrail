using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// //////////////////////////////////////////////////////////////////////////////////////
/// ///////////  Script para almacenar los conceptos que esta interfaz conoce
/// Autor : 	Miguel Angel Fernandez Graciani
/// Fecha :	2021-05-16
/// Observaciones :
/// </summary>
public class ConceptosConocidos : MonoBehaviour
{

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Constantes para cambios de localizacion de conceptos y recursos


    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Conceptos referentes a localizacion de DKSs

    public static string gen_dks_basico_key = "gen_esteDks";  // Es el Key del concepto  Identificador
    public static string gen_dks_basico_host = "http://" + DatosGlobal.localizacionBase_001 + "/klw/dks_basico";  //Es el host del DKSs, coincide con su localizacion

    public static string gen_dks_desarrollo_key = "gen_esteDks";  // Es el Key del concepto  Identificador
    public static string gen_dks_desarrollo_host = "http://" + DatosGlobal.localizacionBase_001 + "/klw/dks_desarrollo";  // Es el host del DKSs, coincide con su localizacion

    public static string gen_dks_Generic_key = "gen_esteDks";  // Es el Key del concepto  Identificador
    public static string gen_dks_Generic_host = "http://" + DatosGlobal.localizacionBase_001 + "/klw/dks_Generic";  // Es el host del DKSs, coincide con su localizacion

    public static string gen_dks_klw_key = "gen_esteDks";  // Es el Key del concepto  Identificador
    public static string gen_dks_klw_host = "http://" + DatosGlobal.localizacionBase_001 + "/klw/dks_klw";  // Es el host del DKSs, coincide con su localizacion

    public static string gen_dks_Languajes_key = "gen_esteDks";  // Es el Key del concepto  Identificador
    public static string gen_dks_Languajes_host = "http://" + DatosGlobal.localizacionBase_001 + "/klw/dks_Languajes";  // Es el host del DKSs, coincide con su localizacion

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Conceptos referentes a ELEMENTOS KLW

    // Para los conceptos de la estructura de KDL

    public static string gen_I_key = "gen_I";  // Es el Key del concepto  Identificador
    public static string gen_I_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto   Identificador
        public static string gen_K_key = "gen_K";  // Es el key del concepto key que se usa por ejemplo para definir el key y host de un conceto a buscar
        public static string gen_K_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto key que se usa por ejemplo para definir el key y host de un conceto a buscar
        public static string gen_H_key = "gen_H";  // Es el key del concepto host que se usa por ejemplo para definir el key y host de un conceto a buscar
        public static string gen_H_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto host que se usa por ejemplo para definir el key y host de un conceto a buscar

    public static string gen_F_key = "gen_F";  // Es el Key del concepto  control de configuracion
    public static string gen_F_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto control de configuracion
        public static string gen_O_key = "gen_O";  // Es el Key del concepto Ordinal
        public static string gen_O_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Ordinal
        public static string gen_M_key = "gen_M";  // Es el Key del concepto ultima modificacion
        public static string gen_M_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto ultima modificacion

    public static string gen_P_key = "gen_P";  // Es el Key del concepto Elemento de ayuda a interfaz
    public static string gen_P_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento de ayuda a interfaz
        public static string gen_L_key = "gen_L";  // Es el Key del concepto Elemento Idioma 
        public static string gen_L_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento Idioma 

    public static string gen_D_key = "gen_D";  // Es el Key del concepto  
    public static string gen_D_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto 
    public static string gen_D_Cualificador = "0";  // se pone a 0 por defecto, si fuera la raiz de un efimero habria que ponerlo a 1 o segun proceda
    public static string gen_D_ordinal = "0";
    public static string gen_D_ultimaModif = "0";



    public static string gen_E_key = "gen_E";  // Es el Key del concepto Elemento Elemento
        public static string gen_E_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto  Elemento Elemento
            public static string gen_A_key = "gen_A";  // Es el Key del concepto Elemento Instancia 
            public static string gen_A_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento Instancia
            public static string gen_R_key = "gen_R";  // Es el Key del concepto Elemento Referencia
            public static string gen_R_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento Referencia
            public static string gen_Z_key = "gen_Z";  // Es el Key del concepto Elemento SIn Techo 
            public static string gen_Z_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento SIn Techo
                public static string gen_T_key = "gen_T";  // Es el Key del concepto Elemento Dato de Sin Techo 
                public static string gen_T_host = ConceptosConocidos.gen_dks_klw_host;  // Es el host del concepto Elemento Dato de Sin Techo


    //  CONCEPTO NUEVO. Se utiliza para generar un concepto nuevo. Lo genero trayendolo del DKS de KLW
    //  porque prefiero que sea el DKS de KLW quien lo actualice y las distintas interfaces mamen de este 
    //  concepto. De esta menera, si hay cambios en lo que seria un concepto recien creado, estos se hacen en el 
    //  concepto del DKS de KLW y cualquiera que lo pida, tendrá la última version
    public static string gen_NuevoConcepto_Key = "gen_ParaConceptoNuevo";
    public static string gen_NuevoConcepto_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_NuevoConcepto_cualifi = "0";
    public static string gen_NuevoConcepto_ordinalConf = "0";
    public static string gen_NuevoConcepto_ultiModConfEnString = "0";

    // ///////////////////////////////////////////////////////////////////
    // Conceptos referentes a TIPOS DE DATOS SIN TECHO
    // gen_solicitudADks
    public static string gen_solicitudADks_key = "gen_solicitudADks";
    public static string gen_solicitudADks_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_solicitudADks_Cualificador = "0";  // se pone a 0 por defecto, si fuera la raiz de un efimero habria que ponerlo a 1 o segun proceda
    public static string gen_solicitudADks_ordinal = "0";
    public static string gen_solicitudADks_ultimaModif = ConceptosConocidos.gen_dks_klw_host;
        // gen_solicitudADks
    public static string gen_solicitud_altaConcepto_key = "gen_solicitud_altaConcepto";
    public static string gen_solicitud_altaConcepto_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_solicitud_altaConcepto_Cualificador = "0";  // se pone a 0 por defecto, si fuera la raiz de un efimero habria que ponerlo a 1 o segun proceda
    public static string gen_solicitud_altaConcepto_ordinal = "0";
    public static string gen_solicitud_altaConcepto_ultimaModif = ConceptosConocidos.gen_dks_klw_host;

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Conceptos referentes a TIPOS DE DATOS SIN TECHO

    //  TEXTO PLANO como tipo de dato sin techo => Se abre el texto (contenido en el elemento T hijo de Z de KDL) en el cambas general
    public static string gen_tipoDeSinTechoTextoPlano_Key = "gen_tipoDeSinTechoTextoPlano";
    public static string gen_tipoDeSinTechoTextoPlano_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTechoTextoPlano_cualifi = "0";

    //  NUMERO ENTERO (numero entero) como tipo de dato sin techo = => Se abre el texto con el numero entero que contiene (contenido en el elemento T hijo de Z de KDL) en el cambas general
    public static string gen_tipoDeSinTecho_NumeroEntero_Key = "gen_tipoDeSinTecho_NumeroEntero";
    public static string gen_tipoDeSinTecho_NumeroEntero_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_NumeroEntero_cualifi = "0";

    //  URL (localizacion en internet) como tipo de dato sin techo = => Se abre la url (contenida como texto en el elemento T hijo de Z de KDL) en el navegador por defecto del sistema
    public static string gen_tipoDeSinTecho_Url_Key = "gen_tipoDeSinTecho_Url";
    public static string gen_tipoDeSinTecho_Url_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_Url_cualifi = "0";

    //  FICHERO GENERICO (fichero generico) como tipo de dato sin techo => Se abre el fichero con una u otra aplicacion del sistema, segun la extension del nombre del fichero
    public static string gen_tipoDeSinTecho_FicheroGenerico_Key = "gen_tipoDeSinTecho_FicheroGenerico";
    public static string gen_tipoDeSinTecho_FicheroGenerico_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_FicheroGenerico_cualifi = "0";

    //  FICHERO DE AUDIO (ficherode de audio) como tipo de dato sin techo => Se abre el fichero de audio con una u otra aplicacion del sistema, segun la extension del nombre del fichero
    public static string gen_tipoDeSinTecho_FicheroAudio_Key = "gen_tipoDeSinTecho_FicheroAudio";
    public static string gen_tipoDeSinTecho_FicheroAudio_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_FicheroAudio_cualifi = "0";

    //  FICHERO DE IMAGEN (ficherode de imagen) como tipo de dato sin techo => Se abre el fichero de imagen con una u otra aplicacion del sistema, segun la extension del nombre del fichero
    public static string gen_tipoDeSinTecho_FicheroImagen_Key = "gen_tipoDeSinTecho_FicheroImagen";
    public static string gen_tipoDeSinTecho_FicheroImagen_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_FicheroImagen_cualifi = "0";

    //  FICHERO DE VIDEO (ficherode de video) como tipo de dato sin techo => Se abre el fichero de video con una u otra aplicacion del sistema, segun la extension del nombre del fichero
    public static string gen_tipoDeSinTecho_FicheroVideo_Key = "gen_tipoDeSinTecho_FicheroVideo";
    public static string gen_tipoDeSinTecho_FicheroVideo_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_FicheroVideo_cualifi = "0";

    //  LLAMADA A FUNCION DE KEE (llamada a funcion KEE) como tipo de dato sin techo => Se llama a la funcion cuyo nombre coincide con el contenido del elemento T del sin techo
    public static string gen_tipoDeSinTecho_LlamadaFuncionKEE_Key = "gen_tipoDeSinTecho_LlamadaFuncionKEE";
    public static string gen_tipoDeSinTecho_LlamadaFuncionKEE_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_tipoDeSinTecho_LlamadaFuncionKEE_cualifi = "0";
        // Funciones de KEE que pueden ser llamadas desde el tipo "gen_tipoDeSinTecho_LlamadaFuncionKEE"
            //  LLAMADA A FUNCION DE KEE (llamada a funcion KEE) como tipo de dato sin techo => Se llama a la funcion cuyo nombre coincide con el contenido del elemento T del sin techo
            public static string funcionKEE_ActivadoBoton_BuscadorKee_por_key_host = "funcionKEE_ActivadoBoton_BuscadorKee_por_key_host";


    //  URL (fichero de extension .jpg) como tipo de dato sin techo = => Se abre el fichero (contenida como texto en el elemento T hijo de Z de KDL) en el navegador por defecto del sistema
    // OJOOOO Falta darlo de alta en el DKS de KLW (PENDIENTE MAFG 2021-07-15)
    public static string gen_tipoDeSinTecho_ext_jpg_Key = "gen_tipoDeSinTecho_ext_jpg"; // OJOOOOO debe coincidir con el key en la base de datos del DKS de KLW (todabia no existe MAFG PENDIENTE 2021-07-16)
    public static string gen_tipoDeSinTecho_ext_jpg_host = "http://" + DatosGlobal.localizacionBase_001 + "/dks_klw";
    //    public static string gen_tipoDeSinTecho_ext_jpg_cualifi = "0";

    // ///////////////////////////////////////////////////////////////////
    // ///////////////////////////////////////////////////////////////////
    // Conceptos referentes a ERRORES DE PROCDESO

    //  Error KLW generico 
    public static string gen_errorKLW_Key = "gen_tipoDeSinTechoTextoPlano";
    public static string gen_errorKLW_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_errorKLW_cualifi = "0";
    public static string gen_errorKLW_ordinal = "0";
    public static string gen_errorKLW_ultiMod = "0";

    //  Error KLW por fallo en tipo de datos de un elemento Sin Techo 
    public static string gen_errorKLW_en_tipo_de_dato_sin_techo_Key = "gen_errorKLW_en_tipo_de_dato_sin_techo";
    public static string gen_errorKLW_en_tipo_de_dato_sin_techo_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_errorKLW_en_tipo_de_dato_sin_techo_cualifi = "0";
    public static string gen_errorKLW_en_tipo_de_dato_sin_techo_ordinal = "0";
    public static string gen_errorKLW_en_tipo_de_dato_sin_techo_ultiMod = "0";


    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // //////////////////////////////////////////////////////////////////////////
    // ///  CONCEPTOS QUE SE USAN EN LA INTERFAZ

    // //////////////////////////////////////////////////////////////////////////
    // ///  en http://www.ideando.net/klw/dks_klw

      // Muro de la interfaz KEE3D
    public static string gen_muro_Key = "gen_muro";
    public static string gen_muro_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_muro_cualifi = "0";
    public static string gen_muro_ordinal = "0";
    public static DateTime gen_muro_ultiMod = new DateTime(0);

    // Lista de usuario para idiomas de interfaz KEE3D
    public static string gen_listaDeIdiomasDeInterfaz_Key = "gen_listaDeIdiomasDeInterfaz";
    public static string gen_listaDeIdiomasDeInterfaz_host = ConceptosConocidos.gen_dks_klw_host;
    public static string gen_listaDeIdiomasDeInterfaz_cualifi = "0";
    public static string gen_listaDeIdiomasDeInterfaz_ordinal = "0";
    public static DateTime gen_listaDeIdiomasDeInterfaz_ultiMod = new DateTime(0);

}  // Fin de - public class ConceptosConocidos : MonoBehaviour

