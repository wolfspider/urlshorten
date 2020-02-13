using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;


namespace urlshorten.Controllers
{
    
    public class _trustController : Controller
    {
        
        /*This is an interesting situation to say the least- 
        MacOS is wanting to have a relying trust while windows does not
        need one at all. So for this I'm thinking that we use a controller here
        for Darwin and *nix but use DI for Windows and just need a way of getting it
        to work for both...*/

        private readonly IConfiguration _configure;

        public _trustController(IConfiguration configure)
        {
            _configure = configure;
        }
        
        public IActionResult Index(string wa = "", string wresult = "")
        {

            if (wresult != "" || wa != "") {
                if (ValidateToken(wresult, _configure["ADFSThumbprint:x509Cert"]))
                    return RedirectToPage("/Home");
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
        protected bool ValidateToken(string token, string expectedCertThumb)
        {
            string userName = string.Empty;

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

            var claims = new List<Claim>();
            
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
                    var groups = claimNodes[1].ChildNodes;

                    StringBuilder adGroup = new StringBuilder();

                    foreach(XmlNode group in groups)
                    {
                        adGroup.Append(group.InnerText+",");
                    }

                    var adGroups = adGroup.ToString().TrimEnd(',');
                    
                    claims.Add(new Claim("Name", userName));
                    claims.Add(new Claim("Group", adGroups));

                    var appIdentity = new ClaimsIdentity(claims, "Basic", "Name", "Group");
                    
                    var currentIdentities = HttpContext.User.Identities as List<ClaimsIdentity>;

                    var currentClaim = currentIdentities?.FirstOrDefault();

                    //For some reason given identity without a claim in it
                    if (currentClaim != null)
                        currentIdentities.Remove(currentClaim);

                    HttpContext.User.AddIdentity(appIdentity);

                    var tokenHandler = new JwtSecurityTokenHandler();

                    //This is just a random GUID
                    
                    var signingKey = "713C7335-3663-40F7-B086-17B813230D92";

                    //certificate needs private key if used
                    //var key = new X509SecurityKey(certificate);

                    var simpleKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = appIdentity,
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(simpleKey, SecurityAlgorithms.HmacSha256)
                    };

                    var jwttoken = tokenHandler.CreateToken(tokenDescriptor);

                    ClaimsPrincipal cp = new ClaimsPrincipal(appIdentity);
                    AuthenticationProperties authprops = new AuthenticationProperties();

                    authprops.StoreTokens(new[]
                    {
                        new AuthenticationToken()
                        {
                            Name = "JWT",
                            Value = jwttoken.ToString()
                        }
                    });

                    HttpContext.SignInAsync(cp, authprops);
                    
                    return true;
                }
            }

            return false;
        }

    }
}