using KFactuSat.com.sf.ws.Timbrado;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KFactuSat.Sat.lib
{
    public class Timbrado
    {
        TimbradoPortTypeClient _portClient = null;
        string _user = "testing@solucionfactible.com";
        string _pass = "timbrado.SF.16672";

        public Timbrado(string user, string pass, bool produccion) {

            _portClient = (produccion)
                ? WcfConfigFactory.CrearTimbradoProduccion()
                    : WcfConfigFactory.CrearTimbradoTesting();



            _user = user;
            _pass = pass;
        }

        public CFDIResultadoCertificacion timbrarCFDI(string xml, ETipoXML eTipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                    throw new ArgumentException("El XML no puede estar vacío");


                byte[] bytesXML;

                if (eTipo == ETipoXML.path)
                {
                    if (!File.Exists(xml))
                        throw new FileNotFoundException("No se encontró el archivo XML", xml);

                    bytesXML = File.ReadAllBytes(xml);
                }
                else
                {
                    bytesXML = Encoding.UTF8.GetBytes(xml);
                }

                var response = _portClient.timbrar(_user, _pass, bytesXML, false);

                if (response == null)
                    throw new Exception("[CFDI] El PAC no devolvió respuesta");

                if (response.resultados.Length > 0)
                    if (response.resultados[0].status!=200)
                        throw new Exception(response.resultados[0].mensaje);


                return response.resultados[0];
            }
            catch (TimeoutException ex)
            {
                throw new Exception("[CFDI] Timeout al timbrar CFDI con el PAC", ex);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new Exception("[CFDI] El servicio de timbrado no está disponible", ex);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                throw new Exception("[CFDI] Error de comunicación con el PAC", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[CFDI]: {0}", ex.Message));
            }
        }
        public async Task<cancelarResponse> CancelarCFDIAsync(string certpath, string keypath, string passcsd, string[] uuids)
        {

            try
            {
                if (uuids == null || uuids.Length == 0)
                    throw new ArgumentException("[CFDI Cancelación] Debe enviar al menos un UUID.");

                if (!File.Exists(certpath))
                    throw new FileNotFoundException("[CFDI Cancelación] No se encontró el archivo .cer", certpath);

                if (!File.Exists(keypath))
                    throw new FileNotFoundException("[CFDI Cancelación] No se encontró el archivo .key", keypath);


                // 🔹 Leer certificados
                var cerbytes = File.ReadAllBytes(certpath);
                var keybytes = File.ReadAllBytes(keypath);

                // 🔹 Llamada al PAC
                var response = await _portClient.cancelarAsync(
                    _user,
                    _pass,
                    uuids,
                    cerbytes,
                    keybytes,
                    passcsd
                );

                if (response == null)
                    throw new Exception("[CFDI Cancelación] El PAC no devolvió respuesta al cancelar CFDI.");

                return response;
            }
            catch (TimeoutException ex)
            {
                throw new Exception("[CFDI Cancelación] Timeout al cancelar CFDI con el PAC.", ex);
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                throw new Exception("[CFDI Cancelación] El servicio del PAC no está disponible.", ex);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                throw new Exception("[CFDI Cancelación] Error de comunicación con el PAC.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("[CFDI Cancelación] "+ex.Message, ex);
            }
        }
        public string CFDIBytesToString(byte[] cfdi)
        {
            return Encoding.UTF8.GetString(cfdi);
        }
    }
    public enum ETipoXML
    {
        xml=0,
        path=1
    }
}
