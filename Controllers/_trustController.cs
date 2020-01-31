using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace urlshorten.Controllers
{
    public class _trustController : Controller
    {
        public IActionResult Index(string wa = "", string wresult = "")
        {

            if (wresult != "" || wa != "")
            {
                //Current cert is save somewhere else ;)
                if (ValidateToken(wresult, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", out String user))
                {
                    var email = user;

                    ViewBag.email = email;

                    return View();
                }

            }

            return Redirect("https://velosimo.acbocc.us");

        }

        public class SamlSignedXml : SignedXml
        {
            public SamlSignedXml(XmlElement e) : base(e) { }

            public override XmlElement GetIdElement(XmlDocument document, string idValue)
            {
                XmlNamespaceManager mgr = new XmlNamespaceManager(document.NameTable);
                mgr.AddNamespace("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");
                mgr.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");

                XmlElement assertionNode =
                       (XmlElement)document.SelectSingleNode("//t:RequestSecurityTokenResponse/" +
                                                             "t:RequestedSecurityToken/saml:Assertion", mgr);

                if (assertionNode.Attributes["AssertionID"] != null &&
                    string.Equals(assertionNode.Attributes["AssertionID"].Value, idValue, StringComparison.InvariantCultureIgnoreCase)
                    )
                    return assertionNode;

                return null;
            }
        }

        // token is the string representation of the SAML1 token
        // expectedCertThumb is the expected certificate's thumbprint
        protected bool ValidateToken(string token, string expectedCertThumb, out string userName)
        {
            userName = string.Empty;

            if (string.IsNullOrEmpty(token)) return false;

            var xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.LoadXml(token);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xd.NameTable);
            mgr.AddNamespace("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");
            mgr.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            mgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");

            // assertion
            XmlElement assertionNode = (XmlElement)xd.SelectSingleNode("//t:RequestSecurityTokenResponse/t:RequestedSecurityToken/saml:Assertion", mgr);

            // signature
            XmlElement signatureNode = (XmlElement)xd.GetElementsByTagName("ds:Signature")[0];

            var signedXml = new SamlSignedXml(assertionNode);
            signedXml.LoadXml(signatureNode);

            X509Certificate2 certificate = null;
            foreach (KeyInfoClause clause in signedXml.KeyInfo)
            {
                if (clause is KeyInfoX509Data)
                {
                    if (((KeyInfoX509Data)clause).Certificates.Count > 0)
                    {
                        certificate =
                        (X509Certificate2)((KeyInfoX509Data)clause).Certificates[0];

                    }
                }
            }

            // cert node missing
            if (certificate == null) return false;

            // check the signature and return the result.
            var signatureValidationResult = signedXml.CheckSignature(certificate, true);

            if (signatureValidationResult == false) return false;

            // validate cert thumb
            if (!string.IsNullOrEmpty(expectedCertThumb))
            {
                if (!string.Equals(expectedCertThumb, certificate.Thumbprint))
                    return false;
            }

            // retrieve username

            // expires = 
            var expNode = xd.SelectSingleNode("//t:RequestSecurityTokenResponse/t:Lifetime/wsu:Expires", mgr);

            if (!DateTime.TryParse(expNode.InnerText, out DateTime expireDate)) return false; // wrong date

            if (DateTime.Now > expireDate) return false; // token too old

            // claims
            var claimNodes =
              xd.SelectNodes("//t:RequestSecurityTokenResponse/t:RequestedSecurityToken/" +
                             "saml:Assertion/saml:AttributeStatement/saml:Attribute", mgr);
            foreach (XmlNode claimNode in claimNodes)
            {
                if (claimNode.Attributes["AttributeName"] != null &&
                            claimNode.Attributes["AttributeNamespace"] != null &&
                     string.Equals(claimNode.Attributes["AttributeName"].Value, "emailaddress", StringComparison.InvariantCultureIgnoreCase) &&
                                   string.Equals(claimNode.Attributes["AttributeNamespace"].Value, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims", StringComparison.InvariantCultureIgnoreCase) &&
                       claimNode.ChildNodes.Count == 1
                    )
                {
                    userName = claimNode.ChildNodes[0].InnerText;
                    return true;
                }
            }

            return false;
        }

    }
}