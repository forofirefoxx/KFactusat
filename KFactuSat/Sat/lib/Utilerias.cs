using KFactuSat.com.sf.ws.Utilerias;
using System;
using System.IO;
using System.Text;

namespace KFactuSat.Sat.lib
{
    public class Utilerias
    {
        UtileriasPortTypeClient _portClient = null;
        string _user = "testing@solucionfactible.com";
        string _pass = "timbrado.SF.16672";

        public Utilerias(string user, string pass, bool produccion)
        {

            _portClient = (produccion)
                ? WcfConfigFactory.CrearUtileriasProduccion()
                    : WcfConfigFactory.CrearUtileriasTesting();


            _user = user;
            _pass = pass;
        }

        public ResponseTimbres getTimbres(string rfcEmisor)
        {
           var response = _portClient.getTimbres(_user, _pass, rfcEmisor);

            return response;
        }
        public ResponseBuscar buscar(ParametrosBuscar parametros)
        {
            var response = _portClient.buscar(_user, _pass, parametros);

            return response;
        }

        public ResponseFecha getFechaCorte()
        {
            var response = _portClient.getFechaCorte(_user, _pass);

            return response;
        }
    }
}
