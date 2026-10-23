namespace KFactuSat.Sat.anexo
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital", IsNullable = false)]
    public partial class TimbreFiscalDigital
    {
        [System.Xml.Serialization.XmlAttribute("SelloSAT")]
        public string SelloSAT { get; set; }

        [System.Xml.Serialization.XmlAttribute("NoCertificadoSAT")]
        public string NoCertificadoSAT { get; set; }

        [System.Xml.Serialization.XmlAttribute("SelloCFD")]
        public string SelloCFD { get; set; }

        [System.Xml.Serialization.XmlAttribute("RfcProvCertif")]
        public string RfcProvCertif { get; set; }
        [System.Xml.Serialization.XmlAttribute("FechaTimbrado")]
        public string FechaTimbrado { get; set; }

        [System.Xml.Serialization.XmlAttribute("UUID")]
        public string UUID { get; set; }

        [System.Xml.Serialization.XmlAttribute("Version")]
        public string Version { get; set; }
    }
}
