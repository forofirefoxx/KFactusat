using KFactuSat.com.sf.ws.Timbrado;
using KFactuSat.com.sf.ws.Utilerias;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

public static class WcfConfigFactory
{
    private const int MAX_SIZE = 10485760; // 10 MB

    private static BasicHttpBinding CrearBinding()
    {
        var binding = new BasicHttpBinding();

        binding.MaxBufferSize = MAX_SIZE;
        binding.MaxReceivedMessageSize = MAX_SIZE;

        binding.ReaderQuotas = new XmlDictionaryReaderQuotas
        {
            MaxDepth = 64,
            MaxStringContentLength = MAX_SIZE,
            MaxArrayLength = MAX_SIZE,
            MaxBytesPerRead = 4096,
            MaxNameTableCharCount = MAX_SIZE
        };

        binding.Security.Mode = BasicHttpSecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

        return binding;
    }
    // 🔹 TIMBRADO
    public static TimbradoPortTypeClient CrearTimbradoTesting()
    {
        var binding = CrearBinding();
        var endpoint = new EndpointAddress("https://testing.solucionfactible.com/ws/services/Timbrado");

        return new TimbradoPortTypeClient(binding, endpoint);
    }

    public static TimbradoPortTypeClient CrearTimbradoProduccion()
    {
        var binding = CrearBinding();
        var endpoint = new EndpointAddress("https://solucionfactible.com/ws/services/Timbrado");

        return new TimbradoPortTypeClient(binding, endpoint);
    }

    // 🔹 UTILERIAS
    public static UtileriasPortTypeClient CrearUtileriasTesting()
    {
        var binding = CrearBinding();
        var endpoint = new EndpointAddress("https://testing.solucionfactible.com/ws/services/Utilerias");

        return new UtileriasPortTypeClient(binding, endpoint);
    }

    public static UtileriasPortTypeClient CrearUtileriasProduccion()
    {
        var binding = CrearBinding();
        var endpoint = new EndpointAddress("https://solucionfactible.com/ws/services/Utilerias");

        return new UtileriasPortTypeClient(binding, endpoint);
    }
}