using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EppLib.Entities;
using EppLib.Extensions.Nominet;
using EppLib.Extensions.Nominet.ContactInfo;
using EppLib.Extensions.Nominet.ContactUpdate;
using EppLib.Extensions.Nominet.DomainCheck;
using EppLib.Extensions.Nominet.DomainCreate;
using EppLib.Extensions.Nominet.DomainInfo;
using EppLib.Extensions.Nominet.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EppLib.Tests
{
    [TestClass]
    public class NominetExtensionLocalTest
    {
        private TestContext _context;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _context; }
            set { _context = value; }
        }

        public NominetExtensionLocalTest(){}

        #region Contact Info

        /// <summary>
        /// Nominet Contact Info response
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#info
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetContactInfoResponse1.xml")]
        public void TestNominetContactInfoResponse1()
        {
            byte[] input = File.ReadAllBytes("NominetContactInfoResponse1.xml");
            var response = new NominetContactInfoResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("sh8013", response.Contact.Id);
            Assert.AreEqual("SH8013-REP", response.Contact.Roid);
            Assert.AreEqual("John Doe", response.Contact.PostalInfo.m_name);
            Assert.AreEqual("Example Inc.", response.Contact.PostalInfo.m_org);
            Assert.AreEqual("123 Example Dr.", response.Contact.PostalInfo.m_address.Street1);
            Assert.AreEqual("Suite 100", response.Contact.PostalInfo.m_address.Street2);
            Assert.AreEqual("Dulles", response.Contact.PostalInfo.m_address.City);
            Assert.AreEqual("VA", response.Contact.PostalInfo.m_address.StateProvince);
            Assert.AreEqual("20166-6503", response.Contact.PostalInfo.m_address.PostalCode);
            Assert.AreEqual("US", response.Contact.PostalInfo.m_address.CountryCode);
            Assert.AreEqual("1234", response.Contact.Voice.Extension);
            Assert.AreEqual("+1.7035555555", response.Contact.Voice.Value);
            Assert.AreEqual("jdoe@example.com", response.Contact.Email);

            Assert.AreEqual("invalid", response.DataQuality.Status);
            Assert.AreEqual("Incorrect Address", response.DataQuality.Reason);
            Assert.IsTrue(response.DataQuality.DateCommenced.HasValue);
            Assert.AreEqual(new DateTime(2015,5,7,13,20,4).ToString(), response.DataQuality.DateCommenced.Value.ToString());
            Assert.IsTrue(response.DataQuality.DateToSuspend.HasValue);
            Assert.AreEqual(new DateTime(2015, 6, 6, 13, 20, 4).ToString(), response.DataQuality.DateToSuspend.Value.ToString());
            Assert.IsTrue(response.DataQuality.LockApplied.HasValue);
            Assert.IsTrue(response.DataQuality.LockApplied.Value);
            Assert.IsNotNull(response.DataQuality.DomainList);
            Assert.AreEqual(2, response.DataQuality.DomainList.Count);
            Assert.AreEqual("epp-example1.co.uk", response.DataQuality.DomainList.First());
            Assert.AreEqual("epp-example2.co.uk", response.DataQuality.DomainList.Last());

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region Contact Update

        /// <summary>
        /// Nominet Contact Update command
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#update
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetContactUpdateCommand1.xml")]
        public void TestNominetContactUpdateCommand1()
        {
            string expected = File.ReadAllText("NominetContactUpdateCommand1.xml");

            var command = new NominetContactUpdate("my_contact");
            command.ContactChange = new ContactChange();
            command.ContactChange.Email = "example@email.co.uk";
            command.ContactChange.PostalInfo = new PostalInfo
            {
                m_type = PostalAddressType.LOC,
                m_name = "Changed main contact name",
                m_address = new PostalAddress
                {
                    Street1 = "10 Modified Street",
                    City = "Oxford",
                    StateProvince = "Oxon",
                    PostalCode = "OX5 5ZZ",
                    CountryCode = "GB"
                }
            };
            command.CompanyNumber = "NI65786";
            command.OptOut = YesNoFlag.N;
            command.TradeName = "Example trading name";
            command.Type = CoType.LTD;
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Nominet Contact Update response
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#update
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/ContactUpdateResponse1.xml")]
        public void TestNominetContactUpdateResponse1()
        {
            byte[] input = File.ReadAllBytes("ContactUpdateResponse1.xml");
            var response = new ContactUpdateResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region Domain Check

        /// <summary>
        /// Nominet Domain check command
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#check
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetDomainCheckCommand1.xml")]
        public void TestNominetDomainCheckCommand1()
        {
            string expected = File.ReadAllText("NominetDomainCheckCommand1.xml");

            var command = new NominetDomainCheck("example1.uk");
            command.Email = "john.smith@example.uk";
            command.PostalInfo = new PostalInfo
            {
                m_type = "loc",
                m_name = "Contact name",
                m_org = "Org name",
                m_address = new PostalAddress
                {
                    Street1 = "222 Test Street",
                    City = "Test City",
                    StateProvince = "Testshire",
                    PostalCode = "TE57 1NG",
                    CountryCode = "GB"
                }
            };
            command.TransactionId = "ABC-12345";
            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        #endregion

        #region Domain Create

        /// <summary>
        /// Nominet Domain create command
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#create
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetDomainCreateCommand1.xml")]
        public void TestNominetDomainCreateCommand1()
        {
            string expected = File.ReadAllText("NominetDomainCreateCommand1.xml");

            var command = new NominetDomainCreate("example.co.uk", "registrant")
            {
                AutoBill = "30"
            };
            command.NameServers.Add("ns1.example.net");
            command.NameServers.Add("ns2.example.net");
            command.NameServers.Add("ns3.example.net");
            command.Period = new DomainPeriod(1, "y");
            command.Password = "password";
            command.TransactionId = "ABC-12345";

            Assert.AreEqual(expected, command.ToXml().InnerXml);
        }

        /// <summary>
        /// Nominet Domain check command
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#create
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetDomainCreateResponse1.xml")]
        public void TestNominetDomainCreateResponse1()
        {
            byte[] input = File.ReadAllBytes("NominetDomainCreateResponse1.xml");
            var response = new DomainCreateResponse(input);

            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

            Assert.AreEqual("example.co.uk", response.DomainCreateResult.DomainName);
            Assert.AreEqual("2015-11-12T09:31:06", response.DomainCreateResult.CreatedDate);
            Assert.AreEqual("2016-11-12T09:31:06", response.DomainCreateResult.ExpirationDate);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54321-XYZ", response.ServerTransactionId);
        }

        #endregion

        #region Domain Info

        /// <summary>
        /// Nominet Domain check command
        /// example http://registrars.nominet.org.uk/namespace/uk/registration-and-domain-management/epp-commands#check
        /// </summary>
        [TestMethod]
        [TestCategory("NominetExtension")]
        [TestCategory("LocalCommand")]
        [DeploymentItem("TestData/NominetDomainInfoCommand1.xml")]
        public void TestNominetDomainInfoCommand1()
        {
            byte[] input = File.ReadAllBytes("NominetDomainInfoCommand1.xml");
            var response = new NominetDomainInfoResponse(input);

            Assert.AreEqual("30", response.Domain.AutoBill);
            Assert.AreEqual("1000", response.Code);
            Assert.AreEqual("Command completed successfully", response.Message);

        }

        #endregion

        #region Domain Name Cancellation Notification

        /// <summary>
        /// Poll response, Domain Name Cancellation notification messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainNameCancelledNotification.xml")]
        public void TestPollMsgsResponse2()
        {
            byte[] input = File.ReadAllBytes("DomainNameCancelledNotification.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1301", response.Code);
            Assert.AreEqual("Command completed successfully; ack to dequeue", response.Message);

            Assert.AreEqual("Domain Name Cancellation Notification", response.Body);

            Assert.AreEqual("ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("54322-XYZ", response.ServerTransactionId);

            var notification = new DomainCancellationNotification(File.ReadAllText("DomainNameCancelledNotification.xml"));

            Assert.AreEqual("cancelleddomain.co.uk", notification.DomainName);
            Assert.AreEqual("originator@nominet.org.uk", notification.Originator);
        }

        #endregion

        #region Abuse Notification

        /// <summary>
        /// Poll response, Domain Name Cancellation notification messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/AbuseNotification.xml")]
        public void TestAbuseNotification()
        {
            byte[] input = File.ReadAllBytes("AbuseNotification.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1301", response.Code);
            Assert.AreEqual("Command completed successfully; ack to dequeue", response.Message);

            Assert.AreEqual("Domain Activity Notification", response.Body);

            Assert.AreEqual("EPP-12345C-8BF-4CAA-A944-2F0EFCD37D9E", response.ClientTransactionId);
            Assert.AreEqual("1254234", response.ServerTransactionId);

            var notification = new AbuseNotification(File.ReadAllText("AbuseNotification.xml"));

            Assert.AreEqual("phished.co.uk", notification.DomainName);
            Assert.AreEqual("phished.co.uk", notification.Key);
            Assert.AreEqual("phishing", notification.Activity);
            Assert.AreEqual("Netcraft", notification.Source);
            Assert.AreEqual("www.youve.been.phished.co.uk", notification.HostName);
            Assert.AreEqual("http://www.youve.been.phished.co.uk/give/us/your/money.htm", notification.Url);
            Assert.IsNotNull(notification.Date);
            Assert.AreEqual(new DateTime(2011, 3, 1, 11, 44, 1), notification.Date.Value);
            Assert.AreEqual("213.135.134.24", notification.Ip);
            Assert.AreEqual("ns0.crooked.dealings.net", notification.Nameserver);
            Assert.AreEqual("hostmaster@crooked.dealings.net", notification.DnsAdmin);
            Assert.AreEqual("paypal", notification.Target);
            Assert.IsNotNull(notification.WholeDomain);
            Assert.AreEqual(YesNoFlag.Y, notification.WholeDomain);
        }

        #endregion

        #region Domains Suspended Notification

        /// <summary>
        /// Poll response, Domain Name Cancellation notification messages
        /// </summary>
        [TestMethod]
        [TestCategory("LocalResponse")]
        [DeploymentItem("TestData/DomainsSuspendedNotification.xml")]
        public void TestDomainsSuspendedNotification()
        {
            byte[] input = File.ReadAllBytes("DomainsSuspendedNotification.xml");
            var response = new PollResponse(input);

            Assert.AreEqual("1301", response.Code);
            Assert.AreEqual("Command completed successfully; ack to dequeue", response.Message);

            Assert.AreEqual("Domains Suspended Notification", response.Body);

            Assert.AreEqual("EPP-ABC-12345", response.ClientTransactionId);
            Assert.AreEqual("203355", response.ServerTransactionId);

            var notification = new DomainsSuspendedNotification(File.ReadAllText("DomainsSuspendedNotification.xml"));

            Assert.AreEqual("Data Quality", notification.SuspendedReason);
            Assert.IsNotNull(notification.CancelDate);
            Assert.AreEqual(new DateTime(2009, 12, 12, 0, 0, 13), notification.CancelDate.Value);
            Assert.IsNotNull(notification.SuspendedDomains);
            Assert.AreEqual(2, notification.SuspendedDomains.Count);
            Assert.AreEqual("epp-example1.co.uk", notification.SuspendedDomains.First());
            Assert.AreEqual("epp-example2.co.uk", notification.SuspendedDomains.Last());
        }

        #endregion
    }
}
