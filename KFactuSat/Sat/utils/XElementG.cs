using KFactuSat.Sat.anexo;
using System;
using System.Xml;

namespace KFactuSat.Sat.utils
{
    public class XElementG
    {
        private anexo.CFDV40.Comprobante comprobante40;
        private anexo.CFDV33.Comprobante comprobante33;
        private int _version = 3;
        public XElementG(anexo.CFDV40.Comprobante comprobante)
        {
            this.comprobante40 = comprobante;
            this._version = 4;
        }
        public XElementG(anexo.CFDV33.Comprobante comprobante)
        {
            this.comprobante33 = comprobante;
            this._version = 3;
        }
        public string getComplementoOuterXML(string nombreComplemento)
        {
            if (this._version == 3)
            {
                var complemento = this.comprobante33.Complemento;
                for (int i = 0; i < complemento[0].Any.Length; i++)
                {
                    var c = complemento[0].Any[i];

                    var item = c;

                    if (item.LocalName.ToLower() == nombreComplemento.ToLower())
                        return item.OuterXml;

                }
            }
            if (this._version == 4)
            {
                var complemento = this.comprobante40.Complemento;
                for (int i = 0; i < complemento.Any.Length; i++)
                {
                    var item = complemento.Any[i];
                    if (item.LocalName.ToLower() == nombreComplemento.ToLower())
                        return item.OuterXml;

                }
            }

            return "<?xml version=\"1.0\" encoding=\"UTF - 8\"?><" + nombreComplemento + ">No es encontró el complemento que buscabas</" + nombreComplemento + ">";
        }
        public TimbreFiscalDigital getTimbradoFiscalDigital()
        {
            string nombreComplemento = "timbrefiscaldigital";
           
            if (this._version == 3)
            {
                var complemento = this.comprobante33.Complemento;
                FactuSat xmlsat = new FactuSat();
                if (complemento == null)
                    return new TimbreFiscalDigital();

                for (int i = 0; i < complemento[0].Any.Length; i++)
                {
                    var item = complemento[0].Any[i];
                    if (item.LocalName.ToLower() == nombreComplemento.ToLower())
                        return xmlsat.DeserializarXML<TimbreFiscalDigital>(item.OuterXml, ModoDeserializado.stringXML);
                }
                return new TimbreFiscalDigital();
            }
            if (this._version == 4)
            {
                var complemento = this.comprobante40.Complemento;
                FactuSat xmlsat = new FactuSat();
                if (complemento == null)
                    return new TimbreFiscalDigital();

                for (int i = 0; i < complemento.Any.Length; i++)
                {
                    var item = complemento.Any[i];
                    if (item.LocalName.ToLower() == nombreComplemento.ToLower())
                        return xmlsat.DeserializarXML<TimbreFiscalDigital>(item.OuterXml, ModoDeserializado.stringXML);

                }
                return new TimbreFiscalDigital();
            }
            return new TimbreFiscalDigital();
        }

    }
}
