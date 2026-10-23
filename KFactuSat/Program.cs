using KFactuSat.Sat;
using KFactuSat.Sat.anexo.CFDV40;
using KFactuSat.Sat.lib;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KFactuSat
{
    class program
    {
        static void Main(string[] args)
        {
            var user = "testing@solucionfactible.com";
            var pass = "timbrado.SF.16672";
            var pathcer = "";
            var pathkey = "";
            var pathxslt = "C:\\certi\\cadenaoriginal_4_0.local.xslt";
            var claveprivada = "";

            var sat = new FactuSat();
            sat.pathcer = pathcer;
            sat.pathkey = pathkey;
            sat.pathxslt = pathxslt;
            sat.claveprivada = claveprivada;

            var comprobante = new Comprobante
            {
                Serie = "T",
                Folio = "TE99",
                Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                SubTotal = 1000m,
                Moneda = "MXN",
                Total = 1160m,
                TipoDeComprobante = "I",
                MetodoPago = "PUE",
                NoCertificado = sat.getNumCertificado(),
                FormaPago = "03",
                LugarExpedicion = "26000",
                Exportacion = "01",
                Emisor = new ComprobanteEmisor
                {
                    Rfc = "FSE040720Q37",
                    Nombre = "FLEET SERVICE",
                    RegimenFiscal = "601"
                },

                Receptor = new ComprobanteReceptor
                {
                    Rfc = "TES900806IS1",
                    Nombre = "TRANS-ENERGETICOS",
                    DomicilioFiscalReceptor = "67480",
                    RegimenFiscalReceptor = "624",
                    UsoCFDI = "G03"
                },

                Conceptos = new[]
    {
        new ComprobanteConcepto
        {
            ClaveProdServ = "01010101",
            Cantidad = 1,
            ClaveUnidad = "H87",
            Descripcion = "Servicio de software",
            ValorUnitario = 1000m,
            Importe = 1000m,
            ObjetoImp = "02",

            Impuestos = new ComprobanteConceptoImpuestos
            {
                Traslados = new[]
                {
                    new ComprobanteConceptoImpuestosTraslado
                    {

                        Base = 1000m,
                        Impuesto = "002",
                        TipoFactor = "Tasa",
                        TasaOCuota = 0.160000m,
                        Importe = 160m
                    }
                }
            }
        }
    },

                Impuestos = new ComprobanteImpuestos
                {
                    TotalImpuestosTrasladados = 160m,
                    Traslados = new[]
        {
            new ComprobanteImpuestosTraslado
            {
                Base = 1000m,
                Impuesto = "002",
                TipoFactor = "Tasa",
                TasaOCuota = 0.160000m,
                Importe = 160m
            }
        }
                }
            };

            //        var NC = new Comprobante
            //        {
            //            Serie = "NC",
            //            Folio = "NC-001",
            //            Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            //            SubTotal = 1000m,
            //            Moneda = "MXN",
            //            Total = 1160m,
            //            TipoDeComprobante = "E",
            //            MetodoPago = "PUE",
            //            NoCertificado = sat.getNumCertificado(),
            //            FormaPago = "03",
            //            LugarExpedicion = "26000",
            //            Exportacion = "01",

            //            // 🔹 Relación con CFDI original
            //            CfdiRelacionados = new ComprobanteCfdiRelacionados[] {
            //            new ComprobanteCfdiRelacionados
            //                {
            //                    TipoRelacion = "01", // Nota de crédito
            //                    CfdiRelacionado = new[]
            //                    {
            //                        new ComprobanteCfdiRelacionadosCfdiRelacionado
            //                        {
            //                            UUID = "UUID-DE-LA-FACTURA-ORIGINAL"
            //                        }
            //                    }
            //                }
            //            },

            //            Emisor = new ComprobanteEmisor
            //            {
            //                Rfc = "FSE040720Q37",
            //                Nombre = "FLEET SERVICE",
            //                RegimenFiscal = "601"
            //            },

            //            Receptor = new ComprobanteReceptor
            //            {
            //                Rfc = "TES900806IS1",
            //                Nombre = "TRANS-ENERGETICOS",
            //                DomicilioFiscalReceptor = "67480",
            //                RegimenFiscalReceptor = "624",
            //                UsoCFDI = "G02" // 🔹 Importante para notas de crédito
            //            },

            //            Conceptos = new[]
            //{
            //    new ComprobanteConcepto
            //    {
            //        ClaveProdServ = "01010101",
            //        Cantidad = 1,
            //        ClaveUnidad = "H87",
            //        Descripcion = "Nota de crédito por servicio de software",
            //        ValorUnitario = 1000m,
            //        Importe = 1000m,
            //        ObjetoImp = "02",

            //        Impuestos = new ComprobanteConceptoImpuestos
            //        {
            //            Traslados = new[]
            //            {
            //                new ComprobanteConceptoImpuestosTraslado
            //                {
            //                    Base = 1000m,
            //                    Impuesto = "002",
            //                    TipoFactor = "Tasa",
            //                    TasaOCuota = 0.160000m,
            //                    Importe = 160m
            //                }
            //            }
            //        }
            //    }
            //},

            //            Impuestos = new ComprobanteImpuestos
            //            {
            //                TotalImpuestosTrasladados = 160m,
            //                Traslados = new[]
            //    {
            //        new ComprobanteImpuestosTraslado
            //        {
            //            Base = 1000m,
            //            Impuesto = "002",
            //            TipoFactor = "Tasa",
            //            TasaOCuota = 0.160000m,
            //            Importe = 160m
            //        }
            //    }
            //            }
            //        };

            try
            {
                #region Timbrado y Cancelacion
                var random = new Random();
                //Console.WriteLine(random.Next()) ;
                //Console.WriteLine(random.Next());
                //Console.ReadKey();
                //for (int i = 0; i < 10; i++)
                //{
                //    comprobante.Folio = "T"+random.Next();
                //var selladoXML = sat.GenerarCFDI(comprobante, null);
                //var timbrado = new Timbrado(user, pass, false);
                //Task.Run(() =>
                //{
                //    var timbradoXML = timbrado.timbrarCFDI(selladoXML, ETipoXML.xml);

                //});
                //Thread.Sleep(500);
                //throw new Exception("o chit");
                //}



                //File.WriteAllText(@"C:\certi\nueva.xml", timbradoXMLstr);


                //cancelar(pathcer, pathkey, claveprivada, timbrado, timbradoXML).GetAwaiter().GetResult();

                #endregion
                #region GetTimbres

                //var utilerias = new Utilerias(user, pass, false);
                //var disponibles = utilerias.getTimbres("CASR900401KL5");

                //Console.WriteLine(disponibles.disponibles);
                //Console.WriteLine(disponibles.contratados);

                #endregion

                #region Busqueda
                //var xml = File.ReadAllText(@"C:\Users\hiram.sanchez\Desktop\55574ecc544f9a564d44bcc3ee1822759f8c.xml");
                //string cadenaOriginal = sat.getCadenaOriginal(xml);

       

                //get(api, url).GetAwaiter().GetResult();


                //var utilerias = new Utilerias(user, pass, false);
                //var response = utilerias.buscar(new com.sf.ws.Utilerias.ParametrosBuscar()
                //{
                //    serie = "CP",
                //    emisorRFC = "FSE040720Q37"

                //});

                //if (response.cfdis == null)
                //{
                //    Console.WriteLine("no hay cfdis");

                //}
                //else
                //{
                //    for (int i = 0; i < response.cfdis.Length; i++)
                //    {
                //        var item = response.cfdis[i];
                //        Console.WriteLine("Serie {0}, Folio: {1}, UUID {2}, emisor:{3}", item.serie, item.folio, item.uuid, item.emisorRFC);
                //    }
                //}




                #endregion

                //#region Corte de timbrado
                ////var utilerias = new Utilerias(user, pass, false);
                //var responsedt = utilerias.getFechaCorte();

                //Console.WriteLine(responsedt.fecha);


                //#endregion
                //            var archivos = Directory.EnumerateFiles(@"C:\repos\a_TEST\Proyectos\amher.fleet.API\Imagenes\FleetService\Facturas\CFC\XML","*.xml");

                //            var uuidsArchivos = archivos
                //.Select(x => Path.GetFileName(x))
                //.ToList();

                //            Console.WriteLine(uuidsArchivos.Count());

                #region Obtener acuse
               // var utilerias = new Utilerias(user, pass, false);

               //var response = utilerias.consultaAcuseRecepcion('A790FD6F-E9F7-46C3-961D-082197D2C05B');

                #endregion

                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
            }

        }
        public static async Task cancelar(string pathcer, string pathkey, string claveprivada, Timbrado timbrado, dynamic timbradoXML)
        {
            var uuids = new string[] { string.Format("{0}|{1}|{2}", timbradoXML.uuid + 1, "02", "") };
            var cancelado = await timbrado.CancelarCFDIAsync(pathcer, pathkey, claveprivada, uuids);
            Console.WriteLine(cancelado.@return.resultados[0].mensaje);


        }
        public static async Task get(string api, string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{api}:X"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Status] {(int)response.StatusCode}");
                    Console.WriteLine($"[Content] {content}");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

