public class WebRequestCert : UnityEngine.Networking.CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //return base.ValidateCertificate(certificateData);
        return true;
    }
}