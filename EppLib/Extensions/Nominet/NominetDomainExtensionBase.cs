using EppLib.Entities;

namespace EppLib.Extensions.Nominet
{
	public abstract class NominetDomainExtensionBase : EppExtension
	{
		private string _ns = "http://www.nominet.org.uk/epp/xml/domain-nom-ext-1.2";
		
        protected override string Namespace
		{
			get { return _ns; }
			set { _ns = value; }
		}
	}
}