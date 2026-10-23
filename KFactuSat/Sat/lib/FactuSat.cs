
using System;
using System.Text;
using System.Xml.Serialization;
using KFactuSat.Sat.utils;
using System.Xml;
using System.IO;
using KFactuSat.Sat.anexo.CFDV40.CartaPorte20;
using System.Xml.Xsl;
using System.Collections.Generic;
using KFactuSat.Sat.lib;
using System.Security.Cryptography.X509Certificates;

namespace KFactuSat.Sat
{
    public class FactuSat
    {
        public string pathcer { get; set; }
        public string pathkey { get; set; }
        public string pathxslt { get; set; }
        public string claveprivada { get; set; }
        /// <summary>
        /// por defecto tiene el algoritmo en SHA256withRSA
        /// visita la pagina de bounceCastle 
        /// </summary>
        public string algoritmo { get; set; } = "SHA256withRSA";
        /// <summary>
        /// Para sellar es necesario que esten llenos los campos pathcer,pathkey,pathxslt,claveprivada
        /// </summary>
        private SelloDigital selloDigital = new SelloDigital();

        /// <summary>
        /// <para>SerializarXML te generar el archivo como el SAT lo requiere. </para>
        /// <para>Recuerda que el sat pide requerimientos minimos para una factura.</para>
        /// <para>Para mas información de es que pide el comprobante entra aqui http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/GuiallenadoCFDIglobal311221.pdf </para>
        /// 
        /// </summary> 
        /// <param name="comprobante">requerido</param>
        /// <param name="namespaces" > opcional por defecto se te agregaran los namespaces cfdi,tfd,xsi </param>
        /// <returns></returns>

        public string SerializarXML<T>(T comprobante, XmlSerializerNamespaces namespaces = null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;

            if (namespaces == null)
            {
                namespaces = new XmlSerializerNamespaces();
                namespaces.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            }

            using (var sw = new StringWriterWithEncoding(Encoding.UTF8))
            using (var writer = XmlWriter.Create(sw, settings))
            {
                serializer.Serialize(writer, comprobante,namespaces);
                return sw.ToString();
            }
        }
        public string GenerarCFDI<T>(T comprobante, XmlSerializerNamespaces namespaces)
        {
            // 1. XML sin sello
            string xml = SerializarXML(comprobante,namespaces);

            // 2. Cadena original
            string cadenaOriginal = getCadenaOriginal(xml);

            // 3. Asignar sello y certificado
            dynamic comp = comprobante;
            comp.Certificado = Certificado();
            comp.Sello = Sellar(cadenaOriginal, algoritmo);

            // 4. XML final
            string xmlFinal = SerializarXML(comp);

            return xmlFinal;
        }

        /// <summary>
        /// Deserializa XML a objecto
        /// </summary>
        /// <param name="xml">string XML </param>
        /// <param name="modo">modo en que deserializara el xml si path o string  </param>
        /// <returns></returns>
        public T DeserializarXML<T>(string xml, ModoDeserializado modo)
        {
            string str = "";
            if (modo == ModoDeserializado.pathXML)
                str = File.ReadAllText(xml);
            if (modo == ModoDeserializado.stringXML)
                str = xml;

            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(str))
            {
              
                return (T)ser.Deserialize(sr);
            }
        }
        public T DeserializarXML<T>(byte[] bufferXML)
        {

            XmlSerializer ser = new XmlSerializer(typeof(T));
            Stream mem = new MemoryStream(bufferXML);
            return (T)ser.Deserialize(mem);
        }

        public XmlElement SerializeToXmlDocument(object obj, XmlSerializerNamespaces namespaces = null, bool removeAllAttr = false)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType());

            XmlDocument xd = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, obj, namespaces);
                memStm.Position = 0;
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }
            if (removeAllAttr)
                xd.DocumentElement.RemoveAllAttributes();

            return xd.DocumentElement;
        }
        /// <summary>
        /// Funcion que agregara la carta porte al comprobante
        /// </summary>
        /// <param name="comprobante"></param>
        /// <param name="cartaPorte"></param>
        /// <returns></returns>
        public anexo.CFDV40.Comprobante AgregarCartaPorte(dynamic comprobante, CartaPorte cartaPorte, XmlSerializerNamespaces namespaces = null)
        {
            List<XmlElement> comprobanteComplementos = new List<XmlElement>(); ;
            if (comprobante.Complemento != null)
            {
                comprobanteComplementos = new List<XmlElement>(comprobante.Complemento.Any);
            }
            dynamic cc = null;
            comprobanteComplementos.Add(SerializeToXmlDocument(cartaPorte, namespaces));
            cc.Any = comprobanteComplementos.ToArray();
            comprobante.Complemento = cc;

            return comprobante;
        }

        //public Comprobante AgregarPago(Comprobante comprobante, anexo.CFDV33.Pago10.Pagos pagos, XmlSerializerNamespaces namespaces = null)
        //{
        //    List<XmlElement> comprobanteComplementos = new List<XmlElement>(); ;
        //    if (comprobante.Complemento != null)
        //    {
        //        comprobanteComplementos = new List<XmlElement>(comprobante.Complemento.Any);
        //    }
        //    ComprobanteComplemento cc = new ComprobanteComplemento();
        //    comprobanteComplementos.Add(SerializeToXmlDocument(pagos, namespaces));
        //    cc.Any = comprobanteComplementos.ToArray();
        //    comprobante.Complemento = cc;

        //    return comprobante;
        //}
        public anexo.CFDV33.Comprobante AgregarPago10(anexo.CFDV33.Comprobante comprobante, anexo.CFDV33.Pago10.Pagos pagos, XmlSerializerNamespaces namespaces = null)
        {
            List<XmlElement> comprobanteComplementos = new List<XmlElement>(); ;
            if (comprobante.Complemento != null)
            {
                comprobanteComplementos = new List<XmlElement>(comprobante.Complemento[0].Any);
            }
            anexo.CFDV33.ComprobanteComplemento cc = new anexo.CFDV33.ComprobanteComplemento();
            comprobanteComplementos.Add(SerializeToXmlDocument(pagos, namespaces));
            cc.Any = comprobanteComplementos.ToArray();
            comprobante.Complemento = new anexo.CFDV33.ComprobanteComplemento[] { cc };

            return comprobante;
        }
        public anexo.CFDV40.Comprobante AgregarPago20(anexo.CFDV40.Comprobante comprobante, anexo.CFDV40.Pago20.Pagos pagos, XmlSerializerNamespaces namespaces = null)
        {
            List<XmlElement> comprobanteComplementos = new List<XmlElement>(); ;
            if (comprobante.Complemento != null)
            {
                comprobanteComplementos = new List<XmlElement>(comprobante.Complemento.Any);
            }
            anexo.CFDV40.ComprobanteComplemento cc = new anexo.CFDV40.ComprobanteComplemento();
            comprobanteComplementos.Add(SerializeToXmlDocument(pagos, namespaces));
            cc.Any = comprobanteComplementos.ToArray();
            comprobante.Complemento = cc;

            return comprobante;
        }
        /// <summary>
        /// Retorna el certificado con la extension .cer
        /// </summary>
        /// <returns></returns>
        public string getNumCertificado()
        {
            string error = "";

            if (filters.isValidCer(pathcer, out error))
            {
                string numeroCertificado, i, f, s;
                SelloDigital.leerCER(pathcer, out i, out f, out s, out numeroCertificado);
                return numeroCertificado;
            }
            else
            {
                error = string.Format("No se pudo realizar el Sellado Digital, se requiere: {0}", error);
                throw new NullReferenceException(error);
            }

        }
        public string getCadenaOriginal(string xml)
        {
            var settings = new XsltSettings(true, true);
            var resolver = new XmlUrlResolver();
            XslCompiledTransform transformador = new XslCompiledTransform();


            transformador.Load(pathxslt,settings,resolver);

            using (var sr = new StringReader(xml))
            using (var xr = XmlReader.Create(sr))
            using (var sw = new StringWriter())
            using (var xw = XmlWriter.Create(sw, transformador.OutputSettings))
            {
                transformador.Transform(xr, xw);
                return sw.ToString().Trim();
            }

        }
        private string Certificado()
        {
            return selloDigital.Certificado(pathcer);
        }
        private string Sellar(string cadena, string algoritmo )
        {
            return selloDigital.Sellar(cadena, pathkey, claveprivada,algoritmo);
        }

        public string SellarSolicitudCancelacion(byte[] certificate,string pass, string rfcEmisor, DateTime fecha, string uuid, string motivo, string folioSustitucion = null)
        {
            return selloDigital.GenerateCancelacion(certificate,pass,  rfcEmisor,  fecha,  uuid,  motivo,  folioSustitucion);
        }

    }
}
