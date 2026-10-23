using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace KFactuSat.Sat.utils
{
    public class SelloDigital
    {
        public string Sellar(string CadenaOriginal, string ArchivoClavePrivada, string lPassword, string algoritmo)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            //byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA256Managed sham = new SHA256Managed();
            // SHA1Managed sham = new SHA1Managed(); version 3.2
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);



            try
            {
                Org.BouncyCastle.Crypto.AsymmetricKeyParameter lrsa = Org.BouncyCastle.Security.PrivateKeyFactory.DecryptKey(lPassword.ToCharArray(), ClavePrivada);
                Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters key = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)lrsa;
                Org.BouncyCastle.Crypto.ISigner sig = Org.BouncyCastle.Security.SignerUtilities.GetSigner(algoritmo);
                sig.Init(true, key);
                byte[] bytes = Encoding.UTF8.GetBytes(CadenaOriginal);
                sig.BlockUpdate(bytes, 0, bytes.Length);
                bytesFirmados = sig.GenerateSignature();

            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Clave privada incorrecta, revisa que la clave que escribes corresponde a los sellos digitales cargados");
            }
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }
        /// <summary>
        /// metodo que realiza el sello reciviendo el archivo key como matriaz de bytes
        /// </summary>
        /// <param name="CadenaOriginal"></param>
        /// <param name="ArchivoClavePrivada"></param>
        /// <param name="lPassword"></param>
        /// <returns></returns>
        public string Sellar(string CadenaOriginal, byte[] ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = ArchivoClavePrivada;
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA256Managed sham = new SHA256Managed();
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Clave privada incorrecta.");
            }
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }

        public bool verificarSello(string CadenaOriginal, string ArchivoClavePrivada, string lPassword, string ArchivoClavePublica)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;

            SecureString lSecStr = new SecureString();
            SHA1Managed sham = new SHA1Managed();
            lSecStr.Clear();

            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Clave privada incorrecta.");
            }

            string sellodigital = Convert.ToBase64String(bytesFirmados);

            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            //rsaCSP.ImportParameters(rsaParams);
            //rsaCSP = JavaScience.opensslkey.(File.ReadAllBytes(ArchivoClavePublica));
            bool dataOK = rsaCSP.VerifyData(Encoding.UTF8.GetBytes(CadenaOriginal), CryptoConfig.MapNameToOID("SHA1"), bytesFirmados);
            hashedData = hash.ComputeHash(bytesFirmados);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), Encoding.UTF8.GetBytes(CadenaOriginal));
        }//*/

        public string SellarMD5(string CadenaOriginal, string ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;
            SecureString lSecStr = new SecureString();
            lSecStr.Clear();
            foreach (char c in lPassword.ToCharArray())
                lSecStr.AppendChar(c);
            RSACryptoServiceProvider lrsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            hasher.ComputeHash(bCadenaOriginal);
            bytesFirmados = lrsa.SignData(bCadenaOriginal, hasher);
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }
        public string Certificado(string ArchivoCER)
        {
            byte[] Certificado = File.ReadAllBytes(ArchivoCER);
            return Base64_Encode(Certificado);
        }
        public string Certificado(byte[] ArchivoCER)
        {
            return Base64_Encode(ArchivoCER);
        }
        string Base64_Encode(byte[] str)
        {
            return Convert.ToBase64String(str);
        }
        byte[] Base64_Decode(string str)
        {
            try
            {
                byte[] decbuff = Convert.FromBase64String(str);
                return decbuff;
            }
            catch
            {
                { return null; }
            }
        }
        public static string getCadenaOriginal(string NombreXML)
        {
            System.Xml.Xsl.XslCompiledTransform transformer = new System.Xml.Xsl.XslCompiledTransform();
            //Encoding utf8 = Encoding.UTF8;
            //byte[] encodedBytes;
            StringWriter strwriter = new StringWriter();
            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    //encodedBytes = utf8.GetBytes(strwriter.ToString());
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    //encodedBytes = utf8.GetBytes(strwriter.ToString());
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw e;
                }
            }
            else return "Error al cargar el validador.";


        }
        public static string md5(string Value)
        {


            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.UTF8.GetBytes(Value);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            string ret = "";
            for (int i = 0; i < encodedBytes.Length; i++)
                ret += encodedBytes[i].ToString("x2").ToLower();
            return ret;

        }
        public static bool leerCER(string NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero)
        {


            if (NombreArchivo.Length < 1)
            {
                Inicio = "";
                Final = "";
                Serie = "INVALIDO";
                Numero = "";
                return false;
            }
            X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
            StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

            //Detalle
            objSB.AppendLine("Persona = " + objCert.Subject);
            objSB.AppendLine("Emisor = " + objCert.Issuer);
            objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
            Inicio = objCert.NotBefore.ToString();
            objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
            Final = objCert.NotAfter.ToString();
            objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
            objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
            Serie = objCert.SerialNumber.ToString();

            objSB.AppendLine("Hash = " + objCert.Thumbprint);
            //Numero = "?";
            string tNumero = "", rNumero = "", tNumero2 = "";

            int X;
            if (Serie.Length < 2)
                Numero = "";
            else
            {
                foreach (char c in Serie)
                {
                    switch (c)
                    {
                        case '0': tNumero += c; break;
                        case '1': tNumero += c; break;
                        case '2': tNumero += c; break;
                        case '3': tNumero += c; break;
                        case '4': tNumero += c; break;
                        case '5': tNumero += c; break;
                        case '6': tNumero += c; break;
                        case '7': tNumero += c; break;
                        case '8': tNumero += c; break;
                        case '9': tNumero += c; break;
                    }
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
                    X += 1;
                    //rNumero = rNumero + 
                    tNumero2 = tNumero.Substring(0, X);
                    rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
                }
                Numero = rNumero;

            }

            if (DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore)
            {
                return true;
            }



            return false;
        }

        /// <summary>
        /// lee el codigo del certificado enviado este como matriz de bytes
        /// </summary>
        /// <param name="NombreArchivo">certificado en matriz de bytes</param>
        /// <param name="Inicio"></param>
        /// <param name="Final"></param>
        /// <param name="Serie"></param>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public static bool leerCER(byte[] NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero)
        {

            if (NombreArchivo.Length < 1)
            {
                Inicio = "";
                Final = "";
                Serie = "INVALIDO";
                Numero = "";
                return false;
            }
            X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
            StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

            //Detalle
            objSB.AppendLine("Persona = " + objCert.Subject);
            objSB.AppendLine("Emisor = " + objCert.Issuer);
            objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
            Inicio = objCert.NotBefore.ToString();
            objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
            Final = objCert.NotAfter.ToString();
            objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
            objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
            Serie = objCert.SerialNumber.ToString();

            objSB.AppendLine("Hash = " + objCert.Thumbprint);
            //Numero = "?";
            string tNumero = "", rNumero = "", tNumero2 = "";

            int X;
            if (Serie.Length < 2)
                Numero = "";
            else
            {
                foreach (char c in Serie)
                {
                    switch (c)
                    {
                        case '0': tNumero += c; break;
                        case '1': tNumero += c; break;
                        case '2': tNumero += c; break;
                        case '3': tNumero += c; break;
                        case '4': tNumero += c; break;
                        case '5': tNumero += c; break;
                        case '6': tNumero += c; break;
                        case '7': tNumero += c; break;
                        case '8': tNumero += c; break;
                        case '9': tNumero += c; break;
                    }
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
                    X += 1;
                    //rNumero = rNumero + 
                    tNumero2 = tNumero.Substring(0, X);
                    rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
                }
                Numero = rNumero;

            }

            if (DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore)
            {
                return true;
            }

            return false;
        }


        public static bool validarCERKEY(string NombreArchivoCER, string NombreArchivoKEY, string ClavePrivada)
        {
            X509Certificate2 certificado = new X509Certificate2(File.ReadAllBytes(NombreArchivoCER));
            //initialze the byte arrays to the public key information.
            byte[] pk = certificado.GetPublicKey();
            X509Certificate2 certPrivado = new X509Certificate2(File.ReadAllBytes(NombreArchivoKEY));
            return false;
        }

        private string CreateDigest(string sourceData)
        {
            byte[] data = GetBytes(sourceData);
            return System.Convert.ToBase64String(HashAlgorithm.Create("SHA1").ComputeHash(data));
        }
        private byte[] GetBytes(string sourceData)
        {
            return Encoding.Default.GetBytes(sourceData);
        }
        private string Sign(string sourceData, X509Certificate2 certificate)
        {
            byte[] data = GetBytes(sourceData);
            byte[] signature = null;

            using (RSA rsaCryptoServiceProvider = certificate.GetRSAPrivateKey())
            {
                signature = rsaCryptoServiceProvider.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            return System.Convert.ToBase64String(signature);
        }
        public string GenerarCancelacion(byte[] certificatebytes, string passpfx, string rfcEmisor, DateTime fecha, string uuid, string motivo, string folioSustitucion = null)
        {

            var certificate = ObtenerX509Certificado(certificatebytes, passpfx);
            // 📅 ISO 8601
            string fechaIso = fecha.ToString("yyyy-MM-ddTHH:mm:ss");

            // 1️⃣ Construir nodo <Folio> según motivo
            string folioXml;
            if (motivo == "01" && !string.IsNullOrEmpty(folioSustitucion))
            {
                folioXml = $"<Folio FolioSustitucion=\"{folioSustitucion}\" Motivo=\"{motivo}\" UUID=\"{uuid}\"/>";
            }
            else
            {
                folioXml = $"<Folio Motivo=\"{motivo}\" UUID=\"{uuid}\"/>";
            }

            // 2️⃣ XML base canónico
            string canonicalXml =
                $"<Cancelacion xmlns=\"http://cancelacfd.sat.gob.mx\" " +
                $"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                $"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                $"Fecha=\"{fechaIso}\" " +
                $"RfcEmisor=\"{rfcEmisor}\">" +
                    $"<Folios>{folioXml}</Folios>" +
                $"</Cancelacion>";

            // 3️⃣ Digest (c14n + SHA1)
            string digest = CreateDigest(canonicalXml);

            // 4️⃣ SignedInfo
            string signedInfo = @"<SignedInfo>" +
                                    @"<CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/>" +
                                    @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                    @"<Reference URI="""">" +
                                        @"<Transforms>" +
                                            @"<Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/>" +
                                        @"</Transforms>" +
                                        @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                        $"<DigestValue>{digest}</DigestValue>" +
                                    @"</Reference>" +
                                @"</SignedInfo>";

            // 5️⃣ Firmar SignedInfo
            string signatureValue = Sign(signedInfo, certificate);

            // 6️⃣ Signature XML
            string signatureXml = @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                      signedInfo +
                                      $"<SignatureValue>{signatureValue}</SignatureValue>" +
                                      @"<KeyInfo>" +
                                          @"<X509Data>" +
                                              $"<X509Certificate>{Convert.ToBase64String(certificate.RawData)}</X509Certificate>" +
                                          @"</X509Data>" +
                                      @"</KeyInfo>" +
                                  @"</Signature>";

            // 7️⃣ XML final
            string cancelacionXml =
                $"<Cancelacion xmlns=\"http://cancelacfd.sat.gob.mx\" " +
                $"xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                $"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                $"Fecha=\"{fechaIso}\" " +
                $"RfcEmisor=\"{rfcEmisor}\">" +
                    $"<Folios>{folioXml}</Folios>" +
                    signatureXml +
                $"</Cancelacion>";

            return cancelacionXml;
        }

        /// <summary>
        /// Genera el XML de cancelación de un CFDI con firma digital.
        /// </summary>
        /// <param name="certificateBytes">Bytes del archivo .pfx del CSD.</param>
        /// <param name="passPfx">Contraseña del archivo .pfx.</param>
        /// <param name="rfcEmisor">RFC del emisor del CFDI a cancelar.</param>
        /// <param name="fecha">Fecha y hora de la solicitud de cancelación.</param>
        /// <param name="uuid">UUID del CFDI a cancelar.</param>
        /// <param name="motivo">Motivo de la cancelación (ej. "01" para errores con relación).</param>
        /// <param name="folioSustitucion">UUID del CFDI que sustituye al cancelado (requerido si motivo es "01").</param>
        /// <returns>Cadena XML del documento de cancelación firmado.</returns>
        public string GenerateCancelacion(byte[] certificateBytes, string passPfx, string rfcEmisor, DateTime fecha, string uuid, string motivo, string folioSustitucion = null)
        {
            // 1. Cargar el certificado
            X509Certificate2 certificate = ObtenerX509Certificado(certificateBytes, passPfx);

            // 2. Crear el documento XML de cancelación
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true; // Importante para la canonicalización

            // Crear el elemento raíz <Cancelacion>
            XmlElement cancelacionElement = xmlDoc.CreateElement("Cancelacion", "http://cancelacfd.sat.gob.mx");
            cancelacionElement.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            cancelacionElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            cancelacionElement.SetAttribute("Fecha", fecha.ToString("yyyy-MM-ddTHH:mm:ss"));
            cancelacionElement.SetAttribute("RfcEmisor", rfcEmisor);
            xmlDoc.AppendChild(cancelacionElement);

            // Crear el elemento <Folios>
            XmlElement foliosElement = xmlDoc.CreateElement("Folios", "http://cancelacfd.sat.gob.mx");
            cancelacionElement.AppendChild(foliosElement);

            // Crear el elemento <Folio>
            XmlElement folioElement = xmlDoc.CreateElement("Folio", "http://cancelacfd.sat.gob.mx");
            folioElement.SetAttribute("Motivo", motivo);
            folioElement.SetAttribute("UUID", uuid);

            if (motivo == "01" && !string.IsNullOrEmpty(folioSustitucion))
            {
                folioElement.SetAttribute("FolioSustitucion", folioSustitucion);
            }
            foliosElement.AppendChild(folioElement);

            // 3. Crear el objeto SignedXml para la firma
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = certificate.GetRSAPrivateKey(); // Asignar la clave privada para firmar

            // 4. Crear la referencia al documento (el nodo Cancelacion)
            Reference reference = new Reference(""); // URI="" significa el documento completo (el elemento raíz)
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform()); // Transformada de firma envuelta
            reference.AddTransform(new XmlDsigC14NTransform()); // Transformada de canonicalización (importante)

            // Asignar el algoritmo de digest (SHA1)
            reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";

            signedXml.AddReference(reference);

            // 5. Asignar el algoritmo de firma (RSA-SHA1)
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

            // 6. Añadir la información de la clave (certificado X509)
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));
            signedXml.KeyInfo = keyInfo;

            // 7. Calcular la firma
            signedXml.ComputeSignature();

            // 8. Obtener el elemento XML de la firma y añadirlo al documento de cancelación
            XmlElement xmlSignature = signedXml.GetXml();
            cancelacionElement.AppendChild(xmlSignature);

            // Devolver el XML completo como cadena
            return xmlDoc.OuterXml;
        }

        private static X509Certificate2 ObtenerX509Certificado(byte[] pfx, string passpfx)
        {
            return new X509Certificate2(pfx, passpfx, X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
        }
    }
}
