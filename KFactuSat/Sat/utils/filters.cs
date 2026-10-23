using System;

namespace KFactuSat.Sat.utils
{
    public class filters
    {
        public static bool isValidSealed(string pathcer, string pathkey, string pathxslt, string claveprivada, out string errorMesage)
        {
            string errcer, errkey, errxslt, errprivada;
            errorMesage = "";
            isValidCer(pathcer, out errcer);
            isValidKey(pathkey, out errkey);
            isValidxslt(pathxslt, out errxslt);
            isValidPrivateKey(claveprivada, out errprivada);

            errorMesage = String.Format("{0}{1}{2}{3}", errcer, errkey, errxslt, errprivada).TrimEnd(',');

            if (String.IsNullOrEmpty(errorMesage)) return true;

            return false;
        }
        public static bool isValidCer(string pathcer, out string errorMesage)
        {
            errorMesage = "";
            if (String.IsNullOrEmpty(pathcer)) errorMesage += "certificado,";
            if (String.IsNullOrEmpty(errorMesage))
                return true;


            return false;
        }
        public static bool isValidKey(string pathkey, out string errorMesage)
        {
            errorMesage = "";
            if (String.IsNullOrEmpty(pathkey)) errorMesage += "llave,";
            if (String.IsNullOrEmpty(errorMesage)) return true;

            return false;
        }
        public static bool isValidxslt(string pathxslt, out string errorMesage)
        {
            errorMesage = "";
            if (String.IsNullOrEmpty(pathxslt)) errorMesage += "xslt,";
            if (String.IsNullOrEmpty(errorMesage)) return true;

            return false;
        }
        public static bool isValidPrivateKey(string claveprivada, out string errorMesage)
        {
            errorMesage = "";
            if (String.IsNullOrEmpty(claveprivada)) errorMesage += "clave privada,";
            if (String.IsNullOrEmpty(errorMesage)) return true;

            return false;
        }
        
    }
}
